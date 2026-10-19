using DragonEnglishKids.Api.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class StartupConfigurationTests
{
    [TestMethod]
    public void MissingConnectionStringFailsWithSettingNameWithoutValue()
    {
        var configuration = new ConfigurationBuilder().Build();

        InvalidOperationException? exception = null;
        try
        {
            DatabaseConfiguration.GetRequiredConnectionString(configuration);
        }
        catch (InvalidOperationException error)
        {
            exception = error;
        }

        Assert.IsNotNull(exception);
        StringAssert.Contains(exception.Message, "ConnectionStrings__Default");
        Assert.IsFalse(exception.Message.Contains("Password=", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void ConfiguredConnectionStringIsReturned()
    {
        const string connectionString = "Host=localhost;Database=dev;Username=dev;Password=test-only";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString
            })
            .Build();

        var result = DatabaseConfiguration.GetRequiredConnectionString(configuration);

        Assert.AreEqual(connectionString, result);
    }

    [TestMethod]
    public void MissingAuditPseudonymKeyFailsWithSettingNameWithoutValue()
    {
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => PrivacyConfiguration.Bind(configuration, new TestHostEnvironment("Development")));

        StringAssert.Contains(exception.Message, "Privacy__AuditPseudonymKey");
    }

    [TestMethod]
    public void DevelopmentUsesProposedDefaultsAndKeepsSpeechOff()
    {
        var options = PrivacyConfiguration.Bind(
            PrivacySettings(("Privacy:AuditPseudonymKey", "k")),
            new TestHostEnvironment("Development"));

        Assert.IsFalse(options.SpeechEnabled);
        Assert.AreEqual(15, options.SpeechLeaseSeconds);
        Assert.AreEqual(24, options.GuestLifetimeHours);
    }

    [TestMethod]
    public void OutsideDevelopmentPolicyValuesMustBeSetExplicitly()
    {
        var configuration = PrivacySettings(("Privacy:AuditPseudonymKey", "k"));

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => PrivacyConfiguration.Bind(configuration, new TestHostEnvironment("Production")));

        StringAssert.Contains(exception.Message, "outside Development");
    }

    [TestMethod]
    public void ExplicitPolicyValuesAreAcceptedOutsideDevelopment()
    {
        var configuration = PrivacySettings(
            ("Privacy:AuditPseudonymKey", "k"),
            ("Privacy:SpeechLeaseSeconds", "20"),
            ("Privacy:GuestLifetimeHours", "12"),
            ("Privacy:GuestDeletionDelayHours", "12"),
            ("Privacy:PrivacyRequestTargetDays", "10"),
            ("Privacy:BackupPurgeDays", "10"));

        var options = PrivacyConfiguration.Bind(configuration, new TestHostEnvironment("Production"));

        Assert.AreEqual(20, options.SpeechLeaseSeconds);
        Assert.AreEqual(12, options.GuestLifetimeHours);
    }

    [TestMethod]
    public void NonPositivePolicyValuesAreRejected()
    {
        var configuration = PrivacySettings(
            ("Privacy:AuditPseudonymKey", "k"),
            ("Privacy:SpeechLeaseSeconds", "0"));

        Assert.ThrowsExactly<InvalidOperationException>(
            () => PrivacyConfiguration.Bind(configuration, new TestHostEnvironment("Development")));
    }

    private static IConfiguration PrivacySettings(params (string Key, string Value)[] settings) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(settings.ToDictionary(x => x.Key, x => (string?)x.Value))
            .Build();
}