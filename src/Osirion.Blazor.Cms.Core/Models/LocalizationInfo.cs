namespace Osirion.Blazor.Cms.Models;

/// <summary>
/// Represents localization information for content
/// </summary>
public class LocalizationInfo
{
    /// <summary>
    /// Gets or sets the default locale
    /// </summary>
    public string DefaultLocale { get; set; } = "en";

    /// <summary>
    /// Gets or sets the available locales
    /// </summary>
    public List<string> AvailableLocales { get; set; } = new();

    /// <summary>
    /// Gets or sets the translations map (localization ID to paths in different locales)
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> Translations { get; set; } = new();
}