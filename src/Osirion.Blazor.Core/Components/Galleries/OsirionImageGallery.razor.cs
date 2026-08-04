using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Displays responsive images with an optional client-side lightbox enhancement.
/// The gallery grid remains useful when rendered without interactivity.
/// </summary>
public partial class OsirionImageGallery : OsirionComponentBase
{
    /// <summary>Gets or sets the images displayed by the gallery.</summary>
    [Parameter]
    [EditorRequired]
    public IReadOnlyList<GalleryItem> Items { get; set; } = Array.Empty<GalleryItem>();

    /// <summary>Gets or sets the number of columns at large viewport widths.</summary>
    [Parameter]
    public int Columns { get; set; } = 2;

    /// <summary>Gets or sets the CSS gap scale used between gallery items.</summary>
    [Parameter]
    public int GapSize { get; set; } = 4;

    /// <summary>Gets or sets whether captions use a dark-surface color treatment.</summary>
    [Parameter]
    public bool Dark { get; set; }

    /// <summary>Describes an image displayed by <see cref="OsirionImageGallery"/>.</summary>
    public sealed record GalleryItem(
        string Src,
        string Alt,
        string Caption = "",
        int Width = 1438,
        int Height = 1136);

    private int SafeColumns => Math.Clamp(Columns, 1, 4);
    private int SafeGapSize => Math.Clamp(GapSize, 0, 5);
}
