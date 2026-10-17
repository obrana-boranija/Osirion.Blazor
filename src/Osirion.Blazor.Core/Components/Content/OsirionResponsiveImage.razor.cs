using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// An image that keeps its space while it loads, lets the browser pick a file sized for the screen, and loads
/// lazily unless it is the page's main image. It renders a plain <c>&lt;img&gt;</c>, so it needs no script.
/// </summary>
public partial class OsirionResponsiveImage
{
    /// <summary>
    /// Gets or sets the image URL, written to <c>src</c>. Browsers that ignore <c>srcset</c> load it, and it is the
    /// first argument of <see cref="SrcFormatter"/>. Required.
    /// </summary>
    [Parameter, EditorRequired]
    public string Src { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text alternative. Describe what the image shows; use an empty string only for an image that
    /// is purely decorative, which screen readers then skip. Required.
    /// </summary>
    [Parameter, EditorRequired]
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the image's intrinsic width in pixels, written to <c>width</c>. With <see cref="Height"/> it
    /// reserves the space before the file arrives, so the content below does not jump (Cumulative Layout Shift).
    /// When set it must be greater than zero, or the component throws <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Parameter]
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the image's intrinsic height in pixels, written to <c>height</c>. See <see cref="Width"/>; the
    /// same rule applies.
    /// </summary>
    [Parameter]
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the <c>srcset</c> candidates as written, for example <c>/img/a-640.jpg 640w, /img/a-1280.jpg
    /// 1280w</c>. Takes precedence over <see cref="Widths"/>.
    /// </summary>
    [Parameter]
    public string? SrcSet { get; set; }

    /// <summary>
    /// Gets or sets the widths, in pixels, to offer in <c>srcset</c>. They are sorted, and duplicates and values of
    /// zero or less are dropped. Each candidate's URL comes from <see cref="SrcFormatter"/>; without one this
    /// parameter is ignored.
    /// </summary>
    [Parameter]
    public IReadOnlyList<int>? Widths { get; set; }

    /// <summary>
    /// Gets or sets how the URL of the file of a given width in pixels is built from <see cref="Src"/>, for an
    /// image service such as <c>(src, width) =&gt; $"{src}?w={width}"</c>. Used with <see cref="Widths"/>.
    /// </summary>
    [Parameter]
    public Func<string, int, string>? SrcFormatter { get; set; }

    /// <summary>
    /// Gets or sets the <c>sizes</c> attribute: how wide the image is shown, for example <c>(min-width: 768px)
    /// 50vw, 100vw</c>. It is written only when a <c>srcset</c> exists. When it is not set, lazily loaded images
    /// use <c>auto, 100vw</c> (the browser measures the layout where it can) and <see cref="Priority"/> images use
    /// <c>100vw</c>.
    /// </summary>
    [Parameter]
    public string? Sizes { get; set; }

    /// <summary>
    /// Gets or sets whether this is the page's main image, typically the largest one in the first view. It then
    /// loads at once (<c>loading="eager"</c>) with <c>fetchpriority="high"</c> instead of lazily, which speeds up
    /// Largest Contentful Paint. Mark only one image per page.
    /// </summary>
    [Parameter]
    public bool Priority { get; set; }

    /// <summary>
    /// Gets or sets a caption. When it is not empty, the image is wrapped in <c>figure</c> with the caption in a
    /// <c>figcaption</c>; the CSS classes and extra attributes stay on the <c>img</c>.
    /// </summary>
    [Parameter]
    public string? Caption { get; set; }

    private string? EffectiveSrcSet { get; set; }

    private string? EffectiveSizes { get; set; }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        if (Width is <= 0 || Height is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Width), "Width and Height must be positive when set.");
        }

        EffectiveSrcSet = BuildSrcSet();
        EffectiveSizes = EffectiveSrcSet is null ? null : Sizes ?? (Priority ? "100vw" : "auto, 100vw");
    }

    private string? BuildSrcSet()
    {
        if (!string.IsNullOrWhiteSpace(SrcSet))
        {
            return SrcSet;
        }

        if (SrcFormatter is null || Widths is null || Widths.Count == 0)
        {
            return null;
        }

        var candidates = Widths
            .Where(width => width > 0)
            .Distinct()
            .Order()
            .Select(width => $"{SrcFormatter(Src, width)} {width}w")
            .ToArray();

        return candidates.Length == 0 ? null : string.Join(", ", candidates);
    }

    private string GetImageClass() =>
        string.IsNullOrWhiteSpace(Class) ? "osirion-responsive-image" : $"osirion-responsive-image {Class}";
}
