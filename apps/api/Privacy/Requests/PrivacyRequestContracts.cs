namespace DragonEnglishKids.Api.Privacy.Requests;

public static class RequestTypes
{
    public const string Access = "access";
    public const string Deletion = "deletion";

    public static bool IsValid(string? type) => type is Access or Deletion;
}

public static class RequestStatuses
{
    public const string PendingVerification = "pending_verification";
    public const string Accepted = "accepted";
}

public sealed record PrivacyRequestCreateDto(string Type);

public sealed record PrivacyRequestStatusDto(Guid RequestId, string Type, string Status, DateTimeOffset DueAt);
