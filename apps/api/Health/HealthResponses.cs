namespace DragonEnglishKids.Api.Health;

public sealed record HealthResponse(int StatusCode, HealthPayload Body);

public sealed record HealthPayload(string Status, HealthChecks? Checks = null);

public sealed record HealthChecks(string Database);

public static class HealthResponses
{
    public static HealthResponse Liveness() =>
        new(StatusCodes.Status200OK, new HealthPayload("live"));

    public static async Task<HealthResponse> ReadinessAsync(
        IDatabaseReadinessProbe databaseProbe,
        CancellationToken cancellationToken)
    {
        try
        {
            var canConnect = await databaseProbe.CanConnectAsync(cancellationToken);
            return canConnect
                ? new HealthResponse(StatusCodes.Status200OK, new HealthPayload("ready", new HealthChecks("ready")))
                : new HealthResponse(StatusCodes.Status503ServiceUnavailable, new HealthPayload("not_ready", new HealthChecks("unavailable")));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return new HealthResponse(
                StatusCodes.Status503ServiceUnavailable,
                new HealthPayload("not_ready", new HealthChecks("unavailable")));
        }
    }
}