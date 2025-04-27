namespace Osirion.Blazor.Cms.Domain.Interfaces;

/// <summary>
/// Service for caching content items and related data
/// </summary>
public interface IContentCacheService
{
    /// <summary>
    /// Gets a cached item, or creates it using the provided factory if not found
    /// </summary>
    /// <typeparam name="T">The type of the cached item</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="factory">Factory function to create the item if not found</param>
    /// <param name="expiration">Optional expiration timespan</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cached or newly created item</returns>
    Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a cached item
    /// </summary>
    /// <typeparam name="T">The type of the cached item</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The cached item, or default if not found</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets an item in the cache
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expiration">Optional expiration timespan</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from the cache
    /// </summary>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all items with the specified prefix from the cache
    /// </summary>
    /// <param name="prefix">The key prefix</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all items from the cache
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ClearAsync(CancellationToken cancellationToken = default);
}