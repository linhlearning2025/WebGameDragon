namespace DragonEnglishKids.Api.Data.Privacy;

public sealed class AuditEvent
{
    public Guid Id { get; set; }

    public DateTimeOffset OccurredAt { get; set; }

    public string ActorRole { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string SubjectRef { get; set; } = string.Empty;

    public string Outcome { get; set; } = string.Empty;

    public string CorrelationId { get; set; } = string.Empty;
}
