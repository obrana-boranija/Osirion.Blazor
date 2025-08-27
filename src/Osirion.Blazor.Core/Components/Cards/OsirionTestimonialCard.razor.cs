using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Components;

/// <summary>
/// Testimonial card component for displaying customer testimonials, reviews, and recommendations.
/// This component provides a professional layout with profile image, rating, testimonial text, and author details.
/// </summary>
public partial class OsirionTestimonialCard : OsirionComponentBase
{
    /// <summary>
    /// Gets or sets the profile image URL
    /// </summary>
    [Parameter]
    public string? ProfileImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the person's name
    /// </summary>
    [Parameter, EditorRequired]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the person's company
    /// </summary>
    [Parameter]
    public string? Company { get; set; }

    /// <summary>
    /// Gets or sets the person's position/title
    /// </summary>
    [Parameter]
    public string? Position { get; set; }

    /// <summary>
    /// Gets or sets the LinkedIn profile URL
    /// </summary>
    [Parameter]
    public string? LinkedInUrl { get; set; }

    /// <summary>
    /// Gets or sets the testimonial text
    /// </summary>
    [Parameter]
    public string? TestimonialText { get; set; }

    /// <summary>
    /// Gets or sets custom testimonial content. When provided, this overrides TestimonialText.
    /// </summary>
    [Parameter]
    public RenderFragment? TestimonialContent { get; set; }

    /// <summary>
    /// Gets or sets the read more link URL (optional)
    /// </summary>
    [Parameter]
    public string? ReadMoreHref { get; set; }

    /// <summary>
    /// Gets or sets the read more link text
    /// </summary>
    [Parameter]
    public string ReadMoreText { get; set; } = "Read full testimonial";

    /// <summary>
    /// Gets or sets the read more link variant
    /// </summary>
    [Parameter]
    public ReadMoreVariant ReadMoreVariant { get; set; } = ReadMoreVariant.Default;

    /// <summary>
    /// Gets or sets the read more link target
    /// </summary>
    [Parameter]
    public string? ReadMoreTarget { get; set; }

    /// <summary>
    /// Gets or sets whether to show rating stars
    /// </summary>
    [Parameter]
    public bool ShowRating { get; set; } = false;

    /// <summary>
    /// Gets or sets the rating (1-5 stars)
    /// </summary>
    [Parameter]
    public int Rating { get; set; } = 5;

    /// <summary>
    /// Gets or sets the card variant for different styling
    /// </summary>
    [Parameter]
    public TestimonialVariant Variant { get; set; } = TestimonialVariant.Default;

    /// <summary>
    /// Gets or sets the card size
    /// </summary>
    [Parameter]
    public TestimonialSize Size { get; set; } = TestimonialSize.Normal;

    /// <summary>
    /// Gets or sets the image size in pixels
    /// </summary>
    [Parameter]
    public int ImageSize { get; set; } = 64;

    /// <summary>
    /// Gets or sets whether the card should have elevated appearance
    /// </summary>
    [Parameter]
    public bool Elevated { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the card should be borderless
    /// </summary>
    [Parameter]
    public bool Borderless { get; set; } = false;

    /// <summary>
    /// Gets or sets whether this testimonial card is hidden from assistive technology.
    /// This affects the tabindex of interactive elements to prevent them from being focusable
    /// when the parent container has aria-hidden="true".
    /// </summary>
    [Parameter]
    public bool IsHiddenFromAssistiveTech { get; set; } = false;

    /// <summary>
    /// Gets or sets additional child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether this is the first visible testimonial card (for LCP optimization)
    /// </summary>
    [Parameter]
    public bool IsFirstVisible { get; set; } = false;

    /// <summary>
    /// Gets the effective theme mode to use for this card
    /// </summary>
    private ThemeMode GetEffectiveTheme()
    {
        // If the card has a specific theme set that's not System, use it
        if (Theme != ThemeMode.System)
        {
            return Theme;
        }

        // If Theme is System, we need to determine the effective theme
        // This will be handled by CSS media queries for prefers-color-scheme
        return ThemeMode.System;
    }

    /// <summary>
    /// Gets the CSS class for the testimonial card element
    /// </summary>
    private string GetTestimonialCardClass()
    {
        var classes = new List<string> { "osirion-testimonial-card" };

        // Add framework-specific card classes using inherited method
        classes.Add(base.GetCardClass());

        // Add variant class
        classes.Add($"osirion-testimonial-{Variant.ToString().ToLower()}");

        // Add size class
        classes.Add($"osirion-testimonial-{Size.ToString().ToLower()}");

        // Add theme class using effective theme
        classes.Add($"osirion-testimonial-theme-{GetEffectiveTheme().ToString().ToLower()}");

        // Add visual modifiers
        if (Elevated)
        {
            classes.Add("osirion-testimonial-elevated");
        }

        if (Borderless)
        {
            classes.Add("osirion-testimonial-borderless");
        }

        // Use inherited CombineCssClasses method
        return CombineCssClasses(string.Join(" ", classes));
    }

    /// <summary>
    /// Gets the image alt text
    /// </summary>
    private string GetImageAltText()
    {
        if (!string.IsNullOrWhiteSpace(Position) && !string.IsNullOrWhiteSpace(Company))
        {
            return $"Profile photo of {Name}, {Position} at {Company}";
        }
        else if (!string.IsNullOrWhiteSpace(Position))
        {
            return $"Profile photo of {Name}, {Position}";
        }
        else if (!string.IsNullOrWhiteSpace(Company))
        {
            return $"Profile photo of {Name} from {Company}";
        }
        
        return $"Profile photo of {Name}";
    }

    /// <summary>
    /// Gets the LinkedIn aria-label
    /// </summary>
    private string GetLinkedInAriaLabel()
    {
        return $"View {Name}'s LinkedIn profile";
    }

    /// <summary>
    /// Gets the rating aria-label
    /// </summary>
    private string GetRatingAriaLabel()
    {
        return $"Rating: {Rating} out of 5 stars";
    }

    /// <summary>
    /// Renders the filled star icon
    /// </summary>
    private RenderFragment GetFilledStarIcon() => builder =>
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", "16");
        builder.AddAttribute(3, "height", "16");
        builder.AddAttribute(4, "fill", "currentColor");
        builder.AddAttribute(5, "viewBox", "0 0 16 16");
        builder.AddAttribute(6, "aria-hidden", "true");
        builder.OpenElement(7, "path");
        builder.AddAttribute(8, "d", "M3.612 15.443c-.386.198-.824-.149-.746-.592l.83-4.73L.173 6.765c-.329-.314-.158-.888.283-.95l4.898-.696L7.538.792c.197-.39.73-.39.927 0l2.184 4.327 4.898.696c.441.062.612.636.282.95l-3.522 3.356.83 4.73c.078.443-.36.79-.746.592L8 13.187l-4.389 2.256z");
        builder.CloseElement();
        builder.CloseElement();
    };

