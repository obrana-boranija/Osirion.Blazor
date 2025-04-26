// Refactored IContentProvider interface with clearer separation of concerns
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Interfaces;

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
    /// Gets whether the provider supports write operations
    /// </summary>
    bool SupportsWriting { get; }

    /// <summary>
    /// Gets a content writer if supported by this provider
    /// </summary>
    IContentWriter? GetContentWriter();

    /// <summary>
    /// Gets a specific content item by its ID
    /// </summary>
    Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default);

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
    /// Gets available localizations for the provider
    /// </summary>
    Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for content providers that support write operations
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

    /// <summary>
    /// Creates a new directory
    /// </summary>
    Task<DirectoryItem> CreateDirectoryAsync(DirectoryItem directory, CancellationToken cancellationToken = default);
}

/// <summary>
/// Factory interface for creating content providers
/// </summary>
public interface IContentProviderFactory
{
    /// <summary>
    /// Creates a provider instance by ID
    /// </summary>
    IContentProvider CreateProvider(string providerId);

    /// <summary>
    /// Gets all available provider types
    /// </summary>
    IEnumerable<string> GetAvailableProviderTypes();

    /// <summary>
    /// Registers a provider factory function
    /// </summary>
    void RegisterProvider<T>(Func<IServiceProvider, T> factory) where T : class, IContentProvider;
}