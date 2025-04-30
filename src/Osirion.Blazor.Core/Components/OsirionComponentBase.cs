using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Osirion.Blazor.Components;

/// <summary>
/// Base component class that provides common functionality for all Osirion.Blazor components.
/// </summary>
public abstract class OsirionComponentBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the CSS class(es) for the component.
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets additional attributes that will be applied to the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets or sets whether the component should be considered interactive.
    /// </summary>
    /// <remarks>
    /// Only applicable in .NET 8. In .NET 9+, interactivity is automatically determined 
    /// via <see cref="RendererInfo.IsInteractive"/>.
    /// </remarks>
#if NET9_0_OR_GREATER
    [Obsolete("This parameter is ignored in .NET 9+ as interactivity is automatically determined.", true)]
#endif
    [Parameter]
    public bool SetInteractive { get; set; }

    /// <summary>
    /// Indicates whether the component is being rendered on the server rather than WebAssembly.
    /// </summary>
    protected bool IsServerSide => !OperatingSystem.IsBrowser();

    /// <summary>
    /// Indicates whether the component is in an interactive rendering mode.
    /// </summary>
    /// <remarks>
    /// In .NET 9+, this is determined by <see cref="RendererInfo.IsInteractive"/>.
    /// In .NET 8, this is determined by the <see cref="SetInteractive"/> parameter.
    /// </remarks>
    protected bool IsInteractive { get; private set; }

    /// <summary>
    /// Initializes the component and determines its interactivity mode.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Determine interactivity based on framework version
#if NET9_0_OR_GREATER
        IsInteractive = RendererInfo?.IsInteractive ?? false;
#else
        IsInteractive = SetInteractive;
#endif
    }

    /// <summary>
    /// Combines the base CSS class with any additional classes provided.
    /// </summary>
    /// <param name="baseClass">The base CSS class for the component</param>
    /// <returns>Combined CSS class string</returns>
    protected string CombineCssClasses(string baseClass)
    {
        return string.IsNullOrWhiteSpace(CssClass)
            ? baseClass
            : $"{baseClass} {CssClass}";
    }
}