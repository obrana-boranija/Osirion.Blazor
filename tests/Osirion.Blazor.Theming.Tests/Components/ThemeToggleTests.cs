using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Components;

public class ThemeToggleTests : BunitContext
{
    public ThemeToggleTests()
    {
        var themeService = Substitute.For<IThemeService>();
        themeService.CurrentMode.Returns(ThemeMode.Light);
        Services.AddSingleton(Options.Create(new ThemingOptions()));
        Services.AddSingleton(themeService);
        SetRendererInfo(new RendererInfo("Static", false));
        ComponentFactories.AddStub<BlazorJSComponents.JS>();
    }

    // Fails before the fix: the button had the fixed id "theme-toggle", so two toggles on a page repeated it.
    [Fact]
    public void ThemeToggle_NoIdGiven_WritesNoIdAttribute()
    {
        var cut = Render<ThemeToggle>();

        cut.Find("button.theme-toggle-btn").HasAttribute("id").ShouldBeFalse();
    }

    [Fact]
    public void ThemeToggle_IdGiven_IsWrittenOnTheButton()
    {
        var cut = Render<ThemeToggle>(parameters => parameters.Add(p => p.Id, "header-theme"));

        cut.Find("button.theme-toggle-btn").GetAttribute("id").ShouldBe("header-theme");
    }

    // Fails before the fix: the text used Bootstrap's visually-hidden class, so it showed on MudBlazor and FluentUI sites.
    [Fact]
    public void ThemeToggle_ScreenReaderText_UsesTheOsirionHiddenTextClass()
    {
        var cut = Render<ThemeToggle>();

        cut.Find(".theme-text").ClassList.ShouldContain("osirion-visually-hidden");
    }

    // Fails before the fix: a hidden form posted to /theme-toggle, which no endpoint maps, with no antiforgery token.
    [Fact]
    public void ThemeToggle_Markup_HasNoDeadFormAndNoFixedIds()
    {
        var cut = Render<ThemeToggle>();

        cut.FindAll("form").ShouldBeEmpty();
        cut.FindAll("#scrollPositionInput, #theme-toggle").ShouldBeEmpty();
    }
}
