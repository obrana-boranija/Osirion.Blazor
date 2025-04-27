using Osirion.Blazor.Cms.Core.Interfaces;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Core.Services;

/// <summary>
/// Implementation of IContentProviderManager that manages access to content providers
/// </summary>
public class ContentProviderManager : IContentProviderManager
{
    private readonly IEnumerable<IContentProvider> _providers;
    private readonly IContentProviderFactory? _providerFactory;

    /// <summary>
    /// Initializes a new instance of the ContentProviderManager class
    /// </summary>
    public ContentProviderManager(
        IEnumerable<IContentProvider> providers,
        IContentProviderFactory? providerFactory = null)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        _providerFactory = providerFactory;
    }

    /// <inheritdoc/>
    public IContentProvider? GetDefaultProvider()
    {
        // If we have a factory, try to get the default provider from it
        if (_providerFactory != null)
        {
            var defaultProviderId = _providerFactory.GetDefaultProviderId();
            if (!string.IsNullOrEmpty(defaultProviderId))
            {
                var defaultProvider = _providers.FirstOrDefault(p => p.ProviderId == defaultProviderId);
                if (defaultProvider != null)
                {
                    return defaultProvider;
                }
            }
        }

        // Fall back to first registered provider
        return _providers.FirstOrDefault();
    }

    /// <inheritdoc/>
    public IContentProvider? GetProvider(string providerId)
    {
        if (string.IsNullOrEmpty(providerId))
            throw new ArgumentException("Provider ID cannot be null or empty", nameof(providerId));

        return _providers.FirstOrDefault(p => p.ProviderId == providerId);
    }

    /// <inheritdoc/>
    public IEnumerable<IContentProvider> GetAllProviders() => _providers;

    /// <inheritdoc/>
    public async Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        return provider != null
            ? await provider.GetLocalizationInfoAsync(cancellationToken)
            : new LocalizationInfo();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        return provider != null
            ? await provider.GetDirectoriesAsync(locale, cancellationToken)
            : Array.Empty<DirectoryItem>().ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider == null)
            return Array.Empty<ContentItem>().ToList().AsReadOnly();

        var query = new ContentQuery { Locale = locale };
        return await provider.GetItemsByQueryAsync(query, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider == null)
            return null;

        var translations = await provider.GetContentTranslationsAsync(localizationId, cancellationToken);
        return translations.FirstOrDefault(t => t.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase));
    }
}