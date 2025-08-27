namespace Osirion.Blazor.Components;

/// <summary>
/// Theme mode options
/// </summary>
public enum ThemeMode
{
    /// <summary>
    /// Light theme
    /// </summary>
    Light,

    /// <summary>
    /// Dark theme
    /// </summary>
    Dark,

    /// <summary>
    /// Follow system preference
    /// </summary>
    System,

    /// <summary>
    /// Inverted theme (dark if system is light, light if system is dark)
    /// </summary>
    Inverted
}