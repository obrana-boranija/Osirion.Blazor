using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Core.Models;

namespace Osirion.Blazor.Components;

/// <summary>
/// Numbered steps for a "how it works" section: side by side on wide screens, stacked on narrow ones or when
/// <see cref="Vertical"/> is set. It renders an ordered list without script.
/// </summary>
public partial class OsirionSteps
{
    /// <summary>Gets or sets the steps, in order.</summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<StepItem> Items { get; set; } = [];

    /// <summary>Gets or sets whether the steps are always stacked, with a line joining their numbers.</summary>
    [Parameter]
    public bool Vertical { get; set; }

    private string GetStepsClass()
    {
        var classes = Vertical ? "osirion-steps osirion-steps-vertical" : "osirion-steps";
        return string.IsNullOrWhiteSpace(Class) ? classes : $"{classes} {Class}";
    }
}
