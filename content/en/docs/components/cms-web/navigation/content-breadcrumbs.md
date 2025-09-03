---
title: "Content Breadcrumbs - Osirion Blazor CMS"
description: "Navigation breadcrumb component with directory path traversal and customizable URL formatting for enhanced user navigation experience."
category: "CMS Web Components"
subcategory: "Navigation"
tags: ["cms", "navigation", "breadcrumbs", "directories", "hierarchy", "user-experience"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "content-breadcrumbs"
section: "components"
layout: "component"
seo:
  title: "Content Breadcrumbs Component | Osirion Blazor CMS Documentation"
  description: "Learn how to implement hierarchical navigation breadcrumbs with directory support and custom URL formatting."
  keywords: ["Blazor", "CMS", "navigation", "breadcrumbs", "hierarchy", "directories", "user experience"]
  canonical: "/docs/components/cms.web/navigation/content-breadcrumbs"
  image: "/images/components/content-breadcrumbs-preview.jpg"
navigation:
  parent: "CMS Web Navigation"
  order: 2
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Navigation"
    link: "/docs/components/cms.web/navigation"
  - text: "Content Breadcrumbs"
    link: "/docs/components/cms.web/navigation/content-breadcrumbs"
---

# Content Breadcrumbs Component

The **ContentBreadcrumbs** component provides hierarchical navigation breadcrumbs that help users understand their current location within your content structure and easily navigate back to parent sections. It automatically builds breadcrumb trails from directory hierarchies and content relationships.

## Overview

This component creates accessible breadcrumb navigation that follows web standards and provides clear visual hierarchy. It supports both content-based and directory-based breadcrumbs, with customizable home links and URL formatting for seamless integration with your routing system.

## Key Features

- **Automatic Hierarchy Detection**: Builds breadcrumbs from content directory structure
- **Accessible Navigation**: ARIA-compliant breadcrumb markup
- **Customizable Home Link**: Configurable home text and URL
- **Directory Path Support**: Full directory traversal from root to current
- **Current Item Handling**: Optional display of current page in breadcrumbs
- **Custom URL Formatting**: Flexible URL generation for directories
- **SEO Optimized**: Structured navigation for search engines
- **Responsive Design**: Mobile-friendly breadcrumb display

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Content` | `ContentItem?` | `null` | Content item to build breadcrumbs from |
| `Directory` | `DirectoryItem?` | `null` | Directory to build breadcrumbs from |
| `ShowHome` | `bool` | `true` | Whether to show home link at the beginning |
| `HomeText` | `string` | `"Home"` | Text displayed for the home link |
| `HomeUrl` | `string` | `"/"` | URL for the home link |
| `HideCurrentItem` | `bool` | `false` | Whether to hide the current page from breadcrumbs |
| `DirectoryUrlFormatter` | `Func<DirectoryItem, string>?` | `null` | Custom function to format directory URLs |

## Basic Usage

### Simple Content Breadcrumbs

```razor
@using Osirion.Blazor.Cms.Web.Components

<ContentBreadcrumbs Content="@currentContent" />

@code {
    private ContentItem? currentContent;
    
    protected override async Task OnInitializedAsync()
    {
        currentContent = await ContentService.GetByPathAsync("/docs/getting-started");
    }
}
```

### Directory-Based Breadcrumbs

```razor
<ContentBreadcrumbs Directory="@currentDirectory"
                   DirectoryUrlFormatter="@FormatDirectoryUrl" />

@code {
    private DirectoryItem? currentDirectory;
    
    private string FormatDirectoryUrl(DirectoryItem directory)
    {
        return $"/browse/{directory.Path}";
    }
}
```

### Custom Home Configuration

```razor
<ContentBreadcrumbs Content="@article"
                   HomeText="Dashboard"
                   HomeUrl="/admin"
                   HideCurrentItem="true" />
```

## Advanced Examples

### Blog Article Breadcrumbs

```razor
@page "/blog/{category}/{slug}"

<ContentBreadcrumbs Content="@blogPost"
                   HomeText="Blog"
                   HomeUrl="/blog"
                   DirectoryUrlFormatter="@FormatBlogDirectoryUrl" />

<article class="blog-post">
    <header>
        <h1>@blogPost?.Title</h1>
        <div class="post-meta">
            <time datetime="@blogPost?.PublishDate.ToString("yyyy-MM-dd")">
                @blogPost?.PublishDate.ToString("MMMM dd, yyyy")
            </time>
        </div>
    </header>
    
    <div class="post-content">
        @if (blogPost != null)
        {
            @((MarkupString)blogPost.Content)
        }
    </div>
</article>

@code {
    [Parameter] public string Category { get; set; } = string.Empty;
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private ContentItem? blogPost;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Slug))
        {
            blogPost = await BlogService.GetPostAsync(Category, Slug);
        }
    }
    
    private string FormatBlogDirectoryUrl(DirectoryItem directory)
    {
        return $"/blog/{directory.Path}";
    }
}
```

### Documentation Breadcrumbs with Version

```razor
@page "/docs/{version}/{*path}"

