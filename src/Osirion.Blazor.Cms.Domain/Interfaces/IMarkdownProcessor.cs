using Osirion.Blazor.Cms.Domain.ValueObjects;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

public interface IMarkdownProcessor
{
    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    string RenderToHtml(string markdown, bool sanitizeHtml = false);

    /// <summary>
    /// Renders markdown to HTML asynchronously
    /// </summary>
    Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = false);

    /// <summary>
    /// Sanitizes markdown to remove potentially harmful content
    /// </summary>
    string SanitizeMarkdown(string markdown);

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