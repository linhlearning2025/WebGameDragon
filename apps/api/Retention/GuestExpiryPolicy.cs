using DragonEnglishKids.Api.Configuration;

namespace DragonEnglishKids.Api.Retention;

// Uses server time only, so client clocks can never extend a guest lifetime.
public sealed class GuestExpiryPolicy(PrivacyOptions options, TimeProvider time)
{
    public DateTimeOffset ExpiresAt(DateTimeOffset createdAt) => createdAt.AddHours(options.GuestLifetimeHours);

    public bool IsExpired(DateTimeOffset createdAt) => time.GetUtcNow() >= ExpiresAt(createdAt);

    public DateTimeOffset DeletionDeadline(DateTimeOffset createdAt) =>
        ExpiresAt(createdAt).AddHours(options.GuestDeletionDelayHours);
}
