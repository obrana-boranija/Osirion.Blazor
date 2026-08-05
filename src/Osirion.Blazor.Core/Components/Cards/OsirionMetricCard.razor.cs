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

    /// <summary>Gets or sets the accent used by the component styles.</summary>
    [Parameter] public MetricCardAccent Accent { get; set; } = MetricCardAccent.Green;

    /// <summary>Gets or sets optional text above the metric.</summary>
    [Parameter] public string? Kicker { get; set; }

    /// <summary>Gets or sets the card border treatment.</summary>
    [Parameter] public MetricCardBorder Border { get; set; } = MetricCardBorder.Accent;

    /// <summary>Gets or sets the card shadow treatment.</summary>
    [Parameter] public MetricCardShadow Shadow { get; set; } = MetricCardShadow.Medium;

    /// <summary>
    /// Gets or sets whether to upgrade shadows smaller than <see cref="MetricCardShadow.Large" /> to a large neutral shadow.
    /// This legacy convenience parameter never downgrades an explicitly selected shadow.
    /// </summary>
    [Parameter] public bool Elevated { get; set; }

    /// <summary>Gets or sets additional CSS classes.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Gets or sets whether the numeric portion animates into view.</summary>
    [Parameter] public bool Animate { get; set; }

    private string NumericValue { get; set; } = string.Empty;
    private string Prefix { get; set; } = string.Empty;
    private string Suffix { get; set; } = string.Empty;
    private MetricCardShadow EffectiveShadow => !Elevated ? Shadow : Shadow switch
    {
        MetricCardShadow.SmallAccent or MetricCardShadow.MediumAccent or MetricCardShadow.LargeAccent => MetricCardShadow.LargeAccent,
        _ => MetricCardShadow.Large
    };

    /// <inheritdoc />
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

/// <summary>Defines the available semantic and visual accent styles for <see cref="OsirionMetricCard" />.</summary>
public enum MetricCardAccent
{
    /// <summary>Uses the primary theme color.</summary>
    Primary,

    /// <summary>Uses the neutral secondary theme color.</summary>
    Secondary,

    /// <summary>Uses the success theme color.</summary>
    Success,

    /// <summary>Uses the warning theme color.</summary>
    Warning,

    /// <summary>Uses the danger theme color.</summary>
    Danger,

    /// <summary>Uses the information theme color.</summary>
    Info,

    /// <summary>Uses the fixed blue visual accent, independent of the host primary color.</summary>
    Blue,

    /// <summary>Uses the fixed teal visual accent, independent of the host information color.</summary>
    Teal,

    /// <summary>Uses the fixed green visual accent, independent of the host success color.</summary>
    Green,

    /// <summary>Uses the fixed amber visual accent, independent of the host warning color.</summary>
    Amber,

    /// <summary>Uses the fixed purple visual accent.</summary>
    Purple,

    /// <summary>Uses the fixed indigo visual accent.</summary>
    Indigo
}

/// <summary>Defines border treatments for <see cref="OsirionMetricCard" />.</summary>
public enum MetricCardBorder
{
    /// <summary>Renders no border.</summary>
    None,

    /// <summary>Renders the standard theme border.</summary>
    Default,

    /// <summary>Renders a border derived from the selected accent.</summary>
    Accent
}

/// <summary>Defines shadow treatments for <see cref="OsirionMetricCard" />.</summary>
public enum MetricCardShadow
{
    /// <summary>Renders no shadow.</summary>
    None,

    /// <summary>Renders a small shadow.</summary>
    Small,

    /// <summary>Renders a medium shadow.</summary>
    Medium,

    /// <summary>Renders a large shadow.</summary>
    Large,

    /// <summary>Renders a small shadow tinted with the selected accent.</summary>
    SmallAccent,

    /// <summary>Renders a medium shadow tinted with the selected accent.</summary>
    MediumAccent,

    /// <summary>Renders a large shadow tinted with the selected accent.</summary>
    LargeAccent
}
