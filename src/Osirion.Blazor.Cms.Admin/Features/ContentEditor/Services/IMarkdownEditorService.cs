namespace Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;

/// <summary>
/// Interface for rendering markdown content
/// </summary>
public interface IMarkdownEditorService
{
    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    /// <param name="markdown">The markdown content</param>
    /// <returns>The HTML content</returns>
    Task<string> RenderToHtmlAsync(string markdown);

    /// <summary>
    /// Parses front matter from markdown
    /// </summary>
    /// <param name="markdown">The markdown content with front matter</param>
    /// <returns>The front matter as a dictionary</returns>
    Task<Dictionary<string, object>> ParseFrontMatterAsync(string markdown);

    /// <summary>
    /// Extracts the content without front matter
    /// </summary>
    /// <param name="markdown">The markdown content with front matter</param>
    /// <returns>The content without front matter</returns>
    Task<string> ExtractContentAsync(string markdown);
}