using System.Security.Claims;

namespace DragonEnglishKids.Api.Privacy;

public sealed record PrivacyActor(Guid? ParentId, string Role)
{
    public const string Parent = "parent";
    public const string Guest = "guest";
    public const string Anonymous = "anonymous";
    public const string System = "system";

    public bool IsGuest => Role == Guest;

    public static PrivacyActor From(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return new PrivacyActor(null, Anonymous);
        }

        var role = user.FindFirstValue(ClaimTypes.Role) ?? Parent;
        var parentId = Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : (Guid?)null;

        return new PrivacyActor(role == Guest ? null : parentId, role);
    }
}
