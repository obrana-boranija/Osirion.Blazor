using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown;

public class MarkdownSanitizerTests
{
    private readonly MarkdownSanitizer _sanitizer;

    public MarkdownSanitizerTests()
    {
        _sanitizer = new MarkdownSanitizer();
    }

    [Fact]
    public void SanitizeMarkdown_WithNullInput_ReturnsEmptyString()
    {
        // Act
        var result = _sanitizer.SanitizeMarkdown(null);

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void SanitizeMarkdown_WithEmptyInput_ReturnsEmptyString()
    {
        // Act
        var result = _sanitizer.SanitizeMarkdown(string.Empty);

        // Assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void SanitizeMarkdown_WithCleanMarkdown_ReturnsSameMarkdown()
    {
        // Arrange
        var markdown = "# Title\n\nThis is a **paragraph** with _formatting_.";

        // Act
        var result = _sanitizer.SanitizeMarkdown(markdown);

        // Assert
        result.ShouldBe(markdown);
    }

    [Fact]
    public void SanitizeMarkdown_WithJavaScriptBlock_RemovesJavaScriptBlock()
    {
        // Arrange
        var markdown = "# Title\n\n```javascript\nalert('xss');\n```\n\nText after";
        var expected = "# Title\n\n```\nCode block removed for security reasons\n```\n\nText after";

        // Act
        var result = _sanitizer.SanitizeMarkdown(markdown);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void SanitizeMarkdown_WithJsBlock_RemovesJsBlock()
    {
        // Arrange
        var markdown = "# Title\n\n```js\nalert('xss');\n```\n\nText after";
        var expected = "# Title\n\n```\nCode block removed for security reasons\n```\n\nText after";

        // Act
        var result = _sanitizer.SanitizeMarkdown(markdown);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void SanitizeMarkdown_WithInlineJavascript_RemovesJavascript()
    {
        // Arrange
        var markdown = "Click [here](`javascript:alert('xss')`) to continue";
        var expected = "Click [here](`[code removed]`) to continue";

        // Act
        var result = _sanitizer.SanitizeMarkdown(markdown);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public void SanitizeMarkdown_WithMultipleJavascriptBlocks_RemovesAllBlocks()
    {
        // Arrange
        var markdown = "```javascript\nalert('xss1');\n```\nText\n```js\nalert('xss2');\n```";
        var expected = "```\nCode block removed for security reasons\n```\nText\n```\nCode block removed for security reasons\n```";

        // Act
        var result = _sanitizer.SanitizeMarkdown(markdown);

        // Assert
        result.ShouldBe(expected);
    }
}