    /// <summary>
    /// Renders the empty star icon
    /// </summary>
    private RenderFragment GetEmptyStarIcon() => builder =>
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", "16");
        builder.AddAttribute(3, "height", "16");
        builder.AddAttribute(4, "fill", "currentColor");
        builder.AddAttribute(5, "viewBox", "0 0 16 16");
        builder.AddAttribute(6, "aria-hidden", "true");
        builder.OpenElement(7, "path");
        builder.AddAttribute(8, "d", "M2.866 14.85c-.078.444.36.791.746.593l4.39-2.256 4.389 2.256c.386.198.824-.149.746-.592l-.83-4.73 3.522-3.356c.33-.314.16-.888-.282-.95l-4.898-.696L8.465.792a.513.513 0 0 0-.927 0L5.354 5.12l-4.898.696c-.441.062-.612.636-.283.95l3.523 3.356-.83 4.73zm4.905-2.767-3.686 1.894.694-3.957a.565.565 0 0 0-.163-.505L1.71 6.745l4.052-.576a.525.525 0 0 0 .393-.288L8 2.223l1.847 3.658a.525.525 0 0 0 .393.288l4.052.575-2.906 2.77a.565.565 0 0 0-.163.506l.694 3.957-3.686-1.894a.503.503 0 0 0-.461 0z");
        builder.CloseElement();
        builder.CloseElement();
    };

    /// <summary>
    /// Renders the LinkedIn icon
    /// </summary>
    private RenderFragment GetLinkedInIcon() => builder =>
    {
        builder.OpenElement(0, "svg");
        builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
        builder.AddAttribute(2, "width", "16");
        builder.AddAttribute(3, "height", "16");
        builder.AddAttribute(4, "fill", "currentColor");
        builder.AddAttribute(5, "class", "bi bi-linkedin");
        builder.AddAttribute(6, "viewBox", "0 0 16 16");
        builder.AddAttribute(7, "aria-hidden", "true");
        builder.OpenElement(8, "path");
        builder.AddAttribute(9, "d", "M0 1.146C0 .513.526 0 1.175 0h13.65C15.474 0 16 .513 16 1.146v13.708c0 .633-.526 1.146-1.175 1.146H1.175C.526 16 0 15.487 0 14.854zm4.943 12.248V6.169H2.542v7.225zm-1.2-8.212c.837 0 1.358-.554 1.358-1.248-.015-.709-.52-1.248-1.342-1.248S2.4 3.226 2.4 3.934c0 .694.521 1.248 1.327 1.248zm4.908 8.212V9.359c0-.216.016-.432.08-.586.173-.431.568-.878 1.232-.878.869 0 1.216.662 1.216 1.634v3.865h2.401V9.25c0-2.22-1.184-3.252-2.764-3.252-1.274 0-1.845.7-2.165 1.193v.025h-.016l.016-.025V6.169h-2.4c.03.678 0 7.225 0 7.225z");
        builder.CloseElement();
        builder.CloseElement();
    };

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // Validate rating range
        if (Rating < 0) Rating = 0;
        if (Rating > 5) Rating = 5;
        
        // Set default image size based on variant
        if (ImageSize == 64) // Only change if using default
        {
            ImageSize = Size switch
            {
                TestimonialSize.Small => 48,
                TestimonialSize.Large => 80,
                _ => 64
            };
        }
    }
}

/// <summary>
/// Testimonial card variants
/// </summary>
public enum TestimonialVariant
{
    /// <summary>
    /// Default card style
    /// </summary>
    Default,

    /// <summary>
    /// Minimal card style with less visual emphasis
    /// </summary>
    Minimal,

    /// <summary>
    /// Highlighted card style with accent colors
    /// </summary>
    Highlighted,

    /// <summary>
    /// Compact card style for dense layouts
    /// </summary>
    Compact
}

/// <summary>
/// Testimonial card sizes
/// </summary>
public enum TestimonialSize
{
    /// <summary>
    /// Small card size
    /// </summary>
    Small,

    /// <summary>
    /// Normal card size (default)
    /// </summary>
    Normal,

    /// <summary>
    /// Large card size
    /// </summary>
    Large
}