using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Services;

public class AutoThemeServiceTests
{
    private readonly ICssFrameworkDetector _detector = Substitute.For<ICssFrameworkDetector>();

    [Fact]
    public void CurrentFramework_FrameworkConfigured_ReturnsItWithoutDetecting()
    {
        // Arrange
        var service = CreateService(CssFramework.MudBlazor);

        // Act
        var framework = service.CurrentFramework;

        // Assert
        framework.ShouldBe(CssFramework.MudBlazor);
        _ = _detector.DidNotReceive().DetectFrameworkAsync();
    }

    [Fact]
    public void CurrentFramework_NoFrameworkConfigured_ReturnsDetectedFramework()
    {
        // Arrange
        _detector.DetectFrameworkAsync().Returns(CssFramework.Bootstrap);
        var service = CreateService(CssFramework.None);

        // Act
        var framework = service.CurrentFramework;

        // Assert
        framework.ShouldBe(CssFramework.Bootstrap);
    }

    [Fact]
    public void CurrentFramework_ReadTwice_DetectsOnlyOnce()
    {
        // Arrange
        _detector.DetectFrameworkAsync().Returns(CssFramework.FluentUI);
        var service = CreateService(CssFramework.None);

        // Act
        _ = service.CurrentFramework;
        var second = service.CurrentFramework;

        // Assert
        second.ShouldBe(CssFramework.FluentUI);
        _ = _detector.Received(1).DetectFrameworkAsync();
    }

    [Fact]
    public void GetFrameworkClass_NoFrameworkConfigured_UsesDetectedFramework()
    {
        // Arrange
        _detector.DetectFrameworkAsync().Returns(CssFramework.Radzen);
        var service = CreateService(CssFramework.None);

        // Act
        var cssClass = service.GetFrameworkClass();

        // Assert
        cssClass.ShouldBe("osirion-radzen-integration");
    }

    [Fact]
    public void CurrentFramework_NothingDetected_ReturnsNone()
    {
        // Arrange
        _detector.DetectFrameworkAsync().Returns(CssFramework.None);
        var service = CreateService(CssFramework.None);

        // Act
        var framework = service.CurrentFramework;

        // Assert
        framework.ShouldBe(CssFramework.None);
    }

    private AutoThemeService CreateService(CssFramework framework) =>
        new(Options.Create(new ThemingOptions { Framework = framework }), _detector);
}
