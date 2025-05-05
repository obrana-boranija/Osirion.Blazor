using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Theming.Options;

namespace Osirion.Blazor.Theming.Components;
public partial class OsirionStyles
{
    /// <summary>
    /// Gets or sets whether to use default styles
    /// </summary>
    [Parameter]
    public bool? UseStyles { get; set; }

    /// <summary>
    /// Gets or sets custom CSS variables
    /// </summary>
    [Parameter]
    public string? CustomVariables { get; set; }

    /// <summary>
    /// Gets or sets the CSS framework to integrate with
    /// </summary>
    [Parameter]
    public CssFramework? Framework { get; set; }

    /// <summary>
    /// Gets or sets the theme mode
    /// </summary>
    [Parameter]
    public ThemeMode? Mode { get; set; }

    /// <summary>
    /// Gets the effective options, merging parameters with configured options
    /// </summary>
    private ThemingOptions EffectiveOptions => new()
    {
        UseDefaultStyles = UseStyles ?? Options?.Value.UseDefaultStyles ?? true,
        CustomVariables = CustomVariables ?? Options?.Value.CustomVariables,
        Framework = Framework ?? Options?.Value.Framework ?? CssFramework.None,
        DefaultMode = Mode ?? Options?.Value.DefaultMode ?? ThemeMode.Light
    };

    /// <summary>
    /// Gets the generated CSS variables for the current theme
    /// </summary>
    private string GeneratedVariables => ThemeService.GenerateThemeVariables();

    /// <summary>
    /// Gets the framework class based on the selected framework
    /// </summary>
    private string GetFrameworkClass()
    {
        return EffectiveOptions.Framework switch
        {
            CssFramework.Bootstrap => "osirion-bootstrap-integration",
            CssFramework.FluentUI => "osirion-fluent-integration",
            CssFramework.MudBlazor => "osirion-mudblazor-integration",
            CssFramework.Radzen => "osirion-radzen-integration",
            _ => ""
        };
    }

    /// <summary>
    /// Gets the CSS declaration for the framework class
    /// </summary>
    private string GetFrameworkClassDeclaration()
    {
        var frameworkClass = GetFrameworkClass();
        return !string.IsNullOrEmpty(frameworkClass) ? $"class: \"{frameworkClass}\"" : "";
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}
