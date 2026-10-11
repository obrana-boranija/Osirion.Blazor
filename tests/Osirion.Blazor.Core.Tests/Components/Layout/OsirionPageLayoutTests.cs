using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Core.Tests.Components.Layout;

public class OsirionPageLayoutTests : OsirionComponentTestBase
{
    public OsirionPageLayoutTests()
    {
        Services.AddJSComponents();
    }

    [Fact]
    public void PageLayout_BodyOnly_RendersMainWithoutHeaderOrFooter()
    {
        var cut = RenderStatic<OsirionPageLayout>(parameters => parameters
            .Add(p => p.Body, Text("Body text")));

        cut.Find("main.osirion-page-main").TextContent.Trim().ShouldBe("Body text");
        cut.FindAll("header").ShouldBeEmpty();
        cut.FindAll("footer").ShouldBeEmpty();
    }

    [Fact]
    public void PageLayout_Defaults_UsesStickyFooterAndViewportHeight()
    {
        var cut = RenderStatic<OsirionPageLayout>(parameters => parameters
            .Add(p => p.Body, Text("Body")));

        cut.Find("div").ClassList.ShouldBe(["osirion-page-layout", "osirion-sticky-footer-layout", "osirion-min-height-viewport"]);
    }

    [Fact]
    public void PageLayout_NoStickyFooterContentHeightAndCustomClass_RendersMatchingClasses()
    {
        var cut = RenderStatic<OsirionPageLayout>(parameters => parameters
            .Add(p => p.StickyFooter, false)
            .Add(p => p.MinHeightStrategy, "content")
            .Add(p => p.Class, "site-shell"));

        cut.Find("div").ClassList.ShouldBe(["osirion-page-layout", "osirion-min-height-content", "site-shell"]);
    }

    [Fact]
    public void PageLayout_HeaderAndFooter_RendersBothAroundTheBody()
    {
        var cut = RenderStatic<OsirionPageLayout>(parameters => parameters
            .Add(p => p.Header, Text("Header text"))
            .Add(p => p.Body, Text("Body text"))
            .Add(p => p.Footer, Text("Footer text")));

        cut.Find("header.osirion-page-header").TextContent.Trim().ShouldBe("Header text");
        cut.Find("footer.osirion-page-footer").TextContent.Trim().ShouldBe("Footer text");
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void PageLayout_HeaderScrollBehavior_TogglesTheScrollClass(bool enabled, bool expected)
    {
        var cut = RenderStatic<OsirionPageLayout>(parameters => parameters
            .Add(p => p.Header, Text("Header"))
            .Add(p => p.EnableHeaderScrollBehavior, enabled));

        cut.Find("header").ClassList.Contains("osirion-page-header-scroll").ShouldBe(expected);
    }

    [Fact]
    public void PageLayout_AdditionalAttributes_AreWrittenOnTheRoot()
    {
        var cut = RenderStatic<OsirionPageLayout>(parameters => parameters
            .AddUnmatched("data-testid", "layout"));

        cut.Find("div.osirion-page-layout").GetAttribute("data-testid").ShouldBe("layout");
    }

    private static RenderFragment Text(string text) => builder => builder.AddContent(0, text);
}
