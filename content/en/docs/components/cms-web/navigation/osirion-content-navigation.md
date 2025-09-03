---
title: "OsirionContentNavigation"
description: "A Blazor component for creating previous/next navigation between content items with customizable display variants and automatic discovery."
keywords: 
  - "osirion-content-navigation"
  - "content pagination"
  - "navigation"
  - "previous-next"
  - "blazor cms"
  - "content discovery"
seo:
  title: "OsirionContentNavigation - Osirion Blazor CMS Web Component"
  description: "Learn how to implement content navigation between pages in Osirion Blazor CMS with customizable display variants, automatic discovery, and accessibility features."
  keywords:
    - "blazor content navigation"
    - "cms pagination"
    - "previous next navigation"
    - "osirion blazor"
date: "2024-12-29"
---

# OsirionContentNavigation

The `OsirionContentNavigation` component provides previous/next navigation between content items with customizable display variants, automatic discovery, and full accessibility support. It can automatically discover adjacent items or use explicitly provided items.

## Features

- **Automatic Item Discovery**: Automatically finds previous and next items in the same directory
- **Custom Navigation Items**: Support for explicitly provided previous and next items
- **Multiple Display Variants**: Default, compact, and card-based styles
- **Customizable Icons**: Support for custom previous and next icons
- **Flexible Content Extraction**: Custom functions for URL generation, title, and description extraction
- **Event Handling**: Navigation events for custom logic
- **Accessibility Support**: Full ARIA labels and keyboard navigation
- **Localization Ready**: Customizable labels for different languages
- **Conditional Display**: Show/hide navigation and icon elements

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Item` | `ContentItem?` | `null` | The current content item for which to show navigation |
| `PreviousItem` | `ContentItem?` | `null` | Explicit previous item (overrides auto-discovery) |
| `NextItem` | `ContentItem?` | `null` | Explicit next item (overrides auto-discovery) |
| `ShowNavigation` | `bool` | `true` | Whether to show the navigation component |
| `ShowIcons` | `bool` | `true` | Whether to show navigation icons |
| `Variant` | `ContentNavigationVariant` | `Default` | Display variant (Default, Compact, Card) |
| `PreviousLabel` | `string` | `"Previous"` | Label for the previous navigation item |
| `NextLabel` | `string` | `"Next"` | Label for the next navigation item |
| `PreviousIcon` | `RenderFragment?` | `null` | Custom icon for previous navigation |
| `NextIcon` | `RenderFragment?` | `null` | Custom icon for next navigation |
| `NoPreviousPlaceholder` | `string?` | `null` | Placeholder text when no previous item exists |
| `NoNextPlaceholder` | `string?` | `null` | Placeholder text when no next item exists |
| `UrlGenerator` | `Func<ContentItem, string>?` | `null` | Custom function for generating navigation URLs |
| `TitleExtractor` | `Func<ContentItem, string>?` | `null` | Custom function for extracting item titles |
| `DescriptionExtractor` | `Func<ContentItem, string?>?` | `null` | Custom function for extracting item descriptions |
| `CssClass` | `string?` | `null` | Additional CSS classes for the navigation container |
| `OnNavigating` | `EventCallback<ContentNavigationEventArgs>` | - | Event fired when navigation is about to occur |
| `OnNavigated` | `EventCallback<ContentNavigationEventArgs>` | - | Event fired after navigation has occurred |

## Navigation Variants

### ContentNavigationVariant Enum

| Value | Description |
|-------|-------------|
| `Default` | Standard navigation layout with full content display |
| `Compact` | Minimized navigation with reduced spacing and content |
| `Card` | Card-based layout with elevated appearance |

## Basic Usage

### Simple Navigation with Auto-Discovery

```razor
@using Osirion.Blazor.Cms.Web.Components

