namespace Osirion.Blazor.Cms.Domain.Models;

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
    public List<LocaleInfo> AvailableLocales { get; set; } = new();

    /// <summary>
    /// Gets or sets the translations map (localization ID to paths in different locales)
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> Translations { get; set; } = new();

    /// <summary>
    /// Gets the best matching locale for the given requested locale
    /// </summary>
    /// <param name="requestedLocale">The requested locale (e.g., en-US)</param>
    /// <returns>The best matching locale from available locales, or default locale if no match</returns>
    public string GetBestMatchingLocale(string requestedLocale)
    {
        requestedLocale.ToLower();
        if (string.IsNullOrEmpty(requestedLocale))
            return DefaultLocale;

        // Exact match
        if (AvailableLocales.Any(l => l.Code.Equals(requestedLocale, StringComparison.OrdinalIgnoreCase)))
            return requestedLocale;

        // Language match (en-US matches en)
        var languagePart = requestedLocale.Split('-')[0];
        var matchingLocale = AvailableLocales.FirstOrDefault(l =>
            l.Code.Equals(languagePart, StringComparison.OrdinalIgnoreCase) ||
            l.Code.StartsWith(languagePart + "-", StringComparison.OrdinalIgnoreCase));

        return matchingLocale?.Code ?? DefaultLocale;
    }

    /// <summary>
    /// Adds a locale to the available locales list if it doesn't exist
    /// </summary>
    public void AddLocale(string code, string name, string? nativeName = null)
    {
        if (!AvailableLocales.Any(l => l.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            AvailableLocales.Add(new LocaleInfo
            {
                Code = code,
                Name = name,
                NativeName = nativeName ?? name
            });
        }
    }
}

/// <summary>
/// Represents information about a locale
/// </summary>
public class LocaleInfo
{
    /// <summary>
    /// Gets or sets the locale code (e.g., en, en-US, fr-FR)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale name in English (e.g., English, French)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the locale name in its native language (e.g., English, Français)
    /// </summary>
    public string NativeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional flag emoji or icon for the locale
    /// </summary>
    public string? Flag { get; set; }

    /// <summary>
    /// Gets or sets the text direction for the locale (LTR or RTL)
    /// </summary>
    public string Direction { get; set; } = "ltr";

    /// <summary>
    /// Gets or sets whether this locale is the default
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets any additional properties for the locale
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = new();
}