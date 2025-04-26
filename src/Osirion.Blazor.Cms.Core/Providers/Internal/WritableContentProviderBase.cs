using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Exceptions;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;
using System.Collections.Concurrent;

namespace Osirion.Blazor.Cms.Providers;

/// <summary>
/// Base class for content providers with common functionality
/// </summary>
public abstract class ContentProviderBase : IContentProvider, IDisposable
{
    private readonly IContentCacheService _cacheService;
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    private bool _disposed;

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

    /// <inheritdoc/>
    public abstract string ProviderId { get; }

    /// <inheritdoc/>
    public abstract string DisplayName { get; }

    /// <inheritdoc/>
    public abstract bool SupportsWriting { get; }

    /// <inheritdoc/>
    public virtual IContentWriter? GetContentWriter() => null;

    /// <summary>
    /// Gets the cache duration for items from this provider
    /// </summary>
    protected virtual TimeSpan CacheDuration => TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets a cache key for the provider
    /// </summary>
    protected string GetCacheKey(string key) => $"{ProviderId}:{key}";

    /// <summary>
    /// Gets a lock for a specific operation
    /// </summary>
    protected SemaphoreSlim GetLock(string operation)
    {
        return _locks.GetOrAdd(operation, _ => new SemaphoreSlim(1, 1));
    }

    /// <summary>
    /// Executes an operation with an auto-releasing lock
    /// </summary>
    protected async Task<T> WithLockAsync<T>(string operation, Func<Task<T>> action)
    {
        var lockObj = GetLock(operation);
        await lockObj.WaitAsync();
        try
        {
            return await action();
        }
        finally
        {
            lockObj.Release();
        }
    }

    /// <summary>
    /// Gets or creates a cached value using the provided factory
    /// </summary>
    protected Task<T?> GetOrCreateCachedAsync<T>(
        string cacheKey,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        return _cacheService.GetOrCreateAsync(
            cacheKey,
            factory,
            CacheDuration,
            cancellationToken);
    }

    /// <summary>
    /// Wraps an operation with proper exception handling
    /// </summary>
    protected async Task<T> ExecuteWithExceptionHandlingAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        T defaultValue)
    {
        try
        {
            return await operation();
        }
        catch (ContentProviderException)
        {
            // Re-throw provider exceptions
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing operation {OperationName} on provider {ProviderId}",
                operationName, ProviderId);

            throw new ContentProviderException(
                $"Error executing {operationName} on provider {DisplayName}: {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    public virtual Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return ExecuteWithExceptionHandlingAsync(
            async () =>
            {
                var allItems = await GetAllItemsAsync(cancellationToken);
                return allItems.FirstOrDefault(i => i.Id == id);
            },
            "GetItemById",
            default(ContentItem));
    }

    /// <inheritdoc/>
    public abstract Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all content items from the provider
    /// </summary>
    public abstract Task<IReadOnlyList<ContentItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<ContentItem>> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("categories");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
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
        }, cancellationToken) ?? Array.Empty<ContentCategory>();
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<ContentTag>> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey("tags");

        return await GetOrCreateCachedAsync(cacheKey, async ct =>
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
        }, cancellationToken) ?? Array.Empty<ContentTag>();
    }

    /// <inheritdoc/>
    public virtual async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        await _cacheService.RemoveByPrefixAsync(ProviderId, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public abstract Task<IReadOnlyList<DirectoryItem>> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a URL-friendly slug from a string
    /// </summary>
    protected virtual string CreateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "untitled";

        // Quick implementation - a proper implementation should be used in production
        return input.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace(".", "-")
            .Replace(":", "")
            .Replace(";", "")
            .Replace(",", "")
            .Replace("\"", "")
            .Replace("'", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("&", "and");
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
            foreach (var lockObj in _locks.Values)
            {
                lockObj.Dispose();
            }
            _locks.Clear();
        }

        _disposed = true;
    }
}

/// <summary>
/// Base class for read-only content providers
/// </summary>
public abstract class ReadOnlyContentProviderBase : ContentProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyContentProviderBase"/> class.
    /// </summary>
    protected ReadOnlyContentProviderBase(IContentCacheService cacheService, ILogger logger)
        : base(cacheService, logger)
    {
    }

    /// <inheritdoc/>
    public override bool SupportsWriting => false;
}

/// <summary>
/// Base class for writable content providers
/// </summary>
public abstract class WritableContentProviderBase : ContentProviderBase, IContentWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WritableContentProviderBase"/> class.
    /// </summary>
    protected WritableContentProviderBase(IContentCacheService cacheService, ILogger logger)
        : base(cacheService, logger)
    {
    }

    /// <inheritdoc/>
    public override bool SupportsWriting => true;

    /// <inheritdoc/>
    public override IContentWriter GetContentWriter() => this;

    /// <inheritdoc/>
    public abstract Task<ContentItem> CreateContentAsync(ContentItem item, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<ContentItem> UpdateContentAsync(ContentItem item, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task DeleteContentAsync(string id, string? commitMessage = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<DirectoryItem> CreateDirectoryAsync(DirectoryItem directory, CancellationToken cancellationToken = default);
}