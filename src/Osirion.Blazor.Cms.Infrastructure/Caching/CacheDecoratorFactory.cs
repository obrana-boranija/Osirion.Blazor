using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Infrastructure.Caching;

/// <summary>
/// Factory for creating cache decorators for repositories
/// </summary>
public class CacheDecoratorFactory
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILoggerFactory _loggerFactory;
    private readonly CacheOptions _cacheOptions;

    public CacheDecoratorFactory(
        IMemoryCache memoryCache,
        ILoggerFactory loggerFactory,
        IOptions<CacheOptions> cacheOptions)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _cacheOptions = cacheOptions?.Value ?? throw new ArgumentNullException(nameof(cacheOptions));
    }

    /// <summary>
    /// Creates a cache decorator for content repositories
    /// </summary>
    public IContentRepository CreateContentCacheDecorator(IContentRepository repository, string providerIdentifier)
    {
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