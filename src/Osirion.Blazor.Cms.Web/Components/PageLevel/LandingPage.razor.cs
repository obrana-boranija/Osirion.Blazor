using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// LandingPage component for marketing and conversion-focused pages
/// </summary>
public partial class LandingPage
{
    [Inject]
    private ILogger<LandingPage> Logger { get; set; } = default!;

    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>
    /// The source path of the item to load from the default provider while <see cref="Content"/> is not set. When
    /// nothing is found, the page shows <see cref="NotFoundText"/> and does not signal 404.
    /// </summary>
    [Parameter]
    public string? ContentPath { get; set; }

    /// <summary>
    /// The item that makes the page: its title and description are the hero's, its featured image the hero's image
    /// and its body follows the hero. Without an item the page shows what the other parameters give it.
    /// </summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>
    /// Whether to show a hero with the title, the description, the image and up to two buttons.
    /// </summary>
    [Parameter]
    public bool ShowHero { get; set; } = true;

    /// <summary>
    /// The hero's title when there is no <see cref="Content"/>. The item's title wins when both are set.
    /// </summary>
    [Parameter]
    public string? HeroTitle { get; set; }

    /// <summary>
    /// The hero's subtitle when there is no <see cref="Content"/>, or when the item has no description.
    /// </summary>
    [Parameter]
    public string? HeroSubtitle { get; set; }

    /// <summary>
    /// The hero's image address when the item has no featured image.
    /// </summary>
    [Parameter]
    public string? HeroBackgroundImage { get; set; }

    /// <summary>
    /// Has no effect: the hero is not given the alignment.
    /// </summary>
    [Parameter]
    public string HeroAlignment { get; set; } = "center";

    /// <summary>
    /// The minimum height of the hero, as a CSS length such as <c>70vh</c> or <c>30rem</c>.
    /// </summary>
    [Parameter]
    public string HeroHeight { get; set; } = "70vh";

    /// <summary>
    /// Has no effect: a hero button shows whenever its text and address are both set.
    /// </summary>
    [Parameter]
    public bool ShowHeroButtons { get; set; } = true;

    /// <summary>
    /// The text of the hero's primary button. The button needs <see cref="HeroPrimaryButtonUrl"/> too.
    /// </summary>
    [Parameter]
    public string? HeroPrimaryButtonText { get; set; }

    /// <summary>
    /// The address of the hero's primary button.
    /// </summary>
    [Parameter]
    public string? HeroPrimaryButtonUrl { get; set; }

    /// <summary>
    /// The text of the hero's secondary button. The button needs <see cref="HeroSecondaryButtonUrl"/> too.
    /// </summary>
    [Parameter]
    public string? HeroSecondaryButtonText { get; set; }

    /// <summary>
    /// The address of the hero's secondary button.
    /// </summary>
    [Parameter]
    public string? HeroSecondaryButtonUrl { get; set; }

    /// <summary>
    /// The width of the section that holds the item's body: <c>narrow</c>, <c>normal</c>, <c>wide</c> or <c>full</c>.
    /// </summary>
    [Parameter]
    public string ContentWidth { get; set; } = "normal";

    /// <summary>
    /// The text alignment of the body: <c>left</c>, <c>center</c> or <c>right</c>.
    /// </summary>
    [Parameter]
    public string ContentAlign { get; set; } = "left";

    /// <summary>
    /// The look of the section that holds the body: <c>default</c>, <c>highlight</c>, <c>bordered</c> or
    /// <c>background</c>.
    /// </summary>
    [Parameter]
    public string ContentVariant { get; set; } = "default";

    /// <summary>
    /// The padding of the section that holds the body: <c>none</c>, <c>small</c>, <c>normal</c> or <c>large</c>.
    /// </summary>
    [Parameter]
    public string ContentPadding { get; set; } = "normal";

    /// <summary>
    /// Has no effect: the page renders no testimonials section.
    /// </summary>
    [Parameter]
    public bool ShowTestimonials { get; set; } = false;

    /// <summary>
    /// Has no effect: the page renders no testimonials section.
    /// </summary>
    [Parameter]
    public string TestimonialsSectionTitle { get; set; } = "What Our Customers Say";

