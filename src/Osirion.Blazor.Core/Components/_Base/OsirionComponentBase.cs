using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using BlazorJSComponents;

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
    public IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the component theme: "Light", "Dark", "System". Defaults "System".
    /// </summary>
    [Parameter]
    public ThemeMode Theme { get; set; } = ThemeMode.System;

    /// <summary>
    /// Gets or sets whether the component should be considered interactive.
    /// </summary>
    /// <remarks>
    /// This is determined by <c>RendererInfo?.IsInteractive</c>.
    /// </remarks>
    [Obsolete("This parameter is ignored in .NET 9+ as interactivity is automatically determined.", true)]
    [Parameter]
    public bool SetInteractive { get; set; }

    [Inject]
    private IOptions<ThemingOptions> ThemingOptions { get; set; } = default!;

    /// <summary>
    /// Indicates whether the component is being rendered on the server rather than WebAssembly.
    /// </summary>
    protected bool IsServerSide => !OperatingSystem.IsBrowser();

    /// <summary>
    /// Indicates whether the component is in an interactive rendering mode.
    /// </summary>
    /// <remarks>
    /// This is determined by <c>RendererInfo?.IsInteractive</c>.
    /// </remarks>
    protected bool IsInteractive { get; private set; }

    /// <summary>
    /// Gets the current theme mode.
    /// </summary>
    protected CssFramework Framework => ThemingOptions.Value.Framework;

    /// <summary>
    /// Initializes the component and determines its interactivity mode.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Theme != ThemeMode.System)
        {
            Attributes[Framework == CssFramework.Bootstrap ? "data-bs-theme" : "data-theme"] = Theme.ToString().ToLower();
        }

        IsInteractive = RendererInfo?.IsInteractive ?? false;
    }

    /// <summary>Creates a render fragment that loads a client-side script.</summary>
    protected RenderFragment LoadScript(string src) => builder =>
    {
        builder.OpenComponent<JS>(0);
        builder.AddAttribute(1, "Src", src);
        builder.CloseComponent();
    };
}
