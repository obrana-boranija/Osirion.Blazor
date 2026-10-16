using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Components;

/// <summary>
/// A list from which visitors pick a <see cref="ThemePalette"/>. The choice is applied in the browser at once and kept in
/// the <c>osirion-preferred-palette</c> cookie, so it works under static SSR and on sites exported to static files.
/// Light and dark stay with <see cref="ThemeToggle"/>.
/// </summary>
public partial class ThemePicker : IDisposable
{
    private static readonly ThemePalette[] AllPalettes = Enum.GetValues<ThemePalette>();

    [Inject] private IThemeService ThemeService { get; set; } = default!;

    /// <summary>Gets or sets the visible label of the list.</summary>
    [Parameter]
    public string Label { get; set; } = "Palette";

    /// <summary>
    /// Gets or sets the palettes offered, in this order. Defaults to every palette. A palette the visitor already uses is
    /// offered even when it is not listed, so the list always shows the palette in effect.
    /// </summary>
    [Parameter]
    public IReadOnlyList<ThemePalette>? Palettes { get; set; }

    /// <summary>
    /// Gets or sets the text of the <see cref="ThemePalette.Framework"/> option, which applies no palette and shows the
    /// site's own colours.
    /// </summary>
    [Parameter]
    public string FrameworkLabel { get; set; } = "Default";

    private ThemePalette SelectedPalette => ThemeService.CurrentPalette;

    private IEnumerable<ThemePalette> OfferedPalettes
    {
        get
        {
            var offered = (Palettes ?? AllPalettes).Where(palette => Enum.IsDefined(palette)).Distinct().ToList();
            if (!offered.Contains(SelectedPalette))
            {
                offered.Insert(0, SelectedPalette);
            }

            return offered;
        }
    }

    private string OptionText(ThemePalette palette) =>
        palette == ThemePalette.Framework ? FrameworkLabel : palette.ToDisplayName();

    /// <summary>Subscribes to theme changes so the selection follows a palette set on the server.</summary>
    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;
        base.OnInitialized();
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => InvokeAsync(StateHasChanged);

    /// <summary>Unsubscribes from theme changes.</summary>
    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
        GC.SuppressFinalize(this);
    }
}
