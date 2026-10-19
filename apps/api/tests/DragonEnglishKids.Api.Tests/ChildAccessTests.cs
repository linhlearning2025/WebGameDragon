using System.Net;
using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Privacy.Consent;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class ChildAccessTests
{
    private static readonly (string Name, Func<HttpClient, Guid, Task<HttpResponseMessage>> Call)[] ChildRoutes =
    [
        ("consent history", (client, childId) => client.GetAsync($"/api/v1/children/{childId}/consents")),
        (
            "record consent",
            (client, childId) => PrivacyTestHelpers.PutConsentAsync(
                client,
                childId,
                Decisions.Granted,
                idempotencyKey: "k1")),
        ("start speech", PrivacyTestHelpers.StartSpeechAsync),
        (
            "speech heartbeat",
            (client, childId) => PrivacyTestHelpers.HeartbeatAsync(client, childId, Guid.NewGuid())),
        (
            "create privacy request",
            (client, childId) => PrivacyTestHelpers.CreatePrivacyRequestAsync(client, childId, "access", "k1"))
    ];

    private readonly Guid _parentId = Guid.NewGuid();
    private readonly Guid _childId = Guid.NewGuid();

    [TestMethod]
    public async Task NonOwnersGuestsAndAnonymousCallersAreDeniedOnEveryChildRoute()
    {
        foreach (var (name, call) in ChildRoutes)
        {
            using var app = new PrivacyTestApp();
            app.Authorizer.Grant(_parentId, _childId);
            using var anonymous = app.CreateClient();
            using var guest = app.CreateClient(role: "guest");
            using var stranger = app.CreateClient(Guid.NewGuid());

            foreach (var client in new[] { anonymous, guest, stranger })
            {
                using var response = await call(client, _childId);
                var body = await response.Content.ReadAsStringAsync();

                Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, name);
                Assert.IsFalse(body.Contains(_childId.ToString(), StringComparison.OrdinalIgnoreCase), name);
            }

            Assert.AreEqual(0, app.Store.Consents.Count, name);
            Assert.AreEqual(0, app.Store.SpeechSessions.Count, name);
            Assert.IsTrue(app.Store.AuditEvents.Count > 0, name);
            Assert.IsTrue(app.Store.AuditEvents.All(x => x.Action == AuditActions.AccessDenied), name);
            Assert.IsTrue(
                app.Store.AuditEvents.All(x =>
                    !x.SubjectRef.Contains(_childId.ToString(), StringComparison.OrdinalIgnoreCase)),
                name);
        }
    }

    [TestMethod]
    public async Task OwnerOfAnotherChildCannotUseTheRoutesForThisChild()
    {
        using var app = new PrivacyTestApp();
        var otherChild = Guid.NewGuid();
        app.Authorizer.Grant(_parentId, otherChild);
        using var client = app.CreateClient(_parentId);

        foreach (var (name, call) in ChildRoutes)
        {
            using var response = await call(client, _childId);

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode, name);
        }

        Assert.AreEqual(0, app.Store.Consents.Count);
    }

    [TestMethod]
    public void EveryRouteThatTakesAChildIdIsCoveredByTheAccessSweep()
    {
        using var app = new PrivacyTestApp();

        var childRoutes = app.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .Count(endpoint => endpoint.RoutePattern.RawText?.Contains("{childId", StringComparison.Ordinal) == true);

        Assert.AreEqual(ChildRoutes.Length, childRoutes, "Add new child routes to the access sweep.");
    }
}
