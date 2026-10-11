using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuItemIconAndStateTests : BunitContext
{
    public MenuItemIconAndStateTests()
    {
        SetRendererInfo(new RendererInfo("Static", false));
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    public static TheoryData<string, string[]> BuiltInIcons => new()
    {
        { "home", ["M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146"] },
        { "HOME", ["M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146"] },
        { "blog", ["M5 10.5a.5.5 0 0 1 .5-.5h2", "M3 0h10a2 2 0 0 1 2 2v12", "M1 5v-.5a.5.5 0 0 1 1 0V5h.5"] },
        { "document", ["M5.5 7a.5.5 0 0 0 0 1h5", "M9.5 0H4a2 2 0 0 0-2 2v12"] },
        { "docs", ["M1 2.828c.885-.37 2.154-.769 3.388-.893"] },
        { "documentation", ["M1 2.828c.885-.37 2.154-.769 3.388-.893"] },
        { "user", ["M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6m2-3"] },
        { "profile", ["M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6m2-3"] },
        { "contact", ["M3.654 1.328a.678.678 0 0 0-1.015-.063"] },
        { "about", ["M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1", "m8.93 6.588-2.29.287-.082.38"] },
        { "Info", ["M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1", "m8.93 6.588-2.29.287-.082.38"] },
        { "settings", ["M8 4.754a3.246 3.246 0 1 0 0 6.492", "M9.796 1.343c-.527-1.79-3.065-1.79-3.592 0"] },
        { "gear", ["M8 4.754a3.246 3.246 0 1 0 0 6.492", "M9.796 1.343c-.527-1.79-3.065-1.79-3.592 0"] },
        { "dashboard", ["M8 4a.5.5 0 0 1 .5.5V6a.5.5 0 0 1-1 0V4.5", "M0 10a8 8 0 1 1 15.547 2.661"] }
    };

    [Theory]
    [MemberData(nameof(BuiltInIcons))]
    public void MenuItem_BuiltInIcon_RendersTheMatchingSvgPaths(string icon, string[] pathStarts)
    {
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Item").Add(c => c.Icon, icon));

        var svg = cut.Find(".osirion-menu-item-icon > svg");
        svg.GetAttribute("viewBox").ShouldBe("0 0 16 16");
        svg.GetAttribute("width").ShouldBe("16");
        svg.GetAttribute("height").ShouldBe("16");
        svg.GetAttribute("fill").ShouldBe("currentColor");
        svg.GetAttribute("aria-hidden").ShouldBe("true");
        var paths = svg.QuerySelectorAll("path");
        paths.Length.ShouldBe(pathStarts.Length);
        for (var i = 0; i < pathStarts.Length; i++)
        {
            paths[i].GetAttribute("d")!.ShouldStartWith(pathStarts[i]);
        }
    }

    [Fact]
    public void MenuItem_DashboardIcon_MarksTheGaugePathEvenOdd()
    {
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Dashboard").Add(c => c.Icon, "dashboard"));

        var paths = cut.FindAll(".osirion-menu-item-icon path");
        paths[0].HasAttribute("fill-rule").ShouldBeFalse();
        paths[1].GetAttribute("fill-rule").ShouldBe("evenodd");
    }

    [Theory]
    [InlineData("home")]
    [InlineData("info")]
    [InlineData("settings")]
    public void MenuItem_IconWithoutFillRule_WritesNoFillRule(string icon)
    {
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Item").Add(c => c.Icon, icon));

        cut.FindAll(".osirion-menu-item-icon path").ShouldAllBe(path => !path.HasAttribute("fill-rule"));
    }

    [Fact]
    public void MenuItem_UnknownIcon_RendersNoIconSlot()
    {
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Item").Add(c => c.Icon, "rocket"));

        cut.FindAll(".osirion-menu-item-icon").ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_WhitespaceIcon_RendersNoIconSlot()
    {
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Item").Add(c => c.Icon, "  "));

        cut.FindAll(".osirion-menu-item-icon").ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_IconTemplate_WinsOverIconName()
    {
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Item")
            .Add(c => c.Icon, "home")
            .Add(c => c.IconTemplate, (RenderFragment)(b => b.AddMarkupContent(0, "<i class=\"custom-icon\"></i>"))));

        var slot = cut.Find(".osirion-menu-item-icon");
        slot.QuerySelector("i.custom-icon").ShouldNotBeNull();
        slot.QuerySelector("svg").ShouldBeNull();
    }

    [Fact]
    public void MenuItem_Description_IsRenderedUnderTheText()
    {
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Docs").Add(c => c.Description, "Read the guides"));

        cut.Find(".osirion-menu-item-content > .osirion-menu-item-description").TextContent.ShouldBe("Read the guides");
    }

    [Fact]
    public void MenuItem_PlainItem_HasOnlyTheBaseClasses()
    {
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Item").Add(c => c.Href, "/item"));

        cut.Find(".osirion-menu-item-wrapper").ClassList.ShouldBe(["osirion-menu-item-wrapper"], ignoreOrder: true);
        cut.Find("a").ClassList.ShouldBe(["osirion-menu-item"], ignoreOrder: true);
        cut.Find("a").HasAttribute("role").ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_ActiveDisabledWithClass_CombinesEveryClass()
    {
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Item")
            .Add(c => c.IsActive, true)
            .Add(c => c.Disabled, true)
            .Add(c => c.Class, "extra"));

        cut.Find(".osirion-menu-item-wrapper").ClassList
            .ShouldBe(["osirion-menu-item-wrapper", "osirion-menu-item-wrapper-active"], ignoreOrder: true);
        cut.Find("a").ClassList
            .ShouldBe(["osirion-menu-item", "osirion-menu-item-active", "osirion-menu-item-disabled", "extra"], ignoreOrder: true);
        cut.Find("a").GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact]
    public void MenuItem_MegaSubmenu_AddsSubmenuAndMegaWrapperClasses()
    {
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Products")
            .Add(c => c.HasSubmenu, true)
            .Add(c => c.SubmenuVariant, SubmenuVariant.Mega));

        cut.Find(".osirion-menu-item-wrapper").ClassList
            .ShouldBe(["osirion-menu-item-wrapper", "osirion-menu-item-has-submenu", "osirion-menu-item-has-mega"], ignoreOrder: true);
    }

    [Fact]
    public void MenuItem_MegaVariantWithoutSubmenu_DoesNotAddTheMegaClass()
    {
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Products")
            .Add(c => c.SubmenuVariant, SubmenuVariant.Mega));

        cut.Find(".osirion-menu-item-wrapper").ClassList.ShouldNotContain("osirion-menu-item-has-mega");
        cut.FindAll(".osirion-submenu").ShouldBeEmpty();
    }

    [Fact]
    public void MenuItem_DisabledWithBlankTarget_RendersNoHrefTargetOrRelAndUsesLinkRole()
    {
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Item")
            .Add(c => c.Href, "https://example.com")
            .Add(c => c.Target, "_BLANK")
            .Add(c => c.Disabled, true));

        var link = cut.Find("a");
        link.HasAttribute("href").ShouldBeFalse();
        link.HasAttribute("target").ShouldBeFalse();
        link.HasAttribute("rel").ShouldBeFalse();
        link.GetAttribute("role").ShouldBe("link");
    }

    [Fact]
    public void MenuItem_UppercaseBlankTarget_StillAddsNoopener()
    {
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Item")
            .Add(c => c.Href, "https://example.com")
            .Add(c => c.Target, "_BLANK"));

        var link = cut.Find("a");
        link.GetAttribute("target").ShouldBe("_BLANK");
        link.GetAttribute("rel").ShouldBe("noopener noreferrer");
    }

    [Fact]
    public void MenuItem_SelfTarget_WritesTargetWithoutRel()
    {
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Item")
            .Add(c => c.Href, "/page")
            .Add(c => c.Target, "_self"));

        var link = cut.Find("a");
        link.GetAttribute("target").ShouldBe("_self");
        link.HasAttribute("rel").ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_Click_InvokesOnClick()
    {
        MouseEventArgs? received = null;
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Item")
            .Add(c => c.OnClick, (MouseEventArgs args) => received = args));

        cut.Find("a").Click(new MouseEventArgs { Detail = 3 });

        received.ShouldNotBeNull();
        received.Detail.ShouldBe(3);
    }

    [Fact]
    public void MenuItem_ClickWhileDisabled_DoesNotInvokeOnClick()
    {
        var clicks = 0;
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Item")
            .Add(c => c.Disabled, true)
            .Add(c => c.OnClick, () => clicks++));

        cut.Find("a").Click();

        clicks.ShouldBe(0);
    }

    [Fact]
    public void MenuItem_ClickWithoutHandler_KeepsTheMarkup()
    {
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Item"));

        cut.Find("a").Click();

        cut.Find(".osirion-menu-item-text").TextContent.ShouldBe("Item");
    }

    [Theory]
    [InlineData("/docs/getting-started?tab=1", "/docs", true)]
    [InlineData("/docs", "/docs/", true)]
    [InlineData("/anything", "/", true)]
    [InlineData("/blog", "/docs", false)]
    public void MenuItem_PrefixMatch_FollowsSegmentBoundaries(string location, string href, bool expectedActive)
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo(location);

        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Item")
            .Add(c => c.Href, href)
            .Add(c => c.Match, NavLinkMatch.Prefix));

        cut.Find("a").HasAttribute("aria-current").ShouldBe(expectedActive);
    }

    [Fact]
    public void MenuItem_ExactMatchIgnoresQueryAndFragment()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/about#team");

        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "About").Add(c => c.Href, "/about?ref=menu"));

        cut.Find("a").GetAttribute("aria-current").ShouldBe("page");
    }

    [Fact]
    public void MenuItem_UnknownMatchMode_IsNeverActive()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/about");

        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "About")
            .Add(c => c.Href, "/about")
            .Add(c => c.Match, (NavLinkMatch)99));

        cut.Find("a").HasAttribute("aria-current").ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_AutoDetectOff_IgnoresTheCurrentLocation()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/about");

        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "About")
            .Add(c => c.Href, "/about")
            .Add(c => c.AutoDetectActive, false));

        cut.Find("a").HasAttribute("aria-current").ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_DisabledOnCurrentLocation_IsNotActive()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/about");

        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "About")
            .Add(c => c.Href, "/about")
            .Add(c => c.Disabled, true));

        cut.Find("a").HasAttribute("aria-current").ShouldBeFalse();
    }

    [Fact]
    public void MenuItem_InteractiveLocationChangeWithoutActiveChange_DoesNotRenderAgain()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/first");
        var cut = Render<MenuItem>(p => p.Add(c => c.Text, "Other").Add(c => c.Href, "/other"));
        var renders = cut.RenderCount;

        navigation.NavigateTo("/second");

        cut.RenderCount.ShouldBe(renders);
    }

    [Fact]
    public void MenuItem_ExplicitId_IsUsedForTheSubmenuIds()
    {
        var cut = Render<MenuItem>(p => p
            .Add(c => c.Text, "Products")
            .Add(c => c.Id, "products")
            .Add(c => c.HasSubmenu, true));

        cut.Find("input.osirion-submenu-toggler").Id.ShouldBe("submenu-products-toggle");
        cut.Find("input.osirion-submenu-toggler").GetAttribute("aria-label").ShouldBe("Toggle Products submenu");
        cut.Find("label.osirion-submenu-toggle").GetAttribute("for").ShouldBe("submenu-products-toggle");
        cut.Find("div.osirion-submenu").Id.ShouldBe("submenu-products");
        cut.Find("div.osirion-submenu").ClassList.ShouldContain("osirion-submenu-regular");
    }
}
