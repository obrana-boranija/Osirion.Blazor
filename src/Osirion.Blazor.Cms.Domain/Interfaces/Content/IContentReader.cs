using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Base interface for reading content items
/// </summary>
public interface IContentReader
{
    /// <summary>
    /// Gets all content items from the provider
    /// </summary>
    Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific content item by its ID
    /// </summary>
    Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific content item by its path
    /// </summary>
    Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific content item by its URL
    /// </summary>
    Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default);
}