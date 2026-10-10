using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class RichMenuTests : BunitContext
{
    public RichMenuTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));
    }

    [Fact]
    public void MenuItem_RendersDescription_WhenProvided()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(item => item.Text, "Tracking & attribution")
            .Add(item => item.Description, "Reliable measurement down to ad level."));

        Assert.Equal("Tracking & attribution", cut.Find(".osirion-menu-item-text").TextContent);
        Assert.Equal("Reliable measurement down to ad level.", cut.Find(".osirion-menu-item-description").TextContent);
    }

    [Fact]
    public void MenuItem_AppliesMegaSubmenuVariant_WhenProvided()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(item => item.Text, "Platform")
            .Add(item => item.HasSubmenu, true)
            .Add(item => item.SubmenuVariant, SubmenuVariant.Mega));

        Assert.Contains("osirion-submenu-mega", cut.Find(".osirion-submenu").ClassList);
    }

    [Fact]
    public void MenuItem_WithSubmenu_RendersDedicatedToggleControl()
    {
        var cut = Render<MenuItem>(parameters => parameters
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
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(group => group.Label, "Core platform")
            .AddChildContent("Tracking & attribution"));

        Assert.Equal("Core platform", cut.Find(".osirion-menu-group-text").TextContent);
        Assert.Equal("Tracking & attribution", cut.Find(".osirion-menu-group-items").TextContent);
    }

    [Fact]
    public void MenuCallToAction_RendersDestinationAndSupportingText()
    {
        var cut = Render<MenuCallToAction>(parameters => parameters
            .Add(callToAction => callToAction.Text, "Platform overview")
            .Add(callToAction => callToAction.Description, "Explore the complete platform.")
            .Add(callToAction => callToAction.Href, "/platform"));

        var link = cut.Find(".osirion-menu-call-to-action");
        Assert.Equal("/platform", link.GetAttribute("href"));
        Assert.Equal("Platform overview", cut.Find(".osirion-menu-call-to-action-text").TextContent);
        Assert.Equal("Explore the complete platform.", cut.Find(".osirion-menu-call-to-action-description").TextContent);
    }

    [Fact]
    public void MenuGroup_RendersDescriptionAndFooterLink_WhenProvided()
    {
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(group => group.Label, "Core HR")
            .Add(group => group.Description, "Your foundation for HR productivity")
            .Add(group => group.FooterText, "Explore Core HR")
            .Add(group => group.FooterHref, "/core-hr")
            .AddChildContent("Items"));

        Assert.Equal("Your foundation for HR productivity", cut.Find(".osirion-menu-group-description").TextContent);

        var footer = cut.Find(".osirion-menu-group-footer");
        Assert.Equal("/core-hr", footer.GetAttribute("href"));
        Assert.Equal("Explore Core HR", cut.Find(".osirion-menu-group-footer-text").TextContent);
    }

    [Fact]
    public void MenuGroup_OmitsFooterLink_WhenHrefMissing()
    {
        var cut = Render<MenuGroup>(parameters => parameters
            .Add(group => group.Label, "Core HR")
            .Add(group => group.FooterText, "Explore Core HR")
            .AddChildContent("Items"));

        Assert.Empty(cut.FindAll(".osirion-menu-group-footer"));
    }

    [Fact]
    public void MenuItem_RendersMegaContentAndAsideZones_WhenAsideProvided()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(item => item.Text, "Platform")
            .Add(item => item.HasSubmenu, true)
            .Add(item => item.SubmenuVariant, SubmenuVariant.Mega)
            .AddChildContent("Groups")
            .Add(item => item.SubmenuAside, "Aside"));

        Assert.Contains("osirion-menu-item-has-mega", cut.Find(".osirion-menu-item-wrapper").ClassList);
        Assert.Equal("Groups", cut.Find(".osirion-submenu-mega-content").TextContent.Trim());
        Assert.Equal("Aside", cut.Find(".osirion-submenu-mega-aside").TextContent.Trim());
    }

    [Fact]
    public void MenuItem_OmitsAsideZone_WhenAsideNotProvided()
    {
        var cut = Render<MenuItem>(parameters => parameters
            .Add(item => item.Text, "Platform")
            .Add(item => item.HasSubmenu, true)
            .Add(item => item.SubmenuVariant, SubmenuVariant.Mega)
            .AddChildContent("Groups"));

        Assert.Empty(cut.FindAll(".osirion-submenu-mega-aside"));
    }

    [Fact]
    public void MenuPanel_RendersTitleAndLinks()
    {
        var cut = Render<MenuPanel>(parameters => parameters
            .Add(panel => panel.Title, "Build your business case")
            .AddChildContent<MenuPanelLink>(link => link
                .Add(l => l.Text, "ROI Assessment")
                .Add(l => l.Href, "/roi")));

        Assert.Equal("Build your business case", cut.Find(".osirion-menu-panel-title").TextContent);
        Assert.Equal("Build your business case", cut.Find(".osirion-menu-panel").GetAttribute("aria-label"));

        var link = cut.Find(".osirion-menu-panel-link");
        Assert.Equal("/roi", link.GetAttribute("href"));
        Assert.Equal("ROI Assessment", cut.Find(".osirion-menu-panel-link-text").TextContent);
    }

    [Fact]
    public void MenuPanelLink_AddsExternalRel_WhenTargetBlank()
    {
        var cut = Render<MenuPanelLink>(parameters => parameters
            .Add(link => link.Text, "API Documentation")
            .Add(link => link.Href, "https://docs.example.com")
            .Add(link => link.Target, "_blank"));

        var link = cut.Find(".osirion-menu-panel-link");
        Assert.Equal("_blank", link.GetAttribute("target"));
        Assert.Equal("noopener noreferrer", link.GetAttribute("rel"));
    }

    [Fact]
    public void MenuImageCard_RendersImageAndCaption()
    {
        var cut = Render<MenuImageCard>(parameters => parameters
            .Add(card => card.ImageUrl, "https://cdn.example.com/story.webp")
            .Add(card => card.ImageAlt, "Customer story")
            .Add(card => card.Title, "Customer Stories")
            .Add(card => card.Description, "Trusted by teams across Europe")
            .Add(card => card.Href, "/stories"));

        Assert.Equal("/stories", cut.Find(".osirion-menu-image-card").GetAttribute("href"));

        var image = cut.Find(".osirion-menu-image-card-image");
        Assert.Equal("https://cdn.example.com/story.webp", image.GetAttribute("src"));
        Assert.Equal("Customer story", image.GetAttribute("alt"));

        Assert.Equal("Customer Stories", cut.Find(".osirion-menu-image-card-title").TextContent);
        Assert.Equal("Trusted by teams across Europe", cut.Find(".osirion-menu-image-card-description").TextContent);
    }

    [Fact]
    public void MenuImageCard_OmitsDescription_WhenNotProvided()
    {
        var cut = Render<MenuImageCard>(parameters => parameters
            .Add(card => card.ImageUrl, "https://cdn.example.com/story.webp")
            .Add(card => card.Title, "Customer Stories")
            .Add(card => card.Href, "/stories"));

        Assert.Empty(cut.FindAll(".osirion-menu-image-card-description"));
    }

    [Theory]
    [InlineData("javascript:alert(1)")]
    [InlineData("JAVASCRIPT:alert(1)")]
    [InlineData("data:text/html,x")]
    public void MenuLinks_DoNotRenderHref_WhenSchemeIsUnsafe(string href)
    {
        var panelLink = Render<MenuPanelLink>(parameters => parameters
            .Add(p => p.Text, "Link")
            .Add(p => p.Href, href));
        var imageCard = Render<MenuImageCard>(parameters => parameters
            .Add(p => p.Title, "Card")
            .Add(p => p.ImageUrl, "/a.png")
            .Add(p => p.Href, href));
        var callToAction = Render<MenuCallToAction>(parameters => parameters
            .Add(p => p.Text, "Go")
            .Add(p => p.Href, href));

        Assert.False(panelLink.Find("a").HasAttribute("href"));
        Assert.False(imageCard.Find("a").HasAttribute("href"));
        Assert.False(callToAction.Find("a").HasAttribute("href"));
    }

    [Fact]
    public void MenuLinks_RenderHref_WhenSchemeIsSafe()
    {
        var panelLink = Render<MenuPanelLink>(parameters => parameters
            .Add(p => p.Text, "Link")
            .Add(p => p.Href, "https://example.com/a"));
        var callToAction = Render<MenuCallToAction>(parameters => parameters
            .Add(p => p.Text, "Go")
            .Add(p => p.Href, "/contact"));

        Assert.Equal("https://example.com/a", panelLink.Find("a").GetAttribute("href"));
        Assert.Equal("/contact", callToAction.Find("a").GetAttribute("href"));
    }
}
