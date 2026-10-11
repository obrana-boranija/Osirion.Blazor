using Bunit;
using Osirion.Blazor.Cms.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

// Markdown files open with "# Title". A component that already shows the item's title as its heading showed it a
// second time from the body, and the page had two h1 elements. Each component test fails before the fix.
public class ContentTitleHeadingTests : CmsWebComponentTestBase
{
    private const string Body = "<h1 id=\"getting-started\">Getting started</h1>\n<p>Install the package.</p>";

    [Theory]
    [InlineData("<h1>Title</h1><p>Body</p>", "<p>Body</p>")]
    [InlineData("\n  <h1 id=\"title\" class=\"x\">Title</h1>\n<p>Body</p>", "<p>Body</p>")]
    [InlineData("<H1>Title with <code>code</code></H1><p>Body</p>", "<p>Body</p>")]
    [InlineData("<h1>First</h1><p>Body</p><h1>Second</h1>", "<p>Body</p><h1>Second</h1>")]
    public void WithoutLeadingTitle_RemovesOnlyTheOpeningH1(string html, string expected)
    {
        ContentHeadings.WithoutLeadingTitle(html).ShouldBe(expected);
    }

    [Theory]
    [InlineData("<p>Intro</p><h1>Title</h1>")]
    [InlineData("<h2>Section</h2><p>Body</p>")]
    [InlineData("<h1x>Not a heading</h1x>")]
    [InlineData("")]
    [InlineData(null)]
    public void WithoutLeadingTitle_LeavesContentThatDoesNotOpenWithAnH1(string? html)
    {
        ContentHeadings.WithoutLeadingTitle(html).ShouldBe(html);
    }

    [Fact]
    public void ContentView_ShowsTheTitleOnce()
    {
        var cut = RenderStatic<ContentView>(p => p.Add(c => c.Item, Item()));

        cut.FindAll("h1").Select(h => h.TextContent.Trim()).ShouldBe(["Getting started"]);
        cut.Find(".osirion-content-body").TextContent.ShouldContain("Install the package.");
    }

    [Fact]
    public void DocumentPage_ShowsTheTitleOnce()
    {
        var cut = RenderStatic<DocumentPage>(p => p.Add(c => c.Content, Item()));

        cut.FindAll("h1").Select(h => h.TextContent.Trim()).ShouldBe(["Getting started"]);
    }

    [Fact]
    public void LocalizedContentView_WithItsHeader_ShowsTheTitleOnce()
    {
        var cut = RenderStatic<LocalizedContentView>(p => p.Add(c => c.Item, Item()));

        cut.FindAll("h1").Count.ShouldBe(1);
    }

    [Fact]
    public void LocalizedContentView_WithoutItsHeader_KeepsTheContentsTitle()
    {
        var cut = RenderStatic<LocalizedContentView>(p => p
            .Add(c => c.Item, Item())
            .Add(c => c.ShowJumbotron, false));

        cut.Find(".osirion-content-body h1").TextContent.ShouldBe("Getting started");
    }

    [Fact]
    public void ContentRenderer_ByDefault_KeepsTheContentsTitle()
    {
        var cut = RenderStatic<ContentRenderer>(p => p.Add(c => c.Item, Item()));

        cut.Find("h1").TextContent.ShouldBe("Getting started");
    }

    [Fact]
    public void ContentRenderer_OmitLeadingTitle_LeavesOutTheContentsTitle()
    {
        var cut = RenderStatic<ContentRenderer>(p => p
            .Add(c => c.Item, Item())
            .Add(c => c.OmitLeadingTitle, true));

        cut.FindAll("h1").ShouldBeEmpty();
        cut.Find("p").TextContent.ShouldBe("Install the package.");
    }

    private static ContentItem Item()
    {
        var item = ContentItem.Create("getting-started", "Getting started", Body, "docs/getting-started.md", "test");
        item.SetUrl("docs/getting-started");
        return item;
    }
}
