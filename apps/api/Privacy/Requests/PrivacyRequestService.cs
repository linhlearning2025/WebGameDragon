using DragonEnglishKids.Api.Configuration;
using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy.Audit;

namespace DragonEnglishKids.Api.Privacy.Requests;

// Tracks request status only; export and deletion are executed by AUTH-007.
public sealed class PrivacyRequestService(
    IPrivacyStore store,
    PrivacyAccessGuard guard,
    IParentVerifier verifier,
    AuditWriter audit,
    PrivacyOptions options,
    TimeProvider time,
    ILogger<PrivacyRequestService> logger)
{
    public async Task<PrivacyResult<PrivacyRequestStatusDto>> CreateAsync(
        PrivacyActor actor,
        Guid childId,
        string? type,
        string? idempotencyKey,
        string correlationId,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await guard.IsAllowedAsync(actor, childId, correlationId, cancellationToken))
            {
                return PrivacyResult<PrivacyRequestStatusDto>.Fail(
                    ProblemTypes.Forbidden,
                    StatusCodes.Status403Forbidden);
            }

            if (!RequestTypes.IsValid(type) || string.IsNullOrWhiteSpace(idempotencyKey))
            {
                return PrivacyResult<PrivacyRequestStatusDto>.Fail(
                    ProblemTypes.InvalidRequest,
                    StatusCodes.Status400BadRequest);
            }

            var parentId = actor.ParentId!.Value;
            var existing = await store.FindPrivacyRequestByIdempotencyKeyAsync(
                parentId,
                childId,
                idempotencyKey,
                cancellationToken);
            if (existing is not null)
            {
                return PrivacyResult<PrivacyRequestStatusDto>.Ok(ToDto(existing));
            }

            var now = time.GetUtcNow();
            var verified = await verifier.IsVerifiedAsync(actor, childId, cancellationToken);
            var request = new PrivacyRequest
            {
                Id = Guid.NewGuid(),
                ParentId = parentId,
                ChildId = childId,
                Type = type!,
                Status = verified ? RequestStatuses.Accepted : RequestStatuses.PendingVerification,
                IdempotencyKey = idempotencyKey,
                CreatedAt = now,
                VerifiedAt = verified ? now : null,
                DueAt = now.AddDays(options.PrivacyRequestTargetDays)
            };
            store.AddPrivacyRequest(request);
            audit.Stage(actor, AuditActions.PrivacyRequestCreated, childId, AuditOutcomes.Success, correlationId);
            await store.SaveChangesAsync(cancellationToken);

            return PrivacyResult<PrivacyRequestStatusDto>.Ok(ToDto(request));
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            logger.LogWarning("Privacy store failure {ErrorType} (trace {TraceId}).", error.GetType().Name, correlationId);
            return Unavailable();
        }
    }

    public async Task<PrivacyResult<PrivacyRequestStatusDto>> GetAsync(
        PrivacyActor actor,
        Guid requestId,
        string correlationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = await store.GetPrivacyRequestAsync(requestId, cancellationToken);

            // Missing and foreign requests look identical so existence is never disclosed.
            if (request is null || actor.IsGuest || actor.ParentId != request.ParentId)
            {
                await audit.WriteAsync(
                    actor,
                    AuditActions.AccessDenied,
                    null,
                    AuditOutcomes.Denied,
                    correlationId,
                    cancellationToken);
                return PrivacyResult<PrivacyRequestStatusDto>.Fail(
                    ProblemTypes.Forbidden,
                    StatusCodes.Status403Forbidden);
            }

            return PrivacyResult<PrivacyRequestStatusDto>.Ok(ToDto(request));
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            logger.LogWarning("Privacy store failure {ErrorType} (trace {TraceId}).", error.GetType().Name, correlationId);
            return Unavailable();
        }
    }

    private static PrivacyRequestStatusDto ToDto(PrivacyRequest request) =>
        new(request.Id, request.Type, request.Status, request.DueAt);

    private static PrivacyResult<PrivacyRequestStatusDto> Unavailable() =>
        PrivacyResult<PrivacyRequestStatusDto>.Fail(
            ProblemTypes.PrivacyUnavailable,
            StatusCodes.Status503ServiceUnavailable);
}
