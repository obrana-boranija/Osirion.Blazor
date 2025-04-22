using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Osirion.Blazor.Content.Models;

namespace Osirion.Blazor.Content.Providers.Internal;

/// <summary>
/// Base class for content providers with common functionality
/// </summary>
public abstract class ContentProviderBase : IContentProvider
{
    /// <summary>
    /// Logger instance
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Memory cache instance
    /// </summary>
    protected readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Lock object for thread safety
    /// </summary>
    protected readonly object _cacheLock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderBase"/> class.
    /// </summary>
    protected ContentProviderBase(
        ILogger logger,
        IMemoryCache memoryCache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    /// <inheritdoc/>
    public abstract string ProviderId { get; }

    /// <inheritdoc/>
    public abstract string DisplayName { get; }

    /// <inheritdoc/>
    public abstract bool IsReadOnly { get; }

    /// <summary>
    /// Gets the cache duration
    /// </summary>
    protected virtual TimeSpan CacheDuration => TimeSpan.FromMinutes(30);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var items = await GetAllItemsAsync(cancellationToken);

        return items
            .SelectMany(item => item.Categories)
            .GroupBy(category => category.ToLowerInvariant())
            .Select(group => new ContentCategory
            {
                Name = group.First(),
                Slug = GenerateSlug(group.Key),
                Count = group.Count()
            })
            .OrderBy(c => c.Name)
            .ToList();
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        var items = await GetAllItemsAsync(cancellationToken);

        return items
            .SelectMany(item => item.Tags)
            .GroupBy(tag => tag.ToLowerInvariant())
            .Select(group => new ContentTag
            {
                Name = group.First(),
                Slug = GenerateSlug(group.Key),
                Count = group.Count()
            })
            .OrderBy(t => t.Name)
            .ToList();
    }

    /// <inheritdoc/>
    public virtual Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        lock (_cacheLock)
        {
            var cacheKeys = new[]
            {
                GetCacheKey("content:all"),
                GetCacheKey("categories"),
                GetCacheKey("tags")
            };

            foreach (var key in cacheKeys)
            {
                _memoryCache.Remove(key);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Generates a URL-friendly slug from text
    /// </summary>
    protected string GenerateSlug(string text)
    {
        return text.Trim()
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");
    }

    /// <summary>
    /// Gets a cache key for the provider
    /// </summary>
    protected string GetCacheKey(string key)
    {
        return $"{ProviderId}:{key}";
    }

    /// <summary>
    /// Gets or creates cached data
    /// </summary>
    protected async Task<T?> GetOrCreateCachedAsync<T>(
        string cacheKey,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            cacheKey,
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                return await factory(cancellationToken);
            });
    }
}