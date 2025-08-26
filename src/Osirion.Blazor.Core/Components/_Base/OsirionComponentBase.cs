using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;


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
    /// In .NET 9+, this is determined by <c>RendererInfo?.IsInteractive</c>.
    /// In .NET 8, this is determined by the <see cref="SetInteractive"/> parameter.
    /// </remarks>
#if NET9_0_OR_GREATER
    [Obsolete("This parameter is ignored in .NET 9+ as interactivity is automatically determined.", true)]
#endif
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
    /// In .NET 9+, this is determined by <c>RendererInfo?.IsInteractive</c>.
    /// In .NET 8, this is determined by the <see cref="SetInteractive"/> parameter.
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

        // Determine interactivity based on framework version
#if NET9_0_OR_GREATER
        IsInteractive = RendererInfo?.IsInteractive ?? false;
#else
        IsInteractive = SetInteractive;
#endif
    }

    /// <summary>
    /// Returns a <see cref="RenderFragment"/> that loads a script from the specified source URL.
    /// </summary>
    /// <param name="src">The source URL of the script to load.</param>
    /// <returns>A <see cref="RenderFragment"/> that renders the script loader component.</returns>
    protected static RenderFragment LoadScript(string src) => builder =>
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