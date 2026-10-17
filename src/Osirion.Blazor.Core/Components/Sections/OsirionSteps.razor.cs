using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Components;

/// <summary>
/// Numbered steps for a "how it works" section: side by side on wide screens, stacked on narrow ones or when
/// <see cref="Vertical"/> is set. It renders an ordered list without script.
/// </summary>
public partial class OsirionSteps
{
    /// <summary>
    /// The steps, in order. Required; with no steps nothing is rendered.
    /// </summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<StepItem> Items { get; set; } = [];

    /// <summary>
    /// Whether the steps are always stacked, with a line joining their numbers; otherwise they sit side by side on
    /// wide screens.
    /// </summary>
    [Parameter]
    public bool Vertical { get; set; }

    private string GetStepsClass()
    {
        var classes = Vertical ? "osirion-steps osirion-steps-vertical" : "osirion-steps";
        return string.IsNullOrWhiteSpace(Class) ? classes : $"{classes} {Class}";
    }
}
