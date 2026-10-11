using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Components;

/// <summary>
/// A table comparing features across plans, with check and cross marks that screen readers read as "Included" and
/// "Not included". It scrolls sideways on narrow screens and renders without script.
/// </summary>
public partial class OsirionFeatureComparison
{
    /// <summary>Gets or sets the plan names, one column each.</summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<string> PlanNames { get; set; } = [];

    /// <summary>Gets or sets the features, one row each, with a value per plan. Missing values are left empty.</summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<FeatureComparisonRow> Rows { get; set; } = [];

    /// <summary>
    /// Gets or sets the table's caption, which also names the scrollable region. Defaults to <c>Compare plans</c>.
    /// </summary>
    [Parameter]
    public string Caption { get; set; } = "Compare plans";

    /// <summary>Gets or sets whether the caption is shown; otherwise only screen readers announce it.</summary>
    [Parameter]
    public bool ShowCaption { get; set; }

    /// <summary>Gets or sets the header of the feature column, read by screen readers. Defaults to <c>Feature</c>.</summary>
    [Parameter]
    public string FeatureColumnText { get; set; } = "Feature";

    /// <summary>Gets or sets what screen readers say for a check mark. Defaults to <c>Included</c>.</summary>
    [Parameter]
    public string IncludedText { get; set; } = "Included";

    /// <summary>Gets or sets what screen readers say for a cross. Defaults to <c>Not included</c>.</summary>
    [Parameter]
    public string NotIncludedText { get; set; } = "Not included";

    private string GetWrapperClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-feature-comparison" : $"osirion-feature-comparison {Class}";
}
