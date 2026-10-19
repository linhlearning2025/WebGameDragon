using System.Net;
using DragonEnglishKids.Api.Privacy.Consent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class NoticeEndpointTests
{
    [TestMethod]
    public async Task CurrentNoticeAndVersionAreReturnedWithoutAuthentication()
    {
        using var app = new PrivacyTestApp();
        using var client = app.CreateClient();

        using var response = await client.GetAsync($"/api/v1/privacy/notice?purpose={Purposes.MicrophoneSpeech}");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var body = await PrivacyTestHelpers.JsonAsync(response);
        Assert.AreEqual(PrivacyTestHelpers.CurrentNoticeVersion, body.GetProperty("noticeVersion").GetString());
        StringAssert.Contains(body.GetProperty("content").GetString(), "PLACEHOLDER");
    }

    [TestMethod]
    public async Task UnsupportedPurposeIsRejected()
    {
        using var app = new PrivacyTestApp();
        using var client = app.CreateClient();

        using var response = await client.GetAsync("/api/v1/privacy/notice?purpose=unknown");

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
