using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Interface for writing/modifying content items
/// </summary>
public interface IContentWriter
{
    /// <summary>
    /// Creates a new content item
    /// </summary>
    Task<ContentItem> CreateContentAsync(ContentItem item, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing content item
    /// </summary>
    Task<ContentItem> UpdateContentAsync(ContentItem item, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a content item
    /// </summary>
    Task DeleteContentAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default);
}