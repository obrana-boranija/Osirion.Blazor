using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Cms.Domain.Entities;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// HomePage component for displaying homepage content with hero section, featured content, and navigation
/// </summary>
public partial class HomePage
{

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
    /// Gets or sets the content URL formatter
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

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

    private string GetHomePageClass()
    {
        return $"osirion-homepage {Class}".Trim();
    }
}