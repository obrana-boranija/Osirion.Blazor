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

        // Base variables for all themes
        variables.AddRange(GetBaseVariables());

        // Framework-specific mappings
        if (CurrentFramework != CssFramework.None)
        {
            variables.AddRange(GetFrameworkVariables());
        }

        // Mode-specific variables
        if (CurrentMode == ThemeMode.Dark)
        {
            variables.AddRange(GetDarkModeVariables());

            // Add framework-specific dark mode variables if applicable
            if (CurrentFramework != CssFramework.None)
            {
                variables.AddRange(GetDarkModeFrameworkVariables());
            }
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

    /// <summary>
    /// Gets the base variables that apply to all themes
    /// </summary>
    private IEnumerable<string> GetBaseVariables()
    {
        return new[]
        {
            // Add any essential base variables here that should always be present
            // regardless of framework or theme mode
            "--osirion-transition-speed: 0.2s;",
            "--osirion-gap: 1.5rem;",
            "--osirion-small-gap: 0.5rem;",
            "--osirion-padding: 1.5rem;",
            "--osirion-small-padding: 0.5rem;",
        };
    }

    /// <summary>
    /// Gets framework-specific variables for the current CSS framework
    /// </summary>
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
                "--osirion-light-background-color: var(--bs-gray-100, #f8f9fa);",
                "--osirion-border-radius: var(--bs-border-radius, 0.375rem);",
                "--osirion-small-border-radius: var(--bs-border-radius-sm, 0.25rem);",
                "--osirion-tag-border-radius: var(--bs-border-radius-pill, 50rem);",
                "--osirion-font-size: var(--bs-body-font-size, 1rem);",
                "--osirion-small-font-size: var(--bs-font-size-sm, 0.875rem);",
                "--osirion-tiny-font-size: var(--bs-font-size-sm, 0.75rem);",
                "--osirion-title-font-size: var(--bs-font-size-lg, 1.25rem);",
                "--osirion-line-height: var(--bs-body-line-height, 1.5);",
                "--osirion-box-shadow: var(--bs-box-shadow, 0 0.5rem 1rem rgba(0, 0, 0, 0.15));",
                // Component-specific variables
                "--osirion-tag-background-color: var(--bs-gray-200, #e9ecef);",
                "--osirion-tag-text-color: var(--bs-gray-700, #495057);",
                "--osirion-category-background-color: var(--bs-primary-subtle, #cfe2ff);",
                "--osirion-category-text-color: var(--bs-primary, #0d6efd);",
                "--osirion-active-background-color: var(--bs-primary-subtle, #cfe2ff);",
                "--osirion-active-text-color: var(--bs-primary, #0d6efd);",
                // Admin-specific variables
                "--osirion-admin-primary: var(--bs-primary, #0d6efd);",
                "--osirion-admin-primary-light: var(--bs-primary-subtle, #cfe2ff);",
                "--osirion-admin-primary-dark: var(--bs-primary-dark, #0a58ca);",
                "--osirion-admin-text: var(--bs-body-color, #212529);",
                "--osirion-admin-text-muted: var(--bs-secondary, #6c757d);",
                "--osirion-admin-bg: var(--bs-body-bg, #ffffff);",
                "--osirion-admin-bg-sidebar: var(--bs-gray-100, #f8f9fa);",
                "--osirion-admin-bg-header: var(--bs-gray-100, #f8f9fa);",
                "--osirion-admin-border: var(--bs-border-color, #dee2e6);",
                "--osirion-admin-light: var(--bs-gray-100, #f8f9fa);",
                "--osirion-admin-bg-editor: var(--bs-body-bg, #ffffff);",
                "--osirion-admin-bg-editor-sidebar: var(--bs-gray-100, #f8f9fa);",
                "--osirion-admin-text-editor: var(--bs-body-color, #212529);",
                "--osirion-admin-border-radius: var(--bs-border-radius, 0.375rem);",
                "--osirion-admin-font-size: var(--bs-body-font-size, 1rem);",
                "--osirion-admin-font-family: var(--bs-font-sans-serif, system-ui, -apple-system, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif);",
            },
            CssFramework.Tailwind => new[]
            {
                "--osirion-primary-color: theme('colors.blue.600');",
                "--osirion-primary-hover-color: theme('colors.blue.700');",
                "--osirion-text-color: theme('colors.gray.700');",
                "--osirion-light-text-color: theme('colors.gray.500');",
                "--osirion-border-color: theme('colors.gray.200');",
                "--osirion-background-color: theme('colors.white');",
                "--osirion-light-background-color: theme('colors.gray.100');",
                "--osirion-border-radius: theme('borderRadius.lg');",
                "--osirion-small-border-radius: theme('borderRadius.md');",
                "--osirion-tag-border-radius: theme('borderRadius.full');",
                "--osirion-font-size: theme('fontSize.base');",
                "--osirion-small-font-size: theme('fontSize.sm');",
                "--osirion-tiny-font-size: theme('fontSize.xs');",
                "--osirion-title-font-size: theme('fontSize.xl');",
                "--osirion-line-height: theme('lineHeight.relaxed');",
                "--osirion-box-shadow: theme('boxShadow.md');",
                // Component-specific variables
                "--osirion-tag-background-color: theme('colors.gray.100');",
                "--osirion-tag-text-color: theme('colors.gray.600');",
                "--osirion-category-background-color: theme('colors.blue.100');",
                "--osirion-category-text-color: theme('colors.blue.600');",
                "--osirion-active-background-color: theme('colors.blue.100');",
                "--osirion-active-text-color: theme('colors.blue.600');",
                // Admin-specific variables
                "--osirion-admin-primary: theme('colors.blue.600');",
                "--osirion-admin-primary-light: theme('colors.blue.400');",
                "--osirion-admin-primary-dark: theme('colors.blue.700');",
                "--osirion-admin-text: theme('colors.gray.700');",
                "--osirion-admin-text-muted: theme('colors.gray.500');",
                "--osirion-admin-bg: theme('colors.white');",
                "--osirion-admin-bg-sidebar: theme('colors.gray.100');",
                "--osirion-admin-bg-header: theme('colors.gray.100');",
                "--osirion-admin-border: theme('colors.gray.200');",
                "--osirion-admin-light: theme('colors.gray.100');",
                "--osirion-admin-bg-editor: theme('colors.white');",
                "--osirion-admin-bg-editor-sidebar: theme('colors.gray.100');",
                "--osirion-admin-text-editor: theme('colors.gray.700');",
                "--osirion-admin-border-radius: theme('borderRadius.lg');",
                "--osirion-admin-font-size: theme('fontSize.base');",
                "--osirion-admin-font-family: theme('fontFamily.sans').toString();",
            },
            CssFramework.FluentUI => new[]
            {
                "--osirion-primary-color: var(--accent-fill-rest, #197834);",
                "--osirion-primary-hover-color: var(--accent-fill-hover, #1c833a);",
                "--osirion-text-color: var(--neutral-foreground-rest, #1a1a1a);",
                "--osirion-light-text-color: var(--neutral-foreground-hint, #717171);",
                "--osirion-border-color: var(--neutral-stroke-rest, #d6d6d6);",
                "--osirion-background-color: var(--neutral-fill-layer-rest, #ffffff);",
                "--osirion-light-background-color: var(--neutral-fill-stealth-rest, #f3f2f1);",
                "--osirion-border-radius: var(--control-corner-radius, 4px);",
                "--osirion-small-border-radius: var(--control-corner-radius, 2px);",
                "--osirion-tag-border-radius: 999px;",
                "--osirion-font-size: var(--type-ramp-base-font-size, 14px);",
                "--osirion-small-font-size: var(--type-ramp-minus1-font-size, 12px);",
                "--osirion-tiny-font-size: var(--type-ramp-minus2-font-size, 10px);",
                "--osirion-title-font-size: var(--type-ramp-plus2-font-size, 20px);",
                "--osirion-line-height: var(--type-ramp-base-line-height, 20px);",
                "--osirion-box-shadow: var(--elevation-shadow-card-rest, 0 0 2px rgba(0, 0, 0, 0.12), 0 2px 4px rgba(0, 0, 0, 0.14));",
                // Component-specific variables
                "--osirion-tag-background-color: var(--neutral-fill-stealth-rest, #f3f2f1);",
                "--osirion-tag-text-color: var(--neutral-foreground-rest, #1a1a1a);",
                "--osirion-category-background-color: var(--neutral-fill-stealth-rest, #e1dfdd);",
                "--osirion-category-text-color: var(--accent-foreground-rest, #0078d4);",
                "--osirion-active-background-color: var(--accent-fill-subtle-rest, #e6f2fc);",
                "--osirion-active-text-color: var(--accent-foreground-rest, #0078d4);",
                // Admin-specific variables
                "--osirion-admin-primary: var(--accent-fill-rest, #197834);",
                "--osirion-admin-primary-light: var(--accent-fill-subtle-rest, #e6f2fc);",
                "--osirion-admin-primary-dark: var(--accent-fill-hover, #1c833a);",
                "--osirion-admin-text: var(--neutral-foreground-rest, #1a1a1a);",
                "--osirion-admin-text-muted: var(--neutral-foreground-hint, #717171);",
                "--osirion-admin-bg: var(--neutral-fill-layer-rest, #ffffff);",
                "--osirion-admin-bg-sidebar: var(--neutral-fill-stealth-rest, #f3f2f1);",
                "--osirion-admin-bg-header: var(--neutral-fill-stealth-rest, #f3f2f1);",
                "--osirion-admin-border: var(--neutral-stroke-rest, #d6d6d6);",
                "--osirion-admin-light: var(--neutral-fill-stealth-rest, #f3f2f1);",
                "--osirion-admin-bg-editor: var(--neutral-fill-layer-rest, #ffffff);",
                "--osirion-admin-bg-editor-sidebar: var(--neutral-fill-stealth-rest, #f3f2f1);",
                "--osirion-admin-text-editor: var(--neutral-foreground-rest, #1a1a1a);",
                "--osirion-admin-border-radius: var(--control-corner-radius, 4px);",
                "--osirion-admin-font-size: var(--type-ramp-base-font-size, 14px);",
                "--osirion-admin-font-family: var(--body-font, 'Segoe UI', 'Segoe UI Web (West European)', 'Segoe UI', -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', sans-serif);",
            },
            CssFramework.MudBlazor => new[]
            {
                "--osirion-primary-color: var(--mud-palette-primary, #594ae2);",
                "--osirion-primary-hover-color: var(--mud-palette-primary-darken, #4538ca);",
                "--osirion-text-color: var(--mud-palette-text-primary, rgba(0, 0, 0, 0.87));",
                "--osirion-light-text-color: var(--mud-palette-text-secondary, rgba(0, 0, 0, 0.6));",
                "--osirion-border-color: var(--mud-palette-divider, rgba(0, 0, 0, 0.12));",
                "--osirion-background-color: var(--mud-palette-background, #ffffff);",
                "--osirion-light-background-color: var(--mud-palette-background-grey, #f5f5f5);",
                "--osirion-border-radius: var(--mud-default-borderradius, 4px);",
                "--osirion-small-border-radius: var(--mud-default-borderradius, 4px);",
                "--osirion-tag-border-radius: 16px;",
                "--osirion-font-size: var(--mud-typography-body1-size, 1rem);",
                "--osirion-small-font-size: var(--mud-typography-body2-size, 0.875rem);",
                "--osirion-tiny-font-size: var(--mud-typography-caption-size, 0.75rem);",
                "--osirion-title-font-size: var(--mud-typography-h6-size, 1.25rem);",
                "--osirion-line-height: var(--mud-typography-body1-lineheight, 1.5);",
                "--osirion-box-shadow: var(--mud-elevation-4, 0px 2px 4px -1px rgba(0,0,0,0.2), 0px 4px 5px 0px rgba(0,0,0,0.14), 0px 1px 10px 0px rgba(0,0,0,0.12));",
                // Component-specific variables
                "--osirion-tag-background-color: var(--mud-palette-background-grey, #f5f5f5);",
                "--osirion-tag-text-color: var(--mud-palette-text-primary, rgba(0, 0, 0, 0.87));",
                "--osirion-category-background-color: var(--mud-palette-primary-lighten, #c5c2f6);",
                "--osirion-category-text-color: var(--mud-palette-primary-darken, #4538ca);",
                "--osirion-active-background-color: var(--mud-palette-primary-lighten, #c5c2f6);",
                "--osirion-active-text-color: var(--mud-palette-primary-darken, #4538ca);",
                // Admin-specific variables
                "--osirion-admin-primary: var(--mud-palette-primary, #594ae2);",
                "--osirion-admin-primary-light: var(--mud-palette-primary-lighten, #c5c2f6);",
                "--osirion-admin-primary-dark: var(--mud-palette-primary-darken, #4538ca);",
                "--osirion-admin-text: var(--mud-palette-text-primary, rgba(0, 0, 0, 0.87));",
                "--osirion-admin-text-muted: var(--mud-palette-text-secondary, rgba(0, 0, 0, 0.6));",
                "--osirion-admin-bg: var(--mud-palette-background, #ffffff);",
                "--osirion-admin-bg-sidebar: var(--mud-palette-background-grey, #f5f5f5);",
                "--osirion-admin-bg-header: var(--mud-palette-background-grey, #f5f5f5);",
                "--osirion-admin-border: var(--mud-palette-divider, rgba(0, 0, 0, 0.12));",
                "--osirion-admin-light: var(--mud-palette-background-grey, #f5f5f5);",
                "--osirion-admin-bg-editor: var(--mud-palette-background, #ffffff);",
                "--osirion-admin-bg-editor-sidebar: var(--mud-palette-background-grey, #f5f5f5);",
                "--osirion-admin-text-editor: var(--mud-palette-text-primary, rgba(0, 0, 0, 0.87));",
                "--osirion-admin-border-radius: var(--mud-default-borderradius, 4px);",
                "--osirion-admin-font-size: var(--mud-typography-body1-size, 1rem);",
                "--osirion-admin-font-family: var(--mud-typography-default-family, 'Roboto', 'Helvetica', 'Arial', sans-serif);",
            },
            CssFramework.Radzen => new[]
            {
                "--osirion-primary-color: var(--rz-primary, #479cc8);",
                "--osirion-primary-hover-color: var(--rz-primary-dark, #3d8bb1);",
                "--osirion-text-color: var(--rz-text-color, #212529);",
                "--osirion-light-text-color: var(--rz-text-secondary-color, #6c757d);",
                "--osirion-border-color: var(--rz-border, #e9ecef);",
                "--osirion-background-color: var(--rz-base-background-color, #ffffff);",
                "--osirion-light-background-color: var(--rz-base-100, #f8f9fa);",
                "--osirion-border-radius: var(--rz-border-radius, 4px);",
                "--osirion-small-border-radius: var(--rz-border-radius, 4px);",
                "--osirion-tag-border-radius: 16px;",
                "--osirion-font-size: var(--rz-body-font-size, 0.875rem);",
                "--osirion-small-font-size: var(--rz-text-sm, 0.875rem);",
                "--osirion-tiny-font-size: var(--rz-text-xs, 0.75rem);",
                "--osirion-title-font-size: var(--rz-h5-font-size, 1.25rem);",
                "--osirion-line-height: var(--rz-body-line-height, 1.5);",
                "--osirion-box-shadow: var(--rz-shadow-2, 0 3px 1px -2px rgba(0,0,0,.2), 0 2px 2px 0 rgba(0,0,0,.14), 0 1px 5px 0 rgba(0,0,0,.12));",
                // Component-specific variables
                "--osirion-tag-background-color: var(--rz-base-200, #e9ecef);",
                "--osirion-tag-text-color: var(--rz-text-color, #212529);",
                "--osirion-category-background-color: var(--rz-primary-lighter, #daecf6);",
                "--osirion-category-text-color: var(--rz-primary-dark, #3d8bb1);",
                "--osirion-active-background-color: var(--rz-primary-lighter, #daecf6);",
                "--osirion-active-text-color: var(--rz-primary-dark, #3d8bb1);",
                // Admin-specific variables
                "--osirion-admin-primary: var(--rz-primary, #479cc8);",
                "--osirion-admin-primary-light: var(--rz-primary-lighter, #daecf6);",
                "--osirion-admin-primary-dark: var(--rz-primary-dark, #3d8bb1);",
                "--osirion-admin-text: var(--rz-text-color, #212529);",
                "--osirion-admin-text-muted: var(--rz-text-secondary-color, #6c757d);",
                "--osirion-admin-bg: var(--rz-base-background-color, #ffffff);",
                "--osirion-admin-bg-sidebar: var(--rz-base-100, #f8f9fa);",
                "--osirion-admin-bg-header: var(--rz-base-100, #f8f9fa);",
                "--osirion-admin-border: var(--rz-border, #e9ecef);",
                "--osirion-admin-light: var(--rz-base-100, #f8f9fa);",
                "--osirion-admin-bg-editor: var(--rz-base-background-color, #ffffff);",
                "--osirion-admin-bg-editor-sidebar: var(--rz-base-100, #f8f9fa);",
                "--osirion-admin-text-editor: var(--rz-text-color, #212529);",
                "--osirion-admin-border-radius: var(--rz-border-radius, 4px);",
                "--osirion-admin-font-size: var(--rz-body-font-size, 0.875rem);",
                "--osirion-admin-font-family: var(--rz-text-font-family, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif);",
            },
            _ => Array.Empty<string>()
        };
    }

    /// <summary>
    /// Gets dark mode variables to apply when dark theme is active
    /// </summary>
    private IEnumerable<string> GetDarkModeVariables()
    {
        return new[]
        {
            // Base colors for dark mode
            "--osirion-primary-color: #60a5fa;",
            "--osirion-primary-hover-color: #93c5fd;",
            "--osirion-text-color: #e5e7eb;",
            "--osirion-light-text-color: #9ca3af;",
            "--osirion-border-color: #4b5563;",
            "--osirion-background-color: #1f2937;",
            "--osirion-light-background-color: #374151;",
            "--osirion-box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);",
            
            // Component-specific variables for dark mode
            "--osirion-tag-background-color: #374151;",
            "--osirion-tag-text-color: #e5e7eb;",
            "--osirion-category-background-color: #1e3a8a;",
            "--osirion-category-text-color: #93c5fd;",
            "--osirion-active-background-color: #1e40af;",
            "--osirion-active-text-color: #bfdbfe;",
            
            // Admin theme compatibility variables for dark mode
            "--osirion-admin-primary: #58a6ff;",
            "--osirion-admin-primary-light: #264f78;",
            "--osirion-admin-primary-dark: #3b82f6;",
            "--osirion-admin-text: #c9d1d9;",
            "--osirion-admin-text-muted: #8b949e;",
            "--osirion-admin-bg: #0d1117;",
            "--osirion-admin-bg-sidebar: #161b22;",
            "--osirion-admin-bg-header: #161b22;",
            "--osirion-admin-border: #30363d;",
            "--osirion-admin-light: #1f2937;",
            
            // Editor-specific colors for dark mode
            "--osirion-admin-bg-editor: #1e293b;",
            "--osirion-admin-bg-editor-sidebar: #111827;",
            "--osirion-admin-border-editor: #374151;",
            "--osirion-admin-text-editor: #e5e7eb;",
            "--osirion-admin-text-editor-muted: #9ca3af;",
            
            // Button and control colors for dark mode
            "--osirion-button-background-color: #374151;",
            "--osirion-button-hover-background-color: #4b5563;",
            "--osirion-button-text-color: #e5e7eb;",
            "--osirion-button-border-color: #4b5563;",
            
            // Input and form colors for dark mode
            "--osirion-input-background-color: #111827;",
            "--osirion-input-text-color: #e5e7eb;",
            "--osirion-input-border-color: #4b5563;",
            "--osirion-input-focus-border-color: #60a5fa;",
            
            // Additional component variables for dark mode
            "--osirion-card-background-color: #1f2937;",
            "--osirion-card-border-color: #4b5563;",
            "--osirion-header-background-color: #111827;",
            "--osirion-sidebar-background-color: #111827;",
            "--osirion-footer-background-color: #111827;",
            
            // Content-specific dark mode variables 
            "--osirion-content-background-color: #1f2937;",
            "--osirion-content-text-color: #e5e7eb;",
            "--osirion-content-heading-color: #f3f4f6;",
            "--osirion-content-link-color: #60a5fa;",
            "--osirion-content-code-background-color: #111827;",
            "--osirion-content-code-text-color: #e5e7eb;",
            "--osirion-content-blockquote-background-color: #111827;",
            "--osirion-content-blockquote-border-color: #4b5563;",
            "--osirion-content-table-background-color: #1f2937;",
            "--osirion-content-table-border-color: #4b5563;",
            "--osirion-content-table-header-background-color: #111827;",

            // Blog listing and content variables - seen in images 1 & 2
            "--osirion-blog-card-background: #1e293b;",
            "--osirion-blog-card-border-color: #374151;",
            "--osirion-blog-card-hover-shadow: 0 4px 14px rgba(0, 0, 0, 0.3);",
            "--osirion-blog-title-color: #f3f4f6;",
            "--osirion-blog-meta-color: #9ca3af;",
            "--osirion-blog-text-color: #e5e7eb;",
            "--osirion-blog-link-color: #60a5fa;",
            "--osirion-blog-link-hover-color: #93c5fd;",
            "--osirion-blog-tag-background: #374151;",
            "--osirion-blog-tag-text-color: #d1d5db;",
            "--osirion-blog-category-background: #1e40af;",
            "--osirion-blog-category-text-color: #bfdbfe;",

            // Navigation and sidebar variables - seen in image 1
            "--osirion-nav-background: #1f2937;",
            "--osirion-nav-border-color: #374151;",
            "--osirion-nav-text-color: #e5e7eb;",
            "--osirion-nav-active-background: #2563eb;",
            "--osirion-nav-active-text-color: #ffffff;",
            "--osirion-sidebar-background: #111827;",
            "--osirion-sidebar-title-color: #f3f4f6;",
            "--osirion-sidebar-text-color: #e5e7eb;",
            "--osirion-sidebar-border-color: #374151;",

            // Search component variables - seen in image 1
            "--osirion-search-background: #111827;",
            "--osirion-search-text-color: #e5e7eb;",
            "--osirion-search-border-color: #4b5563;",
            "--osirion-search-focus-border-color: #60a5fa;",
            "--osirion-search-button-background: #2563eb;",
            "--osirion-search-button-hover-background: #1d4ed8;",

            // ScrollToTop variables - seen in image 3
            "--osirion-scroll-background: rgba(37, 99, 235, 0.9);",
            "--osirion-scroll-hover-background: rgba(29, 78, 216, 1);",
            "--osirion-scroll-color: #ffffff;",
            "--osirion-scroll-hover-color: #ffffff;",
            "--osirion-scroll-border-color: rgba(37, 99, 235, 0.9);",
            "--osirion-scroll-hover-border-color: rgba(29, 78, 216, 1);",
            "--osirion-scroll-box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);",
            "--osirion-scroll-hover-box-shadow: 0 6px 12px rgba(0, 0, 0, 0.4);",

            // Button variables - seen in image 3
            "--osirion-button-primary-background: #2563eb;",
            "--osirion-button-primary-text-color: #ffffff;",
            "--osirion-button-primary-hover-background: #1d4ed8;",
            "--osirion-button-primary-border-color: #2563eb;",
            "--osirion-button-secondary-background: #4b5563;",
            "--osirion-button-secondary-text-color: #ffffff;",
            "--osirion-button-secondary-hover-background: #374151;",
            "--osirion-button-secondary-border-color: #4b5563;",
            "--osirion-button-disabled-background: #374151;",
            "--osirion-button-disabled-text-color: #9ca3af;",

            // Cards grid layout variables - seen in image 3
            "--osirion-grid-gap: 1.5rem;",
            "--osirion-card-padding: 1.5rem;",
            "--osirion-card-border-radius: 0.5rem;",
            "--osirion-card-background: #1e293b;",
            "--osirion-card-border-color: #374151;",
            "--osirion-card-shadow: 0 2px 6px rgba(0, 0, 0, 0.2);",
            "--osirion-card-hover-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);",
            "--osirion-card-title-color: #f3f4f6;",
            "--osirion-card-text-color: #e5e7eb;",
            "--osirion-card-meta-color: #9ca3af;",
        };
    }

    /// <summary>
    /// Gets framework-specific dark mode variables
    /// </summary>
    private IEnumerable<string> GetDarkModeFrameworkVariables()
    {
        return CurrentFramework switch
        {
            CssFramework.Bootstrap => new[]
            {
                // Bootstrap-specific dark mode compatibility 
                "--osirion-primary-color: var(--bs-primary-dark, #3a86ff);",
                "--osirion-primary-hover-color: var(--bs-primary-text, #cfe2ff);",
                "--osirion-background-color: var(--bs-dark, #212529);",
                "--osirion-light-background-color: var(--bs-gray-800, #343a40);",
                "--osirion-text-color: var(--bs-white, #ffffff);",
                "--osirion-light-text-color: var(--bs-gray-300, #dee2e6);",
                "--osirion-border-color: var(--bs-gray-700, #495057);",
                // Admin-specific
                "--osirion-admin-bg: var(--bs-dark, #212529);",
                "--osirion-admin-bg-sidebar: var(--bs-gray-800, #343a40);",
                "--osirion-admin-bg-header: var(--bs-gray-800, #343a40);",
                "--osirion-admin-text: var(--bs-white, #ffffff);",
                "--osirion-admin-text-muted: var(--bs-gray-300, #dee2e6);",
                "--osirion-admin-border: var(--bs-gray-700, #495057);",
            },
            CssFramework.Tailwind => new[]
            {
                // Tailwind-specific dark mode compatibility
                "--osirion-primary-color: theme('colors.blue.400');",
                "--osirion-primary-hover-color: theme('colors.blue.300');",
                "--osirion-background-color: theme('colors.gray.900');",
                "--osirion-light-background-color: theme('colors.gray.800');",
                "--osirion-text-color: theme('colors.gray.100');",
                "--osirion-light-text-color: theme('colors.gray.400');",
                "--osirion-border-color: theme('colors.gray.700');",
                // Admin-specific
                "--osirion-admin-bg: theme('colors.gray.900');",
                "--osirion-admin-bg-sidebar: theme('colors.gray.800');",
                "--osirion-admin-bg-header: theme('colors.gray.800');",
                "--osirion-admin-text: theme('colors.gray.100');",
                "--osirion-admin-text-muted: theme('colors.gray.400');",
                "--osirion-admin-border: theme('colors.gray.700');",
            },
            CssFramework.FluentUI => new[]
            {
                // FluentUI-specific dark mode compatibility
                "--osirion-primary-color: var(--accent-fill-rest, #3b82f6);",
                "--osirion-primary-hover-color: var(--accent-fill-hover, #60a5fa);",
                "--osirion-background-color: var(--neutral-fill-layer-rest, #1f2937);",
                "--osirion-light-background-color: var(--neutral-fill-stealth-rest, #111827);",
                "--osirion-text-color: var(--neutral-foreground-rest, #f3f4f6);",
                "--osirion-light-text-color: var(--neutral-foreground-hint, #9ca3af);",
                "--osirion-border-color: var(--neutral-stroke-rest, #4b5563);",
                // Admin-specific
                "--osirion-admin-bg: var(--neutral-fill-layer-rest, #1f2937);",
                "--osirion-admin-bg-sidebar: var(--neutral-fill-stealth-rest, #111827);",
                "--osirion-admin-bg-header: var(--neutral-fill-stealth-rest, #111827);",
                "--osirion-admin-text: var(--neutral-foreground-rest, #f3f4f6);",
                "--osirion-admin-text-muted: var(--neutral-foreground-hint, #9ca3af);",
                "--osirion-admin-border: var(--neutral-stroke-rest, #4b5563);",
            },
            CssFramework.MudBlazor => new[]
            {
                // MudBlazor-specific dark mode compatibility
                "--osirion-primary-color: var(--mud-palette-primary, #7e6fff);",
                "--osirion-primary-hover-color: var(--mud-palette-primary-lighten, #988dff);",
                "--osirion-background-color: var(--mud-palette-background, #121212);",
                "--osirion-light-background-color: var(--mud-palette-background-grey, #1e1e1e);",
                "--osirion-text-color: var(--mud-palette-text-primary, rgba(255, 255, 255, 0.87));",
                "--osirion-light-text-color: var(--mud-palette-text-secondary, rgba(255, 255, 255, 0.6));",
                "--osirion-border-color: var(--mud-palette-divider, rgba(255, 255, 255, 0.12));",
                // Admin-specific
                "--osirion-admin-bg: var(--mud-palette-background, #121212);",
                "--osirion-admin-bg-sidebar: var(--mud-palette-background-grey, #1e1e1e);",
                "--osirion-admin-bg-header: var(--mud-palette-background-grey, #1e1e1e);",
                "--osirion-admin-text: var(--mud-palette-text-primary, rgba(255, 255, 255, 0.87));",
                "--osirion-admin-text-muted: var(--mud-palette-text-secondary, rgba(255, 255, 255, 0.6));",
                "--osirion-admin-border: var(--mud-palette-divider, rgba(255, 255, 255, 0.12));",
            },
            CssFramework.Radzen => new[]
            {
                // Radzen-specific dark mode compatibility
                "--osirion-primary-color: var(--rz-primary, #6ba6d6);",
                "--osirion-primary-hover-color: var(--rz-primary-light, #8fbeeb);",
                "--osirion-background-color: var(--rz-base-900, #1a1a1a);",
                "--osirion-light-background-color: var(--rz-base-800, #323232);",
                "--osirion-text-color: var(--rz-text-color, #ffffff);",
                "--osirion-light-text-color: var(--rz-text-secondary-color, #aaaaaa);",
                "--osirion-border-color: var(--rz-border, #525252);",
                // Admin-specific
                "--osirion-admin-bg: var(--rz-base-900, #1a1a1a);",
                "--osirion-admin-bg-sidebar: var(--rz-base-800, #323232);",
                "--osirion-admin-bg-header: var(--rz-base-800, #323232);",
                "--osirion-admin-text: var(--rz-text-color, #ffffff);",
                "--osirion-admin-text-muted: var(--rz-text-secondary-color, #aaaaaa);",
                "--osirion-admin-border: var(--rz-border, #525252);",
            },
            _ => Array.Empty<string>()
        };
    }
}