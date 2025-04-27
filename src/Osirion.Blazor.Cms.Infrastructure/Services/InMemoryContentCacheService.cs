using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;

public class InMemoryContentCacheService : IContentCacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InMemoryContentCacheService> _logger;
    private readonly CacheOptions _options;

    public InMemoryContentCacheService(
        IMemoryCache memoryCache,
        ILogger<InMemoryContentCacheService> logger,
        IOptions<CacheOptions> options)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? new CacheOptions();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_memoryCache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }
        return default;
    }

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

            if (result != null)
            {
                var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.MaxAgeMinutes);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(cacheExpiration)
                    .SetSize(1); // Set size for memory limit

                _memoryCache.Set(key, result, cacheEntryOptions);
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

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        _logger.LogDebug("Removed {Key} from cache", key);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Unfortunately, MemoryCache doesn't support prefix-based removal directly
        // We'll use Compact with a high percentage to clear most of the cache
        if (_memoryCache is MemoryCache cache)
        {
            cache.Compact(_options.CompactionPercentage);
            _logger.LogDebug("Removed items with prefix {Prefix} from cache", prefix);
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        if (_memoryCache is MemoryCache cache)
        {
            cache.Compact(1.0);
            _logger.LogInformation("Cleared entire cache");
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || value == null)
        {
            return;
        }

        var cacheExpiration = expiration ?? TimeSpan.FromMinutes(_options.MaxAgeMinutes);

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(cacheExpiration)
            .SetSize(1); // Set size for memory limit

        _memoryCache.Set(key, value, cacheEntryOptions);
        _logger.LogDebug("Set {Key} in cache", key);
    }
}