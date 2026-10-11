using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown;

public class FrontMatterExtractorAsyncTests
{
    private readonly FrontMatterExtractor _extractor = new();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExtractFrontMatterAsync_WithBlankMarkdown_ReturnsNothing(string markdown)
    {
        var (frontMatter, content) = await _extractor.ExtractFrontMatterAsync(markdown);

        frontMatter.ShouldBeEmpty();
        content.ShouldBeEmpty();
    }

    [Fact]
    public async Task ExtractFrontMatterAsync_WithoutFrontMatter_ReturnsTheMarkdownUnchanged()
    {
        const string markdown = "# Title\n\nBody";

        var (frontMatter, content) = await _extractor.ExtractFrontMatterAsync(markdown);

        frontMatter.ShouldBeEmpty();
        content.ShouldBe(markdown);
    }

    [Fact]
    public async Task ExtractFrontMatterAsync_WithFrontMatter_SplitsValuesAndBody()
    {
        var (frontMatter, content) = await _extractor.ExtractFrontMatterAsync("---\ntitle: 'Quoted'\nurl: a:b\n---\nBody");

        frontMatter["TITLE"].ShouldBe("Quoted");
        frontMatter["url"].ShouldBe("a:b");
        content.ShouldBe("Body");
    }
}
