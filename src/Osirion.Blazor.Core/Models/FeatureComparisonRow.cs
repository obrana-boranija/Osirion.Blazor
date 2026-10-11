namespace Osirion.Blazor.Core.Models;

/// <summary>
/// A row of <see cref="Osirion.Blazor.Components.OsirionFeatureComparison"/>: a feature and its value in each plan.
/// </summary>
/// <param name="Feature">The feature, shown as the row's header.</param>
/// <param name="Values">One value per plan, in the order of the table's plans.</param>
public sealed record FeatureComparisonRow(string Feature, IReadOnlyList<ComparisonCell> Values);

/// <summary>
/// A value in <see cref="FeatureComparisonRow"/>: included, not included, or text such as <c>10 GB</c>. A
/// <see langword="bool"/> or <see langword="string"/> converts to it, so a row can be written as
/// <c>new("SSO", [false, true, "Add-on"])</c>.
/// </summary>
/// <param name="Included">Whether the feature is included, when the cell is a yes or no.</param>
/// <param name="Text">The text, when the cell is a value.</param>
public readonly record struct ComparisonCell(bool? Included, string? Text)
{
    /// <summary>Gets a cell for an included feature.</summary>
    public static ComparisonCell Yes => new(true, null);

    /// <summary>Gets a cell for a feature that is not included.</summary>
    public static ComparisonCell No => new(false, null);

    /// <summary>Converts a yes or no into a cell.</summary>
    public static implicit operator ComparisonCell(bool included) => new(included, null);

    /// <summary>Converts text into a cell.</summary>
    public static implicit operator ComparisonCell(string text) => new(null, text);
}
