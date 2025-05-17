using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
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
        var loggerMock = Substitute.For<ILogger<NavigationService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());
        var service = new NavigationService(loggerMock, options);

        // Act
        await service.ScrollToTopAsync(ScrollBehavior.Smooth);

        // Assert
        loggerMock.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Scrolling to top")),
            Arg.Is<ScrollBehavior>(b => b == ScrollBehavior.Smooth)
        );
    }

    [Fact]
    public async Task ScrollToTopAsync_ShouldUseDefaultBehavior_WhenBehaviorIsNull()
    {
        // Arrange
        var loggerMock = Substitute.For<ILogger<NavigationService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions { Behavior = ScrollBehavior.Smooth });
        var service = new NavigationService(loggerMock, options);

        // Act
        await service.ScrollToTopAsync();

        // Assert
        loggerMock.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Scrolling to top")),
            Arg.Is<ScrollBehavior>(b => b == ScrollBehavior.Smooth)
        );
    }

    [Fact]
    public async Task ScrollToElementAsync_ShouldLogInformation()
    {
        // Arrange
        var loggerMock = Substitute.For<ILogger<NavigationService>>();
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());
        var service = new NavigationService(loggerMock, options);

        // Act
        await service.ScrollToElementAsync("test-element", ScrollBehavior.Instant);

        // Assert
        loggerMock.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Scrolling to element")),
            Arg.Is<string>(s => s == "test-element"),
            Arg.Is<ScrollBehavior>(b => b == ScrollBehavior.Auto)
        );
    }

    [Fact]
    public async Task ScrollToElementAsync_ShouldUseDefaultBehavior_WhenBehaviorIsNull()
    {
        // Arrange
        var loggerMock = Substitute.For<ILogger<NavigationService>>();
        // Fix: Replace the incorrect namespace reference for Options.Create
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions { Behavior = ScrollBehavior.Instant });
        var service = new NavigationService(loggerMock, options);

        // Act
        await service.ScrollToElementAsync("test-element");

        // Assert
        loggerMock.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Scrolling to element")),
            Arg.Is<string>(s => s == "test-element"),
            Arg.Is<int>(b => b == (int)ScrollBehavior.Instant)
        );
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
}