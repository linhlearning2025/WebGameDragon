namespace DragonEnglishKids.Api.Privacy.Consent;

public sealed record NoticeVersion(string Version, bool Material);

public interface INoticeCatalog
{
    string CurrentVersion { get; }

    // Unknown versions and versions followed by a material change no longer count as consent.
    bool IsSuperseded(string version);
}

public sealed class NoticeCatalog(IReadOnlyList<NoticeVersion> versions) : INoticeCatalog
{
    // Notice wording and materiality are pending Legal approval (OD-1).
    public static readonly IReadOnlyList<NoticeVersion> Default = [new("2026-10-a", Material: true)];

    public string CurrentVersion => versions[^1].Version;

    public bool IsSuperseded(string version)
    {
        var index = -1;
        for (var i = 0; i < versions.Count; i++)
        {
            if (versions[i].Version == version)
            {
                index = i;
                break;
            }
        }

        if (index < 0)
        {
            return true;
        }

        for (var i = index + 1; i < versions.Count; i++)
        {
            if (versions[i].Material)
            {
                return true;
            }
        }

        return false;
    }
}
