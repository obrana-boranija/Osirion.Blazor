using Osirion.Blazor.Components;

namespace Osirion.Blazor.Theming.Services;

/// <summary>
/// Service for managing themes
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets the current CSS framework
    /// </summary>
    CssFramework CurrentFramework { get; }

    /// <summary>
    /// Gets or sets the current theme mode
    /// </summary>
    ThemeMode CurrentMode { get; set; }

    /// <summary>
    /// Event raised when theme changes
    /// </summary>
    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>
    /// Generates CSS variables for the current theme
    /// </summary>
    /// <returns>The generated CSS variables</returns>
    string GenerateThemeVariables();

    /// <summary>
    /// Sets the theme mode
    /// </summary>
    /// <param name="mode">The theme mode to set</param>
    void SetThemeMode(ThemeMode mode);

    /// <summary>
    /// Gets the CSS class name for the current framework
    /// </summary>
    /// <returns>CSS class for the current framework</returns>
    string GetFrameworkClass();

    /// <summary>
    /// Toggles between light and dark theme and returns the theme in effect afterwards ("light" or "dark").
    /// </summary>
    /// <returns>The theme in effect after the toggle; unchanged when dark mode is disabled</returns>
    string ToggleTheme();

    /// <summary>
    /// Gets the palette in effect for this request: the visitor's choice from the <c>osirion-preferred-palette</c>
    /// cookie, else <see cref="ThemingOptions.DefaultPalette"/>. Implementations written before palettes existed
    /// report <see cref="ThemePalette.Framework"/>.
    /// </summary>
    ThemePalette CurrentPalette => ThemePalette.Framework;

    /// <summary>
    /// Sets the visitor's palette and keeps it in the <c>osirion-preferred-palette</c> cookie when a response can
    /// still carry one. Implementations written before palettes existed ignore it.
    /// </summary>
    /// <param name="palette">The palette to use</param>
    void SetPalette(ThemePalette palette)
    {
    }
}
