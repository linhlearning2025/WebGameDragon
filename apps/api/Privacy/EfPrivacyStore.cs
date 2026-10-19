using DragonEnglishKids.Api.Data;
using DragonEnglishKids.Api.Data.Privacy;
using Microsoft.EntityFrameworkCore;

namespace DragonEnglishKids.Api.Privacy;

public sealed class EfPrivacyStore(ApplicationDbContext database) : IPrivacyStore
{
    public Task<ConsentRecord?> FindConsentByIdempotencyKeyAsync(
        Guid parentId,
        Guid childId,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        database.ConsentRecords.AsNoTracking().FirstOrDefaultAsync(
            x => x.ParentId == parentId && x.ChildId == childId && x.IdempotencyKey == idempotencyKey,
            cancellationToken);

    public Task<ConsentRecord?> GetLatestConsentAsync(
        Guid childId,
        string purpose,
        CancellationToken cancellationToken) =>
        database.ConsentRecords.AsNoTracking()
            .Where(x => x.ChildId == childId && x.Purpose == purpose)
            .OrderByDescending(x => x.DecidedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<ConsentRecord>> ListConsentsAsync(
        Guid childId,
        CancellationToken cancellationToken) =>
        await database.ConsentRecords.AsNoTracking()
            .Where(x => x.ChildId == childId)
            .OrderByDescending(x => x.DecidedAt)
            .ToListAsync(cancellationToken);

    public Task<ConsentRecord?> GetConsentAsync(Guid consentRecordId, CancellationToken cancellationToken) =>
        database.ConsentRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == consentRecordId, cancellationToken);

    public Task<SpeechSession?> GetSpeechSessionAsync(Guid sessionId, CancellationToken cancellationToken) =>
        database.SpeechSessions.FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

    public async Task<IReadOnlyList<SpeechSession>> GetActiveSpeechSessionsAsync(
        Guid childId,
        DateTimeOffset now,
        CancellationToken cancellationToken) =>
        await database.SpeechSessions
            .Where(x => x.ChildId == childId && x.RevokedAt == null && x.ExpiresAt > now)
            .ToListAsync(cancellationToken);

    public void AddConsent(ConsentRecord record) => database.ConsentRecords.Add(record);

    public void AddSpeechSession(SpeechSession session) => database.SpeechSessions.Add(session);

    public void AddAudit(AuditEvent auditEvent) => database.AuditEvents.Add(auditEvent);

    public void AddRetentionRun(RetentionRun run) => database.RetentionRuns.Add(run);

    public Task<PrivacyRequest?> FindPrivacyRequestByIdempotencyKeyAsync(
        Guid parentId,
        Guid childId,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        database.PrivacyRequests.AsNoTracking().FirstOrDefaultAsync(
            x => x.ParentId == parentId && x.ChildId == childId && x.IdempotencyKey == idempotencyKey,
            cancellationToken);

    public Task<PrivacyRequest?> GetPrivacyRequestAsync(Guid requestId, CancellationToken cancellationToken) =>
        database.PrivacyRequests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);

    public void AddPrivacyRequest(PrivacyRequest request) => database.PrivacyRequests.Add(request);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => database.SaveChangesAsync(cancellationToken);
}
