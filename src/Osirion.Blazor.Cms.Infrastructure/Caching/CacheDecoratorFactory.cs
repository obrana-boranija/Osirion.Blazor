using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.Caching;

/// <summary>
/// Factory for creating cache decorators for repositories
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CacheDecoratorFactory"/> class
/// </remarks>
/// <param name="memoryCache">The memory cache instance</param>
/// <param name="loggerFactory">The logger factory</param>
/// <param name="cacheOptions">The cache configuration options</param>
/// <exception cref="ArgumentNullException">Thrown when required dependencies are null</exception>
public class CacheDecoratorFactory(
    IMemoryCache memoryCache,
    ILoggerFactory loggerFactory,
    IOptions<CacheOptions> cacheOptions)
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    private readonly CacheOptions _cacheOptions = cacheOptions?.Value ?? new CacheOptions();

    /// <summary>
    /// Creates a cache decorator for content repositories
    /// </summary>
    public IContentRepository CreateContentCacheDecorator(IContentRepository repository, string providerIdentifier)
    {
        // Skip decoration if caching is disabled
        if (!_cacheOptions.Enabled)
            return repository;

        var logger = _loggerFactory.CreateLogger<StaleWhileRevalidateCacheDecorator>();

        return new StaleWhileRevalidateCacheDecorator(
            repository,
            _memoryCache,
            logger,
            TimeSpan.FromMinutes(_cacheOptions.StaleTimeMinutes),
            TimeSpan.FromMinutes(_cacheOptions.MaxAgeMinutes),
            providerIdentifier);
    }

    /// <summary>
    /// Creates a cache decorator for directory repositories
    /// </summary>
    public IDirectoryRepository CreateDirectoryCacheDecorator(IDirectoryRepository repository, string providerIdentifier)
    {
        // Skip decoration if caching is disabled
        if (!_cacheOptions.Enabled)
            return repository;

        var logger = _loggerFactory.CreateLogger<StaleWhileRevalidateDirectoryCache>();

        return new StaleWhileRevalidateDirectoryCache(
            repository,
            _memoryCache,
            logger,
            TimeSpan.FromMinutes(_cacheOptions.StaleTimeMinutes),
            TimeSpan.FromMinutes(_cacheOptions.MaxAgeMinutes),
            providerIdentifier);
    }
}