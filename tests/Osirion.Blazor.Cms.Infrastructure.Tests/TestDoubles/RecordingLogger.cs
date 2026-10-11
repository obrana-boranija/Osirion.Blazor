using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// A logger that keeps every entry with its level, formatted message and exception. It is safe to write from
/// background work, and a test can wait for an entry that background work writes when it finishes.
/// </summary>
internal sealed class RecordingLogger<T> : ILogger<T>
{
    private readonly object _gate = new();
    private readonly List<LogEntry> _entries = [];
    private readonly List<(LogLevel Level, string MessagePart, TaskCompletionSource Signal)> _waiters = [];

    public IReadOnlyList<LogEntry> Entries
    {
        get
        {
            lock (_gate)
            {
                return [.. _entries];
            }
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var entry = new LogEntry(logLevel, formatter(state, exception), exception);
        List<TaskCompletionSource> released;

        lock (_gate)
        {
            _entries.Add(entry);
            var matching = _waiters.Where(w => entry.Matches(w.Level, w.MessagePart)).ToList();
            _waiters.RemoveAll(matching.Contains);
            released = matching.Select(w => w.Signal).ToList();
        }

        foreach (var signal in released)
        {
            signal.TrySetResult();
        }
    }

    public bool Contains(LogLevel level, string messagePart)
    {
        lock (_gate)
        {
            return _entries.Any(e => e.Matches(level, messagePart));
        }
    }

    /// <summary>
    /// Completes when an entry with <paramref name="level"/> containing <paramref name="messagePart"/> has been
    /// logged, or fails after <paramref name="timeout"/> so a broken test cannot hang the run.
    /// </summary>
    public Task WaitForAsync(LogLevel level, string messagePart, TimeSpan timeout)
    {
        lock (_gate)
        {
            if (_entries.Any(e => e.Matches(level, messagePart)))
            {
                return Task.CompletedTask;
            }

            var signal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _waiters.Add((level, messagePart, signal));
            return signal.Task.WaitAsync(timeout);
        }
    }
}

/// <summary>One recorded log entry.</summary>
internal sealed record LogEntry(LogLevel Level, string Message, Exception? Exception)
{
    public bool Matches(LogLevel level, string messagePart)
    {
        return Level == level && Message.Contains(messagePart, StringComparison.Ordinal);
    }
}
