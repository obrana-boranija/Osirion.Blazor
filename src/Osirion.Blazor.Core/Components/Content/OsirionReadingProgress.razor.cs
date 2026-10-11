namespace Osirion.Blazor.Components;

/// <summary>
/// A thin bar at the top of the window that fills as the reader scrolls through the page. It is driven by a CSS
/// scroll-driven animation, with no script; browsers without <c>animation-timeline</c> support do not show it.
/// </summary>
/// <remarks>
/// The color comes from <c>--osirion-reading-progress-color</c> (default: the primary color) and the thickness from
/// <c>--osirion-reading-progress-height</c> (default 3px).
/// </remarks>
public partial class OsirionReadingProgress
{
    private string GetReadingProgressClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-reading-progress" : $"osirion-reading-progress {Class}";
}
