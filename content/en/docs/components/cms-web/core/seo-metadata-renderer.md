---
title: "SEO Metadata Renderer - Osirion Blazor CMS"
description: "Comprehensive SEO metadata rendering component with Open Graph, Twitter Cards, JSON-LD structured data, and search engine optimization features."
category: "CMS Web Components"
subcategory: "Core"
tags: ["cms", "seo", "metadata", "open-graph", "twitter-cards", "json-ld", "structured-data", "schema"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "seo-metadata-renderer"
section: "components"
layout: "component"
seo:
  title: "SEO Metadata Renderer Component | Osirion Blazor CMS Documentation"
  description: "Master SEO optimization with comprehensive metadata rendering including Open Graph, Twitter Cards, and JSON-LD structured data."
  keywords: ["Blazor", "CMS", "SEO", "metadata", "Open Graph", "Twitter Cards", "JSON-LD", "structured data", "schema.org"]
  canonical: "/docs/components/cms.web/core/seo-metadata-renderer"
  image: "/images/components/seo-metadata-renderer-preview.jpg"
navigation:
  parent: "CMS Web Components"
  order: 6
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Core"
    link: "/docs/components/cms.web/core"
  - text: "SEO Metadata Renderer"
    link: "/docs/components/cms.web/core/seo-metadata-renderer"
---

# SEO Metadata Renderer Component

The **SeoMetadataRenderer** component provides comprehensive search engine optimization (SEO) metadata rendering for Blazor CMS applications. It automatically generates meta tags, Open Graph properties, Twitter Cards, and JSON-LD structured data to maximize search engine visibility and social media sharing capabilities.

## Overview

This component takes a ContentItem and renders all necessary SEO metadata in the document head, including meta tags for search engines, social media platforms, and structured data for rich search results. It supports customizable schema types, AI content discovery tags, and performance optimization hints.

## Key Features

- **Complete Meta Tags**: Title, description, keywords, author, robots, and more
- **Open Graph Support**: Full Facebook/LinkedIn sharing optimization
- **Twitter Cards**: Rich Twitter sharing with image and content previews
- **JSON-LD Structured Data**: Schema.org markup for rich search results
- **AI Content Tags**: Modern AI discovery and attribution metadata
- **Performance Hints**: DNS prefetch and preconnect optimization
- **Canonical URLs**: Duplicate content prevention
- **Multiple Schema Types**: Article, BlogPosting, WebPage support
- **Image URL Normalization**: Automatic relative/absolute URL handling
- **Localization Support**: Locale-aware metadata generation

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Content` | `ContentItem?` | `null` | **Required.** Content item to generate metadata for |
| `SiteNameOverride` | `string?` | `null` | Override the automatically extracted site name |
| `TwitterSite` | `string?` | `null` | Twitter site handle (e.g., "@yoursite") |
| `TwitterCreator` | `string?` | `null` | Twitter creator handle (e.g., "@author") |
| `SchemaType` | `SchemaType?` | `null` | Override schema type (Article, BlogPosting, WebPage) |
| `AllowAiDiscovery` | `bool` | `true` | Include AI content discovery and attribution tags |

## Basic Usage

### Simple Content Page

```razor
@using Osirion.Blazor.Cms.Web.Components

<SeoMetadataRenderer Content="@article" />

@code {
    private ContentItem? article;
    
    protected override async Task OnInitializedAsync()
    {
        article = await ContentService.GetByPathAsync("/articles/getting-started");
    }
}
```

### Blog Post with Twitter Integration

```razor
<SeoMetadataRenderer Content="@blogPost"
                     TwitterSite="@myBlog"
                     TwitterCreator="@author"
                     SchemaType="SchemaType.BlogPosting" />

@code {
    private ContentItem? blogPost;
    private string myBlog = "@MyTechBlog";
    private string author = "@JohnDoe";
}
```

### Custom Site Name and AI Settings

```razor
<SeoMetadataRenderer Content="@content"
                     SiteNameOverride="My Custom Site"
                     AllowAiDiscovery="false" />

@code {
    // AI discovery disabled for sensitive content
    private ContentItem? content;
}
```

## Advanced Examples

### Dynamic Blog Platform

```razor
@page "/blog/{slug}"
@using Osirion.Blazor.Cms.Web.Components

<SeoMetadataRenderer Content="@blogPost"
                     TwitterSite="@twitterHandle"
                     TwitterCreator="@GetAuthorTwitter(blogPost?.Author)"
                     SchemaType="SchemaType.BlogPosting" />

<article class="blog-post">
    @if (blogPost != null)
    {
        <header class="post-header">
            <h1>@blogPost.Title</h1>
            <div class="post-meta">
                <time datetime="@blogPost.PublishDate.ToString("yyyy-MM-dd")">
                    @blogPost.PublishDate.ToString("MMMM dd, yyyy")
                </time>
                <span class="author">by @blogPost.Author</span>
            </div>
        </header>
        
        @if (!string.IsNullOrEmpty(blogPost.FeaturedImageUrl))
        {
            <img src="@blogPost.FeaturedImageUrl" alt="@blogPost.Title" class="featured-image" />
        }
        
        <div class="post-content">
            @((MarkupString)blogPost.Content)
        </div>
        
        @if (blogPost.Tags?.Any() == true)
        {
            <footer class="post-tags">
                <h3>Tags</h3>
                @foreach (var tag in blogPost.Tags)
                {
                    <a href="/blog/tag/@tag.ToLower()" class="tag">@tag</a>
                }
            </footer>
        }
    }
</article>

@code {
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private ContentItem? blogPost;
    private string twitterHandle = "@MyTechBlog";
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Slug))
        {
            blogPost = await ContentService.GetBySlugAsync($"blog/{Slug}");
        }
    }
    
    private string? GetAuthorTwitter(string? author)
    {
        return author switch
        {
            "John Doe" => "@johndoe_dev",
            "Jane Smith" => "@janesmith_tech",
            "Mike Johnson" => "@mikej_coding",
            _ => null
        };
    }
}
```

### E-commerce Product Pages

```razor
@page "/products/{category}/{slug}"

