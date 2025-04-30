namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Responsible for converting HTML content to Markdown
/// </summary>
public interface IHtmlToMarkdownConverter
{
    /// <summary>
    /// Converts HTML content to Markdown
    /// </summary>
    /// <param name="html">The HTML content to convert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Markdown content</returns>
    Task<string> ConvertHtmlToMarkdownAsync(string html, CancellationToken cancellationToken = default);
}