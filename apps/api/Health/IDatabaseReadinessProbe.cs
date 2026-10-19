namespace DragonEnglishKids.Api.Health;

public interface IDatabaseReadinessProbe
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}