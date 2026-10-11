using Bunit;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Layout;

public class OsirionStickySidebarTests : OsirionComponentTestBase
{
    [Fact]
    public void StickySidebar_Defaults_RendersStickyAsideWithoutScrollbarAndChildContent()
    {
        var cut = RenderStatic<OsirionStickySidebar>(parameters => parameters
            .AddChildContent("<nav>Contents</nav>"));

        var aside = cut.Find("aside");
        aside.ClassList.ShouldBe(["osirion-sticky-sidebar", "osirion-no-scrollbar"]);
        aside.QuerySelector("nav")!.TextContent.ShouldBe("Contents");
    }

    [Fact]
    public void StickySidebar_NotStickyWithScrollbarAndCustomClass_RendersMatchingClasses()
    {
        var cut = RenderStatic<OsirionStickySidebar>(parameters => parameters
            .Add(p => p.IsSticky, false)
            .Add(p => p.HideScrollbar, false)
            .Add(p => p.Class, "docs-sidebar"));

        cut.Find("aside").ClassList.ShouldBe(["osirion-sticky-sidebar", "osirion-non-sticky", "docs-sidebar"]);
    }
}
