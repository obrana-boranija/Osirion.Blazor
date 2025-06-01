using Microsoft.AspNetCore.Components;

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

        if (!string.IsNullOrWhiteSpace(CssClass))
        {
            classes.Add(CssClass);
        }

        return string.Join(" ", classes);
    }
}