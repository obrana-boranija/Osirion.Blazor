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
    /// Finds Jekyll front matter: a first line that is exactly "---" and the next line that is exactly "---".
    /// Works with LF and CRLF line endings, ignores a leading byte order mark and trailing white space on the
    /// delimiter lines, and does not mistake "---" inside a value or a later horizontal rule for the end.
    /// </summary>
    /// <returns>
    /// The start and length of the YAML between the delimiters and the index where the body starts, or
    /// <see langword="null"/> when the content has no complete front matter block.
    /// </returns>
    private static (int Start, int End, int Length)? FindFrontmatterBoundaries(string content)
    {
        var openingStart = content.Length > 0 && content[0] == '\uFEFF' ? 1 : 0;
        var openingEnd = GetLineEnd(content, openingStart, out var yamlStart);
        if (!IsDelimiterLine(content, openingStart, openingEnd))
            return null;

        var lineStart = yamlStart;
        while (lineStart < content.Length)
        {
            var lineEnd = GetLineEnd(content, lineStart, out var nextLineStart);
            if (IsDelimiterLine(content, lineStart, lineEnd))
                return (yamlStart, nextLineStart, lineStart - yamlStart);

            lineStart = nextLineStart;
        }

        return null;
    }

    /// <summary>
    /// Returns the index where the text of the line starting at <paramref name="lineStart"/> ends, without its
    /// line break, and sets <paramref name="nextLineStart"/> to the index after the line break.
    /// </summary>
    private static int GetLineEnd(string content, int lineStart, out int nextLineStart)
    {
        var newline = content.IndexOf('\n', lineStart);
        if (newline < 0)
        {
            nextLineStart = content.Length;
            return content.Length;
        }

        nextLineStart = newline + 1;
        return newline > lineStart && content[newline - 1] == '\r' ? newline - 1 : newline;
    }

    private static bool IsDelimiterLine(string content, int lineStart, int lineEnd)
    {
        return content.AsSpan(lineStart, lineEnd - lineStart).TrimEnd() is "---";
    }
}
