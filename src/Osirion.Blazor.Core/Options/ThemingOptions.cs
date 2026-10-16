namespace Osirion.Blazor.Components;

/// <summary>
/// Configuration options for theming
/// </summary>
public class ThemingOptions
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string Section = "Osirion:Theming";

    /// <summary>
    /// Gets or sets whether to use default styles
    /// </summary>
    public bool UseDefaultStyles { get; set; } = false;

    /// <summary>
    /// Gets or sets the CSS framework to integrate with
    /// </summary>
    public CssFramework Framework { get; set; } = CssFramework.None;

    /// <summary>
    /// Gets or sets the default theme mode. Defaults to System to follow user's OS preference.
    /// </summary>
    public ThemeMode DefaultMode { get; set; } = ThemeMode.System;

    /// <summary>
    /// Gets or sets whether dark mode is enabled
    /// </summary>
    public bool EnableDarkMode { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to follow system theme preference
    /// </summary>
    public bool FollowSystemPreference { get; set; } = true;

    /// <summary>
    /// Gets or sets custom CSS variables
    /// </summary>
    public string? CustomVariables { get; set; }

    /// <summary>
    /// Gets or sets the palette a visitor sees until they pick another one. <see cref="ThemePalette.Framework"/>, the
    /// default, applies no palette, so the site looks as it did before palettes existed.
    /// </summary>
    public ThemePalette DefaultPalette { get; set; } = ThemePalette.Framework;

    /// <summary>
    /// Gets or sets whether visitors can pick a palette (with <c>ThemePicker</c>). When it is set, every page links the
    /// palette stylesheet, so a palette chosen on one page is painted on the next without a flash, also on a site
    /// exported to static files. Without it the stylesheet is linked only while a palette other than
    /// <see cref="ThemePalette.Framework"/> is in use.
    /// </summary>
    public bool AllowPaletteSelection { get; set; }
}
