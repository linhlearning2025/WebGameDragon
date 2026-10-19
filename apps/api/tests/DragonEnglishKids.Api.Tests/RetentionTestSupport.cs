using System.Collections.Concurrent;
using DragonEnglishKids.Api.Privacy.Audit;
using DragonEnglishKids.Api.Retention;
using Microsoft.Extensions.Logging;

namespace DragonEnglishKids.Api.Tests;

internal sealed class RetentionHarness
{
    public RetentionHarness(int batchSize = RetentionRunner.DefaultBatchSize)
    {
        Time = new TestTimeProvider(new DateTimeOffset(2026, 10, 19, 8, 0, 0, TimeSpan.Zero));
        var loggers = new LoggerFactory([Logs]);
        Runner = new RetentionRunner(
            Guests,
            Store,
            new AuditWriter(Store, new SubjectPseudonymizer("test-key"), Time),
            Time,
            loggers.CreateLogger<RetentionRunner>(),
            batchSize);
    }

    public TestTimeProvider Time { get; }

    public InMemoryPrivacyStore Store { get; } = new();

    public FakeExpiredGuestStore Guests { get; } = new();

    public CapturingLoggerProvider Logs { get; } = new();

    public RetentionRunner Runner { get; }
}

internal sealed class CapturingLoggerProvider : ILoggerProvider
{
    public ConcurrentQueue<string> Messages { get; } = new();

    public ILogger CreateLogger(string categoryName) => new CapturingLogger(this, categoryName);

    public void Dispose()
    {
    }

    private sealed class CapturingLogger(CapturingLoggerProvider owner, string category) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            owner.Messages.Enqueue($"{logLevel} {category}: {formatter(state, exception)} {exception}");
    }
}

internal sealed class FakeExpiredGuestStore : IExpiredGuestDataStore
{
    private readonly Dictionary<Guid, int> _deleteCounts = [];

    public List<FakeGuestSession> Sessions { get; } = [];

    // Called before each delete batch so tests can simulate a failure.
    public Action<int>? BeforeDelete { get; set; }

    public int DeleteCalls { get; private set; }

    public Guid Add(DateTimeOffset expiresAt, bool claimed = false)
    {
        var session = new FakeGuestSession(Guid.NewGuid(), expiresAt, claimed);
        Sessions.Add(session);
        return session.Id;
    }

    public int TimesDeleted(Guid id) => _deleteCounts.GetValueOrDefault(id);

    public bool Exists(Guid id) => Sessions.Any(x => x.Id == id && !x.Deleted);

    public Task<IReadOnlyList<Guid>> GetDeletableSessionIdsAsync(
        DateTimeOffset expiredBefore,
        int batchSize,
        CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Guid>>(Sessions
            .Where(x => !x.Deleted && !x.Claimed && x.ExpiresAt <= expiredBefore)
            .Take(batchSize)
            .Select(x => x.Id)
            .ToList());

    public Task<int> DeleteTemporaryDataAsync(IReadOnlyList<Guid> sessionIds, CancellationToken cancellationToken)
    {
        DeleteCalls++;
        BeforeDelete?.Invoke(DeleteCalls);
        var removed = 0;
        foreach (var session in Sessions.Where(x => sessionIds.Contains(x.Id) && !x.Deleted))
        {
            session.Deleted = true;
            _deleteCounts[session.Id] = _deleteCounts.GetValueOrDefault(session.Id) + 1;
            removed++;
        }

        return Task.FromResult(removed);
    }
}

internal sealed class FakeGuestSession(Guid id, DateTimeOffset expiresAt, bool claimed)
{
    public Guid Id { get; } = id;

    public DateTimeOffset ExpiresAt { get; } = expiresAt;

    public bool Claimed { get; } = claimed;

    public bool Deleted { get; set; }
}
