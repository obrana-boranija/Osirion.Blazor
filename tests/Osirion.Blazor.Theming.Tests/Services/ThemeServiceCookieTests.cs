using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Services;

public class ThemeServiceCookieTests
{
    private const string ThemeCookie = "osirion-preferred-theme";

    [Theory]
    [InlineData("dark", ThemeMode.Dark)]
    [InlineData("LIGHT", ThemeMode.Light)]
    [InlineData("System", ThemeMode.System)]
    // Fails before the fix: "inverted" was not parsed, so an inverted theme fell back to the default mode.
    [InlineData("inverted", ThemeMode.Inverted)]
    [InlineData("Inverted", ThemeMode.Inverted)]
    public void CurrentMode_ThemeCookiePresent_ReturnsCookieMode(string cookieValue, ThemeMode expected)
    {
        // Arrange
        var service = CreateService(ThemeMode.Light, RequestWithCookies($"{ThemeCookie}={cookieValue}"));

        // Act
        var mode = service.CurrentMode;

        // Assert
        mode.ShouldBe(expected);
    }

    [Fact]
    public void CurrentMode_ThemeCookieUnknown_KeepsDefaultMode()
    {
        // Arrange
        var service = CreateService(ThemeMode.Dark, RequestWithCookies($"{ThemeCookie}=sepia"));

        // Act
        var mode = service.CurrentMode;

        // Assert
        mode.ShouldBe(ThemeMode.Dark);
    }

    [Fact]
    public void CurrentMode_OnlyLegacyCookiePresent_KeepsDefaultMode()
    {
        // Arrange
        var service = CreateService(ThemeMode.Light, RequestWithCookies("preferred-theme=dark"));

        // Act
        var mode = service.CurrentMode;

        // Assert
        mode.ShouldBe(ThemeMode.Light);
    }

