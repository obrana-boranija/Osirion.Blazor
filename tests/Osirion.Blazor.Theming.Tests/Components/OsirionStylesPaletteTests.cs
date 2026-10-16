using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Components;

public class OsirionStylesPaletteTests : BunitContext
{
    private const string PaletteStylesheet = "_content/Osirion.Blazor.Theming/css/palettes.min.css";

    private readonly IThemeService _themeService = Substitute.For<IThemeService>();
    private readonly ThemingOptions _options = new();

    public OsirionStylesPaletteTests()
    {
        Services.AddSingleton(Options.Create(_options));
        Services.AddSingleton(_themeService);
        SetRendererInfo(new RendererInfo("Static", false));
        ComponentFactories.AddStub<BlazorJSComponents.JS>();
        _themeService.GenerateThemeVariables().Returns(string.Empty);
        _themeService.GetFrameworkClass().Returns(string.Empty);
        _themeService.CurrentMode.Returns(ThemeMode.Light);
        _themeService.CurrentPalette.Returns(ThemePalette.Framework);
    }

    [Fact]
    public void OsirionStyles_NoPaletteInUse_DoesNotLinkThePaletteStylesheet()
    {
        // Act
        var cut = Render<OsirionStyles>();

        // Assert
        cut.FindAll("link[data-osirion-palettes]").ShouldBeEmpty();
    }

    [Fact]
    public void OsirionStyles_VisitorPaletteInUse_LinksThePaletteStylesheet()
    {
        // Arrange
        _themeService.CurrentPalette.Returns(ThemePalette.Commit);

        // Act
        var cut = Render<OsirionStyles>();

        // Assert
        cut.Find("link[data-osirion-palettes]").GetAttribute("href").ShouldBe(PaletteStylesheet);
    }

    [Fact]
    public void OsirionStyles_PaletteSelectionAllowed_LinksThePaletteStylesheetOnEveryPage()
    {
        // Arrange
        _options.AllowPaletteSelection = true;

        // Act
        var cut = Render<OsirionStyles>();

        // Assert
        cut.FindAll("link[data-osirion-palettes]").Count.ShouldBe(1);
    }

    [Fact]
    public void OsirionStyles_DefaultStylesOn_StillLinksThePaletteStylesheet()
    {
        // Arrange
        _options.UseDefaultStyles = true;
        _themeService.CurrentPalette.Returns(ThemePalette.Contrast);

        // Act
        var cut = Render<OsirionStyles>();

        // Assert
        cut.FindAll("link[data-osirion-palettes]").Count.ShouldBe(1);
    }

    [Theory]
    [InlineData(ThemePalette.Framework, "framework")]
    [InlineData(ThemePalette.StudioBlue, "studio-blue")]
    public void OsirionStyles_DefaultPalette_IsHandedToTheFirstPaintScript(ThemePalette palette, string id)
    {
        // Arrange
        _options.DefaultPalette = palette;

        // Act
        var cut = Render<OsirionStyles>();

        // Assert
        var script = cut.Find("script").TextContent;
        script.ShouldContain($"defaultPalette: '{id}'");
        script.ShouldContain("osirion-preferred-palette");
    }
}
