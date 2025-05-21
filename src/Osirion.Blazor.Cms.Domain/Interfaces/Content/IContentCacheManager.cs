using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Manages content item caching
/// </summary>
public interface IContentCacheManager
{
    /// <summary>
    /// Gets cached content items or loads them if needed
    /// </summary>
    /// <param name="loadFunc">Function to load items if cache is invalid</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="forceRefresh">Force refresh regardless of cache state</param>
    /// <returns>Dictionary of content items</returns>
    Task<Dictionary<string, ContentItem>> GetCachedItemsAsync(
        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> loadFunc,
        CancellationToken cancellationToken = default,
        bool forceRefresh = false);

    /// <summary>
    /// Invalidates the cache
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task InvalidateCacheAsync(CancellationToken cancellationToken = default);

    ///// <summary>
    ///// Checks if the current cache is still valid
    ///// </summary>
    ///// <returns>True if the cache is valid, false if it needs to be refreshed</returns>
    //Task<bool> IsCacheValidAsync();
}