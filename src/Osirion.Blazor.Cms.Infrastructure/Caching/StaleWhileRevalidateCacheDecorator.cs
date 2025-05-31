using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using System.Text;

namespace Osirion.Blazor.Cms.Infrastructure.Caching;

/// <summary>
/// Implements the stale-while-revalidate caching pattern for content repositories
/// </summary>
public class StaleWhileRevalidateCacheDecorator : IContentRepository
{
    private readonly IContentRepository _decorated;
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    private readonly TimeSpan _staleTime;
    private readonly TimeSpan _maxAge;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private readonly string _providerIdentifier;

    public StaleWhileRevalidateCacheDecorator(
        IContentRepository decorated,
        IMemoryCache cache,
        ILogger<StaleWhileRevalidateCacheDecorator> logger,
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

    public async Task<IReadOnlyList<ContentItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"content:all:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetAllAsync(cancellationToken),
            cancellationToken);
    }

    public async Task<ContentItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        var cacheKey = $"content:{id}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByIdAsync(id, cancellationToken),
            cancellationToken);
    }

    public async Task<ContentItem?> GetByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty", nameof(path));

        var cacheKey = $"content:path:{path}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByPathAsync(path, cancellationToken),
            cancellationToken);
    }

    public async Task<ContentItem?> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));

        var cacheKey = $"content:url:{url}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByUrlAsync(url, cancellationToken),
            cancellationToken);
    }

    public async Task<IReadOnlyList<ContentItem>> FindByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        // Generate a cache key based on the query properties
        var cacheKey = GenerateQueryCacheKey(query);

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.FindByQueryAsync(query, cancellationToken),
            cancellationToken);
    }

    public async Task<IReadOnlyList<ContentItem>> GetByDirectoryAsync(string directoryId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"content:directory:{directoryId}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetByDirectoryAsync(directoryId, cancellationToken),
            cancellationToken);
    }

    public async Task<IReadOnlyList<ContentItem>> GetTranslationsAsync(string contentId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"content:translations:{contentId}:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetTranslationsAsync(contentId, cancellationToken),
            cancellationToken);
    }

    public async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"content:tags:{_providerIdentifier}";

        return await GetOrRevalidateAsync(
            cacheKey,
            () => _decorated.GetTagsAsync(cancellationToken),
            cancellationToken);
    }

    // Write operations should invalidate the cache
    public async Task<ContentItem> SaveAsync(ContentItem entity, CancellationToken cancellationToken = default)
    {
        var result = await _decorated.SaveAsync(entity, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
        return result;
    }

    public async Task<ContentItem> SaveWithCommitMessageAsync(ContentItem entity, string commitMessage, CancellationToken cancellationToken = default)
    {
        var result = await _decorated.SaveWithCommitMessageAsync(entity, commitMessage, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
        return result;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _decorated.DeleteAsync(id, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
    }

    public async Task DeleteWithCommitMessageAsync(string id, string commitMessage, CancellationToken cancellationToken = default)
    {
        await _decorated.DeleteWithCommitMessageAsync(id, commitMessage, cancellationToken);
        await RefreshCacheAsync(cancellationToken);
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

    private string GenerateQueryCacheKey(ContentQuery query)
    {
        // Create a deterministic cache key based on query properties
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"query:{_providerIdentifier}:");

        // Add significant query parameters
        if (!string.IsNullOrWhiteSpace(query.Directory))
            keyBuilder.Append($"dir:{query.Directory}:");

        if (!string.IsNullOrWhiteSpace(query.DirectoryId))
            keyBuilder.Append($"dirid:{query.DirectoryId}:");

        if (!string.IsNullOrWhiteSpace(query.Slug))
            keyBuilder.Append($"slug:{query.Slug}:");

        if (!string.IsNullOrWhiteSpace(query.Category))
            keyBuilder.Append($"cat:{query.Category}:");

        if (!string.IsNullOrWhiteSpace(query.Tag))
            keyBuilder.Append($"tag:{query.Tag}:");

        if (!string.IsNullOrWhiteSpace(query.SearchQuery))
            keyBuilder.Append($"search:{query.SearchQuery}:");

        if (query.IsFeatured.HasValue)
            keyBuilder.Append($"featured:{query.IsFeatured}:");

        if (query.Status.HasValue)
            keyBuilder.Append($"status:{query.Status}:");

        if (!string.IsNullOrWhiteSpace(query.Author))
            keyBuilder.Append($"author:{query.Author}:");

        if (query.DateFrom.HasValue)
            keyBuilder.Append($"from:{query.DateFrom.Value:yyyyMMdd}:");

        if (query.DateTo.HasValue)
            keyBuilder.Append($"to:{query.DateTo.Value:yyyyMMdd}:");

        if (!string.IsNullOrWhiteSpace(query.Locale))
            keyBuilder.Append($"locale:{query.Locale}:");

        // Add sort parameters
        keyBuilder.Append($"sort:{query.SortBy}:{query.SortDirection}:");

        // Add pagination parameters
        if (query.Skip.HasValue)
            keyBuilder.Append($"skip:{query.Skip}:");

        if (query.Take.HasValue)
            keyBuilder.Append($"take:{query.Take}:");

        return keyBuilder.ToString();
    }
}