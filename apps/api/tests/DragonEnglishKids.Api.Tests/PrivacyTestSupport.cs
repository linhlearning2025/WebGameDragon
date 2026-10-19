using System.Security.Claims;
using System.Text.Encodings.Web;
using DragonEnglishKids.Api.Data.Privacy;
using DragonEnglishKids.Api.Privacy;
using DragonEnglishKids.Api.Privacy.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DragonEnglishKids.Api.Tests;

internal sealed class TestTimeProvider(DateTimeOffset start) : TimeProvider
{
    private DateTimeOffset _now = start;

    public override DateTimeOffset GetUtcNow() => _now;

    public void Advance(TimeSpan amount) => _now += amount;
}

internal sealed class TestHostEnvironment(string environmentName) : IHostEnvironment
{
    public string EnvironmentName { get; set; } = environmentName;

    public string ApplicationName { get; set; } = "tests";

    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}

internal sealed class FakeChildAccessAuthorizer : IChildAccessAuthorizer
{
    private readonly HashSet<(Guid ParentId, Guid ChildId)> _owned = [];

    public void Grant(Guid parentId, Guid childId) => _owned.Add((parentId, childId));

    public Task<bool> OwnsChildAsync(Guid parentId, Guid childId, CancellationToken cancellationToken) =>
        Task.FromResult(_owned.Contains((parentId, childId)));
}

internal sealed class FakeParentVerifier : IParentVerifier
{
    public bool Verified { get; set; }

    public Task<bool> IsVerifiedAsync(PrivacyActor actor, Guid childId, CancellationToken cancellationToken) =>
        Task.FromResult(Verified);
}

internal sealed class InMemoryPrivacyStore : IPrivacyStore
{
    private readonly List<ConsentRecord> _pendingConsents = [];
    private readonly List<SpeechSession> _pendingSessions = [];
    private readonly List<AuditEvent> _pendingAudits = [];
    private readonly List<RetentionRun> _pendingRuns = [];
    private readonly List<PrivacyRequest> _pendingRequests = [];

    public List<ConsentRecord> Consents { get; } = [];

    public List<SpeechSession> SpeechSessions { get; } = [];

    public List<AuditEvent> AuditEvents { get; } = [];

    public List<RetentionRun> RetentionRuns { get; } = [];

    public List<PrivacyRequest> PrivacyRequests { get; } = [];

    public bool Unavailable { get; set; }

    public Task<ConsentRecord?> FindConsentByIdempotencyKeyAsync(
        Guid parentId,
        Guid childId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        EnsureAvailable();
        return Task.FromResult(Consents.FirstOrDefault(
            x => x.ParentId == parentId && x.ChildId == childId && x.IdempotencyKey == idempotencyKey));
    }

    public Task<ConsentRecord?> GetLatestConsentAsync(
        Guid childId,
        string purpose,
        CancellationToken cancellationToken)
    {
        EnsureAvailable();
        return Task.FromResult(Consents
            .Where(x => x.ChildId == childId && x.Purpose == purpose)
            .OrderByDescending(x => x.DecidedAt)
            .FirstOrDefault());
    }

    public Task<IReadOnlyList<ConsentRecord>> ListConsentsAsync(Guid childId, CancellationToken cancellationToken)
    {
        EnsureAvailable();
        return Task.FromResult<IReadOnlyList<ConsentRecord>>(
            Consents.Where(x => x.ChildId == childId).OrderByDescending(x => x.DecidedAt).ToList());
    }

    public Task<ConsentRecord?> GetConsentAsync(Guid consentRecordId, CancellationToken cancellationToken)
    {
        EnsureAvailable();
        return Task.FromResult(Consents.FirstOrDefault(x => x.Id == consentRecordId));
    }

