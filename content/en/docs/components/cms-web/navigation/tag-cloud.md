---
title: "TagCloud"
description: "A Blazor component for displaying an interactive cloud of content tags with customizable sorting, filtering, and URL generation for enhanced content discovery."
keywords: 
  - "tag-cloud"
  - "tags"
  - "content tags"
  - "content discovery"
  - "blazor cms"
  - "navigation"
seo:
  title: "TagCloud - Osirion Blazor CMS Web Component"
  description: "Learn how to implement tag clouds for content discovery in Osirion Blazor CMS with the TagCloud component featuring sorting, filtering, and customizable styling."
  keywords:
    - "blazor tag cloud"
    - "cms tags"
    - "content tags"
    - "osirion blazor"
date: "2024-12-29"
---

# TagCloud

The `TagCloud` component displays an interactive cloud of content tags, enabling users to discover related content through tag-based navigation. It supports automatic tag loading, custom sorting, count display, and flexible URL generation.

## Features

- **Automatic Tag Loading**: Automatically loads tags from content providers
- **Custom Tag Data**: Support for explicitly provided tag collections
- **Flexible Sorting**: Sort by tag count or alphabetically
- **Count Display**: Show/hide tag usage counts
- **Active Tag Highlighting**: Visual indication of currently selected tags
- **Custom URL Generation**: Flexible tag URL formatting
- **Loading States**: Built-in loading and empty state handling
- **Responsive Design**: Adapts to different screen sizes
- **Performance Optimized**: Efficient tag loading and rendering

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Tags` | `IReadOnlyList<ContentTag>?` | `null` | Explicit list of tags (overrides auto-loading) |
| `MaxTags` | `int?` | `null` | Maximum number of tags to display |
| `Title` | `string?` | `null` | Optional title displayed above the tag cloud |
| `LoadingText` | `string` | `"Loading tags..."` | Text shown during tag loading |
| `NoContentText` | `string` | `"No tags available."` | Text shown when no tags are found |
| `ActiveTag` | `string?` | `null` | Currently active/selected tag |
| `TagUrlFormatter` | `Func<ContentTag, string>?` | `null` | Custom function for generating tag URLs |
| `ShowCount` | `bool` | `true` | Whether to display tag usage counts |
| `SortByCount` | `bool` | `true` | Whether to sort tags by count (vs. alphabetically) |

## Data Structures

### ContentTag Class

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | The display name of the tag |
| `Slug` | `string` | URL-friendly tag identifier |
| `Count` | `int` | Number of content items with this tag |

## Basic Usage

### Simple Tag Cloud

```razor
@using Osirion.Blazor.Cms.Web.Components

<TagCloud />
```

### Tag Cloud with Title

```razor
<TagCloud 
    Title="Popular Tags"
    MaxTags="20" />
```

### Custom Tag Cloud

```razor
<TagCloud 
    Title="Explore Topics"
    MaxTags="15"
    SortByCount="true"
    ShowCount="false" />
```

## Advanced Examples

### Custom Tag URL Generation

```razor
<TagCloud 
    Title="Browse by Topic"
    TagUrlFormatter="@FormatTagUrl"
    MaxTags="25" />

@code {
    private string FormatTagUrl(ContentTag tag)
    {
        return $"/articles/tagged/{tag.Slug}?sort=recent";
    }
}
```

### Tag Cloud with Active State

```razor
@page "/tags/{ActiveTagSlug?}"
@using Osirion.Blazor.Cms.Web.Components

<div class="tag-page">
    <TagCloud 
        Title="All Tags"
        ActiveTag="@ActiveTagSlug"
        TagUrlFormatter="@FormatTagUrl" />
    
    @if (!string.IsNullOrEmpty(ActiveTagSlug))
    {
        <div class="tagged-content">
            <h2>Content tagged with "@ActiveTagSlug"</h2>
            <ContentList 
                Query="@GetTagQuery()"
                ShowSummary="true" />
        </div>
    }
</div>

