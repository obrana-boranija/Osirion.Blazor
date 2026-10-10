using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Core.Services.Implementations;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown;

public class MarkdownProcessorTests
{
    private static readonly string[] ActiveElements = ["script", "iframe", "object", "embed", "form", "input", "svg", "style"];

    private readonly MarkdownProcessor _processor = new(new DefaultMarkdownRenderer(), new MarkdownSanitizer(), new HtmlToMarkdownConverter());

    // Each row is a way to get active markup out of markdown with the default Markdig pipeline
    // (raw HTML, links, generic attributes and media links). The precondition proves the vector reaches the
    // rendered HTML, so the test fails whenever the rendered output is not sanitized.
    [Theory]
    [InlineData("Intro\n\n<script>alert(1)</script>\n\nOutro")]
    [InlineData("Text <img src=\"x.png\" onerror=\"alert(1)\"> more")]
    [InlineData("[click](javascript:alert(1))")]
    [InlineData("# Title {onclick=\"alert(1)\"}")]
    [InlineData("![video](https://www.youtube.com/watch?v=dQw4w9WgXcQ)")]
    [InlineData("<iframe src=\"https://evil.example\"></iframe>")]
    [InlineData("<form action=\"https://evil.example\"><input name=\"password\"></form>")]
    [InlineData("<div style=\"position:fixed;inset:0\">overlay</div>")]
    public async Task RenderToHtmlAsync_DefaultArgument_NeutralizesMarkdownXssVector(string markdown)
    {
        var raw = await _processor.RenderToHtmlAsync(markdown, sanitizeHtml: false);
        HasActiveMarkup(raw).ShouldBeTrue();

        var html = await ((IMarkdownProcessor)_processor).RenderToHtmlAsync(markdown);

        HasActiveMarkup(html).ShouldBeFalse();
    }

    [Fact]
    public void RenderToHtml_DefaultArgument_RemovesEventHandler()
    {
        var html = ((IMarkdownProcessor)_processor).RenderToHtml("Text <img src=\"x.png\" alt=\"x\" onerror=\"alert(1)\">");

        var image = Parse(html).QuerySelector("img").ShouldNotBeNull();
        image.HasAttribute("onerror").ShouldBeFalse();
        image.GetAttribute("src").ShouldBe("x.png");
    }

    [Fact]
    public async Task RenderToHtmlAsync_HeadingsAndCodeBlocks_KeepsIdsAndLanguageClasses()
    {
        var html = await _processor.RenderToHtmlAsync("## Getting Started\n\n```csharp\nvar total = a < b && c > d;\n```");

        var document = Parse(html);
        document.QuerySelector("h2#getting-started").ShouldNotBeNull();
        document.QuerySelector("pre code.language-csharp").ShouldNotBeNull().TextContent.ShouldBe("var total = a < b && c > d;\n");
    }

    // The markdown-level sanitizer used to run in this path and replaced JavaScript samples.
    [Fact]
    public async Task RenderToHtmlAsync_JavaScriptFencedSample_KeepsTheSample()
    {
        var html = await _processor.RenderToHtmlAsync("```js\nconsole.log('hi');\n```");

        Parse(html).QuerySelector("code.language-js").ShouldNotBeNull().TextContent.ShouldBe("console.log('hi');\n");
    }

    [Fact]
    public async Task RenderToHtmlAsync_SanitizeHtmlFalse_ReturnsRendererOutputUnchanged()
    {
        const string markdown = "<p onclick=\"track()\">trusted</p>";

        var html = await _processor.RenderToHtmlAsync(markdown, sanitizeHtml: false);

        html.ShouldBe(new DefaultMarkdownRenderer().RenderToHtml(markdown));
    }

    // Shape of the consumer's multi-line chart elements: the host allow-list must reach the processor.
    [Fact]
    public async Task RenderToHtmlAsync_InjectedSanitizerWithHostAllowList_KeepsCustomElement()
    {
        var options = new HtmlContentSanitizerOptions().AllowElement("chart", "type", "title", "data");
        var processor = new MarkdownProcessor(
            new DefaultMarkdownRenderer(), new MarkdownSanitizer(), new HtmlToMarkdownConverter(),
            new HtmlContentSanitizer(Options.Create(options)));

        var html = await processor.RenderToHtmlAsync("Intro\n\n<chart type=\"bar\"\n       title=\"Costs\"\n       data=\"[[1, 2], [3, 4]]\"\n       onclick=\"alert(1)\" />\n\nOutro");

        var chart = Parse(html).QuerySelector("chart").ShouldNotBeNull();
        chart.Attributes.Select(attribute => $"{attribute.Name}={attribute.Value}").ToArray()
            .ShouldBe(new[] { "type=bar", "title=Costs", "data=[[1, 2], [3, 4]]" });
    }

    [Fact]
    public void SanitizeHtml_RawHtml_RemovesActiveMarkup()
    {
        var html = _processor.SanitizeHtml("<p onclick=\"alert(1)\">text</p><script>alert(2)</script>");

        html.ShouldBe("<p>text</p>");
    }

    private static IDocument Parse(string html) =>
        new HtmlParser().ParseDocument("<!doctype html><html><body>" + html);

    private static bool HasActiveMarkup(string html)
    {
        var elements = Parse(html).Body!.QuerySelectorAll("*");

        return elements.Any(element => ActiveElements.Contains(element.LocalName))
            || elements.SelectMany(element => element.Attributes).Any(attribute =>
                attribute.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase)
                || attribute.Name == "style"
                || attribute.Value.TrimStart().StartsWith("javascript:", StringComparison.OrdinalIgnoreCase));
    }
}
