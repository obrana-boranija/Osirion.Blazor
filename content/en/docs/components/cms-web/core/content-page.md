---
title: "Content Page - Osirion Blazor CMS"
description: "Display individual CMS content pages with metadata, navigation, breadcrumbs, and related content for Blazor applications."
category: "CMS Web Components"
subcategory: "Core"
tags: ["cms", "content", "page", "article", "blog", "navigation", "seo"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "content-page"
section: "components"
layout: "component"
seo:
  title: "Content Page Component | Osirion Blazor CMS Documentation"
  description: "Learn how to display individual content pages with metadata, navigation, and related content using ContentPage component."
  keywords: ["Blazor", "CMS", "content page", "article view", "blog post", "metadata", "navigation"]
  canonical: "/docs/components/cms.web/core/content-page"
  image: "/images/components/content-page-preview.jpg"
navigation:
  parent: "CMS Web Components"
  order: 2
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Core"
    link: "/docs/components/cms.web/core"
  - text: "Content Page"
    link: "/docs/components/cms.web/core/content-page"
---

# Content Page Component

The **ContentPage** component provides a comprehensive solution for displaying individual content items such as blog posts, articles, documentation pages, and other CMS content. It includes built-in support for metadata, navigation, breadcrumbs, sharing, and related content.

## Overview

This component automatically handles content loading, SEO optimization, and provides a rich reading experience with features like breadcrumb navigation, previous/next navigation, social sharing, and related content suggestions. It's designed to work seamlessly with the Osirion CMS content management system.

## Key Features

- **Complete Content Display**: Full article rendering with metadata and navigation
- **SEO Optimization**: Automatic meta tags and structured data generation
- **Breadcrumb Navigation**: Hierarchical navigation based on content structure
- **Content Navigation**: Previous/next article navigation with smart linking
- **Social Sharing**: Built-in social media sharing functionality
- **Related Content**: Automatic related content suggestions
- **Responsive Design**: Mobile-first layout that adapts to all screen sizes
- **Accessibility Ready**: Full WCAG compliance with screen reader support
- **Localization Support**: Multi-language content display capabilities
- **Schema.org Markup**: Structured data for better search engine understanding
- **Error Handling**: Professional 404 and error state management
- **Performance Optimized**: Lazy loading and efficient rendering

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Path` | `string?` | `null` | Content path to load and display |
| `ContentItem` | `ContentItem?` | `null` | Pre-loaded content item to display |
| `PreviousItem` | `ContentItem?` | `null` | Previous content item for navigation |
| `NextItem` | `ContentItem?` | `null` | Next content item for navigation |
| `RelatedItems` | `IReadOnlyList<ContentItem>?` | `null` | Related content items to display |
| `LoadingText` | `string` | `"Loading content..."` | Text shown during loading |
| `NotFoundTitle` | `string` | `"Content Not Found"` | Title for 404 pages |
| `NotFoundText` | `string` | `"The requested content could not be found."` | 404 page message |
| `ShowBackLink` | `bool` | `true` | Show back link on 404 pages |
| `BackLinkUrl` | `string` | `"/"` | URL for back link |
| `BackLinkText` | `string` | `"Back to Home"` | Text for back link |
| `ShowBreadcrumbs` | `bool` | `true` | Enable breadcrumb navigation |
| `ShowBreadcrumbHome` | `bool` | `true` | Show home link in breadcrumbs |
| `BreadcrumbHomeUrl` | `string` | `"/"` | Home URL for breadcrumbs |
| `BreadcrumbHomeText` | `string` | `"Home"` | Home text for breadcrumbs |
| `ShowMetadata` | `bool` | `true` | Display content metadata |
| `FeaturedImageCaption` | `string?` | `null` | Caption for featured image |
| `ShowFooter` | `bool` | `true` | Show content footer |
| `ShowCategories` | `bool` | `true` | Display categories in sidebar |
| `CategoriesTitle` | `string` | `"Categories:"` | Title for categories section |
| `ShowTags` | `bool` | `true` | Display tags in sidebar |
| `TagsTitle` | `string` | `"Tags:"` | Title for tags section |
| `ShowNavigationLinks` | `bool` | `true` | Show previous/next navigation |
| `PreviousItemLabel` | `string` | `"Previous"` | Label for previous link |
| `NextItemLabel` | `string` | `"Next"` | Label for next link |
| `ShowShareLinks` | `bool` | `false` | Enable social sharing |
| `ShareLinksTitle` | `string` | `"Share:"` | Title for sharing section |
| `ShareLinks` | `List<ShareLink>` | `new()` | Social sharing links |
| `FooterContent` | `string?` | `null` | Custom footer HTML content |
| `ShowRelatedContent` | `bool` | `false` | Display related content |
| `RelatedContentTitle` | `string` | `"Related Content"` | Title for related content section |

## Basic Usage

### Simple Content Display

```razor
@page "/articles/{slug}"
@using Osirion.Blazor.Cms.Web.Components

<ContentPage Path="@($"/articles/{Slug}")" />

@code {
    [Parameter] public string Slug { get; set; } = string.Empty;
}
```

### Pre-loaded Content

```razor
<ContentPage ContentItem="@article"
             PreviousItem="@previousArticle"
             NextItem="@nextArticle" />

@code {
    private ContentItem? article;
    private ContentItem? previousArticle;
    private ContentItem? nextArticle;
    
    protected override async Task OnParametersSetAsync()
    {
        // Load content items
        article = await ContentService.GetBySlugAsync(Slug);
        if (article != null)
        {
            previousArticle = await ContentService.GetPreviousAsync(article.Id);
            nextArticle = await ContentService.GetNextAsync(article.Id);
        }
    }
}
```

## Advanced Examples

### Blog Post with Full Features

```razor
@page "/blog/{slug}"

<ContentPage Path="@contentPath"
             ShowBreadcrumbs="true"
             ShowMetadata="true"
             ShowNavigationLinks="true"
             ShowShareLinks="true"
             ShareLinks="@shareLinks"
             ShowRelatedContent="true"
             RelatedItems="@relatedPosts"
             FooterContent="@footerHtml" />

@code {
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private string contentPath => $"/blog/{Slug}";
    private List<ContentItem>? relatedPosts;
    private List<ShareLink> shareLinks = new();
    private string footerHtml = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        await SetupSocialSharing();
        await LoadRelatedContent();
        SetupFooter();
    }
    
    private async Task SetupSocialSharing()
    {
        var currentUrl = Navigation.Uri;
        var article = await ContentService.GetBySlugAsync(Slug);
        
        if (article != null)
        {
            shareLinks = new List<ShareLink>
            {
                new ShareLink
                {
                    Name = "Twitter",
                    Label = "Share on Twitter",
                    Url = $"https://twitter.com/intent/tweet?url={Uri.EscapeDataString(currentUrl)}&text={Uri.EscapeDataString(article.Title)}",
                    Icon = "<svg>...</svg>" // Twitter icon
                },
                new ShareLink
                {
                    Name = "Facebook",
                    Label = "Share on Facebook",
                    Url = $"https://www.facebook.com/sharer/sharer.php?u={Uri.EscapeDataString(currentUrl)}",
                    Icon = "<svg>...</svg>" // Facebook icon
                },
                new ShareLink
                {
                    Name = "LinkedIn",
                    Label = "Share on LinkedIn",
                    Url = $"https://www.linkedin.com/sharing/share-offsite/?url={Uri.EscapeDataString(currentUrl)}",
                    Icon = "<svg>...</svg>" // LinkedIn icon
                }
            };
        }
    }
    
    private async Task LoadRelatedContent()
    {
        var article = await ContentService.GetBySlugAsync(Slug);
        if (article != null)
        {
            relatedPosts = await ContentService.GetRelatedAsync(article.Id, 3);
        }
    }
    
    private void SetupFooter()
    {
        footerHtml = @"
            <div class='article-footer'>
                <p><strong>Like this article?</strong> Subscribe to our newsletter for more content like this.</p>
                <button class='btn btn-primary'>Subscribe Now</button>
            </div>";
    }
}
```

### Documentation Page

```razor
@page "/docs/{*path}"

