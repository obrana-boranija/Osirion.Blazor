// MenuGroup.razor.cs
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

    /// <summary>
    /// Gets the CSS class for the menu group.
    /// </summary>
    protected string GetMenuGroupCssClass()
    {
        var classes = new List<string> { "osirion-menu-group" };

        if (!string.IsNullOrEmpty(CssClass))
            classes.Add(CssClass);

        if (Collapsible)
            classes.Add("osirion-menu-group-collapsible");

        if (Collapsible && Expanded)
            classes.Add("osirion-menu-group-expanded");

        return string.Join(" ", classes);
    }

    /// <summary>
    /// Gets the CSS class for the group items container.
    /// </summary>
    protected string GetMenuGroupItemsCssClass()
    {
        return "osirion-menu-group-items";
    }

    /// <summary>
    /// Gets additional attributes for the menu group.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (AdditionalAttributes == null)
        {
            AdditionalAttributes = new Dictionary<string, object>();
        }

        // Set ID attribute
        if (!AdditionalAttributes.ContainsKey("id"))
            AdditionalAttributes["id"] = GroupId;

        // Add aria attributes for accessibility
        if (!string.IsNullOrEmpty(Label) && !AdditionalAttributes.ContainsKey("aria-labelledby"))
            AdditionalAttributes["aria-labelledby"] = LabelId;
    }
}