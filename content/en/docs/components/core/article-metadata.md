---
id: 'article-metadata'
order: 2
layout: docs
title: OsirionArticleMetadata Component
permalink: /docs/components/core/article-metadata
description: Documentation for the OsirionArticleMetadata component - displays article metadata including author, publication date, and read time with customizable formatting and icons.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Content
tags:
- article-metadata
- content
- blazor
- components
is_featured: false
published: true
slug: components/core/article-metadata
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionArticleMetadata Component - Osirion.Blazor Documentation'
  description: 'Complete guide to the OsirionArticleMetadata component for displaying article metadata with author, date, and read time information.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/article-metadata'
  lang: en
  robots: index, follow
  og_title: 'OsirionArticleMetadata Component - Osirion.Blazor'
  og_description: 'Documentation for displaying article metadata with the OsirionArticleMetadata component.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionArticleMetadata Component - Osirion.Blazor'
  twitter_description: 'Documentation for displaying article metadata with the OsirionArticleMetadata component.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionArticleMetadata Component

The `OsirionArticleMetadata` component displays essential article metadata including author information, publication date, and estimated read time. It provides a consistent way to present content metadata across your application with built-in icons and flexible formatting options.

## Overview

The OsirionArticleMetadata component is designed for blog posts, articles, documentation pages, and any content where authorship and timing information is relevant. It automatically handles the display logic, showing only the metadata fields that contain data.

**Key Features:**
- Author name display with user icon
- Publication date with customizable formatting
- Read time estimation display
- Conditional rendering (only shows when data is present)
- Built-in SVG icons
- Responsive design
- Accessibility compliant
- Culturally-aware date formatting

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Author` | `string?` | `null` | The author name to display |
| `PublishDate` | `DateTime?` | `null` | The publication date |
| `DateFormat` | `string?` | `"d"` | Date format string (uses .NET format patterns) |
| `ReadTime` | `string?` | `null` | Estimated read time (without "min" suffix) |

## Basic Usage

### Simple Article Metadata

```razor
<OsirionArticleMetadata 
    Author="John Doe"
    PublishDate="@DateTime.Now"
    ReadTime="5" />
```

### With Custom Date Format

```razor
<OsirionArticleMetadata 
    Author="Jane Smith"
    PublishDate="@DateTime.Parse("2025-03-15")"
    DateFormat="MMMM dd, yyyy"
    ReadTime="3" />
```

### Partial Metadata (Only Author)

```razor
<OsirionArticleMetadata 
    Author="Documentation Team" />
```

### From Content Model

```razor
<OsirionArticleMetadata 
    Author="@article.Author"
    PublishDate="@article.PublishDate"
    ReadTime="@article.ReadTime" />
```

## Date Formatting

The component uses .NET's standard date format strings with culture-aware formatting:

### Common Date Formats

```razor
<!-- Short date pattern (default) -->
<OsirionArticleMetadata 
    PublishDate="@DateTime.Now" 
    DateFormat="d" />
<!-- Output: 3/15/2025 (US), 15/03/2025 (UK) -->

<!-- Long date pattern -->
<OsirionArticleMetadata 
    PublishDate="@DateTime.Now" 
    DateFormat="D" />
<!-- Output: Sunday, March 15, 2025 -->

<!-- Custom format -->
<OsirionArticleMetadata 
    PublishDate="@DateTime.Now" 
    DateFormat="MMM dd, yyyy" />
<!-- Output: Mar 15, 2025 -->

<!-- ISO format -->
<OsirionArticleMetadata 
    PublishDate="@DateTime.Now" 
    DateFormat="yyyy-MM-dd" />
<!-- Output: 2025-03-15 -->
```

### Localized Formatting

```razor
<!-- Respects current culture -->
<OsirionArticleMetadata 
    PublishDate="@DateTime.Now" 
    DateFormat="f" />
<!-- Output varies by culture setting -->
```

## Integration Examples

### With Blog Post Model

```razor
@code {
    public class BlogPost
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public string Content { get; set; }
        public int WordCount { get; set; }
        
        public string ReadTime => $"{Math.Ceiling(WordCount / 200.0)}";
    }
    
    private BlogPost post = new BlogPost
    {
        Title = "Getting Started with Blazor",
        Author = "Development Team",
        PublishDate = DateTime.Parse("2025-03-15"),
        WordCount = 1500
    };
}

<article>
    <header>
        <h1>@post.Title</h1>
        <OsirionArticleMetadata 
            Author="@post.Author"
            PublishDate="@post.PublishDate"
            ReadTime="@post.ReadTime" />
    </header>
    <!-- Article content -->
</article>
```

### With CMS Content

```razor
@if (content != null)
{
    <article class="content-article">
        <header class="content-header">
            <h1>@content.Title</h1>
            <OsirionArticleMetadata 
                Author="@content.Author"
                PublishDate="@content.DateCreated"
                ReadTime="@CalculateReadTime(content.Content)"
                DateFormat="MMMM dd, yyyy" />
        </header>
        <div class="content-body">
            @((MarkupString)content.RenderedContent)
        </div>
    </article>
}

@code {
    private string CalculateReadTime(string content)
    {
        if (string.IsNullOrEmpty(content)) return "1";
        
        var wordCount = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var readTime = Math.Ceiling(wordCount / 200.0); // 200 words per minute
        return readTime.ToString();
    }
}
```

### In Hero Section

```razor
<HeroSection 
    Title="@article.Title"
    Summary="@article.Summary"
    ShowMetadata="true"
    Author="@article.Author"
    PublishDate="@article.PublishDate"
    ReadTime="@article.ReadTime" />
```

### With Conditional Display

```razor
@if (showMetadata && HasMetadata())
{
    <OsirionArticleMetadata 
        Author="@currentAuthor"
        PublishDate="@currentDate"
        ReadTime="@currentReadTime" />
}