<ContentPage Path="@($"/docs/{Path}")"
             ShowBreadcrumbs="true"
             BreadcrumbHomeText="Documentation"
             BreadcrumbHomeUrl="/docs"
             ShowCategories="false"
             ShowTags="false"
             ShowNavigationLinks="true"
             PreviousItemLabel="← Previous"
             NextItemLabel="Next →"
             ShowRelatedContent="true"
             RelatedContentTitle="Related Documentation" />

@code {
    [Parameter] public string Path { get; set; } = string.Empty;
}
```

### Localized Content Page

```razor
@page "/{locale}/articles/{slug}"

<ContentPage Path="@contentPath"
             ShowBreadcrumbs="true"
             BreadcrumbHomeText="@GetLocalizedText("home")"
             LoadingText="@GetLocalizedText("loading")"
             NotFoundTitle="@GetLocalizedText("not_found_title")"
             NotFoundText="@GetLocalizedText("not_found_message")"
             BackLinkText="@GetLocalizedText("back_home")"
             CategoriesTitle="@GetLocalizedText("categories")"
             TagsTitle="@GetLocalizedText("tags")"
             ShareLinksTitle="@GetLocalizedText("share")" />

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private string contentPath => $"/{Locale}/articles/{Slug}";
    
    private string GetLocalizedText(string key)
    {
        return Localizer[key];
    }
}
```

### Custom Layout with Sidebar

```razor
<div class="custom-article-layout">
    <div class="article-container">
        <ContentPage ContentItem="@currentArticle"
                     ShowBreadcrumbs="true"
                     ShowCategories="false"
                     ShowTags="false"
                     ShowRelatedContent="false" />
    </div>
    
    <aside class="custom-sidebar">
        <div class="sidebar-section">
            <h3>Table of Contents</h3>
            <TableOfContents Content="@currentArticle?.Content" />
        </div>
        
        <div class="sidebar-section">
            <h3>Categories</h3>
            <CategoriesList ShowCount="true" />
        </div>
        
        <div class="sidebar-section">
            <h3>Popular Tags</h3>
            <TagCloud Limit="20" ShowCount="true" />
        </div>
        
        <div class="sidebar-section">
            <h3>Recent Articles</h3>
            <ContentList Directory="/articles" 
                         ItemsPerPage="5" 
                         ShowPagination="false" />
        </div>
    </aside>
