using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

public class ContentProviderManager : IContentProviderManager
{
    private readonly IContentProviderRegistry _registry;
    private readonly ILogger<ContentProviderManager> _logger;

    public ContentProviderManager(
        IContentProviderRegistry registry,
        ILogger<ContentProviderManager> logger)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IContentProvider? GetDefaultProvider() => _registry.GetDefaultProvider();
    public IContentProvider? GetProvider(string providerId) => _registry.GetProvider(providerId);
    public IEnumerable<IContentProvider> GetAllProviders() => _registry.GetAllProviders();

    #region Directory Methods

    /// <summary>
    /// Gets directory tree from all providers
    /// </summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(
        string? locale = null,
        CancellationToken cancellationToken = default)
    {
        return await GetDirectoryTreeFromProvidersAsync(GetAllProviders(), locale, cancellationToken);
    }

    /// <summary>
    /// Gets directory tree from default provider only
    /// </summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeFromDefaultAsync(
        string? locale = null,
        CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider is null)
        {
            _logger.LogWarning("No default provider available for GetDirectoryTreeFromDefaultAsync");
            return Array.Empty<DirectoryItem>();
        }

        return await GetDirectoryTreeFromProvidersAsync(new[] { provider }, locale, cancellationToken);
    }

    /// <summary>
    /// Gets directory tree from a specific provider
    /// </summary>
    public async Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeFromProviderAsync(
        string providerId,
        string? locale = null,
        CancellationToken cancellationToken = default)
    {
        var provider = GetProvider(providerId);
        if (provider is null)
        {
            _logger.LogWarning("Provider {ProviderId} not found for GetDirectoryTreeFromProviderAsync", providerId);
            return Array.Empty<DirectoryItem>();
        }

        return await GetDirectoryTreeFromProvidersAsync(new[] { provider }, locale, cancellationToken);
    }

    #endregion

    #region Content Query Methods

    /// <summary>
    /// Gets content by query from all providers
    /// </summary>
    public async Task<IReadOnlyList<ContentItem>> GetContentByQueryAsync(
        ContentQuery query,
        CancellationToken cancellationToken = default)
    {
        return await GetContentFromProvidersAsync(GetAllProviders(), query, cancellationToken);
    }

    /// <summary>
    /// Gets content by query from default provider only
    /// </summary>
    public async Task<IReadOnlyList<ContentItem>> GetContentByQueryFromDefaultAsync(
        ContentQuery query,
        CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider is null)
        {
            _logger.LogWarning("No default provider available for GetContentByQueryFromDefaultAsync");
            return Array.Empty<ContentItem>();
        }

        return await GetContentFromProvidersAsync(new[] { provider }, query, cancellationToken);
    }

    /// <summary>
    /// Gets content by query from a specific provider
    /// </summary>
    public async Task<IReadOnlyList<ContentItem>> GetContentByQueryFromProviderAsync(
        string providerId,
        ContentQuery query,
        CancellationToken cancellationToken = default)
    {
        var provider = GetProvider(providerId);
        if (provider is null)
        {
            _logger.LogWarning("Provider {ProviderId} not found for GetContentByQueryFromProviderAsync", providerId);
            return Array.Empty<ContentItem>();
        }

        return await GetContentFromProvidersAsync(new[] { provider }, query, cancellationToken);
    }

    /// <summary>
    /// Gets content by locale - backward compatibility method using default provider
    /// </summary>
    public async Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(
        string locale,
        CancellationToken cancellationToken = default)
    {
        var query = new ContentQuery { Locale = locale };
        return await GetContentByQueryFromDefaultAsync(query, cancellationToken);
    }

    #endregion

    #region Localized Content Methods

    /// <summary>
    /// Gets localized content from default provider with fallback support
    /// </summary>
    public async Task<ContentItem?> GetLocalizedContentAsync(
        string localizationId,
        string locale,
        CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider is null)
        {
            _logger.LogWarning("No default provider available for GetLocalizedContentAsync");
            return null;
        }

        return await GetLocalizedContentFromProviderAsync(provider, localizationId, locale, cancellationToken);
    }

    /// <summary>
    /// Gets localized content from a specific provider with fallback support
    /// </summary>
    public async Task<ContentItem?> GetLocalizedContentFromProviderAsync(
        string providerId,
        string localizationId,
        string locale,
        CancellationToken cancellationToken = default)
    {
        var provider = GetProvider(providerId);
        if (provider is null)
        {
            _logger.LogWarning("Provider {ProviderId} not found for GetLocalizedContentFromProviderAsync", providerId);
            return null;
        }

        return await GetLocalizedContentFromProviderAsync(provider, localizationId, locale, cancellationToken);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Core method to get directories from multiple providers with error handling
    /// </summary>
    private async Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeFromProvidersAsync(
        IEnumerable<IContentProvider> providers,
        string? locale,
        CancellationToken cancellationToken)
    {
        if (!providers.Any())
        {
            _logger.LogWarning("No providers available for directory tree retrieval");
            return Array.Empty<DirectoryItem>();
        }

        var items = new List<DirectoryItem>();

        foreach (var provider in providers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var providerItems = await provider.GetDirectoriesAsync(locale, cancellationToken);
                if (providerItems?.Any() == true)
                {
                    items.AddRange(providerItems);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow cancellation
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error getting directories from provider {Provider} with locale {Locale}",
                    provider.GetType().Name, locale);
            }
        }

        return items;
    }

    /// <summary>
    /// Core method to get content from multiple providers with error handling
    /// </summary>
    private async Task<IReadOnlyList<ContentItem>> GetContentFromProvidersAsync(
        IEnumerable<IContentProvider> providers,
        ContentQuery query,
        CancellationToken cancellationToken)
    {
        if (!providers.Any())
        {
            _logger.LogWarning("No providers available for content retrieval");
            return Array.Empty<ContentItem>();
        }

        var items = new List<ContentItem>();

        foreach (var provider in providers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var providerItems = await provider.GetItemsByQueryAsync(query, cancellationToken);
                if (providerItems?.Any() == true)
                {
                    items.AddRange(providerItems);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Rethrow cancellation
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error getting content from provider {Provider} with query {@Query}",
                    provider.GetType().Name, query);
            }
        }

        // Apply sorting if we combined results from multiple providers
        if (providers.Count() > 1 && items.Count > 1)
        {
            items = ApplySorting(items, query.SortBy, query.SortDirection).ToList();
        }

        // Apply pagination if specified and we're combining multiple providers
        if (providers.Count() > 1)
        {
            if (query.Skip.HasValue)
            {
                items = items.Skip(query.Skip.Value).ToList();
            }
            if (query.Take.HasValue)
            {
                items = items.Take(query.Take.Value).ToList();
            }
        }

        return items;
    }

    /// <summary>
    /// Gets localized content with fallback logic
    /// </summary>
    private async Task<ContentItem?> GetLocalizedContentFromProviderAsync(
        IContentProvider provider,
        string localizationId,
        string locale,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get all translations for the content item
            var query = new ContentQuery
            {
                LocalizationId = localizationId,
                IncludeUnpublished = false
            };

            var allTranslations = await provider.GetItemsByQueryAsync(query, cancellationToken);
            if (allTranslations?.Any() != true)
            {
                _logger.LogDebug("No translations found for localization ID {LocalizationId}", localizationId);
                return null;
            }

            // Find the requested locale
            var localizedContent = allTranslations.FirstOrDefault(c =>
                string.Equals(c.Locale, locale, StringComparison.OrdinalIgnoreCase));

            if (localizedContent is not null)
            {
                return localizedContent;
            }

            // Fallback to default locale if configured
            var defaultLocale = provider is IContentLocalization locProvider
                ? locProvider.DefaultLocale
                : "en";

            _logger.LogDebug(
                "Locale {Locale} not found for {LocalizationId}, falling back to {DefaultLocale}",
                locale, localizationId, defaultLocale);

            return allTranslations.FirstOrDefault(c =>
                string.Equals(c.Locale, defaultLocale, StringComparison.OrdinalIgnoreCase));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error getting localized content: {LocalizationId}, {Locale} from provider {Provider}",
                localizationId, locale, provider.GetType().Name);
            return null;
        }
    }

    /// <summary>
    /// Applies sorting to a collection of content items
    /// </summary>
    private static IEnumerable<ContentItem> ApplySorting(
    IEnumerable<ContentItem> items,
    SortField sortBy,
    SortDirection direction)
    {
        var ordered = sortBy switch
        {
            SortField.Title => direction == SortDirection.Ascending
                ? items.OrderBy(x => x.Title)
                : items.OrderByDescending(x => x.Title),
            SortField.Date => direction == SortDirection.Ascending
                ? items.OrderBy(x => x.PublishDate)
                : items.OrderByDescending(x => x.PublishDate),
            SortField.Author => direction == SortDirection.Ascending
                ? items.OrderBy(x => x.Author)
                : items.OrderByDescending(x => x.Author),
            _ => items
        };

        return ordered;
    }

    #endregion
}