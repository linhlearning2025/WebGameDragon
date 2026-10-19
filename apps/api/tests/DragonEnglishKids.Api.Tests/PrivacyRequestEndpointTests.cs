using System.Net;
using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Privacy.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class PrivacyRequestEndpointTests
{
    private readonly Guid _parentId = Guid.NewGuid();
    private readonly Guid _childId = Guid.NewGuid();

    private PrivacyTestApp CreateApp(bool verified)
    {
        var app = new PrivacyTestApp();
        app.Authorizer.Grant(_parentId, _childId);
        app.Verifier.Verified = verified;
        return app;
    }

    [TestMethod]
    public async Task VerifiedOwnerRequestIsAcceptedWithTheTargetDueDate()
    {
        using var app = CreateApp(verified: true);
        using var client = app.CreateClient(_parentId);

        using var response = await PrivacyTestHelpers.CreatePrivacyRequestAsync(client, _childId, "deletion", "k1");

        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
        var body = await PrivacyTestHelpers.JsonAsync(response);
        Assert.AreEqual(RequestStatuses.Accepted, body.GetProperty("status").GetString());
        Assert.AreEqual("deletion", body.GetProperty("type").GetString());
        Assert.AreEqual(app.Time.GetUtcNow().AddDays(30), body.GetProperty("dueAt").GetDateTimeOffset());
        Assert.IsNotNull(app.Store.PrivacyRequests.Single().VerifiedAt);
        Assert.AreEqual(AuditActions.PrivacyRequestCreated, app.Store.AuditEvents.Single().Action);
    }

    [TestMethod]
    public async Task UnverifiedOwnerRequestStaysPendingAndIsNeverAccepted()
    {
        using var app = CreateApp(verified: false);
        using var client = app.CreateClient(_parentId);

        using var response = await PrivacyTestHelpers.CreatePrivacyRequestAsync(client, _childId, "access", "k1");

        Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
        var body = await PrivacyTestHelpers.JsonAsync(response);
        Assert.AreEqual(RequestStatuses.PendingVerification, body.GetProperty("status").GetString());
        Assert.IsNull(app.Store.PrivacyRequests.Single().VerifiedAt);
    }

    [TestMethod]
    public async Task InvalidTypeOrMissingIdempotencyKeyIsRejected()
    {
        using var app = CreateApp(verified: true);
        using var client = app.CreateClient(_parentId);

        using var badType = await PrivacyTestHelpers.CreatePrivacyRequestAsync(client, _childId, "export-all", "k1");
        using var noKey = await PrivacyTestHelpers.CreatePrivacyRequestAsync(client, _childId, "access");

        Assert.AreEqual(HttpStatusCode.BadRequest, badType.StatusCode);
        Assert.AreEqual(HttpStatusCode.BadRequest, noKey.StatusCode);
        Assert.AreEqual(0, app.Store.PrivacyRequests.Count);
    }

    [TestMethod]
    public async Task ReplayWithSameIdempotencyKeyDoesNotCreateADuplicate()
    {
        using var app = CreateApp(verified: true);
        using var client = app.CreateClient(_parentId);

        using var first = await PrivacyTestHelpers.CreatePrivacyRequestAsync(client, _childId, "access", "same");
        using var second = await PrivacyTestHelpers.CreatePrivacyRequestAsync(client, _childId, "access", "same");

        Assert.AreEqual(HttpStatusCode.Accepted, second.StatusCode);
        Assert.AreEqual(1, app.Store.PrivacyRequests.Count);
        Assert.AreEqual(await first.Content.ReadAsStringAsync(), await second.Content.ReadAsStringAsync());
    }

    [TestMethod]
    public async Task NonOwnerIsRejectedWithoutChangingOrRevealingAnything()
    {
        using var app = CreateApp(verified: true);
        using var stranger = app.CreateClient(Guid.NewGuid());
        using var anonymous = app.CreateClient();

        using var first = await PrivacyTestHelpers.CreatePrivacyRequestAsync(stranger, _childId, "deletion", "k1");
        using var second = await PrivacyTestHelpers.CreatePrivacyRequestAsync(anonymous, _childId, "deletion", "k1");

        Assert.AreEqual(HttpStatusCode.Forbidden, first.StatusCode);
        Assert.AreEqual(HttpStatusCode.Forbidden, second.StatusCode);
        Assert.AreEqual(0, app.Store.PrivacyRequests.Count);
    }

    [TestMethod]
    public async Task OwnerSeesStatusAndOthersCannotTellWhetherARequestExists()
    {
        using var app = CreateApp(verified: false);
        using var owner = app.CreateClient(_parentId);
        using var stranger = app.CreateClient(Guid.NewGuid());
        using var created = await PrivacyTestHelpers.CreatePrivacyRequestAsync(owner, _childId, "access", "k1");
        var requestId = (await PrivacyTestHelpers.JsonAsync(created)).GetProperty("requestId").GetGuid();

        using var ownerView = await owner.GetAsync($"/api/v1/privacy-requests/{requestId}");
        using var strangerView = await stranger.GetAsync($"/api/v1/privacy-requests/{requestId}");
        using var unknown = await stranger.GetAsync($"/api/v1/privacy-requests/{Guid.NewGuid()}");

        Assert.AreEqual(HttpStatusCode.OK, ownerView.StatusCode);
        Assert.AreEqual(RequestStatuses.PendingVerification, (await PrivacyTestHelpers.JsonAsync(ownerView)).GetProperty("status").GetString());
        Assert.AreEqual(HttpStatusCode.Forbidden, strangerView.StatusCode);
        Assert.AreEqual(HttpStatusCode.Forbidden, unknown.StatusCode);
        Assert.IsFalse((await strangerView.Content.ReadAsStringAsync()).Contains(requestId.ToString()));
    }

    [TestMethod]
    public async Task PrivacyLookupFailureFailsClosedWithServiceUnavailable()
    {
        using var app = CreateApp(verified: true);
        using var client = app.CreateClient(_parentId);
        app.Store.Unavailable = true;

        using var response = await PrivacyTestHelpers.CreatePrivacyRequestAsync(client, _childId, "access", "k1");

        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
