using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class RichMenuTests : TestContext
{
    public RichMenuTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));
    }

    [Fact]
    public void MenuItem_RendersDescription_WhenProvided()
    {
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(item => item.Text, "Tracking & attribution")
            .Add(item => item.Description, "Reliable measurement down to ad level."));

        Assert.Equal("Tracking & attribution", cut.Find(".osirion-menu-item-text").TextContent);
        Assert.Equal("Reliable measurement down to ad level.", cut.Find(".osirion-menu-item-description").TextContent);
    }

    [Fact]
    public void MenuItem_AppliesMegaSubmenuVariant_WhenProvided()
    {
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(item => item.Text, "Platform")
            .Add(item => item.HasSubmenu, true)
            .Add(item => item.SubmenuVariant, SubmenuVariant.Mega));

        Assert.Contains("osirion-submenu-mega", cut.Find(".osirion-submenu").ClassList);
    }

    [Fact]
    public void MenuItem_WithSubmenu_RendersDedicatedToggleControl()
    {
        var cut = RenderComponent<MenuItem>(parameters => parameters
            .Add(item => item.Text, "Platform")
            .Add(item => item.HasSubmenu, true));

        var toggle = cut.Find(".osirion-submenu-toggler");
        var submenu = cut.Find(".osirion-submenu");
        var toggleLabel = cut.Find(".osirion-submenu-toggle");

        Assert.Equal(submenu.Id, toggle.GetAttribute("aria-controls"));
        Assert.Equal(toggle.Id, toggleLabel.GetAttribute("for"));
    }

    [Fact]
    public void MenuGroup_RendersLabelAndItems()
    {
        var cut = RenderComponent<MenuGroup>(parameters => parameters
            .Add(group => group.Label, "Core platform")
            .AddChildContent("Tracking & attribution"));

        Assert.Equal("Core platform", cut.Find(".osirion-menu-group-text").TextContent);
        Assert.Equal("Tracking & attribution", cut.Find(".osirion-menu-group-items").TextContent);
    }

    [Fact]
    public void MenuCallToAction_RendersDestinationAndSupportingText()
    {
        var cut = RenderComponent<MenuCallToAction>(parameters => parameters
            .Add(callToAction => callToAction.Text, "Platform overview")
            .Add(callToAction => callToAction.Description, "Explore the complete platform.")
            .Add(callToAction => callToAction.Href, "/platform"));

        var link = cut.Find(".osirion-menu-call-to-action");
        Assert.Equal("/platform", link.GetAttribute("href"));
        Assert.Equal("Platform overview", cut.Find(".osirion-menu-call-to-action-text").TextContent);
        Assert.Equal("Explore the complete platform.", cut.Find(".osirion-menu-call-to-action-description").TextContent);
    }
}