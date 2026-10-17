using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class MenuGroupDisclosureTests : BunitContext
{
    public MenuGroupDisclosureTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));
    }

    // Fails before the fix: the label was a plain div and the items were hidden by a class nothing could toggle, so a
    // group with Expanded="false" stayed closed with no control, for a mouse and for the keyboard alike.
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MenuGroup_Collapsible_LabelIsTheSummaryOfADetailsThatHoldsTheItems(bool expanded)
    {
        var cut = Render<MenuGroup>(p => p
            .Add(c => c.Label, "Guides")
            .Add(c => c.Collapsible, true)
            .Add(c => c.Expanded, expanded)
            .AddChildContent("<a href=\"/a\">A</a>"));

        var details = cut.Find("details");
        details.HasAttribute("open").ShouldBe(expanded);
        var summary = details.QuerySelector(":scope > summary.osirion-menu-group-label")!;
        summary.TextContent.Trim().ShouldBe("Guides");
        details.QuerySelector(":scope > .osirion-menu-group-items a")!.GetAttribute("href").ShouldBe("/a");
        cut.Find(".osirion-menu-group-items").GetAttribute("aria-labelledby").ShouldBe(summary.Id);
    }

    [Fact]
    public void MenuGroup_NotCollapsible_RendersNoDetails()
    {
        var cut = Render<MenuGroup>(p => p
            .Add(c => c.Label, "Guides")
            .AddChildContent("A"));

        cut.FindAll("details").ShouldBeEmpty();
        cut.Find("div.osirion-menu-group-label").TextContent.Trim().ShouldBe("Guides");
    }

    // Fails before the fix: a stylesheet rule hid the items of any collapsible group that was not expanded, label or
    // not, and with no label there was nothing to open it again.
    [Fact]
    public void MenuGroup_CollapsibleWithoutLabel_StaysOpenBecauseThereIsNothingToToggle()
    {
        var cut = Render<MenuGroup>(p => p
            .Add(c => c.Collapsible, true)
            .Add(c => c.Expanded, false)
            .AddChildContent("A"));

        cut.FindAll("details").ShouldBeEmpty();
        File.ReadAllText(MenuBrandLogoTests.StylesheetPath("MenuGroup.razor.css"))
            .ShouldNotContain(":not(.osirion-menu-group-expanded) .osirion-menu-group-items");
    }
}
