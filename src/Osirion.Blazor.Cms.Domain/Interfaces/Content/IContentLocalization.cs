using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Domain.Interfaces.Content;

/// <summary>
/// Interface for localization-related operations on content
/// </summary>
public interface IContentLocalization
{
    /// <summary>
    /// Gets localization information for the provider
    /// </summary>
    //Task<LocalizationInfo> GetLocalizationInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets translations for a specific content item
    /// </summary>
    Task<IReadOnlyList<ContentItem>> GetContentTranslationsAsync(string localizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the default locale for the provider
    /// </summary>
    string DefaultLocale { get; }

    /// <summary>
    /// Gets the supported locales for the provider
    /// </summary>
    IReadOnlyList<string> SupportedLocales { get; }
}