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
}
