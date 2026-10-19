using System.Net;
using System.Net.Http.Json;
using DragonEnglishKids.Api.Health;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class ReadinessEndpointTests
{
    [TestMethod]
    public async Task LivenessReturnsSuccessWithoutCheckingDatabase()
    {
        using var client = CreateClient(databaseAvailable: false);

        using var response = await client.GetAsync("/health/live");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthPayload>();
        Assert.AreEqual("live", body?.Status);
    }

    [TestMethod]
    public async Task ReadinessReturnsSuccessWhenDatabaseProbeSucceeds()
    {
        using var client = CreateClient(databaseAvailable: true);

        using var response = await client.GetAsync("/health/ready");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<HealthPayload>();
        Assert.AreEqual("ready", body?.Status);
        Assert.AreEqual("ready", body?.Checks?.Database);
    }

    [TestMethod]
    public async Task ReadinessReturnsUnavailableWhenDatabaseProbeFails()
    {
        using var client = CreateClient(databaseAvailable: false);

        using var response = await client.GetAsync("/health/ready");
        var bodyText = await response.Content.ReadAsStringAsync();

        Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        StringAssert.Contains(bodyText, "not_ready");
        StringAssert.Contains(bodyText, "unavailable");
        Assert.IsFalse(bodyText.Contains("test-secret", StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(bodyText.Contains("Password=", StringComparison.OrdinalIgnoreCase));
    }

    private static HttpClient CreateClient(bool databaseAvailable)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting(
                    "ConnectionStrings:Default",
                    "Host=localhost;Database=dev;Username=dev;Password=test-secret");
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IDatabaseReadinessProbe>();
                    services.AddSingleton<IDatabaseReadinessProbe>(
                        new StubDatabaseReadinessProbe(databaseAvailable));
                });
            });

        return factory.CreateClient();
    }

    private sealed class StubDatabaseReadinessProbe(bool canConnect) : IDatabaseReadinessProbe
    {
        public Task<bool> CanConnectAsync(CancellationToken cancellationToken) => Task.FromResult(canConnect);
    }

    private sealed record HealthPayload(string Status, HealthChecks? Checks);

    private sealed record HealthChecks(string Database);
}