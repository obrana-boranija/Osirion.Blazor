using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Components;

/// <summary>
/// A table comparing features across plans, with check and cross marks that screen readers read as "Included" and
/// "Not included". It scrolls sideways on narrow screens and renders without script.
/// </summary>
public partial class OsirionFeatureComparison
{
    /// <summary>
    /// The plan names, one column each. Required; with no plan names or no rows nothing is rendered.
    /// </summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<string> PlanNames { get; set; } = [];

    /// <summary>
    /// The features, one row each, with a value per plan in the order of <see cref="PlanNames"/>. Missing values leave
    /// the cell empty. Required.
    /// </summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<FeatureComparisonRow> Rows { get; set; } = [];

    /// <summary>
    /// The table's caption, which also names the scrollable region.
    /// </summary>
    [Parameter]
    public string Caption { get; set; } = "Compare plans";

    /// <summary>
    /// Whether the caption is shown; otherwise only screen readers announce it.
    /// </summary>
    [Parameter]
    public bool ShowCaption { get; set; }

    /// <summary>
    /// The header of the feature column, read by screen readers only.
    /// </summary>
    [Parameter]
    public string FeatureColumnText { get; set; } = "Feature";

    /// <summary>
    /// What screen readers say for a check mark.
    /// </summary>
    [Parameter]
    public string IncludedText { get; set; } = "Included";

    /// <summary>
    /// What screen readers say for a dash.
    /// </summary>
    [Parameter]
    public string NotIncludedText { get; set; } = "Not included";

    private string GetWrapperClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-feature-comparison" : $"osirion-feature-comparison {Class}";
}
