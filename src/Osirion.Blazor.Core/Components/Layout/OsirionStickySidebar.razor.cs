using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Sticky Sidebar component for creating sidebars that stick below the header
/// </summary>
public partial class OsirionStickySidebar
{
    /// <summary>
    /// Gets or sets the content of the <c>aside</c> element.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the intended offset from the top of the viewport, in pixels (default 40, which is 2.5rem). The
    /// value is not written to the markup, so it has no effect; set the CSS custom property
    /// <c>--osirion-header-height</c> instead.
    /// </summary>
    [Parameter]
    public int TopOffset { get; set; } = (int)(2.5 * 16);

    /// <summary>
    /// Gets or sets whether the scrollbar is hidden (class <c>osirion-no-scrollbar</c>). The sidebar still scrolls with
    /// the wheel, touch and keyboard.
    /// </summary>
    [Parameter]
    public bool HideScrollbar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the sidebar sticks below the header. <see langword="false"/> adds <c>osirion-non-sticky</c>,
    /// which renders a normal, static <c>aside</c> with its natural height.
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

        if (!string.IsNullOrWhiteSpace(Class))
        {
            baseClass += $" {Class}";
        }

        return baseClass;
    }
}
