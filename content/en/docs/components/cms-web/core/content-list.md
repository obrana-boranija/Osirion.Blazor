---
title: "Content List - Osirion Blazor CMS"
description: "Display lists of CMS content with pagination, filtering, and customizable layouts for Blazor applications."
category: "CMS Web Components"
subcategory: "Core"
tags: ["cms", "content", "list", "pagination", "filtering", "blog"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "content-list"
section: "components"
layout: "component"
seo:
  title: "Content List Component | Osirion Blazor CMS Documentation"
  description: "Learn how to display content lists with filtering, pagination, and sorting using ContentList component in Osirion Blazor CMS."
  keywords: ["Blazor", "CMS", "content list", "pagination", "filtering", "blog posts", "articles"]
  canonical: "/docs/components/cms.web/core/content-list"
  image: "/images/components/content-list-preview.jpg"
navigation:
  parent: "CMS Web Components"
  order: 1
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Core"
    link: "/docs/components/cms.web/core"
  - text: "Content List"
    link: "/docs/components/cms.web/core/content-list"
---

# Content List Component

The **ContentList** component is the foundation for displaying collections of CMS content. It provides comprehensive functionality for listing blog posts, articles, documents, and other content types with built-in filtering, pagination, sorting, and responsive layouts.

## Overview

This component automatically loads and displays content from your CMS, with support for real-time updates, SEO optimization, and flexible customization. It's perfect for blog home pages, category listings, search results, and any scenario where you need to display multiple content items.

## Key Features

- **Flexible Content Loading**: Support for multiple content providers and sources
- **Advanced Filtering**: Filter by directory, category, tags, featured status, and locale
- **Intelligent Pagination**: Built-in pagination with SEO-friendly URLs
- **Responsive Grid Layout**: Mobile-first design with adaptive card layouts
- **Real-time Updates**: Automatic content refresh when underlying data changes
- **Performance Optimized**: Lazy loading and efficient rendering
- **SEO Friendly**: Proper meta tags and structured data support
- **Accessibility Ready**: WCAG compliant with screen reader support
- **Customizable Templates**: Flexible content display with custom formatters
- **Loading States**: Professional loading and empty state handling

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Directory` | `string?` | `null` | Content directory path for filtering |
| `Category` | `string?` | `null` | Category filter for content items |
| `Tag` | `string?` | `null` | Tag filter for content items |
| `OnlyFeatured` | `bool?` | `null` | Show only featured content |
| `FeaturedCount` | `int?` | `null` | Number of featured items to display |
| `LoadingText` | `string` | `"Loading content..."` | Text displayed during loading |
| `NoContentText` | `string` | `"No content available."` | Text when no content found |
| `ReadMoreText` | `string` | `"Read more"` | Text for read more links |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Custom URL formatting function |
| `ShowPagination` | `bool` | `false` | Enable pagination display |
| `ItemsPerPage` | `int` | `10` | Items to display per page |
| `CurrentPage` | `int` | `1` | Current page number |
| `PageChanged` | `EventCallback<int>` | - | Callback when page changes |
| `PaginationUrlFormatter` | `Func<int, string>?` | `null` | Custom pagination URL formatter |
| `Locale` | `string?` | `null` | Locale for content filtering |
| `DirectoryId` | `string?` | `null` | Directory ID for filtering |
| `IgnorePathSegment` | `string?` | `null` | Path segment to ignore in URLs |
| `PathReplacePattern` | `string?` | `null` | Regex pattern for path replacement |
| `SortBy` | `SortField` | `SortField.Date` | Field to sort content by |
| `SortDirection` | `SortDirection` | `SortDirection.Descending` | Sort direction |
| `OnItemSelected` | `EventCallback<ContentItem>` | - | Callback when item is selected |

## Basic Usage

### Simple Content List

```razor
@using Osirion.Blazor.Cms.Web.Components

<ContentList Directory="/blog" 
             ItemsPerPage="6" 
             ShowPagination="true" />
```

### Filtered Content Display

```razor
<ContentList Category="technology"
             Tag="blazor"
             OnlyFeatured="true"
             FeaturedCount="3" />
```

## Advanced Examples

### Blog Home Page

```razor
<div class="blog-home">
    <section class="featured-posts mb-5">
        <h2>Featured Articles</h2>
        <ContentList Directory="/blog"
                     OnlyFeatured="true"
                     FeaturedCount="3"
                     SortBy="SortField.Date"
                     SortDirection="SortDirection.Descending" />
    </section>
    
    <section class="recent-posts">
        <h2>Latest Posts</h2>
        <ContentList Directory="/blog"
                     ItemsPerPage="9"
                     ShowPagination="true"
                     CurrentPage="@currentPage"
                     PageChanged="HandlePageChanged"
                     ContentUrlFormatter="FormatBlogUrl" />
    </section>
</div>

@code {
    private int currentPage = 1;
    
    private async Task HandlePageChanged(int newPage)
    {
        currentPage = newPage;
        await InvokeAsync(StateHasChanged);
    }
    
    private string FormatBlogUrl(ContentItem item)
    {
        return $"/blog/{item.Slug}";
    }
}
```

### Category-Based Listing

```razor
<div class="category-content">
    <header class="category-header">
        <h1>@CategoryName Articles</h1>
        <p class="category-description">@CategoryDescription</p>
    </header>
    
    <ContentList Category="@CategorySlug"
                 ItemsPerPage="12"
                 ShowPagination="true"
                 SortBy="@selectedSort"
                 SortDirection="@sortDirection"
                 OnItemSelected="HandleItemSelected" />
    
    <div class="sorting-controls mt-4">
        <label for="sortSelect">Sort by:</label>
        <select id="sortSelect" class="form-select" @onchange="HandleSortChange">
            <option value="Date">Latest First</option>
            <option value="Title">Title A-Z</option>
            <option value="ReadTime">Read Time</option>
            <option value="Views">Most Popular</option>
        </select>
    </div>
</div>

@code {
    [Parameter] public string? CategorySlug { get; set; }
    
    private string CategoryName = string.Empty;
    private string CategoryDescription = string.Empty;
    private SortField selectedSort = SortField.Date;
    private SortDirection sortDirection = SortDirection.Descending;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(CategorySlug))
        {
            // Load category metadata
            var category = await CategoryService.GetBySlugAsync(CategorySlug);
            CategoryName = category?.Name ?? "Category";
            CategoryDescription = category?.Description ?? "";
        }
    }
    
    private void HandleSortChange(ChangeEventArgs e)
    {
        if (Enum.TryParse<SortField>(e.Value?.ToString(), out var newSort))
        {
            selectedSort = newSort;
        }
    }
    
    private async Task HandleItemSelected(ContentItem item)
    {
        // Track analytics or perform custom actions
        await AnalyticsService.TrackContentView(item.Id);
        Navigation.NavigateTo($"/articles/{item.Slug}");
    }
}
```

### Multi-Language Content

```razor
<div class="localized-content">
    <div class="language-selector mb-3">
        @foreach (var locale in supportedLocales)
        {
            <button class="btn @(locale == currentLocale ? "btn-primary" : "btn-outline-primary") me-2"
                    @onclick="() => ChangeLocale(locale)">
                @GetLocaleName(locale)
            </button>
        }
    </div>
    
    <ContentList Directory="/articles"
                 Locale="@currentLocale"
                 ItemsPerPage="8"
                 ShowPagination="true"
                 LoadingText="@GetLocalizedText("loading")"
                 NoContentText="@GetLocalizedText("no_content")"
                 ReadMoreText="@GetLocalizedText("read_more")" />
