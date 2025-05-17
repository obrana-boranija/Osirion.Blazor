using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuItemTests : TestContext
{
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
        cut.MarkupMatches(
            @"<a href=""#"" class=""osirion-menu-item"" role=""menuitem"">
                <span class=""osirion-menu-item-text"">Menu Item</span>
            </a>");
    }

    [Fact]
    public void MenuItem_ShouldRenderWithLink_WhenHrefProvided()
    {
        // Act
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Menu Item")
            .Add(p => p.Href, "/test-page"));

        // Assert
        cut.MarkupMatches(
            @"<a href=""/test-page"" class=""osirion-menu-item"" role=""menuitem"">
                <span class=""osirion-menu-item-text"">Menu Item</span>
            </a>");
    }

    [Fact]
    public void MenuItem_ShouldRenderWithIcon_WhenIconProvided()
    {
        // Act
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Home")
            .Add(p => p.Icon, "home"));

        // Assert
        cut.MarkupMatches(
            @"<a href=""#"" class=""osirion-menu-item"" role=""menuitem"">
                <span class=""osirion-menu-item-icon"">
                    <svg xmlns=""http://www.w3.org/2000/svg"" width=""20"" height=""20"" fill=""currentColor"" class=""bi bi-house"" viewBox=""0 0 16 16"">
                        <path d=""M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.708L2 8.207V13.5A1.5 1.5 0 0 0 3.5 15h9a1.5 1.5 0 0 0 1.5-1.5V8.207l.646.647a.5.5 0 0 0 .708-.708L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293zM13 7.207V13.5a.5.5 0 0 1-.5.5h-9a.5.5 0 0 1-.5-.5V7.207l5-5z""/>
                    </svg>
                </span>
                <span class=""osirion-menu-item-text"">Home</span>
            </a>");
    }

    [Fact]
    public void MenuItem_ShouldRenderWithActiveClass_WhenActive()
    {
        // Act
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Active Item")
            .Add(p => p.IsActive, true));

        // Assert
        cut.MarkupMatches(
            @"<a href=""#"" class=""osirion-menu-item osirion-menu-item-active"" role=""menuitem"">
                <span class=""osirion-menu-item-text"">Active Item</span>
            </a>");
    }

    [Fact]
    public void MenuItem_ShouldRenderWithDisabledAttributes_WhenDisabled()
    {
        // Act
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(p => p.Text, "Disabled Item")
            .Add(p => p.Disabled, true));

        // Assert
        cut.MarkupMatches(
            @"<a href=""#"" class=""osirion-menu-item osirion-menu-item-disabled"" role=""menuitem"" aria-disabled=""true"" tabindex=""-1"">
                <span class=""osirion-menu-item-text"">Disabled Item</span>
            </a>");
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
        cut.MarkupMatches(
            @"<a href=""#"" class=""osirion-menu-item osirion-menu-item-has-submenu"" role=""menuitem"">
                <span class=""osirion-menu-item-text"">Parent Item</span>
                <span class=""osirion-menu-item-chevron"">
                    <svg xmlns=""http://www.w3.org/2000/svg"" width=""16"" height=""16"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round"">
                        <polyline points=""9 18 15 12 9 6"" />
                    </svg>
                </span>
            </a>
            <div class=""osirion-submenu"">
                <div>Submenu Content</div>
            </div>");
    }
}