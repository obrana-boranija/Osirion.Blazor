using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Configuration;
using Osirion.Blazor.Cms.Core.Configuration;
using Osirion.Blazor.Cms.Caching;

namespace Osirion.Blazor.Cms.Core.Caching;

/// <summary>
/// Implementation of the IContentCacheService interface using IMemoryCache
/// </summary>
public class ContentCacheService : IContentCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ContentCacheService> _logger;
    private readonly ContentCacheOptions _options;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly HashSet<string> _cachedKeys = new();

    /// <summary>
    /// Initializes a new instance of the ContentCacheService
    /// </summary>
    public ContentCacheService(
        IMemoryCache memoryCache,
        IOptions<ContentCacheOptions> options,
        ILogger<ContentCacheService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new ContentCacheOptions();
    }

    /// <inheritdoc/>
    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        // If caching is disabled, just call the factory
        if (!_options.Enabled)
        {
            return await factory(cancellationToken);
        }

        key = NormalizeKey(key);

        // Try to get from cache first
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            _logger.LogTrace("Cache hit for key: {Key}", key);
            return cachedValue;
        }

        // Cache miss, acquire lock for creation
        await _lock.WaitAsync(cancellationToken);
        try
        {
            // Double check if another thread already created the item
            if (_memoryCache.TryGetValue(key, out cachedValue))
            {
                return cachedValue;
            }

            // Create the value
            var value = await factory(cancellationToken);
            if (value == null)
            {
                return default;
            }

            // Store in cache
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration ?? TimeSpan.FromMinutes(_options.DefaultDurationMinutes));

            if (_options.SetSizeLimit)
            {
                cacheOptions.SetSize(1); // Default size of 1 unit
            }

            _memoryCache.Set(key, value, cacheOptions);
            TrackKey(key);

            return value;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc/>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return Task.FromResult<T?>(default);
        }

        key = NormalizeKey(key);

        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            return Task.FromResult(cachedValue);
        }

        return Task.FromResult<T?>(default);
    }

    /// <inheritdoc/>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || value == null)
        {
            return Task.CompletedTask;
        }

        key = NormalizeKey(key);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration ?? TimeSpan.FromMinutes(_options.DefaultDurationMinutes));

        if (_options.SetSizeLimit)
        {
            cacheOptions.SetSize(1); // Default size of 1 unit
        }

        _memoryCache.Set(key, value, cacheOptions);
        TrackKey(key);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        key = NormalizeKey(key);

        _memoryCache.Remove(key);

        lock (_cachedKeys)
        {
            _cachedKeys.Remove(key);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        prefix = NormalizeKey(prefix);

        // Find all keys with the given prefix
        HashSet<string> keysToRemove;
        lock (_cachedKeys)
        {
            keysToRemove = _cachedKeys
                .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToHashSet();
        }

        // Remove each key
        foreach (var key in keysToRemove)
        {
            await RemoveAsync(key, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        HashSet<string> allKeys;
        lock (_cachedKeys)
        {
            allKeys = new HashSet<string>(_cachedKeys);
            _cachedKeys.Clear();
        }

        foreach (var key in allKeys)
        {
            await RemoveAsync(key, cancellationToken);
        }
    }

    private string NormalizeKey(string key)
    {
        return key.Trim().ToLowerInvariant();
    }

    private void TrackKey(string key)
    {
        lock (_cachedKeys)
        {
            _cachedKeys.Add(key);
        }
    }
}