</div>

@code {
    [Parameter] public ContentItem? CurrentArticle { get; set; }
    
    private ContentItem? currentArticle => CurrentArticle;
}
```

### E-commerce Product Page

```razor
@page "/products/{slug}"

<ContentPage Path="@($"/products/{Slug}")"
             ShowBreadcrumbs="true"
             BreadcrumbHomeText="Products"
             BreadcrumbHomeUrl="/products"
             ShowMetadata="false"
             ShowCategories="true"
             CategoriesTitle="Product Categories"
             ShowTags="true"
             TagsTitle="Product Tags"
             ShowRelatedContent="true"
             RelatedContentTitle="Related Products"
             FooterContent="@productFooter" />

@code {
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private string productFooter = @"
        <div class='product-actions'>
            <button class='btn btn-primary btn-lg'>Add to Cart</button>
            <button class='btn btn-outline-secondary'>Add to Wishlist</button>
        </div>
        <div class='product-guarantee mt-3'>
            <p><i class='fa fa-shield'></i> 30-day money-back guarantee</p>
        </div>";
}
```

### Newsletter Article

```razor
<ContentPage ContentItem="@newsletterArticle"
             ShowBreadcrumbs="false"
             ShowCategories="false"
             ShowTags="false"
             ShowNavigationLinks="false"
             ShowShareLinks="true"
             ShareLinks="@emailShareLinks"
             FooterContent="@newsletterFooter" />

