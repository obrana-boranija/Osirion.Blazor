using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;

namespace Osirion.Blazor.Cms.Infrastructure.Directory;

/// <summary>
/// Implementation of IDirectoryCacheManager for managing directory cache
/// </summary>
public class DirectoryCacheManager : IDirectoryCacheManager
{
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    private readonly ILogger<DirectoryCacheManager> _logger;

    // Initialize to empty dictionary to prevent null references
    private Dictionary<string, DirectoryItem> _directoryCache = new();
    private bool _updateInProgress = false;

    public DirectoryCacheManager(ILogger<DirectoryCacheManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, DirectoryItem>> GetCachedDirectoriesAsync(
        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loadFunc,
        CancellationToken cancellationToken = default,
        bool forceRefresh = false)
    {
        // If cache is valid and not forcing refresh, return current cache
        if (!forceRefresh && _directoryCache.Count > 0)
        {
            return _directoryCache;
        }

        // Return existing cache if update is in progress
        if (_updateInProgress)
        {
            _logger.LogDebug("Directory cache update already in progress, using existing cache with {Count} items",
                _directoryCache?.Count ?? 0);
            return _directoryCache;
        }

        bool lockTaken = false;
        try
        {
            // Try to acquire lock with a shorter timeout for webhook scenarios
            lockTaken = await _cacheLock.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
            if (!lockTaken)
            {
                _logger.LogWarning("Timeout waiting for directory cache lock - returning existing cache");
                return _directoryCache;
            }

            // Mark update as in progress
            _updateInProgress = true;

            // Double-check after acquiring lock
            if (!forceRefresh && _directoryCache.Count > 0)
            {
                return _directoryCache;
            }

            _logger.LogInformation("Loading directory cache");

            try
            {
                // Load directories using the provided function
                var directories = await loadFunc(cancellationToken);

                // Ensure we never have a null cache
                if (directories is null)
                {
                    _logger.LogWarning("Load function returned null directory cache, using empty dictionary");
                    directories = new Dictionary<string, DirectoryItem>();
                }

                // Update cache
                _directoryCache = directories;

                _logger.LogInformation("Directory cache refreshed, {Count} directories loaded", directories.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading directory cache");

                // If we have an existing cache, keep it
                if (_directoryCache is null)
                {
                    _directoryCache = new Dictionary<string, DirectoryItem>();
                }

                // Rethrow to let caller know about the error
                throw;
            }

            return _directoryCache;
        }
        finally
        {
            _updateInProgress = false;

            // Only release if we acquired the lock
            if (lockTaken)
            {
                _cacheLock.Release();
            }
        }
    }

    /// <inheritdoc/>
    public async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
    {
        // Skip if update is already in progress
        if (_updateInProgress)
        {
            _logger.LogWarning("Cache invalidation skipped - update already in progress");
            return;
        }

        bool lockTaken = false;
        try
        {
            // Try to acquire the lock with a short timeout
            lockTaken = await _cacheLock.WaitAsync(TimeSpan.FromSeconds(2), cancellationToken);
            if (!lockTaken)
            {
                _logger.LogWarning("Timeout waiting for directory cache lock during invalidation");
                return;
            }

            // Just invalidate expiration but keep the cache (don't set to null)
            _logger.LogInformation("Directory cache invalidated");
        }
        finally
        {
            if (lockTaken)
            {
                _cacheLock.Release();
            }
        }
    }
}