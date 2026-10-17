using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;

#pragma warning disable CS0618 // CacheOptions is obsolete, and this code still honors it until it is removed.

namespace Osirion.Blazor.Cms.Infrastructure.Services;

/// <summary>
/// In-memory implementation of <see cref="IContentCacheService"/> using <see cref="IMemoryCache"/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="InMemoryContentCacheService"/> class
/// </remarks>
/// <param name="memoryCache">The memory cache service</param>
/// <param name="logger">The logger instance</param>
/// <param name="options">Cache configuration options</param>
/// <exception cref="ArgumentNullException">Thrown when required dependencies are null</exception>
public class InMemoryContentCacheService(
    IMemoryCache memoryCache,
    ILogger<InMemoryContentCacheService> logger,
    IOptions<CacheOptions> options) : IContentCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly ILogger<InMemoryContentCacheService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly CacheOptions _options = options?.Value ?? new CacheOptions();

    // The cache is shared with the host, so the keys this service wrote are tracked per IMemoryCache instance.
    // That lets RemoveByPrefixAsync and ClearAsync evict only this library's entries.
    private static readonly ConditionalWeakTable<IMemoryCache, ConcurrentDictionary<string, byte>> TrackedKeysByCache = new();

    private ConcurrentDictionary<string, byte> TrackedKeys => TrackedKeysByCache.GetOrCreateValue(_memoryCache);

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }
        return default;
    }

    /// <inheritdoc />
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

        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            _logger.LogDebug("Cache hit for {Key}", key);
            return cachedValue;
        }

        _logger.LogDebug("Cache miss for {Key}", key);

        try
        {
            var result = await factory(cancellationToken);

            if (result is not null)
            {
                var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.MaxAgeMinutes);

                _memoryCache.Set(key, result, CreateEntryOptions(key, cacheExpiration));
                _logger.LogDebug("Added {Key} to cache", key);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cached item for {Key}", key);
            throw;
        }
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        TrackedKeys.TryRemove(key, out _);
        _logger.LogDebug("Removed {Key} from cache", key);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        var removed = 0;
        foreach (var key in TrackedKeys.Keys)
        {
            if (key.StartsWith(prefix, StringComparison.Ordinal))
            {
                _memoryCache.Remove(key);
                TrackedKeys.TryRemove(key, out _);
                removed++;
            }
        }

        _logger.LogDebug("Removed {Count} items with prefix {Prefix} from cache", removed, prefix);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    /// <remarks>Removes only the entries written through this service, never the host's other cache entries.</remarks>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        foreach (var key in TrackedKeys.Keys)
        {
            _memoryCache.Remove(key);
            TrackedKeys.TryRemove(key, out _);
        }

        _logger.LogInformation("Cleared entries written by the content cache service");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || value is null)
        {
            return;
        }

        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.MaxAgeMinutes);

        _memoryCache.Set(key, value, CreateEntryOptions(key, cacheExpiration));
        _logger.LogDebug("Set {Key} in cache", key);
    }

    private MemoryCacheEntryOptions CreateEntryOptions(string key, TimeSpan expiration)
    {
        var trackedKeys = TrackedKeys;
        trackedKeys[key] = 0;

        return new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(expiration)
            .SetSize(1)
            .RegisterPostEvictionCallback(static (evictedKey, value, reason, state) =>
            {
                if (reason != EvictionReason.Replaced && evictedKey is string text && state is ConcurrentDictionary<string, byte> keys)
                {
                    keys.TryRemove(text, out byte _);
                }
            }, trackedKeys);
    }
}
