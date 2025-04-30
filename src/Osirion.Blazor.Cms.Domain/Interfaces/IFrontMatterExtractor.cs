namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Responsible for extracting front matter from Markdown content
/// </summary>
public interface IFrontMatterExtractor
{
    /// <summary>
    /// Extracts front matter metadata from markdown content
    /// </summary>
    /// <param name="content">The full content including front matter</param>
    /// <returns>Dictionary of key-value pairs from front matter</returns>
    Dictionary<string, string> ExtractFrontMatter(string content);

    /// <summary>
    /// Extracts front matter and content asynchronously
    /// </summary>
    /// <param name="markdown">The full markdown content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple with front matter dictionary and content</returns>
    Task<(Dictionary<string, string> FrontMatter, string Content)> ExtractFrontMatterAsync(
        string markdown, CancellationToken cancellationToken = default);
}