using Microsoft.AspNetCore.Components;

#if NET9_0_OR_GREATER
using BlazorJSComponents;
#else
using BlazorPageScript;
#endif

namespace Osirion.Blazor.Components;

/// <summary>
/// Base component class that provides common functionality for all Osirion.Blazor components.
/// </summary>
public abstract partial class OsirionComponentBase : ComponentBase
{
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

    protected RenderFragment LoadScript(string src) => builder =>
    {
#if NET9_0_OR_GREATER
        builder.OpenComponent<JS>(0);
        builder.AddAttribute(1, "Src", src);
        builder.CloseComponent();
#else
        builder.OpenComponent<PageScript>(0);
        builder.AddAttribute(1, "Src", src);
        builder.CloseComponent();
#endif
    };
}