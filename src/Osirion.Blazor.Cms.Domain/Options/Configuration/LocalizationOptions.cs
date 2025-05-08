namespace Osirion.Blazor.Cms.Domain.Options.Configuration;

/// <summary>
/// Configuration options for localization
/// </summary>
public class LocalizationOptions
{
    /// <summary>
    /// Gets or sets the supported cultures
    /// </summary>
    public List<string> SupportedCultures { get; set; } = new() { "en-US" };

    /// <summary>
    /// Gets or sets the default culture
    /// </summary>
    public string DefaultCulture { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets whether to enable culture fallback
    /// </summary>
    public bool EnableCultureFallback { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the culture selector
    /// </summary>
    public bool ShowCultureSelector { get; set; } = true;

    /// <summary>
    /// Gets or sets the culture display format (e.g., "NativeName", "EnglishName", "Name")
    /// </summary>
    public string CultureDisplayFormat { get; set; } = "NativeName";

    /// <summary>
    /// Gets or sets whether to require culture prefix in URLs
    /// </summary>
    public bool RequireCulturePrefix { get; set; } = false;

    /// <summary>
    /// Adds supported cultures
    /// </summary>
    /// <param name="cultures">The cultures to add</param>
    public void AddSupportedCultures(params string[] cultures)
    {
        foreach (var culture in cultures)
        {
            if (!SupportedCultures.Contains(culture))
            {
                SupportedCultures.Add(culture);
            }
        }
    }

    /// <summary>
    /// Sets the default culture
    /// </summary>
    /// <param name="culture">The default culture</param>
    public void SetDefaultCulture(string culture)
    {
        if (SupportedCultures.Contains(culture) || culture == "en-US")
        {
            DefaultCulture = culture;

            // Ensure the default culture is in the supported cultures list
            if (!SupportedCultures.Contains(culture))
            {
                SupportedCultures.Add(culture);
            }
        }
    }
}