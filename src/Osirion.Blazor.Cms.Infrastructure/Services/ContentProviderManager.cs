using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Exceptions;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Infrastructure.Services;

public class ContentProviderManager : IContentProviderManager
{
    private readonly IEnumerable<IContentProvider> _providers;
    private readonly IContentProviderFactory _providerFactory;
    private readonly ILogger<ContentProviderManager> _logger;

    public ContentProviderManager(
        IEnumerable<IContentProvider> providers,
        IContentProviderFactory providerFactory,
        ILogger<ContentProviderManager> logger)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IContentProvider? GetDefaultProvider()
    {
        // Try to get from factory first
        if (_providerFactory != null)
        {
            var defaultProviderId = _providerFactory.GetDefaultProviderId();
            if (!string.IsNullOrEmpty(defaultProviderId))
            {
                var provider = _providers.FirstOrDefault(p => p.ProviderId == defaultProviderId);
                if (provider != null)
                {
                    return provider;
                }

                _logger.LogWarning("Default provider ID {ProviderId} configured but not found", defaultProviderId);
            }
        }

        // Fall back to first registered provider
        var firstProvider = _providers.FirstOrDefault();
        if (firstProvider == null)
        {
            _logger.LogWarning("No content providers registered");
        }

        return firstProvider;
    }

    public IContentProvider? GetProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be empty", nameof(providerId));

        var provider = _providers.FirstOrDefault(p => p.ProviderId == providerId);
        if (provider == null)
        {
            _logger.LogWarning("Provider not found: {ProviderId}", providerId);
        }

        return provider;
    }

    public IEnumerable<IContentProvider> GetAllProviders() => _providers;

    public async Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider == null)
        {
            _logger.LogWarning("No default provider available for GetDirectoryTreeAsync");
            return Array.Empty<DirectoryItem>();
        }

        try
        {
            return await provider.GetDirectoriesAsync(locale, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting directory tree with locale {Locale}", locale);
            throw new ContentProviderException("Failed to get directory tree", ex);
        }
    }

    public async Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider == null)
        {
            _logger.LogWarning("No default provider available for GetContentByLocaleAsync");
            return Array.Empty<ContentItem>();
        }

        try
        {
            var query = new ContentQuery { Locale = locale };
            return await provider.GetItemsByQueryAsync(query, cancellationToken) ?? Array.Empty<ContentItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content by locale {Locale}", locale);
            throw new ContentProviderException($"Failed to get content by locale {locale}", ex);
        }
    }

    public async Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider == null)
        {
            _logger.LogWarning("No default provider available for GetLocalizedContentAsync");
            return null;
        }

        try
        {
            // Get all translations for the content item
            var query = new ContentQuery
            {
                LocalizationId = localizationId,
                IncludeUnpublished = false
            };

            var allTranslations = await provider.GetItemsByQueryAsync(query, cancellationToken);
            if (allTranslations == null || !allTranslations.Any())
            {
                return null;
            }

            // Find the requested locale
            var localizedContent = allTranslations.FirstOrDefault(c =>
                c.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase));

            if (localizedContent != null)
            {
                return localizedContent;
            }

            // Fallback to default locale if configured
            var defaultLocale = provider is IContentLocalization locProvider
                ? locProvider.DefaultLocale
                : "en";

            return allTranslations.FirstOrDefault(c =>
                c.Locale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting localized content: {LocalizationId}, {Locale}",
                localizationId, locale);
            return null;
        }
    }
}