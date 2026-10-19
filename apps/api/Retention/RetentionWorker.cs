using DragonEnglishKids.Api.Configuration;

namespace DragonEnglishKids.Api.Retention;

public sealed class RetentionWorker(
    IServiceScopeFactory scopes,
    PrivacyOptions options,
    TimeProvider time,
    ILogger<RetentionWorker> logger) : BackgroundService
{
    // Several runs fit inside the deletion window so a failed run can retry before the limit passes.
    public static TimeSpan IntervalFor(PrivacyOptions options)
    {
        var interval = TimeSpan.FromHours(options.GuestDeletionDelayHours) / 4;
        return interval < TimeSpan.FromMinutes(1) ? TimeSpan.FromMinutes(1) : interval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(IntervalFor(options), time);
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = scopes.CreateScope();
                    await scope.ServiceProvider.GetRequiredService<RetentionRunner>().RunOnceAsync(stoppingToken);
                }
                catch (Exception error) when (error is not OperationCanceledException)
                {
                    logger.LogWarning("Guest retention worker iteration failed: {ErrorType}.", error.GetType().Name);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown.
        }
    }
}
