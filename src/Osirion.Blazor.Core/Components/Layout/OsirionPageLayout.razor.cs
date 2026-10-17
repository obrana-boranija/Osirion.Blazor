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
    /// Gets or sets the content of the <c>header</c> element. Without it no <c>header</c> is rendered.
    /// </summary>
    [Parameter]
    public RenderFragment? Header { get; set; }

    /// <summary>
    /// Gets or sets the content of the <c>main</c> element, which carries the id <see cref="MainContentId"/> and
    /// <c>tabindex="-1"</c> so the skip link can move focus to it.
    /// </summary>
    [Parameter]
    public RenderFragment? Body { get; set; }

    /// <summary>
    /// Gets or sets the content of the <c>footer</c> element. Without it no <c>footer</c> is rendered. An
    /// <c>OsirionFooter</c> placed here renders a <c>div</c>, so the page keeps one footer landmark.
    /// </summary>
    [Parameter]
    public RenderFragment? Footer { get; set; }

    /// <summary>
    /// Gets or sets whether the layout adds the class <c>osirion-sticky-footer-layout</c>, which pushes the footer to the
    /// bottom of the viewport when the content is short. The base layout is already a flex column whose <c>main</c>
    /// grows, so the footer also sits at the bottom with this set to <see langword="false"/>.
    /// </summary>
    [Parameter]
    public bool StickyFooter { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the header is sticky (class <c>osirion-page-header-scroll</c>, <c>z-index: 1000</c>) and
    /// slides out of view while the reader scrolls down, returning on scrolling up. The sliding is done by the
    /// layout script, which works in every render mode, including static SSR. It does nothing when the visitor
    /// prefers reduced motion, and the header stays while it is hovered or holds focus.
    /// </summary>
    [Parameter]
    public bool EnableHeaderScrollBehavior { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum height of the layout: <c>"viewport"</c> sets <c>min-height: 100vh</c> and
    /// <c>"content"</c> sets <c>min-height: 100%</c>. The value becomes the class <c>osirion-min-height-{value}</c>, so any
    /// other value adds a class the stylesheet does not define.
    /// </summary>
    [Parameter]
    public string MinHeightStrategy { get; set; } = "viewport";

    /// <summary>
    /// Gets or sets whether a skip link is rendered as the first focusable element, so keyboard and screen reader users
    /// can jump past the header and menu to the main content. The link is off screen until it receives focus.
    /// </summary>
    [Parameter]
    public bool ShowSkipLink { get; set; } = true;

    /// <summary>
    /// Gets or sets the text of the skip link. Set it for other languages.
    /// </summary>
    [Parameter]
    public string SkipLinkText { get; set; } = "Skip to main content";

    /// <summary>
    /// Gets or sets the id of the <c>main</c> element, which the skip link targets.
    /// </summary>
    [Parameter]
    public string MainContentId { get; set; } = "osirion-main-content";

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    /// <summary>
    /// Gets the skip link's address: the current page, relative to the document's base address, with the main
    /// element's fragment. A bare "#id" would resolve against the base, which Blazor apps set to "/", and so lead to
    /// the home page. The address is relative so that HTML rendered on one host (a static export) works on another.
    /// </summary>
    private string SkipLinkHref
    {
        get
        {
            var current = Navigation.ToBaseRelativePath(Navigation.Uri);
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
