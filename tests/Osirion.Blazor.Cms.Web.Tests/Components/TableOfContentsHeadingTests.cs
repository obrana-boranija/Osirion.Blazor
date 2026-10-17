using Bunit;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Web.Components;
using Osirion.Blazor.Cms.Web.Tests.Base;
using Shouldly;

namespace Osirion.Blazor.Cms.Web.Tests.Components;

public class TableOfContentsHeadingTests : CmsWebComponentTestBase
{
    private readonly RecordingLoggerFactory _logs = new();

    public TableOfContentsHeadingTests() => _logs.Register(Services);

    // Fails before the fix: the pattern had no Singleline option, so a heading whose text runs over two lines did not match and was missing from the contents.
    [Fact]
    public void TableOfContents_HeadingTextOverSeveralLines_IsListed()
    {
        var cut = RenderStatic<TableOfContents>(p => p
            .Add(c => c.Content, "<h2 id=\"two-lines\">First line\nsecond line</h2><h2 id=\"next\">Next</h2>"));

        var links = cut.FindAll("a.osirion-toc-link");
        links.Select(a => a.GetAttribute("href")).ShouldBe(["/#two-lines", "/#next"]);
        links[0].TextContent.ShouldContain("First line");
        links[0].TextContent.ShouldContain("second line");
    }

    // Fails before the fix: the id was read only when it was the first attribute, so the link pointed at an id generated from the text, not at the heading's own id.
    [Theory]
    [InlineData("<h2 class=\"title\" id=\"custom\">Intro</h2>")]
    [InlineData("<h2 data-x=\"1\" id='custom'>Intro</h2>")]
    [InlineData("<h2\n  class=\"title\"\n  id=custom>Intro</h2>")]
    [InlineData("<H2 CLASS=\"title\" ID=\"custom\">Intro</H2>")]
    public void TableOfContents_IdAfterOtherAttributes_IsUsed(string html)
    {
        var cut = RenderStatic<TableOfContents>(p => p.Add(c => c.Content, html));

        cut.Find("a.osirion-toc-link").GetAttribute("href").ShouldBe("/#custom");
    }

    [Fact]
    public void TableOfContents_AttributeEndingInId_IsNotTakenForTheId()
    {
        var cut = RenderStatic<TableOfContents>(p => p.Add(c => c.Content, "<h2 data-id=\"wrong\">Intro</h2>"));

        cut.Find("a.osirion-toc-link").GetAttribute("href").ShouldBe("/#intro");
    }

    // Fails before the fix: the pattern had no match timeout, so a long input of unclosed headings kept the render busy for minutes instead of ending after a second.
    [Fact]
    public async Task TableOfContents_HostileContent_EndsAtTheMatchTimeoutAndListsNothing()
    {
        var content = string.Concat(Enumerable.Repeat("<h2>a", 150_000));

        var render = Task.Run(() => RenderStatic<TableOfContents>(p => p.Add(c => c.Content, content)));
        var winner = await Task.WhenAny(render, Task.Delay(TimeSpan.FromSeconds(30)));

        winner.ShouldBeSameAs(render, "the heading search must stop at its match timeout");
        (await render).FindAll("a.osirion-toc-link").ShouldBeEmpty();
        _logs.Entries.ShouldContain(e => e.Level == LogLevel.Warning);
    }
}
