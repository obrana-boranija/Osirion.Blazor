---
title: "Article Metadata - Osirion Blazor"
description: "Display article metadata including author, publish date, and read time with customizable formatting for Blazor applications."
category: "Core Components"
subcategory: "Content"
tags: ["metadata", "article", "author", "date", "read-time", "blog"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "article-metadata"
section: "components"
layout: "component"
seo:
  title: "Article Metadata Component | Osirion Blazor Documentation"
  description: "Learn how to display article metadata with author, publish date, and read time using OsirionArticleMetdata in Blazor applications."
  keywords: ["Blazor", "article metadata", "author", "publish date", "read time", "blog components"]
  canonical: "/docs/components/core/content/article-metadata"
  image: "/images/components/article-metadata-preview.jpg"
navigation:
  parent: "Core Components"
  order: 32
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "Core"
    link: "/docs/components/core"
  - text: "Content"
    link: "/docs/components/core/content"
  - text: "Article Metadata"
    link: "/docs/components/core/content/article-metadata"
---

# Article Metadata Component

The **OsirionArticleMetdata** component displays article metadata including author information, publish dates, and estimated read times. It's perfect for blog posts, articles, documentation, and any content that benefits from contextual information display.

## Overview

This component provides a clean, consistent way to display article metadata with built-in icons and flexible formatting options. It automatically handles conditional rendering, only displaying metadata when values are provided, and formats dates according to cultural preferences.

## Key Features

- **Conditional Rendering**: Only displays metadata when values are provided
- **Flexible Date Formatting**: Customizable date display formats with culture support
- **Built-in Icons**: SVG icons for visual enhancement
- **Responsive Design**: Mobile-friendly layout
- **Accessibility Support**: Proper semantic structure and ARIA support
- **Theme Compatible**: Works with light and dark themes
- **Performance Optimized**: Lightweight with minimal rendering overhead
- **Globalization Ready**: Supports culture-specific date formatting

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Author` | `string?` | `null` | The author name to display |
| `PublishDate` | `DateTime?` | `null` | The publication date |
| `DateFormat` | `string?` | `"d"` | Date format string (uses .NET date formatting) |
| `ReadTime` | `string?` | `null` | Estimated read time (automatically appends "min") |

## Basic Usage

### Simple Metadata Display

```razor
@using Osirion.Blazor.Components

<OsirionArticleMetdata Author="John Doe"
                       PublishDate="@DateTime.Parse("2025-01-26")"
                       ReadTime="5" />
```

### With Custom Date Format

```razor
<OsirionArticleMetdata Author="Jane Smith"
                       PublishDate="@publishDate"
                       DateFormat="MMMM dd, yyyy"
                       ReadTime="8" />

@code {
    private DateTime publishDate = new DateTime(2025, 1, 26);
}
```

## Advanced Examples

### Blog Post Header

```razor
<article class="blog-post">
    <header class="blog-header">
        <h1 class="blog-title">Getting Started with Blazor</h1>
        <OsirionArticleMetdata Author="@post.Author"
                               PublishDate="@post.PublishDate"
                               ReadTime="@post.EstimatedReadTime"
                               DateFormat="MMM dd, yyyy" />
    </header>
    
    <div class="blog-content">
        @* Article content *@
    </div>
</article>

@code {
    private BlogPost post = new()
    {
        Author = "Osirion Team",
        PublishDate = DateTime.Now.AddDays(-7),
        EstimatedReadTime = "12"
    };
    
    public class BlogPost
    {
        public string Author { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public string EstimatedReadTime { get; set; } = string.Empty;
    }
}
```

### Dynamic Metadata Loading

```razor
@if (article != null)
{
    <div class="article-wrapper">
        <h1>@article.Title</h1>
        
        <OsirionArticleMetdata Author="@article.Author"
                               PublishDate="@article.PublishDate"
                               ReadTime="@CalculateReadTime(article.Content)"
                               DateFormat="@GetDateFormat()" />
        
        <div class="article-content">
            @((MarkupString)article.Content)
        </div>
    </div>
}

@code {
    [Parameter] public string? ArticleId { get; set; }
    
    private Article? article;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(ArticleId))
        {
            article = await ArticleService.GetByIdAsync(ArticleId);
        }
    }
    
    private string CalculateReadTime(string content)
    {
        var wordCount = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var readTime = Math.Ceiling(wordCount / 200.0); // 200 words per minute
        return readTime.ToString();
    }
    
    private string GetDateFormat()
    {
        return CultureInfo.CurrentCulture.Name.StartsWith("en") ? "MMM dd, yyyy" : "dd.MM.yyyy";
    }
}
```

### Multiple Articles List

```razor
<div class="articles-list">
    @foreach (var article in articles)
    {
        <div class="article-card">
            <h3><a href="/articles/@article.Slug">@article.Title</a></h3>
            <p class="article-excerpt">@article.Excerpt</p>
            
            <OsirionArticleMetdata Author="@article.Author"
                                   PublishDate="@article.PublishDate"
                                   ReadTime="@article.ReadTime"
                                   DateFormat="MMM dd" />
        </div>
    }
