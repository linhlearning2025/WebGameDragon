namespace DragonEnglishKids.Api.Privacy.Consent;

public static class NoticeContent
{
    // Placeholder text only: final wording, retention, and processor details await Legal (OD-1) and OD-3.
    public static string For(string purpose) => purpose switch
    {
        Purposes.MicrophoneSpeech =>
            "[PLACEHOLDER - final wording pending Legal review] "
            + "What: your child's voice during speaking activities. "
            + "Why: to practice and check pronunciation. "
            + "By whom: [to be confirmed - browser or speech provider, pending approval]. "
            + "How long: raw audio and transcripts are not stored by default; "
            + "[retention wording pending Legal review].",
        _ => string.Empty
    };
}