<div class="documentation-header">
    <ContentBreadcrumbs Content="@docContent"
                       HomeText="@($"Docs {Version}")"
                       HomeUrl="@($"/docs/{Version}")"
                       DirectoryUrlFormatter="@FormatDocDirectoryUrl" />
</div>

<main class="documentation-content">
    @if (docContent != null)
    {
        <h1>@docContent.Title</h1>
        @((MarkupString)docContent.Content)
    }
</main>

@code {
    [Parameter] public string Version { get; set; } = "latest";
    [Parameter] public string Path { get; set; } = string.Empty;
    
    private ContentItem? docContent;
    
    protected override async Task OnParametersSetAsync()
    {
        var fullPath = $"/docs/{Version}/{Path}";
        docContent = await DocumentationService.GetByPathAsync(fullPath);
    }
    
    private string FormatDocDirectoryUrl(DirectoryItem directory)
    {
        return $"/docs/{Version}/{directory.Path}";
    }
}
```

### E-commerce Product Breadcrumbs

```razor
@page "/products/{*categoryPath}/{productSlug}"

<div class="product-navigation">
    <ContentBreadcrumbs Directory="@productCategory"
                       HomeText="Shop"
                       HomeUrl="/products"
                       DirectoryUrlFormatter="@FormatCategoryUrl" />
</div>

<div class="product-details">
    @if (product != null)
    {
        <h1>@product.Name</h1>
        <div class="product-info">
            <p class="price">@product.Price.ToString("C")</p>
            <p class="description">@product.Description</p>
        </div>
    }
</div>

@code {
    [Parameter] public string CategoryPath { get; set; } = string.Empty;
    [Parameter] public string ProductSlug { get; set; } = string.Empty;
    
    private ProductItem? product;
    private DirectoryItem? productCategory;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(ProductSlug))
        {
            product = await ProductService.GetBySlugAsync(ProductSlug);
            
            if (!string.IsNullOrEmpty(CategoryPath))
            {
                productCategory = await CategoryService.GetDirectoryAsync(CategoryPath);
            }
        }
    }
    
    private string FormatCategoryUrl(DirectoryItem directory)
    {
        return $"/products/{directory.Path}";
    }
}
```

### News Article with Regional Breadcrumbs

```razor
@page "/news/{region}/{category}/{slug}"

<nav class="news-breadcrumbs">
    <ContentBreadcrumbs Content="@newsArticle"
                       HomeText="News"
                       HomeUrl="/news"
                       DirectoryUrlFormatter="@FormatNewsDirectoryUrl" />
</nav>

<article class="news-article">
    @if (newsArticle != null)
    {
        <header class="article-header">
            <div class="article-meta">
                <span class="region-badge">@Region.ToUpper()</span>
                <time datetime="@newsArticle.PublishDate.ToString("yyyy-MM-dd")">
                    @newsArticle.PublishDate.ToString("MMM dd, yyyy")
                </time>
            </div>
            <h1>@newsArticle.Title</h1>
            <p class="lead">@newsArticle.Description</p>
        </header>
        
        <div class="article-content">
            @((MarkupString)newsArticle.Content)
        </div>
    }
</article>

@code {
    [Parameter] public string Region { get; set; } = string.Empty;
    [Parameter] public string Category { get; set; } = string.Empty;
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private ContentItem? newsArticle;
    
    protected override async Task OnParametersSetAsync()
    {
        var path = $"/news/{Region}/{Category}/{Slug}";
        newsArticle = await NewsService.GetByPathAsync(path);
    }
    
    private string FormatNewsDirectoryUrl(DirectoryItem directory)
    {
        return $"/news/{Region}/{directory.Path}";
    }
}
```

### Multi-level Directory Navigation

```razor
@page "/resources/{*directoryPath}"

<div class="resources-header">
    <ContentBreadcrumbs Directory="@currentDirectory"
                       HomeText="Resources"
                       HomeUrl="/resources"
                       DirectoryUrlFormatter="@FormatResourceDirectoryUrl"
                       HideCurrentItem="false" />
    
    @if (currentDirectory?.Children?.Any() == true)
    {
        <div class="subdirectories">
            <h3>Subdirectories</h3>
            <ul class="subdirectory-list">
                @foreach (var subdir in currentDirectory.Children)
                {
                    <li>
                        <a href="@FormatResourceDirectoryUrl(subdir)">
                            @subdir.Name
                            @if (subdir.ContentCount > 0)
                            {
                                <span class="content-count">(@subdir.ContentCount)</span>
                            }
                        </a>
                    </li>
                }
            </ul>
        </div>
    }
