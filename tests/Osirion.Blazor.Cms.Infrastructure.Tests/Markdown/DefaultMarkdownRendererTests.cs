using Markdig;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown;

public class DefaultMarkdownRendererTests
{
    private readonly DefaultMarkdownRenderer _renderer = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RenderToHtml_WithBlankMarkdown_ReturnsEmpty(string markdown)
    {
        _renderer.RenderToHtml(markdown).ShouldBeEmpty();
    }

    [Fact]
    public void RenderToHtml_WithDefaultPipeline_RendersAdvancedExtensions()
    {
        var html = _renderer.RenderToHtml("| a | b |\n|---|---|\n| 1 | 2 |");

        html.ShouldContain("<table>");
        html.ShouldContain("<td>1</td>");
    }

    [Fact]
    public void RenderToHtml_WithDefaultPipeline_DropsYamlFrontMatter()
    {
        var html = _renderer.RenderToHtml("---\ntitle: Hidden\n---\n# Heading");

        html.ShouldNotContain("Hidden");
        html.ShouldContain("Heading</h1>");
    }

    [Fact]
    public void RenderToHtml_WithConfigureOptions_AppliesTheConfiguredPipeline()
    {
        var configured = false;

        var html = _renderer.RenderToHtml("<b>raw</b>", builder =>
        {
            configured = true;
            builder.DisableHtml();
        });

        configured.ShouldBeTrue();
        html.ShouldNotContain("<b>");
        html.ShouldContain("&lt;b&gt;");
    }

    [Fact]
    public void RenderToHtml_WithoutConfigureOptions_KeepsRawHtml()
    {
        var html = _renderer.RenderToHtml("<b>raw</b>");

        html.ShouldContain("<b>raw</b>");
    }

    [Fact]
    public async Task RenderToHtmlAsync_ReturnsTheSynchronousResult()
    {
        var html = await _renderer.RenderToHtmlAsync("**bold**");

        html.ShouldBe(_renderer.RenderToHtml("**bold**"));
        html.ShouldContain("<strong>bold</strong>");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void SanitizeMarkdown_WithBlankMarkdown_ReturnsEmpty(string markdown)
    {
        _renderer.SanitizeMarkdown(markdown).ShouldBeEmpty();
    }

    [Fact]
    public void SanitizeMarkdown_RemovesScriptBlocks()
    {
        var result = _renderer.SanitizeMarkdown("before<SCRIPT type=\"x\">alert(1)\n</script >after");

        result.ShouldBe("beforeafter");
    }

    [Fact]
    public void SanitizeMarkdown_RemovesInlineEventHandlers()
    {
        var result = _renderer.SanitizeMarkdown("<img src=\"a.png\" onerror=\"alert(1)\">");

        result.ShouldNotContain("onerror");
        result.ShouldContain("src=\"a.png\"");
    }

    [Fact]
    public void SanitizeMarkdown_RemovesIframes()
    {
        var result = _renderer.SanitizeMarkdown("a<iframe src=\"x\"></iframe>b");

        result.ShouldBe("ab");
    }

    [Theory]
    [InlineData("a<object data=\"x\"></object>b")]
    [InlineData("a<embed src=\"x\"></embed>b")]
    public void SanitizeMarkdown_RemovesObjectAndEmbedTags(string markdown)
    {
        _renderer.SanitizeMarkdown(markdown).ShouldBe("ab");
    }

    [Fact]
    public void SanitizeMarkdown_NeutralizesJavaScriptLinks()
    {
        var result = _renderer.SanitizeMarkdown("[click](JavaScript:alert(1)) and [safe](https://example.com)");

        result.ShouldBe("[click](#)) and [safe](https://example.com)");
    }

    [Fact]
    public void SanitizeMarkdown_LeavesSafeMarkdownUnchanged()
    {
        const string markdown = "# Title\n\nSome *text* with a [link](https://example.com).";

        _renderer.SanitizeMarkdown(markdown).ShouldBe(markdown);
    }
}
