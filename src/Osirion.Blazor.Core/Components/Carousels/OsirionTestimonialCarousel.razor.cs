using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Infinite testimonial carousel component that displays customer testimonials in a continuous scrolling animation.
/// Works in Static SSR without JavaScript requirements.
/// </summary>
public partial class OsirionTestimonialCarousel : OsirionComponentBase
{
    /// <summary>
    /// The heading above the rail, rendered as an <c>h2</c>; nothing is rendered when it is empty.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// The accessible name (<c>aria-label</c>) of the <c>section</c>.
    /// </summary>
    [Parameter]
    public string SectionTitle { get; set; } = "Customer Testimonials";

    /// <summary>
    /// The seconds one full loop takes at <c>Normal</c> speed. Lower is faster.
    /// </summary>
    [Parameter]
    public int AnimationDuration { get; set; } = 60;

    /// <summary>
    /// The testimonials. Without it the carousel shows built-in sample testimonials, so always pass your own. Fewer
    /// than three are repeated on the rail to close the loop; your list is not changed.
    /// </summary>
    [Parameter]
    public List<TestimonialItem>? CustomTestimonials { get; set; }

    /// <summary>
    /// Whether the loop pauses while the pointer is over the carousel. Keyboard focus inside it always pauses the
    /// loop.
    /// </summary>
    [Parameter]
    public bool PauseOnHover { get; set; } = true;

    /// <summary>
    /// <c>Right</c> moves the cards from right to left, <c>Left</c> from left to right.
    /// </summary>
    [Parameter]
    public AnimationDirection Direction { get; set; } = AnimationDirection.Right;

    /// <summary>
    /// The width of each card in pixels.
    /// </summary>
    [Parameter]
    public int CardWidth { get; set; } = 400;

    /// <summary>
    /// The fixed height of each card in pixels; size it for your longest quote.
    /// </summary>
    [Parameter]
    public int CardHeight { get; set; } = 300;

    /// <summary>
    /// The gap between cards in pixels.
    /// </summary>
    [Parameter]
    public int CardGap { get; set; } = 24;

    /// <summary>
    /// Shows only the first N testimonials. A value of zero or less, or none, shows all of them.
    /// </summary>
    [Parameter]
    public int? MaxVisibleTestimonials { get; set; }

    /// <summary>
    /// The cards' look: <c>Default</c>, <c>Minimal</c>, <c>Highlighted</c> or <c>Compact</c>.
    /// </summary>
    [Parameter]
    public TestimonialVariant CardVariant { get; set; } = TestimonialVariant.Default;

    /// <summary>
    /// The cards' size: <c>Small</c>, <c>Normal</c> or <c>Large</c>.
    /// </summary>
    [Parameter]
    public TestimonialSize CardSize { get; set; } = TestimonialSize.Normal;

    /// <summary>
    /// Whether the cards are elevated, for every testimonial that does not set its own <c>Elevated</c>.
    /// </summary>
    [Parameter]
    public bool? CardElevated { get; set; }

    /// <summary>
    /// Whether the cards are drawn without a border.
    /// </summary>
    [Parameter]
    public bool CardBorderless { get; set; } = false;

    /// <summary>
    /// Changes the loop time: <c>Slow</c> adds 30 seconds to <see cref="AnimationDuration"/>, <c>Fast</c> takes 30
    /// off, to a minimum of 10.
    /// </summary>
    [Parameter]
    public CarouselSpeed Speed { get; set; } = CarouselSpeed.Normal;

    /// <summary>
    /// Whether star ratings are allowed; a card shows its rating only when its testimonial's <c>ShowRating</c> is also
    /// on.
    /// </summary>
    [Parameter]
    public bool ShowRating { get; set; } = false;

    /// <summary>
    /// Whether the cards take the carousel's <see cref="OsirionComponentBase.Theme"/> instead of following the system.
    /// </summary>
    [Parameter]
    public bool InheritTheme { get; set; } = false;

    private List<TestimonialItem> TestimonialList => CustomTestimonials ?? GetSampleTestimonials();
    private const int MinimumLoopLength = 3;
    private List<TestimonialItem> _visibleTestimonials = [];
    private List<TestimonialItem> VisibleTestimonials => _visibleTestimonials;

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
            $"--osirion-testimonial-card-height: {CardHeight}px",
            $"--osirion-testimonial-card-gap: {CardGap}px"
        };

        return string.Join("; ", styles) + ";";
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _visibleTestimonials = BuildVisibleTestimonials();
    }

    // Built into a new list: the padding must not leak into the caller's CustomTestimonials.
    private List<TestimonialItem> BuildVisibleTestimonials()
    {
        var visible = MaxVisibleTestimonials is > 0
            ? TestimonialList.Take(MaxVisibleTestimonials.Value).ToList()
            : new List<TestimonialItem>(TestimonialList);

        // Fewer cards than this leave a visible gap in the loop, so the first ones are repeated.
        var originalCount = visible.Count;
        for (var index = 0; originalCount > 0 && visible.Count < MinimumLoopLength; index++)
        {
            visible.Add(visible[index % originalCount]);
        }

        return visible;
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
    string? AdditionalCssClass = null,
    bool? Elevated = null);
