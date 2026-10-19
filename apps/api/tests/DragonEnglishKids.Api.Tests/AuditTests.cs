using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy;
using DragonEnglishKids.Api.Privacy.Audit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class AuditTests
{
    [TestMethod]
    public void PseudonymIsStableKeyedAndHidesTheChildIdentifier()
    {
        var childId = Guid.NewGuid();
        var first = new SubjectPseudonymizer("key-one").Pseudonymize(childId);

        Assert.AreEqual(first, new SubjectPseudonymizer("key-one").Pseudonymize(childId));
        Assert.AreNotEqual(first, new SubjectPseudonymizer("key-two").Pseudonymize(childId));
        Assert.IsFalse(first.Contains(childId.ToString("N"), StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public async Task AuditEventsCarryOnlyMinimizedFields()
    {
        var store = new InMemoryPrivacyStore();
        var childId = Guid.NewGuid();
        var writer = new AuditWriter(
            store,
            new SubjectPseudonymizer("key"),
            new TestTimeProvider(new DateTimeOffset(2026, 10, 19, 0, 0, 0, TimeSpan.Zero)));

        await writer.WriteAsync(
            new PrivacyActor(Guid.NewGuid(), PrivacyActor.Parent),
            AuditActions.ConsentGranted,
            childId,
            AuditOutcomes.Success,
            "trace-1",
            CancellationToken.None);

        var audit = store.AuditEvents.Single();
        Assert.AreEqual("trace-1", audit.CorrelationId);
        Assert.AreEqual(PrivacyActor.Parent, audit.ActorRole);
        Assert.IsFalse(audit.SubjectRef.Contains(childId.ToString(), StringComparison.OrdinalIgnoreCase));
        var fields = typeof(AuditEvent).GetProperties().Select(property => property.Name).ToList();
        CollectionAssert.AreEquivalent(
            new[] { "Id", "OccurredAt", "ActorRole", "Action", "SubjectRef", "Outcome", "CorrelationId" },
            fields);
    }
}
