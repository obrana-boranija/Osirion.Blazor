using Bunit;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Navigation;

public class OsirionBreadcrumbsTests : OsirionComponentTestBase
{
    // Fails before the fix: no item carried aria-current, so a screen reader could not tell which crumb is the page.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Breadcrumbs_LastSegment_IsMarkedAsTheCurrentPage(bool linkLastItem)
    {
        var cut = RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/blog/post")
            .Add(p => p.LinkLastItem, linkLastItem));

        var current = cut.FindAll("[aria-current]");
        current.Count.ShouldBe(1);
        current[0].GetAttribute("aria-current").ShouldBe("page");
        current[0].Closest("li")!.ClassList.ShouldContain("osirion-breadcrumbs-current");
    }

    // Fails before the fix: the last crumb was always built from the slug, so "Pre-MVP: make it right" read
    // "Pre Mvp Make It Right".
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Breadcrumbs_CurrentPageText_NamesTheLastCrumb(bool linkLastItem)
    {
        var cut = RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/blog/pre-mvp-make-it-right")
            .Add(p => p.CurrentPageText, "Pre-MVP: make it right")
            .Add(p => p.LinkLastItem, linkLastItem));

        cut.Find("[aria-current]").TextContent.Trim().ShouldBe("Pre-MVP: make it right");
    }

    [Fact]
    public void Breadcrumbs_CurrentPageText_LeavesEarlierCrumbsFormattedFromTheirSegments()
    {
        var cut = RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/blog/pre-mvp-make-it-right")
            .Add(p => p.CurrentPageText, "Pre-MVP: make it right"));

        cut.FindAll("li a").Select(a => a.TextContent.Trim()).ShouldBe(["Home", "Blog"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Breadcrumbs_WithoutCurrentPageText_FormatsTheLastSegment(string? currentPageText)
    {
        var cut = RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/blog/pre-mvp-make-it-right")
            .Add(p => p.CurrentPageText, currentPageText));

        cut.Find("[aria-current]").TextContent.Trim().ShouldBe("Pre Mvp Make It Right");
    }

    // Fails before the fix: segments were shown as they sit in the URL, so "/docs/caf%C3%A9" read "Caf%C3%A9".
    [Fact]
    public void Breadcrumbs_PercentEncodedSegment_IsDecodedForDisplayAndKeptEncodedInTheHref()
    {
        var cut = RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/caf%C3%A9/menu")
            .Add(p => p.LinkLastItem, true));

        var links = cut.FindAll("li a");
        links.Select(a => a.TextContent.Trim()).ShouldBe(["Home", "Café", "Menu"]);
        links.Select(a => a.GetAttribute("href")).ShouldBe(["/", "/caf%C3%A9", "/caf%C3%A9/menu"]);
    }

    // Fails before the fix: see Breadcrumbs_PercentEncodedSegment_IsDecodedForDisplayAndKeptEncodedInTheHref.
    [Fact]
    public void Breadcrumbs_PercentEncodedLastSegment_IsDecodedForDisplay()
    {
        var cut = RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/docs/caf%C3%A9"));

        cut.Find("[aria-current]").TextContent.Trim().ShouldBe("Café");
    }

    [Theory]
    [InlineData("100%", "100%")]
    [InlineData("%E0%A4%A", "%E0%A4%A")]
    [InlineData("50%zz-off", "50%zz Off")]
    public void Breadcrumbs_InvalidEscape_IsShownAsWrittenWithoutThrowing(string segment, string shown)
    {
        var cut = RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/docs/" + segment));

        cut.Find("[aria-current]").TextContent.Trim().ShouldBe(shown);
    }

    [Fact]
    public void Breadcrumbs_SegmentFormatter_StillReceivesTheRawSlug()
    {
        string? received = null;
        RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/caf%C3%A9")
            .Add(p => p.SegmentFormatter, slug => received = slug));

        received.ShouldBe("caf%C3%A9");
    }

    // Fails before the fix: Style is declared on the base component but the nav never wrote it, so a style in the
    // markup was dropped.
    [Fact]
    public void Breadcrumbs_Style_IsWrittenOnTheNav()
    {
        var cut = RenderStatic<OsirionBreadcrumbs>(parameters => parameters
            .Add(p => p.Path, "/docs")
            .Add(p => p.Style, "color: red;"));

        cut.Find("nav").GetAttribute("style").ShouldBe("color: red;");
    }
}
