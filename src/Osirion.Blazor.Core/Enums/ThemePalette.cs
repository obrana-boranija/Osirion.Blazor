namespace Osirion.Blazor.Components;

/// <summary>
/// A named color palette. The palette sets the colors; <see cref="ThemeMode"/> still picks light or dark, and each
/// palette has a variant for both.
/// </summary>
public enum ThemePalette
{
    /// <summary>
    /// No palette: the light and dark theme files apply, with the active CSS framework adapter's mappings on top.
    /// This is the behavior without palettes, and the default.
    /// </summary>
    Framework,

    /// <summary>Osirion's own colors, kept even when a CSS framework adapter is active.</summary>
    Osirion,

    /// <summary>Inspired by GitHub's Primer colors.</summary>
    Commit,

    /// <summary>Inspired by Visual Studio Code's Light Modern and Dark Modern themes.</summary>
    Editor,

    /// <summary>Approximates Visual Studio's Light and Dark themes.</summary>
    Studio,

    /// <summary>Approximates Visual Studio's Blue theme: a light page in a dark blue frame. In dark mode it is <see cref="Studio"/>'s dark variant.</summary>
    StudioBlue,

    /// <summary>High contrast, inspired by Visual Studio Code's High Contrast themes; text reaches 7:1 (WCAG AAA).</summary>
    Contrast,
}

/// <summary>
/// Converts <see cref="ThemePalette"/> values to and from the id written in the <c>data-osirion-palette</c> attribute
/// and the <c>osirion-preferred-palette</c> cookie.
/// </summary>
public static class ThemePaletteNames
{
    /// <summary>The name of the cookie that keeps the visitor's palette.</summary>
    public const string CookieName = "osirion-preferred-palette";

    private static readonly Dictionary<ThemePalette, (string Id, string DisplayName)> Names = new()
    {
        [ThemePalette.Framework] = ("framework", "Framework"),
        [ThemePalette.Osirion] = ("osirion", "Osirion"),
        [ThemePalette.Commit] = ("commit", "Commit"),
        [ThemePalette.Editor] = ("editor", "Editor"),
        [ThemePalette.Studio] = ("studio", "Studio"),
        [ThemePalette.StudioBlue] = ("studio-blue", "Studio Blue"),
        [ThemePalette.Contrast] = ("contrast", "Contrast"),
    };

    /// <summary>Gets the palette's id, such as <c>studio-blue</c>.</summary>
    /// <param name="palette">The palette.</param>
    /// <returns>The lower-case id; <c>framework</c> for a value outside the enum.</returns>
    public static string ToId(this ThemePalette palette) =>
        Names.TryGetValue(palette, out var name) ? name.Id : Names[ThemePalette.Framework].Id;

    /// <summary>Gets the palette's name for people, such as <c>Studio Blue</c>.</summary>
    /// <param name="palette">The palette.</param>
    /// <returns>The display name; <c>Framework</c> for a value outside the enum.</returns>
    public static string ToDisplayName(this ThemePalette palette) =>
        Names.TryGetValue(palette, out var name) ? name.DisplayName : Names[ThemePalette.Framework].DisplayName;

    /// <summary>Reads a palette id, ignoring case. Anything else, including enum names such as <c>StudioBlue</c>, is refused.</summary>
    /// <param name="id">The id, for example from the cookie.</param>
    /// <param name="palette">The palette when the id is known.</param>
    /// <returns><see langword="true"/> when <paramref name="id"/> names a palette.</returns>
    public static bool TryParse(string? id, out ThemePalette palette)
    {
        foreach (var (value, name) in Names)
        {
            if (string.Equals(name.Id, id?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                palette = value;
                return true;
            }
        }

        palette = ThemePalette.Framework;
        return false;
    }
}
