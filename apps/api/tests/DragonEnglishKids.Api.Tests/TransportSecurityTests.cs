using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class TransportSecurityTests
{
    private static WebApplicationFactory<Program> CreateFactory(string environment) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
            builder.UseSetting("HTTPS_PORT", "443");
            builder.UseSetting("ConnectionStrings:Default", "Host=localhost;Database=dev;Username=dev;Password=test-secret");
            builder.UseSetting("Privacy:AuditPseudonymKey", "test-only-audit-key");
            builder.UseSetting("Privacy:SpeechLeaseSeconds", "15");
            builder.UseSetting("Privacy:GuestLifetimeHours", "24");
            builder.UseSetting("Privacy:GuestDeletionDelayHours", "24");
            builder.UseSetting("Privacy:PrivacyRequestTargetDays", "30");
            builder.UseSetting("Privacy:BackupPurgeDays", "30");
        });

    [TestMethod]
    public async Task OutsideDevelopmentPlainHttpIsRedirectedToHttps()
    {
        using var factory = CreateFactory("Production");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://app.example.test")
        });

        using var response = await client.GetAsync("/health/live");

        Assert.AreEqual(HttpStatusCode.TemporaryRedirect, response.StatusCode);
        StringAssert.StartsWith(response.Headers.Location?.ToString(), "https://");
    }

    [TestMethod]
    public async Task OutsideDevelopmentHttpsResponsesCarryStrictTransportSecurity()
    {
        using var factory = CreateFactory("Production");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://app.example.test")
        });

        using var response = await client.GetAsync("/health/live");

        Assert.IsTrue(response.Headers.Contains("Strict-Transport-Security"));
    }

    [TestMethod]
    public async Task DevelopmentDoesNotRedirect()
    {
        using var factory = CreateFactory("Development");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://localhost")
        });

        using var response = await client.GetAsync("/health/live");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
