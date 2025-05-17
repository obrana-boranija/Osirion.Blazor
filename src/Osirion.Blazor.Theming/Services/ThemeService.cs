using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;

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

        // Custom variables
        if (!string.IsNullOrEmpty(_options.CustomVariables))
        {
            variables.Add(_options.CustomVariables);
        }

        return string.Join("\n", variables);
    }

    /// <inheritdoc/>
    public string GetFrameworkClass()
    {
        return CurrentFramework switch
        {
            CssFramework.Bootstrap => "osirion-bootstrap-integration",
            CssFramework.FluentUI => "osirion-fluent-integration",
            CssFramework.MudBlazor => "osirion-mudblazor-integration",
            CssFramework.Radzen => "osirion-radzen-integration",
            _ => ""
        };
    }

    /// <inheritdoc/>
    public void SetThemeMode(ThemeMode mode)
    {
        if (_currentMode != mode && _options.EnableDarkMode != false)
        {
            var previousMode = _currentMode;
            _currentMode = mode;
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(mode, previousMode));
        }
    }
}