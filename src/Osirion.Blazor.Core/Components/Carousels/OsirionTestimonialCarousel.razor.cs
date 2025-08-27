using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Infinite testimonial carousel component that displays customer testimonials in a continuous scrolling animation.
/// Works in Static SSR without JavaScript requirements.
/// </summary>
public partial class OsirionTestimonialCarousel : OsirionComponentBase
{
    /// <summary>
    /// Optional title to display above the testimonials
    /// </summary>
    [Parameter]
    public string? Title { get; set; }
    
    /// <summary>
    /// Section aria-label for accessibility
    /// </summary>
    [Parameter]
    public string SectionTitle { get; set; } = "Customer Testimonials";
    
    /// <summary>
    /// Animation duration in seconds (default: 60)
    /// </summary>
    [Parameter]
    public int AnimationDuration { get; set; } = 60;
    
    /// <summary>
    /// Custom list of testimonials. If not provided, uses sample testimonials.
    /// </summary>
    [Parameter]
    public List<TestimonialItem>? CustomTestimonials { get; set; }

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
    /// Gets or sets the card width in pixels
    /// </summary>
    [Parameter]
    public int CardWidth { get; set; } = 400;

    /// <summary>
    /// Gets or sets the gap between cards in pixels
    /// </summary>
    [Parameter]
    public int CardGap { get; set; } = 24;

    /// <summary>
    /// Gets or sets the amount of testimonials to display before repeating (for performance)
    /// </summary>
    [Parameter]
    public int? MaxVisibleTestimonials { get; set; }

    /// <summary>
    /// Gets or sets the testimonial card variant
    /// </summary>
    [Parameter]
    public TestimonialVariant CardVariant { get; set; } = TestimonialVariant.Default;

    /// <summary>
    /// Gets or sets the testimonial card size
    /// </summary>
    [Parameter]
    public TestimonialSize CardSize { get; set; } = TestimonialSize.Normal;

    /// <summary>
    /// Gets or sets whether cards should have elevated appearance
    /// </summary>
    [Parameter]
    public bool CardElevated { get; set; } = true;

    /// <summary>
    /// Gets or sets whether cards should be borderless
    /// </summary>
    [Parameter]
    public bool CardBorderless { get; set; } = false;

    /// <summary>
    /// Gets or sets the animation speed (slow, normal, fast)
    /// </summary>
    [Parameter]
    public CarouselSpeed Speed { get; set; } = CarouselSpeed.Normal;

    /// <summary>
    /// Gets or sets whether to show rating stars on testimonial cards
    /// </summary>
    [Parameter]
    public bool ShowRating { get; set; } = false;

    /// <summary>
    /// Gets or sets whether cards should inherit the carousel's theme instead of using System
    /// </summary>
    [Parameter]
    public bool InheritTheme { get; set; } = false;

    private List<TestimonialItem> TestimonialList => CustomTestimonials ?? GetSampleTestimonials();
    private List<TestimonialItem> VisibleTestimonials 
    {
        get
        {
            if (MaxVisibleTestimonials.HasValue && MaxVisibleTestimonials.Value > 0)
            {
                return TestimonialList.Take(MaxVisibleTestimonials.Value).ToList();
            }
            return TestimonialList;
        }
    }

    /// <summary>
    /// Gets the theme to use for testimonial cards
    /// </summary>
    private ThemeMode GetCardTheme()
    {
        // If InheritTheme is true and card would use System, use the carousel's theme instead
        return InheritTheme ? Theme : ThemeMode.System;
    }

    /// <summary>
    /// Gets the CSS class for the carousel container
    /// </summary>
    private string GetCarouselClass()
    {
        var cssClass = "osirion-testimonial-carousel";
        
        if (PauseOnHover)
            cssClass += " osirion-testimonial-carousel-pausable";

        // Toggle direction via class
        if (Direction == AnimationDirection.Left)
            cssClass += " osirion-testimonial-carousel-reverse";

        // Add speed class
        cssClass += $" osirion-testimonial-carousel-{Speed.ToString().ToLower()}";
            
        return CombineCssClasses(cssClass);
    }

    /// <summary>
    /// Gets the effective animation duration based on speed setting
    /// </summary>
    private int GetEffectiveAnimationDuration()
    {
        return Speed switch
        {
            CarouselSpeed.Slow => AnimationDuration + 30,
            CarouselSpeed.Fast => Math.Max(AnimationDuration - 30, 10),
            _ => AnimationDuration
        };
    }

