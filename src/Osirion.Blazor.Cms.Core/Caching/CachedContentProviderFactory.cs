using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Configuration;
using Osirion.Blazor.Cms.Interfaces;

namespace Osirion.Blazor.Cms.Core.Caching;

/// <summary>
/// Factory for creating cached content providers
/// </summary>
public class CachedContentProviderFactory
{
    private readonly IContentCacheService _cacheService;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ContentCacheOptions _cacheOptions;

    /// <summary>
    /// Initializes a new instance of the CachedContentProviderFactory class
    /// </summary>
    public CachedContentProviderFactory(
        IContentCacheService cacheService,
        ILoggerFactory loggerFactory,
        IOptions<ContentCacheOptions> cacheOptions)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _cacheOptions = cacheOptions?.Value ?? new ContentCacheOptions();
    }

    /// <summary>
    /// Creates a cached decorator around a content provider
    /// </summary>
    public IContentProvider CreateCachedProvider(IContentProvider provider)
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
            _cacheService,
            logger,
            TimeSpan.FromMinutes(_cacheOptions.DefaultDurationMinutes),
            true);
    }
}