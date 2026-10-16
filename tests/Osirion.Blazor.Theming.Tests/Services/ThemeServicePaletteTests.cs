using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Services;

public class ThemeServicePaletteTests
{
    private const string PaletteCookie = "osirion-preferred-palette";

    [Fact]
    public void CurrentPalette_NoCookie_ReturnsTheConfiguredDefault()
    {
        // Arrange
        var service = CreateService(ThemePalette.Editor, new DefaultHttpContext());

        // Act
        var palette = service.CurrentPalette;

        // Assert
        palette.ShouldBe(ThemePalette.Editor);
    }

    [Fact]
    public void CurrentPalette_NothingConfigured_IsFramework()
    {
        // Arrange
        var service = new ThemeService(Options.Create(new ThemingOptions()));

        // Act
        var palette = service.CurrentPalette;

        // Assert
        palette.ShouldBe(ThemePalette.Framework);
    }

    [Theory]
    [InlineData("commit", ThemePalette.Commit)]
    [InlineData("STUDIO-BLUE", ThemePalette.StudioBlue)]
    [InlineData("framework", ThemePalette.Framework)]
    public void CurrentPalette_CookieNamesAPalette_ReturnsIt(string cookieValue, ThemePalette expected)
    {
        // Arrange
        var service = CreateService(ThemePalette.Osirion, RequestWithCookies($"{PaletteCookie}={cookieValue}"));

        // Act
        var palette = service.CurrentPalette;

        // Assert
        palette.ShouldBe(expected);
    }

    [Theory]
    [InlineData("StudioBlue")]
    [InlineData("sepia")]
    [InlineData("commit%22%3E%3Cscript%3E")]
    [InlineData("")]
    public void CurrentPalette_CookieIsNotAPaletteId_KeepsTheDefault(string cookieValue)
    {
        // Arrange
        var service = CreateService(ThemePalette.Contrast, RequestWithCookies($"{PaletteCookie}={cookieValue}"));

        // Act
        var palette = service.CurrentPalette;

        // Assert
        palette.ShouldBe(ThemePalette.Contrast);
    }

    [Fact]
    public void SetPalette_WithAResponse_WritesTheIdToTheCookieAndReportsIt()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var service = CreateService(ThemePalette.Framework, context);
        var raised = 0;
        service.ThemeChanged += (_, _) => raised++;

        // Act
        service.SetPalette(ThemePalette.StudioBlue);

        // Assert
        var header = context.Response.Headers.SetCookie.ToString().ToLowerInvariant();
        header.ShouldContain($"{PaletteCookie}=studio-blue");
        header.ShouldContain("path=/");
        header.ShouldContain("samesite=lax");
        header.ShouldNotContain("httponly");
        service.CurrentPalette.ShouldBe(ThemePalette.StudioBlue);
        raised.ShouldBe(1);
    }

    [Fact]
    public void SetPalette_SamePaletteAgain_RaisesNoChange()
    {
        // Arrange
        var service = CreateService(ThemePalette.Commit, new DefaultHttpContext());
        var raised = 0;
        service.ThemeChanged += (_, _) => raised++;

        // Act
        service.SetPalette(ThemePalette.Commit);

        // Assert
        raised.ShouldBe(0);
    }

    [Fact]
    public void SetPalette_ResponseAlreadyStarted_KeepsThePaletteWithoutACookie()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Features.Set<IHttpResponseFeature>(new StartedResponse());
        var service = CreateService(ThemePalette.Framework, context);

        // Act
        service.SetPalette(ThemePalette.Editor);

        // Assert
        context.Response.Headers.SetCookie.Count.ShouldBe(0);
        service.CurrentPalette.ShouldBe(ThemePalette.Editor);
    }

    [Fact]
    public void SetPalette_ValueOutsideTheEnum_Throws()
    {
        // Arrange
        var service = CreateService(ThemePalette.Framework, new DefaultHttpContext());

        // Act & Assert
        Should.Throw<ArgumentOutOfRangeException>(() => service.SetPalette((ThemePalette)42));
    }

    [Fact]
    public void PaletteMembers_ImplementationWithoutPalettes_ReportFrameworkAndIgnoreChanges()
    {
        // Arrange
        IThemeService legacy = new ServiceWithoutPalettes();

        // Act
        legacy.SetPalette(ThemePalette.Commit);

        // Assert
        legacy.CurrentPalette.ShouldBe(ThemePalette.Framework);
    }

    private static ThemeService CreateService(ThemePalette defaultPalette, HttpContext context)
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(context);
        return new ThemeService(Options.Create(new ThemingOptions { DefaultPalette = defaultPalette }), accessor);
    }

    private static DefaultHttpContext RequestWithCookies(string cookieHeader)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = cookieHeader;
        return context;
    }

    private sealed class StartedResponse : HttpResponseFeature
    {
        public override bool HasStarted => true;
    }

    // An IThemeService written against the interface before palettes were added.
    private sealed class ServiceWithoutPalettes : IThemeService
    {
        public CssFramework CurrentFramework => CssFramework.None;
        public ThemeMode CurrentMode { get; set; }
        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged { add { } remove { } }
        public string GenerateThemeVariables() => string.Empty;
        public void SetThemeMode(ThemeMode mode) => CurrentMode = mode;
        public string GetFrameworkClass() => string.Empty;
        public string ToggleTheme() => "light";
    }
}
