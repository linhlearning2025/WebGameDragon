using DragonEnglishKids.Api.Data.Privacy;
using Microsoft.EntityFrameworkCore;

namespace DragonEnglishKids.Api.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<ConsentRecord> ConsentRecords => Set<ConsentRecord>();

    public DbSet<SpeechSession> SpeechSessions => Set<SpeechSession>();

    public DbSet<PrivacyRequest> PrivacyRequests => Set<PrivacyRequest>();

    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    public DbSet<RetentionRun> RetentionRuns => Set<RetentionRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConsentRecord>(entity =>
        {
            entity.ToTable("consent_records");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Purpose).HasMaxLength(64).IsRequired();
            entity.Property(x => x.NoticeVersion).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Decision).HasMaxLength(16).IsRequired();
            entity.Property(x => x.CorrelationId).HasMaxLength(128).IsRequired();
            entity.Property(x => x.IdempotencyKey).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => new { x.ParentId, x.ChildId, x.IdempotencyKey }).IsUnique();
            entity.HasIndex(x => new { x.ChildId, x.Purpose, x.DecidedAt });
        });

        modelBuilder.Entity<SpeechSession>(entity =>
        {
            entity.ToTable("speech_sessions");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.ChildId);
        });

        modelBuilder.Entity<PrivacyRequest>(entity =>
        {
            entity.ToTable("privacy_requests");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Type).HasMaxLength(16).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.IdempotencyKey).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => new { x.ParentId, x.ChildId, x.IdempotencyKey }).IsUnique();
        });

        modelBuilder.Entity<AuditEvent>(entity =>
        {
            entity.ToTable("audit_events");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ActorRole).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Action).HasMaxLength(64).IsRequired();
            entity.Property(x => x.SubjectRef).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Outcome).HasMaxLength(16).IsRequired();
            entity.Property(x => x.CorrelationId).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.OccurredAt);
        });

        modelBuilder.Entity<RetentionRun>(entity =>
        {
            entity.ToTable("retention_runs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Outcome).HasMaxLength(16).IsRequired();
            entity.Property(x => x.FailureCode).HasMaxLength(64);
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        EnsureAppendOnly();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        EnsureAppendOnly();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void EnsureAppendOnly()
    {
        var violated = ChangeTracker.Entries().Any(entry =>
            entry.Entity is ConsentRecord or AuditEvent
            && entry.State is EntityState.Modified or EntityState.Deleted);

        if (violated)
        {
            throw new InvalidOperationException("Consent records and audit events are append-only.");
        }
    }
}