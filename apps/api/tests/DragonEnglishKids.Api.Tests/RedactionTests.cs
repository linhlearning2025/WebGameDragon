using System.Net.Http.Headers;
using System.Text;
using DragonEnglishKids.Api.Privacy.Consent;
using DragonEnglishKids.Api.Privacy.Redaction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DragonEnglishKids.Api.Tests;

[TestClass]
public sealed class RedactionTests
{
    [TestMethod]
    public void SanitizeRemovesTokensSecretsAndIdentifiers()
    {
        var childId = Guid.NewGuid();

        var result = RedactionPolicy.Sanitize($"child {childId} Bearer abc.def pin=1234 transcript: hello");

        Assert.IsFalse(result.Contains(childId.ToString(), StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(result.Contains("abc.def", StringComparison.Ordinal));
        Assert.IsFalse(result.Contains("1234", StringComparison.Ordinal));
        Assert.IsFalse(result.Contains("hello", StringComparison.Ordinal));
        Assert.AreEqual(string.Empty, RedactionPolicy.Sanitize(null));
    }

    [TestMethod]
    public void ConsoleFormatterSanitizesMessagesAndNeverWritesExceptionText()
    {
        var childId = Guid.NewGuid();
        var entry = new LogEntry<string>(
            LogLevel.Warning,
            "Category",
            new EventId(1),
            "state",
            new InvalidOperationException("password=secret-value"),
            (_, _) => $"failure for {childId} Bearer token123");
        using var writer = new StringWriter();

        new RedactingConsoleFormatter().Write(entry, null, writer);

        var output = writer.ToString();
        Assert.IsFalse(output.Contains(childId.ToString(), StringComparison.OrdinalIgnoreCase));
        Assert.IsFalse(output.Contains("token123", StringComparison.Ordinal));
        Assert.IsFalse(output.Contains("secret-value", StringComparison.Ordinal));
        StringAssert.Contains(output, "InvalidOperationException");
    }

    [TestMethod]
    public async Task RepresentativeFlowsLeaveNoSensitiveDataInLogsResponsesOrAuditRows()
    {
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        using var app = new PrivacyTestApp();
        app.Authorizer.Grant(parentId, childId);
        using var owner = app.CreateClient(parentId);
        using var guest = app.CreateClient(role: "guest");
        using var stranger = app.CreateClient(Guid.NewGuid());
        var bodies = new List<string>();

        async Task Capture(Task<HttpResponseMessage> request)
        {
            using var response = await request;
            bodies.Add(await response.Content.ReadAsStringAsync());
        }

        await Capture(PrivacyTestHelpers.PutConsentAsync(owner, childId, Decisions.Granted, idempotencyKey: "k1"));
        var start = await PrivacyTestHelpers.StartSpeechAsync(owner, childId);
        var session = (await PrivacyTestHelpers.JsonAsync(start)).GetProperty("speechSessionId").GetGuid();
        await Capture(PrivacyTestHelpers.HeartbeatAsync(owner, childId, session));
        await Capture(PrivacyTestHelpers.StartSpeechAsync(guest, childId));
        await Capture(PrivacyTestHelpers.StartSpeechAsync(stranger, childId));
        await Capture(PrivacyTestHelpers.PutConsentAsync(owner, childId, "maybe", idempotencyKey: "k2"));
        await Capture(PrivacyTestHelpers.PutConsentAsync(owner, childId, Decisions.Granted, "old-version", "k3"));
        using var malformed = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/v1/children/{childId}/consents/{Purposes.MicrophoneSpeech}")
        {
            Content = new StringContent("{bad json", Encoding.UTF8, new MediaTypeHeaderValue("application/json"))
        };
        malformed.Headers.Add("Idempotency-Key", "k4");
        await Capture(owner.SendAsync(malformed));
        app.Store.Unavailable = true;
        await Capture(PrivacyTestHelpers.StartSpeechAsync(owner, childId));
        app.Store.Unavailable = false;
        await Capture(PrivacyTestHelpers.PutConsentAsync(owner, childId, Decisions.Withdrawn, idempotencyKey: "k5"));

        var logs = string.Join('\n', app.Logs.Messages);
        var audit = string.Join(
            '\n',
            app.Store.AuditEvents.Select(x => $"{x.ActorRole} {x.Action} {x.SubjectRef} {x.Outcome} {x.CorrelationId}"));
        var everything = string.Join('\n', bodies.Append(logs).Append(audit));

        foreach (var forbidden in new[]
                 {
                     childId.ToString(),
                     childId.ToString("N"),
                     "Bearer",
                     "Password=",
                     "test-secret",
                     "transcript",
                     "   at DragonEnglishKids"
                 })
        {
            Assert.IsFalse(
                everything.Contains(forbidden, StringComparison.OrdinalIgnoreCase),
                $"Found forbidden text: {forbidden}");
        }

        StringAssert.Contains(logs, "Privacy store failure");
    }
}
