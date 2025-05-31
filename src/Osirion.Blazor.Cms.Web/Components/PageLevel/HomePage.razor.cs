using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// HomePage component for displaying homepage content with hero section, featured content, and navigation
/// </summary>
public partial class HomePage
{
    [Inject]
    private IContentProviderManager ContentProviderManager { get; set; } = default!;

    /// <summary>
    /// Gets or sets the path to the homepage content item
    /// </summary>
    [Parameter]
    public string? ContentPath { get; set; }

    /// <summary>
    /// Gets or sets the content item to display
    /// </summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>
    /// Gets or sets the locale for content filtering
    /// </summary>
    [Parameter]
    public string? Locale { get; set; }

    /// <summary>
    /// Gets or sets whether to show the hero section
    /// </summary>
    [Parameter]
    public bool ShowHero { get; set; } = true;

    /// <summary>
    /// Gets or sets the hero content item
    /// </summary>
    [Parameter]
    public ContentItem? HeroContent { get; set; }

    /// <summary>
    /// Gets or sets the hero title (used if HeroContent is null)
    /// </summary>
    [Parameter]
    public string? HeroTitle { get; set; }

    /// <summary>
    /// Gets or sets the hero subtitle (used if HeroContent is null)
    /// </summary>
    [Parameter]
    public string? HeroSubtitle { get; set; }

    /// <summary>
    /// Gets or sets the hero background image URL
    /// </summary>
    [Parameter]
    public string? HeroBackgroundImage { get; set; }

    /// <summary>
    /// Gets or sets the hero alignment
    /// </summary>
    [Parameter]
    public string HeroAlignment { get; set; } = "center";

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
    /// Gets or sets whether to show featured content section
    /// </summary>
    [Parameter]
    public bool ShowFeaturedContent { get; set; } = true;

    /// <summary>
    /// Gets or sets the featured section title
    /// </summary>
    [Parameter]
    public string FeaturedSectionTitle { get; set; } = "Featured Content";

    /// <summary>
    /// Gets or sets the featured section description
    /// </summary>
    [Parameter]
    public string? FeaturedSectionDescription { get; set; }

    /// <summary>
    /// Gets or sets the featured content layout
    /// </summary>
    [Parameter]
    public string FeaturedLayout { get; set; } = "grid";

    /// <summary>
    /// Gets or sets the number of featured columns
    /// </summary>
    [Parameter]
    public int FeaturedColumns { get; set; } = 3;

    /// <summary>
    /// Gets or sets the number of featured items to display
    /// </summary>
    [Parameter]
    public int FeaturedCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets whether to show excerpts in featured content
    /// </summary>
    [Parameter]
    public bool ShowFeaturedExcerpt { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show authors in featured content
    /// </summary>
    [Parameter]
    public bool ShowFeaturedAuthor { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show categories in featured content
    /// </summary>
    [Parameter]
    public bool ShowFeaturedCategories { get; set; } = true;

    /// <summary>
    /// Gets or sets the featured view all URL
    /// </summary>
    [Parameter]
    public string? FeaturedViewAllUrl { get; set; }

    /// <summary>
    /// Gets or sets whether to show the featured view all link
    /// </summary>
    [Parameter]
    public bool ShowFeaturedViewAll { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show recent content section
    /// </summary>
    [Parameter]
    public bool ShowRecentContent { get; set; } = true;

    /// <summary>
    /// Gets or sets the recent section title
    /// </summary>
    [Parameter]
    public string RecentSectionTitle { get; set; } = "Recent Posts";

    /// <summary>
    /// Gets or sets the number of recent items to display
    /// </summary>
    [Parameter]
    public int RecentCount { get; set; } = 6;

    /// <summary>
    /// Gets or sets whether to show categories section
    /// </summary>
    [Parameter]
    public bool ShowCategories { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show tags section
    /// </summary>
    [Parameter]
    public bool ShowTags { get; set; } = true;

    /// <summary>
    /// Gets or sets the browse section title
    /// </summary>
    [Parameter]
    public string BrowseSectionTitle { get; set; } = "Browse Content";

    /// <summary>
    /// Gets or sets the categories section title
    /// </summary>
    [Parameter]
    public string CategoriesSectionTitle { get; set; } = "Categories";

    /// <summary>
    /// Gets or sets the tags section title
    /// </summary>
    [Parameter]
    public string TagsSectionTitle { get; set; } = "Tags";

    /// <summary>
    /// Gets or sets the maximum number of categories to show
    /// </summary>
    [Parameter]
    public int MaxCategories { get; set; } = 8;

    /// <summary>
    /// Gets or sets the maximum number of tags to show
    /// </summary>
    [Parameter]
    public int MaxTags { get; set; } = 15;

    /// <summary>
    /// Gets or sets whether to show category counts
    /// </summary>
    [Parameter]
    public bool ShowCategoryCounts { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show tag counts
    /// </summary>
    [Parameter]
    public bool ShowTagCounts { get; set; } = true;

    /// <summary>
    /// Gets or sets custom HTML content to display
    /// </summary>
    [Parameter]
    public string? CustomContent { get; set; }

    /// <summary>
    /// Gets or sets the category URL formatter
    /// </summary>
    [Parameter]
    public Func<ContentCategory, string>? CategoryUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the tag URL formatter
    /// </summary>
    [Parameter]
    public Func<ContentTag, string>? TagUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the content URL formatter
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// Gets or sets the loading text
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Loading homepage...";

    /// <summary>
    /// Gets or sets the not found text
    /// </summary>
    [Parameter]
    public string NotFoundText { get; set; } = "Homepage content not found.";

    /// <summary>
    /// Gets or sets the read more text
    /// </summary>
    [Parameter]
    public string ReadMoreText { get; set; } = "Read more";

    /// <summary>
    /// Gets or sets additional child content
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the featured content items
    /// </summary>
    private IReadOnlyList<ContentItem>? FeaturedItems { get; set; }

    /// <summary>
    /// Gets or sets the recent content items
    /// </summary>
    private IReadOnlyList<ContentItem>? RecentItems { get; set; }

    /// <summary>
    /// Gets or sets whether the component is loading
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        if (Content is null && !string.IsNullOrWhiteSpace(ContentPath))
        {
            await LoadContentAsync();
        }
        else if (Content is not null)
        {
            await LoadRelatedContentAsync();
        }
        else
        {
            await LoadRelatedContentAsync();
        }
    }

    private async Task LoadContentAsync()
    {
        IsLoading = true;
        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                Content = await provider.GetItemByPathAsync(ContentPath);
                await LoadRelatedContentAsync();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading homepage content: {ex.Message}");
            Content = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadRelatedContentAsync()
    {
        try
        {
            var provider = ContentProviderManager.GetDefaultProvider();
            if (provider is not null)
            {
                // Load featured content
                if (ShowFeaturedContent)
                {
                    var featuredQuery = new ContentQuery
                    {
                        IsFeatured = true,
                        Locale = Locale,
                        Take = FeaturedCount,
                        SortBy = SortField.Date,
                        SortDirection = SortDirection.Descending
                    };
                    FeaturedItems = await provider.GetItemsByQueryAsync(featuredQuery);
                }

                // Load recent content
                if (ShowRecentContent)
                {
                    var recentQuery = new ContentQuery
                    {
                        Locale = Locale,
                        Take = RecentCount,
                        SortBy = SortField.Date,
                        SortDirection = SortDirection.Descending
                    };
                    RecentItems = await provider.GetItemsByQueryAsync(recentQuery);
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading related content: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private string GetHomePageClass()
    {
        return $"osirion-homepage {CssClass}".Trim();
    }
}