</div>

@code {
    private List<ArticleSummary> articles = new()
    {
        new() { Title = "Blazor Fundamentals", Author = "John Doe", PublishDate = DateTime.Now.AddDays(-1), ReadTime = "7" },
        new() { Title = "Advanced Components", Author = "Jane Smith", PublishDate = DateTime.Now.AddDays(-3), ReadTime = "12" },
        new() { Title = "Performance Tips", Author = "Bob Wilson", PublishDate = DateTime.Now.AddDays(-7), ReadTime = "5" }
    };
    
    public class ArticleSummary
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
        public string ReadTime { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
    }
}
```

### Conditional Metadata Display

```razor
<div class="article-meta-section">
    @if (showFullMetadata)
    {
        <OsirionArticleMetdata Author="@currentArticle.Author"
                               PublishDate="@currentArticle.PublishDate"
                               ReadTime="@currentArticle.ReadTime"
                               DateFormat="dddd, MMMM dd, yyyy" />
    }
    else
    {
        <OsirionArticleMetdata PublishDate="@currentArticle.PublishDate"
                               DateFormat="MMM yyyy" />
    }
    
    <button class="btn btn-sm btn-outline-secondary mt-2" 
            @onclick="ToggleMetadataView">
        @(showFullMetadata ? "Hide Details" : "Show Details")
    </button>
</div>

@code {
    private bool showFullMetadata = false;
    private Article currentArticle = new()
    {
        Author = "Tech Writer",
        PublishDate = DateTime.Now.AddDays(-5),
        ReadTime = "9"
    };
    
    private void ToggleMetadataView()
    {
        showFullMetadata = !showFullMetadata;
    }
}
```

### Localized Date Formats

```razor
<div class="language-selector mb-3">
    <label for="cultureSelect">Language:</label>
    <select id="cultureSelect" class="form-select" @onchange="ChangeCulture">
        <option value="en-US">English (US)</option>
        <option value="en-GB">English (UK)</option>
        <option value="de-DE">German</option>
        <option value="fr-FR">French</option>
        <option value="es-ES">Spanish</option>
    </select>
</div>

<OsirionArticleMetdata Author="@article.Author"
                       PublishDate="@article.PublishDate"
                       ReadTime="@article.ReadTime"
                       DateFormat="@GetLocalizedDateFormat()" />