    [Fact]
    public void CurrentMode_ReadingHttpContextThrows_KeepsDefaultMode()
    {
        // Arrange
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(_ => throw new InvalidOperationException("No context."));
        var service = new ThemeService(Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Dark }), accessor);

        // Act
        var mode = service.CurrentMode;

        // Assert
        mode.ShouldBe(ThemeMode.Dark);
    }

    [Fact]
    public void Constructor_NullOptions_UsesDefaultThemingOptions()
    {
        // Arrange
        var defaults = new ThemingOptions();

        // Act
        var service = new ThemeService(null!);

        // Assert
        service.CurrentMode.ShouldBe(defaults.DefaultMode);
        service.CurrentFramework.ShouldBe(defaults.Framework);
    }

    [Fact]
    public void CurrentFramework_ReturnsConfiguredFramework()
    {
        // Arrange
        var service = new ThemeService(Options.Create(new ThemingOptions { Framework = CssFramework.Radzen }));

        // Act
        var framework = service.CurrentFramework;

        // Assert
        framework.ShouldBe(CssFramework.Radzen);
    }

    [Fact]
    public void GenerateThemeVariables_NoCustomVariables_ReturnsEmptyString()
    {
        // Arrange
        var service = new ThemeService(Options.Create(new ThemingOptions { CustomVariables = "   " }));

        // Act
        var variables = service.GenerateThemeVariables();

        // Assert
        variables.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(ThemeMode.Dark, "dark")]
    [InlineData(ThemeMode.System, "system")]
    // Fails before the fix: Inverted wrote no cookie, so the choice did not survive the next request.
    [InlineData(ThemeMode.Inverted, "inverted")]
    public void SetThemeMode_HttpContextAvailable_WritesThemeCookie(ThemeMode mode, string expectedValue)
    {
        // Arrange
        var context = new DefaultHttpContext();
        var service = CreateService(ThemeMode.Light, context);

        // Act
        service.SetThemeMode(mode);

        // Assert
        var setCookie = SetCookieHeader(context);
        setCookie.ShouldStartWith($"{ThemeCookie}={expectedValue}");
        setCookie.ShouldContain("path=/");
        setCookie.ShouldContain("samesite=lax");
        setCookie.ShouldContain("expires=");
        setCookie.ShouldNotContain("httponly");
        setCookie.ShouldContain("secure");
    }

    [Fact]
    public void SetThemeMode_LightFromDark_WritesLightCookie()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var service = CreateService(ThemeMode.Dark, context);

        // Act
        service.SetThemeMode(ThemeMode.Light);

        // Assert
        SetCookieHeader(context).ShouldStartWith($"{ThemeCookie}=light");
    }

    [Fact]
    public void SetThemeMode_HttpsRequest_WritesSecureCookie()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.IsHttps = true;
        var service = CreateService(ThemeMode.Light, context);

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        SetCookieHeader(context).ShouldContain("secure");
    }

    [Fact]
    public void SetThemeMode_LegacyCookiePresent_DeletesLegacyCookie()
    {
        // Arrange
        var context = RequestWithCookies("preferred-theme=dark");
        var service = CreateService(ThemeMode.Light, context);

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        var headers = context.Response.Headers.SetCookie.ToArray();
        headers.ShouldContain(h => h!.StartsWith($"{ThemeCookie}=dark", StringComparison.Ordinal));
        headers.ShouldContain(h => h!.StartsWith("preferred-theme=;", StringComparison.Ordinal));
    }

    [Fact]
    public void SetThemeMode_NoLegacyCookie_WritesOnlyThemeCookie()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var service = CreateService(ThemeMode.Light, context);

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        context.Response.Headers.SetCookie.Count.ShouldBe(1);
    }

    [Fact]
    public void SetThemeMode_UndefinedMode_ChangesModeWithoutWritingCookie()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var service = CreateService(ThemeMode.Light, context);
        var undefinedMode = (ThemeMode)42;

        // Act
        service.SetThemeMode(undefinedMode);

        // Assert
        service.CurrentMode.ShouldBe(undefinedMode);
        context.Response.Headers.SetCookie.Count.ShouldBe(0);
    }

    [Fact]
    public void SetThemeMode_NoHttpContext_ChangesModeAndRaisesEvent()
    {
        // Arrange
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns((HttpContext?)null);
        var service = new ThemeService(
            Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light, EnableDarkMode = true }),
            accessor);
        ThemeChangedEventArgs? raised = null;
        service.ThemeChanged += (_, args) => raised = args;

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        service.CurrentMode.ShouldBe(ThemeMode.Dark);
        raised.ShouldNotBeNull();
        raised.NewMode.ShouldBe(ThemeMode.Dark);
    }

    [Fact]
    public void SetThemeMode_WritingCookieThrows_StillChangesModeAndRaisesEvent()
    {
        // Arrange
        var context = Substitute.For<HttpContext>();
        context.Request.Returns(new DefaultHttpContext().Request);
        context.Response.Returns(_ => throw new InvalidOperationException("Response already started."));
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(context);
        var service = new ThemeService(
            Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light, EnableDarkMode = true }),
            accessor);
        var raised = false;
        service.ThemeChanged += (_, _) => raised = true;

        // Act
        service.SetThemeMode(ThemeMode.Dark);

        // Assert
        service.CurrentMode.ShouldBe(ThemeMode.Dark);
        raised.ShouldBeTrue();
    }

    [Theory]
    [InlineData(ThemeMode.Light, "dark", ThemeMode.Dark)]
    [InlineData(ThemeMode.Dark, "light", ThemeMode.Light)]
    [InlineData(ThemeMode.System, "dark", ThemeMode.Dark)]
    public void ToggleTheme_FromMode_SwitchesToOppositeMode(ThemeMode start, string expectedName, ThemeMode expectedMode)
    {
        // Arrange
        var service = new ThemeService(Options.Create(new ThemingOptions { DefaultMode = start, EnableDarkMode = true }));

        // Act
        var result = service.ToggleTheme();

        // Assert
        result.ShouldBe(expectedName);
        service.CurrentMode.ShouldBe(expectedMode);
    }

    [Fact]
    public void ToggleTheme_DarkModeDisabled_KeepsAndReportsLightMode()
    {
        // Arrange
        var service = new ThemeService(Options.Create(new ThemingOptions { DefaultMode = ThemeMode.Light, EnableDarkMode = false }));

        // Act
        var result = service.ToggleTheme();

        // Assert
        result.ShouldBe("light");
        service.CurrentMode.ShouldBe(ThemeMode.Light);
    }

    private static ThemeService CreateService(ThemeMode defaultMode, HttpContext context)
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(context);
        var options = Options.Create(new ThemingOptions { DefaultMode = defaultMode, EnableDarkMode = true });
        return new ThemeService(options, accessor);
    }

    private static DefaultHttpContext RequestWithCookies(string cookieHeader)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = cookieHeader;
        return context;
    }

    private static string SetCookieHeader(HttpContext context) =>
        context.Response.Headers.SetCookie.ToString().ToLowerInvariant();
}
