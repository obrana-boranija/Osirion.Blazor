using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Domain.Services;

public interface IContentProviderManager
{
    IEnumerable<IContentProvider> GetAllProviders();
    IContentProvider? GetProvider(string providerId);
    IContentProvider? GetDefaultProvider();

    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeFromDefaultAsync(string? locale = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeFromProviderAsync(string providerId, string? locale = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentItem>> GetContentByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentItem>> GetContentByQueryFromDefaultAsync(ContentQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentItem>> GetContentByQueryFromProviderAsync(string providerId, ContentQuery query, CancellationToken cancellationToken = default);
    Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default);
    Task<ContentItem?> GetLocalizedContentFromProviderAsync(string providerId, string localizationId, string locale, CancellationToken cancellationToken = default);
}