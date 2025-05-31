using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.Caching;

/// <summary>
/// Implements the stale-while-revalidate caching pattern for directory repositories
/// </summary>
public class StaleWhileRevalidateDirectoryCache : IDirectoryRepository
{
    private readonly IDirectoryRepository _decorated;
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    private readonly TimeSpan _staleTime;
    private readonly TimeSpan _maxAge;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly string _providerIdentifier;

    public StaleWhileRevalidateDirectoryCache(
        IDirectoryRepository decorated,
        IMemoryCache cache,
        ILogger<StaleWhileRevalidateDirectoryCache> logger,
        TimeSpan staleTime,
        TimeSpan maxAge,
        string providerIdentifier)
    {
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _staleTime = staleTime;
        _maxAge = maxAge;
        _providerIdentifier = providerIdentifier ?? throw new ArgumentNullException(nameof(providerIdentifier));
    }

    public async Task<IReadOnlyList<DirectoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"directory:all:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetAllAsync(cancellationToken),
            cancellationToken);
    }

    public async Task<DirectoryItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        var cacheKey = $"directory:{id}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByIdAsync(id, cancellationToken),
            cancellationToken);
    }

    public async Task<DirectoryItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        var cacheKey = $"directory:path:{path}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByPathAsync(path, cancellationToken),
            cancellationToken);
    }

    public async Task<DirectoryItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        var cacheKey = $"directory:url:{url}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByUrlAsync(url, cancellationToken),
            cancellationToken);
    }

    public async Task<IReadOnlyList<DirectoryItem>> GetByLocaleAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var localeKey = locale ?? "all";
        var cacheKey = $"directory:locale:{localeKey}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByLocaleAsync(locale, cancellationToken),
            cancellationToken);
    }

    public async Task<IReadOnlyList<DirectoryItem>> GetChildrenAsync(string parentId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"directory:children:{parentId}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetChildrenAsync(parentId, cancellationToken),
            cancellationToken);
    }

    public async Task<IReadOnlyList<DirectoryItem>> GetTreeAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var localeKey = locale ?? "all";
        var cacheKey = $"directory:tree:{localeKey}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetTreeAsync(locale, cancellationToken),
            cancellationToken);
    }

    public async Task<DirectoryItem?> GetByNameAsync(string? name, string? locale = null, CancellationToken cancellationToken = default)
    {
        var localeKey = locale ?? "all";
        var cacheKey = $"directory:name:{localeKey}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByNameAsync(name, locale, cancellationToken),
            cancellationToken);
    }

    // Write operations should invalidate the cache
    public async Task<DirectoryItem> SaveAsync(DirectoryItem entity, CancellationToken cancellationToken = default)
    {
        var result = await _decorated.SaveAsync(entity, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
        return result;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _decorated.DeleteAsync(id, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
    }

    public async Task DeleteRecursiveAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default)
    {
        await _decorated.DeleteRecursiveAsync(id, commitMessage, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
    }

    public async Task<DirectoryItem> MoveAsync(string id, string? newParentId, CancellationToken cancellationToken = default)
    {
        var result = await _decorated.MoveAsync(id, newParentId, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
        return result;
    }

    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            // Clear the entire cache for this provider
            if (_cache is MemoryCache memoryCache)
            {
                // Use compact to remove a significant portion of the cache
                memoryCache.Compact(1.0);
                _logger.LogInformation("Cache cleared for provider: {ProviderId}", _providerIdentifier);
            }

            // Also refresh the underlying repository's cache
            await _decorated.RefreshCacheAsync(cancellationToken);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task<T> GetOrRevalidateAsync<T>(
        string cacheKey,
        Func<Task<T>> factory,
        CancellationToken cancellationToken)
    {
        // Try to get the value from the cache
        if (_cache.TryGetValue(cacheKey, out CacheEntry<T>? cacheEntry) && cacheEntry is not null)
        {
            // Check if the entry is stale and needs background refresh
            if (DateTime.UtcNow - cacheEntry.LastUpdated > _staleTime && !cacheEntry.IsRefreshing)
            {
                // Start background refresh
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await RefreshCacheEntryAsync(cacheKey, factory, cacheEntry);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error refreshing cache entry: {CacheKey}", cacheKey);
                    }
                }, CancellationToken.None);
            }

            return cacheEntry.Value;
        }

        // Not in cache, create it
        return await CreateCacheEntryAsync(cacheKey, factory, cancellationToken);
    }

    private async Task<T> CreateCacheEntryAsync<T>(
        string cacheKey,
        Func<Task<T>> factory,
        CancellationToken cancellationToken)
    {
        // Ensure we don't have multiple threads creating the same entry
        await _refreshLock.WaitAsync(cancellationToken);
        try
        {
            // Double check if another thread already created it
            if (_cache.TryGetValue(cacheKey, out CacheEntry<T>? existingEntry) && existingEntry is not null)
            {
                return existingEntry.Value;
            }

            // Get the value from the source
            var value = await factory();

            // Create a new cache entry
            var cacheEntry = new CacheEntry<T>
            {
                Value = value,
                LastUpdated = DateTime.UtcNow,
                IsRefreshing = false
            };

            // Store in cache
            _cache.Set(cacheKey, cacheEntry, _maxAge);
            _logger.LogDebug("Created cache entry: {CacheKey}", cacheKey);

            return value;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task RefreshCacheEntryAsync<T>(
        string cacheKey,
        Func<Task<T>> factory,
        CacheEntry<T> existingEntry)
    {
        // Prevent concurrent refreshes of the same entry
        await _refreshLock.WaitAsync();
        try
        {
            // Check if the entry is still in the cache and not being refreshed
            if (_cache.TryGetValue(cacheKey, out CacheEntry<T>? currentEntry) &&
                currentEntry is not null &&
                !currentEntry.IsRefreshing)
            {
                // Mark as refreshing
                currentEntry.IsRefreshing = true;

                // Refresh the value
                _logger.LogDebug("Refreshing stale cache entry: {CacheKey}", cacheKey);
                var freshValue = await factory();

                // Update the entry
                var updatedEntry = new CacheEntry<T>
                {
                    Value = freshValue,
                    LastUpdated = DateTime.UtcNow,
                    IsRefreshing = false
                };

                // Store updated entry
                _cache.Set(cacheKey, updatedEntry, _maxAge);
                _logger.LogDebug("Refreshed cache entry: {CacheKey}", cacheKey);
            }
        }
        catch (Exception ex)
        {
            // If refresh fails, mark the entry as not refreshing so it can be tried again
            if (_cache.TryGetValue(cacheKey, out CacheEntry<T>? currentEntry) && currentEntry is not null)
            {
                currentEntry.IsRefreshing = false;
            }

            _logger.LogError(ex, "Failed to refresh cache entry: {CacheKey}", cacheKey);
            throw;
        }
        finally
        {
            _refreshLock.Release();
        }
    }
}