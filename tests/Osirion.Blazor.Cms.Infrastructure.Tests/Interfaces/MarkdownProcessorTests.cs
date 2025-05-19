using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Interfaces
{
    public class MarkdownProcessorTests
    {
        private readonly IMarkdownProcessor _processor;

        public MarkdownProcessorTests()
        {
            _processor = Substitute.For<IMarkdownProcessor>();

            _processor.RenderToHtml(Arg.Any<string>(), Arg.Any<bool>())
                .Returns(callInfo =>
                {
                    var markdown = callInfo.Arg<string>();
                    if (string.IsNullOrEmpty(markdown)) return string.Empty;
                    return $"<p>{markdown}</p>";
                });

            _processor.ExtractFrontMatter(Arg.Any<string>())
                .Returns(callInfo =>
                {
                    var content = callInfo.Arg<string>();
                    if (!content.Contains("---")) return new Dictionary<string, string>();

                    var dict = new Dictionary<string, string>();
                    if (content.Contains("title:")) dict["title"] = "Test Title";
                    return dict;
                });

            _processor.ExtractFrontMatterAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(callInfo =>
                {
                    var content = callInfo.Arg<string>();
                    if (!content.Contains("---"))
                        return Task.FromResult((new Dictionary<string, string>(), content));

                    var dict = new Dictionary<string, string>();
                    if (content.Contains("title:")) dict["title"] = "Test Title";

                    var contentPart = content.Substring(content.LastIndexOf("---") + 3).Trim();
                    return Task.FromResult((dict, contentPart));
                });
        }

        [Fact]
        public void RenderToHtml_WithValidMarkdown_ProducesExpectedOutput()
        {
            // Act
            var result = _processor.RenderToHtml("Test content", true);

            // Assert
            result.ShouldBe("<p>Test content</p>");
        }

        [Fact]
        public void RenderToHtml_WithEmptyInput_ReturnsEmptyString()
        {
            // Act
            var result = _processor.RenderToHtml(string.Empty);

            // Assert
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void ExtractFrontMatter_WithValidInput_ExtractsDictionary()
        {
            // Arrange
            var content = "---\ntitle: Test Title\n---\nThis is content";

            // Act
            var result = _processor.ExtractFrontMatter(content);

            // Assert
            result.ShouldContainKey("title");
            result["title"].ShouldBe("Test Title");
        }

        [Fact]
        public async Task ExtractFrontMatterAsync_WithValidInput_ExtractsContentAndFrontMatter()
        {
            // Arrange
            var content = "---\ntitle: Test Title\n---\nThis is content";

            // Act
            var result = await _processor.ExtractFrontMatterAsync(content);

            // Assert
            result.FrontMatter.ShouldContainKey("title");
            result.FrontMatter["title"].ShouldBe("Test Title");
            result.Content.ShouldBe("This is content");
        }
    }
}