namespace DragonEnglishKids.Api.Privacy.Requests;

public interface IParentVerifier
{
    // True only when the parent is signed in, passed the parental gate, and confirmed through the account email.
    Task<bool> IsVerifiedAsync(PrivacyActor actor, Guid childId, CancellationToken cancellationToken);
}

// Placeholder until AUTH-001 and AUTH-005 provide verification; every request stays pending.
public sealed class DenyAllParentVerifier : IParentVerifier
{
    public Task<bool> IsVerifiedAsync(PrivacyActor actor, Guid childId, CancellationToken cancellationToken) =>
        Task.FromResult(false);
}
