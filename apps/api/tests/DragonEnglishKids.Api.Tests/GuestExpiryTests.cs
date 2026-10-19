using DragonEnglishKids.Api.Configuration;
using DragonEnglishKids.Api.Retention;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class GuestExpiryTests
{
    private static (GuestExpiryPolicy Policy, TestTimeProvider Time) Create()
    {
        var time = new TestTimeProvider(new DateTimeOffset(2026, 10, 19, 8, 0, 0, TimeSpan.Zero));
        return (new GuestExpiryPolicy(new PrivacyOptions(), time), time);
    }

    [TestMethod]
    public void SessionIsActiveBeforeAndExpiredAtTheConfiguredLifetime()
    {
        var (policy, time) = Create();
        var created = time.GetUtcNow();

        time.Advance(TimeSpan.FromHours(24) - TimeSpan.FromSeconds(1));
        Assert.IsFalse(policy.IsExpired(created));

        time.Advance(TimeSpan.FromSeconds(1));
        Assert.IsTrue(policy.IsExpired(created));
    }

    [TestMethod]
    public void ExpiredSessionNeverBecomesActiveAgain()
    {
        var (policy, time) = Create();
        var created = time.GetUtcNow();
        time.Advance(TimeSpan.FromHours(30));

        Assert.IsTrue(policy.IsExpired(created));

        time.Advance(TimeSpan.FromHours(100));
        Assert.IsTrue(policy.IsExpired(created));
    }

    [TestMethod]
    public void NewSessionAfterExpiryGetsItsOwnFreshLifetime()
    {
        var (policy, time) = Create();
        var old = time.GetUtcNow();
        time.Advance(TimeSpan.FromHours(25));
        var fresh = time.GetUtcNow();

        Assert.IsTrue(policy.IsExpired(old));
        Assert.IsFalse(policy.IsExpired(fresh));
        Assert.AreEqual(fresh.AddHours(24), policy.ExpiresAt(fresh));
    }

    [TestMethod]
    public void TimezoneOffsetsDoNotChangeTheExpiryInstant()
    {
        var (policy, time) = Create();
        var created = time.GetUtcNow();
        var sameInstantElsewhere = created.ToOffset(TimeSpan.FromHours(7));

        Assert.AreEqual(policy.ExpiresAt(created), policy.ExpiresAt(sameInstantElsewhere));
        time.Advance(TimeSpan.FromHours(24));
        Assert.AreEqual(policy.IsExpired(created), policy.IsExpired(sameInstantElsewhere));
    }

    [TestMethod]
    public void DeletionDeadlineFollowsExpiryByTheConfiguredDelay()
    {
        var (policy, time) = Create();
        var created = time.GetUtcNow();

        Assert.AreEqual(created.AddHours(48), policy.DeletionDeadline(created));
    }
}