@code {
    [Parameter] public string? ActiveTagSlug { get; set; }
    
    private string FormatTagUrl(ContentTag tag)
    {
        return $"/tags/{tag.Slug}";
    }
    
    private ContentQuery GetTagQuery()
    {
        return new ContentQuery
        {
            Tags = new[] { ActiveTagSlug! },
            SortBy = SortField.PublishedDate,
            SortDirection = SortDirection.Descending
        };
    }
}
```

### Explicit Tag Data

```razor
<TagCloud 
    Tags="@customTags"
    Title="Featured Topics"
    SortByCount="false"
    ShowCount="true"
    TagUrlFormatter="@CustomTagUrl" />

@code {
    private List<ContentTag> customTags = new()
    {
        new ContentTag { Name = "Technology", Slug = "technology", Count = 45 },
        new ContentTag { Name = "Design", Slug = "design", Count = 32 },
        new ContentTag { Name = "Business", Slug = "business", Count = 28 },
        new ContentTag { Name = "Development", Slug = "development", Count = 67 }
    };

    private string CustomTagUrl(ContentTag tag)
    {
        return $"/category/{tag.Slug}";
    }
}
```

### Filtered Tag Cloud

```razor
<div class="tag-filters">
    <TagCloud 
        Tags="@filteredTags"
        Title="@GetFilterTitle()"
        ShowCount="true" />
    
    <div class="filter-controls">
        <button @onclick="@(() => SetFilter("all"))" 
                class="@GetFilterClass("all")">
            All Tags
        </button>
        <button @onclick="@(() => SetFilter("popular"))" 
                class="@GetFilterClass("popular")">
            Popular (10+)
        </button>
        <button @onclick="@(() => SetFilter("recent"))" 
                class="@GetFilterClass("recent")">
            Recently Used
        </button>
    </div>
</div>

@code {
    private string currentFilter = "all";
    private List<ContentTag> allTags = new();
    private List<ContentTag> filteredTags = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadAllTags();
        ApplyFilter();
    }

    private void SetFilter(string filter)
    {
        currentFilter = filter;
        ApplyFilter();
        StateHasChanged();
    }

    private void ApplyFilter()
    {
        filteredTags = currentFilter switch
        {
            "popular" => allTags.Where(t => t.Count >= 10).ToList(),
            "recent" => allTags.OrderByDescending(t => t.LastUsed).Take(20).ToList(),
            _ => allTags
        };
    }

    private string GetFilterTitle() => currentFilter switch
    {
        "popular" => "Popular Tags",
        "recent" => "Recently Used Tags",
        _ => "All Tags"
    };

    private string GetFilterClass(string filter) =>
        currentFilter == filter ? "filter-button active" : "filter-button";
}
```

### Multi-Language Tag Cloud

```razor
<TagCloud 
    Tags="@localizedTags"
    Title="@Localizer["Tags"]"
    LoadingText="@Localizer["LoadingTags"]"
    NoContentText="@Localizer["NoTags"]"
    TagUrlFormatter="@LocalizedTagUrl" />

@code {
    [Inject] private IStringLocalizer<TagResources> Localizer { get; set; } = default!;
    [Inject] private ILocalizationService LocalizationService { get; set; } = default!;
    
    private List<ContentTag> localizedTags = new();

    protected override async Task OnInitializedAsync()
    {
        var currentLocale = LocalizationService.CurrentLocale;
        localizedTags = await LoadTagsForLocale(currentLocale);
    }

    private string LocalizedTagUrl(ContentTag tag)
    {
        var locale = LocalizationService.CurrentLocale;
        return $"/{locale}/tags/{tag.Slug}";
    }
}
```

### Responsive Tag Cloud with Size Variants

```razor
<div class="responsive-tag-container">
    <!-- Large screens: Full tag cloud -->
    <div class="d-none d-lg-block">
        <TagCloud 
            Title="Explore Topics"
            MaxTags="50"
            ShowCount="true"
            CssClass="large-tag-cloud" />
    </div>
    
    <!-- Medium screens: Limited tags -->
    <div class="d-none d-md-block d-lg-none">
        <TagCloud 
            Title="Popular Topics"
            MaxTags="25"
            ShowCount="false"
            CssClass="medium-tag-cloud" />
    </div>
    
    <!-- Small screens: Top tags only -->
    <div class="d-block d-md-none">
        <TagCloud 
            MaxTags="10"
            ShowCount="false"
            CssClass="small-tag-cloud" />
    </div>
