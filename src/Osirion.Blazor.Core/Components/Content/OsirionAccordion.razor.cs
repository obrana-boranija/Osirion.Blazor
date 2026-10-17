using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// A list of collapsible sections built on the native <c>&lt;details&gt;</c> element, so it opens and closes without
/// script and is keyboard accessible as is.
/// </summary>
public partial class OsirionAccordion
{
    // Generated once per instance so every render gives the items the same group name.
    private readonly string _generatedName = $"osirion-accordion-{Guid.NewGuid():N}";

    /// <summary>Gets or sets the <see cref="OsirionAccordionItem"/> components.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether several items can be open at once. When <see langword="false"/>, the items share a
    /// <c>name</c> and the browser closes the others when one opens; browsers without support for exclusive
    /// <c>details</c> elements let several stay open.
    /// </summary>
    [Parameter]
    public bool AllowMultiple { get; set; } = true;

    /// <summary>Gets the name the items share when only one may be open, or <see langword="null"/>.</summary>
    internal string? GroupName => AllowMultiple ? null : _generatedName;

    private string GetContainerClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-accordion" : $"osirion-accordion {Class}";
}