</div>

@code {
    private string currentLocale = "en";
    private readonly string[] supportedLocales = { "en", "es", "fr", "de" };
    
    private void ChangeLocale(string locale)
    {
        currentLocale = locale;
        // Update URL or state as needed
    }
    
    private string GetLocaleName(string locale) => locale switch
    {
        "en" => "English",
        "es" => "Español",
        "fr" => "Français",
        "de" => "Deutsch",
        _ => locale.ToUpper()
    };
    
    private string GetLocalizedText(string key)
    {
        // Implement localization logic
        return Localizer[key];
    }
}
```

### Search Results

```razor
<div class="search-results">
    <header class="search-header">
        <h1>Search Results</h1>
        @if (!string.IsNullOrEmpty(SearchQuery))
        {
            <p>Results for: <strong>"@SearchQuery"</strong></p>
        }
    </header>
    
    <ContentList Directory="@GetSearchDirectory()"
                 ItemsPerPage="10"
                 ShowPagination="true"
                 CurrentPage="@currentPage"
                 PageChanged="HandlePageChanged"
                 NoContentText="@GetNoResultsText()" />
</div>

@code {
    [Parameter] public string? SearchQuery { get; set; }
    [SupplyParameterFromQuery] public int Page { get; set; } = 1;
    
    private int currentPage => Page;
    
    private string GetSearchDirectory()
    {
        // Implement search logic that returns appropriate directory
        return string.IsNullOrEmpty(SearchQuery) ? "/" : $"/search/{Uri.EscapeDataString(SearchQuery)}";
    }
    
    private string GetNoResultsText()
    {
        return string.IsNullOrEmpty(SearchQuery) 
            ? "Enter a search term to find content."
            : $"No results found for \"{SearchQuery}\". Try different keywords.";
    }
    
    private async Task HandlePageChanged(int newPage)
    {
        await Navigation.NavigateToAsync($"/search?q={Uri.EscapeDataString(SearchQuery)}&page={newPage}");
    }
}
```

### Custom Content Cards

```razor
<ContentList Directory="/portfolio"
             ItemsPerPage="6"
             ShowPagination="true"
             ContentUrlFormatter="FormatPortfolioUrl"
             OnItemSelected="HandlePortfolioClick">
    
    @* Custom card template using CSS classes *@
    <style>
        .osirion-content-card {
            border-radius: 12px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            transition: transform 0.2s ease;
        }
        
        .osirion-content-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
        }
        
        .osirion-content-featured-image {
            height: 200px;
            object-fit: cover;
            border-radius: 12px 12px 0 0;
        }
    </style>
