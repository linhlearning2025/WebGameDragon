using System.Text.RegularExpressions;

namespace DragonEnglishKids.Api.Privacy.Redaction;

// Last-line defense for text that may reach logs or errors; never relied on as the only protection.
public static partial class RedactionPolicy
{
    public const string Placeholder = "[redacted]";

    public static string Sanitize(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var result = BearerPattern().Replace(value, Placeholder);
        result = SecretPairPattern().Replace(result, match => $"{match.Groups[1].Value}={Placeholder}");
        return GuidPattern().Replace(result, Placeholder);
    }

    [GeneratedRegex(@"Bearer\s+[A-Za-z0-9._~+/=-]+", RegexOptions.IgnoreCase)]
    private static partial Regex BearerPattern();

    [GeneratedRegex(@"\b(token|pin|password|secret|transcript|audio)\s*[=:]\s*\S+", RegexOptions.IgnoreCase)]
    private static partial Regex SecretPairPattern();

    [GeneratedRegex(@"\b[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\b")]
    private static partial Regex GuidPattern();
}
