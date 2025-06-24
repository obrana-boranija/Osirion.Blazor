using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuGroupTests : TestContext
{
    public MenuGroupTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));

        // Set default JSInterop mode
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void MenuGroup_ShouldRenderWithoutLabel_WhenLabelNotProvided()
    {
        // Act
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .AddChildContent("MenuGroup Content"));

        // Assert
        cut.MarkupMatches(
            @"<div class=""osirion-menu-group"">
                <div class=""osirion-menu-group-items"">
                    MenuGroup Content
                </div>
            </div>");
    }

    [Fact]
    public void MenuGroup_ShouldRenderWithLabel_WhenLabelProvided()
    {
        // Act
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .Add(p => p.Label, "Group Label")
            .AddChildContent("MenuGroup Content"));

        // Assert
        var labelId = cut.Find(".osirion-menu-group-label").Id;
        labelId.ShouldNotBeNullOrEmpty();

        cut.MarkupMatches(
            $@"<div class=""osirion-menu-group"">
                <div class=""osirion-menu-group-label"" id=""{labelId}"">
                    Group Label
                </div>
                <div class=""osirion-menu-group-items"">
                    MenuGroup Content
                </div>
            </div>");
    }

    [Fact]
    public void MenuGroup_ShouldRenderWithCustomClass_WhenProvided()
    {
        // Act
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .Add(p => p.Class, "custom-group")
            .AddChildContent("MenuGroup Content"));

        // Assert
        cut.MarkupMatches(
            @"<div class=""osirion-menu-group custom-group"">
                <div class=""osirion-menu-group-items"">
                    MenuGroup Content
                </div>
            </div>");
    }

    [Fact]
    public void MenuGroup_ShouldRenderWithAdditionalAttributes_WhenProvided()
    {
        // Act
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .Add(p => p.Attributes, new Dictionary<string, object>
            {
                { "data-testid", "main-group" }
            })
            .AddChildContent("MenuGroup Content"));

        // Assert
        cut.MarkupMatches(
            @"<div class=""osirion-menu-group"" data-testid=""main-group"">
                <div class=""osirion-menu-group-items"">
                    MenuGroup Content
                </div>
            </div>");
    }
}