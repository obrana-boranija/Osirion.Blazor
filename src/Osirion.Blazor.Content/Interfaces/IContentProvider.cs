using Osirion.Blazor.Content.Models;

namespace Osirion.Blazor.Content;

/// <summary>
/// Interface for content providers
/// </summary>
public interface IContentProvider
{
    /// <summary>
    /// Gets the unique identifier for the provider
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Gets the display name for the provider
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets whether the provider supports write operations
    /// </summary>
    bool IsReadOnly { get; }

    /// <summary>
    /// Gets all content items from the provider
    /// </summary>
    Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific content item by its path
    /// </summary>
    Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets content items based on a query
    /// </summary>
    Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories from the provider
    /// </summary>
    Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tags from the provider
    /// </summary>
    Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the provider cache
    /// </summary>
    Task RefreshCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes the provider
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}