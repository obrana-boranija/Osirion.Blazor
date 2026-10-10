using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuTests : BunitContext
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
        var cut = Render<Menu>(parameters => parameters
                    .Add(p => p.Orientation, MenuOrientation.Vertical)
                    .AddChildContent("Menu Content"));

        // Assert
        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldContain("osirion-menu-vertical");
        container.ClassList.ShouldContain("osirion-menu-auto-expand");
        container.ClassList.ShouldNotContain("osirion-menu-horizontal");
        container.ClassList.ShouldNotContain("osirion-menu-sticky");
        container.HasAttribute("style").ShouldBeFalse();
        container.Id.ShouldStartWith("osirion-menu-vertical-");
        cut.Find(".osirion-menu > .osirion-menu-inner").TextContent.Trim().ShouldBe("Menu Content");
    }

    [Fact]
    public void Menu_ShouldRenderHorizontalMenu_WhenSpecified()
    {
        // Act
        var cut = Render<Menu>(parameters => parameters
                    .Add(p => p.Orientation, MenuOrientation.Horizontal)
                    .AddChildContent("Menu Content"));

        // Assert
        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldContain("osirion-menu-horizontal");
        container.ClassList.ShouldContain("osirion-menu-align-center");
        container.ClassList.ShouldContain("osirion-menu-collapsible");
        container.ClassList.ShouldNotContain("osirion-menu-vertical");
        container.Id.ShouldStartWith("osirion-menu-horizontal-");
        var toggler = cut.Find("input.osirion-navbar-toggler");
        cut.Find("label.osirion-navbar-toggle").GetAttribute("for").ShouldBe(toggler.Id);
        cut.Find(".osirion-menu > .osirion-menu-inner").TextContent.Trim().ShouldBe("Menu Content");
    }

    [Fact]
    public void Menu_ShouldRenderStickyMenu_WhenIsSticky()
    {
        // Act
        var cut = Render<Menu>(parameters => parameters
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
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.AriaLabel, "Main Menu")
            .AddChildContent("Menu Content"));

        // Assert
        cut.Find("nav.osirion-menu-nav").GetAttribute("aria-label").ShouldBe("Main Menu");
    }

    [Fact]
    public void Menu_ShouldRenderWithoutCollapseClass_WhenCollapseOnMobileFalse()
    {
        // Act
        var cut = Render<Menu>(parameters => parameters
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
        var cut = Render<Menu>(parameters => parameters
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
        var cut = Render<Menu>(parameters => parameters
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

    [Fact]
    public void Menu_ShouldGenerateUniqueIds_WhenTwoMenusShareAnOrientation()
    {
        // Act
        var first = Render<Menu>(parameters => parameters.AddChildContent("A"));
        var second = Render<Menu>(parameters => parameters.AddChildContent("B"));

        // Assert
        var firstContainer = first.Find(".osirion-menu-container");
        var secondContainer = second.Find(".osirion-menu-container");
        firstContainer.Id.ShouldNotBe(secondContainer.Id);

        var firstToggler = first.Find("input.osirion-navbar-toggler");
        var secondToggler = second.Find("input.osirion-navbar-toggler");
        firstToggler.Id.ShouldNotBe(secondToggler.Id);
        first.Find("label.osirion-navbar-toggle").GetAttribute("for").ShouldBe(firstToggler.Id);
        second.Find("label.osirion-navbar-toggle").GetAttribute("for").ShouldBe(secondToggler.Id);
    }

    [Fact]
    public void Menu_ShouldKeepTheSameIds_AcrossRenders()
    {
        var cut = Render<Menu>(parameters => parameters.AddChildContent("A"));
        var id = cut.Find(".osirion-menu-container").Id;
        var togglerId = cut.Find("input.osirion-navbar-toggler").Id;

        cut.Render(parameters => parameters.Add(p => p.AriaLabel, "Other label"));

        cut.Find(".osirion-menu-container").Id.ShouldBe(id);
        cut.Find("input.osirion-navbar-toggler").Id.ShouldBe(togglerId);
    }

    [Fact]
    public void Menu_ShouldHonourAnExplicitId()
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.Id, "main-menu")
            .AddChildContent("A"));

        cut.Find(".osirion-menu-container").Id.ShouldBe("main-menu");
        cut.Find("input.osirion-navbar-toggler").Id.ShouldBe("main-menu-toggle");
    }

    [Fact]
    public void Menu_ShouldRenderAKeyboardOperableToggle()
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.ToggleAriaLabel, "Open the menu")
            .AddChildContent("A"));

        var toggler = cut.Find("input.osirion-navbar-toggler");
        toggler.GetAttribute("type").ShouldBe("checkbox");
        toggler.HasAttribute("aria-hidden").ShouldBeFalse();
        toggler.HasAttribute("tabindex").ShouldBeFalse();
        toggler.GetAttribute("aria-label").ShouldBe("Open the menu");
        toggler.GetAttribute("aria-controls").ShouldBe(cut.Find("nav.osirion-menu-nav").Id);
    }

    [Fact]
    public void Menu_ShouldKeepClassesAndStyle_WhenTheParentRendersAgain()
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.Class, "custom-menu")
            .Add(p => p.StickyZIndex, 999)
            .AddChildContent("A"));

        cut.Render(parameters => parameters
            .Add(p => p.Class, "custom-menu")
            .Add(p => p.StickyZIndex, 999)
            .Add(p => p.AriaLabel, "Changed"));

        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldContain("osirion-menu-horizontal");
        container.ClassList.ShouldContain("osirion-menu-sticky");
        container.ClassList.ShouldContain("custom-menu");
        container.ClassList.Count(c => c == "osirion-menu-horizontal").ShouldBe(1);
        container.GetAttribute("style").ShouldBe("z-index: 999;");
        container.Id.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Menu_ShouldReflectParameterChanges_InClassesAndStyle()
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.Orientation, MenuOrientation.Horizontal)
            .AddChildContent("A"));
        cut.Find(".osirion-menu-container").ClassList.ShouldContain("osirion-menu-sticky");

        cut.Render(parameters => parameters
            .Add(p => p.Orientation, MenuOrientation.Vertical)
            .Add(p => p.Sticky, false));

        var container = cut.Find(".osirion-menu-container");
        container.ClassList.ShouldContain("osirion-menu-vertical");
        container.ClassList.ShouldNotContain("osirion-menu-horizontal");
        container.ClassList.ShouldNotContain("osirion-menu-sticky");
        container.HasAttribute("style").ShouldBeFalse();
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("data:text/html,x")]
    public void Menu_ShouldNotRenderBrandHref_WhenSchemeIsUnsafe(string href)
    {
        var cut = Render<Menu>(parameters => parameters
            .Add(p => p.Href, href)
            .Add(p => p.BrandText, "Brand")
            .AddChildContent("A"));

        cut.Find("a.osirion-brand-link").HasAttribute("href").ShouldBeFalse();
    }
}
