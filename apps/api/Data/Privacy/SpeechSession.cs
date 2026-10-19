namespace DragonEnglishKids.Api.Data.Privacy;

public sealed class SpeechSession
{
    public Guid Id { get; set; }

    public Guid ChildId { get; set; }

    public Guid ConsentRecordId { get; set; }

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }
}
