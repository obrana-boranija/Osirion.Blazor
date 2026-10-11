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
}