    /// <summary>
    /// Gets the CSS style for the carousel track
    /// </summary>
    private string GetCarouselTrackStyle()
    {
        var styles = new List<string>
        {
            $"--osirion-testimonial-carousel-duration: {GetEffectiveAnimationDuration()}s",
            $"--osirion-testimonial-card-width: {CardWidth}px",
            $"--osirion-testimonial-card-gap: {CardGap}px"
        };

        return string.Join("; ", styles) + ";";
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // Ensure we have at least 3 testimonials for smooth looping
        if (VisibleTestimonials.Count < 3)
        {
            // Duplicate testimonials to ensure smooth animation
            var originalCount = VisibleTestimonials.Count;
            while (VisibleTestimonials.Count < 3)
            {
                var indexToDuplicate = VisibleTestimonials.Count % originalCount;
                VisibleTestimonials.Add(TestimonialList[indexToDuplicate]);
            }
        }
    }

    /// <summary>
    /// Gets the first visible testimonial image for preloading (LCP optimization)
    /// </summary>
    public string? GetFirstVisibleImageUrl()
    {
        return VisibleTestimonials.FirstOrDefault()?.ProfileImageUrl;
    }

    private static List<TestimonialItem> GetSampleTestimonials()
    {
        return new List<TestimonialItem>
        {
            new("Sarah Johnson", 
                "Senior Developer", 
                "Tech Solutions Inc.",
                "This product has completely transformed our development workflow. The team's responsiveness and attention to detail is outstanding.",
                "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
                "https://linkedin.com/in/sarahjohnson",
                ShowRating: true,
                Rating: 5,
                ReadMoreHref: "/testimonials/sarah-johnson", ReadMoreText: "Find more in Case Study"),

            new("Michael Chen", 
                "CTO", 
                "Innovation Labs",
                "Exceptional quality and support. Our team productivity increased by 40% after implementation. The integration was seamless.",
                "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
                "https://linkedin.com/in/michaelchen",
                ShowRating: true,
                Rating: 5),

            new("Lisa Rodriguez", 
                "Product Manager", 
                "Digital Dynamics",
                "The integration was seamless and the results exceeded our expectations. This platform has become essential to our operations.",
                "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
                ShowRating: true,
                Rating: 5,
                ReadMoreHref: "/testimonials/lisa-rodriguez",
                ReadMoreText: "Read full case study"),

            new("David Park", 
                "Founder & CEO", 
                "StartupCo",
                "This solution has been a game-changer for our startup. The ROI was evident within the first month.",
                "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
                "https://linkedin.com/in/davidpark",
                ShowRating: true,
                Rating: 5),

            new("Anna Thompson", 
                "Lead Designer", 
                "Creative Agency",
                "Clean, efficient, and reliable. The user experience is exceptional and our clients love the results.",
                "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
                ShowRating: true,
                Rating: 4),

            new("Robert Kim", 
                "Lead Engineer", 
                "Enterprise Corp",
                "Outstanding platform with excellent developer experience. The documentation and support are top-notch.",
                "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
                "https://linkedin.com/in/robertkim",
                ShowRating: true,
                Rating: 5),

            new("Maria Garcia", 
                "UX Designer", 
                "Design Studio",
                "Intuitive design and powerful features. Our design process has become much more efficient and collaborative.",
                "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
                ShowRating: true,
                Rating: 5),

            new("James Wilson",
                "Technical Director",
                "Enterprise Solutions",
                "The platform's reliability and performance have exceeded our expectations. Our development velocity has increased significantly.",
                "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=150&h=150&fit=crop&crop=face&auto=format&q=80",
                "https://linkedin.com/in/jameswilson",
                ShowRating: true,
                Rating: 5)
        };
    }
}

/// <summary>
/// Animation speed options for the carousel
/// </summary>
public enum CarouselSpeed
{
    /// <summary>
    /// Slow animation speed
    /// </summary>
    Slow,

    /// <summary>
    /// Normal animation speed (default)
    /// </summary>
    Normal,

    /// <summary>
    /// Fast animation speed
    /// </summary>
    Fast
}

/// <summary>
/// Represents a testimonial item for the carousel
/// </summary>
public record TestimonialItem(
    string Name,
    string? Position = null,
    string? Company = null,
    string? TestimonialText = null,
    string? ProfileImageUrl = null,
    string? LinkedInUrl = null,
    bool ShowRating = false,
    int Rating = 5,
    string? ReadMoreHref = null,
    string? ReadMoreText = "Read more",
    ReadMoreVariant ReadMoreVariant = ReadMoreVariant.Default,
    string? ReadMoreTarget = null,
    string? AdditionalCssClass = null);