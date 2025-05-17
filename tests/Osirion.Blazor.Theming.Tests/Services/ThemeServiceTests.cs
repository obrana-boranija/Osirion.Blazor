using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Services;

public class ThemeServiceTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultMode()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light });

        // Act
        var service = new ThemeService(options);

        // Assert
        service.CurrentMode.ShouldBe(ThemeMode.Light);
    }

    [Fact]
    public void SetThemeMode_ShouldChangeCurrentMode()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light, EnableDarkMode = true });
        var service = new ThemeService(options);

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        service.CurrentMode.ShouldBe(ThemeMode.Dark);
    }

    [Fact]
    public void SetThemeMode_ShouldNotChangeToDarkMode_WhenDarkModeDisabled()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light, EnableDarkMode = false });
        var service = new ThemeService(options);

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        service.CurrentMode.ShouldBe(ThemeMode.Light);
    }

    [Fact]
    public void SetThemeMode_ShouldRaiseThemeChangedEvent()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light, EnableDarkMode = true });
        var service = new ThemeService(options);

        ThemeChangedEventArgs? eventArgs = null;
        service.ThemeChanged += (sender, args) => eventArgs = args;

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        eventArgs.ShouldNotBeNull();
        eventArgs.NewMode.ShouldBe(ThemeMode.Dark);
        eventArgs.PreviousMode.ShouldBe(ThemeMode.Light);
    }

    [Fact]
    public void SetThemeMode_ShouldNotRaiseThemeChangedEvent_WhenModeUnchanged()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light, EnableDarkMode = true });
        var service = new ThemeService(options);

        bool eventRaised = false;
        service.ThemeChanged += (sender, args) => eventRaised = true;

        // Act
        service.SetThemeMode(ThemeMode.Light); // Same as current mode

        // Assert
        eventRaised.ShouldBeFalse();
    }

    [Fact]
    public void CurrentMode_PropertySetter_ShouldCallSetThemeMode()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light, EnableDarkMode = true });
        var service = new ThemeService(options);

        ThemeChangedEventArgs? eventArgs = null;
        service.ThemeChanged += (sender, args) => eventArgs = args;

        // Act
        service.CurrentMode = ThemeMode.Dark;

        // Assert
        service.CurrentMode.ShouldBe(ThemeMode.Dark);
        eventArgs.ShouldNotBeNull();
        eventArgs.NewMode.ShouldBe(ThemeMode.Dark);
    }

    [Fact]
    public void GetFrameworkClass_ShouldReturnCorrectClass_ForBootstrap()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { Framework = CssFramework.Bootstrap });
        var service = new ThemeService(options);

        // Act
        var result = service.GetFrameworkClass();

        // Assert
        result.ShouldBe("osirion-bootstrap-integration");
    }

    [Fact]
    public void GetFrameworkClass_ShouldReturnCorrectClass_ForFluentUI()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { Framework = CssFramework.FluentUI });
        var service = new ThemeService(options);

        // Act
        var result = service.GetFrameworkClass();

        // Assert
        result.ShouldBe("osirion-fluent-integration");
    }

    [Fact]
    public void GetFrameworkClass_ShouldReturnCorrectClass_ForMudBlazor()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { Framework = CssFramework.MudBlazor });
        var service = new ThemeService(options);

        // Act
        var result = service.GetFrameworkClass();

        // Assert
        result.ShouldBe("osirion-mudblazor-integration");
    }

    [Fact]
    public void GetFrameworkClass_ShouldReturnCorrectClass_ForRadzen()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { Framework = CssFramework.Radzen });
        var service = new ThemeService(options);

        // Act
        var result = service.GetFrameworkClass();

        // Assert
        result.ShouldBe("osirion-radzen-integration");
    }

    [Fact]
    public void GetFrameworkClass_ShouldReturnEmptyString_ForNone()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions { Framework = CssFramework.None });
        var service = new ThemeService(options);

        // Act
        var result = service.GetFrameworkClass();

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void GenerateThemeVariables_ShouldIncludeCustomVariables()
    {
        // Arrange
        var options = Options.Create(new ThemingOptions
        {
            DefaultMode = ThemeMode.Light,
            CustomVariables = "--custom-color: blue;"
        });
        var service = new ThemeService(options);

        // Act
        var variables = service.GenerateThemeVariables();

        // Assert
        variables.ShouldContain("--custom-color: blue;");
    }
}