@code {
    private bool HasMetadata()
    {
        return !string.IsNullOrEmpty(currentAuthor) || 
               currentDate.HasValue || 
               !string.IsNullOrEmpty(currentReadTime);
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS classes:

```css
.osirion-article-metadata {
    /* Container for all metadata items */
}

.osirion-article-metadata-author {
    /* Author name with icon */
}

.osirion-article-metadata-date {
    /* Publication date with icon */
}

.osirion-article-metadata-read-time {
    /* Read time with icon */
}

.osirion-article-meta-icon,
.osirion-article-metadata-icon {
    /* SVG icons */
}
```

### Custom Styling

```css
.osirion-article-metadata {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    font-size: 0.875rem;
    color: #6b7280;
    margin: 1rem 0;
}

.osirion-article-metadata > span {
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

.osirion-article-metadata-icon {
    width: 16px;
    height: 16px;
    opacity: 0.7;
}

.osirion-article-metadata-author {
    font-weight: 500;
}

.osirion-article-metadata-date {
    color: #9ca3af;
}

.osirion-article-metadata-read-time {
    color: #9ca3af;
}
```

### Dark Theme Support

```css
[data-theme="dark"] .osirion-article-metadata {
    color: #d1d5db;
}

[data-theme="dark"] .osirion-article-metadata-date,
[data-theme="dark"] .osirion-article-metadata-read-time {
    color: #9ca3af;
}
```

### Custom Layout

```css
/* Vertical layout */
.osirion-article-metadata.vertical {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
}

/* Compact layout */
.osirion-article-metadata.compact {
    gap: 0.75rem;
    font-size: 0.8rem;
}

.osirion-article-metadata.compact .osirion-article-metadata-icon {
    width: 14px;
    height: 14px;
}
```

## Accessibility Features

The component includes comprehensive accessibility support:

- **Semantic HTML**: Uses appropriate span elements with descriptive classes
- **Icon Accessibility**: SVG icons are decorative and properly marked
- **Screen Reader Support**: All text content is accessible to screen readers
- **Color Independence**: Information is not conveyed through color alone
- **Keyboard Navigation**: Component doesn't interfere with keyboard navigation

## Best Practices

### Content Guidelines

```razor
<!-- Good: Clear, concise author names -->
<OsirionArticleMetadata Author="John Smith" />

<!-- Good: Full names for better identification -->
<OsirionArticleMetadata Author="Dr. Sarah Johnson" />

<!-- Avoid: Generic or unclear names -->
<OsirionArticleMetadata Author="Admin" />
```

### Date Consistency

```razor
<!-- Good: Consistent date format across your site -->
<OsirionArticleMetadata 
    PublishDate="@article.Date" 
    DateFormat="MMM dd, yyyy" />

<!-- Good: Use appropriate format for your audience -->
<OsirionArticleMetadata 
    PublishDate="@article.Date" 
    DateFormat="dd/MM/yyyy" /> <!-- For UK audience -->
```

### Read Time Calculation

```csharp
// Good: Realistic read time calculation
private string CalculateReadTime(string content)
{
    if (string.IsNullOrEmpty(content)) return "1";
    
    // Remove HTML tags for accurate word count
    var plainText = Regex.Replace(content, "<.*?>", string.Empty);
    var wordCount = plainText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    
    // 200-250 words per minute is average reading speed
    var readTime = Math.Ceiling(wordCount / 225.0);
    return Math.Max(1, (int)readTime).ToString();
}
```

### Conditional Display

```razor
<!-- Good: Only show when data is available -->
@if (!string.IsNullOrEmpty(article.Author) || 
     article.PublishDate.HasValue || 
     !string.IsNullOrEmpty(article.ReadTime))
{
    <OsirionArticleMetadata 
        Author="@article.Author"
        PublishDate="@article.PublishDate"
        ReadTime="@article.ReadTime" />
}
```

## Performance Considerations

- **Lightweight Rendering**: Component only renders when metadata is available
- **Efficient Date Formatting**: Uses built-in .NET culture-aware formatting
- **Minimal DOM**: Generates minimal HTML structure
- **CSS Optimization**: Uses efficient CSS selectors

## Common Use Cases

### Blog Article Header

```razor
<article class="blog-article">
    <header class="article-header">
        <h1 class="article-title">@Model.Title</h1>
        <p class="article-summary">@Model.Summary</p>
        <OsirionArticleMetadata 
            Author="@Model.Author"
            PublishDate="@Model.PublishDate"
            ReadTime="@Model.ReadTime"
            DateFormat="MMMM dd, yyyy" />
    </header>
    <div class="article-content">
        @((MarkupString)Model.Content)
    </div>
</article>
```

### Documentation Page

```razor
<div class="docs-page">
    <header class="docs-header">
        <h1>@page.Title</h1>
        <OsirionArticleMetadata 
            Author="Documentation Team"
            PublishDate="@page.LastModified"
            DateFormat="MMM dd, yyyy" />
    </header>
    <div class="docs-content">
        @((MarkupString)page.Content)
    </div>
</div>
```

### News Article Card

```razor
<div class="news-card">
    <img src="@article.ImageUrl" alt="@article.Title" class="news-image" />
    <div class="news-content">
        <h3 class="news-title">@article.Title</h3>
        <p class="news-excerpt">@article.Excerpt</p>
        <OsirionArticleMetadata 
            Author="@article.Author"
            PublishDate="@article.PublishDate"
            ReadTime="@article.ReadTime"
            DateFormat="MMM dd" />
    </div>
</div>
```

The OsirionArticleMetadata component provides a consistent and accessible way to display article metadata, enhancing content credibility and user experience across your Blazor application.
