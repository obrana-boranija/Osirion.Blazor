using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Interfaces;

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
    /// Gets a specific content item by its url
    /// </summary>
    Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Gets the directory structure from the provider
    /// </summary>
    Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific directory by its path
    /// </summary>
    Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific directory by its url
    /// </summary>
    Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific directory by its ID
    /// </summary>
    Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available localizations for the provider
    /// </summary>
    Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all translations of a specific content item
    /// </summary>
    Task<IReadOnlyList<ContentItem>> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default);
}