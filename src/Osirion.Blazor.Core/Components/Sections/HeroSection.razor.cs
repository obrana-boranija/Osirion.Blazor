using Microsoft.AspNetCore.Components;
namespace Osirion.Blazor.Components;

/// <summary>
/// HeroSection component for displaying hero content with optional background or side image and buttons
/// </summary>
public partial class HeroSection
{
    /// <summary>
    /// Gets or sets the hero title
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets custom title content. When provided, this overrides the Title parameter.
    /// </summary>
    [Parameter]
    public RenderFragment? TitleContent { get; set; }

    /// <summary>
    /// Gets or sets the hero subtitle
    /// </summary>
    [Parameter]
    public string? Subtitle { get; set; }

    /// <summary>
    /// Gets or sets custom subtitle content. When provided, this overrides the Subtitle parameter.
    /// </summary>
    [Parameter]
    public RenderFragment? SubtitleContent { get; set; }

    /// <summary>
    /// Gets or sets the hero summary/description
    /// </summary>
    [Parameter]
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets custom summary content. When provided, this overrides the Summary parameter.
    /// </summary>
    [Parameter]
    public RenderFragment? SummaryContent { get; set; }

    /// <summary>
    /// Gets or sets the image URL
    /// </summary>
    [Parameter]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the image alt text
    /// </summary>
    [Parameter]
    public string? ImageAlt { get; set; }

    /// <summary>
    /// Gets or sets if image should be displayed as a background image
    /// </summary>
    [Parameter]
    public bool UseBackgroundImage { get; set; } = false;

    /// <summary>
    /// Gets or sets if image has overlay if displayed as a background image
    /// </summary>
    [Parameter]
    public bool HasOverlay { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    [Parameter]
    public BackgroundPatternType? BackgroundPattern { get; set; }

    /// <summary>
    /// Gets or sets the hero alignment: "left", "center", "right". Defaults to "left" or if justified is used, otherwise uses the specified alignment.
    /// </summary>
    [Parameter]
    public Alignment Alignment { get; set; } = Alignment.Left;

    /// <summary>
    /// Gets or sets whether to show the primary button
    /// </summary>
    [Parameter]
    public bool ShowPrimaryButton { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the secondary button
    /// </summary>
    [Parameter]
    public bool ShowSecondaryButton { get; set; } = true;

    /// <summary>
    /// Gets or sets the primary button text
    /// </summary>
    [Parameter]
    public string? PrimaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the primary button URL
    /// </summary>
    [Parameter]
    public string? PrimaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the secondary button text
    /// </summary>
    [Parameter]
    public string? SecondaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the secondary button URL
    /// </summary>
    [Parameter]
    public string? SecondaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the background color (fallback if no image)
    /// </summary>
    [Parameter]
    public string? BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the text color override
    /// </summary>
    [Parameter]
    public string? TextColor { get; set; }

    /// <summary>
    /// Gets or sets the minimum height of the hero section
    /// </summary>
    [Parameter]
    public string MinHeight { get; set; } = "60vh";

    /// <summary>
    /// Gets or sets the author name (for metadata)
    /// </summary>
    [Parameter]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the publish date (for metadata)
    /// </summary>
    [Parameter]
    public DateTime? PublishDate { get; set; }

    /// <summary>
    /// Gets or sets the read time (for metadata)
    /// </summary>
    [Parameter]
    public string? ReadTime { get; set; }

    /// <summary>
    /// Gets or sets whether to show metadata
    /// </summary>
    [Parameter]
    public bool ShowMetadata { get; set; } = false;

    /// <summary>
    /// Gets or sets the image position when not using background: "left", "right". Defaults to "right" or if image is on the left or justified, otherwise uses the specified alignment.
    /// </summary>
    [Parameter]
    public Alignment ImagePosition { get; set; } = Alignment.Right;

    /// <summary>
    /// Gets or sets the layout variant: "hero", "jumbotron", "minimal". Defaults to "hero".
    /// </summary>
    [Parameter]
    public HeroVariant Variant { get; set; } = HeroVariant.Hero;

    /// <summary>
    /// Gets or sets additional child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets divider to separate it from other sections
    /// </summary>
    [Parameter]
    public bool HasDivider { get; set; } = true;

    /// <summary>
    /// Gets whether any buttons should be shown
    /// </summary>
    private bool HasButtons => (ShowPrimaryButton && !string.IsNullOrWhiteSpace(PrimaryButtonText) && !string.IsNullOrWhiteSpace(PrimaryButtonUrl)) ||
                               (ShowSecondaryButton && !string.IsNullOrWhiteSpace(SecondaryButtonText) && !string.IsNullOrWhiteSpace(SecondaryButtonUrl));

    private Alignment _imageposition => ImagePosition == Alignment.Left || ImagePosition == Alignment.Justify ? Alignment.Right : ImagePosition;
    private Alignment _alignment => Alignment == Alignment.Justify ? Alignment.Left : Alignment;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        var att = Attributes;
    }

    /// <summary>
    /// Gets the CSS class for the hero section
    /// </summary>
    private string GetHeroSectionClass()
    {
        var classes = new List<string> { "osirion-hero-section" };

        classes.Add($"osirion-hero-variant-{Variant.ToString().ToLower()}");
        classes.Add($"osirion-hero-align-{_alignment.ToString().ToLower()}");

        if (!string.IsNullOrWhiteSpace(ImageUrl))
        {
            if (UseBackgroundImage)
            {
                classes.Add("osirion-hero-with-background");
                if (HasOverlay)
                {
                    classes.Add("osirion-hero-with-overlay");
                }
            }
            else
            {
                classes.Add("osirion-hero-with-side-image");
                classes.Add($"osirion-hero-image-{_imageposition.ToString().ToLower()}");
            }
        }

        if (!string.IsNullOrWhiteSpace(Class))
        {
            classes.Add(Class);
        }

        if (HasDivider)
        {
            classes.Add("hero-divider");
        }


        return string.Join(" ", classes) + Class;
    }

    /// <summary>
    /// Gets the background style for the hero section
    /// </summary>
    private string GetBackgroundStyle()
    {
        var styles = new List<string>();

        if (UseBackgroundImage && !string.IsNullOrWhiteSpace(ImageUrl))
        {
            styles.Add($"background-image: url('{ImageUrl}')");
        }

        if (!string.IsNullOrWhiteSpace(BackgroundColor))
        {
            styles.Add($"background-color: {BackgroundColor}");
        }

        if (!string.IsNullOrWhiteSpace(TextColor))
        {
            styles.Add($"color: {TextColor}");
        }

        styles.Add($"min-height: {MinHeight}");

        return string.Join("; ", styles) + ";" + Style;
    }

    private (string Width, string Heigth) GetImageSize()
    {
        return Variant switch
        {
            HeroVariant.Hero => ("600", "315"),
            HeroVariant.Jumbotron => ("600", "600"),
            HeroVariant.Minimal => ("315", "315"),
            _ => ("600", "315"),
        };
    }

    /// <summary>
    /// Formats the publish date for display
    /// </summary>
    private string FormatPublishDate()
    {
        return PublishDate?.ToString("MMM dd, yyyy") ?? string.Empty;
    }
}