using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuTests : TestContext
{
    public MenuTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));

        // Set default JSInterop mode
        JSInterop.Mode = JSRuntimeMode.Loose;
    }


    [Fact]
    public void Menu_ShouldRenderVerticalMenu_WhenSpecified()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
                    .Add(p => p.Orientation, MenuOrientation.Vertical)
                    .AddChildContent("Menu Content"));

        // Assert
        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldContain("osirion-menu-vertical");
        container.ClassList.ShouldContain("osirion-menu-auto-expand");
        container.ClassList.ShouldNotContain("osirion-menu-horizontal");
        container.ClassList.ShouldNotContain("osirion-menu-sticky");
        container.HasAttribute("style").ShouldBeFalse();
        container.Id.ShouldBe("osirion-menu-vertical");
        cut.Find(".osirion-menu > .osirion-menu-inner").TextContent.Trim().ShouldBe("Menu Content");
    }

    [Fact]
    public void Menu_ShouldRenderHorizontalMenu_WhenSpecified()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
                    .Add(p => p.Orientation, MenuOrientation.Horizontal)
                    .AddChildContent("Menu Content"));

        // Assert
        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldContain("osirion-menu-horizontal");
        container.ClassList.ShouldContain("osirion-menu-align-center");
        container.ClassList.ShouldContain("osirion-menu-collapsible");
        container.ClassList.ShouldNotContain("osirion-menu-vertical");
        container.Id.ShouldBe("osirion-menu-horizontal");
        var toggler = cut.Find("input.osirion-navbar-toggler");
        cut.Find("label.osirion-navbar-toggle").GetAttribute("for").ShouldBe(toggler.Id);
        cut.Find(".osirion-menu > .osirion-menu-inner").TextContent.Trim().ShouldBe("Menu Content");
    }

    [Fact]
    public void Menu_ShouldRenderStickyMenu_WhenIsSticky()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.Sticky, true)
            .Add(p => p.StickyZIndex, 999)
            .AddChildContent("Menu Content"));

        // Assert
        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldContain("osirion-menu-sticky");
        container.GetAttribute("style").ShouldBe("z-index: 999;");
    }

    [Fact]
    public void Menu_ShouldRenderWithAriaLabel_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.AriaLabel, "Main Menu")
            .AddChildContent("Menu Content"));

        // Assert
        cut.Find("nav.osirion-menu-nav").GetAttribute("aria-label").ShouldBe("Main Menu");
    }

    [Fact]
    public void Menu_ShouldRenderWithoutCollapseClass_WhenCollapseOnMobileFalse()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.CollapseOnMobile, false)
            .AddChildContent("Menu Content"));

        // Assert
        cut.Find(".osirion-menu-container").ClassList.ShouldNotContain("osirion-menu-collapsible");
        cut.FindAll(".osirion-navbar-toggler").ShouldBeEmpty();
        cut.FindAll(".osirion-navbar-toggle").ShouldBeEmpty();
    }

    [Fact]
    public void Menu_ShouldRenderWithCustomClass_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.Class, "custom-menu")
            .AddChildContent("Menu Content"));

        // Assert
        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldContain("custom-menu");
        container.ClassList.ShouldContain("osirion-menu-horizontal");
    }

    [Fact]
    public void Menu_ShouldRenderWithAdditionalAttributes_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.Attributes, new Dictionary<string, object>
            {
                { "data-testid", "main-menu" },
                { "aria-expanded", "true" }
            })
            .AddChildContent("Menu Content"));

        // Assert
        var container = cut.Find(".osirion-menu-container");
        container.GetAttribute("data-testid").ShouldBe("main-menu");
        container.GetAttribute("aria-expanded").ShouldBe("true");
    }
}
