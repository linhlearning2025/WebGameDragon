using DragonEnglishKids.Api.Data.Privacy;

namespace DragonEnglishKids.Api.Privacy.Consent;

public sealed class ConsentEvaluator(INoticeCatalog notices)
{
    // Unknown or superseded state is never consent.
    public bool IsValid(ConsentRecord? latest) =>
        latest is { Decision: Decisions.Granted } && !notices.IsSuperseded(latest.NoticeVersion);

    public bool IsCurrent(ConsentRecord record) =>
        record.Decision != Decisions.Granted || !notices.IsSuperseded(record.NoticeVersion);
}
