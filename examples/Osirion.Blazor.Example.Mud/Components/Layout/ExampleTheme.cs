using MudBlazor;

namespace Osirion.Blazor.Example.Mud.Components.Layout;

/// <summary>
/// The site's MudBlazor theme, rendered twice: the light palette on <c>:root</c> and the dark palette on
/// <c>:root.mud-theme-dark</c>, the class Osirion's theme toggle sets on the html element.
/// </summary>
internal static class ExampleTheme
{
    private const string DarkScope = ":root.mud-theme-dark";

    private static readonly PaletteLight LightPalette = new()
    {
        Primary = "#5c5cb0",
        Secondary = "#9090d6",
        AppbarBackground = "#5c5cb0",
    };

    private static readonly PaletteDark DarkPalette = new()
    {
        Primary = "#9090d6",
        Secondary = "#b4b4e6",
        Background = "#14142a",
        BackgroundGray = "#24244a",
        Surface = "#1c1c38",
        AppbarBackground = "#1c1c38",
    };

    /// <summary>Gets the theme whose light palette is written to <c>:root</c>.</summary>
    public static MudTheme Light { get; } = new()
    {
        PaletteLight = LightPalette,
        PaletteDark = DarkPalette,
    };

    /// <summary>Gets the theme whose dark palette is written to <c>:root.mud-theme-dark</c>.</summary>
    public static MudTheme Dark { get; } = new()
    {
        PaletteLight = LightPalette,
        PaletteDark = DarkPalette,
        PseudoCss = new PseudoCss { Scope = DarkScope },
    };
}
