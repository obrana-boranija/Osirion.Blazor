using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Navigation.Components;

public partial class MenuGroup
{
    /// <summary>
    /// Gets or sets the group label.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

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

    /// <summary>
    /// Gets the unique identifier for the menu group.
    /// </summary>
    private string GroupId => Id ?? $"osirion-menu-group-{Guid.NewGuid():N}";

    /// <summary>
    /// Gets the ID for the label element.
    /// </summary>
    protected string LabelId => $"{GroupId}-label";

    /// <summary>
    /// Gets the ID for the group items container.
    /// </summary>
    protected string ItemsId => $"{GroupId}-items";

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