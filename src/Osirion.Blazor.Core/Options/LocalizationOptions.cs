namespace Osirion.Blazor.Components;

/// <summary>Configures localization defaults and supported locales.</summary>
public class LocalizationOptions
{
    private List<string> _supportedLocales = new();
    private bool _supportedLocalesAssigned;

    /// <summary>
    /// Gets or sets the default locale to use when localization is disabled
    /// </summary>
    public string DefaultLocale { get; set; } = "en";

    /// <summary>
    /// Gets or sets the supported locales. Empty until configured, so a configured list (for example
    /// <c>["en", "sr"]</c> in appsettings) is exactly the list, not appended to a default. Read the list to use with
    /// <see cref="GetEffectiveSupportedLocales"/>.
    /// </summary>
    public List<string> SupportedLocales
    {
        get => _supportedLocales;
        set
        {
            _supportedLocales = value ?? new List<string>();
            _supportedLocalesAssigned = true;
        }
    }

    /// <summary>
    /// Gets the locales to use: <see cref="SupportedLocales"/>, or <c>en</c> when none was configured. A list assigned
    /// as empty stays empty.
    /// </summary>
    /// <returns>The locales to use.</returns>
    public List<string> GetEffectiveSupportedLocales() =>
        _supportedLocalesAssigned || _supportedLocales.Count > 0 ? _supportedLocales : new List<string> { "en" };
}
