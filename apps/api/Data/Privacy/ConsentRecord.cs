namespace DragonEnglishKids.Api.Data.Privacy;

public sealed class ConsentRecord
{
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    public Guid ChildId { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public string NoticeVersion { get; set; } = string.Empty;

    public string Decision { get; set; } = string.Empty;

    public DateTimeOffset DecidedAt { get; set; }

    public string CorrelationId { get; set; } = string.Empty;

    public string IdempotencyKey { get; set; } = string.Empty;
}
