using DragonEnglishKids.Api.Data;
using DragonEnglishKids.Api.Health;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class PostgresReadinessIntegrationTests
{
    [TestMethod]
    public async Task ReadinessProbeConnectsToConfiguredPostgres()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Assert.Inconclusive("Set ConnectionStrings__Default to run the PostgreSQL integration test.");
        }

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        await using var database = new ApplicationDbContext(options);
        var probe = new PostgresReadinessProbe(database);

        Assert.IsTrue(await probe.CanConnectAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task ReadinessProbeReportsFalseWhenPostgresIsUnavailable()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=127.0.0.1;Port=1;Database=unavailable;Username=test;Password=test;Timeout=1;Pooling=false")
            .Options;
        await using var database = new ApplicationDbContext(options);
        var probe = new PostgresReadinessProbe(database);

        Assert.IsFalse(await probe.CanConnectAsync(CancellationToken.None));
    }
}