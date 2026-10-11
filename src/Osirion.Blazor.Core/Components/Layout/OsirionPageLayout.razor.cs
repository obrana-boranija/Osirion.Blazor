using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Page layout component that provides a flexible layout structure with sticky footer support
/// </summary>
public partial class OsirionPageLayout
{
    /// <summary>
    /// The name of the cascading value that tells content in the footer slot the layout already renders the
    /// <c>&lt;footer&gt;</c> landmark.
    /// </summary>
    internal const string FooterSlotCascadeName = "OsirionPageLayout.FooterSlot";

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
    /// This enhancement is available in interactive render modes.
    /// </summary>
    [Parameter]
    public bool EnableHeaderScrollBehavior { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum height strategy: "viewport" or "content"
    /// </summary>
    [Parameter]
    public string MinHeightStrategy { get; set; } = "viewport";

    /// <summary>
    /// Gets or sets whether a skip link is rendered as the first focusable element, so keyboard and screen reader
    /// users can jump past the header and menu to the main content. Defaults to <see langword="true"/>.
    /// </summary>
    [Parameter]
    public bool ShowSkipLink { get; set; } = true;

    /// <summary>
    /// Gets or sets the text of the skip link. Defaults to "Skip to main content"; set it for other languages.
    /// </summary>
    [Parameter]
    public string SkipLinkText { get; set; } = "Skip to main content";

    /// <summary>
    /// Gets or sets the id of the main element, which the skip link targets. Defaults to "osirion-main-content".
    /// </summary>
    [Parameter]
    public string MainContentId { get; set; } = "osirion-main-content";

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    /// <summary>
    /// Gets the skip link's address: the current page with the main element's fragment. A bare "#id" would resolve
    /// against the document's base address, which Blazor apps set to "/", and so lead to the home page.
    /// </summary>
    private string SkipLinkHref
    {
        get
        {
            var current = Navigation.Uri;
            var fragmentStart = current.IndexOf('#');
            var withoutFragment = fragmentStart < 0 ? current : current[..fragmentStart];
            return $"{withoutFragment}#{MainContentId}";
        }
    }

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

}