</div>

@code {
    [Parameter] public string DirectoryPath { get; set; } = string.Empty;
    
    private DirectoryItem? currentDirectory;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(DirectoryPath))
        {
            currentDirectory = await DirectoryService.GetByPathAsync(DirectoryPath);
        }
        else
        {
            currentDirectory = await DirectoryService.GetRootAsync();
        }
    }
    
    private string FormatResourceDirectoryUrl(DirectoryItem directory)
    {
        return $"/resources/{directory.Path}";
    }
}
```

### Admin Panel Breadcrumbs

```razor
@page "/admin/{section}/{*subsection}"
@attribute [Authorize(Roles = "Admin")]

<div class="admin-header">
    <ContentBreadcrumbs Directory="@adminSection"
                       HomeText="Admin"
                       HomeUrl="/admin"
                       DirectoryUrlFormatter="@FormatAdminDirectoryUrl" />
    
    <div class="admin-actions">
        <button class="btn btn-primary" @onclick="ShowHelp">
            <i class="fas fa-question-circle"></i> Help
        </button>
    </div>
</div>

<main class="admin-content">
    @if (adminSection != null)
    {
        <h1>@adminSection.Name</h1>
        
        @switch (Section.ToLower())
        {
            case "content":
                <ContentManagement CurrentPath="@Subsection" />
                break;
            case "users":
                <UserManagement CurrentPath="@Subsection" />
                break;
            case "settings":
                <SettingsManagement CurrentPath="@Subsection" />
                break;
            default:
                <AdminDashboard />
                break;
        }
    }
</main>

@code {
    [Parameter] public string Section { get; set; } = string.Empty;
    [Parameter] public string Subsection { get; set; } = string.Empty;
    
    private DirectoryItem? adminSection;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Section))
        {
            var path = string.IsNullOrEmpty(Subsection) 
                ? Section 
                : $"{Section}/{Subsection}";
            adminSection = await AdminService.GetSectionAsync(path);
        }
    }
    
    private string FormatAdminDirectoryUrl(DirectoryItem directory)
    {
        return $"/admin/{directory.Path}";
    }
    
    private void ShowHelp()
    {
        // Implementation for help modal
    }
}
```

### Localized Breadcrumbs

```razor
@page "/{locale}/docs/{*path}"

<div class="localized-breadcrumbs">
    <ContentBreadcrumbs Content="@localizedContent"
                       HomeText="@GetLocalizedHomeText(Locale)"
                       HomeUrl="@($"/{Locale}/docs")"
                       DirectoryUrlFormatter="@FormatLocalizedDirectoryUrl" />
