using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Components;

/// <summary>Defines the ThemeToggle type.</summary>
public partial class ThemeToggle : IDisposable
{
    [Inject] private IOptions<ThemingOptions> ThemingOptions { get; set; } = default!;

    /// <summary>
    /// Gets or sets the id of the toggle button. No id is written when it is <see langword="null"/>, so several toggles
    /// can share a page without repeating an id; set it only when a host stylesheet or script needs to address one.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets a callback that is declared but never raised: the theme changes in the browser and the server is
    /// not notified. Listen for the <c>osirion-theme-update</c> browser event on <c>window</c> instead.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnThemeChanged { get; set; }

    private string CurrentTheme => ThemeService.CurrentMode == ThemeMode.Dark ? "dark" : "light";
    private string GetTooltipText() => CurrentTheme == "dark" ? "Switch to light mode" : "Switch to dark mode";
    private string GetAriaLabel() => $"Toggle theme. Currently using {CurrentTheme} mode. Click to switch to {(CurrentTheme == "dark" ? "light" : "dark")} mode.";
    private string GetScreenReaderText() => $"Current theme: {CurrentTheme}. Click to toggle.";
    private string GetDefaultTheme() => ThemingOptions.Value.DefaultMode.ToString().ToLowerInvariant();
    private string GetFramework() => ThemingOptions.Value.Framework.ToString();

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChangedHandler;
        base.OnInitialized();
    }

    private void OnThemeChangedHandler(object? sender, ThemeChangedEventArgs e)
    {
        InvokeAsync(StateHasChanged);
    }

    /// <summary>Releases resources held by the component or service.</summary>
    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChangedHandler;
        GC.SuppressFinalize(this);
    }
}
