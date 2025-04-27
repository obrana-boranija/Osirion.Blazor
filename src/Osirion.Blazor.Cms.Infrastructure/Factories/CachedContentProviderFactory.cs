using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Decorators;

namespace Osirion.Blazor.Cms.Infrastructure.Factories;

/// <summary>
/// Factory for creating cached content providers
/// </summary>
public class CachedContentProviderFactory
{
    private readonly IContentCacheService _cacheService;
    private readonly ILoggerFactory _loggerFactory;
    private readonly CacheOptions _cacheOptions;

    /// <summary>
    /// Initializes a new instance of the CachedContentProviderFactory class
    /// </summary>
    public CachedContentProviderFactory(
        IContentCacheService cacheService,
        ILoggerFactory loggerFactory,
        IOptions<CacheOptions> cacheOptions)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _cacheOptions = cacheOptions?.Value ?? new CacheOptions();
    }

    /// <summary>
    /// Creates a cached decorator around a content provider
    /// </summary>
    public IReadContentProvider CreateCachedProvider(IReadContentProvider provider)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        // If caching is disabled globally, return the original provider
        if (!_cacheOptions.Enabled)
            return provider;

        // Create a logger for the decorator
        var logger = _loggerFactory.CreateLogger<CachingContentProviderDecorator>();

        // Create and return the decorator
        return new CachingContentProviderDecorator(
            provider,
            _cacheService);
    }
}