namespace Osirion.Blazor.Cms.Domain.Options;

/// <summary>
/// Base options for content providers
/// </summary>
public class ContentProviderOptions
{
    /// <summary>
    /// Gets or sets the unique identifier for the provider
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets whether this is the default provider
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets the content path within the repository
    /// </summary>
    public string ContentPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether localization is enabled
    /// </summary>
    public bool EnableLocalization { get; set; } = false;

    /// <summary>
    /// Gets or sets the default locale to use when localization is disabled
    /// </summary>
    public string DefaultLocale { get; set; } = "en";

    private List<string> _supportedLocales = new();
    private bool _supportedLocalesAssigned;

    /// <summary>
    /// Gets or sets the supported locales: the folder names that count as locales. Empty until configured, so a
    /// configured list (for example <c>["en", "sr"]</c> in appsettings) is exactly the list, not appended to a default.
    /// Read the list a provider uses with <see cref="GetEffectiveSupportedLocales"/>.
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
    /// Gets the locales the provider uses: <see cref="SupportedLocales"/>, or <c>en</c> when none was configured.
    /// A list assigned as empty stays empty, which accepts any language code shaped folder name as a locale.
    /// </summary>
    /// <returns>The locales to use.</returns>
    public List<string> GetEffectiveSupportedLocales() =>
        _supportedLocalesAssigned || _supportedLocales.Count > 0 ? _supportedLocales : new List<string> { "en" };

    /// <summary>
    /// Gets or sets the supported file extensions
    /// </summary>
    public List<string> SupportedExtensions { get; set; } = new() { ".md", ".markdown" };

    /// <summary>
    /// Gets or sets whether to validate content on write operations
    /// </summary>
    public bool ValidateContent { get; set; } = true;
}
