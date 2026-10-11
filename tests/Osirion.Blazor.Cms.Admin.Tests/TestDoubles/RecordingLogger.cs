using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>One recorded log call: level, formatted message and the exception, if any.</summary>
public sealed record LogEntry(LogLevel Level, string Message, Exception? Exception);

/// <summary>
/// LogError and friends are extension methods over ILogger.Log&lt;TState&gt;, so a substitute cannot
/// match them; this logger records the level, the formatted message and the exception instead.
/// </summary>
public sealed class RecordingLogger<T> : ILogger<T>
{
    public List<LogEntry> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    /// <summary>True when an entry of the level exists whose message contains every fragment.</summary>
    public bool HasEntry(LogLevel level, params string[] fragments) =>
        Entries.Any(entry => entry.Level == level && fragments.All(fragment => entry.Message.Contains(fragment, StringComparison.Ordinal)));

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
    }
}
