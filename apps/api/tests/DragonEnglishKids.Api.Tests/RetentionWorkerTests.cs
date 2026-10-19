using DragonEnglishKids.Api.Configuration;
using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Retention;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class RetentionWorkerTests
{
    [TestMethod]
    public async Task ExpiredUnclaimedDataIsDeletedAndEverythingElseIsKept()
    {
        var harness = new RetentionHarness();
        var now = harness.Time.GetUtcNow();
        var expired = harness.Guests.Add(now.AddHours(-1));
        var active = harness.Guests.Add(now.AddHours(1));
        var claimed = harness.Guests.Add(now.AddHours(-1), claimed: true);

        var run = await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.IsFalse(harness.Guests.Exists(expired));
        Assert.IsTrue(harness.Guests.Exists(active));
        Assert.IsTrue(harness.Guests.Exists(claimed));
        Assert.AreEqual(RetentionOutcomes.Succeeded, run?.Outcome);
        Assert.AreEqual(1, run?.ItemsDeleted);
    }

    [TestMethod]
    public async Task SuccessfulRunIsRecordedAndAuditedAsASystemAction()
    {
        var harness = new RetentionHarness();
        harness.Guests.Add(harness.Time.GetUtcNow().AddHours(-1));

        await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.AreEqual(1, harness.Store.RetentionRuns.Count);
        var audit = harness.Store.AuditEvents.Single();
        Assert.AreEqual(AuditActions.RetentionCleanup, audit.Action);
        Assert.AreEqual("system", audit.ActorRole);
        Assert.AreEqual(AuditOutcomes.Success, audit.Outcome);
        Assert.AreEqual(string.Empty, audit.SubjectRef);
    }

    [TestMethod]
    public async Task DataIsDeletedBeforeTheDeletionDeadline()
    {
        var harness = new RetentionHarness();
        var policy = new GuestExpiryPolicy(new PrivacyOptions(), harness.Time);
        var created = harness.Time.GetUtcNow();
        var id = harness.Guests.Add(policy.ExpiresAt(created));
        harness.Time.Advance(TimeSpan.FromHours(25));

        await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.IsFalse(harness.Guests.Exists(id));
        Assert.IsTrue(harness.Time.GetUtcNow() <= policy.DeletionDeadline(created));
    }

    [TestMethod]
    public async Task NothingToDeleteWritesNoRunRecord()
    {
        var harness = new RetentionHarness();
        harness.Guests.Add(harness.Time.GetUtcNow().AddHours(1));

        var run = await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.IsNull(run);
        Assert.AreEqual(0, harness.Store.RetentionRuns.Count);
        Assert.AreEqual(0, harness.Store.AuditEvents.Count);
    }

    [TestMethod]
    public void RunIntervalFitsSeveralTimesInsideTheDeletionWindow()
    {
        var interval = RetentionWorker.IntervalFor(new PrivacyOptions());

        Assert.AreEqual(TimeSpan.FromHours(6), interval);
        Assert.IsTrue(interval < TimeSpan.FromHours(24));
        Assert.AreEqual(
            TimeSpan.FromMinutes(1),
            RetentionWorker.IntervalFor(new PrivacyOptions { GuestDeletionDelayHours = 0 }));
    }
}
