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
}