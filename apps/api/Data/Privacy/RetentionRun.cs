namespace DragonEnglishKids.Api.Data.Privacy;

public sealed class RetentionRun
{
    public Guid Id { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset? FinishedAt { get; set; }

    public string Outcome { get; set; } = string.Empty;

    public int ItemsDeleted { get; set; }

    public string? FailureCode { get; set; }
}
