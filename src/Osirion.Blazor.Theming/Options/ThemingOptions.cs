namespace Osirion.Blazor.Theming.Options;

/// <summary>
/// Configuration options for theming
/// </summary>
public class ThemingOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Theming";

    /// <summary>
    /// Gets or sets whether to use default styles
    /// </summary>
    public bool UseDefaultStyles { get; set; } = true;

    /// <summary>
    /// Gets or sets the CSS framework to integrate with
    /// </summary>
    public CssFramework Framework { get; set; } = CssFramework.None;

    /// <summary>
    /// Gets or sets the default theme mode
    /// </summary>
    public ThemeMode DefaultMode { get; set; } = ThemeMode.Light;

    /// <summary>
    /// Gets or sets whether dark mode is enabled
    /// </summary>
    public bool EnableDarkMode { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to follow system theme preference
    /// </summary>
    public bool FollowSystemPreference { get; set; } = false;

    /// <summary>
    /// Gets or sets custom CSS variables
    /// </summary>
    public string? CustomVariables { get; set; }
}