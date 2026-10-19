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
}