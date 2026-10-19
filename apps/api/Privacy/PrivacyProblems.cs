using Microsoft.AspNetCore.Http;

namespace DragonEnglishKids.Api.Privacy;

public static class ProblemTypes
{
    public const string ConsentRequired = "consent_required";
    public const string SpeechDisabled = "speech_disabled";
    public const string PrivacyUnavailable = "privacy_unavailable";
    public const string GuestNotAllowed = "guest_not_allowed";
    public const string LeaseRefused = "lease_refused";
    public const string Forbidden = "forbidden";
    public const string Conflict = "conflict";
    public const string InvalidRequest = "invalid_request";
}

public sealed record PrivacyResult<T>(T? Value, string? ProblemType, int StatusCode)
{
    public bool Succeeded => ProblemType is null;

    public static PrivacyResult<T> Ok(T value) => new(value, null, StatusCodes.Status200OK);

    public static PrivacyResult<T> Fail(string problemType, int statusCode) => new(default, problemType, statusCode);

    public IResult ToHttpResult(HttpContext context, int successStatusCode = StatusCodes.Status200OK)
    {
        if (Succeeded)
        {
            return Results.Json(Value, statusCode: successStatusCode);
        }

        return Results.Problem(
            type: ProblemType,
            title: Title(ProblemType!),
            statusCode: StatusCode,
            extensions: new Dictionary<string, object?> { ["traceId"] = context.TraceIdentifier });
    }

    private static string Title(string type) => type switch
    {
        ProblemTypes.ConsentRequired => "Consent required",
        ProblemTypes.SpeechDisabled => "Speech is not available",
        ProblemTypes.PrivacyUnavailable => "Privacy state is unavailable",
        ProblemTypes.GuestNotAllowed => "Not available for guests",
        ProblemTypes.LeaseRefused => "Speech session is no longer valid",
        ProblemTypes.Conflict => "Conflict",
        ProblemTypes.InvalidRequest => "Invalid request",
        _ => "Forbidden"
    };
}
