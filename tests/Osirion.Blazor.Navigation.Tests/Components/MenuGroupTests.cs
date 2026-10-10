using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuGroupTests : BunitContext
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
        var cut = Render<MenuGroup>(parameters => parameters
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
        var cut = Render<MenuGroup>(parameters => parameters
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
        var cut = Render<MenuGroup>(parameters => parameters
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
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(p => p.Label, "Group Label")
            .AddChildContent("MenuGroup Content"));
        var labelIdBefore = cut.Find(".osirion-menu-group-label").Id;

        // Act
        cut.Render(parameters => parameters
            .Add(p => p.Label, "Renamed Label"));

        // Assert
        cut.Find(".osirion-menu-group-label").Id.ShouldBe(labelIdBefore);
    }

    [Fact]
    public void MenuGroup_ShouldRenderWithCustomClass_WhenProvided()
    {
        // Act
        var cut = Render<MenuGroup>(parameters => parameters
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
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(p => p.Attributes, new Dictionary<string, object>
            {
                { "data-testid", "main-group" }
            })
            .AddChildContent("MenuGroup Content"));

        // Assert
        cut.Find(".osirion-menu-group").GetAttribute("data-testid").ShouldBe("main-group");
    }

    [Fact]
    public void MenuGroup_ShouldKeepClassesAndId_WhenTheParentRendersAgain()
    {
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(p => p.Label, "Group")
            .Add(p => p.Collapsible, true)
            .Add(p => p.Class, "custom-group")
            .AddChildContent("Content"));
        var id = cut.Find(".osirion-menu-group").Id;

        cut.Render(parameters => parameters
            .Add(p => p.Label, "Group")
            .Add(p => p.Collapsible, true)
            .Add(p => p.Class, "custom-group")
            .Add(p => p.Description, "Changed"));

        var group = cut.Find(".osirion-menu-group");
        group.Id.ShouldBe(id);
        group.ClassList.ShouldContain("osirion-menu-group-collapsible");
        group.ClassList.ShouldContain("osirion-menu-group-expanded");
        group.ClassList.ShouldContain("custom-group");
        group.ClassList.Count(c => c == "custom-group").ShouldBe(1);
    }

    [Fact]
    public void MenuGroup_ShouldReflectParameterChanges_InClasses()
    {
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(p => p.Collapsible, true)
            .AddChildContent("Content"));
        cut.Find(".osirion-menu-group").ClassList.ShouldContain("osirion-menu-group-collapsible");

        cut.Render(parameters => parameters.Add(p => p.Collapsible, false));

        cut.Find(".osirion-menu-group").ClassList.ShouldNotContain("osirion-menu-group-collapsible");
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("data:text/html,x")]
    public void MenuGroup_ShouldNotRenderFooter_WhenFooterSchemeIsUnsafe(string href)
    {
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(p => p.FooterText, "More")
            .Add(p => p.FooterHref, href)
            .AddChildContent("Content"));

        cut.FindAll("a.osirion-menu-group-footer").ShouldBeEmpty();
    }

    [Fact]
    public void MenuGroup_ShouldRenderFooter_WhenFooterHrefIsSafe()
    {
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(p => p.FooterText, "More")
            .Add(p => p.FooterHref, "/more")
            .AddChildContent("Content"));

        cut.Find("a.osirion-menu-group-footer").GetAttribute("href").ShouldBe("/more");
    }
}
