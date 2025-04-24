using Osirion.Blazor.Cms;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;

public class ContentProviderManager : IContentProviderManager
{
    private readonly IEnumerable<IContentProvider> _providers;

    public ContentProviderManager(IEnumerable<IContentProvider> providers)
    {
        _providers = providers ?? throw new ArgumentNullException(nameof(providers));
    }

    public IContentProvider? GetDefaultProvider() => _providers.FirstOrDefault();

    public IContentProvider? GetProvider(string providerId) =>
        _providers.FirstOrDefault(p => p.ProviderId == providerId);

    public IEnumerable<IContentProvider> GetAllProviders() => _providers;

    public async Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        return provider != null
            ? await provider.GetLocalizationInfoAsync(cancellationToken)
            : new LocalizationInfo();
    }

    public async Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        return provider != null
            ? await provider.GetDirectoriesAsync(locale, cancellationToken)
            : Array.Empty<DirectoryItem>().ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider == null)
            return Array.Empty<ContentItem>().ToList().AsReadOnly();

        var query = new ContentQuery { Locale = locale };
        return await provider.GetItemsByQueryAsync(query, cancellationToken);
    }

    public async Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default)
    {
        var provider = GetDefaultProvider();
        if (provider == null)
            return null;

        var translations = await provider.GetContentTranslationsAsync(localizationId, cancellationToken);
        return translations.FirstOrDefault(t => t.Locale.Equals(locale, StringComparison.OrdinalIgnoreCase));
    }
}