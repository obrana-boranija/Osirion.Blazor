using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Renders a decorative background pattern wrapper.</summary>
public partial class OsirionBackgroundPattern
{
    /// <summary>Gets or sets the pattern to render.</summary>
    [Parameter]
    public BackgroundPatternType? BackgroundPattern { get; set; }

    /// <summary>Gets or sets whether the pattern image uses a mask.</summary>
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
