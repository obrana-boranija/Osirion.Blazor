using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuItemTests : TestContext
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
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Menu Item"));

        // Assert
        var link = cut.Find(".osirion-menu-item-wrapper > a.osirion-menu-item");
        link.GetAttribute("href").ShouldBe("#");
        link.GetAttribute("aria-disabled").ShouldBe("false");
        link.GetAttribute("aria-haspopup").ShouldBe("false");
        link.HasAttribute("aria-current").ShouldBeFalse();
        cut.Find(".osirion-menu-item-content > .osirion-menu-item-text").TextContent.ShouldBe("Menu Item");
        cut.FindAll(".osirion-menu-item-icon").ShouldBeEmpty();
        cut.FindAll(".osirion-submenu").ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_ShouldRenderWithLink_WhenHrefProvided()
    {
        // Act
        var cut = RenderComponent<MenuItem>(parameters => parameters
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
        var cut = RenderComponent<MenuItem>(parameters => parameters
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
        var cut = RenderComponent<MenuItem>(parameters => parameters
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
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Disabled Item")
            .Add(p => p.Href, "/disabled-page")
            .Add(p => p.Disabled, true));

        // Assert
        var link = cut.Find("a.osirion-menu-item");
        link.ClassList.ShouldContain("osirion-menu-item-disabled");
        link.GetAttribute("aria-disabled").ShouldBe("true");
        link.GetAttribute("href").ShouldBe("#");
    }

    [Fact]
    public void MenuItem_ShouldRenderWithSubmenu_WhenHasSubmenu()
    {
        // Act
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Parent Item")
            .Add(p => p.HasSubmenu, true)
            .AddChildContent(@"<div>Submenu Content</div>"));

        // Assert
        cut.Find(".osirion-menu-item-wrapper").ClassList.ShouldContain("osirion-menu-item-has-submenu");
        var link = cut.Find("a.osirion-menu-item");
        var submenu = cut.Find(".osirion-submenu");
        link.GetAttribute("aria-haspopup").ShouldBe("true");
        link.GetAttribute("aria-controls").ShouldBe(submenu.Id);
        submenu.GetAttribute("role").ShouldBe("menu");
        submenu.QuerySelector("div")!.TextContent.ShouldBe("Submenu Content");
        cut.Find(".osirion-submenu-toggle > .osirion-menu-item-chevron > svg").ShouldNotBeNull();
    }
}
