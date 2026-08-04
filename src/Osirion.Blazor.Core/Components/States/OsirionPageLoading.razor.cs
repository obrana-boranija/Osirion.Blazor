using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;
/// <summary>Displays a loading state while page content is unavailable.</summary>
public partial class OsirionPageLoading
{
    /// <summary>
    /// Gets or sets the loading text
    /// </summary>
    [Parameter]
    public bool ShowText { get; set; } = true;

    /// <summary>
    /// Gets or sets the loading text
    /// </summary>
    [Parameter]
    public string Text { get; set; } = "Loading content...";
}
