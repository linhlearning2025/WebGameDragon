using System.Net.Http.Json;
using System.Text.Json;
using DragonEnglishKids.Api.Privacy.Consent;

namespace DragonEnglishKids.Api.Tests;

internal static class PrivacyTestHelpers
{
    public static string CurrentNoticeVersion => new NoticeCatalog(NoticeCatalog.Default).CurrentVersion;

    public static async Task<HttpResponseMessage> PutConsentAsync(
        HttpClient client,
        Guid childId,
        string decision,
        string? noticeVersion = null,
        string? idempotencyKey = null)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/v1/children/{childId}/consents/{Purposes.MicrophoneSpeech}")
        {
            Content = JsonContent.Create(new { decision, noticeVersion = noticeVersion ?? CurrentNoticeVersion })
        };
        if (idempotencyKey is not null)
        {
            request.Headers.Add("Idempotency-Key", idempotencyKey);
        }

        return await client.SendAsync(request);
    }

    public static Task<HttpResponseMessage> StartSpeechAsync(HttpClient client, Guid childId) =>
        client.PostAsync($"/api/v1/children/{childId}/speech-sessions", content: null);

    public static async Task<HttpResponseMessage> CreatePrivacyRequestAsync(
        HttpClient client,
        Guid childId,
        string type,
        string? idempotencyKey = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/children/{childId}/privacy-requests")
        {
            Content = JsonContent.Create(new { type })
        };
        if (idempotencyKey is not null)
        {
            request.Headers.Add("Idempotency-Key", idempotencyKey);
        }

        return await client.SendAsync(request);
    }

    public static Task<HttpResponseMessage> HeartbeatAsync(HttpClient client, Guid childId, Guid sessionId) =>
        client.PostAsync($"/api/v1/children/{childId}/speech-sessions/{sessionId}/heartbeat", content: null);

    public static async Task<string?> ProblemTypeAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.TryGetProperty("type", out var type) ? type.GetString() : null;
    }

    public static async Task<JsonElement> JsonAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }
}
