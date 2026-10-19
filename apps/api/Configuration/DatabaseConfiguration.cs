using Microsoft.Extensions.Configuration;

namespace DragonEnglishKids.Api.Configuration;

public static class DatabaseConfiguration
{
    public static string GetRequiredConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Required environment variable ConnectionStrings__Default is missing.");
        }

        return connectionString;
    }
}