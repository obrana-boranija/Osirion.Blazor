using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Osirion.Blazor.Core.Services;
using Osirion.Blazor.Core.Services.Implementations;
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
    private readonly IHtmlContentSanitizer _htmlSanitizer;

    /// <summary>
    /// Initializes a processor that sanitizes HTML with the default allow-list.
    /// </summary>
    /// <remarks>
    /// Host configuration from <c>AddOsirionHtmlSanitizer</c> does not apply to an instance created with this
    /// constructor. Resolve <see cref="IMarkdownProcessor"/> from dependency injection, or use the constructor
    /// that takes an <see cref="IHtmlContentSanitizer"/>, to honour it.
    /// </remarks>
    public MarkdownProcessor(
        IMarkdownRendererService renderer,
        IMarkdownSanitizer sanitizer,
        IHtmlToMarkdownConverter htmlToMarkdownConverter)
        : this(renderer, sanitizer, htmlToMarkdownConverter, HtmlContentSanitizer.Default)
    {
    }

    /// <summary>
    /// Initializes a processor that sanitizes HTML with the given sanitizer.
    /// </summary>
    public MarkdownProcessor(
        IMarkdownRendererService renderer,
        IMarkdownSanitizer sanitizer,
        IHtmlToMarkdownConverter htmlToMarkdownConverter,
        IHtmlContentSanitizer htmlSanitizer)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
        _htmlToMarkdownConverter = htmlToMarkdownConverter ?? throw new ArgumentNullException(nameof(htmlToMarkdownConverter));
        _htmlSanitizer = htmlSanitizer ?? throw new ArgumentNullException(nameof(htmlSanitizer));
    }

    /// <inheritdoc/>
    public string RenderToHtml(string markdown, bool sanitizeHtml = true)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var html = _renderer.RenderToHtml(markdown);

        return sanitizeHtml ? _htmlSanitizer.Sanitize(html) : html;
    }

    /// <inheritdoc/>
    public async Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = true)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var html = await _renderer.RenderToHtmlAsync(markdown).ConfigureAwait(false);

        return sanitizeHtml ? _htmlSanitizer.Sanitize(html) : html;
    }

    /// <summary>Gets or sets the SanitizeMarkdown value.</summary>
    public string SanitizeMarkdown(string markdown)
    {
        return _sanitizer.SanitizeMarkdown(markdown);
    }

    /// <inheritdoc/>
    public string SanitizeHtml(string html)
    {
        return _htmlSanitizer.Sanitize(html);
    }

    /// <summary>Gets or sets the ExtractFrontMatterAndContent value.</summary>
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

    /// <summary>Performs the ConvertHtmlToMarkdown operation asynchronously.</summary>
    public Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken = default)
    {
        return _htmlToMarkdownConverter.ConvertHtmlToMarkdownAsync(html, cancellationToken);
    }

    /// <summary>Performs the ConvertHtmlToMarkdown operation asynchronously.</summary>
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
