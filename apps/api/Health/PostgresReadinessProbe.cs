using DragonEnglishKids.Api.Data;

namespace DragonEnglishKids.Api.Health;

public sealed class PostgresReadinessProbe(ApplicationDbContext database)
    : IDatabaseReadinessProbe
{
    public Task<bool> CanConnectAsync(CancellationToken cancellationToken) =>
        database.Database.CanConnectAsync(cancellationToken);
}