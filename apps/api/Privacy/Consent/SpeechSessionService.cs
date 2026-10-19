using DragonEnglishKids.Api.Configuration;
using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy.Audit;

namespace DragonEnglishKids.Api.Privacy.Consent;

public sealed class SpeechSessionService(
    IPrivacyStore store,
    PrivacyAccessGuard guard,
    AuditWriter audit,
    ConsentEvaluator evaluator,
    PrivacyOptions options,
    TimeProvider time,
    ILogger<SpeechSessionService> logger)
{
    public async Task<PrivacyResult<SpeechSessionGrantDto>> StartAsync(
        PrivacyActor actor,
        Guid childId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var gate = await CheckGatesAsync(actor, childId, correlationId, cancellationToken);
            if (gate is not null)
            {
                return gate;
            }

            var latest = await store.GetLatestConsentAsync(childId, Purposes.MicrophoneSpeech, cancellationToken);
            if (!evaluator.IsValid(latest))
            {
                return PrivacyResult<SpeechSessionGrantDto>.Fail(
                    ProblemTypes.ConsentRequired,
                    StatusCodes.Status403Forbidden);
            }

            var now = time.GetUtcNow();
            var session = new SpeechSession
            {
                Id = Guid.NewGuid(),
                ChildId = childId,
                ConsentRecordId = latest!.Id,
                StartedAt = now,
                ExpiresAt = now.AddSeconds(options.SpeechLeaseSeconds)
            };
            store.AddSpeechSession(session);
            await store.SaveChangesAsync(cancellationToken);

            return PrivacyResult<SpeechSessionGrantDto>.Ok(new SpeechSessionGrantDto(session.Id, session.ExpiresAt));
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            logger.LogWarning("Privacy store failure {ErrorType} (trace {TraceId}).", error.GetType().Name, correlationId);
            return Unavailable();
        }
    }

    public async Task<PrivacyResult<SpeechSessionGrantDto>> HeartbeatAsync(
        PrivacyActor actor,
        Guid childId,
        Guid speechSessionId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var gate = await CheckGatesAsync(actor, childId, correlationId, cancellationToken);
            if (gate is not null)
            {
                return gate;
            }

            var now = time.GetUtcNow();
            var session = await store.GetSpeechSessionAsync(speechSessionId, cancellationToken);
            if (session is null || session.ChildId != childId || session.RevokedAt is not null || session.ExpiresAt <= now)
            {
                return PrivacyResult<SpeechSessionGrantDto>.Fail(
                    ProblemTypes.LeaseRefused,
                    StatusCodes.Status403Forbidden);
            }

            var latest = await store.GetLatestConsentAsync(childId, Purposes.MicrophoneSpeech, cancellationToken);
            if (!evaluator.IsValid(latest))
            {
                session.RevokedAt = now;
                await store.SaveChangesAsync(cancellationToken);
                return PrivacyResult<SpeechSessionGrantDto>.Fail(
                    ProblemTypes.LeaseRefused,
                    StatusCodes.Status403Forbidden);
            }

            session.ExpiresAt = now.AddSeconds(options.SpeechLeaseSeconds);
            await store.SaveChangesAsync(cancellationToken);

            return PrivacyResult<SpeechSessionGrantDto>.Ok(new SpeechSessionGrantDto(session.Id, session.ExpiresAt));
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            logger.LogWarning("Privacy store failure {ErrorType} (trace {TraceId}).", error.GetType().Name, correlationId);
            return Unavailable();
        }
    }

    private async Task<PrivacyResult<SpeechSessionGrantDto>?> CheckGatesAsync(
        PrivacyActor actor,
        Guid childId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        if (actor.IsGuest)
        {
            await audit.WriteAsync(
                actor,
                AuditActions.AccessDenied,
                childId,
                AuditOutcomes.Denied,
                correlationId,
                cancellationToken);
            return PrivacyResult<SpeechSessionGrantDto>.Fail(
                ProblemTypes.GuestNotAllowed,
                StatusCodes.Status403Forbidden);
        }

        if (!options.SpeechEnabled)
        {
            return PrivacyResult<SpeechSessionGrantDto>.Fail(
                ProblemTypes.SpeechDisabled,
                StatusCodes.Status403Forbidden);
        }

        return await guard.IsAllowedAsync(actor, childId, correlationId, cancellationToken)
            ? null
            : PrivacyResult<SpeechSessionGrantDto>.Fail(ProblemTypes.Forbidden, StatusCodes.Status403Forbidden);
    }

    private static PrivacyResult<SpeechSessionGrantDto> Unavailable() =>
        PrivacyResult<SpeechSessionGrantDto>.Fail(
            ProblemTypes.PrivacyUnavailable,
            StatusCodes.Status503ServiceUnavailable);
}
