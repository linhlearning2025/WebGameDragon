using DragonEnglishKids.Api.Privacy.Audit;

namespace DragonEnglishKids.Api.Privacy;

public sealed class PrivacyAccessGuard(IChildAccessAuthorizer authorizer, AuditWriter audit)
{
    public async Task<bool> IsAllowedAsync(
        PrivacyActor actor,
        Guid childId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        var allowed = actor is { IsGuest: false, ParentId: { } parentId }
            && await authorizer.OwnsChildAsync(parentId, childId, cancellationToken);

        if (!allowed)
        {
            await audit.WriteAsync(
                actor,
                AuditActions.AccessDenied,
                childId,
                AuditOutcomes.Denied,
                correlationId,
                cancellationToken);
        }

        return allowed;
    }
}
