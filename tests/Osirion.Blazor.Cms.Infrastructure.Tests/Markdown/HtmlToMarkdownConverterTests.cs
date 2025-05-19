using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown;

public class HtmlToMarkdownConverterTests
{
    private readonly HtmlToMarkdownConverter _converter;

    public HtmlToMarkdownConverterTests()
    {
        _converter = new HtmlToMarkdownConverter();
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithNullInput_ReturnsEmptyString()
    {
        // Act
        var result = await _converter.ConvertHtmlToMarkdownAsync(null);

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithEmptyInput_ReturnsEmptyString()
    {
        // Act
        var result = await _converter.ConvertHtmlToMarkdownAsync(string.Empty);

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithHeadings_ConvertsToMarkdownHeadings()
    {
        // Arrange
        var html = "<h1>Title</h1><h2>Subtitle</h2><h3>Section</h3>";

        // Act
        var result = await _converter.ConvertHtmlToMarkdownAsync(html);

        // Assert
        result.ShouldContain("# Title");
        result.ShouldContain("## Subtitle");
        result.ShouldContain("### Section");
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithFormattedText_ConvertsToMarkdownFormatting()
    {
        // Arrange
        var html = "<strong>Bold</strong> and <em>italic</em> text";

        // Act
        var result = await _converter.ConvertHtmlToMarkdownAsync(html);

        // Assert
        result.ShouldContain("**Bold** and *italic* text");
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithLinks_ConvertsToMarkdownLinks()
    {
        // Arrange
        var html = "<a href=\"https://example.com\">Link</a>";

        // Act
        var result = await _converter.ConvertHtmlToMarkdownAsync(html);

        // Assert
        result.ShouldContain("[Link](https://example.com)");
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithImages_ConvertsToMarkdownImages()
    {
        // Arrange
        var html = "<img src=\"image.jpg\">";

        // Act
        var result = await _converter.ConvertHtmlToMarkdownAsync(html);

        // Assert
        result.ShouldContain("![](image.jpg)");
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithComplexHtml_RemovesUnhandledTags()
    {
        // Arrange
        var html = "<div><p>Text in <span>paragraph</span> with <unknown>tags</unknown></p></div>";

        // Act
        var result = await _converter.ConvertHtmlToMarkdownAsync(html);

        // Assert
        result.ShouldContain("Text in paragraph with tags");
        result.ShouldNotContain("<div>");
        result.ShouldNotContain("<unknown>");
    }

    [Fact]
    public async Task ConvertHtmlToMarkdownAsync_WithNestedFormatting_ConvertsCorrectly()
    {
        // Arrange
        var html = "<p><strong>Bold <em>and</em> text</strong></p>";

        // Act
        var result = await _converter.ConvertHtmlToMarkdownAsync(html);

        // Assert
        result.ShouldContain("**Bold *and* text**");
    }
}