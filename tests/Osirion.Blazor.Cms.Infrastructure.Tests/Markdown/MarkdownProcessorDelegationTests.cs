using Markdig;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Osirion.Blazor.Core.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown;

public class MarkdownProcessorDelegationTests
{
    private readonly IMarkdownRendererService _renderer = Substitute.For<IMarkdownRendererService>();
    private readonly IMarkdownSanitizer _sanitizer = Substitute.For<IMarkdownSanitizer>();
    private readonly IHtmlToMarkdownConverter _converter = Substitute.For<IHtmlToMarkdownConverter>();
    private readonly IHtmlContentSanitizer _htmlSanitizer = Substitute.For<IHtmlContentSanitizer>();
    private readonly MarkdownProcessor _processor;

    public MarkdownProcessorDelegationTests()
    {
        _processor = new MarkdownProcessor(_renderer, _sanitizer, _converter, _htmlSanitizer);
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        Should.Throw<ArgumentNullException>(() => new MarkdownProcessor(null!, _sanitizer, _converter, _htmlSanitizer));
        Should.Throw<ArgumentNullException>(() => new MarkdownProcessor(_renderer, null!, _converter, _htmlSanitizer));
        Should.Throw<ArgumentNullException>(() => new MarkdownProcessor(_renderer, _sanitizer, null!, _htmlSanitizer));
        Should.Throw<ArgumentNullException>(() => new MarkdownProcessor(_renderer, _sanitizer, _converter, null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task RenderToHtml_WithBlankMarkdown_ReturnsEmptyWithoutRendering(string markdown)
    {
        _processor.RenderToHtml(markdown).ShouldBeEmpty();
        (await _processor.RenderToHtmlAsync(markdown)).ShouldBeEmpty();

        _renderer.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public void RenderToHtml_SanitizesTheRenderedHtmlByDefault()
    {
        _renderer.RenderToHtml("# T", Arg.Any<Action<MarkdownPipelineBuilder>?>()).Returns("<h1>T</h1>");
        _htmlSanitizer.Sanitize("<h1>T</h1>").Returns("clean");

        _processor.RenderToHtml("# T").ShouldBe("clean");
        _processor.RenderToHtml("# T", sanitizeHtml: false).ShouldBe("<h1>T</h1>");
    }

    [Fact]
    public void SanitizeMarkdown_DelegatesToTheMarkdownSanitizer()
    {
        _sanitizer.SanitizeMarkdown("raw").Returns("safe");

        _processor.SanitizeMarkdown("raw").ShouldBe("safe");
    }

    [Fact]
    public void SanitizeHtml_DelegatesToTheHtmlSanitizer()
    {
        _htmlSanitizer.Sanitize("<b>x</b>").Returns("x");

        _processor.SanitizeHtml("<b>x</b>").ShouldBe("x");
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithToken_PassesTheTokenToTheConverter()
    {
        using var cts = new CancellationTokenSource();
        _converter.ConvertHtmlToMarkdownAsync("<b>x</b>", cts.Token).Returns("**x**");

        var result = await _processor.ConvertHtmlToMarkdownAsync("<b>x</b>", cts.Token);

        result.ShouldBe("**x**");
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithoutToken_DelegatesToTheConverter()
    {
        _converter.ConvertHtmlToMarkdownAsync("<i>x</i>", Arg.Any<CancellationToken>()).Returns("*x*");

        var result = await _processor.ConvertHtmlToMarkdownAsync("<i>x</i>");

        result.ShouldBe("*x*");
    }
}
