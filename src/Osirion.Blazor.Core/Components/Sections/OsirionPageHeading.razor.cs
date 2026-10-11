using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// The heading of a page without a hero: its title as the page's <c>h1</c> and an optional sentence below it, in the
/// same readable width as the page body. It renders without script.
/// </summary>
public partial class OsirionPageHeading
{
    /// <summary>Gets or sets the page title, rendered as the <c>h1</c>.</summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the sentence under the title.</summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>Gets or sets content after the description, for example badges or a date.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
