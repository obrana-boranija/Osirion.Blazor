using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Osirion.Blazor.Cms.Core.Interfaces;

/// <summary>
/// Interface for the content cache service
/// </summary>
public interface IContentCacheService
{
    /// <summary>
    /// Gets a cached item or creates it using the provided factory
    /// </summary>
    Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a cached item if available
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets an item in the cache
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from the cache
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all items with the specified prefix
    /// </summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the entire cache
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of content cache service with multi-level caching support
/// </summary>
public class ContentCacheService : IContentCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache? _distributedCache;
    private readonly ILogger<ContentCacheService> _logger;
    private readonly ContentCacheOptions _options;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly HashSet<string> _keyTracker = new();

    /// <summary>
    /// Initializes a new instance of the ContentCacheService
    /// </summary>
    public ContentCacheService(
        IMemoryCache memoryCache,
        IOptions<ContentCacheOptions> options,
        ILogger<ContentCacheService> logger,
        IDistributedCache? distributedCache = null)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new ContentCacheOptions();
        _distributedCache = distributedCache;
    }

    /// <inheritdoc/>
    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return await factory(cancellationToken);
        }

        key = NormalizeKey(key);

        // Try to get from memory cache first (fast path)
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            _logger.LogTrace("Cache hit for key: {Key}", key);
            return cachedValue;
        }

        // Try to get from distributed cache if available
        if (_distributedCache != null)
        {
            var cachedBytes = await _distributedCache.GetAsync(key, cancellationToken);
            if (cachedBytes != null && cachedBytes.Length > 0)
            {
                try
                {
                    var value = JsonSerializer.Deserialize<T>(cachedBytes);
                    if (value != null)
                    {
                        // Store in memory cache for faster access next time
                        var memoryCacheOptions = new MemoryCacheEntryOptions()
                            .SetSize(cachedBytes.Length)
                            .SetAbsoluteExpiration(expiration ?? TimeSpan.FromMinutes(_options.DefaultDurationMinutes));

                        _memoryCache.Set(key, value, memoryCacheOptions);
                        TrackKey(key);

                        _logger.LogTrace("Distributed cache hit for key: {Key}", key);
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error deserializing cached value for key: {Key}", key);
                }
            }
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

            // Generate the value
            var value = await factory(cancellationToken);
            if (value == null)
            {
                return default;
            }

            // Cache expiration
            var actualExpiration = expiration ?? TimeSpan.FromMinutes(_options.DefaultDurationMinutes);

            // Store in memory cache
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(actualExpiration);

            _memoryCache.Set(key, value, cacheOptions);
            TrackKey(key);

            // Store in distributed cache if available
            if (_distributedCache != null)
            {
                try
                {
                    var serializedValue = JsonSerializer.SerializeToUtf8Bytes(value);

                    // Only cache if within size limits
                    if (serializedValue.Length <= _options.MaxItemSize)
                    {
                        var distributedOptions = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = actualExpiration
                        };

                        await _distributedCache.SetAsync(key, serializedValue, distributedOptions, cancellationToken);
                        _logger.LogTrace("Item stored in distributed cache: {Key}", key);
                    }
                    else
                    {
                        _logger.LogWarning("Item too large for distributed cache: {Key}, Size: {Size} bytes",
                            key, serializedValue.Length);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error storing item in distributed cache: {Key}", key);
                }
            }

            return value;
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return default;
        }

        key = NormalizeKey(key);

        // Try memory cache first
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }

        // Try distributed cache if available
        if (_distributedCache != null)
        {
            var cachedBytes = await _distributedCache.GetAsync(key, cancellationToken);
            if (cachedBytes != null && cachedBytes.Length > 0)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(cachedBytes);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error deserializing cached value for key: {Key}", key);
                }
            }
        }

        return default;
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || value == null)
        {
            return;
        }

        key = NormalizeKey(key);
        var actualExpiration = expiration ?? TimeSpan.FromMinutes(_options.DefaultDurationMinutes);

        // Store in memory cache
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(actualExpiration);

        _memoryCache.Set(key, value, cacheOptions);
        TrackKey(key);

        // Store in distributed cache if available
        if (_distributedCache != null)
        {
            try
            {
                var serializedValue = JsonSerializer.SerializeToUtf8Bytes(value);

                // Only cache if within size limits
                if (serializedValue.Length <= _options.MaxItemSize)
                {
                    var distributedOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = actualExpiration
                    };

                    await _distributedCache.SetAsync(key, serializedValue, distributedOptions, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Item too large for distributed cache: {Key}, Size: {Size} bytes",
                        key, serializedValue.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error storing item in distributed cache: {Key}", key);
            }
        }
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        key = NormalizeKey(key);

        // Remove from memory cache
        _memoryCache.Remove(key);

        // Remove from key tracker
        lock (_keyTracker)
        {
            _keyTracker.Remove(key);
        }

        // Remove from distributed cache if available
        if (_distributedCache != null)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        prefix = NormalizeKey(prefix);

        // Find all keys that start with the prefix
        HashSet<string> keysToRemove;
        lock (_keyTracker)
        {
            keysToRemove = _keyTracker.Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToHashSet();
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
        // Get all tracked keys
        HashSet<string> allKeys;
        lock (_keyTracker)
        {
            allKeys = new HashSet<string>(_keyTracker);
            _keyTracker.Clear();
        }

        // Remove all keys from both caches
        foreach (var key in allKeys)
        {
            _memoryCache.Remove(key);

            if (_distributedCache != null)
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
            }
        }
    }

    private string NormalizeKey(string key)
    {
        // Ensure consistent key format
        return key.Trim().ToLowerInvariant();
    }

    private void TrackKey(string key)
    {
        lock (_keyTracker)
        {
            _keyTracker.Add(key);
        }
    }
}