@code {
    private string currentCulture = "en-US";
    private Article article = new()
    {
        Author = "International Author",
        PublishDate = new DateTime(2025, 1, 26),
        ReadTime = "6"
    };
    
    private void ChangeCulture(ChangeEventArgs e)
    {
        currentCulture = e.Value?.ToString() ?? "en-US";
        CultureInfo.CurrentCulture = new CultureInfo(currentCulture);
    }
    
    private string GetLocalizedDateFormat()
    {
        return currentCulture switch
        {
            "en-US" => "MMM dd, yyyy",
            "en-GB" => "dd MMM yyyy",
            "de-DE" => "dd.MM.yyyy",
            "fr-FR" => "dd/MM/yyyy",
            "es-ES" => "dd/MM/yyyy",
            _ => "d"
        };
    }
}
```

### Integration with CMS

```razor
@if (cmsArticle != null)
{
    <div class="cms-article">
        <h1>@cmsArticle.Title</h1>
        
        <OsirionArticleMetdata Author="@GetAuthorName(cmsArticle.AuthorId)"
                               PublishDate="@cmsArticle.PublishDate"
                               ReadTime="@cmsArticle.EstimatedReadTime?.ToString()"
                               DateFormat="@siteSettings.DateFormat" />
        
        <div class="article-tags">
            @foreach (var tag in cmsArticle.Tags)
            {
                <span class="badge bg-secondary me-1">@tag</span>
            }
        </div>
        
        <div class="article-content mt-4">
            @((MarkupString)cmsArticle.Content)
        </div>
    </div>
}

