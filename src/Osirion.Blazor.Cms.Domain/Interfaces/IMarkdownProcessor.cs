namespace Osirion.Blazor.Cms.Domain.Interfaces;

public interface IMarkdownProcessor
{
    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    string RenderToHtml(string markdown, bool sanitizeHtml = true);

    /// <summary>
    /// Renders markdown to HTML asynchronously
    /// </summary>
    Task<string> RenderToHtmlAsync(string markdown, bool sanitizeHtml = true);

    /// <summary>
    /// Sanitizes markdown to remove potentially harmful content
    /// </summary>
    string SanitizeMarkdown(string markdown);

    /// <summary>
    /// Extracts front matter from markdown content
    /// </summary>
    Dictionary<string, string> ExtractFrontMatter(string content);

    /// <summary>
    /// Extracts front matter from markdown content asynchronously
    /// </summary>
    Task<(Dictionary<string, string> FrontMatter, string Content)> ExtractFrontMatterAsync(
        string markdown, CancellationToken cancellationToken = default);

    /// <summary>
    /// Converts HTML content to markdown
    /// </summary>
    Task<string> ConvertHtmlToMarkdownAsync(string html);

    /// <summary>
    /// Converts HTML content to markdown asynchronously
    /// </summary>
    Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken);
}