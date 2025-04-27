using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Services;

public interface IContentProviderManager
{
    IEnumerable<IContentProvider> GetAllProviders();
    Task<IReadOnlyList<Entities.ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default);
    IContentProvider? GetDefaultProvider();
    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default);
    //Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default);
    Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default);
    IContentProvider? GetProvider(string providerId);
}