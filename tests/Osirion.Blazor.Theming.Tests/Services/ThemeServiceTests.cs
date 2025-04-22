using Osirion.Blazor.Theming.Options;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Services;

public class ThemeServiceTests
{
    [Fact]
    public void CurrentMode_ShouldReturnDefaultMode_WhenInitialized()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Dark });
        var service = new ThemeService(options);

        // Act & Assert
        service.CurrentMode.ShouldBe(ThemeMode.Dark);
    }

    [Fact]
    public void SetThemeMode_ShouldRaiseThemeChangedEvent()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ThemingOptions());
        var service = new ThemeService(options);
        ThemeChangedEventArgs? eventArgs = null;
        service.ThemeChanged += (s, e) => eventArgs = e;

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        eventArgs.ShouldNotBeNull();
        eventArgs.NewMode.ShouldBe(ThemeMode.Dark);
        eventArgs.PreviousMode.ShouldBe(ThemeMode.Light);
    }

    [Fact]
    public void GenerateThemeVariables_ShouldIncludeFrameworkVariables_WhenFrameworkSet()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ThemingOptions { Framework = CssFramework.Bootstrap });
        var service = new ThemeService(options);

        // Act
        var variables = service.GenerateThemeVariables();

        // Assert
        variables.ShouldContain("--bs-primary");
    }

    [Fact]
    public void GenerateThemeVariables_ShouldIncludeDarkModeVariables_WhenDarkMode()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ThemingOptions());
        var service = new ThemeService(options);
        service.SetThemeMode(ThemeMode.Dark);

        // Act
        var variables = service.GenerateThemeVariables();

        // Assert
        variables.ShouldContain("#60a5fa"); // Dark mode primary color
    }
}