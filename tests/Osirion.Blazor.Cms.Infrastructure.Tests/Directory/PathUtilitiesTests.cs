using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Directory;

public class PathUtilitiesTests
{
    private readonly IOptions<FileSystemOptions> _options;
    private readonly PathUtilities _pathUtils;

    public PathUtilitiesTests()
    {
        var options = new FileSystemOptions
        {
            ContentRoot = "content",
            EnableLocalization = true,
            DefaultLocale = "en",
            SupportedLocales = new List<string> { "en", "fr", "de" }
        };

        _options = Options.Create(options);
        _pathUtils = new PathUtilities(_options);
    }

    [Fact]
    public void NormalizePath_ConvertsBackslashesToForwardSlashes()
    {
        // Act
        var result = _pathUtils.NormalizePath("path\\to\\file");

        // Assert
        result.ShouldBe("path/to/file");
    }

    [Fact]
    public void NormalizePath_TrimsTrailingSlashes()
    {
        // Act
        var result = _pathUtils.NormalizePath("path/to/file/");

        // Assert
        result.ShouldBe("path/to/file");
    }

    [Fact]
    public void ExtractLocaleFromPath_WithValidLocale_ReturnsLocale()
    {
        // Act
        var result = _pathUtils.ExtractLocaleFromPath("content/fr/blog/post.md");

        // Assert
        result.ShouldBe("fr");
    }

    [Fact]
    public void ExtractLocaleFromPath_WithInvalidLocale_ReturnsDefaultLocale()
    {
        // Act
        var result = _pathUtils.ExtractLocaleFromPath("content/xyz/blog/post.md");

        // Assert
        result.ShouldBe("en"); // Default locale
    }

    [Fact]
    public void ExtractLocaleFromPath_WithDisabledLocalization_ReturnsDefaultLocale()
    {
        // Arrange
        var disabledOptions = new FileSystemOptions
        {
            ContentRoot = "content",
            EnableLocalization = false,
            DefaultLocale = "en"
        };

        var pathUtils = new PathUtilities(Options.Create(disabledOptions));

        // Act
        var result = pathUtils.ExtractLocaleFromPath("content/fr/blog/post.md");

        // Assert
        result.ShouldBe("en"); // Default locale
    }

    [Fact]
    public void MatchesGlobPattern_WithMatchingPattern_ReturnsTrue()
    {
        // Act
        var result = _pathUtils.MatchesGlobPattern(
            "content/blog/post.md",
            "content/blog/*.md");

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void MatchesGlobPattern_WithNonMatchingPattern_ReturnsFalse()
    {
        // Act
        var result = _pathUtils.MatchesGlobPattern(
            "content/blog/post.md",
            "content/pages/*.md");

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void RemoveLocaleFromPath_WithLocale_RemovesLocaleSegment()
    {
        // Act
        var result = _pathUtils.RemoveLocaleFromPath("content/fr/blog/post.md");

        // Assert
        result.ShouldBe("blog/post.md");
    }

    [Fact]
    public void GenerateDirectoryUrl_WithValidPath_ReturnsUrl()
    {
        // Act
        var result = _pathUtils.GenerateDirectoryUrl("content/fr/blog");

        // Assert
        result.ShouldBe("blog");
    }

    [Fact]
    public void GenerateContentUrl_WithValidPathAndSlug_ReturnsUrl()
    {
        // Act
        var result = _pathUtils.GenerateContentUrl("content/fr/blog/post.md", "my-post");

        // Assert
        result.ShouldBe("blog/my-post");
    }
}