<SeoMetadataRenderer Content="@productContent"
                     SiteNameOverride="TechShop"
                     SchemaType="SchemaType.WebPage" />

<!-- Additional Product Schema -->
<script type="application/ld+json">
{
  "@context": "https://schema.org/",
  "@type": "Product",
  "name": "@product?.Name",
  "image": "@GetProductImages()",
  "description": "@product?.Description",
  "brand": {
    "@type": "Brand",
    "name": "@product?.Brand"
  },
  "offers": {
    "@type": "Offer",
    "url": "@NavigationManager.Uri",
    "priceCurrency": "@product?.Currency",
    "price": "@product?.Price.ToString("F2")",
    "availability": "@GetAvailabilitySchema()"
  },
  "aggregateRating": {
    "@type": "AggregateRating",
    "ratingValue": "@product?.AverageRating.ToString("F1")",
    "reviewCount": "@product?.ReviewCount"
  }
}
</script>

@code {
    [Parameter] public string Category { get; set; } = string.Empty;
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private ContentItem? productContent;
    private ProductItem? product;
    
    protected override async Task OnParametersSetAsync()
    {
        product = await ProductService.GetBySlugAsync(Category, Slug);
        
        if (product != null)
        {
            // Convert product to ContentItem for SEO rendering
            productContent = new ContentItem
            {
                Title = product.Name,
                Description = product.Description,
                FeaturedImageUrl = product.MainImage,
                Tags = product.Tags,
                Categories = new[] { Category },
                PublishDate = product.CreatedDate,
                LastModified = product.UpdatedDate,
                Author = "TechShop",
                Path = $"/products/{Category}/{Slug}",
                Metadata = new ContentMetadata
                {
                    SeoProperties = new SeoProperties
                    {
                        Title = $"{product.Name} - {product.Brand} | TechShop",
                        Description = $"Buy {product.Name} by {product.Brand}. {product.Description}",
                        OgType = "product",
                        OgImageUrl = product.MainImage,
                        TwitterCard = "summary_large_image"
                    }
                }
            };
        }
    }
    
    private string GetProductImages()
    {
        return product?.Images?.Any() == true
            ? JsonSerializer.Serialize(product.Images.Select(img => $"{NavigationManager.BaseUri.TrimEnd('/')}/{img}"))
            : $"\"{NavigationManager.BaseUri.TrimEnd('/')}/{product?.MainImage}\"";
    }
    
    private string GetAvailabilitySchema()
    {
        return product?.InStock == true
            ? "https://schema.org/InStock"
            : "https://schema.org/OutOfStock";
    }
}
```

### Documentation Site with Version Support

```razor
@page "/docs/{version}/{*path}"

