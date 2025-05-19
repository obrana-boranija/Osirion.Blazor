using Markdig;
using Markdig.Syntax;
using NSubstitute;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown;

public class MarkdigRendererTests
{
    private readonly MarkdigRenderer _renderer;

    public MarkdigRendererTests()
    {
        _renderer = new MarkdigRenderer();
    }

    [Fact]
    public void RenderToHtml_WithValidMarkdown_RendersCorrectHtml()
    {
        // Arrange
        var markdown = "# Heading\nThis is a **bold** paragraph.";

        // Act
        var result = _renderer.RenderToHtml(markdown);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldContain("<h1 id=\"heading\">Heading</h1>");
        result.ShouldContain("<p>This is a <strong>bold</strong> paragraph.</p>");
    }

    [Fact]
    public void RenderToHtml_WithEmptyMarkdown_ReturnsEmptyString()
    {
        // Arrange
        var markdown = string.Empty;

        // Act
        var result = _renderer.RenderToHtml(markdown);

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void RenderToHtml_WithNullMarkdown_ReturnsEmptyString()
    {
        // Act
        var result = _renderer.RenderToHtml(null);

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void RenderToHtml_WithConfigOptions_UsesExtendedConfiguration()
    {
        // Arrange
        var markdown = "> This is a blockquote with a [link](https://example.com)";
        bool configureOptionsCalled = false;

        // Act
        var result = _renderer.RenderToHtml(markdown, options => {
            // Verify that options is a MarkdownPipelineBuilder
            options.ShouldBeOfType<MarkdownPipelineBuilder>();
            configureOptionsCalled = true;
        });

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldContain("<blockquote>");
        result.ShouldContain("<a href=\"https://example.com\">link</a>");
        configureOptionsCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task RenderToHtmlAsync_WithValidMarkdown_RendersCorrectHtml()
    {
        // Arrange
        var markdown = "# Heading\nThis is a **bold** paragraph.";

        // Act
        var result = await _renderer.RenderToHtmlAsync(markdown);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldContain("<h1 id=\"heading\">Heading</h1>");
        result.ShouldContain("<p>This is a <strong>bold</strong> paragraph.</p>");
    }

    [Fact]
    public async Task RenderToHtmlAsync_WithConfigOptions_UsesExtendedConfiguration()
    {
        // Arrange
        var markdown = "```csharp\nvar x = 42;\n```";
        bool configureOptionsCalled = false;

        // Act
        var result = await _renderer.RenderToHtmlAsync(markdown, options => {
            options.ShouldBeOfType<MarkdownPipelineBuilder>();
            configureOptionsCalled = true;
        });

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldContain("<pre><code class=\"language-csharp\">");
        configureOptionsCalled.ShouldBeTrue();
    }

    [Fact]
    public void ObjectRenderers_ThrowsNotImplementedException()
    {
        // Act & Assert
        Should.Throw<NotImplementedException>(() => _renderer.ObjectRenderers);
    }

    [Fact]
    public void Render_ThrowsNotImplementedException()
    {
        // Arrange
        var markdownObject = Substitute.For<MarkdownObject>();

        // Act & Assert
        Should.Throw<NotImplementedException>(() => _renderer.Render(markdownObject));
    }

    [Fact]
    public void Events_CanBeSubscribedTo()
    {
        // Arrange
        bool beforeEventFired = false;
        bool afterEventFired = false;
        var markdownObject = Substitute.For<MarkdownObject>();

        // Act
        _renderer.ObjectWriteBefore += (renderer, obj) => {
            renderer.ShouldBe(_renderer);
            obj.ShouldBe(markdownObject);
            beforeEventFired = true;
        };

        _renderer.ObjectWriteAfter += (renderer, obj) => {
            renderer.ShouldBe(_renderer);
            obj.ShouldBe(markdownObject);
            afterEventFired = true;
        };

        // Assert - events are subscribed but not raised
        beforeEventFired.ShouldBeFalse();
        afterEventFired.ShouldBeFalse();
    }
}