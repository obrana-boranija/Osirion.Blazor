using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Core.Services;
using Osirion.Blazor.Cms.Core.Tests.TestFixtures;
using Shouldly;

namespace Osirion.Blazor.Cms.Core.Tests.Services;

public class MarkdownProcessorTests
{
    private readonly ILogger<MarkdownProcessor> _logger;
    private readonly MarkdownProcessor _processor;

    public MarkdownProcessorTests()
    {
        _logger = Substitute.For<ILogger<MarkdownProcessor>>();
        _processor = new MarkdownProcessor(_logger);
    }

    [Fact]
    public void RenderToHtml_WithValidMarkdown_ShouldReturnHtml()
    {
        // Arrange
        var markdown = TestData.BasicMarkdown;

        // Act
        var result = _processor.RenderToHtml(markdown);

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("<h1 id=\"test-heading\">Test Heading</h1>");
        result.ShouldContain("<strong>bold</strong>");
        result.ShouldContain("<em>italic</em>");
        result.ShouldContain("<li>List item 1</li>");
    }

    [Fact]
    public void RenderToHtml_WithNullOrEmptyMarkdown_ShouldReturnEmpty()
    {
        // Arrange & Act
        var result1 = _processor.RenderToHtml(null);
        var result2 = _processor.RenderToHtml(string.Empty);
        var result3 = _processor.RenderToHtml("   ");

        // Assert
        result1.ShouldBeEmpty();
        result2.ShouldBeEmpty();
        result3.ShouldBeEmpty();
    }

    [Fact]
    public void RenderToHtml_WithFrontMatter_ShouldRemoveFrontMatterAndRenderContent()
    {
        // Arrange
        var markdownWithFrontMatter = TestData.MarkdownWithFrontmatter;

        // Act
        var result = _processor.RenderToHtml(markdownWithFrontMatter);

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("<h1 id=\"test-heading\">Test Heading</h1>");
        result.ShouldNotContain("title: Test Page");
    }

    [Fact]
    public async Task RenderToHtmlAsync_ShouldMatchSyncVersion()
    {
        // Arrange
        var markdown = TestData.BasicMarkdown;

        // Act
        var syncResult = _processor.RenderToHtml(markdown);
        var asyncResult = await _processor.RenderToHtmlAsync(markdown);

        // Assert
        asyncResult.ShouldBe(syncResult);
    }

    [Fact]
    public void ExtractFrontMatter_WithValidFrontMatter_ShouldReturnMetadata()
    {
        // Arrange
        var markdownWithFrontMatter = TestData.MarkdownWithFrontmatter;

        // Act
        var metadata = _processor.ExtractFrontMatter(markdownWithFrontMatter);

        // Assert
        metadata.ShouldNotBeNull();
        metadata.Count.ShouldBeGreaterThan(0);
        metadata.ShouldContainKey("title");
        metadata["title"].ShouldBe("Test Page");
        metadata.ShouldContainKey("description");
        metadata["description"].ShouldBe("This is a test page");
        metadata.ShouldContainKey("date");
        metadata["date"].ShouldBe("2025-05-01");
        metadata.ShouldContainKey("author");
        metadata["author"].ShouldBe("Test Author");
        metadata.ShouldContainKey("tags");
        metadata["tags"].ShouldBe("[test, markdown]");
    }

    [Fact]
    public void ExtractFrontMatter_WithNoFrontMatter_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var markdown = TestData.BasicMarkdown;

        // Act
        var metadata = _processor.ExtractFrontMatter(markdown);

        // Assert
        metadata.ShouldNotBeNull();
        metadata.Count.ShouldBe(0);
    }

    [Fact]
    public void SanitizeMarkdown_ShouldRemoveDangerousContent()
    {
        // Arrange
        var unsafeMarkdown = @"
                                # Test Heading

                                <script>alert('XSS')</script>

                                [Click me](javascript:alert('XSS'))

                                <iframe src=""https://malicious.com""></iframe>

                                <div onclick=""alert('XSS')"">Click me</div>
                                ";

        // Act
        var sanitized = _processor.SanitizeMarkdown(unsafeMarkdown);

        // Assert
        sanitized.ShouldNotContain("<script>");
        sanitized.ShouldNotContain("</script>");
        sanitized.ShouldNotContain("javascript:");
        sanitized.ShouldNotContain("<iframe");
        sanitized.ShouldNotContain("onclick=");
        sanitized.ShouldContain("# Test Heading");
        sanitized.ShouldContain("[Click me](#)");
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_ShouldConvertBasicFormatting()
    {
        // Arrange
        var html = "<h1>Title</h1><p>This is <strong>bold</strong> and <em>italic</em> text.</p><ul><li>Item 1</li><li>Item 2</li></ul>";

        // Act
        var markdown = await _processor.ConvertHtmlToMarkdownAsync(html);

        // Assert
        markdown.ShouldNotBeNullOrWhiteSpace();
        markdown.ShouldContain("Title");
        markdown.ShouldContain("This is bold and italic text.");
        markdown.ShouldContain("Item 1");
        markdown.ShouldContain("Item 2");

        // Check that HTML tags are removed
        markdown.ShouldNotContain("<h1>");
        markdown.ShouldNotContain("<p>");
        markdown.ShouldNotContain("<strong>");
        markdown.ShouldNotContain("<em>");
    }
}