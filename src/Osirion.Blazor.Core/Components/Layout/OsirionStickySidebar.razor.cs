using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Sticky Sidebar component for creating sidebars that stick below the header
/// </summary>
public partial class OsirionStickySidebar
{
    /// <summary>
    /// Gets or sets the content to display in the sidebar
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the offset from the top of the viewport (in pixels). Default 2.5rem
    /// </summary>
    [Parameter]
    public int TopOffset { get; set; } = (int)(2.5 * 16);

    /// <summary>
    /// Gets or sets whether to hide the scrollbar
    /// </summary>
    [Parameter]
    public bool HideScrollbar { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the sidebar should stick to the top
    /// </summary>
    [Parameter]
    public bool IsSticky { get; set; } = true;

    /// <summary>
    /// Gets the CSS variable declaration for the top offset
    /// </summary>
    private string TopOffsetStyle => $"--osirion-header-height: {TopOffset}px;";

    /// <summary>
    /// Gets the CSS class for the sidebar
    /// </summary>
    private string GetSidebarClass()
    {
        var baseClass = "osirion-sticky-sidebar";

        if (!IsSticky)
        {
            baseClass += " osirion-non-sticky";
        }

        if (HideScrollbar)
        {
            baseClass += " osirion-no-scrollbar";
        }

        if (!string.IsNullOrEmpty(CssClass))
        {
            baseClass += $" {CssClass}";
        }

        return baseClass;
    }
}
