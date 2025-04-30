namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Responsible for sanitizing Markdown content to prevent security issues
/// </summary>
public interface IMarkdownSanitizer
{
    /// <summary>
    /// Sanitizes markdown content to remove potentially harmful elements
    /// </summary>
    /// <param name="markdown">The markdown content to sanitize</param>
    /// <returns>Sanitized markdown</returns>
    string SanitizeMarkdown(string markdown);
}