</div>
```

## CSS Customization

### Default Styling

```css
.osirion-tag-cloud {
    margin-bottom: 2rem;
}

.osirion-tag-cloud-title {
    margin-bottom: 1rem;
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--heading-color);
}

.osirion-tags-container {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    align-items: center;
}

.osirion-tag-link {
    display: inline-flex;
    align-items: center;
    padding: 0.25rem 0.75rem;
    background-color: var(--tag-background);
    color: var(--tag-color);
    text-decoration: none;
    border-radius: 1rem;
    font-size: 0.875rem;
    border: 1px solid var(--tag-border);
    transition: all 0.2s ease;
}

.osirion-tag-link:hover {
    background-color: var(--tag-hover-background);
    color: var(--tag-hover-color);
    border-color: var(--tag-hover-border);
    transform: translateY(-1px);
}

.osirion-tag-active {
    background-color: var(--primary-color);
    color: white;
    border-color: var(--primary-color);
}

.osirion-tag-count {
    margin-left: 0.375rem;
    padding: 0.125rem 0.375rem;
    background-color: rgba(255, 255, 255, 0.2);
    border-radius: 0.75rem;
    font-size: 0.75rem;
    font-weight: 500;
}
```

### Weighted Tag Sizes

```css
.osirion-tag-cloud.weighted .osirion-tag-link {
    font-size: calc(0.75rem + var(--tag-weight) * 0.5rem);
    font-weight: calc(400 + var(--tag-weight) * 200);
}

/* CSS custom properties set via JavaScript based on tag count */
.osirion-tag-link[data-count="1"] { --tag-weight: 0.2; }
.osirion-tag-link[data-count="2"] { --tag-weight: 0.3; }
.osirion-tag-link[data-count="3"] { --tag-weight: 0.4; }
.osirion-tag-link[data-count="4"] { --tag-weight: 0.6; }
.osirion-tag-link[data-count="5"] { --tag-weight: 0.8; }
.osirion-tag-link[data-count="6"] { --tag-weight: 1.0; }
```

### Compact Tag Cloud

```css
.osirion-tag-cloud.compact {
    margin-bottom: 1rem;
}

.osirion-tag-cloud.compact .osirion-tag-link {
    padding: 0.125rem 0.5rem;
    font-size: 0.75rem;
    border-radius: 0.5rem;
}

.osirion-tag-cloud.compact .osirion-tag-count {
    margin-left: 0.25rem;
    padding: 0.0625rem 0.25rem;
    font-size: 0.625rem;
}
```

### Colorful Tag Cloud

```css
.osirion-tag-cloud.colorful .osirion-tag-link {
    background: linear-gradient(45deg, 
        hsl(calc(var(--tag-index) * 137.5deg), 70%, 85%), 
        hsl(calc(var(--tag-index) * 137.5deg + 30deg), 70%, 75%));
    color: hsl(calc(var(--tag-index) * 137.5deg), 70%, 25%);
    border: none;
}

.osirion-tag-cloud.colorful .osirion-tag-link:hover {
    filter: brightness(1.1) saturate(1.2);
}
```

## Framework-Specific Styling

### Bootstrap Integration

```css
.osirion-tag-cloud.bootstrap .osirion-tag-link {
    @extend .badge, .badge-outline-primary;
    margin-right: 0.25rem;
    margin-bottom: 0.25rem;
}

.osirion-tag-cloud.bootstrap .osirion-tag-active {
    @extend .badge-primary;
}
```

### Tailwind CSS

```css
.osirion-tag-link {
    @apply inline-flex items-center px-3 py-1 rounded-full text-sm font-medium;
    @apply bg-gray-100 text-gray-800 border border-gray-200;
    @apply hover:bg-gray-200 hover:text-gray-900 hover:-translate-y-0.5;
    @apply transition-all duration-200;
}

.osirion-tag-active {
    @apply bg-blue-600 text-white border-blue-600;
}

