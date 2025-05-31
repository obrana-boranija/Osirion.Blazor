using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Osirion.Blazor.Cms.Infrastructure.Markdown;

/// <summary>
/// Composition-based implementation of IMarkdownProcessor
/// </summary>
public class MarkdownProcessor : IMarkdownProcessor
{
    private readonly IMarkdownRendererService _renderer;
    private readonly IMarkdownSanitizer _sanitizer;
    private readonly IHtmlToMarkdownConverter _htmlToMarkdownConverter;

    public MarkdownProcessor(
        IMarkdownRendererService renderer,
        IMarkdownSanitizer sanitizer,
        IHtmlToMarkdownConverter htmlToMarkdownConverter)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
        _htmlToMarkdownConverter = htmlToMarkdownConverter ?? throw new ArgumentNullException(nameof(htmlToMarkdownConverter));
    }

    public string RenderToHtml(string markdown, bool sanitizeHtml = false)
    {
        if (string.IsNullOrEmpty(markdown))
            return string.Empty;

        // Apply sanitization if requested
        if (sanitizeHtml)
            markdown = _sanitizer.SanitizeMarkdown(markdown);

        // Render to HTML
        return _renderer.RenderToHtml(markdown);
    }

    public Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = false)
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

    public (FrontMatter? FrontMatter, string Content) ExtractFrontMatterAndContent(string markdown)
    {
        var frontmatterBoundaries = FindFrontmatterBoundaries(markdown);
        if (frontmatterBoundaries is null)
        {
            return (null, markdown);
        }

        // Extract the YAML frontmatter
        string yamlContent = markdown.Substring(
            frontmatterBoundaries.Value.Start,
            frontmatterBoundaries.Value.Length
        );

        // Configure deserializer with custom settings for flexibility
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        // Deserialize frontmatter to our comprehensive class
        var frontmatter = deserializer.Deserialize<FrontMatter>(yamlContent);

        string markdownContent = markdown.Substring(frontmatterBoundaries.Value.End);

        return (frontmatter, markdownContent);
    }

    public Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken = default)
    {
        return _htmlToMarkdownConverter.ConvertHtmlToMarkdownAsync(html, cancellationToken);
    }

    public async Task<string> ConvertHtmlToMarkdownAsync(string html)
    {
        return await _htmlToMarkdownConverter.ConvertHtmlToMarkdownAsync(html);
    }

    /// <summary>
    /// Helper method to find Jekyll frontmatter boundaries.
    /// Frontmatter starts and ends with "---" on its own line.
    /// </summary>
    private static (int Start, int End, int Length)? FindFrontmatterBoundaries(string content)
    {
        const string delimiter = "---";

        // Must start with delimiter
        if (!content.StartsWith(delimiter))
            return null;

        // Find the ending delimiter (after the opening one)
        int firstDelimiterEnd = content.IndexOf('\n') + 1;
        int secondDelimiterPos = content.IndexOf($"{delimiter}", firstDelimiterEnd);

        if (secondDelimiterPos == -1)
            return null;

        int yamlStart = firstDelimiterEnd;
        int yamlEnd = secondDelimiterPos;
        int fullFrontmatterEnd = secondDelimiterPos + delimiter.Length + 2; // Include delimiter and newline

        return (yamlStart, fullFrontmatterEnd, yamlEnd - yamlStart);
    }
}