using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Infinite logo carousel component that displays client logos in a continuous scrolling animation
/// Works in Static SSR without JavaScript requirements
/// </summary>
public partial class OsirionInfiniteLogoCarousel : OsirionComponentBase
{
    /// <summary>
    /// Optional title to display above the logos
    /// </summary>
    [Parameter]
    public string? Title { get; set; }
    
    /// <summary>
    /// Section aria-label for accessibility
    /// </summary>
    [Parameter]
    public string SectionTitle { get; set; } = "Our Clients";
    
    /// <summary>
    /// Animation duration in seconds (default: 60)
    /// </summary>
    [Parameter]
    public int AnimationDuration { get; set; } = 60;
    
    /// <summary>
    /// Custom list of client logos. If not provided, uses default logos.
    /// </summary>
    [Parameter]
    public List<LogoItem>? CustomLogos { get; set; }

    /// <summary>
    /// Gets or sets whether to pause animation on hover
    /// </summary>
    [Parameter]
    public bool PauseOnHover { get; set; } = true;

    /// <summary>
    /// Gets or sets the animation direction (left or right)
    /// </summary>
    [Parameter]
    public AnimationDirection Direction { get; set; } = AnimationDirection.Right;

    /// <summary>
    /// Gets or sets whether to enable grayscale effect for logos (default: true)
    /// </summary>
    [Parameter]
    public bool EnableGrayscale { get; set; } = true;

    /// <summary>
    /// Gets or sets the logo width in pixels
    /// </summary>
    [Parameter]
    public int LogoWidth { get; set; } = 200;

    /// <summary>
    /// Gets or sets the logo height in pixels
    /// </summary>
    [Parameter]
    public int LogoHeight { get; set; } = 80;

    /// <summary>
    /// Gets or sets the gap between logos in pixels
    /// </summary>
    [Parameter]
    public int LogoGap { get; set; } = 24;

    /// <summary>
    /// Gets or sets the amount of logos to display before repeating (for performance)
    /// </summary>
    [Parameter]
    public int? MaxVisibleLogos { get; set; }

    private List<LogoItem> LogoList => CustomLogos ?? GetDefaultLogos();
    private List<LogoItem> VisibleLogos 
    {
        get
        {
            if (MaxVisibleLogos.HasValue && MaxVisibleLogos.Value > 0)
            {
                return LogoList.Take(MaxVisibleLogos.Value).ToList();
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

        // Toggle direction via class so CSS (isolated) can switch animation-name without inline keyframes
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

    private static List<LogoItem> GetDefaultLogos()
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
    Right,
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
    public bool HasDualLogos => !string.IsNullOrWhiteSpace(LightImageUrl) && !string.IsNullOrWhiteSpace(DarkImageUrl);
}