    /// <summary>
    /// Has no effect: the page renders no testimonials section.
    /// </summary>
    [Parameter]
    public string TestimonialsLayout { get; set; } = "grid";

    /// <summary>
    /// Has no effect: the page renders no testimonials section.
    /// </summary>
    [Parameter]
    public bool ShowTestimonialImages { get; set; } = true;

    /// <summary>
    /// Has no effect: the page renders no testimonials section.
    /// </summary>
    [Parameter]
    public IReadOnlyList<TestimonialItem>? TestimonialItems { get; set; }

    /// <summary>
    /// Has no effect: the page renders no features section.
    /// </summary>
    [Parameter]
    public bool ShowFeatures { get; set; } = false;

    /// <summary>
    /// Has no effect: the page renders no features section.
    /// </summary>
    [Parameter]
    public string FeaturesSectionTitle { get; set; } = "Features";

    /// <summary>
    /// Has no effect: the page renders no features section.
    /// </summary>
    [Parameter]
    public string? FeaturesSectionDescription { get; set; }

    /// <summary>
    /// Has no effect: the page renders no features section.
    /// </summary>
    [Parameter]
    public string FeaturesLayout { get; set; } = "grid";

    /// <summary>
    /// Has no effect: the page renders no features section.
    /// </summary>
    [Parameter]
    public int FeaturesColumns { get; set; } = 3;

    /// <summary>
    /// Has no effect: the page renders no features section.
    /// </summary>
    [Parameter]
    public IReadOnlyList<FeatureItem>? FeatureItems { get; set; }

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public bool ShowCallToAction { get; set; } = true;

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public string? CtaTitle { get; set; }

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public string? CtaDescription { get; set; }

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public string? CtaPrimaryButtonText { get; set; }

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public string? CtaPrimaryButtonUrl { get; set; }

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public string? CtaSecondaryButtonText { get; set; }

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public string? CtaSecondaryButtonUrl { get; set; }

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public string? CtaBackgroundColor { get; set; }

    /// <summary>
    /// Has no effect: the page renders no call to action section.
    /// </summary>
    [Parameter]
    public string CtaVariant { get; set; } = "primary";

    /// <summary>
    /// HTML shown in a wide, highlighted section after the body. It is written through <c>OsirionHtmlRenderer</c>,
    /// which sanitizes it with the registered HTML sanitizer.
    /// </summary>
    [Parameter]
    public string? CustomContent { get; set; }

    /// <summary>
    /// The text shown while the item loads, in interactive render modes only. A statically rendered response is sent
    /// once the item has loaded.
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading page...";

    /// <summary>
    /// The message shown when <see cref="ContentPath"/> finds no item. The response keeps its status code.
    /// </summary>
    [Parameter]
    public string NotFoundText { get; set; } = "Page content not found.";

    /// <summary>
    /// Content shown in a wide section after the body and the custom content.
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
        if (Content is null && ContentPath is { } contentPath && !string.IsNullOrWhiteSpace(contentPath))
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
            if (provider is not null && ContentPath is { } contentPath && !string.IsNullOrWhiteSpace(contentPath))
            {
                Content = await provider.GetItemByPathAsync(contentPath);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load landing page content {ContentPath}", ContentPath);
            Content = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetLandingPageClass()
    {
        return $"osirion-landing-page {Class}".Trim();
    }
}

/// <summary>
/// Represents a testimonial item
/// </summary>
public class TestimonialItem
{
    /// <summary>
    /// Gets or sets the name of the person giving the testimonial
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the role of the person giving the testimonial
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the company of the person giving the testimonial
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Gets or sets the quote text of the testimonial
    /// </summary>
    public string? Quote { get; set; }

    /// <summary>
    /// Gets or sets the image URL for the testimonial
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the image alt text for accessibility
    /// </summary>
    public int Rating { get; set; } = 5;
}

/// <summary>
/// Represents a feature item
/// </summary>
public class FeatureItem
{
    /// <summary>
    /// Gets or sets the title of the feature
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the feature
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the icon for the feature
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the URL for the feature icon
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Gets or sets the link URL for the feature
    /// </summary>
    public string? LinkUrl { get; set; }
}
