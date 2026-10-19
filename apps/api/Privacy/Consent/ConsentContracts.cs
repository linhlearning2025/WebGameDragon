namespace DragonEnglishKids.Api.Privacy.Consent;

public static class Purposes
{
    public const string MicrophoneSpeech = "microphone_speech";

    public static bool IsSupported(string? purpose) => purpose == MicrophoneSpeech;
}

public static class Decisions
{
    public const string Granted = "granted";
    public const string Denied = "denied";
    public const string Withdrawn = "withdrawn";

    public static bool IsValid(string? decision) => decision is Granted or Denied or Withdrawn;
}

public sealed record ConsentDecisionRequest(string Decision, string NoticeVersion);

public sealed record ConsentStateDto(
    string Purpose,
    string Decision,
    string NoticeVersion,
    DateTimeOffset DecidedAt,
    bool Valid);

public sealed record ConsentHistoryDto(IReadOnlyList<ConsentStateDto> Items);

public sealed record SpeechSessionGrantDto(Guid SpeechSessionId, DateTimeOffset ExpiresAt);

public sealed record NoticeDocument(string Purpose, string NoticeVersion, string Content);