<SeoMetadataRenderer Content="@docContent"
                     SiteNameOverride="DevDocs"
                     SchemaType="SchemaType.Article"
                     AllowAiDiscovery="true" />

<!-- Additional Technical Documentation Schema -->
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "TechArticle",
  "headline": "@docContent?.Title",
  "description": "@docContent?.Description", 
  "author": {
    "@type": "Organization",
    "name": "DevDocs Team"
  },
  "publisher": {
    "@type": "Organization",
    "name": "DevDocs",
    "logo": {
      "@type": "ImageObject",
      "url": "@($"{NavigationManager.BaseUri}logo.png")"
    }
  },
  "datePublished": "@docContent?.PublishDate.ToString("yyyy-MM-ddTHH:mm:ssZ")",
  "dateModified": "@docContent?.LastModified?.ToString("yyyy-MM-ddTHH:mm:ssZ")",
  "about": {
    "@type": "SoftwareApplication",
    "name": "@GetSoftwareName()",
    "applicationCategory": "DeveloperApplication"
  },
  "audience": {
    "@type": "Audience",
    "audienceType": "Developers"
  }
}
</script>

@code {
    [Parameter] public string Version { get; set; } = "latest";
    [Parameter] public string Path { get; set; } = string.Empty;
    
    private ContentItem? docContent;
    
    protected override async Task OnParametersSetAsync()
    {
        var fullPath = $"/docs/{Version}/{Path}";
        docContent = await DocumentationService.GetByPathAsync(fullPath);
        
        if (docContent?.Metadata?.SeoProperties == null)
        {
            // Enhance SEO for documentation
            docContent.Metadata ??= new ContentMetadata();
            docContent.Metadata.SeoProperties = new SeoProperties
            {
                Title = $"{docContent.Title} - {Version} | DevDocs",
                Description = $"{docContent.Description} Documentation for version {Version}.",
                OgType = "article",
                TwitterCard = "summary",
                Robots = "index,follow,noarchive"
            };
        }
    }
    
    private string GetSoftwareName()
    {
        return Path.Split('/').FirstOrDefault()?.Replace("-", " ").ToTitleCase() ?? "DevDocs";
    }
}
```

### News Article with Rich Media

```razor
@page "/news/{year:int}/{month:int}/{slug}"

<SeoMetadataRenderer Content="@newsArticle"
                     TwitterSite="@newsHandle"
                     TwitterCreator="@GetJournalistTwitter(newsArticle?.Author)"
                     SchemaType="SchemaType.Article" />

<!-- News-specific Schema -->
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "NewsArticle",
  "headline": "@newsArticle?.Title",
  "description": "@newsArticle?.Description",
  "image": "@GetNewsImages()",
  "author": {
    "@type": "Person", 
    "name": "@newsArticle?.Author",
    "url": "@GetAuthorProfileUrl(newsArticle?.Author)"
  },
  "publisher": {
    "@type": "Organization",
    "name": "Daily Tech News",
    "logo": {
      "@type": "ImageObject",
      "url": "@($"{NavigationManager.BaseUri}images/logo.png")"
    }
  },
  "datePublished": "@newsArticle?.PublishDate.ToString("yyyy-MM-ddTHH:mm:ssZ")",
  "dateModified": "@newsArticle?.LastModified?.ToString("yyyy-MM-ddTHH:mm:ssZ")",
  "articleSection": "@newsArticle?.Categories?.FirstOrDefault()",
  "keywords": "@GetKeywords()",
  "wordCount": "@GetWordCount()",
  "isAccessibleForFree": true,
  "hasPart": @GetArticleParts()
}
</script>