@code {
    private ContentItem? newsletterArticle;
    private List<ShareLink> emailShareLinks = new();
    private string newsletterFooter = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        await SetupEmailSharing();
        SetupNewsletterFooter();
    }
    
    private async Task SetupEmailSharing()
    {
        emailShareLinks = new List<ShareLink>
        {
            new ShareLink
            {
                Name = "Email",
                Label = "Share via Email",
                Url = $"mailto:?subject={Uri.EscapeDataString("Check out this article")}&body={Uri.EscapeDataString(Navigation.Uri)}",
                Icon = "<svg>...</svg>"
            },
            new ShareLink
            {
                Name = "Print",
                Label = "Print Article",
                Url = "javascript:window.print()",
                Icon = "<svg>...</svg>"
            }
        };
    }
    
    private void SetupNewsletterFooter()
    {
        newsletterFooter = @"
            <div class='newsletter-footer'>
                <p>You received this email because you subscribed to our newsletter.</p>
                <a href='/unsubscribe'>Unsubscribe</a> | <a href='/newsletter/archive'>View Archive</a>
            </div>";
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-content-page {
    /* Main article container */
    max-width: 800px;
    margin: 0 auto;
    padding: 2rem 1rem;
}

.osirion-content-loading {
    /* Loading state */
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 3rem 1rem;
}

.osirion-content-not-found {
    /* 404 state */
    text-align: center;
    padding: 3rem 1rem;
    color: var(--bs-text-muted);
}

.osirion-content-main {
    /* Main content area */
    display: grid;
    grid-template-columns: 1fr 300px;
    gap: 2rem;
    margin-top: 2rem;
}

.osirion-content-primary {
    /* Primary content column */
    min-width: 0; /* Prevents overflow */
}

.osirion-content-sidebar {
    /* Sidebar column */
    position: sticky;
    top: 2rem;
    height: fit-content;
}

.osirion-content-share {
    /* Social sharing section */
    border-top: 1px solid var(--bs-border-color);
    padding-top: 1.5rem;
    margin-top: 2rem;
}

.osirion-content-related {
    /* Related content section */
    border-top: 2px solid var(--bs-border-color);
    padding-top: 2rem;
    margin-top: 3rem;
}

.osirion-content-related-grid {
    /* Related content grid */
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1.5rem;
    margin-top: 1rem;
}
```

### Responsive Design

```css
/* Mobile-first responsive design */
@media (max-width: 768px) {
    .osirion-content-main {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .osirion-content-sidebar {
        position: static;
        order: 2;
    }
    
    .osirion-content-related-grid {
        grid-template-columns: 1fr;
    }
}

@media (max-width: 576px) {
    .osirion-content-page {
        padding: 1rem 0.5rem;
    }
    
    .osirion-content-share-links {
        flex-direction: column;
        gap: 0.5rem;
    }
}
```

### Custom Styling Examples

```css
/* Modern article design */
.modern-article .osirion-content-page {
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    padding: 3rem;
}

/* Dark theme support */
@media (prefers-color-scheme: dark) {
    .osirion-content-page {
        background: var(--bs-dark);
        color: var(--bs-light);
    }
    
    .osirion-content-share {
        border-color: var(--bs-secondary);
    }
}

/* Print styles */
@media print {
    .osirion-content-sidebar,
    .osirion-content-share,
    .osirion-content-related {
        display: none;
    }
    
    .osirion-content-main {
        grid-template-columns: 1fr;
    }
}
```

## SEO and Schema.org Integration

### Automatic SEO Tags

```razor
<ContentPage Path="@articlePath"
             @oncontentloaded="HandleContentLoaded" />

@code {
    private async Task HandleContentLoaded(ContentItem content)
    {
        // Set page title and meta tags
        await JSRuntime.InvokeVoidAsync("updatePageMeta", new
        {
            title = content.Title,
            description = content.Description,
            keywords = string.Join(", ", content.Tags),
            author = content.Author,
            publishedTime = content.DateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            modifiedTime = content.DateModified?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            image = content.FeaturedImageUrl,
            url = Navigation.Uri
        });
    }
}
```

### Custom Schema.org Markup

```razor
<ContentPage ContentItem="@article">
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "Article",
        "headline": "@article?.Title",
        "description": "@article?.Description",
        "author": {
            "@type": "Person",
            "name": "@article?.Author"
        },
        "datePublished": "@article?.DateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ")",
        "dateModified": "@(article?.DateModified?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? article?.DateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ"))",
        "image": "@article?.FeaturedImageUrl",
        "publisher": {
            "@type": "Organization",
            "name": "Your Site Name",
            "logo": {
                "@type": "ImageObject",
                "url": "@(Navigation.BaseUri)logo.png"
            }
        }
    }
    </script>
