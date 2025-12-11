using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;

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

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        _logger.LogDebug("Removed {Key} from cache", key);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        if (_memoryCache is MemoryCache cache)
        {
            cache.Compact(1.0);
            _logger.LogInformation("Cleared entire cache");
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || value is null)
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