@code {
    [Parameter] public int Year { get; set; }
    [Parameter] public int Month { get; set; }
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private ContentItem? newsArticle;
    private string newsHandle = "@DailyTechNews";
    
    protected override async Task OnParametersSetAsync()
    {
        var path = $"/news/{Year:D4}/{Month:D2}/{Slug}";
        newsArticle = await NewsService.GetByPathAsync(path);
    }
    
    private string? GetJournalistTwitter(string? author)
    {
        return author switch
        {
            "Sarah Connor" => "@sarahc_tech",
            "John Smith" => "@johnsmith_news",
            "Maria Garcia" => "@mariagarcia_reporter",
            _ => null
        };
    }
    
    private string GetNewsImages()
    {
        var images = new List<string>();
        
        if (!string.IsNullOrEmpty(newsArticle?.FeaturedImageUrl))
            images.Add($"{NavigationManager.BaseUri.TrimEnd('/')}/{newsArticle.FeaturedImageUrl}");
            
        // Add any gallery images
        if (newsArticle?.Metadata?.CustomProperties?.ContainsKey("gallery") == true)
        {
            var gallery = newsArticle.Metadata.CustomProperties["gallery"] as string[];
            if (gallery?.Any() == true)
            {
                images.AddRange(gallery.Select(img => $"{NavigationManager.BaseUri.TrimEnd('/')}/{img}"));
            }
        }
        
        return JsonSerializer.Serialize(images);
    }
    
    private string GetAuthorProfileUrl(string? author)
    {
        return string.IsNullOrEmpty(author) 
            ? string.Empty 
            : $"{NavigationManager.BaseUri}authors/{author.ToLower().Replace(" ", "-")}";
    }
    
    private string GetKeywords()
    {
        return newsArticle?.Tags?.Any() == true 
            ? string.Join(", ", newsArticle.Tags) 
            : string.Empty;
    }
    
    private int GetWordCount()
    {
        return newsArticle?.Content?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length ?? 0;
    }
    
    private string GetArticleParts()
    {
        // For articles with multiple sections/videos/images
        var parts = new List<object>();
        
        // Add video if present
        if (newsArticle?.Metadata?.CustomProperties?.ContainsKey("videoUrl") == true)
        {
            parts.Add(new
            {
                @type = "VideoObject",
                name = $"{newsArticle.Title} - Video",
                embedUrl = newsArticle.Metadata.CustomProperties["videoUrl"]
            });
        }
        
        return JsonSerializer.Serialize(parts);
    }
}
```

### Multi-language Content

```razor
@page "/{locale}/articles/{slug}"

<SeoMetadataRenderer Content="@localizedArticle"
                     SiteNameOverride="@GetLocalizedSiteName(Locale)" />

