using Microsoft.AspNetCore.Components;

#if NET9_0_OR_GREATER
using BlazorJSComponents;
#endif

namespace Osirion.Blazor.Components;

/// <summary>
/// Page layout component that provides a flexible layout structure with sticky footer support
/// </summary>
public partial class OsirionPageLayout
{
    /// <summary>
    /// Gets or sets the header content
    /// </summary>
    [Parameter]
    public RenderFragment? Header { get; set; }

    /// <summary>
    /// Gets or sets the main body content
    /// </summary>
    [Parameter]
    public RenderFragment? Body { get; set; }

    /// <summary>
    /// Gets or sets the footer content
    /// </summary>
    [Parameter]
    public RenderFragment? Footer { get; set; }

    /// <summary>
    /// Gets or sets whether to use sticky footer layout
    /// </summary>
    [Parameter]
    public bool StickyFooter { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the header hides while scrolling down and reappears while scrolling up.
    /// This enhancement is available for .NET 9 and later interactive render modes.
    /// </summary>
    [Parameter]
    public bool EnableHeaderScrollBehavior { get; set; }

    /// <summary>
    /// Gets or sets the minimum height strategy: "viewport" or "content"
    /// </summary>
    [Parameter]
    public string MinHeightStrategy { get; set; } = "viewport";

    /// <summary>
    /// Gets the CSS class for the page layout
    /// </summary>
    private string GetPageLayoutClass()
    {
        var classes = new List<string> { "osirion-page-layout" };

        if (StickyFooter)
        {
            classes.Add("osirion-sticky-footer-layout");
        }

        classes.Add($"osirion-min-height-{MinHeightStrategy}");

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }

    protected RenderFragment HeaderScrollScript => builder =>
    {
#if NET9_0_OR_GREATER
        if (EnableHeaderScrollBehavior && Header is not null)
        {
            builder.OpenComponent<JS>(0);
            builder.AddAttribute(1, "Src", "./_content/Osirion.Blazor.Core/Components/Layout/OsirionPageLayout.razor.js");
            builder.CloseComponent();
        }
#endif
    };
}