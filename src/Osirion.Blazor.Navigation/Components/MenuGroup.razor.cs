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
    /// Gets or sets the menu items to be displayed.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets the ID for the label element.
    /// </summary>
    protected string LabelId { get; } = $"menu-group-{Guid.NewGuid():N}";

    private string? GetMenuGroupItemsCssClass()
    {
        // Fix for CS0029: Return a string instead of a Dictionary.
        // Fix for CS8603: Ensure nullability is handled correctly.
        return string.IsNullOrEmpty(Label) ? null : $"aria-labelledby-{LabelId}";
    }
}
