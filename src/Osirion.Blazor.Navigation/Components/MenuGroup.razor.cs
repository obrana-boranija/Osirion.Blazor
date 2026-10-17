using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Internal;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>
/// Groups menu items in a submenu column, with an optional label, description and footer link. Use it inside a
/// <c>Mega</c> submenu to build its columns.
/// </summary>
public partial class MenuGroup
{
    /// <summary>
    /// Gets or sets the heading of the group. Without it the group has no heading and the items are not labelled.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets supporting text under the label, which describes the column in a mega submenu.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the text of the footer link, for example <c>All features</c>. The link is rendered only when
    /// <see cref="FooterHref"/> also gives a safe address.
    /// </summary>
    [Parameter]
    public string? FooterText { get; set; }

    /// <summary>
    /// Gets or sets the address of the footer link. A scheme other than a relative URL, <c>http</c>, <c>https</c>,
    /// <c>mailto</c> or <c>tel</c> leaves the footer link out.
    /// </summary>
    [Parameter]
    public string? FooterHref { get; set; }

    /// <summary>
    /// Gets or sets the <c>target</c> attribute of the footer link. <c>_blank</c> also adds
    /// <c>rel="noopener noreferrer"</c>.
    /// </summary>
    [Parameter]
    public string? FooterTarget { get; set; }

    /// <summary>
    /// Gets or sets whether the group is styled as collapsible: its label gets a chevron and, with
    /// <see cref="Expanded"/> off, its items are hidden. Nothing in the package opens a collapsed group again, so
    /// leave it off unless your own script toggles the <c>osirion-menu-group-expanded</c> class.
    /// </summary>
    [Parameter]
    public bool Collapsible { get; set; }

    /// <summary>
    /// Gets or sets whether a collapsible group shows its items. It has no effect unless <see cref="Collapsible"/> is
    /// on.
    /// </summary>
    [Parameter]
    public bool Expanded { get; set; } = true;

    /// <summary>
    /// Gets or sets the items of the group, rendered in a <c>role="group"</c> container.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the id of the group; the ids of its label and item container derive from it. Without it a unique
    /// id is generated.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    // Generated once per instance: the label id, the items id and aria-labelledby
    // must all derive from the same value, on every render.
    private readonly string _generatedId = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Gets the unique identifier for the menu group.
    /// </summary>
    private string GroupId => Id ?? $"osirion-menu-group-{_generatedId}";

    /// <summary>
    /// Gets the ID for the label element.
    /// </summary>
    protected string LabelId => $"{GroupId}-label";

    /// <summary>
    /// Gets the ID for the group items container.
    /// </summary>
    protected string ItemsId => $"{GroupId}-items";

    // Derived render state, rebuilt on every parameter update and never written back into a parameter,
    // so a parent re-render cannot wipe it or append to it twice.
    private string _groupClass = "osirion-menu-group";
    private Dictionary<string, object> _groupAttributes = [];

    private string? _footerHref;

    /// <summary>Recomputes the group classes, attributes and the footer link from the current parameters.</summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var cssClasses = new List<string> { "osirion-menu-group" };

        if (Collapsible)
            cssClasses.Add("osirion-menu-group-collapsible");

        if (Collapsible && Expanded)
            cssClasses.Add("osirion-menu-group-expanded");

        if (!string.IsNullOrWhiteSpace(Class))
            cssClasses.Add(Class);

        _groupClass = string.Join(' ', cssClasses);

        var attributes = new Dictionary<string, object>(Attributes ?? new Dictionary<string, object>());
        if (!attributes.ContainsKey("id"))
            attributes["id"] = GroupId;

        _groupAttributes = attributes;
        _footerHref = SafeHref.Sanitize(FooterHref);
    }
}
