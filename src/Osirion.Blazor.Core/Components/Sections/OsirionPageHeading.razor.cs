using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// The heading of a page without a hero: its title as the page's <c>h1</c> and an optional sentence below it, in the
/// same readable width as the page body. It renders without script.
/// </summary>
public partial class OsirionPageHeading
{
    /// <summary>
    /// The page title, rendered as the page's <c>h1</c>. Required.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The sentence under the title; nothing is rendered when it is empty.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Extra content after the description, for example badges or a date.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
