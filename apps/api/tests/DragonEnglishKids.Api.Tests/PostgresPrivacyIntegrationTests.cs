using System.Net;
using DragonEnglishKids.Api.Data;
using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy;
using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Privacy.Consent;
using DragonEnglishKids.Api.Privacy.Requests;
using DragonEnglishKids.Api.Retention;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

// Runs the QA-002 quickstart scenarios against a real PostgreSQL (set ConnectionStrings__Default).
[TestClass]
public sealed class PostgresPrivacyIntegrationTests
{
    private const string AuditKey = "test-only-audit-key";

    private readonly Guid _parentId = Guid.NewGuid();
    private readonly Guid _childId = Guid.NewGuid();

    private static string RequireConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Assert.Inconclusive("Set ConnectionStrings__Default to run the PostgreSQL privacy tests.");
        }

        return connectionString!;
    }

    private static ApplicationDbContext OpenDatabase(string connectionString) =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(connectionString).Options);

    private PrivacyTestApp CreateApp(bool speechEnabled = true)
    {
        var app = new PrivacyTestApp(speechEnabled, RequireConnectionString());
        app.Authorizer.Grant(_parentId, _childId);
        return app;
    }

    private static string Subject(Guid childId) => new SubjectPseudonymizer(AuditKey).Pseudonymize(childId);

    [TestMethod]
    public async Task GrantStartSpeechAndWithdrawArePersistedAndRevokeTheSession()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "g1");
        using var started = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        Assert.AreEqual(HttpStatusCode.OK, granted.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, started.StatusCode);
        await using (var database = OpenDatabase(RequireConnectionString()))
        {
            var consent = await database.ConsentRecords.SingleAsync(x => x.ChildId == _childId);
            Assert.AreEqual(Decisions.Granted, consent.Decision);
            Assert.AreEqual(PrivacyTestHelpers.CurrentNoticeVersion, consent.NoticeVersion);
            Assert.IsNull((await database.SpeechSessions.SingleAsync(x => x.ChildId == _childId)).RevokedAt);
        }

        app.Time.Advance(TimeSpan.FromSeconds(1));
        using var withdrawn = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Withdrawn, idempotencyKey: "w1");

        Assert.AreEqual(HttpStatusCode.OK, withdrawn.StatusCode);
        await using var after = OpenDatabase(RequireConnectionString());
        Assert.IsNotNull((await after.SpeechSessions.SingleAsync(x => x.ChildId == _childId)).RevokedAt);
        var actions = await after.AuditEvents
            .Where(x => x.SubjectRef == Subject(_childId))
            .Select(x => x.Action)
            .ToListAsync();
        CollectionAssert.AreEquivalent(new[] { AuditActions.ConsentGranted, AuditActions.ConsentWithdrawn }, actions);

        using var again = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);
        Assert.AreEqual(HttpStatusCode.Forbidden, again.StatusCode);
    }

    [TestMethod]
    public async Task DeniedOrMissingConsentRefusesSpeechAndStoresNoSession()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var none = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);
        using var denied = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Denied, idempotencyKey: "d1");
        using var afterDenial = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        Assert.AreEqual(ProblemTypes.ConsentRequired, await PrivacyTestHelpers.ProblemTypeAsync(none));
        Assert.AreEqual(ProblemTypes.ConsentRequired, await PrivacyTestHelpers.ProblemTypeAsync(afterDenial));
        await using var database = OpenDatabase(RequireConnectionString());
        Assert.AreEqual(0, await database.SpeechSessions.CountAsync(x => x.ChildId == _childId));
        Assert.AreEqual(Decisions.Denied, (await database.ConsentRecords.SingleAsync(x => x.ChildId == _childId)).Decision);
    }

    [TestMethod]
    public async Task NonOwnerAndGuestAreRefusedAndAuditedWithoutAnyStoredData()
    {
        using var app = CreateApp();
        using var stranger = app.CreateClient(Guid.NewGuid());
        using var guest = app.CreateClient(role: "guest");

        using var consent = await PrivacyTestHelpers.PutConsentAsync(stranger, _childId, Decisions.Granted, idempotencyKey: "s1");
        using var speech = await PrivacyTestHelpers.StartSpeechAsync(guest, _childId);

        Assert.AreEqual(HttpStatusCode.Forbidden, consent.StatusCode);
        Assert.AreEqual(ProblemTypes.GuestNotAllowed, await PrivacyTestHelpers.ProblemTypeAsync(speech));
        await using var database = OpenDatabase(RequireConnectionString());
        Assert.AreEqual(0, await database.ConsentRecords.CountAsync(x => x.ChildId == _childId));
        Assert.AreEqual(0, await database.SpeechSessions.CountAsync(x => x.ChildId == _childId));
        var denied = await database.AuditEvents.Where(x => x.SubjectRef == Subject(_childId)).ToListAsync();
        Assert.AreEqual(2, denied.Count);
        Assert.IsTrue(denied.All(x => x.Action == AuditActions.AccessDenied));
    }

    [TestMethod]
    public async Task AuditRowsInTheDatabaseNeverContainTheChildIdentifier()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "a1");
        using var history = await client.GetAsync($"/api/v1/children/{_childId}/consents");

        await using var database = OpenDatabase(RequireConnectionString());
        var rows = await database.AuditEvents.Where(x => x.SubjectRef == Subject(_childId)).ToListAsync();

        Assert.AreEqual(2, rows.Count);
        var text = string.Join(' ', rows.Select(x => $"{x.ActorRole} {x.Action} {x.SubjectRef} {x.Outcome} {x.CorrelationId}"));
        Assert.IsFalse(text.Contains(_childId.ToString(), StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(text.Contains(_childId.ToString("N"), StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public async Task SpeechLeaseIsRenewedAndThenRefusedAfterWithdrawal()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "l1");
        using var started = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);
        var sessionId = (await PrivacyTestHelpers.JsonAsync(started)).GetProperty("speechSessionId").GetGuid();
        app.Time.Advance(TimeSpan.FromSeconds(10));

        using var renewed = await PrivacyTestHelpers.HeartbeatAsync(client, _childId, sessionId);

        Assert.AreEqual(HttpStatusCode.OK, renewed.StatusCode);
        await using (var database = OpenDatabase(RequireConnectionString()))
        {
            Assert.AreEqual(
                app.Time.GetUtcNow().AddSeconds(15),
                (await database.SpeechSessions.SingleAsync(x => x.Id == sessionId)).ExpiresAt);
        }

        using var withdrawn = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Withdrawn, idempotencyKey: "l2");
        using var refused = await PrivacyTestHelpers.HeartbeatAsync(client, _childId, sessionId);

        Assert.AreEqual(ProblemTypes.LeaseRefused, await PrivacyTestHelpers.ProblemTypeAsync(refused));
    }

    [TestMethod]
    public async Task IdempotentReplayAndUniqueKeyAreEnforcedByTheDatabase()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var first = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "same");
        using var second = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "same");

        Assert.AreEqual(HttpStatusCode.OK, second.StatusCode);
        await using var database = OpenDatabase(RequireConnectionString());
        Assert.AreEqual(1, await database.ConsentRecords.CountAsync(x => x.ChildId == _childId));

        database.ConsentRecords.Add(new ConsentRecord
        {
            Id = Guid.NewGuid(),
            ParentId = _parentId,
            ChildId = _childId,
            Purpose = Purposes.MicrophoneSpeech,
            NoticeVersion = PrivacyTestHelpers.CurrentNoticeVersion,
            Decision = Decisions.Granted,
            DecidedAt = DateTimeOffset.UtcNow,
            CorrelationId = "dup",
            IdempotencyKey = "same"
        });
        await Assert.ThrowsExactlyAsync<DbUpdateException>(() => database.SaveChangesAsync());
    }

    [TestMethod]
    public async Task ConsentRecordsAreAppendOnlyInTheDatabase()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "ao1");
        await using var database = OpenDatabase(RequireConnectionString());
        var record = await database.ConsentRecords.SingleAsync(x => x.ChildId == _childId);

        record.Decision = Decisions.Denied;

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => database.SaveChangesAsync());
        database.ChangeTracker.Clear();
        Assert.AreEqual(Decisions.Granted, (await database.ConsentRecords.SingleAsync(x => x.ChildId == _childId)).Decision);
    }

    [TestMethod]
    public async Task PrivacyRequestsArePersistedWithTheirDueDate()
    {
        using var app = CreateApp();
        app.Verifier.Verified = true;
        using var client = app.CreateClient(_parentId);

        using var created = await PrivacyTestHelpers.CreatePrivacyRequestAsync(client, _childId, "deletion", "pr1");
        var requestId = (await PrivacyTestHelpers.JsonAsync(created)).GetProperty("requestId").GetGuid();
        using var status = await client.GetAsync($"/api/v1/privacy-requests/{requestId}");

        Assert.AreEqual(HttpStatusCode.Accepted, created.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, status.StatusCode);
        await using var database = OpenDatabase(RequireConnectionString());
        var stored = await database.PrivacyRequests.SingleAsync(x => x.Id == requestId);
        Assert.AreEqual(RequestStatuses.Accepted, stored.Status);
        Assert.AreEqual(app.Time.GetUtcNow().AddDays(30), stored.DueAt);
        Assert.IsNotNull(stored.VerifiedAt);
    }

    [TestMethod]
    public async Task RetentionRunsAreRecordedAndFailuresCanBeRetried()
    {
        var connectionString = RequireConnectionString();
        await using var database = OpenDatabase(connectionString);
        var store = new EfPrivacyStore(database);
        var time = new TestTimeProvider(new DateTimeOffset(2026, 10, 19, 8, 0, 0, TimeSpan.Zero));
        var guests = new FakeExpiredGuestStore();
        guests.Add(time.GetUtcNow().AddHours(-1));
        var runner = new RetentionRunner(
            guests,
            store,
            new AuditWriter(store, new SubjectPseudonymizer(AuditKey), time),
            time,
            NullLogger<RetentionRunner>.Instance);
        guests.BeforeDelete = _ => throw new InvalidOperationException("offline");

        var failed = await runner.RunOnceAsync(CancellationToken.None);
        guests.BeforeDelete = null;
        var succeeded = await runner.RunOnceAsync(CancellationToken.None);

        await using var verify = OpenDatabase(connectionString);
        var failedRow = await verify.RetentionRuns.SingleAsync(x => x.Id == failed!.Id);
        var succeededRow = await verify.RetentionRuns.SingleAsync(x => x.Id == succeeded!.Id);
        Assert.AreEqual(RetentionOutcomes.Failed, failedRow.Outcome);
        Assert.AreEqual("InvalidOperationException", failedRow.FailureCode);
        Assert.AreEqual(RetentionOutcomes.Succeeded, succeededRow.Outcome);
        Assert.AreEqual(1, succeededRow.ItemsDeleted);
        Assert.AreEqual(2, await verify.AuditEvents.CountAsync(x => x.Action == AuditActions.RetentionCleanup
            && (x.CorrelationId == failed!.Id.ToString() || x.CorrelationId == succeeded!.Id.ToString())));
    }

    [TestMethod]
    public async Task UnreachableDatabaseFailsClosedWithServiceUnavailable()
    {
        using var app = new PrivacyTestApp(
            speechEnabled: true,
            realConnectionString: "Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=2;Pooling=false");
        app.Authorizer.Grant(_parentId, _childId);
        using var client = app.CreateClient(_parentId);

        using var consent = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "u1");
        using var speech = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, consent.StatusCode);
        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, speech.StatusCode);
        Assert.AreEqual(ProblemTypes.PrivacyUnavailable, await PrivacyTestHelpers.ProblemTypeAsync(speech));
        Assert.IsFalse(string.Join(' ', app.Logs.Messages).Contains("Password=", StringComparison.OrdinalIgnoreCase));
    }
}