.osirion-tag-count {
    @apply ml-1.5 px-1.5 py-0.5 bg-white bg-opacity-20 rounded-full text-xs font-semibold;
}
```

## JavaScript Enhancements

### Weighted Tag Sizing

```javascript
document.addEventListener('DOMContentLoaded', function() {
    const tagCloud = document.querySelector('.osirion-tag-cloud.weighted');
    if (!tagCloud) return;

    const tags = tagCloud.querySelectorAll('.osirion-tag-link');
    const counts = Array.from(tags).map(tag => parseInt(tag.dataset.count || '1'));
    const maxCount = Math.max(...counts);
    const minCount = Math.min(...counts);

    tags.forEach((tag, index) => {
        const count = counts[index];
        const weight = (count - minCount) / (maxCount - minCount);
        tag.style.setProperty('--tag-weight', weight.toString());
        tag.style.setProperty('--tag-index', index.toString());
    });
});
```

### Tag Cloud Animation

```javascript
function animateTagCloud() {
    const tags = document.querySelectorAll('.osirion-tag-link');
    
    tags.forEach((tag, index) => {
        tag.style.opacity = '0';
        tag.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            tag.style.transition = 'all 0.3s ease';
            tag.style.opacity = '1';
            tag.style.transform = 'translateY(0)';
        }, index * 50);
    });
}
```

## Accessibility Features

- **Keyboard Navigation**: Full keyboard support for tag links
- **Screen Reader Support**: Proper labeling and structure
- **Focus Management**: Clear focus indicators
- **Semantic HTML**: Uses appropriate link elements
- **ARIA Attributes**: Enhanced accessibility information

## Best Practices

1. **Tag Limits**: Use reasonable tag limits to avoid overwhelming users
2. **Visual Hierarchy**: Use size or color to indicate tag popularity
3. **Performance**: Consider caching tag data for frequently accessed pages
4. **Mobile Experience**: Ensure tags are easily tappable on mobile devices
5. **Search Integration**: Combine with search functionality for better discovery
6. **Content Strategy**: Maintain consistent tagging across your content

## Integration Examples

### Blog with Tag Cloud Sidebar

```razor
@page "/blog"
@using Osirion.Blazor.Cms.Web.Components

<div class="blog-layout">
    <main class="blog-content">
        <h1>Blog Posts</h1>
        <ContentList 
            Query="@GetBlogQuery()"
            ShowSummary="true"
            ShowTags="true" />
    </main>
    
    <aside class="blog-sidebar">
        <TagCloud 
            Title="Popular Topics"
            MaxTags="20"
            TagUrlFormatter="@FormatBlogTagUrl"
            ActiveTag="@SelectedTag" />
        
        <CategoriesList 
            Title="Categories"
            MaxCategories="10" />
    </aside>
</div>

@code {
    [Parameter, SupplyParameterFromQuery] public string? SelectedTag { get; set; }

    private ContentQuery GetBlogQuery()
    {
        var query = new ContentQuery
        {
            ContentType = "blog-post",
            SortBy = SortField.PublishedDate,
            SortDirection = SortDirection.Descending
        };

        if (!string.IsNullOrEmpty(SelectedTag))
        {
            query.Tags = new[] { SelectedTag };
        }

        return query;
    }

    private string FormatBlogTagUrl(ContentTag tag)
    {
        return $"/blog?tag={tag.Slug}";
    }
}
```

### Tag-Based Content Discovery

```razor
<div class="content-discovery">
    <section class="discovery-section">
        <h2>Discover Content</h2>
        <TagCloud 
            Title="Browse by Topic"
            MaxTags="30"
            SortByCount="true"
            TagUrlFormatter="@DiscoveryTagUrl" />
    </section>
    
    <section class="trending-section">
        <h2>Trending Now</h2>
        <TagCloud 
            Tags="@trendingTags"
            ShowCount="false"
            CssClass="trending-tags"
            TagUrlFormatter="@TrendingTagUrl" />
    </section>
</div>

@code {
    private List<ContentTag> trendingTags = new();

    protected override async Task OnInitializedAsync()
    {
        trendingTags = await GetTrendingTags();
    }

    private string DiscoveryTagUrl(ContentTag tag) => $"/discover/{tag.Slug}";
    private string TrendingTagUrl(ContentTag tag) => $"/trending/{tag.Slug}";
}
```

## Related Components

- [`CategoriesList`](categories-list.md) - For category-based navigation
- [`ContentList`](../core/content-list.md) - For displaying tagged content
- [`SearchBox`](search-box.md) - For search-based content discovery
- [`ContentView`](../core/content-view.md) - For displaying individual content items
