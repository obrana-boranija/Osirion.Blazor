using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;
/// <summary>Displays a loading state while page content is unavailable.</summary>
public partial class OsirionPageLoading
{
    /// <summary>
    /// Gets or sets whether <see cref="Text"/> is shown under the spinner.
    /// </summary>
    [Parameter]
    public bool ShowText { get; set; } = true;

    /// <summary>
    /// Gets or sets the text under the spinner, rendered as a paragraph. When it is empty or white space, no
    /// paragraph is rendered.
    /// </summary>
    [Parameter]
    public string Text { get; set; } = "Loading content...";
}
