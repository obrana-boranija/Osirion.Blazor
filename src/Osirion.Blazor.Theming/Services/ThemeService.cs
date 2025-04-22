using Microsoft.Extensions.Options;
using Osirion.Blazor.Theming.Options;

namespace Osirion.Blazor.Theming.Services;

/// <summary>
/// Default implementation of the theme service
/// </summary>
public class ThemeService : IThemeService
{
    private readonly ThemingOptions _options;
    private ThemeMode _currentMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeService"/> class.
    /// </summary>
    public ThemeService(IOptions<ThemingOptions> options)
    {
        _options = options?.Value ?? new ThemingOptions();
        _currentMode = _options.DefaultMode;
    }

    /// <inheritdoc/>
    public CssFramework CurrentFramework => _options.Framework;

    /// <inheritdoc/>
    public ThemeMode CurrentMode
    {
        get => _currentMode;
        set => SetThemeMode(value);
    }

    /// <inheritdoc/>
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <inheritdoc/>
    public string GenerateThemeVariables()
    {
        var variables = new List<string>();

        // Framework-specific mappings
        if (CurrentFramework != CssFramework.None)
        {
            variables.AddRange(GetFrameworkVariables());
        }

        // Mode-specific variables
        if (CurrentMode == ThemeMode.Dark)
        {
            variables.AddRange(GetDarkModeVariables());
        }

        // Custom variables
        if (!string.IsNullOrEmpty(_options.CustomVariables))
        {
            variables.Add(_options.CustomVariables);
        }

        return string.Join("\n", variables);
    }

    /// <inheritdoc/>
    public void SetThemeMode(ThemeMode mode)
    {
        if (_currentMode != mode)
        {
            var previousMode = _currentMode;
            _currentMode = mode;
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(mode, previousMode));
        }
    }

    private IEnumerable<string> GetFrameworkVariables()
    {
        return CurrentFramework switch
        {
            CssFramework.Bootstrap => new[]
            {
                "--osirion-primary-color: var(--bs-primary, #0d6efd);",
                "--osirion-primary-hover-color: var(--bs-primary-dark, #0a58ca);",
                "--osirion-text-color: var(--bs-body-color, #212529);",
                "--osirion-light-text-color: var(--bs-secondary, #6c757d);",
                "--osirion-border-color: var(--bs-border-color, #dee2e6);",
                "--osirion-background-color: var(--bs-body-bg, #ffffff);",
                "--osirion-border-radius: var(--bs-border-radius, 0.375rem);",
                "--osirion-font-size: var(--bs-body-font-size, 1rem);",
                "--osirion-box-shadow: var(--bs-box-shadow, 0 0.5rem 1rem rgba(0, 0, 0, 0.15));"
            },
            CssFramework.Tailwind => new[]
            {
                "--osirion-primary-color: theme('colors.blue.600');",
                "--osirion-primary-hover-color: theme('colors.blue.700');",
                "--osirion-text-color: theme('colors.gray.700');",
                "--osirion-light-text-color: theme('colors.gray.500');",
                "--osirion-border-color: theme('colors.gray.200');",
                "--osirion-background-color: theme('colors.white');",
                "--osirion-border-radius: theme('borderRadius.lg');",
                "--osirion-font-size: theme('fontSize.base');",
                "--osirion-box-shadow: theme('boxShadow.md');"
            },
            CssFramework.FluentUI => new[]
            {
                "--osirion-primary-color: var(--accent-fill-rest, #197834);",
                "--osirion-primary-hover-color: var(--accent-fill-hover, #1c833a);",
                "--osirion-text-color: var(--neutral-foreground-rest, #1a1a1a);",
                "--osirion-light-text-color: var(--neutral-foreground-hint, #717171);",
                "--osirion-border-color: var(--neutral-stroke-rest, #d6d6d6);",
                "--osirion-background-color: var(--neutral-fill-layer-rest, #ffffff);",
                "--osirion-border-radius: var(--control-corner-radius, 4px);",
                "--osirion-font-size: var(--type-ramp-base-font-size, 14px);",
                "--osirion-box-shadow: var(--elevation-shadow-card-rest, 0 0 2px rgba(0, 0, 0, 0.12), 0 2px 4px rgba(0, 0, 0, 0.14));"
            },
            CssFramework.MudBlazor => new[]
            {
                "--osirion-primary-color: var(--mud-palette-primary, #594ae2);",
                "--osirion-primary-hover-color: var(--mud-palette-primary-darken, #4538ca);",
                "--osirion-text-color: var(--mud-palette-text-primary, rgba(0, 0, 0, 0.87));",
                "--osirion-light-text-color: var(--mud-palette-text-secondary, rgba(0, 0, 0, 0.6));",
                "--osirion-border-color: var(--mud-palette-divider, rgba(0, 0, 0, 0.12));",
                "--osirion-background-color: var(--mud-palette-background, #ffffff);",
                "--osirion-border-radius: var(--mud-default-borderradius, 4px);",
                "--osirion-font-size: var(--mud-typography-body1-size, 1rem);",
                "--osirion-box-shadow: var(--mud-elevation-4, 0px 2px 4px -1px rgba(0,0,0,0.2));"
            },
            CssFramework.Radzen => new[]
            {
                "--osirion-primary-color: var(--rz-primary, #479cc8);",
                "--osirion-primary-hover-color: var(--rz-primary-dark, #3d8bb1);",
                "--osirion-text-color: var(--rz-text-color, #212529);",
                "--osirion-light-text-color: var(--rz-text-secondary-color, #6c757d);",
                "--osirion-border-color: var(--rz-border, #e9ecef);",
                "--osirion-background-color: var(--rz-base-background-color, #ffffff);",
                "--osirion-border-radius: var(--rz-border-radius, 4px);",
                "--osirion-font-size: var(--rz-body-font-size, 1rem);",
                "--osirion-box-shadow: var(--rz-shadow-2, 0 3px 1px -2px rgba(0,0,0,.2));"
            },
            _ => Array.Empty<string>()
        };
    }

    private IEnumerable<string> GetDarkModeVariables()
    {
        return new[]
        {
            "--osirion-primary-color: #60a5fa;",
            "--osirion-primary-hover-color: #93c5fd;",
            "--osirion-text-color: #e5e7eb;",
            "--osirion-light-text-color: #9ca3af;",
            "--osirion-border-color: #4b5563;",
            "--osirion-background-color: #1f2937;",
            "--osirion-light-background-color: #374151;"
        };
    }
}