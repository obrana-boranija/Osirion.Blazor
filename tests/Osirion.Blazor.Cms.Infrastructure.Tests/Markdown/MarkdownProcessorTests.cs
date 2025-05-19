using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Markdown;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Markdown
{
    public class MarkdownProcessorTests
    {
        private readonly IMarkdownRendererService _renderer;
        private readonly IFrontMatterExtractor _frontMatterExtractor;
        private readonly IMarkdownSanitizer _sanitizer;
        private readonly IHtmlToMarkdownConverter _htmlToMarkdownConverter;
        private readonly MarkdownProcessor _processor;

        public MarkdownProcessorTests()
        {
            _renderer = Substitute.For<IMarkdownRendererService>();
            _frontMatterExtractor = Substitute.For<IFrontMatterExtractor>();
            _sanitizer = Substitute.For<IMarkdownSanitizer>();
            _htmlToMarkdownConverter = Substitute.For<IHtmlToMarkdownConverter>();

            _processor = new MarkdownProcessor(
                _renderer,
                _frontMatterExtractor,
                _sanitizer,
                _htmlToMarkdownConverter);
        }

        [Fact]
        public void RenderToHtml_WithValidMarkdown_CallsRenderer()
        {
            // Arrange
            var markdown = "#Test\nThis is a test";
            var expected = "<h1>Test</h1><p>This is a test</p>";
            _renderer.RenderToHtml(markdown).Returns(expected);

            // Act
            var result = _processor.RenderToHtml(markdown, false);

            // Assert
            result.ShouldBe(expected);
            //_sanitizer.Received(1).SanitizeMarkdown(markdown);
            _renderer.Received(1).RenderToHtml(markdown);
        }

        [Fact]
        public async Task RenderToHtmlAsync_WithValidMarkdown_CallsRenderer()
        {
            // Arrange
            var markdown = "#Test\nThis is a test";
            var expected = "<h1>Test</h1><p>This is a test</p>";
            _renderer.RenderToHtmlAsync(markdown).Returns(expected);

            // Act
            var result = await _processor.RenderToHtmlAsync(markdown, false);

            // Assert
            result.ShouldBe(expected);
            //_sanitizer.Received(1).SanitizeMarkdown(markdown);
            await _renderer.Received(1).RenderToHtmlAsync(markdown);
        }

        [Fact]
        public void SanitizeMarkdown_CallsSanitizer()
        {
            // Arrange
            var markdown = "Test with <script>alert('xss')</script>";
            var sanitized = "Test with ";
            _sanitizer.SanitizeMarkdown(markdown).Returns(sanitized);

            // Act
            var result = _processor.SanitizeMarkdown(markdown);

            // Assert
            result.ShouldBe(sanitized);
            _sanitizer.Received(1).SanitizeMarkdown(markdown);
        }

        [Fact]
        public void ExtractFrontMatter_CallsExtractor()
        {
            // Arrange
            var content = "---\ntitle: Test\n---\nContent";
            var expected = new Dictionary<string, string> { { "title", "Test" } };
            _frontMatterExtractor.ExtractFrontMatter(content).Returns(expected);

            // Act
            var result = _processor.ExtractFrontMatter(content);

            // Assert
            result.ShouldBeSameAs(expected);
            _frontMatterExtractor.Received(1).ExtractFrontMatter(content);
        }

        [Fact]
        public async Task ExtractFrontMatterAsync_CallsExtractor()
        {
            // Arrange
            var content = "---\ntitle: Test\n---\nContent";
            var frontMatter = new Dictionary<string, string> { { "title", "Test" } };
            var expected = (frontMatter, "Content");
            _frontMatterExtractor.ExtractFrontMatterAsync(content, Arg.Any<CancellationToken>()).Returns(expected);

            // Act
            var result = await _processor.ExtractFrontMatterAsync(content);

            // Assert
            result.ShouldBe(expected);
            await _frontMatterExtractor.Received(1).ExtractFrontMatterAsync(content, Arg.Any<CancellationToken>());
        }
    }
}