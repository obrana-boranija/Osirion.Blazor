using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Osirion.Blazor.Cms.Web.Tests.Base;

/// <summary>
/// Collects every log entry written through any logger it creates, so a test can assert that a failure was logged.
/// </summary>
internal sealed class RecordingLoggerFactory : ILoggerFactory
{
    private readonly RecordingLogger _logger = new();

    /// <summary>Gets the entries written so far, in order.</summary>
    public IReadOnlyList<(LogLevel Level, Exception? Exception)> Entries => _logger.Entries;

    /// <summary>
    /// Registers this factory and the generic logger on top of it, replacing the null loggers the test context
    /// registers, so both <see cref="ILogger{TCategoryName}"/> and <see cref="ILoggerFactory"/> injections record.
    /// </summary>
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<ILoggerFactory>(this);
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
    }

    public void AddProvider(ILoggerProvider provider)
    {
    }

    public ILogger CreateLogger(string categoryName) => _logger;

    public void Dispose()
    {
    }

    private sealed class RecordingLogger : ILogger
    {
        public List<(LogLevel Level, Exception? Exception)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, exception));
    }
}