    public Task<SpeechSession?> GetSpeechSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        EnsureAvailable();
        return Task.FromResult(SpeechSessions.FirstOrDefault(x => x.Id == sessionId));
    }

    public Task<IReadOnlyList<SpeechSession>> GetActiveSpeechSessionsAsync(
        Guid childId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        EnsureAvailable();
        return Task.FromResult<IReadOnlyList<SpeechSession>>(SpeechSessions
            .Where(x => x.ChildId == childId && x.RevokedAt is null && x.ExpiresAt > now)
            .ToList());
    }

    public void AddConsent(ConsentRecord record) => _pendingConsents.Add(record);

    public void AddSpeechSession(SpeechSession session) => _pendingSessions.Add(session);

    public void AddAudit(AuditEvent auditEvent) => _pendingAudits.Add(auditEvent);

    public void AddRetentionRun(RetentionRun run) => _pendingRuns.Add(run);

    public Task<PrivacyRequest?> FindPrivacyRequestByIdempotencyKeyAsync(
        Guid parentId,
        Guid childId,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        EnsureAvailable();
        return Task.FromResult(PrivacyRequests.FirstOrDefault(
            x => x.ParentId == parentId && x.ChildId == childId && x.IdempotencyKey == idempotencyKey));
    }

    public Task<PrivacyRequest?> GetPrivacyRequestAsync(Guid requestId, CancellationToken cancellationToken)
    {
        EnsureAvailable();
        return Task.FromResult(PrivacyRequests.FirstOrDefault(x => x.Id == requestId));
    }

    public void AddPrivacyRequest(PrivacyRequest request) => _pendingRequests.Add(request);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        EnsureAvailable();
        Consents.AddRange(_pendingConsents);
        SpeechSessions.AddRange(_pendingSessions);
        AuditEvents.AddRange(_pendingAudits);
        RetentionRuns.AddRange(_pendingRuns);
        _pendingRuns.Clear();
        PrivacyRequests.AddRange(_pendingRequests);
        _pendingRequests.Clear();
        _pendingConsents.Clear();
        _pendingSessions.Clear();
        _pendingAudits.Clear();
        return Task.CompletedTask;
    }

    private void EnsureAvailable()
    {
        if (Unavailable)
        {
            throw new InvalidOperationException("Privacy store unavailable (test).");
        }
    }
}

internal sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";
    public const string ParentHeader = "X-Test-Parent";
    public const string RoleHeader = "X-Test-Role";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var parent = Request.Headers[ParentHeader].ToString();
        var role = Request.Headers[RoleHeader].ToString();
        if (string.IsNullOrEmpty(parent) && string.IsNullOrEmpty(role))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>();
        if (!string.IsNullOrEmpty(parent))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, parent));
        }

        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

internal sealed class PrivacyTestApp : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    public PrivacyTestApp(bool speechEnabled = true, string? realConnectionString = null)
    {
        Time = new TestTimeProvider(new DateTimeOffset(2026, 10, 19, 8, 0, 0, TimeSpan.Zero));
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting(
                "ConnectionStrings:Default",
                realConnectionString ?? "Host=localhost;Database=dev;Username=dev;Password=test-secret");
            builder.UseSetting("Privacy:AuditPseudonymKey", "test-only-audit-key");
            builder.UseSetting("Privacy:SpeechEnabled", speechEnabled ? "true" : "false");
            builder.ConfigureLogging(logging => logging.AddProvider(Logs));
            builder.ConfigureServices(services =>
            {
                if (realConnectionString is null)
                {
                    services.RemoveAll<IPrivacyStore>();
                    services.AddSingleton<IPrivacyStore>(Store);
                }
                services.RemoveAll<IChildAccessAuthorizer>();
                services.AddSingleton<IChildAccessAuthorizer>(Authorizer);
                services.RemoveAll<IParentVerifier>();
                services.AddSingleton<IParentVerifier>(Verifier);
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(Time);
                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                        options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
            });
        });
    }

    public InMemoryPrivacyStore Store { get; } = new();

    public CapturingLoggerProvider Logs { get; } = new();

    public IServiceProvider Services => _factory.Services;

    public FakeChildAccessAuthorizer Authorizer { get; } = new();

    public FakeParentVerifier Verifier { get; } = new();

    public TestTimeProvider Time { get; }

    public HttpClient CreateClient(Guid? parentId = null, string? role = null)
    {
        var client = _factory.CreateClient();
        if (parentId is { } id)
        {
            client.DefaultRequestHeaders.Add(TestAuthHandler.ParentHeader, id.ToString());
        }

        if (role is not null)
        {
            client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role);
        }

        return client;
    }

    public void Dispose() => _factory.Dispose();
}
