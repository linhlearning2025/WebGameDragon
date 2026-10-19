namespace DragonEnglishKids.Api.Data.Privacy;

public sealed class PrivacyRequest
{
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    public Guid ChildId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string IdempotencyKey { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? VerifiedAt { get; set; }

    public DateTimeOffset DueAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }
}
