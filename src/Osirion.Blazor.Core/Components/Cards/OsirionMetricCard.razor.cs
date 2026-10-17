using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Displays a prominent metric with optional viewport-triggered count-up animation.</summary>
public partial class OsirionMetricCard : OsirionComponentBase
{
    /// <summary>
    /// Gets or sets the value as it is shown, such as <c>99.99%</c> or <c>18-32%</c>. Its number, or a range's two
    /// numbers, are what <see cref="Animate"/> counts up; the text around them stays. Required.
    /// </summary>
    [Parameter, EditorRequired]
    public string Value { get; set; } = string.Empty;

    /// <summary>Gets or sets the label under the value.</summary>
    [Parameter] public string? Label { get; set; }

    /// <summary>Gets or sets supporting text under the label.</summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the accent color: a theme color (<c>Primary</c>, <c>Secondary</c>, <c>Success</c>, <c>Warning</c>,
    /// <c>Danger</c>, <c>Info</c>) or a fixed one (<c>Blue</c>, <c>Teal</c>, <c>Green</c>, <c>Amber</c>, <c>Purple</c>,
    /// <c>Indigo</c>).
    /// </summary>
    [Parameter] public MetricCardAccent Accent { get; set; } = MetricCardAccent.Green;

    /// <summary>Gets or sets short text above the value.</summary>
    [Parameter] public string? Kicker { get; set; }

    /// <summary>Gets or sets the border: <c>None</c>, <c>Default</c> (the theme border) or <c>Accent</c>.</summary>
    [Parameter] public MetricCardBorder Border { get; set; } = MetricCardBorder.Accent;

    /// <summary>
    /// Gets or sets the shadow: <c>None</c>, <c>Small</c>, <c>Medium</c> or <c>Large</c>, or the same sizes tinted with
    /// the accent (<c>SmallAccent</c>, <c>MediumAccent</c>, <c>LargeAccent</c>). A card with a shadow lifts on hover.
    /// </summary>
    [Parameter] public MetricCardShadow Shadow { get; set; } = MetricCardShadow.Medium;

    /// <summary>
    /// Gets or sets whether the shadow is made large: <c>Large</c>, or <c>LargeAccent</c> for an accent shadow, whatever
    /// <see cref="Shadow"/> says.
    /// </summary>
    [Parameter] public bool Elevated { get; set; }

    /// <summary>Gets or sets extra CSS classes on the card, as <c>Class</c> does.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets whether the numbers count up from zero, once, when the card scrolls into view. The server renders
    /// the final value, so the card reads correctly before and without script; with reduced motion requested, nothing
    /// moves.
    /// </summary>
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
        // A dash belongs to the number only as a range separator, which always has a digit after it, so a trailing
        // dash stays in the suffix instead of turning "5-" into a 5 to 0 count-up range.
        while (end >= start && !char.IsDigit(value[end]) && value[end] != '.') end--;

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
