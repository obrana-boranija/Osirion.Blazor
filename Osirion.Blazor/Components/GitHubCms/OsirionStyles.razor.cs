using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Options;

namespace Osirion.Blazor.Components.GitHubCms;

/// <summary>
/// Component that provides styles for Osirion components
/// </summary>
public partial class OsirionStyles
{
    /// <summary>
    /// The options for Osirion styling
    /// </summary>
    [Inject]
    protected IOptions<OsirionStyleOptions>? Options { get; set; }

    /// <summary>
    /// Gets or sets whether to use the default styles
    /// </summary>
    [Parameter]
    public bool? UseStyles { get; set; }

    /// <summary>
    /// Gets or sets custom CSS variables to override the default values
    /// </summary>
    [Parameter]
    public string? CustomVariables { get; set; }

    /// <summary>
    /// Gets or sets the CSS framework to integrate with
    /// </summary>
    [Parameter]
    public CssFramework FrameworkIntegration { get; set; } = CssFramework.None;

    /// <summary>
    /// Gets whether to use styles based on the parameter or global option
    /// </summary>
    private bool ShouldUseStyles => UseStyles ?? Options?.Value.UseStyles ?? true;

    /// <summary>
    /// Gets the effective custom variables from parameter or options
    /// </summary>
    private string? EffectiveCustomVariables => CustomVariables ?? Options?.Value.CustomVariables;

    /// <summary>
    /// Gets the effective framework integration from parameter or options
    /// </summary>
    private CssFramework EffectiveFrameworkIntegration =>
        FrameworkIntegration != CssFramework.None ?
        FrameworkIntegration :
        Options?.Value.FrameworkIntegration ?? CssFramework.None;

    /// <summary>
    /// Gets the CSS class for framework integration
    /// </summary>
    private string? GetFrameworkIntegrationClass()
    {
        return EffectiveFrameworkIntegration switch
        {
            CssFramework.Bootstrap => "osirion-bootstrap-integration",
            CssFramework.Tailwind => "osirion-tailwind-integration",
            CssFramework.FluentUI => "osirion-fluent-integration",
            CssFramework.MudBlazor => "osirion-mudblazor-integration",
            CssFramework.Radzen => "osirion-radzen-integration",
            _ => null
        };
    }

    /// <summary>
    /// Initializes the component after render
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && EffectiveFrameworkIntegration != CssFramework.None)
        {
            var className = GetFrameworkIntegrationClass();
            if (!string.IsNullOrEmpty(className))
            {
                // Add the framework integration class to the document root element
                // This is done using JS interop to avoid issues with SSR
                ApplyFrameworkIntegrationClass(className);
            }
        }

        base.OnAfterRender(firstRender);
    }

    private void ApplyFrameworkIntegrationClass(string className)
    {
        // This is implemented in the component's razor file using script
        // to ensure it works with SSR by running only on the client
    }
}