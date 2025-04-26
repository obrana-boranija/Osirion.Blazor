namespace Osirion.Blazor.Cms.Options;

/// <summary>
/// Base options for content providers
/// </summary>
public abstract class ContentProviderOptions
{
    /// <summary>
    /// Gets or sets the unique identifier for the provider
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// Gets or sets whether caching is enabled
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the cache duration in minutes
    /// </summary>
    public int CacheDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets the supported file extensions
    /// </summary>
    public List<string> SupportedExtensions { get; set; } = new() { ".md", ".markdown" };

    /// <summary>
    /// Gets or sets whether the provider is the default provider
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets whether localization is enabled
    /// </summary>
    /// <remarks>
    /// When enabled, the provider will extract locale information from directory paths 
    /// and group content by localization ID. When disabled, all content is treated as
    /// if it belonged to the default locale.
    /// </remarks>
    public bool EnableLocalization { get; set; } = false;

    /// <summary>
    /// Gets or sets the default locale to use when localization is disabled
    /// </summary>
    public string DefaultLocale { get; set; } = "en";

    /// <summary>
    /// Gets or sets the supported locales
    /// </summary>
    public List<string> SupportedLocales { get; set; } = new() { "en" };

    /// <summary>
    /// Gets or sets the content root path (for URL generation)
    /// </summary>
    public string? ContentRoot { get; set; }

    /// <summary>
    /// Gets or sets whether to validate content on write operations
    /// </summary>
    public bool ValidateContent { get; set; } = true;
}