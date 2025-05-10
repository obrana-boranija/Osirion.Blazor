using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Extensions;

namespace Osirion.Blazor.Cms.Infrastructure.Providers;

/// <summary>
/// Base class for content providers with in-memory caching
/// </summary>
public abstract class ContentProviderBase : IContentProvider, IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly ContentProviderOptions _options;
    protected readonly ILogger Logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderBase"/> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache</param>
    /// <param name="logger">The logger</param>
    protected ContentProviderBase(
        IMemoryCache memoryCache,
        IOptions<ContentProviderOptions> options,
        ILogger logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _options = options.Value;
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public abstract string ProviderId { get; }

    /// <inheritdoc/>
    public abstract string DisplayName { get; }

    /// <inheritdoc/>
    public abstract bool IsReadOnly { get; }

    /// <summary>
    /// Gets the cache duration for items from this provider
    /// </summary>
    protected virtual TimeSpan CacheDuration => TimeSpan.FromMinutes(30);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        var cacheKey = GetCacheKey($"item:id:{id}");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await GetOrCreateCachedAsync(
                cacheKey,
                async ct =>
                {
                    // Get all items and find the one with matching ID
                    var allItems = await GetAllItemsAsync(ct);
                    return allItems.FirstOrDefault(i => i.Id == id);
                },
                cancellationToken),
            nameof(GetItemByIdAsync));
    }

    /// <inheritdoc/>
    public abstract Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentCategory>?> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("categories");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await GetOrCreateCachedAsync(
                cacheKey,
                async ct =>
                {
                    var items = await GetAllItemsAsync(ct);

                    return items
                        .SelectMany(item => item.Categories)
                        .GroupBy(category => category.ToLowerInvariant())
                        .Select(group => ContentCategory.Create(
                            group.First(),
                            group.First().ToUrlSlug(),
                            group.Count()))
                        .OrderBy(c => c.Name)
                        .ToList()
                        .AsReadOnly();
                },
                cancellationToken),
            nameof(GetCategoriesAsync));
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentTag>?> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("tags");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await GetOrCreateCachedAsync(
                cacheKey,
                async ct =>
                {
                    var items = await GetAllItemsAsync(ct);

                    return items
                        .SelectMany(item => item.Tags)
                        .GroupBy(tag => tag.ToLowerInvariant())
                        .Select(group => ContentTag.Create(
                            group.First(),
                            group.First().ToUrlSlug(),
                            group.Count()))
                        .OrderBy(t => t.Name)
                        .ToList()
                        .AsReadOnly();
                },
                cancellationToken),
            nameof(GetTagsAsync));
    }

    /// <inheritdoc/>
    public virtual async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        // Get all cache keys for this provider
        var cachePrefix = $"{ProviderId}:";

        // There's no direct way to clear by prefix in MemoryCache
        // For a server-side app, we can just clear everything related to this provider
        if (_memoryCache is MemoryCache memCache)
        {
            // Use Compact with percentage 1.0 to clear all
            memCache.Compact(1.0);
            Logger.LogInformation("Cleared entire in-memory cache for provider: {ProviderId}", ProviderId);
        }
    }

    /// <inheritdoc/>
    public virtual Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        var cacheKey = GetCacheKey($"directory:path:{path}");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await GetOrCreateCachedAsync(
                cacheKey,
                async ct =>
                {
                    var directories = await GetDirectoriesAsync(null, ct);
                    return FindDirectoryByPath(directories, path);
                },
                cancellationToken),
            nameof(GetDirectoryByPathAsync));
    }

    /// <inheritdoc/>
    public virtual async Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        var cacheKey = GetCacheKey($"directory:id:{id}");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await GetOrCreateCachedAsync(
                cacheKey,
                async ct =>
                {
                    var directories = await GetDirectoriesAsync(locale, ct);
                    return FindDirectoryById(directories, id);
                },
                cancellationToken),
            nameof(GetDirectoryByIdAsync));
    }

    /// <inheritdoc/>
    public virtual async Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        var cacheKey = GetCacheKey($"directory:url:{url}");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await GetOrCreateCachedAsync(
                cacheKey,
                async ct =>
                {
                    var directories = await GetDirectoriesAsync(null, ct);
                    return FindDirectoryByUrl(directories, url);
                },
                cancellationToken),
            nameof(GetDirectoryByUrlAsync));
    }

    /// <inheritdoc/>
    //public virtual async Task<LocalizationInfo?> GetLocalizationInfoAsync(CancellationToken cancellationToken = default)
    //{
    //    var cacheKey = GetCacheKey("localization");

    //    return await ExecuteWithExceptionHandlingAsync(
    //        async () => await GetOrCreateCachedAsync(
    //            cacheKey,
    //            async ct =>
    //            {
    //                // Default implementation returns a basic localization info
    //                // Derived classes should override this method to provide actual localization information
    //                var info = new LocalizationInfo
    //                {
    //                    DefaultLocale = "en"
    //                };

    //                info.AddLocale("en", "English");

    //                return info;
    //            },
    //            cancellationToken),
    //        nameof(GetLocalizationInfoAsync));
    //}

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentItem>?> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(localizationId))
            return Array.Empty<ContentItem>();

        var cacheKey = GetCacheKey($"translations:{localizationId}");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await GetOrCreateCachedAsync(
                cacheKey,
                async ct =>
                {
                    var query = new ContentQuery { LocalizationId = localizationId };
                    return await GetItemsByQueryAsync(query, ct);
                },
                cancellationToken),
            nameof(GetContentTranslationsAsync));
    }

    /// <summary>
    /// Gets a cache key for the provider
    /// </summary>
    protected string GetCacheKey(string key)
    {
        return $"{ProviderId}:{key}";
    }

    /// <summary>
    /// Simple in-memory caching implementation
    /// </summary>
    protected async Task<T?> GetOrCreateCachedAsync<T>(
        string cacheKey,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        // See if the item is in the cache
        if (_memoryCache.TryGetValue(cacheKey, out T? cachedValue))
        {
            Logger.LogDebug("Cache hit for key: {Key}", cacheKey);
            return cachedValue;
        }

        // Not in cache, create it
        Logger.LogDebug("Cache miss for key: {Key}", cacheKey);
        var result = await factory(cancellationToken);

        // Only cache non-null values
        if (result != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration);

            _memoryCache.Set(cacheKey, result, cacheOptions);
            Logger.LogDebug("Added to cache: {Key}", cacheKey);
        }

        return result;
    }

    /// <summary>
    /// Finds a directory with the specified path in a directory tree
    /// </summary>
    protected DirectoryItem? FindDirectoryByPath(IEnumerable<DirectoryItem> directories, string path)
    {
        var normalizedPath = path.Replace('\\', '/').TrimEnd('/');

        foreach (var directory in directories)
        {
            var directoryPath = directory.Path.Replace('\\', '/').TrimEnd('/');
            if (directoryPath.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase))
                return directory;

            var childResult = FindDirectoryByPath(directory.Children, path);
            if (childResult != null)
                return childResult;
        }

        return null;
    }

    /// <summary>
    /// Finds a directory with the specified ID in a directory tree
    /// </summary>
    protected DirectoryItem? FindDirectoryById(IEnumerable<DirectoryItem> directories, string id)
    {
        foreach (var directory in directories)
        {
            if (directory.Id == id)
                return directory;

            var childResult = FindDirectoryById(directory.Children, id);
            if (childResult != null)
                return childResult;
        }

        return null;
    }

    /// <summary>
    /// Finds a directory with the specified URL in a directory tree
    /// </summary>
    protected DirectoryItem? FindDirectoryByUrl(IEnumerable<DirectoryItem> directories, string url)
    {
        foreach (var directory in directories)
        {
            if (directory.Url.Equals(url, StringComparison.OrdinalIgnoreCase))
                return directory;

            var childResult = FindDirectoryByUrl(directory.Children, url);
            if (childResult != null)
                return childResult;
        }

        return null;
    }

    /// <summary>
    /// Executes an operation with exception handling
    /// </summary>
    protected async Task<T?> ExecuteWithExceptionHandlingAsync<T>(Func<Task<T?>> operation, string operationName)
    {
        try
        {
            return await operation();
        }
        catch (ContentProviderException)
        {
            // Re-throw content provider exceptions
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in {OperationName} for provider {ProviderId}: {Message}",
                operationName, ProviderId, ex.Message);

            throw new ContentProviderException(
                $"Error in {operationName} for provider {DisplayName}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Disposes resources used by the provider
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes resources used by the provider
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public virtual async Task<Dictionary<string, ContentItem>> GetContentTranslationsAsync(string localizationId)
    {
        if (string.IsNullOrEmpty(localizationId))
        {
            return new Dictionary<string, ContentItem>();
        }

        // Try to get from cache first
        string cacheKey = $"translations_{localizationId}";
        if (_memoryCache.TryGetValue(cacheKey, out Dictionary<string, ContentItem>? cachedTranslations) &&
            cachedTranslations != null)
        {
            return cachedTranslations;
        }

        try
        {
            // Create a dictionary to hold translations
            var translations = new Dictionary<string, ContentItem>();

            // For each supported locale, try to find content with the same localization ID
            foreach (var locale in _options.SupportedLocales)
            {
                // Query by localization ID and locale
                var query = new ContentQuery
                {
                    LocalizationId = localizationId,
                    Locale = locale,
                    Take = 1
                };

                var results = await GetItemsByQueryAsync(query);
                var contentItem = results.FirstOrDefault();

                if (contentItem != null)
                {
                    translations[locale] = contentItem;
                }
            }

            // Cache the result
            if (_options.EnableCaching)
            {
                _memoryCache.Set(cacheKey, translations, TimeSpan.FromMinutes(_options.CacheDurationMinutes));
            }

            return translations;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving translations for localization ID {LocalizationId}", localizationId);
            return new Dictionary<string, ContentItem>();
        }
    }
}