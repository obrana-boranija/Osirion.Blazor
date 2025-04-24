using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Interfaces;

public interface IContentProviderManager
{
    IContentProvider? GetDefaultProvider();
    IContentProvider? GetProvider(string providerId);
    IEnumerable<IContentProvider> GetAllProviders();

    Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default);
    Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default);
}