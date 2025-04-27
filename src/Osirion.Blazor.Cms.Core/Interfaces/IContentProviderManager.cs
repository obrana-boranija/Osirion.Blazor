using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Interfaces;
using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Core.Interfaces
{
    public interface IContentProviderManager
    {
        IEnumerable<IContentProvider> GetAllProviders();
        Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default);
        IContentProvider? GetDefaultProvider();
        Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default);
        Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default);
        Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default);
        IContentProvider? GetProvider(string providerId);
    }
}