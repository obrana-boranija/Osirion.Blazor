using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;

namespace Osirion.Blazor.Cms.Infrastructure.Content;

/// <summary>
/// Implementation of IContentCacheManager for managing content cache
/// </summary>
public class ContentCacheManager : IContentCacheManager
{
    private readonly SemaphoreSlim _cacheLock = new(1, 1);
    private readonly ILogger<ContentCacheManager> _logger;
    private readonly TimeSpan _cacheDuration;

    private Dictionary<string, ContentItem>? _contentCache;
    private DateTime _cacheExpiration = DateTime.MinValue;

    public ContentCacheManager(
        ILogger<ContentCacheManager> logger,
        TimeSpan cacheDuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheDuration = cacheDuration;
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, ContentItem>> GetCachedItemsAsync(
        Func<CancellationToken, Task<Dictionary<string, ContentItem>>> loadFunc,
        CancellationToken cancellationToken = default,
        bool forceRefresh = false)
    {
        if (!forceRefresh && _contentCache != null && DateTime.UtcNow < _cacheExpiration)
        {
            return _contentCache;
        }

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            // Double-check inside the lock
            if (!forceRefresh && _contentCache != null && DateTime.UtcNow < _cacheExpiration)
            {
                return _contentCache;
            }

            // Load content items using the provided function
            var items = await loadFunc(cancellationToken);

            // Update cache
            _contentCache = items;
            _cacheExpiration = DateTime.UtcNow.Add(_cacheDuration);

            _logger.LogDebug("Content cache refreshed, {Count} items loaded", items.Count);
            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing content cache");

            // If we have an existing cache, return it even if expired
            if (_contentCache != null)
            {
                _logger.LogWarning("Returning stale content cache after refresh error");
                return _contentCache;
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
            _contentCache = null;
            _cacheExpiration = DateTime.MinValue;
            _logger.LogInformation("Content cache invalidated");
        }
        finally
        {
            _cacheLock.Release();
        }
    }
}