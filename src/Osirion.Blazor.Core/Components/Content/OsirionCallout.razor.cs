using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// A highlighted note, tip, important point, warning or caution inside content. It renders without script and shares
/// its look with GitHub-style alerts written in Markdown (<c>&gt; [!WARNING]</c>), which the CMS renders.
/// </summary>
public partial class OsirionCallout
{
    /// <summary>
    /// Gets or sets the kind of callout: <c>Note</c>, <c>Tip</c>, <c>Important</c>, <c>Warning</c> or
    /// <c>Caution</c>. It sets the color, the icon and the default <see cref="Title"/>.
    /// </summary>
    [Parameter]
    public CalloutType Type { get; set; } = CalloutType.Note;

    /// <summary>
    /// Gets or sets the heading shown above the content. When empty or white space, the name of the
    /// <see cref="Type"/> is shown ("Note", "Warning" and so on), as Markdown alerts show it; set it for other
    /// languages or a more specific heading.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>Gets or sets the callout's content, rendered in the body under the title.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string EffectiveTitle => string.IsNullOrWhiteSpace(Title) ? Type.ToString() : Title;

    private string GetCalloutClass()
    {
        var classes = new List<string> { "osirion-callout", $"osirion-callout-{Type.ToString().ToLowerInvariant()}" };

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }
}
