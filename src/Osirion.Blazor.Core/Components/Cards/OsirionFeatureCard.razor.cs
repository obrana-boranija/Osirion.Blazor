using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

public partial class OsirionFeatureCard
{
    /// <summary>
    /// Gets or sets the fetured card title
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the fetured card description.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show the description.
    /// </summary>
    [Parameter]
    public bool ShowDescription { get; set; } = true;

    /// <summary>
    /// Gets or sets the fetured card url.
    /// </summary>
    [Parameter]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the fetured card image url.
    /// </summary>
    [Parameter]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the fetured card image alt.
    /// </summary>
    [Parameter]
    public string? ImageAlt { get; set; }

    /// <summary>
    /// Gets or sets the fetured card image loading.
    /// </summary>
    [Parameter]
    public bool ImageLazyLoading { get; set; } = true;

    /// <summary>
    /// Gets or sets the child content of the feature card.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the image position (Top, Bottom, Left, Right).
    /// </summary>
    [Parameter]
    public ImagePosition ImagePosition { get; set; } = ImagePosition.Left;

    /// <summary>
    /// Gets or sets the card size (ExtraExtraLarge, ExtraLarge, Large, Normal, Small, ExtraSmall, ExtraExtraSmall).
    /// </summary>
    [Parameter]
    public CardSize CardSize { get; set; } = CardSize.Normal;


    private string GetImagePositionClass() => ImagePosition switch
    {
        ImagePosition.Top => "osirion-featured-post-image-top",
        ImagePosition.Bottom => "osirion-featured-post-image-bottom",
        ImagePosition.Right => "osirion-featured-post-image-right",
        _ => "osirion-featured-post-image-left"
    };

    private string GetCardSizeClass() => CardSize switch
    {
        CardSize.ExtraExtraLarge => "osirion-featured-post-xxl",
        CardSize.ExtraLarge => "osirion-featured-post-xl",
        CardSize.Large => "osirion-featured-post-lg",
        CardSize.Normal => "osirion-featured-post-md",
        CardSize.Small => "osirion-featured-post-sm",
        CardSize.ExtraSmall => "osirion-featured-post-xs",
        CardSize.ExtraExtraSmall => "osirion-featured-post-xxs",
        _ => "osirion-featured-post-md"
    };

    private string GetNoImageClass() => CardSize switch
    {
        CardSize.ExtraExtraLarge => "1200x1200",
        CardSize.ExtraLarge => "900x900",
        CardSize.Large => "600x600",
        CardSize.Normal => "450x450",
        CardSize.Small => "300x300",
        CardSize.ExtraSmall => "150x150",
        CardSize.ExtraExtraSmall => "75x75",
        _ => "450x450"
    };

    private string NoImageUrl()
    {
        return $"https://placehold.co/{GetNoImageClass()}/1a1a1a/ffffff?text=No+Image";
    }
}
