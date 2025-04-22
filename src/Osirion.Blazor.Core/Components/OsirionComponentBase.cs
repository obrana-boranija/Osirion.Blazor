using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Base component class for all Osirion.Blazor components
/// </summary>
public abstract class OsirionComponentBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the CSS class for the component
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets additional attributes that will be applied to the component
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Determines whether the component is in an interactive rendering mode.
    /// </summary>
    protected bool IsInteractive => OperatingSystem.IsBrowser();

    /// <summary>
    /// Determines whether the component is being rendered on the server.
    /// </summary>
    protected bool IsServerSide => !OperatingSystem.IsBrowser();
}