</ContentList>

@code {
    private string FormatPortfolioUrl(ContentItem item)
    {
        return $"/portfolio/{item.Category?.ToLower()}/{item.Slug}";
    }
    
    private async Task HandlePortfolioClick(ContentItem item)
    {
        // Custom analytics or actions
        await PortfolioService.IncrementViewCount(item.Id);
    }
}
```

### Dynamic Loading with Filters

```razor
<div class="dynamic-content-list">
    <div class="filter-controls mb-4">
        <div class="row">
            <div class="col-md-3">
                <label class="form-label">Category</label>
                <select class="form-select" @bind="selectedCategory">
                    <option value="">All Categories</option>
                    @foreach (var category in categories)
                    {
                        <option value="@category.Slug">@category.Name</option>
                    }
                </select>
            </div>
            
            <div class="col-md-3">
                <label class="form-label">Tag</label>
                <select class="form-select" @bind="selectedTag">
                    <option value="">All Tags</option>
                    @foreach (var tag in tags)
                    {
                        <option value="@tag">@tag</option>
                    }
                </select>
            </div>
            
            <div class="col-md-3">
                <div class="form-check mt-4">
                    <input class="form-check-input" type="checkbox" @bind="onlyFeatured" id="featuredCheck">
                    <label class="form-check-label" for="featuredCheck">
                        Featured Only
                    </label>
                </div>
            </div>
            
            <div class="col-md-3">
                <label class="form-label">&nbsp;</label>
                <button class="btn btn-primary d-block" @onclick="ApplyFilters">
                    Apply Filters
                </button>
            </div>
        </div>
    </div>
    
    <ContentList Category="@appliedCategory"
                 Tag="@appliedTag"
                 OnlyFeatured="@appliedFeatured"
                 ItemsPerPage="9"
                 ShowPagination="true" />
</div>

@code {
    private string selectedCategory = string.Empty;
    private string selectedTag = string.Empty;
    private bool onlyFeatured = false;
    
    private string appliedCategory = string.Empty;
    private string appliedTag = string.Empty;
    private bool appliedFeatured = false;
    
    private List<Category> categories = new();
    private List<string> tags = new();
    
    protected override async Task OnInitializedAsync()
    {
        categories = await ContentService.GetCategoriesAsync();
        tags = await ContentService.GetTagsAsync();
    }
    
    private void ApplyFilters()
    {
        appliedCategory = selectedCategory;
        appliedTag = selectedTag;
        appliedFeatured = onlyFeatured;
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-content-list {
    /* Main container */
}

.osirion-content-loading {
    /* Loading state container */
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 2rem;
}

.osirion-spinner {
    /* Loading spinner */
    width: 2rem;
    height: 2rem;
    border: 2px solid #f3f3f3;
    border-top: 2px solid #007bff;
    border-radius: 50%;
    animation: spin 1s linear infinite;
}

.osirion-no-content {
    /* Empty state container */
    text-align: center;
    padding: 3rem 1rem;
    color: var(--bs-text-muted);
}

.osirion-content-grid {
    /* Content grid layout */
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 1.5rem;
}

.osirion-content-card {
    /* Individual content card */
    background: white;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    transition: transform 0.2s ease;
}

.osirion-content-pagination {
    /* Pagination container */
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 0.5rem;
    margin-top: 2rem;
}
```

### Custom Styling Examples

```css
/* Modern card design */
.modern-cards .osirion-content-card {
    border-radius: 16px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    border: 1px solid rgba(0, 0, 0, 0.05);
}

.modern-cards .osirion-content-card:hover {
    transform: translateY(-8px);
    box-shadow: 0 12px 40px rgba(0, 0, 0, 0.15);
}

/* Masonry layout */
.masonry-layout .osirion-content-grid {
    columns: 3;
    column-gap: 1.5rem;
    column-fill: balance;
}

.masonry-layout .osirion-content-card {
    break-inside: avoid;
    margin-bottom: 1.5rem;
}

/* Compact list view */
.compact-view .osirion-content-grid {
    display: block;
}

.compact-view .osirion-content-card {
    display: flex;
    margin-bottom: 1rem;
    height: 120px;
}

.compact-view .osirion-content-featured-image {
    width: 200px;
    height: 120px;
    object-fit: cover;
}
```

## Performance Optimization

### Lazy Loading Implementation

```razor
<ContentList Directory="/blog"
             ItemsPerPage="6"
             ShowPagination="false"
             @ref="contentListRef" />

<div class="load-more-container" @ref="loadMoreTrigger">
    @if (hasMoreContent)
    {
        <button class="btn btn-outline-primary" @onclick="LoadMoreContent">
            Load More Articles
        </button>
    }
</div>

@code {
    private ContentList? contentListRef;
    private ElementReference loadMoreTrigger;
    private bool hasMoreContent = true;
    private int currentPage = 1;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetupIntersectionObserver();
        }
    }
    
    private async Task SetupIntersectionObserver()
    {
        await JSRuntime.InvokeVoidAsync("setupIntersectionObserver", 
            loadMoreTrigger, 
            DotNetObjectReference.Create(this));
    }
    
    [JSInvokable]
    public async Task LoadMoreContent()
    {
        currentPage++;
        // Implementation for loading additional content
        await InvokeAsync(StateHasChanged);
    }
}
```

### Caching Strategy

```razor
@implements IDisposable

