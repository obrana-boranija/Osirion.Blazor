using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// An image that keeps its space while it loads, lets the browser pick a file sized for the screen, and loads
/// lazily unless it is the page's main image. It renders a plain <c>&lt;img&gt;</c>, so it needs no script.
/// </summary>
public partial class OsirionResponsiveImage
{
    /// <summary>
    /// Gets or sets the image URL, used by browsers that ignore <c>srcset</c> and as the base for
    /// <see cref="SrcFormatter"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public string Src { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text alternative. Describe what the image shows; use an empty string only for an image that
    /// is purely decorative, which screen readers then skip.
    /// </summary>
    [Parameter, EditorRequired]
    public string Alt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the image's intrinsic width in pixels. With <see cref="Height"/> it reserves the space before the
    /// file arrives, so the content below does not jump (Cumulative Layout Shift).
    /// </summary>
    [Parameter]
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the image's intrinsic height in pixels. See <see cref="Width"/>.
    /// </summary>
    [Parameter]
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the <c>srcset</c> candidates as written, for example <c>/img/a-640.jpg 640w, /img/a-1280.jpg 1280w</c>.
    /// Takes precedence over <see cref="Widths"/>.
    /// </summary>
    [Parameter]
    public string? SrcSet { get; set; }

    /// <summary>
    /// Gets or sets the widths, in pixels, to offer in <c>srcset</c>. Each candidate's URL comes from
    /// <see cref="SrcFormatter"/>; without one this parameter is ignored.
    /// </summary>
    [Parameter]
    public IReadOnlyList<int>? Widths { get; set; }

    /// <summary>
    /// Gets or sets how the URL of the file of a given width in pixels is built from <see cref="Src"/>, for an image
    /// service such as <c>(src, width) =&gt; $"{src}?w={width}"</c>.
    /// </summary>
    [Parameter]
    public Func<string, int, string>? SrcFormatter { get; set; }

    /// <summary>
    /// Gets or sets the <c>sizes</c> attribute: how wide the image is shown, for example
    /// <c>(min-width: 768px) 50vw, 100vw</c>. When <c>srcset</c> is set and this is not, lazily loaded images use
    /// <c>auto, 100vw</c> (the browser measures the layout where it can) and priority images use <c>100vw</c>.
    /// </summary>
    [Parameter]
    public string? Sizes { get; set; }

    /// <summary>
    /// Gets or sets whether this is the page's main image (typically the largest one in the first view). It is then
    /// loaded at once with high fetch priority instead of lazily, which speeds up Largest Contentful Paint.
    /// </summary>
    [Parameter]
    public bool Priority { get; set; }

    /// <summary>
    /// Gets or sets a caption. When set, the image is wrapped in <c>&lt;figure&gt;</c> with a <c>&lt;figcaption&gt;</c>.
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
