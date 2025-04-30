using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Directory;

/// <summary>
/// Manages caching for directory repositories
/// </summary>
public interface IDirectoryCacheManager
{
    /// <summary>
    /// Gets a cached dictionary of directories or refreshes if needed
    /// </summary>
    /// <param name="loadFunc">Function to load directories if cache is invalid</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="forceRefresh">Force a refresh regardless of cache state</param>
    /// <returns>Dictionary of directories</returns>
    Task<Dictionary<string, DirectoryItem>> GetCachedDirectoriesAsync(
        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loadFunc,
        CancellationToken cancellationToken = default,
        bool forceRefresh = false);

    /// <summary>
    /// Invalidates the cache
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task InvalidateCacheAsync(CancellationToken cancellationToken = default);
}