namespace DragonEnglishKids.Api.Retention;

// Seam to guest tables, wired when GUEST-001 and PLAT-003 exist. Operations must be idempotent.
public interface IExpiredGuestDataStore
{
    // Sessions expired at or before the cutoff whose data was not claimed by a parent.
    Task<IReadOnlyList<Guid>> GetDeletableSessionIdsAsync(
        DateTimeOffset expiredBefore,
        int batchSize,
        CancellationToken cancellationToken);

    // Deletes temporary data for the sessions and returns how many were actually removed.
    Task<int> DeleteTemporaryDataAsync(IReadOnlyList<Guid> sessionIds, CancellationToken cancellationToken);
}

public sealed class NullExpiredGuestDataStore : IExpiredGuestDataStore
{
    public Task<IReadOnlyList<Guid>> GetDeletableSessionIdsAsync(
        DateTimeOffset expiredBefore,
        int batchSize,
        CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Guid>>([]);

    public Task<int> DeleteTemporaryDataAsync(IReadOnlyList<Guid> sessionIds, CancellationToken cancellationToken) =>
        Task.FromResult(0);
}
