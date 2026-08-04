namespace Osirion.Blazor.Components;

/// <summary>Configures localization defaults and supported locales.</summary>
public class LocalizationOptions
{
    /// <summary>
    /// Gets or sets the default locale to use when localization is disabled
    /// </summary>
    public string DefaultLocale { get; set; } = "en";

    /// <summary>
    /// Gets or sets the supported locales
    /// </summary>
    public List<string> SupportedLocales { get; set; } = new() { "en" };
}
