using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// LandingPage component for marketing and conversion-focused pages
/// </summary>
public partial class LandingPage
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the path to the landing page content
    /// </summary>
    [Parameter]
    public string? ContentPath { get; set; }

    /// <summary>
    /// Gets or sets the content item to display
    /// </summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>
    /// Gets or sets whether to show the hero section
    /// </summary>
    [Parameter]
    public bool ShowHero { get; set; } = true;

    /// <summary>
    /// Gets or sets the hero title (override)
    /// </summary>
    [Parameter]
    public string? HeroTitle { get; set; }

    /// <summary>
    /// Gets or sets the hero subtitle (override)
    /// </summary>
    [Parameter]
    public string? HeroSubtitle { get; set; }

    /// <summary>
    /// Gets or sets the hero background image
    /// </summary>
    [Parameter]
    public string? HeroBackgroundImage { get; set; }

    /// <summary>
    /// Gets or sets the hero alignment
    /// </summary>
    [Parameter]
    public string HeroAlignment { get; set; } = "center";

    /// <summary>
    /// Gets or sets the hero height
    /// </summary>
    [Parameter]
    public string HeroHeight { get; set; } = "70vh";

    /// <summary>
    /// Gets or sets whether to show hero buttons
    /// </summary>
    [Parameter]
    public bool ShowHeroButtons { get; set; } = true;

    /// <summary>
    /// Gets or sets the primary button text
    /// </summary>
    [Parameter]
    public string? HeroPrimaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the primary button URL
    /// </summary>
    [Parameter]
    public string? HeroPrimaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the secondary button text
    /// </summary>
    [Parameter]
    public string? HeroSecondaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the secondary button URL
    /// </summary>
    [Parameter]
    public string? HeroSecondaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the content width
    /// </summary>
    [Parameter]
    public string ContentWidth { get; set; } = "normal";

    /// <summary>
    /// Gets or sets the content alignment
    /// </summary>
    [Parameter]
    public string ContentAlign { get; set; } = "left";

    /// <summary>
    /// Gets or sets the content variant
    /// </summary>
    [Parameter]
    public string ContentVariant { get; set; } = "default";

    /// <summary>
    /// Gets or sets the content padding
    /// </summary>
    [Parameter]
    public string ContentPadding { get; set; } = "normal";

    /// <summary>
    /// Gets or sets whether to show testimonials section
    /// </summary>
    [Parameter]
    public bool ShowTestimonials { get; set; } = false;

    /// <summary>
    /// Gets or sets the testimonials section title
    /// </summary>
    [Parameter]
    public string TestimonialsSectionTitle { get; set; } = "What Our Customers Say";

    /// <summary>
    /// Gets or sets the testimonials layout
    /// </summary>
    [Parameter]
    public string TestimonialsLayout { get; set; } = "grid";

    /// <summary>
    /// Gets or sets whether to show testimonial images
    /// </summary>
    [Parameter]
    public bool ShowTestimonialImages { get; set; } = true;

    /// <summary>
    /// Gets or sets the testimonial items
    /// </summary>
    [Parameter]
    public IReadOnlyList<TestimonialItem>? TestimonialItems { get; set; }

    /// <summary>
    /// Gets or sets whether to show features section
    /// </summary>
    [Parameter]
    public bool ShowFeatures { get; set; } = false;

    /// <summary>
    /// Gets or sets the features section title
    /// </summary>
    [Parameter]
    public string FeaturesSectionTitle { get; set; } = "Features";

    /// <summary>
    /// Gets or sets the features section description
    /// </summary>
    [Parameter]
    public string? FeaturesSectionDescription { get; set; }

    /// <summary>
    /// Gets or sets the features layout
    /// </summary>
    [Parameter]
    public string FeaturesLayout { get; set; } = "grid";

    /// <summary>
    /// Gets or sets the number of features columns
    /// </summary>
    [Parameter]
    public int FeaturesColumns { get; set; } = 3;

    /// <summary>
    /// Gets or sets the feature items
    /// </summary>
    [Parameter]
    public IReadOnlyList<FeatureItem>? FeatureItems { get; set; }

    /// <summary>
    /// Gets or sets whether to show call to action section
    /// </summary>
    [Parameter]
    public bool ShowCallToAction { get; set; } = true;

    /// <summary>
    /// Gets or sets the CTA title
    /// </summary>
    [Parameter]
    public string? CtaTitle { get; set; }

    /// <summary>
    /// Gets or sets the CTA description
    /// </summary>
    [Parameter]
    public string? CtaDescription { get; set; }

    /// <summary>
    /// Gets or sets the CTA primary button text
    /// </summary>
    [Parameter]
    public string? CtaPrimaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the CTA primary button URL
    /// </summary>
    [Parameter]
    public string? CtaPrimaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the CTA secondary button text
    /// </summary>
    [Parameter]
    public string? CtaSecondaryButtonText { get; set; }

    /// <summary>
    /// Gets or sets the CTA secondary button URL
    /// </summary>
    [Parameter]
    public string? CtaSecondaryButtonUrl { get; set; }

    /// <summary>
    /// Gets or sets the CTA background color
    /// </summary>
    [Parameter]
    public string? CtaBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the CTA variant
    /// </summary>
    [Parameter]
    public string CtaVariant { get; set; } = "primary";

    /// <summary>
    /// Gets or sets custom HTML content
    /// </summary>
    [Parameter]
    public string? CustomContent { get; set; }

    /// <summary>
    /// Gets or sets the loading text
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading page...";

    /// <summary>
    /// Gets or sets the not found text
    /// </summary>
    [Parameter]
    public string NotFoundText { get; set; } = "Page content not found.";

    /// <summary>
    /// Gets or sets additional child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets whether the component is loading
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        if (Content == null && !string.IsNullOrEmpty(ContentPath))
        {
            await LoadContentAsync();
        }
        else
        {
            IsLoading = false;
        }
    }

    private async Task LoadContentAsync()
    {
        IsLoading = true;
        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider != null)
            {
                Content = await provider.GetItemByPathAsync(ContentPath);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading landing page content: {ex.Message}");
            Content = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetLandingPageClass()
    {
        return $"osirion-landing-page {CssClass}".Trim();
    }
}

/// <summary>
/// Represents a testimonial item
/// </summary>
public class TestimonialItem
{
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? Company { get; set; }
    public string? Quote { get; set; }
    public string? ImageUrl { get; set; }
    public int Rating { get; set; } = 5;
}

/// <summary>
/// Represents a feature item
/// </summary>
public class FeatureItem
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? IconUrl { get; set; }
    public string? LinkUrl { get; set; }
}