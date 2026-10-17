using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Tabs that switch without script: each <see cref="OsirionTab"/> is a radio button, its label and a panel, so they
/// work under static server-side rendering and are keyboard accessible (the arrow keys move between tabs).
/// </summary>
public partial class OsirionTabs
{
    // Generated once per instance so the ids stay the same on every render.
    private readonly string _generatedId = $"osirion-tabs-{Guid.NewGuid():N}";
    private int _registeredTabs;

    /// <summary>
    /// Gets or sets the <see cref="OsirionTab"/> components. Each tab takes its position from the order in which
    /// the tabs render.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the zero-based index of the tab selected when the page renders. An index with no tab leaves
    /// every panel hidden. It sets only the starting choice: the visitor's later choices are not written back.
    /// </summary>
    [Parameter]
    public int SelectedIndex { get; set; }

    /// <summary>
    /// Gets or sets the radio group name, also the prefix of each tab's ids: <c>{Id}-0</c> for the radio button,
    /// <c>{Id}-0-label</c> for the label and <c>{Id}-0-panel</c> for the panel (then <c>-1</c> and so on). Defaults
    /// to a value generated for each instance; set it for stable ids, for example to link to a tab, and use a
    /// different value for each tab set on a page.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the accessible name of the tab group, for example "Package manager". Without it the group has
    /// no name.
    /// </summary>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>Gets the radio group name and id prefix shared by the tabs.</summary>
    internal string GroupId => string.IsNullOrWhiteSpace(Id) ? _generatedId : Id;

    /// <summary>Gives a tab its position, in the order the tabs are first rendered.</summary>
    internal int RegisterTab() => _registeredTabs++;

    private string GetContainerClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-tabs" : $"osirion-tabs {Class}";
}
