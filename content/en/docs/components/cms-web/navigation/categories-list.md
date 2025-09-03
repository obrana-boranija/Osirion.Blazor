---
title: "Categories List - Osirion Blazor CMS"
description: "Dynamic category navigation component with customizable sorting, filtering, and URL formatting for Blazor CMS applications."
category: "CMS Web Components"
subcategory: "Navigation"
tags: ["cms", "navigation", "categories", "taxonomy", "sorting", "filtering"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "categories-list"
section: "components"
layout: "component"
seo:
  title: "Categories List Component | Osirion Blazor CMS Documentation"
  description: "Learn how to display categorized content navigation with sorting, filtering, and custom URL formatting."
  keywords: ["Blazor", "CMS", "navigation", "categories", "taxonomy", "content organization"]
  canonical: "/docs/components/cms.web/navigation/categories-list"
  image: "/images/components/categories-list-preview.jpg"
navigation:
  parent: "CMS Web Navigation"
  order: 1
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Navigation"
    link: "/docs/components/cms.web/navigation"
  - text: "Categories List"
    link: "/docs/components/cms.web/navigation/categories-list"
---

# Categories List Component

The **CategoriesList** component provides a dynamic, customizable navigation interface for content categories. It automatically loads categories from your CMS content provider and renders them with optional post counts, active state highlighting, and flexible URL formatting.

## Overview

This component creates a categorized navigation menu that helps users browse content by topic. It supports both alphabetical and count-based sorting, active category highlighting, and customizable URL generation for seamless integration with your site's routing structure.

## Key Features

- **Automatic Category Loading**: Dynamically loads categories from content provider
- **Flexible Sorting**: Sort by name (alphabetical) or post count (popularity)
- **Active State Highlighting**: Visual indication of current category
- **Post Count Display**: Shows number of posts per category
- **Custom URL Formatting**: Configurable URL generation for categories
- **Loading States**: Built-in loading and empty state handling
- **Responsive Design**: Mobile-friendly category navigation
- **Performance Optimized**: Efficient category loading and caching

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | Optional title displayed above the categories list |
| `LoadingText` | `string` | `"Loading categories..."` | Text shown while loading categories |
| `NoContentText` | `string` | `"No categories available."` | Text shown when no categories exist |
| `CategoryUrlFormatter` | `Func<ContentCategory, string>?` | `null` | Custom function to format category URLs |
| `ActiveCategory` | `string?` | `null` | Slug or name of currently active category |
| `ShowCount` | `bool` | `true` | Whether to display post count for each category |
| `SortByCount` | `bool` | `true` | Sort by post count (desc) if true, alphabetically if false |
| `MaxCategories` | `int?` | `null` | Maximum number of categories to display |

## Basic Usage

### Simple Categories List

```razor
@using Osirion.Blazor.Cms.Web.Components

<CategoriesList Title="Browse Categories" />
```

### Alphabetical Sorting without Counts

```razor
<CategoriesList Title="Categories"
                SortByCount="false"
                ShowCount="false" />
```

### Limited Categories with Custom Text

```razor
<CategoriesList Title="Popular Topics"
                MaxCategories="5"
                LoadingText="Loading topics..."
                NoContentText="No topics found." />
```

## Advanced Examples

### Blog Sidebar with Active Category

```razor
@page "/blog/category/{categorySlug?}"

<div class="blog-layout">
    <main class="blog-content">
        <!-- Blog posts content -->
        <ContentList CategoryFilter="@CategorySlug" />
    </main>
    
    <aside class="blog-sidebar">
        <CategoriesList Title="Categories"
                        ActiveCategory="@CategorySlug"
                        CategoryUrlFormatter="@FormatBlogCategoryUrl"
                        SortByCount="true"
                        ShowCount="true" />
        
        <TagCloud Title="Popular Tags" MaxTags="20" />
    </aside>
</div>

@code {
    [Parameter] public string? CategorySlug { get; set; }
    
    private string FormatBlogCategoryUrl(ContentCategory category)
    {
        return $"/blog/category/{category.Slug}";
    }
}
```

### News Portal with Regional Categories

```razor
@page "/news/{region?}"

<div class="news-header">
    <nav class="news-navigation">
        <CategoriesList Title="@GetRegionalTitle(Region)"
                        CategoryUrlFormatter="@FormatNewsCategoryUrl"
                        ActiveCategory="@GetActiveCategory()"
                        SortByCount="false"
                        MaxCategories="10" />
    </nav>
</div>

@code {
    [Parameter] public string? Region { get; set; }
    
    private string GetRegionalTitle(string? region)
    {
        return region switch
        {
            "us" => "US News Categories",
            "world" => "World News Categories", 
            "tech" => "Technology Categories",
            _ => "All Categories"
        };
    }
    
    private string FormatNewsCategoryUrl(ContentCategory category)
    {
        return string.IsNullOrEmpty(Region)
            ? $"/news/category/{category.Slug}"
            : $"/news/{Region}/category/{category.Slug}";
    }
    
    private string? GetActiveCategory()
    {
        // Extract category from current URL or other logic
        return NavigationManager.GetQueryString("category");
    }
}
```

### E-commerce Product Categories

```razor
@page "/products/{category?}"

<div class="product-categories">
    <CategoriesList Title="Shop by Category"
                    CategoryUrlFormatter="@FormatProductCategoryUrl"
                    ActiveCategory="@Category"
                    SortByCount="true"
                    ShowCount="true"
                    NoContentText="Coming soon - new product categories!" />
</div>

@code {
    [Parameter] public string? Category { get; set; }
    
    private string FormatProductCategoryUrl(ContentCategory category)
    {
        return $"/products/{category.Slug}";
    }
}
```

### Documentation Categories with Custom Styling

```razor
<div class="documentation-nav">
    <CategoriesList Title="Documentation Sections"
                    CategoryUrlFormatter="@FormatDocCategoryUrl"
                    ActiveCategory="@GetCurrentDocCategory()"
                    SortByCount="false"
                    ShowCount="false"
                    LoadingText="Loading documentation sections..." />
</div>

<style>
    .documentation-nav .osirion-categories-list {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 1rem;
    }
    
    .documentation-nav .osirion-category-item {
        background: var(--bs-light);
        border-radius: 0.5rem;
        border: 1px solid var(--bs-border-color);
    }
    
    .documentation-nav .osirion-category-link {
        display: block;
        padding: 1rem;
        text-decoration: none;
        color: var(--bs-body-color);
        font-weight: 500;
        transition: all 0.2s ease;
    }
    
    .documentation-nav .osirion-category-link:hover {
        background: var(--bs-primary);
        color: white;
        transform: translateY(-2px);
    }
    
    .documentation-nav .osirion-category-link.osirion-active {
        background: var(--bs-primary);
        color: white;
        border-color: var(--bs-primary);
    }
</style>

@code {
    private string FormatDocCategoryUrl(ContentCategory category)
    {
        return $"/docs/{category.Slug}";
    }
    
    private string? GetCurrentDocCategory()
    {
        var segments = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Segments;
        return segments.Length > 2 ? segments[2].TrimEnd('/') : null;
    }
}
```

### Multi-level Category Navigation

```razor
<div class="category-navigation">
    <!-- Primary Categories -->
    <CategoriesList Title="Main Categories" 
                    CategoryUrlFormatter="@FormatPrimaryCategoryUrl"
                    ActiveCategory="@PrimaryCategory"
                    SortByCount="true"
                    MaxCategories="8" />
    
    @if (!string.IsNullOrEmpty(PrimaryCategory))
    {
        <!-- Subcategories -->
        <SubcategoriesList ParentCategory="@PrimaryCategory"
                          Title="Subcategories"
                          CategoryUrlFormatter="@FormatSubcategoryUrl"
                          ActiveCategory="@Subcategory" />
    }
</div>

@code {
    [Parameter] public string? PrimaryCategory { get; set; }
    [Parameter] public string? Subcategory { get; set; }
    
    private string FormatPrimaryCategoryUrl(ContentCategory category)
    {
        return $"/browse/{category.Slug}";
    }
    
    private string FormatSubcategoryUrl(ContentCategory category)
    {
        return $"/browse/{PrimaryCategory}/{category.Slug}";
    }
}
```

### Filtered Categories by Content Type

```razor
@page "/resources/{contentType?}"

<div class="resources-navigation">
    @if (ContentType == "tutorials")
    {
        <CategoriesList Title="Tutorial Categories"
                        CategoryUrlFormatter="@(cat => $"/resources/tutorials/{cat.Slug}")"
                        ActiveCategory="@GetActiveResourceCategory()"
                        SortByCount="true" />
    }
    else if (ContentType == "articles")
    {
        <CategoriesList Title="Article Categories"
                        CategoryUrlFormatter="@(cat => $"/resources/articles/{cat.Slug}")"
                        ActiveCategory="@GetActiveResourceCategory()"
                        SortByCount="false" />
    }
    else
    {
        <CategoriesList Title="All Resource Categories"
                        CategoryUrlFormatter="@FormatResourceCategoryUrl"
                        SortByCount="true"
                        MaxCategories="15" />
    }
</div>

@code {
    [Parameter] public string? ContentType { get; set; }
    
    private string FormatResourceCategoryUrl(ContentCategory category)
    {
        return $"/resources/{category.Slug}";
    }
    
    private string? GetActiveResourceCategory()
    {
        // Extract from query parameters or route
        return NavigationManager.GetQueryString("category");
    }
}
```

### AJAX Category Loading

```razor
<div class="dynamic-categories">
    <div class="category-filters">
        <button @onclick="@(() => LoadCategoriesByType("all"))" 
                class="btn btn-outline-primary">All</button>
        <button @onclick="@(() => LoadCategoriesByType("featured"))" 
                class="btn btn-outline-primary">Featured</button>
        <button @onclick="@(() => LoadCategoriesByType("recent"))" 
                class="btn btn-outline-primary">Recent</button>
    </div>
    
    @if (isCustomLoading)
    {
        <div class="text-center">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading categories...</span>
            </div>
        </div>
    }
    else
    {
        <CategoriesList Title="@categoryTitle"
                        CategoryUrlFormatter="@FormatFilteredCategoryUrl"
                        SortByCount="true"
                        ShowCount="true" />
    }
</div>

@code {
    private bool isCustomLoading = false;
    private string categoryTitle = "All Categories";
    
    private async Task LoadCategoriesByType(string type)
    {
        isCustomLoading = true;
        StateHasChanged();
        
        try
        {
            // Simulate API call or custom filtering logic
            await Task.Delay(500);
            
            categoryTitle = type switch
            {
                "featured" => "Featured Categories",
                "recent" => "Recently Active Categories", 
                _ => "All Categories"
            };
            
            // Custom category loading logic would go here
            // await CategoryService.LoadCategoriesByTypeAsync(type);
        }
        finally
        {
            isCustomLoading = false;
            StateHasChanged();
        }
    }
    
    private string FormatFilteredCategoryUrl(ContentCategory category)
    {
        return $"/filtered/{category.Slug}";
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-categories-list-container {
    /* Main container */
}

.osirion-categories-title {
    /* Category list title */
    margin-bottom: 1rem;
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--bs-heading-color);
}

.osirion-categories-list {
    /* Categories list container */
    list-style: none;
    padding: 0;
    margin: 0;
}

.osirion-category-item {
    /* Individual category item */
    margin-bottom: 0.5rem;
}

.osirion-category-link {
    /* Category link styling */
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem 1rem;
    text-decoration: none;
    color: var(--bs-body-color);
    border-radius: 0.375rem;
    border: 1px solid transparent;
    transition: all 0.2s ease;
}

.osirion-category-link:hover {
    background: var(--bs-light);
    color: var(--bs-primary);
    border-color: var(--bs-border-color);
}

.osirion-category-link.osirion-active {
    background: var(--bs-primary);
    color: white;
    border-color: var(--bs-primary);
}

.osirion-category-count {
    /* Post count badge */
    background: var(--bs-secondary);
    color: white;
    font-size: 0.875rem;
    padding: 0.25rem 0.5rem;
    border-radius: 1rem;
    min-width: 2rem;
    text-align: center;
}

.osirion-active .osirion-category-count {
    background: rgba(255, 255, 255, 0.2);
}

.osirion-loading,
.osirion-no-categories {
    /* Loading and empty states */
    text-align: center;
    padding: 2rem;
    color: var(--bs-text-muted);
    font-style: italic;
}
```

### Custom Styling Examples

#### Card-based Layout
```css
.osirion-categories-list {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 1rem;
}

.osirion-category-item {
    background: white;
    border-radius: 0.5rem;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    margin-bottom: 0;
}

.osirion-category-link {
    height: 100%;
    border: none;
}
```

#### Horizontal Layout
```css
.osirion-categories-list {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
}

.osirion-category-item {
    margin-bottom: 0;
}

.osirion-category-link {
    padding: 0.5rem 1rem;
    background: var(--bs-light);
    border-radius: 2rem;
    white-space: nowrap;
}
```

#### Minimal Design
```css
.osirion-categories-list {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.osirion-category-link {
    padding: 0.5rem 0;
    border: none;
    border-bottom: 1px solid transparent;
    border-radius: 0;
    background: transparent;
}

.osirion-category-link:hover {
    background: transparent;
    border-bottom-color: var(--bs-primary);
}
```

## Performance Optimization

### Category Caching

```razor
@implements IDisposable

<CategoriesList CategoryUrlFormatter="@FormatCachedCategoryUrl" />

@code {
    private readonly MemoryCache categoryCache = new MemoryCache(new MemoryCacheOptions());
    
    private string FormatCachedCategoryUrl(ContentCategory category)
    {
        var cacheKey = $"category_url_{category.Slug}";
        
        if (!categoryCache.TryGetValue(cacheKey, out string? url))
        {
            url = $"/category/{category.Slug}";
            categoryCache.Set(cacheKey, url, TimeSpan.FromMinutes(30));
        }
        
        return url!;
    }
    
    public void Dispose()
    {
        categoryCache.Dispose();
    }
}
```

### Lazy Loading

```razor
<div class="categories-container">
    @if (showCategories)
    {
        <CategoriesList Title="Categories" />
    }
    else
    {
        <button @onclick="LoadCategories" class="btn btn-outline-primary">
            Load Categories
        </button>
    }
</div>

@code {
    private bool showCategories = false;
    
    private void LoadCategories()
    {
        showCategories = true;
    }
}
```

## Common Use Cases

- **Blog Sidebars**: Category navigation for blog posts
- **News Portals**: News category organization
- **E-commerce Sites**: Product category browsing
- **Documentation**: Content section navigation
- **Portfolio Sites**: Project category filtering
- **Educational Platforms**: Course category organization
- **Content Hubs**: Topic-based content discovery

## Best Practices

1. **Consistent URL Structure**: Use consistent category URL patterns across your site
2. **Active State Management**: Properly highlight the current category
3. **Performance**: Limit categories displayed on mobile devices
4. **Accessibility**: Ensure proper ARIA labels and keyboard navigation
5. **SEO**: Use nofollow for category links to avoid crawl budget issues
6. **User Experience**: Sort by popularity (count) for better discovery
7. **Loading States**: Provide clear feedback during category loading
8. **Error Handling**: Gracefully handle empty or failed category loads

The CategoriesList component provides a robust foundation for content categorization and navigation in Blazor CMS applications, with extensive customization options for different use cases and design requirements.
