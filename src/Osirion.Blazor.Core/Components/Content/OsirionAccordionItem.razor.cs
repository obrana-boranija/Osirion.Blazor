using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// One collapsible section: a native <c>&lt;details&gt;</c> element whose <c>&lt;summary&gt;</c> is the title. Use it
/// inside <see cref="OsirionAccordion"/>, or on its own.
/// </summary>
public partial class OsirionAccordionItem
{
    [CascadingParameter]
    private OsirionAccordion? Parent { get; set; }

    /// <summary>
    /// Gets or sets the title, which the visitor clicks or presses Enter on to open and close the section.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the section is open when the page renders. Defaults to <see langword="false"/>.
    /// </summary>
    [Parameter]
    public bool Open { get; set; }

    /// <summary>
    /// Gets or sets the section's content.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string GetItemClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-accordion-item" : $"osirion-accordion-item {Class}";
}
