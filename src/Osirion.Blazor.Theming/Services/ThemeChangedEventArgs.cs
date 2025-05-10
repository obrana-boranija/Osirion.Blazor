using Osirion.Blazor.Components;

namespace Osirion.Blazor.Theming.Services;

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
