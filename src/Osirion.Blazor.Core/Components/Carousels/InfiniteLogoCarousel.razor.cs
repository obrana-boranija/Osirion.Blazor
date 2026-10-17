using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Horizontally scrollable carousel for logos and arbitrary Blazor content.
/// It renders meaningful content during static SSR and progressively enhances with controls and drag support.
/// </summary>
public partial class InfiniteLogoCarousel : OsirionComponentBase
{
    /// <summary>
    /// The heading above the logos, rendered as an <c>h2</c>; nothing is rendered when it is empty.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// The accessible name (<c>aria-label</c>) of the <c>section</c>; set it to something that describes the logos.
    /// </summary>
    [Parameter]
    public string SectionTitle { get; set; } = "Our Clients";

    /// <summary>
    /// The seconds one full loop takes when <see cref="EnableAutoScroll"/> is on. Lower is faster.
    /// </summary>
    [Parameter]
    public int AnimationDuration { get; set; } = 60;

    /// <summary>
    /// The logos. Without it the carousel shows built-in sample logos that load from external addresses, so always
    /// pass your own.
    /// </summary>
    [Parameter]
    public IReadOnlyList<LogoItem>? CustomLogos { get; set; }

    /// <summary>
    /// Extra content rendered as one more item after the logos.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether the row loops continuously with a CSS animation. With it off the row is static.
    /// </summary>
    [Parameter]
    public bool EnableAutoScroll { get; set; }

    /// <summary>
    /// Whether the loop pauses while the pointer is over the carousel. Keyboard focus inside it always pauses the
    /// loop.
    /// </summary>
    [Parameter]
    public bool PauseOnHover { get; set; } = true;

    /// <summary>
    /// <c>Right</c> moves the logos from right to left, <c>Left</c> from left to right.
    /// </summary>
    [Parameter]
    public AnimationDirection Direction { get; set; } = AnimationDirection.Right;

    /// <summary>
    /// Whether logos are shown in grayscale until hovered; with script, the logos nearest the center are in color too.
    /// A logo's own <c>EnableGrayscale</c> overrides it.
    /// </summary>
    [Parameter]
    public bool EnableGrayscale { get; set; } = true;

    /// <summary>
    /// The width of each logo slot in pixels, also written as the images' <c>width</c>; slots are 160px wide below
    /// 768px.
    /// </summary>
    [Parameter]
    public int LogoWidth { get; set; } = 200;

    /// <summary>
    /// The height of each logo slot in pixels, also written as the images' <c>height</c>.
    /// </summary>
    [Parameter]
    public int LogoHeight { get; set; } = 80;

    /// <summary>
    /// The gap between logos in pixels.
    /// </summary>
    [Parameter]
    public int LogoGap { get; set; } = 24;

    /// <summary>
    /// Shows only the first N logos. A value of zero or less, or none, shows all of them.
    /// </summary>
    [Parameter]
    public int? MaxVisibleLogos { get; set; }

    private IReadOnlyList<LogoItem> LogoList => CustomLogos ?? GetDefaultLogos();
    private IReadOnlyList<LogoItem> VisibleLogos
    {
        get
        {
            if (MaxVisibleLogos.HasValue && MaxVisibleLogos.Value > 0)
            {
                return LogoList.Take(MaxVisibleLogos.Value).ToArray();
            }
            return LogoList;
        }
    }

    /// <summary>
    /// Gets the CSS class for the carousel container
    /// </summary>
    private string GetCarouselClass()
    {
        var cssClass = "osirion-infinite-carousel";

        if (PauseOnHover)
            cssClass += " osirion-infinite-carousel-pausable";

        if (EnableGrayscale)
            cssClass += " osirion-carousel-grayscale-enabled";

        if (EnableAutoScroll)
            cssClass += " osirion-carousel-autoscroll";

        if (Direction == AnimationDirection.Left)
            cssClass += " osirion-carousel-reverse";

        return CombineCssClasses(cssClass);
    }

    /// <summary>
    /// Gets the animation duration CSS variable value
    /// </summary>
    private string GetAnimationDuration() => $"{AnimationDuration}s";

    /// <summary>
    /// Gets the CSS style for the carousel list.
    /// Only sets CSS custom properties to drive sizing and duration.
    /// </summary>
    private string GetCarouselListStyle()
    {
        var styles = new List<string>
        {
            $"--osirion-carousel-duration: {GetAnimationDuration()}",
            $"--osirion-logo-width: {LogoWidth}px",
            $"--osirion-logo-height: {LogoHeight}px",
            $"--osirion-logo-gap: {LogoGap}px"
        };

        return string.Join("; ", styles) + ";";
    }

