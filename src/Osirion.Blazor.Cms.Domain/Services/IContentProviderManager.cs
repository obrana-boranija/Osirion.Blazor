using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Domain.Services;

/// <summary>Coordinates access to registered content providers.</summary>
public interface IContentProviderManager
{
    /// <summary>Gets all registered providers.</summary>
    IEnumerable<IContentProvider> GetAllProviders();
    /// <summary>Gets a provider by identifier.</summary>
    IContentProvider? GetProvider(string providerId);
    /// <summary>Gets the default provider.</summary>
    IContentProvider? GetDefaultProvider();

    /// <summary>Gets the directory tree from the default provider.</summary>
    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default);
    /// <summary>Gets the directory tree from the configured default provider.</summary>
    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeFromDefaultAsync(string? locale = null, CancellationToken cancellationToken = default);
    /// <summary>Gets the directory tree from a specified provider.</summary>
    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeFromProviderAsync(string providerId, string? locale = null, CancellationToken cancellationToken = default);

    /// <summary>Gets content for a locale.</summary>
    Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default);
    /// <summary>Gets content matching a query.</summary>
    Task<IReadOnlyList<ContentItem>> GetContentByQueryAsync(ContentQuery query, CancellationToken cancellationToken = default);
    /// <summary>Gets content matching a query from the default provider.</summary>
    Task<IReadOnlyList<ContentItem>> GetContentByQueryFromDefaultAsync(ContentQuery query, CancellationToken cancellationToken = default);
    /// <summary>Gets content matching a query from a specified provider.</summary>
    Task<IReadOnlyList<ContentItem>> GetContentByQueryFromProviderAsync(string providerId, ContentQuery query, CancellationToken cancellationToken = default);
    /// <summary>Gets localized content.</summary>
    Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default);
    /// <summary>Gets localized content from a specified provider.</summary>
    Task<ContentItem?> GetLocalizedContentFromProviderAsync(string providerId, string localizationId, string locale, CancellationToken cancellationToken = default);
}
