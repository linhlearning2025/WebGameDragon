using System.Net;
using DragonEnglishKids.Api.Privacy;
using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Privacy.Consent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class ConsentEndpointTests
{
    private readonly Guid _parentId = Guid.NewGuid();
    private readonly Guid _childId = Guid.NewGuid();

    private PrivacyTestApp CreateApp()
    {
        var app = new PrivacyTestApp();
        app.Authorizer.Grant(_parentId, _childId);
        return app;
    }

    [TestMethod]
    public async Task OwnerGrantsConsentAndItIsRecordedWithNoticeVersionAndAudit()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);

        using var response = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await PrivacyTestHelpers.JsonAsync(response);
        Assert.AreEqual("granted", body.GetProperty("decision").GetString());
        Assert.IsTrue(body.GetProperty("valid").GetBoolean());
        Assert.AreEqual(1, app.Store.Consents.Count);
        Assert.AreEqual(PrivacyTestHelpers.CurrentNoticeVersion, app.Store.Consents[0].NoticeVersion);
        var audit = app.Store.AuditEvents.Single();
        Assert.AreEqual(AuditActions.ConsentGranted, audit.Action);
        Assert.IsFalse(audit.SubjectRef.Contains(_childId.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public async Task OwnerDeniesAndWithdrawsAreRecorded()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);

        using var denied = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Denied, idempotencyKey: "k1");
        app.Time.Advance(TimeSpan.FromSeconds(1));
        using var withdrawn = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Withdrawn, idempotencyKey: "k2");

        Assert.AreEqual(HttpStatusCode.OK, denied.StatusCode);
        Assert.AreEqual(HttpStatusCode.OK, withdrawn.StatusCode);
        CollectionAssert.AreEqual(
            new[] { AuditActions.ConsentDenied, AuditActions.ConsentWithdrawn },
            app.Store.AuditEvents.Select(x => x.Action).ToArray());
    }

    [TestMethod]
    public async Task ReplayWithSameIdempotencyKeyDoesNotCreateDuplicates()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);

        using var first = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "same");
        using var second = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "same");

        Assert.AreEqual(HttpStatusCode.OK, second.StatusCode);
        Assert.AreEqual(1, app.Store.Consents.Count);
        Assert.AreEqual(await first.Content.ReadAsStringAsync(), await second.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task NonOwnerIsDeniedWithoutDisclosureAndAuditedMinimally()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(Guid.NewGuid());

        using var response = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.AreEqual(ProblemTypes.Forbidden, await PrivacyTestHelpers.ProblemTypeAsync(response));
        Assert.AreEqual(0, app.Store.Consents.Count);
        Assert.AreEqual(AuditActions.AccessDenied, app.Store.AuditEvents.Single().Action);
    }

    [TestMethod]
    public async Task UnauthenticatedCallerIsDenied()
    {
        using var app = CreateApp();
        using var client = app.CreateClient();

        using var response = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.AreEqual(0, app.Store.Consents.Count);
    }

    [TestMethod]
    public async Task OutdatedNoticeVersionIsRejected()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);

        using var response = await PrivacyTestHelpers.PutConsentAsync(
            client,
            _childId,
            Decisions.Granted,
            noticeVersion: "old-version",
            idempotencyKey: "k1");

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        Assert.AreEqual(0, app.Store.Consents.Count);
    }

    [TestMethod]
    public async Task InvalidDecisionOrMissingIdempotencyKeyIsRejected()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);

        using var badDecision = await PrivacyTestHelpers.PutConsentAsync(client, _childId, "maybe", idempotencyKey: "k1");
        using var noKey = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted);

        Assert.AreEqual(HttpStatusCode.BadRequest, badDecision.StatusCode);
        Assert.AreEqual(HttpStatusCode.BadRequest, noKey.StatusCode);
    }

    [TestMethod]
    public async Task OwnerSeesHistoryAndTheReadIsAudited()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var grant = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");

        using var response = await client.GetAsync($"/api/v1/children/{_childId}/consents");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await PrivacyTestHelpers.JsonAsync(response);
        Assert.AreEqual(1, body.GetProperty("items").GetArrayLength());
        Assert.IsTrue(app.Store.AuditEvents.Any(x => x.Action == AuditActions.ConsentHistoryViewed));
    }

    [TestMethod]
    public async Task NonOwnerCannotReadHistory()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(Guid.NewGuid());

        using var response = await client.GetAsync($"/api/v1/children/{_childId}/consents");

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.IsFalse(app.Store.AuditEvents.Any(x => x.Action == AuditActions.ConsentHistoryViewed));
    }

    [TestMethod]
    public async Task ConsentForOneChildDoesNotApplyToASibling()
    {
        using var app = CreateApp();
        var sibling = Guid.NewGuid();
        app.Authorizer.Grant(_parentId, sibling);
        using var client = app.CreateClient(_parentId);
        using var grant = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");

        using var response = await PrivacyTestHelpers.StartSpeechAsync(client, sibling);

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.AreEqual(ProblemTypes.ConsentRequired, await PrivacyTestHelpers.ProblemTypeAsync(response));
    }
}
