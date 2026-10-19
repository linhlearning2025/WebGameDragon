using DragonEnglishKids.Api.Data.Privacy;

namespace DragonEnglishKids.Api.Privacy.Audit;

public static class AuditActions
{
    public const string ConsentGranted = "consent.granted";
    public const string ConsentDenied = "consent.denied";
    public const string ConsentWithdrawn = "consent.withdrawn";
    public const string ConsentHistoryViewed = "consent.history.viewed";
    public const string AccessDenied = "access.denied";
    public const string RetentionCleanup = "retention.cleanup";
    public const string PrivacyRequestCreated = "privacy.request.created";
}

public static class AuditOutcomes
{
    public const string Success = "success";
    public const string Denied = "denied";
    public const string Failed = "failed";
}

// Stages minimized events; the caller commits them in the same save as the audited action.
public sealed class AuditWriter(IPrivacyStore store, SubjectPseudonymizer pseudonymizer, TimeProvider time)
{
    public void Stage(PrivacyActor actor, string action, Guid? childId, string outcome, string correlationId)
    {
        store.AddAudit(new AuditEvent
        {
            Id = Guid.NewGuid(),
            OccurredAt = time.GetUtcNow(),
            ActorRole = actor.Role,
            Action = action,
            SubjectRef = childId is { } id ? pseudonymizer.Pseudonymize(id) : string.Empty,
            Outcome = outcome,
            CorrelationId = correlationId
        });
    }

    public async Task WriteAsync(
        PrivacyActor actor,
        string action,
        Guid? childId,
        string outcome,
        string correlationId,
        CancellationToken cancellationToken)
    {
        Stage(actor, action, childId, outcome, correlationId);
        await store.SaveChangesAsync(cancellationToken);
    }
}
