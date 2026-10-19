namespace DragonEnglishKids.Api.Configuration;

// Numeric values are proposed defaults pending approval (OD-1/OD-2); outside Development they must be set explicitly.
public sealed class PrivacyOptions
{
    public const string SectionName = "Privacy";

    public bool SpeechEnabled { get; set; }

    public int SpeechLeaseSeconds { get; set; } = 15;

    public int GuestLifetimeHours { get; set; } = 24;

    public int GuestDeletionDelayHours { get; set; } = 24;

    public int PrivacyRequestTargetDays { get; set; } = 30;

    public int BackupPurgeDays { get; set; } = 30;

    public string AuditPseudonymKey { get; set; } = string.Empty;
}
