using System.Net;
using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Privacy.Consent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class ConsentWithdrawalTests
{
    private readonly Guid _parentId = Guid.NewGuid();
    private readonly Guid _childId = Guid.NewGuid();

    [TestMethod]
    public async Task WithdrawalRevokesActiveSessionsAndIsAudited()
    {
        using var app = new PrivacyTestApp();
        app.Authorizer.Grant(_parentId, _childId);
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");
        using var started = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);
        Assert.AreEqual(HttpStatusCode.OK, started.StatusCode);

        app.Time.Advance(TimeSpan.FromSeconds(1));
        using var withdrawn = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Withdrawn, idempotencyKey: "k2");

        Assert.AreEqual(HttpStatusCode.OK, withdrawn.StatusCode);
        Assert.IsNotNull(app.Store.SpeechSessions.Single().RevokedAt);
        Assert.IsTrue(app.Store.AuditEvents.Any(x => x.Action == AuditActions.ConsentWithdrawn));

        using var again = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);
        Assert.AreEqual(HttpStatusCode.Forbidden, again.StatusCode);
    }

    [TestMethod]
    public void PersistedTypesHoldNoAudioOrTranscriptData()
    {
        var persisted = new[] { typeof(ConsentRecord), typeof(SpeechSession), typeof(AuditEvent) };

        var names = persisted.SelectMany(type => type.GetProperties().Select(property => property.Name)).ToList();

        Assert.IsFalse(names.Any(name =>
            name.Contains("audio", StringComparison.OrdinalIgnoreCase)
            || name.Contains("transcript", StringComparison.OrdinalIgnoreCase)));
    }
}
