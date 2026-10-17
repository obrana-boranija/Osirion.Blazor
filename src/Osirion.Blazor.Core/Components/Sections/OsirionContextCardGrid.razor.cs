using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Displays labeled detail cards in a responsive section.</summary>
public partial class OsirionContextCardGrid : OsirionComponentBase
{
    /// <summary>
    /// The section's heading, rendered as an <c>h2</c>; with an empty value no heading is rendered and the section
    /// has no <c>aria-labelledby</c>.
    /// </summary>
    [Parameter] public string Title { get; set; } = "Context by category";

    /// <summary>
    /// Supporting text under the heading; nothing is rendered when it is empty.
    /// </summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>
    /// The cards. With none, only the heading is rendered.
    /// </summary>
    [Parameter] public ContextCardItem[] Items { get; set; } = [];

    /// <summary>
    /// The <c>id</c> of the heading, which the section's <c>aria-labelledby</c> points at. Each grid gets its own
    /// generated value unless one is set.
    /// </summary>
    [Parameter] public string HeadingId { get; set; } = $"osirion-context-card-grid-{Guid.NewGuid():N}";

    /// <summary>
    /// The section's <c>id</c>, for in-page links.
    /// </summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>
    /// Extra CSS classes on the section.
    /// </summary>
    [Parameter] public string? SectionClass { get; set; }

    /// <summary>
    /// CSS classes of the inner container; <c>container</c> is a Bootstrap class, so set another value for other
    /// frameworks.
    /// </summary>
    [Parameter] public string ContainerClass { get; set; } = "container";

    private bool HasTitle => !string.IsNullOrWhiteSpace(Title);

    /// <summary>Describes a card label, detail, and optional accent.</summary>
    public record ContextCardItem(string Label, string Description, string Accent = "green");
}
