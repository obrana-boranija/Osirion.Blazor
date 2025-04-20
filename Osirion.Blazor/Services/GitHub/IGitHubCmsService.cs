using Osirion.Blazor.Models.Cms;

namespace Osirion.Blazor.Services.GitHub;

/// <summary>
/// Interface for GitHub-based CMS service
/// </summary>
public interface IGitHubCmsService
{
    /// <summary>
    /// Gets all content items from the repository
    /// </summary>
    Task<List<ContentItem>> GetAllContentItemsAsync();

    /// <summary>
    /// Gets a specific content item by its path
    /// </summary>
    Task<ContentItem?> GetContentItemByPathAsync(string path);

    /// <summary>
    /// Gets content items by directory
    /// </summary>
    Task<List<ContentItem>> GetContentItemsByDirectoryAsync(string directory);

    /// <summary>
    /// Gets all categories from the repository
    /// </summary>
    Task<List<ContentCategory>> GetCategoriesAsync();

    /// <summary>
    /// Gets content items by category
    /// </summary>
    Task<List<ContentItem>> GetContentItemsByCategoryAsync(string category);

    /// <summary>
    /// Gets all tags from the repository
    /// </summary>
    Task<List<ContentTag>> GetTagsAsync();

    /// <summary>
    /// Gets content items by tag
    /// </summary>
    Task<List<ContentItem>> GetContentItemsByTagAsync(string tag);

    /// <summary>
    /// Gets featured content items
    /// </summary>
    Task<List<ContentItem>> GetFeaturedContentItemsAsync(int count = 3);

    /// <summary>
    /// Searches content items by query
    /// </summary>
    Task<List<ContentItem>> SearchContentItemsAsync(string query);

    /// <summary>
    /// Refreshes the cache
    /// </summary>
    Task RefreshCacheAsync();
}