using Markdig.Renderers;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Composition-based implementation of IMarkdownProcessor that delegates to specialized services
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public string SanitizeMarkdown(string markdown)
    {
        return _sanitizer.SanitizeMarkdown(markdown);
    }

    /// <inheritdoc/>
    public Dictionary<string, string> ExtractFrontMatter(string content)
    {
        return _frontMatterExtractor.ExtractFrontMatter(content);
    }

    /// <inheritdoc/>
    public Task<(Dictionary<string, string> FrontMatter, string Content)> ExtractFrontMatterAsync(
        string markdown, CancellationToken cancellationToken = default)
    {
        return _frontMatterExtractor.ExtractFrontMatterAsync(markdown, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken = default)
    {
        return _htmlToMarkdownConverter.ConvertHtmlToMarkdownAsync(html, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task ProcessMarkdownContentAsync(string markdownContent, ContentItem contentItem)
    {
        if (string.IsNullOrWhiteSpace(markdownContent))
        {
            contentItem.SetContent(string.Empty);
            contentItem.SetOriginalMarkdown(string.Empty);
            return;
        }

        // Extract front matter and content
        var (frontMatter, content) = await _frontMatterExtractor.ExtractFrontMatterAsync(markdownContent);

        // Set markdown content
        contentItem.SetOriginalMarkdown(content);

        // Render HTML
        var html = await _renderer.RenderToHtmlAsync(content);
        contentItem.SetContent(html);

        // Process front matter
        foreach (var kvp in frontMatter)
        {
            string key = kvp.Key.ToLowerInvariant();
            string value = kvp.Value;

            switch (key)
            {
                case "title":
                    contentItem.SetTitle(value);
                    break;
                case "description":
                    contentItem.SetDescription(value);
                    break;
                case "author":
                    contentItem.SetAuthor(value);
                    break;
                case "date":
                    if (DateTime.TryParse(value, out var date))
                        contentItem.SetCreatedDate(date);
                    break;
                case "tags":
                case "tag":
                    foreach (var tag in ParseListValue(value))
                    {
                        contentItem.AddTag(tag);
                    }
                    break;
                case "categories":
                case "category":
                    foreach (var category in ParseListValue(value))
                    {
                        contentItem.AddCategory(category);
                    }
                    break;
                    // Other front matter fields can be processed here
            }
        }
    }

    /// <summary>
    /// Helper method to parse comma-separated front matter values
    /// </summary>
    private IEnumerable<string> ParseListValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        // Handle YAML array format [item1, item2]
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            value = value.Substring(1, value.Length - 2);
        }

        foreach (var item in value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmedItem = item.Trim();

            // Remove quotes if present
            if ((trimmedItem.StartsWith("\"") && trimmedItem.EndsWith("\"")) ||
                (trimmedItem.StartsWith("'") && trimmedItem.EndsWith("'")))
            {
                trimmedItem = trimmedItem.Substring(1, trimmedItem.Length - 2);
            }

            if (!string.IsNullOrEmpty(trimmedItem))
            {
                yield return trimmedItem;
            }
        }
    }

    public Task<string> ConvertHtmlToMarkdownAsync(string html)
    {
        throw new NotImplementedException();
    }
}