using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Enums;
using Osirion.Blazor.Cms.Domain.Repositories;

namespace Osirion.Blazor.Cms.Components;

/// <summary>
/// HomePage component for displaying homepage content with hero section, featured content, and navigation
/// </summary>
public partial class HomePage
{

    /// <summary>
    /// Has no effect: <c>HomePage</c> does not load by path. Pass the item as <see cref="Content"/>.
    /// </summary>
    [Parameter]
    public string? ContentPath { get; set; }

    /// <summary>
    /// The item that makes the page: its title and description are the hero's, its featured image the hero's image
    /// and its body follows the hero (without its own leading <c>h1</c> while the hero is shown). With neither
    /// <c>Content</c> nor <see cref="ChildContent"/> the page shows the not-found state and signals 404.
    /// </summary>
    [Parameter]
    public ContentItem? Content { get; set; }

    /// <summary>
    /// Whether to show the featured section, an <c>OsirionContentListSection</c> of the newest <see
    /// cref="FeaturedCount"/> items of the page's <c>Locale</c> that are marked <c>featured: true</c> in their front
    /// matter. Without such items the section is left out.
    /// </summary>
    [Parameter]
    public bool ShowFeaturedContent { get; set; } = true;

    /// <summary>
    /// The heading of the featured section.
    /// </summary>
    [Parameter]
    public string FeaturedSectionTitle { get; set; } = "Featured Content";

    /// <summary>
    /// The sentence under the featured section's heading. None when null.
    /// </summary>
    [Parameter]
    public string? FeaturedSectionDescription { get; set; }

    /// <summary>
    /// The layout of the featured items: <c>grid</c>, <c>list</c>, <c>carousel</c> or <c>featured-first</c>.
    /// </summary>
    [Parameter]
    public string FeaturedLayout { get; set; } = "grid";

    /// <summary>
    /// The number of columns of the featured items in the <c>grid</c> layout.
    /// </summary>
    [Parameter]
    public int FeaturedColumns { get; set; } = 3;

    /// <summary>
    /// The number of items the featured section shows.
    /// </summary>
    [Parameter]
    public int FeaturedCount { get; set; } = 3;

    /// <summary>
    /// Whether each featured item shows an excerpt of its body.
    /// </summary>
    [Parameter]
    public bool ShowFeaturedExcerpt { get; set; } = true;

    /// <summary>
    /// Whether each featured item shows its author.
    /// </summary>
    [Parameter]
    public bool ShowFeaturedAuthor { get; set; } = true;

    /// <summary>
    /// Whether each featured item shows its categories.
    /// </summary>
    [Parameter]
    public bool ShowFeaturedCategories { get; set; } = true;

    /// <summary>
    /// The address of the "View all posts" link under the featured items. The link needs this and <see
    /// cref="ShowFeaturedViewAll"/>.
    /// </summary>
    [Parameter]
    public string? FeaturedViewAllUrl { get; set; }

    /// <summary>
    /// Whether the "View all posts" link shows. It also needs <see cref="FeaturedViewAllUrl"/>.
    /// </summary>
    [Parameter]
    public bool ShowFeaturedViewAll { get; set; } = true;

    /// <summary>
    /// Whether to show the recent section: the newest <see cref="RecentCount"/> items of the page's <c>Locale</c>,
    /// featured or not, under <see cref="RecentSectionTitle"/>. Without items the section is left out.
    /// </summary>
    [Parameter]
    public bool ShowRecentContent { get; set; } = true;

    /// <summary>
    /// The heading of the recent section.
    /// </summary>
    [Parameter]
    public string RecentSectionTitle { get; set; } = "Recent Posts";

    /// <summary>
    /// The number of items the recent section shows.
    /// </summary>
    [Parameter]
    public int RecentCount { get; set; } = 6;

    /// <summary>
    /// Whether the browse section lists the categories, under the heading <c>CategoriesSectionTitle</c>.
    /// </summary>
    [Parameter]
    public bool ShowCategories { get; set; } = false;

    /// <summary>
    /// Whether the browse section shows the tag cloud, under the heading <c>TagsSectionTitle</c>.
    /// </summary>
    [Parameter]
    public bool ShowTags { get; set; } = false;

    /// <summary>
    /// The heading of the browse section, which holds the category list and the tag cloud.
    /// </summary>
    [Parameter]
    public string BrowseSectionTitle { get; set; } = "Browse Content";

    /// <summary>
    /// The most categories the browse section lists.
    /// </summary>
    [Parameter]
    public int MaxCategories { get; set; } = 8;

    /// <summary>
    /// The most tags the browse section shows.
    /// </summary>
    [Parameter]
    public int MaxTags { get; set; } = 15;

    /// <summary>
    /// Whether each listed category shows its item count.
    /// </summary>
    [Parameter]
    public bool ShowCategoryCounts { get; set; } = true;

    /// <summary>
    /// Whether each tag shows its item count.
    /// </summary>
    [Parameter]
    public bool ShowTagCounts { get; set; } = true;

    /// <summary>
    /// HTML shown in a section after the other sections. It is written through <c>OsirionHtmlRenderer</c>, which
    /// sanitizes it with the registered HTML sanitizer.
    /// </summary>
    [Parameter]
    public string? CustomContent { get; set; }

    /// <summary>
    /// Builds the link of each item in the featured and recent sections from the item. Without it a link is
    /// <c>/{item.Url}</c>.
    /// </summary>
    [Parameter]
    public Func<ContentItem, string>? ContentUrlFormatter { get; set; }

    /// <summary>
    /// The text of the link under each item of the recent section. No link is shown when it is empty.
    /// </summary>
    [Parameter]
    public string ReadMoreText { get; set; } = "Read more";

    /// <summary>
    /// Content shown after the body and before the featured section. Child content alone keeps the page from the
    /// not-found state.
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

    /// <inheritdoc/>
    protected override bool LoadsItem => false;

    // What the sections were loaded for, so the page queries again only when one of these changes.
    private (string? Locale, bool Featured, int FeaturedCount, bool Recent, int RecentCount)? _loadedSections;

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        await LoadSectionsAsync();

        if (NoContent() && !IsLoading)
        {
            SignalNotFound();
        }
        else
        {
            ResetNotFoundSignal();
        }
    }

    private async Task LoadSectionsAsync()
    {
        var sections = (Locale, ShowFeaturedContent, FeaturedCount, ShowRecentContent, RecentCount);
        if (_loadedSections == sections)
        {
            return;
        }

        _loadedSections = sections;
        IsLoading = true;
        try
        {
            FeaturedItems = ShowFeaturedContent && FeaturedCount > 0
                ? await QueryAsync(new ContentQuery { IsFeatured = true, Locale = Locale, Take = FeaturedCount })
                : null;
            RecentItems = ShowRecentContent && RecentCount > 0
                ? await QueryAsync(new ContentQuery { Locale = Locale, Take = RecentCount })
                : null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    // A failed query leaves the section out and keeps the page, as the lists do.
    private async Task<IReadOnlyList<ContentItem>?> QueryAsync(ContentQuery query)
    {
        query.SortBy = SortField.Date;
        query.SortDirection = Osirion.Blazor.Cms.Domain.Enums.SortDirection.Descending;
        try
        {
            return await ContentProviderManager.GetContentByQueryAsync(query);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load the home page items (featured {Featured}, locale {Locale})", query.IsFeatured, query.Locale);
            return null;
        }
    }

    private bool NoContent() => Content is null && ChildContent is null;
}
