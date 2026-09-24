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
        cut.FindAll(".osirion-menu-group-label").ShouldBeEmpty();
        var items = cut.Find(".osirion-menu-group > .osirion-menu-group-items");
        items.GetAttribute("role").ShouldBe("group");
        items.HasAttribute("aria-labelledby").ShouldBeFalse();
        items.TextContent.Trim().ShouldBe("MenuGroup Content");
    }

    [Fact]
    public void MenuGroup_ShouldRenderWithLabel_WhenLabelProvided()
    {
        // Act
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .Add(p => p.Label, "Group Label")
            .AddChildContent("MenuGroup Content"));

        // Assert
        var label = cut.Find(".osirion-menu-group-label");
        label.Id.ShouldNotBeNullOrEmpty();
        label.QuerySelector(".osirion-menu-group-text")!.TextContent.ShouldBe("Group Label");
        var items = cut.Find(".osirion-menu-group-items");
        items.GetAttribute("aria-labelledby").ShouldBe(label.Id);
        items.TextContent.Trim().ShouldBe("MenuGroup Content");
    }

    [Fact]
    public void MenuGroup_ShouldDeriveElementIds_FromOneGroupId()
    {
        // Act
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .Add(p => p.Label, "Group Label")
            .AddChildContent("MenuGroup Content"));

        // Assert
        var groupId = cut.Find(".osirion-menu-group").Id;
        groupId.ShouldNotBeNullOrEmpty();
        cut.Find(".osirion-menu-group-label").Id.ShouldBe($"{groupId}-label");
        cut.Find(".osirion-menu-group-items").Id.ShouldBe($"{groupId}-items");
    }

    [Fact]
    public void MenuGroup_ShouldKeepElementIds_AcrossRenders()
    {
        // Arrange
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .Add(p => p.Label, "Group Label")
            .AddChildContent("MenuGroup Content"));
        var labelIdBefore = cut.Find(".osirion-menu-group-label").Id;

        // Act
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Label, "Renamed Label"));

        // Assert
        cut.Find(".osirion-menu-group-label").Id.ShouldBe(labelIdBefore);
    }

    [Fact]
    public void MenuGroup_ShouldRenderWithCustomClass_WhenProvided()
    {
        // Act
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .Add(p => p.Class, "custom-group")
            .AddChildContent("MenuGroup Content"));

        // Assert
        var group = cut.Find(".osirion-menu-group");
        group.ClassList.ShouldContain("custom-group");
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
        cut.Find(".osirion-menu-group").GetAttribute("data-testid").ShouldBe("main-group");
    }
}
