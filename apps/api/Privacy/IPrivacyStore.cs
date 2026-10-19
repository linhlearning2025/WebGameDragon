using DragonEnglishKids.Api.Data.Privacy;

namespace DragonEnglishKids.Api.Privacy;

// Persistence seam so privacy rules are testable without a database; one SaveChanges commits a whole action.
public interface IPrivacyStore
{
    Task<ConsentRecord?> FindConsentByIdempotencyKeyAsync(
        Guid parentId,
        Guid childId,
        string idempotencyKey,
        CancellationToken cancellationToken);

    Task<ConsentRecord?> GetLatestConsentAsync(
        Guid childId,
        string purpose,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ConsentRecord>> ListConsentsAsync(Guid childId, CancellationToken cancellationToken);

    Task<ConsentRecord?> GetConsentAsync(Guid consentRecordId, CancellationToken cancellationToken);

    Task<SpeechSession?> GetSpeechSessionAsync(Guid sessionId, CancellationToken cancellationToken);

    Task<IReadOnlyList<SpeechSession>> GetActiveSpeechSessionsAsync(
        Guid childId,
        DateTimeOffset now,
        CancellationToken cancellationToken);

    void AddConsent(ConsentRecord record);

    void AddSpeechSession(SpeechSession session);

    void AddAudit(AuditEvent auditEvent);

    void AddRetentionRun(RetentionRun run);

    Task<PrivacyRequest?> FindPrivacyRequestByIdempotencyKeyAsync(
        Guid parentId,
        Guid childId,
        string idempotencyKey,
        CancellationToken cancellationToken);

    Task<PrivacyRequest?> GetPrivacyRequestAsync(Guid requestId, CancellationToken cancellationToken);

    void AddPrivacyRequest(PrivacyRequest request);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
