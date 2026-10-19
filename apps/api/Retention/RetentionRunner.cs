using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy;
using DragonEnglishKids.Api.Privacy.Audit;

namespace DragonEnglishKids.Api.Retention;

public static class RetentionOutcomes
{
    public const string Succeeded = "succeeded";
    public const string Partial = "partial";
    public const string Failed = "failed";
}

public sealed class RetentionRunner(
    IExpiredGuestDataStore guests,
    IPrivacyStore store,
    AuditWriter audit,
    TimeProvider time,
    ILogger<RetentionRunner> logger,
    int batchSize = RetentionRunner.DefaultBatchSize)
{
    public const int DefaultBatchSize = 100;
    private const int MaxBatchesPerRun = 100;

    // Returns null when there was nothing to delete and nothing failed.
    public async Task<RetentionRun?> RunOnceAsync(CancellationToken cancellationToken)
    {
        var started = time.GetUtcNow();
        var deleted = 0;
        var drained = false;
        string? failureCode = null;

        try
        {
            for (var batch = 0; batch < MaxBatchesPerRun; batch++)
            {
                var ids = await guests.GetDeletableSessionIdsAsync(started, batchSize, cancellationToken);
                if (ids.Count == 0)
                {
                    drained = true;
                    break;
                }

                var removed = await guests.DeleteTemporaryDataAsync(ids, cancellationToken);
                deleted += removed;
                if (removed == 0)
                {
                    failureCode = "no_progress";
                    break;
                }
            }
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            failureCode = error.GetType().Name;
        }

        if (failureCode is null && deleted == 0)
        {
            return null;
        }

        var outcome = failureCode is null
            ? (drained ? RetentionOutcomes.Succeeded : RetentionOutcomes.Partial)
            : (deleted > 0 ? RetentionOutcomes.Partial : RetentionOutcomes.Failed);

        var run = new RetentionRun
        {
            Id = Guid.NewGuid(),
            StartedAt = started,
            FinishedAt = time.GetUtcNow(),
            Outcome = outcome,
            ItemsDeleted = deleted,
            FailureCode = failureCode
        };

        // Only counts and codes are logged; exception messages may contain sensitive text.
        if (outcome == RetentionOutcomes.Succeeded)
        {
            logger.LogInformation("Guest retention run {Outcome}: deleted {Count} sessions.", outcome, deleted);
        }
        else
        {
            logger.LogWarning(
                "Guest retention run {Outcome}: deleted {Count} sessions, failure code {FailureCode}.",
                outcome,
                deleted,
                failureCode ?? "incomplete");
        }

        try
        {
            store.AddRetentionRun(run);
            audit.Stage(
                new PrivacyActor(null, PrivacyActor.System),
                AuditActions.RetentionCleanup,
                null,
                outcome == RetentionOutcomes.Succeeded ? AuditOutcomes.Success : AuditOutcomes.Failed,
                run.Id.ToString());
            await store.SaveChangesAsync(cancellationToken);
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            logger.LogWarning("Guest retention run record could not be saved: {ErrorType}.", error.GetType().Name);
        }

        return run;
    }
}
