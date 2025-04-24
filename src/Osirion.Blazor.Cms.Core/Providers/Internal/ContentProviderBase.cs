using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Osirion.Blazor.Cms.Models;
using Osirion.Blazor.Cms.Interfaces;
using System.Text.RegularExpressions;
using Osirion.Blazor.Core.Extensions;

namespace Osirion.Blazor.Cms.Providers.Internal;

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
    /// SemaphoreSlim for thread safety
    /// </summary>
    protected readonly SemaphoreSlim _cacheLock = new(1, 1);

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
    public abstract Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default);

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
                Slug = group.Key.ToUrlSlug(),
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
                Slug = group.Key.ToUrlSlug(),
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
                GetCacheKey("tags"),
                GetCacheKey("directories"),
                GetCacheKey("localization")
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

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey($"directories:{locale ?? "all"}");

        return await GetOrCreateCachedAsync(cacheKey, async token =>
        {
            // Default implementation returns an empty list
            // Derived classes should override this method to provide actual directories
            return await Task.FromResult(new List<DirectoryItem>().AsReadOnly());
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        var directories = await GetDirectoriesAsync(null, cancellationToken);
        return directories.FirstOrDefault(d => string.Equals(d.Path, path, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public virtual async Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        var directories = await GetDirectoriesAsync(locale, cancellationToken);
        return directories.FirstOrDefault(d => string.Equals(d.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public virtual async Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("localization");

        return await GetOrCreateCachedAsync(cacheKey, async token =>
        {
            // Default implementation returns a basic localization info
            // Derived classes should override this method to provide actual localization information
            return await Task.FromResult(new LocalizationInfo
            {
                DefaultLocale = "en",
                //SupportedLocales = new List<string> { "en" }
            });
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentItem>> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(localizationId))
            return new List<ContentItem>().AsReadOnly();

        var query = new ContentQuery { LocalizationId = localizationId };
        return await GetItemsByQueryAsync(query, cancellationToken);
    }

    private static string GenerateRandomLetters(int length)
    {
        Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Generates an item URL
    /// </summary>
    protected string GenerateUrl(string path, string slug, string? skipSegment = null)
    {
        // Check if pathToTrim is null, whitespace, or just "/"
        bool skipPrefixRemoval = string.IsNullOrWhiteSpace(skipSegment) || skipSegment == "/";

        // Step 1: Remove string a from the beginning of string b
        if (!skipPrefixRemoval && path.StartsWith(skipSegment!))
        {
            // Only remove 'a' if it's followed by a slash or is the entire string
            if (path.Length == skipSegment!.Length || path[skipSegment.Length] == '/')
            {
                // If a is followed by a slash, remove both a and the slash
                // If a is the entire string, this will result in an empty string
                path = path.Length > skipSegment.Length ? path.Substring(skipSegment.Length + 1) : "";
            }
        }

        // Step 2: Remove the last slash-delimited segment from string b
        int lastSlashIndex = path.LastIndexOf('/');
        if (lastSlashIndex >= 0)
        {
            path = path.Substring(0, lastSlashIndex);
        }
        else
        {
            // If there's no slash, b is a single segment, so clear it
            path = "";
        }

        // Step 3: Append string c, with a slash if b is not empty
        if (!string.IsNullOrEmpty(path))
        {
            return path + "/" + slug;
        }
        else
        {
            return slug;
        }
    }

    /// <summary>
    /// Gets a cache key for the provider
    /// </summary>
    protected string GetCacheKey(string key)
    {
        return $"{ProviderId}:{key}";
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

    public Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}