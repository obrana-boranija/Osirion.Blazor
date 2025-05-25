using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

public partial class OsirionBackgroundPattern
{
    [Parameter]
    public BackgroundPatternType? BackgroundPattern { get; set; }

    private string GetBackgroundPatternClass()
    {
        return $"osirion-bg-wrapper {OsirionPattern.BackgroundPattern(BackgroundPattern)} {CssClass}".Trim();
    }
}