<OsirionContentNavigation Item="@currentItem" />
```

### Navigation with Custom Labels

```razor
<OsirionContentNavigation 
    Item="@currentItem"
    PreviousLabel="← Previous Article"
    NextLabel="Next Article →" />
```

### Compact Navigation

```razor
<OsirionContentNavigation 
    Item="@currentItem"
    Variant="ContentNavigationVariant.Compact"
    ShowIcons="false" />
```

## Advanced Examples

### Custom Navigation Items

```razor
<OsirionContentNavigation 
    Item="@currentItem"
    PreviousItem="@customPreviousItem"
    NextItem="@customNextItem"
    Variant="ContentNavigationVariant.Card" />

@code {
    private ContentItem currentItem = new();
    private ContentItem? customPreviousItem;
    private ContentItem? customNextItem;

    protected override async Task OnInitializedAsync()
    {
        // Load custom navigation items based on your logic
        customPreviousItem = await GetRelatedItem("previous");
        customNextItem = await GetRelatedItem("next");
    }

    private async Task<ContentItem?> GetRelatedItem(string direction)
    {
        // Your custom logic for finding related items
        return await ContentService.GetRelatedItemAsync(currentItem.Id, direction);
    }
}
```

### Custom Icons and Styling

```razor
<OsirionContentNavigation 
    Item="@currentItem"
    CssClass="my-custom-navigation"
    PreviousIcon="@previousIcon"
    NextIcon="@nextIcon" />

@code {
    private RenderFragment previousIcon = @<i class="fas fa-chevron-left"></i>;
    private RenderFragment nextIcon = @<i class="fas fa-chevron-right"></i>;
}
```

### Custom Content Extraction

```razor
<OsirionContentNavigation 
    Item="@currentItem"
    UrlGenerator="@GenerateCustomUrl"
    TitleExtractor="@ExtractCustomTitle"
    DescriptionExtractor="@ExtractCustomDescription" />

@code {
    private string GenerateCustomUrl(ContentItem item)
    {
        return $"/articles/{item.Slug}?ref=navigation";
    }

    private string ExtractCustomTitle(ContentItem item)
    {
        return item.Metadata?.GetValueOrDefault("NavigationTitle") ?? item.Title ?? "Untitled";
    }

    private string? ExtractCustomDescription(ContentItem item)
    {
        return item.Metadata?.GetValueOrDefault("NavigationDescription") ?? item.Summary;
    }
}
```

### Navigation with Event Handling

```razor
<OsirionContentNavigation 
    Item="@currentItem"
    OnNavigating="@HandleNavigating"
    OnNavigated="@HandleNavigated" />

@code {
    private async Task HandleNavigating(ContentNavigationEventArgs args)
    {
        // Analytics tracking
        await Analytics.TrackEvent("content_navigation_start", new
        {
            from = args.FromItem?.Path,
            to = args.ToItem?.Path,
            direction = args.Direction
        });
    }

    private async Task HandleNavigated(ContentNavigationEventArgs args)
    {
        // Update reading progress
        await ReadingProgressService.UpdateProgressAsync(args.ToItem?.Id);
        
        // Scroll to top
        await JSRuntime.InvokeVoidAsync("scrollToTop");
    }
}
```

### Localized Navigation

```razor
<OsirionContentNavigation 
    Item="@currentItem"
    PreviousLabel="@Localizer["Previous"]"
    NextLabel="@Localizer["Next"]"
    NoPreviousPlaceholder="@Localizer["FirstArticle"]"
    NoNextPlaceholder="@Localizer["LastArticle"]" />

@code {
    [Inject] private IStringLocalizer<NavigationResources> Localizer { get; set; } = default!;
}
```

### Conditional Navigation Display

```razor
<OsirionContentNavigation 
    Item="@currentItem"
    ShowNavigation="@showNavigation"
    ShowIcons="@userPreferences.ShowNavigationIcons" />

