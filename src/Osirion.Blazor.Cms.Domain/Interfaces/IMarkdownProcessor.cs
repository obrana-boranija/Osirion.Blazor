using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>Defines the IMarkdownProcessor API contract.</summary>
public interface IMarkdownProcessor
{
    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    /// <param name="markdown">The markdown to render.</param>
    /// <param name="sanitizeHtml">
    /// When <see langword="true"/> (the default), the rendered HTML is passed through the HTML allow-list
    /// sanitizer, which removes script, event handlers, unsafe URLs and other active markup, including markup
    /// produced by markdown extensions. Pass <see langword="false"/> only for markdown the application trusts.
    /// </param>
    string RenderToHtml(string markdown, bool sanitizeHtml = true);

    /// <summary>
    /// Renders markdown to HTML asynchronously
    /// </summary>
    /// <param name="markdown">The markdown to render.</param>
    /// <param name="sanitizeHtml">
    /// When <see langword="true"/> (the default), the rendered HTML is passed through the HTML allow-list
    /// sanitizer. Pass <see langword="false"/> only for markdown the application trusts.
    /// </param>
    Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = true);

    /// <summary>
    /// Sanitizes markdown to remove potentially harmful content
    /// </summary>
    /// <remarks>
    /// This is a markdown-level clean-up, not an HTML security control. Use <see cref="SanitizeHtml"/> or the
    /// <c>sanitizeHtml</c> argument of the render methods to make output safe to render.
    /// </remarks>
    string SanitizeMarkdown(string markdown);

    /// <summary>
    /// Sanitizes an HTML fragment with the HTML allow-list sanitizer, for content that is stored as HTML
    /// rather than rendered from markdown.
    /// </summary>
    /// <param name="html">The HTML fragment to sanitize.</param>
    /// <returns>The sanitized fragment.</returns>
    /// <remarks>
    /// The default implementation HTML-encodes the whole fragment, so an implementation written before this
    /// member existed shows such content as text instead of rendering it unsanitized.
    /// </remarks>
    string SanitizeHtml(string html) => System.Net.WebUtility.HtmlEncode(html) ?? string.Empty;

    ///// <summary>
    ///// Extracts front matter from markdown content
    ///// </summary>
    //Dictionary<string, string> ExtractFrontMatter(string content);

    /// <summary>
    /// Extracts front matter from and content markdown
    /// </summary>
    public (FrontMatter? FrontMatter, string Content) ExtractFrontMatterAndContent(string markdown);

    /// <summary>
    /// Converts HTML content to markdown
    /// </summary>
    Task<string> ConvertHtmlToMarkdownAsync(string html);

    /// <summary>
    /// Converts HTML content to markdown asynchronously
    /// </summary>
    Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken);
}