    /// <summary>
    /// Determines the CSS class for a logo based on its characteristics
    /// </summary>
    private string GetLogoClass(LogoItem logoItem)
    {
        var baseClass = "osirion-logo-item";

        // Add grayscale class if enabled globally and not disabled for this specific logo
        var logoGrayscale = logoItem.EnableGrayscale ?? EnableGrayscale;
        if (logoGrayscale)
        {
            baseClass += " osirion-logo-grayscale";
        }

        // Add dual logo class if logo has both light and dark variants
        if (logoItem.HasDualLogos)
        {
            baseClass += " osirion-logo-dual";
        }

        return baseClass;
    }

    /// <summary>
    /// Gets the link attributes for a logo
    /// </summary>
    private Dictionary<string, object> GetLogoLinkAttributes(LogoItem logo)
    {
        var attributes = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(logo.Url))
        {
            attributes["href"] = logo.Url;
            attributes["target"] = logo.Target ?? "_blank";

            // Default to nofollow and noopener for external links
            var relValues = new List<string>();
            if (logo.NoFollow ?? true) relValues.Add("nofollow");
            if (logo.NoOpener ?? true) relValues.Add("noopener");

            if (relValues.Any())
                attributes["rel"] = string.Join(" ", relValues);

            attributes["title"] = logo.AltText;
        }

        return attributes;
    }

    /// <summary>
    /// Gets the appropriate logo URL based on current theme (CSS handles dual logos)
    /// </summary>
    private static string GetLogoUrl(LogoItem logo) => logo.ImageUrl;

    private static IReadOnlyList<LogoItem> GetDefaultLogos()
    {
        return new List<LogoItem>
        {
            // Dual logo example with placeholder URLs (replace with real dual logos)
            new("https://cdn.tridesetri.com/hexavera-storage/hexavera-inline-logo-light-transparent.png",
                "Hexavera - Technology Solutions",
                DarkImageUrl: "https://cdn.tridesetri.com/hexavera-storage/hexavera-inline-logo-light-transparent.png",
                LightImageUrl: "https://cdn.tridesetri.com/hexavera-storage/hexavera-light-inline.png",
                Url: "https://hexavera.com"),
                
            // Single logo examples
            new("https://placehold.co/200x80/28a745/ffffff?text=Company+B",
                "Company B - Software Development",
                Url: "https://example.com/company-b"),

            new("https://cdn.tridesetri.com/hexavera-storage/hexavera-inline-logo-light-transparent.png",
                "Hexavera - Technology Solutions",
                DarkImageUrl: "https://cdn.tridesetri.com/hexavera-storage/hexavera-inline-logo-light-transparent.png",
                LightImageUrl: "https://cdn.tridesetri.com/hexavera-storage/hexavera-light-inline.png",
                Url: "https://hexavera.com"),
                
            // Logo without grayscale
            new("https://placehold.co/200x80/ffc107/000000?text=No+Grayscale",
                "Company D - Always Colorful",
                Url: "https://example.com/company-d",
                EnableGrayscale: false),
                
            // Another dual logo example
            new("https://placehold.co/200x80/007bff/ffffff?text=DUAL+LIGHT",
                "Another Dual Logo",
                LightImageUrl: "https://placehold.co/200x80/007bff/ffffff?text=DUAL+LIGHT",
                DarkImageUrl: "https://placehold.co/200x80/6c757d/ffffff?text=DUAL+DARK",
                Url: "https://example.com/another-dual"),

            new("https://placehold.co/200x80/fd7e14/ffffff?text=Company+F",
                "Company F - Strategic Alliance",
                Url: "https://example.com/company-f")
        };
    }
}

/// <summary>
/// Animation direction for the carousel
/// </summary>
public enum AnimationDirection
{
    /// <summary>Moves logos from right to left.</summary>
    Right,
    /// <summary>Moves logos from left to right.</summary>
    Left
}

/// <summary>
/// Represents a logo item with URL and accessibility text
/// </summary>
public record LogoItem(
    string ImageUrl,
    string AltText,
    string? LightImageUrl = null,
    string? DarkImageUrl = null,
    string? Url = null,
    string? Target = null,
    bool? NoFollow = null,
    bool? NoOpener = null,
    bool? EnableGrayscale = null)
{
    /// <summary>Gets whether both light and dark logo variants are available.</summary>
    public bool HasDualLogos => !string.IsNullOrWhiteSpace(LightImageUrl) && !string.IsNullOrWhiteSpace(DarkImageUrl);
}