<ContentList Directory="@cacheKey"
             @key="@cacheKey" />

@code {
    private string cacheKey = string.Empty;
    private Timer? cacheTimer;
    
    protected override void OnInitialized()
    {
        cacheKey = $"{Directory}-{Category}-{Tag}";
        
        // Set up cache invalidation
        cacheTimer = new Timer(InvalidateCache, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    private void InvalidateCache(object? state)
    {
        InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
        cacheTimer?.Dispose();
    }
}
```

## Accessibility Features

- **Semantic HTML**: Uses proper `<article>` and `<section>` elements
- **ARIA Labels**: Comprehensive labeling for screen readers
- **Keyboard Navigation**: Full keyboard support for pagination
- **Focus Management**: Proper focus indicators and management
- **Screen Reader Support**: Descriptive content for assistive technologies

### Accessibility Enhancement

```razor
<ContentList Directory="/articles"
             aria-label="Article listing"
             role="region" />

<style>
    .osirion-content-card:focus-within {
        outline: 2px solid var(--bs-primary);
        outline-offset: 2px;
    }
    
    .osirion-pagination-link:focus {
        outline: 2px solid var(--bs-primary);
        outline-offset: 2px;
    }
</style>
```

## Integration Examples

### With State Management

```razor
@using Fluxor
@inherits FluxorComponent

<ContentList Category="@ContentState.Value.SelectedCategory"
             Tag="@ContentState.Value.SelectedTag"
             CurrentPage="@ContentState.Value.CurrentPage"
             PageChanged="DispatchPageChange" />

@code {
    [Inject] private IState<ContentState> ContentState { get; set; } = null!;
    [Inject] private IDispatcher Dispatcher { get; set; } = null!;
    
    private void DispatchPageChange(int newPage)
    {
        Dispatcher.Dispatch(new ChangePageAction(newPage));
    }
}
```

### With Analytics

```razor
<ContentList Directory="/blog"
             OnItemSelected="TrackContentClick"
             ContentUrlFormatter="CreateTrackingUrl" />

@code {
    private async Task TrackContentClick(ContentItem item)
    {
        await AnalyticsService.TrackEvent("content_click", new
        {
            content_id = item.Id,
            content_title = item.Title,
            content_category = item.Category
        });
    }
    
    private string CreateTrackingUrl(ContentItem item)
    {
        var baseUrl = $"/articles/{item.Slug}";
        return $"{baseUrl}?utm_source=content_list&utm_medium=card_click";
    }
}
```

## Best Practices

1. **Performance**: Use pagination for large content sets
2. **SEO**: Implement proper URL structures and meta tags
3. **UX**: Provide clear loading and empty states
4. **Accessibility**: Ensure keyboard navigation and screen reader support
5. **Mobile**: Test responsive behavior on various devices
6. **Caching**: Implement appropriate caching strategies
7. **Analytics**: Track user interactions for insights
8. **Error Handling**: Gracefully handle content loading failures

## Common Use Cases

- **Blog Home Pages**: Display latest and featured posts
- **Category Listings**: Show content filtered by category
- **Search Results**: Display search results with pagination
- **Portfolio Galleries**: Showcase work with visual cards
- **Documentation Lists**: Organize technical documentation
- **News Feeds**: Display news articles and updates
- **Product Catalogs**: List products with filtering options

The ContentList component provides a robust foundation for any content-driven Blazor application, offering the flexibility and performance needed for modern web experiences.
