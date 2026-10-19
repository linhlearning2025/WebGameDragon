using System.Net;
using DragonEnglishKids.Api.Privacy.Consent;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class AuditVisibilityTests
{
    private readonly Guid _parentId = Guid.NewGuid();
    private readonly Guid _childId = Guid.NewGuid();

    [TestMethod]
    public void NoPublicEndpointExposesAuditEvents()
    {
        using var app = new PrivacyTestApp();

        var routes = app.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText ?? string.Empty)
            .ToList();

        Assert.IsFalse(routes.Any(route => route.Contains("audit", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public async Task ParentsSeeOnlyConsentHistoryFieldsForTheirOwnChild()
    {
        using var app = new PrivacyTestApp();
        app.Authorizer.Grant(_parentId, _childId);
        using var client = app.CreateClient(_parentId);
        using var granted = await PrivacyTestHelpers.PutConsentAsync(client, _childId, Decisions.Granted, idempotencyKey: "k1");

        using var response = await client.GetAsync($"/api/v1/children/{_childId}/consents");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var item = (await PrivacyTestHelpers.JsonAsync(response)).GetProperty("items")[0];
        CollectionAssert.AreEquivalent(
            new[] { "purpose", "decision", "noticeVersion", "decidedAt", "valid" },
            item.EnumerateObject().Select(property => property.Name).ToArray());
    }

    [TestMethod]
    public async Task AnotherParentCannotSeeThisChildsConsentHistory()
    {
        using var app = new PrivacyTestApp();
        app.Authorizer.Grant(_parentId, _childId);
        using var owner = app.CreateClient(_parentId);
        using var other = app.CreateClient(Guid.NewGuid());
        using var granted = await PrivacyTestHelpers.PutConsentAsync(owner, _childId, Decisions.Granted, idempotencyKey: "k1");

        using var response = await other.GetAsync($"/api/v1/children/{_childId}/consents");

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
