using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Caching;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Core.Caching;

/// <summary>
/// Decorator for content providers that adds caching capabilities
/// </summary>
public class CachingContentProviderDecorator : IContentProvider
{
    private readonly IContentProvider _decoratedProvider;
    private readonly IContentCacheService _cacheService;
    private readonly ILogger<CachingContentProviderDecorator> _logger;
    private readonly TimeSpan _cacheDuration;
    private readonly bool _cacheEnabled;

    /// <summary>
    /// Initializes a new instance of the CachingContentProviderDecorator class
    /// </summary>
    public CachingContentProviderDecorator(
        IContentProvider decoratedProvider,
        IContentCacheService cacheService,
        ILogger<CachingContentProviderDecorator> logger,
        TimeSpan cacheDuration,
        bool cacheEnabled = true)
    {
        _decoratedProvider = decoratedProvider ?? throw new ArgumentNullException(nameof(decoratedProvider));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheDuration = cacheDuration;
        _cacheEnabled = cacheEnabled;
    }

    /// <inheritdoc/>
    public string ProviderId => _decoratedProvider.ProviderId;

    /// <inheritdoc/>
    public string DisplayName => _decoratedProvider.DisplayName;

    /// <inheritdoc/>
    public bool IsReadOnly => _decoratedProvider.IsReadOnly;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentCategory>?> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetCategoriesAsync(cancellationToken);

        var cacheKey = $"{ProviderId}:categories";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetCategoriesAsync(ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ContentItem?> GetItemByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetItemByIdAsync(id, cancellationToken);

        var cacheKey = $"{ProviderId}:item:id:{id}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetItemByIdAsync(id, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ContentItem?> GetItemByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetItemByPathAsync(path, cancellationToken);

        var cacheKey = $"{ProviderId}:item:path:{path}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetItemByPathAsync(path, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ContentItem?> GetItemByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetItemByUrlAsync(url, cancellationToken);

        var cacheKey = $"{ProviderId}:item:url:{url}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetItemByUrlAsync(url, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>?> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetAllItemsAsync(cancellationToken);

        var cacheKey = $"{ProviderId}:items:all";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetAllItemsAsync(ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>?> GetItemsByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetItemsByQueryAsync(query, cancellationToken);

        // Generate a cache key based on query parameters
        var queryCacheKey = GenerateQueryCacheKey(query);
        var cacheKey = $"{ProviderId}:items:query:{queryCacheKey}";

        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetItemsByQueryAsync(query, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentTag>?> GetTagsAsync(CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetTagsAsync(cancellationToken);

        var cacheKey = $"{ProviderId}:tags";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetTagsAsync(ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DirectoryItem>?> GetDirectoriesAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetDirectoriesAsync(locale, cancellationToken);

        var cacheKey = $"{ProviderId}:directories:{locale ?? "all"}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetDirectoriesAsync(locale, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DirectoryItem?> GetDirectoryByIdAsync(string id, string? locale = null, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetDirectoryByIdAsync(id, locale, cancellationToken);

        var cacheKey = $"{ProviderId}:directory:id:{id}:{locale ?? "all"}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetDirectoryByIdAsync(id, locale, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DirectoryItem?> GetDirectoryByPathAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetDirectoryByPathAsync(path, cancellationToken);

        var cacheKey = $"{ProviderId}:directory:path:{path}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetDirectoryByPathAsync(path, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DirectoryItem?> GetDirectoryByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetDirectoryByUrlAsync(url, cancellationToken);

        var cacheKey = $"{ProviderId}:directory:url:{url}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetDirectoryByUrlAsync(url, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<LocalizationInfo?> GetLocalizationInfoAsync(CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetLocalizationInfoAsync(cancellationToken);

        var cacheKey = $"{ProviderId}:localization:info";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetLocalizationInfoAsync(ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>?> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default)
    {
        if (!_cacheEnabled)
            return await _decoratedProvider.GetContentTranslationsAsync(localizationId, cancellationToken);

        var cacheKey = $"{ProviderId}:translations:{localizationId}";
        return await _cacheService.GetOrCreateAsync(
            cacheKey,
            async ct => await _decoratedProvider.GetContentTranslationsAsync(localizationId, ct),
            _cacheDuration,
            cancellationToken);
    }

    /// <inheritdoc/>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _decoratedProvider.InitializeAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        if (_cacheEnabled)
        {
            await _cacheService.RemoveByPrefixAsync(ProviderId + ":", cancellationToken);
        }

        await _decoratedProvider.RefreshCacheAsync(cancellationToken);
    }

    /// <summary>
    /// Generates a cache key for a content query
    /// </summary>
    private string GenerateQueryCacheKey(ContentQuery query)
    {
        var keyParts = new List<string>();

        if (!string.IsNullOrEmpty(query.Directory))
            keyParts.Add($"dir:{query.Directory}");

        if (!string.IsNullOrEmpty(query.DirectoryId))
            keyParts.Add($"dirid:{query.DirectoryId}");

        if (!string.IsNullOrEmpty(query.Category))
            keyParts.Add($"cat:{query.Category}");

        if (!string.IsNullOrEmpty(query.Tag))
            keyParts.Add($"tag:{query.Tag}");

        if (query.IsFeatured.HasValue)
            keyParts.Add($"feat:{query.IsFeatured.Value}");

        if (!string.IsNullOrEmpty(query.Author))
            keyParts.Add($"author:{query.Author}");

        if (query.Status.HasValue)
            keyParts.Add($"status:{query.Status.Value}");

        if (query.DateFrom.HasValue)
            keyParts.Add($"from:{query.DateFrom.Value:yyyyMMdd}");

        if (query.DateTo.HasValue)
            keyParts.Add($"to:{query.DateTo.Value:yyyyMMdd}");

        if (!string.IsNullOrEmpty(query.SearchQuery))
            keyParts.Add($"q:{query.SearchQuery}");

        if (!string.IsNullOrEmpty(query.Locale))
            keyParts.Add($"loc:{query.Locale}");

        if (!string.IsNullOrEmpty(query.LocalizationId))
            keyParts.Add($"locid:{query.LocalizationId}");

        keyParts.Add($"sort:{query.SortBy}:{query.SortDirection}");

        if (query.IncludeUnpublished)
            keyParts.Add("unpub:true");

        if (query.Skip.HasValue)
            keyParts.Add($"skip:{query.Skip.Value}");

        if (query.Take.HasValue)
            keyParts.Add($"take:{query.Take.Value}");

        return string.Join("|", keyParts);
    }
}