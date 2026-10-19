using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy.Consent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class ConsentEvaluatorTests
{
    private static ConsentRecord Record(string decision, string version) => new()
    {
        Id = Guid.NewGuid(),
        ChildId = Guid.NewGuid(),
        Purpose = Purposes.MicrophoneSpeech,
        Decision = decision,
        NoticeVersion = version
    };

    private static ConsentEvaluator Evaluator(params NoticeVersion[] versions) => new(new NoticeCatalog(versions));

    [TestMethod]
    public void GrantedOnCurrentNoticeIsValid()
    {
        var evaluator = Evaluator(new NoticeVersion("v1", Material: true));

        Assert.IsTrue(evaluator.IsValid(Record(Decisions.Granted, "v1")));
    }

    [TestMethod]
    public void DeniedWithdrawnAndMissingConsentAreNeverValid()
    {
        var evaluator = Evaluator(new NoticeVersion("v1", Material: true));

        Assert.IsFalse(evaluator.IsValid(Record(Decisions.Denied, "v1")));
        Assert.IsFalse(evaluator.IsValid(Record(Decisions.Withdrawn, "v1")));
        Assert.IsFalse(evaluator.IsValid(null));
    }

    [TestMethod]
    public void MaterialNoticeChangeInvalidatesEarlierConsent()
    {
        var evaluator = Evaluator(new NoticeVersion("v1", Material: true), new NoticeVersion("v2", Material: true));

        Assert.IsFalse(evaluator.IsValid(Record(Decisions.Granted, "v1")));
        Assert.IsTrue(evaluator.IsValid(Record(Decisions.Granted, "v2")));
    }

    [TestMethod]
    public void NonMaterialNoticeChangeKeepsEarlierConsentValid()
    {
        var evaluator = Evaluator(new NoticeVersion("v1", Material: true), new NoticeVersion("v1-edit", Material: false));

        Assert.IsTrue(evaluator.IsValid(Record(Decisions.Granted, "v1")));
    }

    [TestMethod]
    public void UnknownNoticeVersionIsNotConsent()
    {
        var evaluator = Evaluator(new NoticeVersion("v1", Material: true));

        Assert.IsFalse(evaluator.IsValid(Record(Decisions.Granted, "unknown")));
    }
}
