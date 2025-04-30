using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Processes content metadata
/// </summary>
public interface IContentMetadataProcessor
{
    /// <summary>
    /// Applies front matter metadata to a content item
    /// </summary>
    /// <param name="frontMatter">Extracted front matter</param>
    /// <param name="contentItem">Content item to update</param>
    /// <returns>Updated content item</returns>
    ContentItem ApplyMetadataToContentItem(Dictionary<string, string> frontMatter, ContentItem contentItem);

    /// <summary>
    /// Generates front matter from a content item
    /// </summary>
    /// <param name="entity">Content item with metadata</param>
    /// <returns>Front matter string</returns>
    string GenerateFrontMatter(ContentItem entity);

    /// <summary>
    /// Parses a comma-separated list value
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <returns>List of items</returns>
    IEnumerable<string> ParseListValue(string value);
}