</div>

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string Path { get; set; } = string.Empty;
    
    private ContentItem? localizedContent;
    
    protected override async Task OnParametersSetAsync()
    {
        var fullPath = $"/{Locale}/docs/{Path}";
        localizedContent = await ContentService.GetLocalizedAsync(fullPath, Locale);
    }
    
    private string GetLocalizedHomeText(string locale)
    {
        return locale switch
        {
            "es" => "Documentación",
            "fr" => "Documentation",
            "de" => "Dokumentation",
            "it" => "Documentazione",
            "pt" => "Documentação",
            _ => "Documentation"
        };
    }
    
    private string FormatLocalizedDirectoryUrl(DirectoryItem directory)
    {
        return $"/{Locale}/docs/{directory.Path}";
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-breadcrumbs {
    /* Main breadcrumb container */
    margin-bottom: 1rem;
}

.osirion-breadcrumbs-list {
    /* Breadcrumb list */
    display: flex;
    flex-wrap: wrap;
    list-style: none;
    padding: 0;
    margin: 0;
    align-items: center;
}

.osirion-breadcrumbs-item {
    /* Individual breadcrumb item */
    display: flex;
    align-items: center;
}

.osirion-breadcrumbs-item:not(:last-child)::after {
    /* Separator between items */
    content: "/";
    margin: 0 0.5rem;
    color: var(--bs-text-muted);
    font-weight: normal;
}

.osirion-breadcrumbs-link {
    /* Breadcrumb links */
    color: var(--bs-primary);
    text-decoration: none;
    transition: color 0.2s ease;
}

.osirion-breadcrumbs-link:hover {
    color: var(--bs-primary-darker);
    text-decoration: underline;
}

.osirion-breadcrumbs-text {
    /* Current page text (non-link) */
    color: var(--bs-text-muted);
    font-weight: 500;
}

.osirion-breadcrumbs-current {
    /* Current page item */
    color: var(--bs-text-muted);
}
```

### Custom Styling Examples

#### Modern Design with Icons
```css
.osirion-breadcrumbs-item:not(:last-child)::after {
    content: "›";
    font-size: 1.2em;
    color: var(--bs-border-color);
    margin: 0 0.75rem;
}

.osirion-breadcrumbs-link {
    padding: 0.25rem 0.5rem;
    border-radius: 0.25rem;
    transition: all 0.2s ease;
}

.osirion-breadcrumbs-link:hover {
    background: var(--bs-light);
    text-decoration: none;
}

/* Add home icon */
.osirion-breadcrumbs-item:first-child .osirion-breadcrumbs-link::before {
    content: "🏠";
    margin-right: 0.25rem;
}
```

#### Minimalist Style
```css
.osirion-breadcrumbs-list {
    font-size: 0.875rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.osirion-breadcrumbs-item:not(:last-child)::after {
    content: "•";
    margin: 0 1rem;
    color: var(--bs-primary);
}

.osirion-breadcrumbs-link {
    color: var(--bs-body-color);
    font-weight: 600;
}

.osirion-breadcrumbs-text {
    color: var(--bs-primary);
    font-weight: 600;
}
```

#### Card-based Breadcrumbs
```css
.osirion-breadcrumbs {
    background: white;
    border: 1px solid var(--bs-border-color);
    border-radius: 0.5rem;
    padding: 1rem;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
}

.osirion-breadcrumbs-item {
    background: var(--bs-light);
    border-radius: 0.25rem;
    padding: 0.25rem 0.75rem;
    margin-right: 0.5rem;
}

.osirion-breadcrumbs-item:not(:last-child)::after {
    content: none;
}

.osirion-breadcrumbs-current {
    background: var(--bs-primary);
    color: white;
}
```

## Accessibility Features

### ARIA Labels and Structure

```html
<!-- Generated markup includes proper ARIA attributes -->
<nav aria-label="breadcrumb" class="osirion-breadcrumbs">
    <ol class="osirion-breadcrumbs-list">
        <li class="osirion-breadcrumbs-item">
            <a href="/" class="osirion-breadcrumbs-link">Home</a>
        </li>
        <li class="osirion-breadcrumbs-item">
            <a href="/docs" class="osirion-breadcrumbs-link">Documentation</a>
        </li>
        <li class="osirion-breadcrumbs-item osirion-breadcrumbs-current">
            <span class="osirion-breadcrumbs-text">Getting Started</span>
        </li>
    </ol>
</nav>
```

### Enhanced Accessibility

```razor
<ContentBreadcrumbs Content="@content" />

<style>
    /* Focus indicators */
    .osirion-breadcrumbs-link:focus {
        outline: 2px solid var(--bs-primary);
        outline-offset: 2px;
    }
    
    /* High contrast mode support */
    @media (prefers-contrast: high) {
        .osirion-breadcrumbs-item:not(:last-child)::after {
            color: ButtonText;
        }
        
        .osirion-breadcrumbs-link {
            border: 1px solid ButtonText;
        }
    }
    
    /* Reduced motion support */
    @media (prefers-reduced-motion: reduce) {
        .osirion-breadcrumbs-link {
            transition: none;
        }
    }
</style>
```

## Performance Optimization

### Directory Caching

```razor
@implements IDisposable

<ContentBreadcrumbs Directory="@directory"
                   DirectoryUrlFormatter="@FormatCachedDirectoryUrl" />

@code {
    private readonly MemoryCache directoryCache = new MemoryCache(new MemoryCacheOptions());
    
    private string FormatCachedDirectoryUrl(DirectoryItem directory)
    {
        var cacheKey = $"directory_url_{directory.Path}";
        
        if (!directoryCache.TryGetValue(cacheKey, out string? url))
        {
            url = $"/browse/{directory.Path}";
            directoryCache.Set(cacheKey, url, TimeSpan.FromMinutes(30));
        }
        
        return url!;
    }
    
    public void Dispose()
    {
        directoryCache.Dispose();
    }
}
```

## Common Use Cases

- **Blog Navigation**: Article category and post breadcrumbs
- **Documentation Sites**: Hierarchical content navigation
- **E-commerce Platforms**: Product category breadcrumbs
- **File Browsers**: Directory-based navigation
- **Admin Panels**: Section and subsection breadcrumbs
- **Multi-language Sites**: Localized navigation paths
- **News Portals**: Regional and category-based breadcrumbs

## Best Practices

1. **Consistent URLs**: Ensure directory URLs match your routing structure
2. **Mobile Optimization**: Consider truncating long breadcrumbs on mobile
3. **Accessibility**: Always include proper ARIA labels
4. **SEO Benefits**: Breadcrumbs help search engines understand site structure
5. **User Experience**: Don't include the current page in clickable breadcrumbs
6. **Performance**: Cache directory hierarchy for frequently accessed paths
7. **Visual Hierarchy**: Use consistent separators and styling
8. **Localization**: Translate breadcrumb text for international sites

The ContentBreadcrumbs component provides essential navigation functionality that improves user experience and helps users understand their location within your content hierarchy.
