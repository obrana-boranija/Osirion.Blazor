using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Navigation.Services;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Services;

public class NavigationServiceTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new NavigationService(null!, options));
    }

    [Fact]
    public void IsEnhancedNavigationEnabled_ShouldReturnTrue()
    {
        // Arrange
        var loggerMock = Substitute.For<ILogger<NavigationService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());
        var service = new NavigationService(loggerMock, options);

        // Act & Assert
        service.IsEnhancedNavigationEnabled.ShouldBeTrue();
    }

    [Fact]
    public async Task ScrollToTopAsync_ShouldLogInformation()
    {
        // Arrange
        var logger = new RecordingLogger();
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());
        var service = new NavigationService(logger, options);

        // Act
        await service.ScrollToTopAsync(ScrollBehavior.Smooth);

        // Assert
        var entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Information);
        entry.Message.ShouldContain("Scrolling to top");
        entry.Values["Behavior"].ShouldBe(ScrollBehavior.Smooth);
    }

    [Fact]
    public async Task ScrollToTopAsync_ShouldUseDefaultBehavior_WhenBehaviorIsNull()
    {
        // Arrange
        var logger = new RecordingLogger();
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions { Behavior = ScrollBehavior.Smooth });
        var service = new NavigationService(logger, options);

        // Act
        await service.ScrollToTopAsync();

        // Assert
        var entry = logger.Entries.ShouldHaveSingleItem();
        entry.Message.ShouldContain("Scrolling to top");
        entry.Values["Behavior"].ShouldBe(ScrollBehavior.Smooth);
    }

    [Fact]
    public async Task ScrollToElementAsync_ShouldLogInformation()
    {
        // Arrange
        var logger = new RecordingLogger();
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());
        var service = new NavigationService(logger, options);

        // Act
        await service.ScrollToElementAsync("test-element", ScrollBehavior.Instant);

        // Assert
        var entry = logger.Entries.ShouldHaveSingleItem();
        entry.Level.ShouldBe(LogLevel.Information);
        entry.Message.ShouldContain("Scrolling to element");
        entry.Values["ElementId"].ShouldBe("test-element");
        entry.Values["Behavior"].ShouldBe(ScrollBehavior.Instant);
    }

    [Fact]
    public async Task ScrollToElementAsync_ShouldUseDefaultBehavior_WhenBehaviorIsNull()
    {
        // Arrange
        var logger = new RecordingLogger();
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions { Behavior = ScrollBehavior.Instant });
        var service = new NavigationService(logger, options);

        // Act
        await service.ScrollToElementAsync("test-element");

        // Assert
        var entry = logger.Entries.ShouldHaveSingleItem();
        entry.Values["ElementId"].ShouldBe("test-element");
        entry.Values["Behavior"].ShouldBe(ScrollBehavior.Instant);
    }

    [Fact]
    public void Constructor_ShouldCreateDefaultOptions_WhenOptionsIsNull()
    {
        // Arrange
        var loggerMock = Substitute.For<ILogger<NavigationService>>();
        IOptions<EnhancedNavigationOptions>? nullOptions = null;

        // Act
        var service = new NavigationService(loggerMock, nullOptions!);

        // Assert - no exception should be thrown
        service.ShouldNotBeNull();
    }

    private sealed record LogEntry(LogLevel Level, string Message, IReadOnlyDictionary<string, object?> Values);

    // LogInformation is an extension method over ILogger.Log<TState>, so a substitute cannot
    // match it; this logger records the level, the formatted message and the template values.
    private sealed class RecordingLogger : ILogger<NavigationService>
    {
        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var values = state as IEnumerable<KeyValuePair<string, object?>> ?? [];
            Entries.Add(new LogEntry(logLevel, formatter(state, exception), values.ToDictionary(pair => pair.Key, pair => pair.Value)));
        }
    }
}
