using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy.Audit;

namespace DragonEnglishKids.Api.Privacy.Consent;

public sealed class ConsentService(
    IPrivacyStore store,
    PrivacyAccessGuard guard,
    AuditWriter audit,
    ConsentEvaluator evaluator,
    INoticeCatalog notices,
    TimeProvider time,
    ILogger<ConsentService> logger)
{
    public async Task<PrivacyResult<ConsentStateDto>> RecordDecisionAsync(
        PrivacyActor actor,
        Guid childId,
        string purpose,
        ConsentDecisionRequest request,
        string? idempotencyKey,
        string correlationId,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await guard.IsAllowedAsync(actor, childId, correlationId, cancellationToken))
            {
                return PrivacyResult<ConsentStateDto>.Fail(ProblemTypes.Forbidden, StatusCodes.Status403Forbidden);
            }

            if (!Purposes.IsSupported(purpose)
                || string.IsNullOrWhiteSpace(idempotencyKey)
                || !Decisions.IsValid(request.Decision))
            {
                return PrivacyResult<ConsentStateDto>.Fail(
                    ProblemTypes.InvalidRequest,
                    StatusCodes.Status400BadRequest);
            }

            var parentId = actor.ParentId!.Value;
            var existing = await store.FindConsentByIdempotencyKeyAsync(
                parentId,
                childId,
                idempotencyKey,
                cancellationToken);
            if (existing is not null)
            {
                return PrivacyResult<ConsentStateDto>.Ok(ToState(existing));
            }

            if (request.NoticeVersion != notices.CurrentVersion)
            {
                return PrivacyResult<ConsentStateDto>.Fail(ProblemTypes.Conflict, StatusCodes.Status409Conflict);
            }

            var now = time.GetUtcNow();
            var record = new ConsentRecord
            {
                Id = Guid.NewGuid(),
                ParentId = parentId,
                ChildId = childId,
                Purpose = purpose,
                NoticeVersion = request.NoticeVersion,
                Decision = request.Decision,
                DecidedAt = now,
                CorrelationId = correlationId,
                IdempotencyKey = idempotencyKey
            };
            store.AddConsent(record);

            if (request.Decision != Decisions.Granted)
            {
                foreach (var session in await store.GetActiveSpeechSessionsAsync(childId, now, cancellationToken))
                {
                    session.RevokedAt = now;
                }
            }

            audit.Stage(actor, ActionFor(request.Decision), childId, AuditOutcomes.Success, correlationId);
            await store.SaveChangesAsync(cancellationToken);

            return PrivacyResult<ConsentStateDto>.Ok(ToState(record));
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            logger.LogWarning("Privacy store failure {ErrorType} (trace {TraceId}).", error.GetType().Name, correlationId);
            return PrivacyResult<ConsentStateDto>.Fail(
                ProblemTypes.PrivacyUnavailable,
                StatusCodes.Status503ServiceUnavailable);
        }
    }

    public async Task<PrivacyResult<ConsentHistoryDto>> GetHistoryAsync(
        PrivacyActor actor,
        Guid childId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await guard.IsAllowedAsync(actor, childId, correlationId, cancellationToken))
            {
                return PrivacyResult<ConsentHistoryDto>.Fail(ProblemTypes.Forbidden, StatusCodes.Status403Forbidden);
            }

            var records = await store.ListConsentsAsync(childId, cancellationToken);
            audit.Stage(actor, AuditActions.ConsentHistoryViewed, childId, AuditOutcomes.Success, correlationId);
            await store.SaveChangesAsync(cancellationToken);

            return PrivacyResult<ConsentHistoryDto>.Ok(new ConsentHistoryDto(records.Select(ToState).ToList()));
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            logger.LogWarning("Privacy store failure {ErrorType} (trace {TraceId}).", error.GetType().Name, correlationId);
            return PrivacyResult<ConsentHistoryDto>.Fail(
                ProblemTypes.PrivacyUnavailable,
                StatusCodes.Status503ServiceUnavailable);
        }
    }

    private ConsentStateDto ToState(ConsentRecord record) =>
        new(record.Purpose, record.Decision, record.NoticeVersion, record.DecidedAt, evaluator.IsCurrent(record));

    private static string ActionFor(string decision) => decision switch
    {
        Decisions.Granted => AuditActions.ConsentGranted,
        Decisions.Denied => AuditActions.ConsentDenied,
        _ => AuditActions.ConsentWithdrawn
    };
}
