namespace DragonEnglishKids.Api.Privacy;

public interface IChildAccessAuthorizer
{
    Task<bool> OwnsChildAsync(Guid parentId, Guid childId, CancellationToken cancellationToken);
}

// Placeholder until PLAT-005 supplies real ownership checks; denies everything.
public sealed class DenyAllChildAccessAuthorizer : IChildAccessAuthorizer
{
    public Task<bool> OwnsChildAsync(Guid parentId, Guid childId, CancellationToken cancellationToken) =>
        Task.FromResult(false);
}
