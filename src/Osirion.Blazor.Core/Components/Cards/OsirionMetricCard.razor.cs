using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Displays a prominent metric with optional viewport-triggered count-up animation.</summary>
public partial class OsirionMetricCard : OsirionComponentBase
{
    /// <summary>Gets or sets the complete display value, such as 99.99% or 18-32%.</summary>
    [Parameter, EditorRequired]
    public string Value { get; set; } = string.Empty;

    /// <summary>Gets or sets the label below the value.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Gets or sets supporting text below the label.</summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>Gets or sets the accent token used by the component styles.</summary>
    [Parameter] public string Accent { get; set; } = "green";

    /// <summary>Gets or sets optional text above the metric.</summary>
    [Parameter] public string? Kicker { get; set; }

    /// <summary>Gets or sets whether the card receives an elevated shadow.</summary>
    [Parameter] public bool Elevated { get; set; }

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Gets or sets whether the numeric portion animates into view.</summary>
    [Parameter] public bool Animate { get; set; }

    private string NumericValue { get; set; } = string.Empty;
    private string Prefix { get; set; } = string.Empty;
    private string Suffix { get; set; } = string.Empty;

    protected override void OnParametersSet()
    {
        var value = Value.Trim();
        var start = 0;
        while (start < value.Length && !char.IsDigit(value[start]) && value[start] != '.') start++;
        var end = value.Length - 1;
        while (end >= start && !char.IsDigit(value[end]) && value[end] != '.' && value[end] != '-' && value[end] != '\u2013') end--;

        Prefix = value[..start];
        NumericValue = start <= end ? value[start..(end + 1)] : string.Empty;
        Suffix = end + 1 < value.Length ? value[(end + 1)..] : string.Empty;
    }
}