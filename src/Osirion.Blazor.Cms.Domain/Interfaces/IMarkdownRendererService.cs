using Markdig;

namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Markdown renderer service for Blazor applications
/// </summary>
/// <remarks>
/// This service provides methods to render markdown content to HTML with enhanced security and sanitization.
/// It uses the Markdig library for markdown processing and includes methods for both synchronous and asynchronous rendering.
/// </remarks>
public interface IMarkdownRendererService
{
    /// <summary>
    /// Renders markdown to HTML
    /// </summary>
    /// <param name="markdown">Markdown content to render</param>
    /// <param name="configureMarkdig">Optional action to configure the Markdig pipeline</param>
    /// <returns>Rendered HTML</returns>
    string RenderToHtml(string markdown, Action<MarkdownPipelineBuilder>? configureMarkdig = null);

    /// <summary>
    /// Renders markdown to HTML asynchronously
    /// </summary>
    /// <param name="markdown">Markdown content to render</param>
    /// <param name="configureMarkdig">Optional action to configure the Markdig pipeline</param>
    /// <returns>Rendered HTML</returns>
    Task<string> RenderToHtmlAsync(string markdown, Action<MarkdownPipelineBuilder>? configureOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sanitizes markdown content to remove potentially harmful content
    /// </summary>
    /// <param name="markdown">Markdown content to sanitize</param>
    /// <returns>Sanitized markdown</returns>
    string SanitizeMarkdown(string markdown);
}