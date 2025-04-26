using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Exceptions;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Providers.Base;

/// <summary>
/// Base class for content providers with common functionality
/// </summary>
public abstract class ContentProviderBase : IContentProvider, IDisposable
{
    private readonly IContentCacheService _cacheService;
    private readonly ILogger _logger;
    private bool _disposed;
    private Microsoft.Extensions.Caching.Memory.IMemoryCache _memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentProviderBase"/> class.
    /// </summary>
    protected ContentProviderBase(
        IContentCacheService cacheService,
        ILogger logger)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected ContentProviderBase(ILogger<FileSystemContentProvider> logger, Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
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
            async () => await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    // Get all items and find the one with matching ID
                    var allItems = await GetAllItemsAsync(ct);
                    return allItems.FirstOrDefault(i => i.Id == id);
                },
                CacheDuration,
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
    public virtual async Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("categories");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var items = await GetAllItemsAsync(ct);

                    return items
                        .SelectMany(item => item.Categories)
                        .GroupBy(category => category.ToLowerInvariant())
                        .Select(group => new ContentCategory
                        {
                            Name = group.First(),
                            Slug = CreateSlug(group.First()),
                            Count = group.Count()
                        })
                        .OrderBy(c => c.Name)
                        .ToList()
                        .AsReadOnly();
                },
                CacheDuration,
                cancellationToken),
            nameof(GetCategoriesAsync));
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("tags");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var items = await GetAllItemsAsync(ct);

                    return items
                        .SelectMany(item => item.Tags)
                        .GroupBy(tag => tag.ToLowerInvariant())
                        .Select(group => new ContentTag
                        {
                            Name = group.First(),
                            Slug = CreateSlug(group.First()),
                            Count = group.Count()
                        })
                        .OrderBy(t => t.Name)
                        .ToList()
                        .AsReadOnly();
                },
                CacheDuration,
                cancellationToken),
            nameof(GetTagsAsync));
    }

    /// <inheritdoc/>
    public virtual async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await _cacheService.RemoveByPrefixAsync(ProviderId, cancellationToken);
        _logger.LogInformation("Cache refreshed for provider: {ProviderId}", ProviderId);
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
            async () => await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var directories = await GetDirectoriesAsync(null, ct);
                    return directories.FirstOrDefault(d => d.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
                },
                CacheDuration,
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
            async () => await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var directories = await GetDirectoriesAsync(locale, ct);
                    return FindDirectoryById(directories, id);
                },
                CacheDuration,
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
            async () => await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var directories = await GetDirectoriesAsync(null, ct);
                    return FindDirectoryByUrl(directories, url);
                },
                CacheDuration,
                cancellationToken),
            nameof(GetDirectoryByUrlAsync));
    }

    /// <inheritdoc/>
    public virtual async Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("localization");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    // Default implementation returns a basic localization info
                    // Derived classes should override this method to provide actual localization information
                    var info = new LocalizationInfo
                    {
                        DefaultLocale = "en"
                    };

                    info.AddLocale("en", "English");

                    return info;
                },
                CacheDuration,
                cancellationToken),
            nameof(GetLocalizationInfoAsync));
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentItem>> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(localizationId))
            return Array.Empty<ContentItem>();

        var cacheKey = GetCacheKey($"translations:{localizationId}");

        return await ExecuteWithExceptionHandlingAsync(
            async () => await _cacheService.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var query = new ContentQuery { LocalizationId = localizationId };
                    return await GetItemsByQueryAsync(query, ct);
                },
                CacheDuration,
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
    /// Creates a URL-friendly slug from a string
    /// </summary>
    protected virtual string CreateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "untitled";

        // Convert to lowercase
        var slug = input.ToLowerInvariant();

        // Remove accents, replace spaces with hyphens, remove invalid chars
        slug = slug
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace(".", "-");

        // Remove any characters that aren't alphanumerics, hyphens, or underscores
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-_]", "");

        // Replace double hyphens with single hyphen
        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        // Trim hyphens from start and end
        slug = slug.Trim('-');

        // If slug is empty, use "untitled"
        if (string.IsNullOrEmpty(slug))
            return "untitled";

        return slug;
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
            _logger.LogError(ex, "Error in {OperationName} for provider {ProviderId}: {Message}",
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

    /// <summary>
    /// Gets or creates a cached value using async factory method
    /// </summary>
    protected async Task<T> GetOrCreateCachedAsync<T>(
    string cacheKey,
    Func<CancellationToken, Task<T>> factory,
    CancellationToken cancellationToken = default)
    {
        // Try to get from cache first
        if (_memoryCache.TryGetValue(cacheKey, out T? cachedValue))
        {
            return cachedValue!;
        }

        // Compute the value - we accept that multiple threads might do this simultaneously
        var value = await factory(cancellationToken);

        // Store in cache with absolute expiration
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(CacheDuration);

        _memoryCache.Set(cacheKey, value, cacheEntryOptions);
        return value;
    }
}