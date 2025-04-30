using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown
{
    /// <summary>
    /// Composition-based implementation of IMarkdownProcessor
    /// </summary>
    public class MarkdownProcessor : IMarkdownProcessor
    {
        private readonly IMarkdownRendererService _renderer;
        private readonly IFrontMatterExtractor _frontMatterExtractor;
        private readonly IMarkdownSanitizer _sanitizer;
        private readonly IHtmlToMarkdownConverter _htmlToMarkdownConverter;

        public MarkdownProcessor(
            IMarkdownRendererService renderer,
            IFrontMatterExtractor frontMatterExtractor,
            IMarkdownSanitizer sanitizer,
            IHtmlToMarkdownConverter htmlToMarkdownConverter)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _frontMatterExtractor = frontMatterExtractor ?? throw new ArgumentNullException(nameof(frontMatterExtractor));
            _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
            _htmlToMarkdownConverter = htmlToMarkdownConverter ?? throw new ArgumentNullException(nameof(htmlToMarkdownConverter));
        }

        public string RenderToHtml(string markdown, bool sanitizeHtml = true)
        {
            if (string.IsNullOrEmpty(markdown))
                return string.Empty;

            // Apply sanitization if requested
            if (sanitizeHtml)
                markdown = _sanitizer.SanitizeMarkdown(markdown);

            // Render to HTML
            return _renderer.RenderToHtml(markdown);
        }

        public Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = true)
        {
            if (string.IsNullOrEmpty(markdown))
                return Task.FromResult(string.Empty);

            // Apply sanitization if requested
            if (sanitizeHtml)
                markdown = _sanitizer.SanitizeMarkdown(markdown);

            // Render to HTML asynchronously
            return _renderer.RenderToHtmlAsync(markdown);
        }

        public string SanitizeMarkdown(string markdown)
        {
            return _sanitizer.SanitizeMarkdown(markdown);
        }

        public Dictionary<string, string> ExtractFrontMatter(string content)
        {
            return _frontMatterExtractor.ExtractFrontMatter(content);
        }

        public Task<(Dictionary<string, string> FrontMatter, string Content)> ExtractFrontMatterAsync(
            string markdown, CancellationToken cancellationToken = default)
        {
            return _frontMatterExtractor.ExtractFrontMatterAsync(markdown, cancellationToken);
        }

        public Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken = default)
        {
            return _htmlToMarkdownConverter.ConvertHtmlToMarkdownAsync(html, cancellationToken);
        }

        public async Task<string> ConvertHtmlToMarkdownAsync(string html)
        {
            return await _htmlToMarkdownConverter.ConvertHtmlToMarkdownAsync(html);
        }
    }
}