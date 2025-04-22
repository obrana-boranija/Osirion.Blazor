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
}

/// <summary>
/// Event arguments for theme changes
/// </summary>
public class ThemeChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the new theme mode
    /// </summary>
    public ThemeMode NewMode { get; }

    /// <summary>
    /// Gets the previous theme mode
    /// </summary>
    public ThemeMode PreviousMode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeChangedEventArgs"/> class.
    /// </summary>
    public ThemeChangedEventArgs(ThemeMode newMode, ThemeMode previousMode)
    {
        NewMode = newMode;
        PreviousMode = previousMode;
    }
}