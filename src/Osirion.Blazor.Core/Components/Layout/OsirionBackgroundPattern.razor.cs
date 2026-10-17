using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Renders a decorative background pattern wrapper.</summary>
public partial class OsirionBackgroundPattern
{
    /// <summary>
    /// Gets or sets the pattern to draw, as the class <c>osirion-bg-{pattern}</c>. When it is <see langword="null"/>,
    /// nothing is rendered.
    /// </summary>
    [Parameter]
    public BackgroundPatternType? BackgroundPattern { get; set; }

    /// <summary>
    /// Gets or sets whether the class <c>osirion-bg-image-mask</c> is added, a vertical gradient mask that fades the
    /// pattern out toward the bottom edge.
    /// </summary>
    [Parameter]
    public bool MaskImage { get; set; } = true;

    private string GetBackgroundPatternClass()
    {
        var classes = new List<string>
        {
            "osirion-bg-wrapper"
        };

        if (MaskImage)
        {
            classes.Add("osirion-bg-image-mask");
        }

        if (BackgroundPattern is not null)
        {
            classes.Add(OsirionPattern.BackgroundPattern(BackgroundPattern));
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        return string.Join(" ", classes);
    }
}
