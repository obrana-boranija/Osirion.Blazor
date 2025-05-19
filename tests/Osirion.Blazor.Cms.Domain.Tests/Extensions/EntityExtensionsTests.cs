using Osirion.Blazor.Cms.Domain.Extensions;

namespace Osirion.Blazor.Cms.Tests.Domain.Extensions;

public class EntityExtensionsTests
{
    [Fact]
    public void GenerateSlug_WithNormalText_ReturnsSlug()
    {
        // Arrange
        string text = "This is a test title";

        // Act
        string slug = text.GenerateSlug();

        // Assert
        Assert.Equal("this-is-a-test-title", slug);
    }

    [Fact]
    public void GenerateSlug_WithSpecialCharacters_RemovesSpecialCharacters()
    {
        // Arrange
        string text = "Test: Title! @#$%^&*()";

        // Act
        string slug = text.GenerateSlug();

        // Assert
        Assert.Equal("test-title", slug);
    }

    [Fact]
    public void GenerateSlug_WithMultipleSpaces_ReplaceWithSingleHyphen()
    {
        // Arrange
        string text = "This   has    many   spaces";

        // Act
        string slug = text.GenerateSlug();

        // Assert
        Assert.Equal("this-has-many-spaces", slug);
    }

    [Fact]
    public void GenerateSlug_WithMultipleHyphens_ConsolidatesHyphens()
    {
        // Arrange
        string text = "Test--with---hyphens";

        // Act
        string slug = text.GenerateSlug();

        // Assert
        Assert.Equal("test-with-hyphens", slug);
    }

    [Fact]
    public void GenerateSlug_WithLeadingAndTrailingHyphens_TrimsHyphens()
    {
        // Arrange
        string text = "-Test with hyphens-";

        // Act
        string slug = text.GenerateSlug();

        // Assert
        Assert.Equal("test-with-hyphens", slug);
    }

    [Fact]
    public void GenerateSlug_WithEmptyString_ReturnsUntitled()
    {
        // Arrange
        string text = "";

        // Act
        string slug = text.GenerateSlug();

        // Assert
        Assert.Equal("untitled", slug);
    }

    [Fact]
    public void GenerateSlug_WithNullString_ReturnsUntitled()
    {
        // Arrange
        string text = null;

        // Act
        string slug = text.GenerateSlug();

        // Assert
        Assert.Equal("untitled", slug);
    }

    [Fact]
    public void GenerateSlug_WithOnlySpecialCharacters_ReturnsUntitled()
    {
        // Arrange
        string text = "@#$%^&*()";

        // Act
        string slug = text.GenerateSlug();

        // Assert
        Assert.Equal("untitled", slug);
    }

    [Fact]
    public void IsValidSlug_WithValidSlug_ReturnsTrue()
    {
        // Arrange
        var validSlugs = new[]
        {
            "test",
            "test-slug",
            "test-123",
            "123-test",
            "a-very-long-slug-with-many-words-and-numbers-123456789"
        };

        // Act & Assert
        foreach (var slug in validSlugs)
        {
            Assert.True(slug.IsValidSlug(), $"Slug '{slug}' should be valid");
        }
    }

    [Fact]
    public void IsValidSlug_WithInvalidSlug_ReturnsFalse()
    {
        // Arrange
        var invalidSlugs = new[]
        {
            "Test",           // Uppercase
            "test slug",      // Space
            "test_slug",      // Underscore
            "test.slug",      // Period
            "test@slug",      // Special character
            "tést",           // Accent
            "",               // Empty string
            " "               // Whitespace
        };

        // Act & Assert
        foreach (var slug in invalidSlugs)
        {
            Assert.False(slug.IsValidSlug(), $"Slug '{slug}' should be invalid");
        }
    }

    [Fact]
    public void EscapeYamlString_WithBasicString_ReturnsUnchanged()
    {
        // Arrange
        string input = "This is a basic string";

        // Act
        string result = input.EscapeYamlString();

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void EscapeYamlString_WithSpecialCharacters_EscapesCharacters()
    {
        // Arrange
        string input = "String with \"quotes\" and \\ backslash";
        string expected = "String with \\\"quotes\\\" and \\\\ backslash";

        // Act
        string result = input.EscapeYamlString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EscapeYamlString_WithNewlines_EscapesNewlines()
    {
        // Arrange
        string input = "Line 1\nLine 2\rLine 3\r\nLine 4";
        string expected = "Line 1\\nLine 2\\rLine 3\\r\\nLine 4";

        // Act
        string result = input.EscapeYamlString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EscapeYamlString_WithTabs_EscapesTabs()
    {
        // Arrange
        string input = "Column 1\tColumn 2\tColumn 3";
        string expected = "Column 1\\tColumn 2\\tColumn 3";

        // Act
        string result = input.EscapeYamlString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EscapeYamlString_WithEmptyString_ReturnsEmptyString()
    {
        // Arrange
        string input = "";

        // Act
        string result = input.EscapeYamlString();

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void EscapeYamlString_WithNullString_ReturnsNull()
    {
        // Arrange
        string input = null;

        // Act
        string result = input.EscapeYamlString();

        // Assert
        Assert.Equal(input, result);
    }
}