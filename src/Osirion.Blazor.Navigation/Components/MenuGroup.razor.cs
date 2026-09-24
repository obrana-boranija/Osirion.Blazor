using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

/// <summary>Defines the MenuGroup type.</summary>
public partial class MenuGroup
{
    /// <summary>
    /// Gets or sets the group label.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets optional supporting text rendered below the label.
    /// Used by mega menus to describe the column, Personio-style.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets optional footer link text (for example "Explore Core HR").
    /// Rendered at the bottom of the group when <see cref="FooterHref"/> is also set.
    /// </summary>
    [Parameter]
    public string? FooterText { get; set; }

    /// <summary>
    /// Gets or sets the destination of the footer link.
    /// </summary>
    [Parameter]
    public string? FooterHref { get; set; }

    /// <summary>
    /// Gets or sets how to open the footer link destination.
    /// </summary>
    [Parameter]
    public string? FooterTarget { get; set; }

    /// <summary>
    /// Gets or sets whether the group is collapsible.
    /// Only applies to vertical menus.
    /// </summary>
    [Parameter]
    public bool Collapsible { get; set; }

    /// <summary>
    /// Gets or sets whether the group is expanded by default.
    /// Only applies when Collapsible is true.
    /// </summary>
    [Parameter]
    public bool Expanded { get; set; } = true;

    /// <summary>
    /// Gets or sets the menu items to be displayed.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the group identifier.
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

    /// <summary>Initializes the component state and required services.</summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Attributes is null)
        {
            Attributes = new Dictionary<string, object>();
        }

        // Set ID attribute
        if (!Attributes.ContainsKey("id"))
            Attributes["id"] = GroupId;

        // Add CSS classes for state
        var cssClasses = new List<string>();

        if (Collapsible)
            cssClasses.Add("osirion-menu-group-collapsible");

        if (Collapsible && Expanded)
            cssClasses.Add("osirion-menu-group-expanded");

        if (!string.IsNullOrWhiteSpace(Class))
            cssClasses.Add(Class);

        Class = string.Join(" ", cssClasses);
    }
}
