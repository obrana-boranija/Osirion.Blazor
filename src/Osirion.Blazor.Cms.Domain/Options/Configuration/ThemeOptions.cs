namespace Osirion.Blazor.Cms.Domain.Options.Configuration;

/// <summary>
/// Configuration options for UI theming
/// </summary>
public class ThemeOptions
{
    /// <summary>
    /// Gets or sets the default theme mode
    /// </summary>
    public string DefaultMode { get; set; } = "light";

    /// <summary>
    /// Gets or sets the primary color
    /// </summary>
    public string PrimaryColor { get; set; } = "#0366d6";

    /// <summary>
    /// Gets or sets whether to use dark mode
    /// </summary>
    public bool UseDarkMode { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to respect system theme preferences
    /// </summary>
    public bool RespectSystemPreferences { get; set; } = true;
}