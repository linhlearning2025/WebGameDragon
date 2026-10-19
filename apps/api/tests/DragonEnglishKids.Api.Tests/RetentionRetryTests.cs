using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Retention;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class RetentionRetryTests
{
    [TestMethod]
    public async Task FailedRunIsRecordedAndTheNextRunDeletesEverythingExactlyOnce()
    {
        var harness = new RetentionHarness();
        var ids = Enumerable.Range(0, 3)
            .Select(_ => harness.Guests.Add(harness.Time.GetUtcNow().AddHours(-1)))
            .ToList();
        harness.Guests.BeforeDelete = _ => throw new InvalidOperationException("store offline");

        var failed = await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.AreEqual(RetentionOutcomes.Failed, failed?.Outcome);
        Assert.AreEqual("InvalidOperationException", failed?.FailureCode);
        Assert.IsTrue(ids.All(harness.Guests.Exists));
        Assert.AreEqual(AuditOutcomes.Failed, harness.Store.AuditEvents.Single().Outcome);

        harness.Guests.BeforeDelete = null;
        var retried = await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.AreEqual(RetentionOutcomes.Succeeded, retried?.Outcome);
        Assert.IsTrue(ids.All(id => !harness.Guests.Exists(id) && harness.Guests.TimesDeleted(id) == 1));
    }

    [TestMethod]
    public async Task PartialRunResumesWithoutDuplicatesOrMisses()
    {
        var harness = new RetentionHarness(batchSize: 2);
        var ids = Enumerable.Range(0, 5)
            .Select(_ => harness.Guests.Add(harness.Time.GetUtcNow().AddHours(-1)))
            .ToList();
        harness.Guests.BeforeDelete = call =>
        {
            if (call == 2)
            {
                throw new InvalidOperationException("transient");
            }
        };

        var partial = await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.AreEqual(RetentionOutcomes.Partial, partial?.Outcome);
        Assert.AreEqual(2, partial?.ItemsDeleted);

        harness.Guests.BeforeDelete = null;
        var resumed = await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.AreEqual(3, resumed?.ItemsDeleted);
        Assert.IsTrue(ids.All(id => harness.Guests.TimesDeleted(id) == 1));
        Assert.IsNull(await harness.Runner.RunOnceAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task FailuresAreVisibleToOperatorsWithoutSensitiveText()
    {
        var harness = new RetentionHarness();
        harness.Guests.Add(harness.Time.GetUtcNow().AddHours(-1));
        harness.Guests.BeforeDelete = _ => throw new InvalidOperationException("password=hunter2 for child");

        await harness.Runner.RunOnceAsync(CancellationToken.None);

        var logs = string.Join('\n', harness.Logs.Messages);
        StringAssert.Contains(logs, "failed");
        StringAssert.Contains(logs, "InvalidOperationException");
        StringAssert.Contains(logs, "deleted 0");
        Assert.IsFalse(logs.Contains("hunter2", StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual(1, harness.Store.RetentionRuns.Count);
    }

    [TestMethod]
    public async Task RepeatedFailuresAreEachRecordedAndNothingIsDeletedMeanwhile()
    {
        var harness = new RetentionHarness();
        var id = harness.Guests.Add(harness.Time.GetUtcNow().AddHours(-1));
        harness.Guests.BeforeDelete = _ => throw new InvalidOperationException("offline");

        await harness.Runner.RunOnceAsync(CancellationToken.None);
        await harness.Runner.RunOnceAsync(CancellationToken.None);

        Assert.IsTrue(harness.Guests.Exists(id));
        Assert.AreEqual(2, harness.Store.RetentionRuns.Count);
        Assert.IsTrue(harness.Store.RetentionRuns.All(run => run.Outcome == RetentionOutcomes.Failed));
    }
}
