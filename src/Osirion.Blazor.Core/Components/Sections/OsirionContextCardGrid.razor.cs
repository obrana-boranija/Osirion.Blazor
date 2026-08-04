using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>Displays labeled detail cards in a responsive section.</summary>
public partial class OsirionContextCardGrid : OsirionComponentBase
{
    /// <summary>Gets or sets the panel heading.</summary>
    [Parameter] public string Title { get; set; } = "Context by category";

    /// <summary>Gets or sets optional supporting text.</summary>
    [Parameter] public string? Description { get; set; }

    /// <summary>Gets or sets the labeled detail cards.</summary>
    [Parameter] public ContextCardItem[] Items { get; set; } = [];

    /// <summary>Gets or sets an id used by the heading and section landmark.</summary>
    [Parameter] public string HeadingId { get; set; } = "osirion-context-card-grid-heading";

    /// <summary>Gets or sets the optional section id used for in-page navigation.</summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>Gets or sets additional classes for the section.</summary>
    [Parameter] public string? SectionClass { get; set; }

    /// <summary>Gets or sets classes for the content container.</summary>
    [Parameter] public string ContainerClass { get; set; } = "container";

    /// <summary>Describes a card label, detail, and optional accent.</summary>
    public record ContextCardItem(string Label, string Description, string Accent = "green");
}