</ContentPage>
```

## Performance Optimization

### Lazy Loading Implementation

```razor
<ContentPage Path="@articlePath"
             @ref="contentPageRef" />

@code {
    private ContentPage? contentPageRef;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("observeImages", ".osirion-content-page img");
        }
    }
}
```

### Caching Strategy

```razor
@implements IDisposable

<ContentPage ContentItem="@cachedContent" />

@code {
    private ContentItem? cachedContent;
    private readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
    
    protected override async Task OnParametersSetAsync()
    {
        var cacheKey = $"content_{Path}";
        
        if (!cache.TryGetValue(cacheKey, out cachedContent))
        {
            cachedContent = await ContentService.GetByPathAsync(Path);
            
            if (cachedContent != null)
            {
                cache.Set(cacheKey, cachedContent, TimeSpan.FromMinutes(10));
            }
        }
    }
    
    public void Dispose()
    {
        cache.Dispose();
    }
}
```

## Accessibility Features

- **Semantic HTML**: Proper use of `<article>`, `<nav>`, and heading structure
- **ARIA Labels**: Comprehensive labeling for screen readers
- **Keyboard Navigation**: Full keyboard support for all interactive elements
- **Focus Management**: Logical focus flow and visible indicators
- **Screen Reader Support**: Descriptive content and navigation aids
- **Skip Links**: Allow users to skip to main content

### Accessibility Enhancement

```razor
<ContentPage Path="@articlePath"
             aria-label="Article content"
             role="main">
    
    <!-- Skip link for screen readers -->
    <a href="#main-content" class="sr-only sr-only-focusable">
        Skip to main content
    </a>
    
    <!-- Enhanced focus indicators -->
    <style>
        .osirion-content-page *:focus {
            outline: 2px solid var(--bs-primary);
            outline-offset: 2px;
        }
        
        .sr-only:not(:focus):not(:active) {
            position: absolute;
            width: 1px;
            height: 1px;
            padding: 0;
            margin: -1px;
            overflow: hidden;
            clip: rect(0, 0, 0, 0);
            white-space: nowrap;
            border: 0;
        }
    </style>
</ContentPage>
```

## Integration Examples

### With Analytics

```razor
<ContentPage Path="@articlePath"
             OnContentViewed="TrackPageView"
             OnShareClicked="TrackShare" />

@code {
    private async Task TrackPageView(ContentItem content)
    {
        await AnalyticsService.TrackEvent("page_view", new
        {
            page_title = content.Title,
            page_location = Navigation.Uri,
            content_group1 = content.Category,
            content_group2 = string.Join(",", content.Tags)
        });
    }
    
    private async Task TrackShare(string platform)
    {
        await AnalyticsService.TrackEvent("share", new
        {
            method = platform,
            content_type = "article",
            item_id = currentArticle?.Id
        });
    }
}
```

### With Comments System

```razor
<ContentPage Path="@articlePath"
             FooterContent="@commentsHtml" />

@code {
    private string commentsHtml = @"
        <div id='comments-section'>
            <h3>Comments</h3>
            <div id='disqus_thread'></div>
            <script>
                var disqus_config = function () {
                    this.page.url = window.location.href;
                    this.page.identifier = '" + articleId + @"';
                };
                // Load Disqus script
            </script>
        </div>";
}
```

## Best Practices

1. **SEO**: Always include proper meta tags and structured data
2. **Performance**: Implement lazy loading for images and related content
3. **Accessibility**: Ensure proper heading structure and ARIA labels
4. **Mobile**: Test responsive behavior on various devices
5. **Social**: Implement Open Graph and Twitter Card meta tags
6. **Analytics**: Track user engagement and content performance
7. **Error Handling**: Provide meaningful 404 and error pages
8. **Caching**: Cache content appropriately to improve performance

## Common Use Cases

- **Blog Articles**: Personal and corporate blog posts
- **Documentation**: Technical documentation and guides
- **News Articles**: News and journalism content
- **Product Pages**: E-commerce product descriptions
- **Portfolio Items**: Creative work showcases
- **Case Studies**: Business case studies and reports
- **Help Articles**: Support and knowledge base content

The ContentPage component provides a robust foundation for displaying any type of content with professional features and excellent user experience.
