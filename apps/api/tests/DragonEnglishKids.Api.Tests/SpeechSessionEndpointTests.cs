using System.Net;
using DragonEnglishKids.Api.Privacy;
using DragonEnglishKids.Api.Privacy.Consent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class SpeechSessionEndpointTests
{
    private readonly Guid _parentId = Guid.NewGuid();
    private readonly Guid _childId = Guid.NewGuid();

    private PrivacyTestApp CreateApp(bool speechEnabled = true)
    {
        var app = new PrivacyTestApp(speechEnabled);
        app.Authorizer.Grant(_parentId, _childId);
        return app;
    }

    private async Task AssertRefusedAsync(HttpResponseMessage response, HttpStatusCode status, string problemType)
    {
        Assert.AreEqual(status, response.StatusCode);
        Assert.AreEqual(problemType, await PrivacyTestHelpers.ProblemTypeAsync(response));
    }

    [TestMethod]
    public async Task SpeechIsRefusedWithoutAnyConsent()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);

        using var response = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        await AssertRefusedAsync(response, HttpStatusCode.Forbidden, ProblemTypes.ConsentRequired);
        Assert.AreEqual(0, app.Store.SpeechSessions.Count);
    }

    [TestMethod]
    public async Task SpeechIsRefusedWhenConsentWasDeniedOrWithdrawn()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var denied = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Denied, idempotencyKey: "k1");

        using var afterDenial = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        await AssertRefusedAsync(afterDenial, HttpStatusCode.Forbidden, ProblemTypes.ConsentRequired);

        app.Time.Advance(TimeSpan.FromSeconds(1));
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k2");
        app.Time.Advance(TimeSpan.FromSeconds(1));
        using var withdrawn = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Withdrawn, idempotencyKey: "k3");
        using var afterWithdrawal = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        await AssertRefusedAsync(afterWithdrawal, HttpStatusCode.Forbidden, ProblemTypes.ConsentRequired);
    }

    [TestMethod]
    public async Task SpeechIsGrantedAShortLeaseWhenConsentIsValid()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");

        using var response = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await PrivacyTestHelpers.JsonAsync(response);
        Assert.IsTrue(body.TryGetProperty("speechSessionId", out _));
        Assert.AreEqual(app.Time.GetUtcNow().AddSeconds(15), body.GetProperty("expiresAt").GetDateTimeOffset());
    }

    [TestMethod]
    public async Task SpeechStaysDisabledUntilAnApproachIsApproved()
    {
        using var app = CreateApp(speechEnabled: false);
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");

        using var response = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        await AssertRefusedAsync(response, HttpStatusCode.Forbidden, ProblemTypes.SpeechDisabled);
    }

    [TestMethod]
    public async Task GuestsNeverReceiveASpeechSession()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(role: "guest");

        using var response = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        await AssertRefusedAsync(response, HttpStatusCode.Forbidden, ProblemTypes.GuestNotAllowed);
        Assert.AreEqual(0, app.Store.SpeechSessions.Count);
    }

    [TestMethod]
    public async Task UnauthenticatedAndNonOwnerCallersAreForbidden()
    {
        using var app = CreateApp();
        using var anonymous = app.CreateClient();
        using var stranger = app.CreateClient(Guid.NewGuid());

        using var first = await PrivacyTestHelpers.StartSpeechAsync(anonymous, _childId);
        using var second = await PrivacyTestHelpers.StartSpeechAsync(stranger, _childId);

        await AssertRefusedAsync(first, HttpStatusCode.Forbidden, ProblemTypes.Forbidden);
        await AssertRefusedAsync(second, HttpStatusCode.Forbidden, ProblemTypes.Forbidden);
    }

    [TestMethod]
    public async Task PrivacyLookupFailureFailsClosedWithServiceUnavailable()
    {
        using var app = CreateApp();
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");
        app.Store.Unavailable = true;

        using var response = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);

        await AssertRefusedAsync(response, HttpStatusCode.ServiceUnavailable, ProblemTypes.PrivacyUnavailable);
        Assert.AreEqual(0, app.Store.SpeechSessions.Count);
    }
}
