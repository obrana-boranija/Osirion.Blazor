using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;

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
        cut.MarkupMatches(
            @"<div class=""osirion-menu osirion-menu-vertical osirion-menu-collapsible"" role=""menu"">
                <div class=""osirion-menu-inner"">
                    Menu Content
                </div>
            </div>");
    }

    [Fact]
    public void Menu_ShouldRenderHorizontalMenu_WhenSpecified()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
                    .Add(p => p.Orientation, MenuOrientation.Horizontal)
                    .AddChildContent("Menu Content"));

        // Assert
        cut.MarkupMatches(
            @"<div class=""osirion-menu osirion-menu-horizontal osirion-menu-collapsible"" role=""menu"">
                <div class=""osirion-menu-inner"">
                Menu Content
                </div>
            </div>");
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
        cut.MarkupMatches(
            @"<div class=""osirion-menu osirion-menu-horizontal osirion-menu-collapsible osirion-menu-sticky"" role=""menu"" style=""z-index: 999;"">
                <div class=""osirion-menu-inner"">
                    Menu Content
                </div>
            </div>");
    }

    [Fact]
    public void Menu_ShouldRenderWithAriaLabel_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.AriaLabel, "Main Menu")
            .AddChildContent("Menu Content"));

        // Assert
        cut.MarkupMatches(
            @"<div class=""osirion-menu osirion-menu-horizontal osirion-menu-collapsible"" role=""menu"" aria-label=""Main Menu"">
                <div class=""osirion-menu-inner"">
                    Menu Content
                </div>
            </div>");
    }

    [Fact]
    public void Menu_ShouldRenderWithoutCollapseClass_WhenCollapseOnMobileFalse()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.CollapseOnMobile, false)
            .AddChildContent("Menu Content"));

        // Assert
        cut.MarkupMatches(
            @"<div class=""osirion-menu osirion-menu-horizontal"" role=""menu"">
                <div class=""osirion-menu-inner"">
                    Menu Content
                </div>
            </div>");
    }

    [Fact]
    public void Menu_ShouldRenderWithCustomClass_WhenProvided()
    {
        // Act
        var cut = RenderComponent<Menu>(parameters => parameters
            .Add(p => p.Class, "custom-menu")
            .AddChildContent("Menu Content"));

        // Assert
        cut.MarkupMatches(
            @"<div class=""osirion-menu custom-menu osirion-menu-horizontal osirion-menu-collapsible"" role=""menu"">
                <div class=""osirion-menu-inner"">
                    Menu Content
                </div>
            </div>");
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
        cut.MarkupMatches(
            @"<div class=""osirion-menu osirion-menu-horizontal osirion-menu-collapsible"" 
                 role=""menu"" 
                 data-testid=""main-menu"" 
                 aria-expanded=""true"">
                <div class=""osirion-menu-inner"">
                    Menu Content
                </div>
            </div>");
    }
}