@code {
    private bool showNavigation => currentItem?.Type == "article" && !currentItem.IsLandingPage;
    
    [Inject] private UserPreferencesService userPreferences { get; set; } = default!;
}
```

## CSS Customization

### Default Styling

```css
.osirion-content-navigation {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem 0;
    border-top: 1px solid var(--border-color);
    margin-top: 2rem;
}

.osirion-content-navigation .nav-item {
    display: flex;
    align-items: center;
    text-decoration: none;
    color: var(--text-color);
    transition: color 0.2s ease;
}

.osirion-content-navigation .nav-item:hover {
    color: var(--primary-color);
}
```

### Compact Variant

```css
.osirion-content-navigation.compact {
    padding: 0.5rem 0;
    margin-top: 1rem;
}

.osirion-content-navigation.compact .nav-item {
    font-size: 0.875rem;
}
```

### Card Variant

```css
.osirion-content-navigation.card {
    background: var(--card-background);
    border: 1px solid var(--border-color);
    border-radius: 0.5rem;
    padding: 1.5rem;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.osirion-content-navigation.card .nav-item {
    background: var(--item-background);
    padding: 1rem;
    border-radius: 0.25rem;
    border: 1px solid var(--border-color);
}
```

## Accessibility Features

- **ARIA Labels**: Proper labeling for screen readers
- **Keyboard Navigation**: Full keyboard support for navigation
- **Focus Management**: Proper focus indicators and management
- **Semantic HTML**: Uses appropriate HTML elements for navigation

## Best Practices

1. **Consistent Navigation**: Use consistent labeling and styling across your site
2. **Loading States**: Show loading indicators when navigation items are being discovered
3. **Error Handling**: Gracefully handle cases where navigation items cannot be loaded
4. **Performance**: Cache navigation items when possible to avoid repeated queries
5. **SEO Optimization**: Use meaningful titles and descriptions for navigation items
6. **Mobile Responsiveness**: Ensure navigation works well on mobile devices

## Integration Examples

### With Content Service

```razor
@page "/article/{slug}"
@using Osirion.Blazor.Cms.Web.Components

<article>
    <h1>@article?.Title</h1>
    <div>@((MarkupString)(article?.Content ?? ""))</div>
</article>

<OsirionContentNavigation 
    Item="@article"
    Variant="ContentNavigationVariant.Card"
    OnNavigated="@UpdateLastRead" />

@code {
    [Parameter] public string Slug { get; set; } = "";
    [Inject] private IContentService ContentService { get; set; } = default!;
    [Inject] private IUserActivityService ActivityService { get; set; } = default!;
    
    private ContentItem? article;

    protected override async Task OnParametersSetAsync()
    {
        article = await ContentService.GetBySlugAsync(Slug);
    }

    private async Task UpdateLastRead(ContentNavigationEventArgs args)
    {
        if (args.ToItem != null)
        {
            await ActivityService.RecordLastReadAsync(args.ToItem.Id);
        }
    }
}
```

### Series Navigation

```razor
<OsirionContentNavigation 
    Item="@currentChapter"
    UrlGenerator="@GenerateSeriesUrl"
    TitleExtractor="@GetChapterTitle"
    PreviousLabel="← Previous Chapter"
    NextLabel="Next Chapter →" />

@code {
    private string GenerateSeriesUrl(ContentItem chapter)
    {
        return $"/series/{seriesSlug}/chapter/{chapter.Slug}";
    }

    private string GetChapterTitle(ContentItem chapter)
    {
        var chapterNumber = chapter.Metadata?.GetValueOrDefault("ChapterNumber");
        return $"Chapter {chapterNumber}: {chapter.Title}";
    }
}
```

## Related Components

- [`ContentBreadcrumbs`](content-breadcrumbs.md) - For hierarchical navigation
- [`DirectoryNavigation`](directory-navigation.md) - For directory-based navigation
- [`CategoriesList`](categories-list.md) - For category-based navigation
- [`ContentView`](../core/content-view.md) - For displaying content items
