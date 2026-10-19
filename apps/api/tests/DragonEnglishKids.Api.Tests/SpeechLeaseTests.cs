using System.Net;
using DragonEnglishKids.Api.Privacy;
using DragonEnglishKids.Api.Privacy.Consent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class SpeechLeaseTests
{
    private readonly Guid _parentId = Guid.NewGuid();
    private readonly Guid _childId = Guid.NewGuid();

    private async Task<(PrivacyTestApp App, HttpClient Client, Guid SessionId)> StartAsync()
    {
        var app = new PrivacyTestApp();
        app.Authorizer.Grant(_parentId, _childId);
        var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");
        using var started = await PrivacyTestHelpers.StartSpeechAsync(client, _childId);
        var body = await PrivacyTestHelpers.JsonAsync(started);
        return (app, client, body.GetProperty("speechSessionId").GetGuid());
    }

    [TestMethod]
    public async Task HeartbeatRenewsTheLeaseWhileConsentStaysValid()
    {
        var (app, client, sessionId) = await StartAsync();
        using var _ = app;
        using var __ = client;
        app.Time.Advance(TimeSpan.FromSeconds(10));

        using var response = await PrivacyTestHelpers.HeartbeatAsync(client, _childId, sessionId);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(app.Time.GetUtcNow().AddSeconds(15), app.Store.SpeechSessions.Single().ExpiresAt);
    }

    [TestMethod]
    public async Task WithdrawalFromAnotherDeviceStopsSpeechAtTheNextHeartbeat()
    {
        var (app, client, sessionId) = await StartAsync();
        using var _ = app;
        using var __ = client;
        using var otherDevice = app.CreateClient(_parentId);
        app.Time.Advance(TimeSpan.FromSeconds(5));
        using var withdrawn = await PrivacyTestHelpers.PutConsentAsync(otherDevice, _childId, Decisions.Withdrawn, idempotencyKey: "k2");

        using var response = await PrivacyTestHelpers.HeartbeatAsync(client, _childId, sessionId);

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.AreEqual(ProblemTypes.LeaseRefused, await PrivacyTestHelpers.ProblemTypeAsync(response));
    }

    [TestMethod]
    public async Task ExpiredLeaseIsRefused()
    {
        var (app, client, sessionId) = await StartAsync();
        using var _ = app;
        using var __ = client;
        app.Time.Advance(TimeSpan.FromSeconds(16));

        using var response = await PrivacyTestHelpers.HeartbeatAsync(client, _childId, sessionId);

        Assert.AreEqual(ProblemTypes.LeaseRefused, await PrivacyTestHelpers.ProblemTypeAsync(response));
    }

    [TestMethod]
    public async Task HeartbeatForAnotherChildIsRefusedWithoutDisclosure()
    {
        var (app, client, sessionId) = await StartAsync();
        using var _ = app;
        using var __ = client;
        var other = Guid.NewGuid();
        app.Authorizer.Grant(_parentId, other);

        using var response = await PrivacyTestHelpers.HeartbeatAsync(client, other, sessionId);

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.AreEqual(ProblemTypes.LeaseRefused, await PrivacyTestHelpers.ProblemTypeAsync(response));
    }

    [TestMethod]
    public async Task GuestsAndStrangersCannotRenewALease()
    {
        var (app, client, sessionId) = await StartAsync();
        using var _ = app;
        using var __ = client;
        using var guest = app.CreateClient(role: "guest");
        using var stranger = app.CreateClient(Guid.NewGuid());

        using var guestResponse = await PrivacyTestHelpers.HeartbeatAsync(guest, _childId, sessionId);
        using var strangerResponse = await PrivacyTestHelpers.HeartbeatAsync(stranger, _childId, sessionId);

        Assert.AreEqual(ProblemTypes.GuestNotAllowed, await PrivacyTestHelpers.ProblemTypeAsync(guestResponse));
        Assert.AreEqual(ProblemTypes.Forbidden, await PrivacyTestHelpers.ProblemTypeAsync(strangerResponse));
    }
}
