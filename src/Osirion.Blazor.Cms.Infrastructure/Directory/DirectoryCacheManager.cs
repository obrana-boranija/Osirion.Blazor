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
    private readonly TimeSpan _cacheDuration;

    private Dictionary<string, DirectoryItem>? _directoryCache;
    private DateTime _cacheExpiration = DateTime.MinValue;

    public DirectoryCacheManager(
        ILogger<DirectoryCacheManager> logger,
        TimeSpan cacheDuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheDuration = cacheDuration;
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, DirectoryItem>> GetCachedDirectoriesAsync(
        Func<CancellationToken, Task<Dictionary<string, DirectoryItem>>> loadFunc,
        CancellationToken cancellationToken = default,
        bool forceRefresh = false)
    {
        if (!forceRefresh && _directoryCache != null && DateTime.UtcNow < _cacheExpiration)
        {
            return _directoryCache;
        }

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            // Double-check inside the lock
            if (!forceRefresh && _directoryCache != null && DateTime.UtcNow < _cacheExpiration)
            {
                return _directoryCache;
            }

            // Load directories using the provided function
            var directories = await loadFunc(cancellationToken);

            // Update cache
            _directoryCache = directories;
            _cacheExpiration = DateTime.UtcNow.Add(_cacheDuration);

            _logger.LogDebug("Directory cache refreshed, {Count} directories loaded", directories.Count);
            return directories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing directory cache");

            // If we have an existing cache, return it even if expired
            if (_directoryCache != null)
            {
                _logger.LogWarning("Returning stale directory cache after refresh error");
                return _directoryCache;
            }

            throw;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task InvalidateCacheAsync(CancellationToken cancellationToken = default)
    {
        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            _directoryCache = null;
            _cacheExpiration = DateTime.MinValue;
            _logger.LogInformation("Directory cache invalidated");
        }
        finally
        {
            _cacheLock.Release();
        }
    }
}