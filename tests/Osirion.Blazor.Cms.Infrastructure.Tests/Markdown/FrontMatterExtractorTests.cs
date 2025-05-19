using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown
{
    public class FrontMatterExtractorTests
    {
        private readonly FrontMatterExtractor _extractor;

        public FrontMatterExtractorTests()
        {
            _extractor = new FrontMatterExtractor();
        }

        [Fact]
        public void ExtractFrontMatter_WithValidFrontMatter_ReturnsKeyValuePairs()
        {
            // Arrange
            var content = "---\ntitle: Test Post\nauthor: John Doe\n---\nThis is the content.";

            // Act
            var result = _extractor.ExtractFrontMatter(content);

            // Assert
            result.Count.ShouldBe(2);
            result["title"].ShouldBe("Test Post");
            result["author"].ShouldBe("John Doe");
        }

        [Fact]
        public void ExtractFrontMatter_WithEmptyContent_ReturnsEmptyDictionary()
        {
            // Arrange
            var content = string.Empty;

            // Act
            var result = _extractor.ExtractFrontMatter(content);

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void ExtractFrontMatter_WithNoFrontMatter_ReturnsEmptyDictionary()
        {
            // Arrange
            var content = "This is just content with no front matter.";

            // Act
            var result = _extractor.ExtractFrontMatter(content);

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ExtractFrontMatterAsync_WithValidFrontMatter_ReturnsParsedFrontMatterAndContent()
        {
            // Arrange
            var content = "---\ntitle: Test Post\nauthor: John Doe\n---\nThis is the content.";

            // Act
            var result = await _extractor.ExtractFrontMatterAsync(content);

            // Assert
            result.FrontMatter.Count.ShouldBe(2);
            result.FrontMatter["title"].ShouldBe("Test Post");
            result.FrontMatter["author"].ShouldBe("John Doe");
            result.Content.ShouldBe("This is the content.");
        }

        [Fact]
        public async Task ExtractFrontMatterAsync_WithQuotedValues_HandlesQuotesCorrectly()
        {
            // Arrange
            var content = "---\ntitle: \"Quoted Title\"\nauthor: 'Single Quoted'\n---\nContent";

            // Act
            var result = await _extractor.ExtractFrontMatterAsync(content);

            // Assert
            result.FrontMatter["title"].ShouldBe("Quoted Title");
            result.FrontMatter["author"].ShouldBe("Single Quoted");
        }
    }
}