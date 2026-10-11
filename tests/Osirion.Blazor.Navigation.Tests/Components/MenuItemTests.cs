using Bunit;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuItemTests : BunitContext
{
    // Start of the Bootstrap Icons "house" path that MenuItem renders for Icon="home".
    private const string HomeIconPathStart = "M8.707 1.5a1 1 0 0 0-1.414 0";

    public MenuItemTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));

        // Set default JSInterop mode
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void MenuItem_ShouldRenderWithText()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Menu Item"));

        // Assert
        var link = cut.Find(".osirion-menu-item-wrapper > a.osirion-menu-item");
        link.GetAttribute("href").ShouldBe("#");
        link.GetAttribute("aria-disabled").ShouldBe("false");
        link.HasAttribute("aria-haspopup").ShouldBeFalse();
        link.HasAttribute("aria-expanded").ShouldBeFalse();
        link.HasAttribute("aria-current").ShouldBeFalse();
        cut.Find(".osirion-menu-item-content > .osirion-menu-item-text").TextContent.ShouldBe("Menu Item");
        cut.FindAll(".osirion-menu-item-icon").ShouldBeEmpty();
        cut.FindAll(".osirion-submenu").ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_ShouldRenderWithLink_WhenHrefProvided()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Menu Item")
            .Add(p => p.Href, "/test-page"));

        // Assert
        cut.Find("a.osirion-menu-item").GetAttribute("href").ShouldBe("/test-page");
        cut.Find(".osirion-menu-item-text").TextContent.ShouldBe("Menu Item");
    }

    [Fact]
    public void MenuItem_ShouldRenderWithIcon_WhenIconProvided()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Home")
            .Add(p => p.Icon, "home"));

        // Assert
        var icon = cut.Find("a.osirion-menu-item > .osirion-menu-item-icon > svg");
        icon.GetAttribute("aria-hidden").ShouldBe("true");
        icon.QuerySelector("path")!.GetAttribute("d")!.ShouldStartWith(HomeIconPathStart);
        cut.Find(".osirion-menu-item-text").TextContent.ShouldBe("Home");
    }

    [Fact]
    public void MenuItem_ShouldRenderWithActiveClass_WhenActive()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Active Item")
            .Add(p => p.IsActive, true));

        // Assert
        cut.Find(".osirion-menu-item-wrapper").ClassList.ShouldContain("osirion-menu-item-wrapper-active");
        var link = cut.Find("a.osirion-menu-item");
        link.ClassList.ShouldContain("osirion-menu-item-active");
        link.GetAttribute("aria-current").ShouldBe("page");
    }

    [Fact]
    public void MenuItem_ShouldRenderWithDisabledAttributes_WhenDisabled()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Disabled Item")
            .Add(p => p.Href, "/disabled-page")
            .Add(p => p.Disabled, true));

        // Assert
        var link = cut.Find("a.osirion-menu-item");
        link.ClassList.ShouldContain("osirion-menu-item-disabled");
        link.GetAttribute("aria-disabled").ShouldBe("true");
        link.HasAttribute("href").ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_ShouldRenderWithSubmenu_WhenHasSubmenu()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Parent Item")
            .Add(p => p.Href, "/parent")
            .Add(p => p.HasSubmenu, true)
            .AddChildContent(@"<div>Submenu Content</div>"));

        // Assert
        cut.Find(".osirion-menu-item-wrapper").ClassList.ShouldContain("osirion-menu-item-has-submenu");
        var link = cut.Find("a.osirion-menu-item");
        link.GetAttribute("href").ShouldBe("/parent");
        var submenu = cut.Find(".osirion-submenu");
        link.HasAttribute("aria-haspopup").ShouldBeFalse();
        link.HasAttribute("aria-expanded").ShouldBeFalse();
        submenu.HasAttribute("role").ShouldBeFalse();
        var toggler = cut.Find("input.osirion-submenu-toggler");
        toggler.GetAttribute("aria-controls").ShouldBe(submenu.Id);
        toggler.GetAttribute("aria-label").ShouldBe("Toggle Parent Item submenu");
        toggler.HasAttribute("aria-hidden").ShouldBeFalse();
        toggler.GetAttribute("tabindex").ShouldBeNull();
        submenu.QuerySelector("div")!.TextContent.ShouldBe("Submenu Content");
        cut.Find(".osirion-submenu-toggle > .osirion-menu-item-chevron > svg").ShouldNotBeNull();
    }

    [Fact]
    public void MenuItem_ShouldRenderTargetAndRelOnTheAnchor_WhenTargetIsBlank()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Docs")
            .Add(p => p.Href, "https://docs.example.com")
            .Add(p => p.Target, "_blank"));

        // Assert
        var link = cut.Find("a.osirion-menu-item");
        link.GetAttribute("target").ShouldBe("_blank");
        link.GetAttribute("rel").ShouldBe("noopener noreferrer");
        cut.Find(".osirion-menu-item-wrapper").HasAttribute("rel").ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_ShouldNotRenderTargetOrRel_WhenTargetIsNotSet()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Docs")
            .Add(p => p.Href, "/docs"));

        var link = cut.Find("a.osirion-menu-item");
        link.HasAttribute("target").ShouldBeFalse();
        link.HasAttribute("rel").ShouldBeFalse();
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("JaVaScRiPt:alert(1)")]
    [InlineData("  javascript:alert(1)")]
    [InlineData("java\tscript:alert(1)")]
    [InlineData("data:text/html,<script>alert(1)</script>")]
    [InlineData("vbscript:msgbox(1)")]
    public void MenuItem_ShouldNotRenderHref_WhenSchemeIsUnsafe(string href)
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Evil")
            .Add(p => p.Href, href));

        cut.Find("a.osirion-menu-item").HasAttribute("href").ShouldBeFalse();
    }

    [Theory]
    [InlineData("/relative/path")]
    [InlineData("relative")]
    [InlineData("#section")]
    [InlineData("?page=2")]
    [InlineData("https://example.com/a")]
    [InlineData("http://example.com/a")]
    [InlineData("mailto:someone@example.com")]
    [InlineData("tel:+123456789")]
    [InlineData("/search?q=a:b")]
    public void MenuItem_ShouldRenderHref_WhenSchemeIsSafe(string href)
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Fine")
            .Add(p => p.Href, href));

        cut.Find("a.osirion-menu-item").GetAttribute("href").ShouldBe(href);
    }

    [Fact]
    public void MenuItem_ShouldNotWriteComputedActiveStateIntoTheIsActiveParameter()
    {
        // Arrange
        Services.GetRequiredService<NavigationManager>().NavigateTo("/current");

        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Current")
            .Add(p => p.Href, "/current"));

        // Assert
        cut.Find("a.osirion-menu-item").GetAttribute("aria-current").ShouldBe("page");
        cut.Instance.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_ShouldFollowLocationChanges_WhenInteractive()
    {
        // Arrange
        SetRendererInfo(new RendererInfo("Server", true));
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/first");

        var first = Render<MenuItem>(parameters => parameters.Add(p => p.Text, "First").Add(p => p.Href, "/first"));
        var second = Render<MenuItem>(parameters => parameters.Add(p => p.Text, "Second").Add(p => p.Href, "/second"));
        first.Find("a.osirion-menu-item").GetAttribute("aria-current").ShouldBe("page");
        second.Find("a.osirion-menu-item").HasAttribute("aria-current").ShouldBeFalse();

        // Act
        navigation.NavigateTo("/second");

        // Assert
        first.WaitForAssertion(() => first.Find("a.osirion-menu-item").HasAttribute("aria-current").ShouldBeFalse());
        second.WaitForAssertion(() => second.Find("a.osirion-menu-item").GetAttribute("aria-current").ShouldBe("page"));
    }

    [Fact]
    public void MenuItem_ShouldStopListeningToLocationChanges_WhenDisposed()
    {
        // Arrange
        SetRendererInfo(new RendererInfo("Server", true));
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/first");
        var cut = Render<MenuItem>(parameters => parameters.Add(p => p.Text, "First").Add(p => p.Href, "/first"));

        var renderCount = cut.RenderCount;

        // Act
        cut.Instance.Dispose();
        navigation.NavigateTo("/second");

        // Assert: a component that was still subscribed would have re-rendered without aria-current
        cut.RenderCount.ShouldBe(renderCount);
        cut.Find("a.osirion-menu-item").GetAttribute("aria-current").ShouldBe("page");
    }

    [Fact]
    public void MenuItem_ShouldMatchPrefixOnSegmentBoundary()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/modules-extra/a");

        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Modules")
            .Add(p => p.Href, "/modules")
            .Add(p => p.Match, NavLinkMatch.Prefix));

        cut.Find("a.osirion-menu-item").HasAttribute("aria-current").ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("#")]
    public void MenuItem_ShouldRenderParentAsSubmenuToggle_WhenItHasNoDestination(string? href)
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Platform")
            .Add(p => p.Href, href!)
            .Add(p => p.HasSubmenu, true)
            .AddChildContent("<div>Submenu Content</div>"));

        // Assert
        var toggler = cut.Find("input.osirion-submenu-toggler");
        var parent = cut.Find("label.osirion-menu-item.osirion-menu-item-toggle-only");
        parent.GetAttribute("for").ShouldBe(toggler.Id);
        parent.QuerySelector(".osirion-menu-item-text")!.TextContent.ShouldBe("Platform");
        cut.FindAll("a.osirion-menu-item").ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_ShouldKeepTheLink_WhenAParentWithoutDestinationHandlesClicks()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Platform")
            .Add(p => p.HasSubmenu, true)
            .Add(p => p.OnClick, () => { }));

        // Assert
        cut.Find("a.osirion-menu-item").GetAttribute("href").ShouldBe("#");
        cut.FindAll(".osirion-menu-item-toggle-only").ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_ShouldRenderALink_WhenItHasNoSubmenuAndNoDestination()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Placeholder"));

        // Assert
        cut.Find("a.osirion-menu-item").GetAttribute("href").ShouldBe("#");
        cut.FindAll("label.osirion-menu-item").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("components")]
    [InlineData("osirion.blazor components overview")]
    public void MenuItem_ShouldRenderNoIconSlot_WhenTheIconNameIsNotBuiltIn(string icon)
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Components")
            .Add(p => p.Icon, icon));

        // Assert
        cut.FindAll(".osirion-menu-item-icon").ShouldBeEmpty();
        cut.Find(".osirion-menu-item-text").TextContent.ShouldBe("Components");
    }

    [Theory]
    [InlineData("blog")]
    [InlineData("BLOG")]
    [InlineData("document")]
    [InlineData("docs")]
    [InlineData("contact")]
    public void MenuItem_ShouldRenderTheBuiltInIcon_RegardlessOfCase(string icon)
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Item")
            .Add(p => p.Icon, icon));

        // Assert
        var svg = cut.Find("a.osirion-menu-item > .osirion-menu-item-icon > svg");
        svg.GetAttribute("aria-hidden").ShouldBe("true");
        svg.QuerySelectorAll("path").Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void MenuItem_ShouldRenderTheIconTemplate_InsteadOfABuiltInIcon()
    {
        // Act
        var cut = Render<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Docs")
            .Add(p => p.Icon, "docs")
            .Add(p => p.IconTemplate, "<svg class=\"custom-icon\"></svg>"));

        // Assert
        cut.FindAll(".osirion-menu-item-icon").Count.ShouldBe(1);
        cut.Find(".osirion-menu-item-icon > svg.custom-icon").ShouldNotBeNull();
    }
}
