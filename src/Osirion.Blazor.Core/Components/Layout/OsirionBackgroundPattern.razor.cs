using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

public partial class OsirionBackgroundPattern
{
    [Parameter]
    public BackgroundPatternType? BackgroundPattern { get; set; }

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

        if (!string.IsNullOrWhiteSpace(CssClass))
        {
            classes.Add(CssClass);
        }

        return string.Join(" ", classes);
    }
}