<!-- Language-specific hreflang tags -->
@if (availableTranslations?.Any() == true)
{
    @foreach (var translation in availableTranslations)
    {
        <link rel="alternate" hreflang="@translation.Key" href="@GetTranslationUrl(translation)" />
    }
    <link rel="alternate" hreflang="x-default" href="@GetDefaultUrl()" />
}

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private ContentItem? localizedArticle;
    private Dictionary<string, string>? availableTranslations;
    
    protected override async Task OnParametersSetAsync()
    {
        localizedArticle = await ContentService.GetLocalizedAsync($"articles/{Slug}", Locale);
        availableTranslations = await ContentService.GetTranslationsAsync($"articles/{Slug}");
        
        // Ensure proper locale in metadata
        if (localizedArticle?.Metadata?.SeoProperties != null)
        {
            localizedArticle.Metadata.SeoProperties.Locale = Locale;
        }
    }
    
    private string GetLocalizedSiteName(string locale)
    {
        return locale switch
        {
            "es" => "Mi Sitio Web",
            "fr" => "Mon Site Web",
            "de" => "Meine Website",
            _ => "My Website"
        };
    }
    
    private string GetTranslationUrl(KeyValuePair<string, string> translation)
    {
        return $"{NavigationManager.BaseUri.TrimEnd('/')}/{translation.Key}/articles/{translation.Value}";
    }
    
    private string GetDefaultUrl()
    {
        return $"{NavigationManager.BaseUri.TrimEnd('/')}/en/articles/{Slug}";
    }
}
```

## Generated Metadata Structure

The SeoMetadataRenderer generates the following metadata:

### Basic Meta Tags
```html
<title>Article Title | Site Name</title>
<meta name="description" content="Article description..." />
<meta name="author" content="Author Name" />
<meta name="robots" content="index,follow" />
<meta name="keywords" content="tag1, tag2, tag3" />
<meta name="generator" content="Osirion Blazor CMS" />
```

### Open Graph Tags
```html
<meta name="og:title" content="Article Title" />
<meta name="og:description" content="Article description..." />
<meta name="og:type" content="article" />
<meta name="og:url" content="https://example.com/article" />
<meta name="og:image" content="https://example.com/image.jpg" />
<meta name="og:site_name" content="Site Name" />
<meta name="og:locale" content="en" />
```

### Twitter Cards
```html
<meta name="twitter:card" content="summary_large_image" />
<meta name="twitter:title" content="Article Title" />
<meta name="twitter:description" content="Article description..." />
<meta name="twitter:image" content="https://example.com/image.jpg" />
<meta name="twitter:site" content="@yoursite" />
```

### Article-Specific Tags
```html
<meta name="article:published_time" content="2024-01-15" />
<meta name="article:modified_time" content="2024-01-20" />
<meta name="article:author" content="Author Name" />
<meta name="article:section" content="Technology" />
<meta name="article:tag" content="blazor" />
<meta name="article:tag" content="cms" />
```

### AI Discovery Tags
```html
<meta name="ai-content-declaration" content="human-created" />
<meta name="ai-content-detection" content="allow" />
<meta name="ai-content-attribution" content="original" />
<meta name="ai-crawling" content="all" />
<meta name="ai-training" content="allow" />
```

### JSON-LD Structured Data
```json
{
  "@context": "https://schema.org",
  "@type": "Article",
  "headline": "Article Title",
  "description": "Article description...",
  "author": {
    "@type": "Person",
    "name": "Author Name"
  },
  "publisher": {
    "@type": "Organization",
    "name": "Site Name"
  },
  "datePublished": "2024-01-15T10:00:00Z",
  "dateModified": "2024-01-20T15:30:00Z",
  "image": "https://example.com/image.jpg",
  "url": "https://example.com/article"
}
```

## Schema Types

### Article Schema
Best for news articles, blog posts, and editorial content:
```csharp
SchemaType.Article
```

### BlogPosting Schema  
Optimized for blog posts and personal articles:
```csharp
SchemaType.BlogPosting
```

### WebPage Schema
Generic page content, products, services:
```csharp
SchemaType.WebPage
```

## Performance Optimization

### DNS Prefetch and Preconnect
The component automatically adds performance hints:
```html
<meta name="dns-prefetch" content="https://example.com" />
<meta name="preconnect" content="https://example.com" />
```

### Image URL Optimization
Automatic URL normalization for images:
```csharp
// Relative URLs are converted to absolute
"/images/hero.jpg" → "https://example.com/images/hero.jpg"

// Absolute URLs are preserved
"https://cdn.example.com/image.jpg" → "https://cdn.example.com/image.jpg"
```

## SEO Best Practices

1. **Content Quality**: Ensure content has meaningful titles and descriptions
2. **Image Optimization**: Provide featured images for better social sharing
3. **Structured Data**: Use appropriate schema types for content
4. **Canonical URLs**: Prevent duplicate content issues
5. **Social Media**: Configure Twitter handles for proper attribution
6. **Performance**: Utilize DNS prefetch hints
7. **Localization**: Set proper locale information for international sites
8. **AI Attribution**: Consider AI discovery settings based on content type

## Common Use Cases

- **Blog Platforms**: Complete SEO for blog posts and articles
- **News Websites**: Rich article metadata with journalist attribution
- **E-commerce Sites**: Product pages with enhanced social sharing
- **Documentation**: Technical content with proper categorization
- **Corporate Websites**: Company pages with brand optimization
- **Portfolio Sites**: Creative work with visual emphasis
- **Educational Platforms**: Course and lesson content optimization

## Troubleshooting

### Missing Meta Tags
Ensure ContentItem has required properties:
```csharp
// Check if content has necessary SEO data
if (content?.Metadata?.SeoProperties == null)
{
    content.Metadata ??= new ContentMetadata();
    content.Metadata.SeoProperties = new SeoProperties
    {
        Title = content.Title,
        Description = content.Description
    };
}
```

### Image URL Issues
Verify image URLs are accessible:
```csharp
// Test image URL normalization
var imageUrl = "/images/hero.jpg";
var normalizedUrl = $"{NavigationManager.BaseUri.TrimEnd('/')}/{imageUrl.TrimStart('/')}";
```

### JSON-LD Validation
Use Google's Structured Data Testing Tool to validate generated markup.

The SeoMetadataRenderer component provides enterprise-grade SEO capabilities that automatically optimize your Blazor CMS content for search engines, social media platforms, and modern web discovery systems.
