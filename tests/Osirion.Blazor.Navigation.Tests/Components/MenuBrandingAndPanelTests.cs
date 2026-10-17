using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuBrandingAndPanelTests : BunitContext
{
    public MenuBrandingAndPanelTests()
    {
        SetRendererInfo(new RendererInfo("Static", false));
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Menu_BrandingTemplate_ReplacesLogoAndText()
    {
        var cut = Render<Menu>(p => p
            .Add(c => c.BrandLogo, "/logo.svg")
            .Add(c => c.BrandText, "Osirion")
            .Add(c => c.BrandingTemplate, (RenderFragment)(b => b.AddMarkupContent(0, "<strong class=\"custom-brand\">Brand</strong>"))));

        var branding = cut.Find(".osirion-nav-branding");
        branding.QuerySelector("strong.custom-brand").ShouldNotBeNull();
        branding.QuerySelector("img").ShouldBeNull();
        branding.QuerySelector("a").ShouldBeNull();
    }

    [Fact]
    public void Menu_BrandLogoAndText_RendersLogoLinkWithText()
    {
        var cut = Render<Menu>(p => p
            .Add(c => c.Href, "/home")
            .Add(c => c.BrandLogo, "/logo.svg")
            .Add(c => c.BrandText, "Osirion"));

        var link = cut.Find(".osirion-nav-branding > a.osirion-brand-link");
        link.GetAttribute("href").ShouldBe("/home");
        var logo = link.QuerySelector("img.osirion-brand-logo")!;
        logo.GetAttribute("src").ShouldBe("/logo.svg");
        logo.GetAttribute("alt").ShouldBe(string.Empty);
        link.QuerySelector("span.osirion-brand-text")!.TextContent.ShouldBe("Osirion");
    }

    [Fact]
    public void Menu_BrandLogoOnly_RendersLogoWithoutText()
    {
        var cut = Render<Menu>(p => p.Add(c => c.BrandLogo, "/logo.svg"));

        var link = cut.Find(".osirion-nav-branding > a.osirion-brand-link");
        link.QuerySelector("img")!.GetAttribute("src").ShouldBe("/logo.svg");
        link.QuerySelector("span.osirion-brand-text").ShouldBeNull();
    }

    [Fact]
    public void Menu_BrandTextOnly_RendersTextLink()
    {
        var cut = Render<Menu>(p => p.Add(c => c.BrandText, "Osirion"));

        var link = cut.Find(".osirion-nav-branding > a.osirion-brand-link.osirion-brand-text");
        link.TextContent.ShouldBe("Osirion");
        link.GetAttribute("href").ShouldBe("/");
    }

    [Fact]
    public void Menu_NoBranding_RendersAnEmptyBrandingContainer()
    {
        var cut = Render<Menu>();

        cut.Find(".osirion-nav-branding").Children.Length.ShouldBe(0);
    }

    [Theory]
    [InlineData(ThemeMode.Light, "light")]
    [InlineData(ThemeMode.Dark, "dark")]
    public void Menu_ExplicitTheme_IsWrittenAsComponentTheme(ThemeMode theme, string expected)
    {
        var cut = Render<Menu>(p => p.Add(c => c.Theme, theme));

        cut.Find(".osirion-menu-container").GetAttribute("data-osirion-component-theme").ShouldBe(expected);
    }

    [Theory]
    [InlineData(ThemeMode.System)]
    [InlineData((ThemeMode)42)]
    public void Menu_SystemOrUnknownTheme_WritesNoComponentTheme(ThemeMode theme)
    {
        var cut = Render<Menu>(p => p.Add(c => c.Theme, theme));

        cut.Find(".osirion-menu-container").HasAttribute("data-osirion-component-theme").ShouldBeFalse();
    }

    [Fact]
    public void Menu_ExtraLargeBreakpoint_AddsTheExtraLargeCollapseClass()
    {
        var cut = Render<Menu>(p => p.Add(c => c.CollapseBreakpoint, MenuCollapseBreakpoint.ExtraLarge));

        cut.Find(".osirion-menu-container").ClassList.ShouldContain("osirion-menu-collapse-extra-large");
    }

    [Fact]
    public void Menu_ExtraLargeBreakpointWithoutCollapse_AddsNoCollapseClasses()
    {
        var cut = Render<Menu>(p => p
            .Add(c => c.CollapseBreakpoint, MenuCollapseBreakpoint.ExtraLarge)
            .Add(c => c.CollapseOnMobile, false));

        var classes = cut.Find(".osirion-menu-container").ClassList;
        classes.ShouldNotContain("osirion-menu-collapse-extra-large");
        classes.ShouldNotContain("osirion-menu-collapsible");
    }

    [Fact]
    public void Menu_VerticalWithoutAutoExpand_OmitsAutoExpandAndAlignment()
    {
        var cut = Render<Menu>(p => p
            .Add(c => c.Orientation, MenuOrientation.Vertical)
            .Add(c => c.AutoExpandActive, false));

        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldNotContain("osirion-menu-auto-expand");
        container.ClassList.ShouldNotContain("osirion-menu-align-center");
        container.ClassList.ShouldNotContain("osirion-menu-sticky");
        container.HasAttribute("style").ShouldBeFalse();
    }

    [Fact]
    public void MenuDivider_Label_IsRenderedInsideTheSeparator()
    {
        var cut = Render<MenuDivider>(p => p.Add(c => c.Label, "More"));

        cut.Find("div.osirion-menu-divider[role=separator] > span.osirion-menu-divider-label").TextContent.ShouldBe("More");
    }

    [Fact]
    public void MenuDivider_BlankLabel_RendersNoLabel()
    {
        var cut = Render<MenuDivider>(p => p.Add(c => c.Label, " "));

        cut.FindAll(".osirion-menu-divider-label").ShouldBeEmpty();
    }

    [Fact]
    public void MenuPanel_Description_IsRenderedBelowTheTitle()
    {
        var cut = Render<MenuPanel>(p => p
            .Add(c => c.Title, "Featured")
            .Add(c => c.Description, "Hand-picked guides"));

        cut.Find("aside > p.osirion-menu-panel-description").TextContent.ShouldBe("Hand-picked guides");
        cut.Find("aside").GetAttribute("aria-label").ShouldBe("Featured");
    }

    [Fact]
    public void MenuPanel_AriaLabel_WinsOverTitle()
    {
        var cut = Render<MenuPanel>(p => p
            .Add(c => c.Title, "Featured")
            .Add(c => c.AriaLabel, "Featured guides"));

        cut.Find("aside").GetAttribute("aria-label").ShouldBe("Featured guides");
    }

    [Fact]
    public void MenuPanel_NoTitleOrDescription_RendersOnlyTheLinks()
    {
        var cut = Render<MenuPanel>(p => p.AddChildContent("<a href=\"/x\">X</a>"));

        cut.FindAll(".osirion-menu-panel-title").ShouldBeEmpty();
        cut.FindAll(".osirion-menu-panel-description").ShouldBeEmpty();
        cut.Find(".osirion-menu-panel-links > a").GetAttribute("href").ShouldBe("/x");
    }

    [Fact]
    public void MenuPanelLink_IconTemplate_IsRenderedAsHiddenIcon()
    {
        var cut = Render<MenuPanelLink>(p => p
            .Add(c => c.Text, "Guides")
            .Add(c => c.Href, "/guides")
            .Add(c => c.IconTemplate, (RenderFragment)(b => b.AddMarkupContent(0, "<i class=\"icon-book\"></i>"))));

        var icon = cut.Find("a > span.osirion-menu-panel-link-icon");
        icon.GetAttribute("aria-hidden").ShouldBe("true");
        icon.QuerySelector("i.icon-book").ShouldNotBeNull();
    }

    [Fact]
    public void MenuPanelLink_NoIconOrTarget_RendersTextAndArrowOnly()
    {
        var cut = Render<MenuPanelLink>(p => p.Add(c => c.Text, "Guides").Add(c => c.Href, "/guides"));

        var link = cut.Find("a.osirion-menu-panel-link");
        link.QuerySelector(".osirion-menu-panel-link-icon").ShouldBeNull();
        link.HasAttribute("target").ShouldBeFalse();
        link.HasAttribute("rel").ShouldBeFalse();
        link.QuerySelector(".osirion-menu-panel-link-text")!.TextContent.ShouldBe("Guides");
    }
}
