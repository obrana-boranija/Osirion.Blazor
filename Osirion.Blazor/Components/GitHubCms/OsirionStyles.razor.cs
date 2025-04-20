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
    /// The options for Osirion CMS
    /// </summary>
    [Inject]
    protected IOptions<GitHubCmsOptions>? Options { get; set; }

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
    /// Gets whether to use styles based on the parameter or global option
    /// </summary>
    private bool ShouldUseStyles => UseStyles ?? Options?.Value.UseStyles ?? true;

    /// <summary>
    /// Gets the effective custom variables from parameter or options
    /// </summary>
    private string? EffectiveCustomVariables => CustomVariables ?? Options?.Value.CustomVariables;
}