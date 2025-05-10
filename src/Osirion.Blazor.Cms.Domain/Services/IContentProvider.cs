using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Services;

/// <summary>
/// Core interface for content providers with read capabilities
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
    /// Gets whether the provider is read-only or supports write operations
    /// </summary>
    bool IsReadOnly { get; }

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

    /// <summary>
    /// Gets all available translations for content with the specified localization ID
    /// </summary>
    /// <param name="localizationId">The unique identifier connecting translations of the same content</param>
    /// <returns>A dictionary of locale codes mapped to their respective content items</returns>
    Task<Dictionary<string, ContentItem>> GetContentTranslationsAsync(string localizationId);

    /// <summary>
    /// Gets content items based on a query
    /// </summary>
    Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories from the provider
    /// </summary>
    Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tags from the provider
    /// </summary>
    Task<IReadOnlyList<Repositories.ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available directories from the provider, optionally filtered by locale
    /// </summary>
    Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific directory by its path
    /// </summary>
    Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific directory by its ID
    /// </summary>
    Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific directory by its URL
    /// </summary>
    Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default);
}