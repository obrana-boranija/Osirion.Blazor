using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuDividerTests : BunitContext
{
    public MenuDividerTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));

        // Set default JSInterop mode
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void MenuDivider_ShouldRenderWithDefaultClass()
    {
        // Act
        var cut = Render<MenuDivider>();

        // Assert
        cut.MarkupMatches(@"<div class=""osirion-menu-divider"" role=""separator""></div>");
    }

    [Fact]
    public void MenuDivider_ShouldRenderWithCustomClass_WhenProvided()
    {
        // Act
        var cut = Render<MenuDivider>(parameters => parameters
            .Add(p => p.Class, "custom-divider"));

        // Assert
        cut.MarkupMatches(@"<div class=""osirion-menu-divider custom-divider"" role=""separator""></div>");
    }

    [Fact]
    public void MenuDivider_ShouldRenderWithAdditionalAttributes_WhenProvided()
    {
        // Act
        var cut = Render<MenuDivider>(parameters => parameters
            .Add(p => p.Attributes, new Dictionary<string, object>
            {
                { "data-testid", "main-divider" },
                { "aria-orientation", "horizontal" }
            }));

        // Assert
        cut.MarkupMatches(
            @"<div class=""osirion-menu-divider"" 
                 role=""separator"" 
                 data-testid=""main-divider"" 
                 aria-orientation=""horizontal"">
             </div>");
    }
}