@code {
    [Parameter] public string? ArticleSlug { get; set; }
    
    private CmsArticle? cmsArticle;
    private SiteSettings siteSettings = new() { DateFormat = "MMM dd, yyyy" };
    private Dictionary<string, string> authors = new();
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(ArticleSlug))
        {
            cmsArticle = await CmsService.GetArticleBySlugAsync(ArticleSlug);
            authors = await CmsService.GetAuthorsAsync();
        }
    }
    
    private string GetAuthorName(string authorId)
    {
        return authors.TryGetValue(authorId, out var name) ? name : "Unknown Author";
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-article-metadata {
    /* Main container */
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    align-items: center;
    color: var(--bs-text-muted);
    font-size: 0.875rem;
}

.osirion-article-metadata-author,
.osirion-article-metadata-date,
.osirion-article-metadata-read-time {
    /* Individual metadata items */
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

.osirion-article-metadata-icon {
    /* SVG icons */
    flex-shrink: 0;
}
```

### Custom Styling Examples

```css
/* Vertical layout for mobile */
@media (max-width: 576px) {
    .osirion-article-metadata {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
    }
}

/* Custom color scheme */
.article-header .osirion-article-metadata {
    color: var(--bs-primary);
    font-weight: 500;
}

/* Bordered layout */
.bordered-metadata .osirion-article-metadata {
    border: 1px solid var(--bs-border-color);
    border-radius: 0.375rem;
    padding: 0.75rem;
    background-color: var(--bs-light);
}

/* Icon styling */
.osirion-article-metadata-icon {
    color: var(--bs-primary);
    stroke-width: 1.5;
}
```

## Date Format Options

The component supports standard .NET date formatting strings:

| Format | Example Output | Description |
|--------|---------------|-------------|
| `"d"` | 1/26/2025 | Short date pattern |
| `"D"` | Sunday, January 26, 2025 | Long date pattern |
| `"MMM dd, yyyy"` | Jan 26, 2025 | Custom format |
| `"MMMM dd, yyyy"` | January 26, 2025 | Full month name |
| `"dd.MM.yyyy"` | 26.01.2025 | European format |
| `"yyyy-MM-dd"` | 2025-01-26 | ISO format |

### Culture-Specific Formatting

```razor
<OsirionArticleMetdata PublishDate="@DateTime.Now"
                       DateFormat="@GetCultureSpecificFormat()" />

@code {
    private string GetCultureSpecificFormat()
    {
        var culture = CultureInfo.CurrentCulture;
        
        return culture.Name switch
        {
            "en-US" => "MMM dd, yyyy",
            "en-GB" => "dd MMM yyyy",
            "de-DE" => "dd. MMMM yyyy",
            "fr-FR" => "dd MMMM yyyy",
            _ => culture.DateTimeFormat.ShortDatePattern
        };
    }
}
```

## Accessibility Features

- **Semantic Structure**: Uses appropriate HTML elements for metadata
- **Screen Reader Support**: Icons are decorative and don't interfere with content
- **Color Independence**: Information is not conveyed through color alone
- **Focus Management**: Focusable elements have proper indicators
- **ARIA Support**: Proper labels and roles for assistive technologies

### Accessibility Enhancement

```razor
<div role="contentinfo" aria-label="Article information">
    <OsirionArticleMetdata Author="@article.Author"
                           PublishDate="@article.PublishDate"
                           ReadTime="@article.ReadTime" />
</div>
```

## Performance Considerations

### Efficient Rendering

```razor
@* Only render when metadata is available *@
@if (HasMetadata)
{
    <OsirionArticleMetdata Author="@cachedAuthor"
                           PublishDate="@cachedDate"
                           ReadTime="@cachedReadTime" />
}

@code {
    private string? cachedAuthor;
    private DateTime? cachedDate;
    private string? cachedReadTime;
    
    private bool HasMetadata => 
        !string.IsNullOrEmpty(cachedAuthor) || 
        cachedDate.HasValue || 
        !string.IsNullOrEmpty(cachedReadTime);
}
```

### Memory Optimization

```razor
@implements IDisposable

<OsirionArticleMetdata Author="@article?.Author"
                       PublishDate="@article?.PublishDate"
                       ReadTime="@GetReadTime()" />

@code {
    private Timer? readTimeCalculationTimer;
    
    private string? GetReadTime()
    {
        // Cache expensive calculations
        return readTimeCache ??= CalculateReadTime();
    }
    
    public void Dispose()
    {
        readTimeCalculationTimer?.Dispose();
    }
}
```

## Integration Patterns

### With State Management

```razor
@using Fluxor
@inherits FluxorComponent

<OsirionArticleMetdata Author="@ArticleState.Value.CurrentArticle?.Author"
                       PublishDate="@ArticleState.Value.CurrentArticle?.PublishDate"
                       ReadTime="@ArticleState.Value.CurrentArticle?.ReadTime" />

@code {
    [Inject] private IState<ArticleState> ArticleState { get; set; } = null!;
}
```

### With Validation

```razor
<EditForm Model="@articleModel" OnValidSubmit="SaveArticle">
    <DataAnnotationsValidator />
    
    <div class="mb-3">
        <label class="form-label">Author</label>
        <InputText @bind-Value="articleModel.Author" class="form-control" />
        <ValidationMessage For="@(() => articleModel.Author)" />
    </div>
    
    <div class="mb-3">
        <label class="form-label">Publish Date</label>
        <InputDate @bind-Value="articleModel.PublishDate" class="form-control" />
    </div>
    
    <div class="preview-section">
        <h6>Preview:</h6>
        <OsirionArticleMetdata Author="@articleModel.Author"
                               PublishDate="@articleModel.PublishDate"
                               ReadTime="@articleModel.EstimatedReadTime" />
    </div>
</EditForm>
```

## Common Use Cases

- **Blog Posts**: Display author and publication information
- **News Articles**: Show journalist bylines and publication dates
- **Documentation**: Display last updated dates and contributors
- **Knowledge Base**: Show article metadata for better organization
- **Content Management**: Preview metadata during content creation
- **Archive Pages**: Display publication dates for chronological organization

## Best Practices

1. **Consistent Formatting**: Use consistent date formats across your application
2. **Performance**: Cache calculated values like read time when possible
3. **Accessibility**: Ensure metadata is accessible to screen readers
4. **Responsive Design**: Test metadata display on various screen sizes
5. **Localization**: Support multiple cultures and languages
6. **SEO**: Include structured data for better search engine understanding
7. **User Experience**: Make metadata scannable and easy to read
8. **Data Validation**: Validate dates and format strings before rendering

The OsirionArticleMetdata component provides a professional and flexible way to display article metadata, enhancing the user experience and providing valuable context for your content.
