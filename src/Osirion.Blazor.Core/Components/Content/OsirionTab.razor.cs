using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// One tab of an <see cref="OsirionTabs"/>: its label and its panel.
/// </summary>
public partial class OsirionTab
{
    private int _index;

    [CascadingParameter]
    private OsirionTabs? Parent { get; set; }

    /// <summary>
    /// Gets or sets the tab's label.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the panel content shown while the tab is selected.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string InputId => $"{Parent!.GroupId}-{_index}";
    private string LabelId => $"{InputId}-label";
    private string PanelId => $"{InputId}-panel";
    private bool IsSelected => _index == Parent!.SelectedIndex;

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">The tab is not inside an <see cref="OsirionTabs"/>.</exception>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Parent is null)
        {
            throw new InvalidOperationException($"{nameof(OsirionTab)} must be placed inside {nameof(OsirionTabs)}.");
        }

        _index = Parent.RegisterTab();
    }

    private string GetPanelClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-tabs-panel" : $"osirion-tabs-panel {Class}";
}
