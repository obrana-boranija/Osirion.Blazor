using Osirion.Blazor.Cms.Models;

namespace Osirion.Blazor.Cms.Interfaces;

public interface IContentProviderManager
{
    /// <summary>
    /// Gets the default content provider
    /// </summary>
    IContentProvider? GetDefaultProvider();

    /// <summary>
    /// Gets a specific content provider by ID
    /// </summary>
    IContentProvider? GetProvider(string providerId);

    /// <summary>
    /// Gets all registered content providers
    /// </summary>
    IEnumerable<IContentProvider> GetAllProviders();

    /// <summary>
    /// Gets localization information
    /// </summary>
    Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the directory tree, optionally filtered by locale
    /// </summary>
    Task<IReadOnlyList<DirectoryItem>> GetDirectoryTreeAsync(string? locale = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets content by locale
    /// </summary>
    Task<IReadOnlyList<ContentItem>> GetContentByLocaleAsync(string locale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific localized content by its localization ID and locale
    /// </summary>
    Task<ContentItem?> GetLocalizedContentAsync(string localizationId, string locale, CancellationToken cancellationToken = default);
}