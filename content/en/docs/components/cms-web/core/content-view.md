---
title: "Content View - Osirion Blazor CMS"
description: "Complete content display component with metadata, navigation links, and full article rendering for Blazor CMS applications."
category: "CMS Web Components"
subcategory: "Core"
tags: ["cms", "content", "view", "metadata", "article", "navigation"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "content-view"
section: "components"
layout: "component"
seo:
  title: "Content View Component | Osirion Blazor CMS Documentation"
  description: "Learn how to display complete content items with metadata, navigation, and rich formatting using ContentView component."
  keywords: ["Blazor", "CMS", "content view", "article display", "metadata", "navigation", "content rendering"]
  canonical: "/docs/components/cms.web/core/content-view"
  image: "/images/components/content-view-preview.jpg"
navigation:
  parent: "CMS Web Components"
  order: 4
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Core"
    link: "/docs/components/cms.web/core"
  - text: "Content View"
    link: "/docs/components/cms.web/core/content-view"
---

# Content View Component

The **ContentView** component provides a comprehensive solution for displaying individual content items with rich metadata, featured images, category/tag navigation, and optional previous/next navigation. It's designed for full article displays, blog posts, and detailed content pages.

## Overview

This component combines content rendering with rich metadata display, automatic navigation link generation, and responsive design. It handles loading states, error conditions, and provides extensive customization options for URL formatting and display preferences.

## Key Features

- **Complete Article Display**: Full content rendering with title, metadata, and body
- **Rich Metadata**: Author, date, read time, categories, and tags display
- **Featured Image Support**: Automatic featured image display with proper lazy loading
- **Navigation Links**: Optional previous/next content navigation
- **Responsive Design**: Mobile-first layout that adapts to all screen sizes
- **SEO Optimized**: Structured markup for better search engine understanding
- **Loading States**: Professional loading and error state handling
- **Customizable URLs**: Flexible URL formatting for categories, tags, and content
- **Performance Optimized**: Efficient loading and rendering strategies

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Path` | `string` | `string.Empty` | Content path to load and display |
| `LoadingText` | `string` | `"Loading content..."` | Text shown during content loading |
| `NotFoundText` | `string` | `"Content not found."` | Text shown when content is not found |
| `CategoryUrlFormatter` | `Func<string, string>?` | `null` | Custom category URL formatting function |
| `TagUrlFormatter` | `Func<string, string>?` | `null` | Custom tag URL formatting function |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Custom content URL formatting function |
| `Item` | `ContentItem?` | `null` | Pre-loaded content item to display |
| `PreviousItem` | `ContentItem?` | `null` | Previous content item for navigation |
| `NextItem` | `ContentItem?` | `null` | Next content item for navigation |
| `ShowNavigationLinks` | `bool` | `false` | Enable previous/next navigation links |

## Basic Usage

### Simple Content Display

```razor
@using Osirion.Blazor.Cms.Web.Components

<ContentView Path="/articles/getting-started-with-blazor" />
```

### Pre-loaded Content

```razor
<ContentView Item="@currentArticle"
             ShowNavigationLinks="true" />

@code {
    private ContentItem? currentArticle;
    
    protected override async Task OnInitializedAsync()
    {
        currentArticle = await ContentService.GetBySlugAsync("my-article");
    }
}
```

## Advanced Examples

### Blog Post with Custom URLs

```razor
<ContentView Path="@articlePath"
             CategoryUrlFormatter="@FormatCategoryUrl"
             TagUrlFormatter="@FormatTagUrl"
             ContentUrlFormatter="@FormatContentUrl"
             ShowNavigationLinks="true" />

@code {
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private string articlePath => $"/blog/{Slug}";
    
    private string FormatCategoryUrl(string category)
    {
        return $"/blog/category/{category.ToLower().Replace(' ', '-')}";
    }
    
    private string FormatTagUrl(string tag)
    {
        return $"/blog/tag/{tag.ToLower().Replace(' ', '-')}";
    }
    
    private string FormatContentUrl(ContentItem item)
    {
        return $"/blog/{item.Slug}";
    }
}
```

### Documentation Page

```razor
<div class="documentation-layout">
    <nav class="doc-sidebar">
        <DirectoryNavigation Path="/docs" />
    </nav>
    
    <main class="doc-content">
        <ContentView Path="@docPath"
                     CategoryUrlFormatter="@FormatDocCategoryUrl"
                     ShowNavigationLinks="true"
                     LoadingText="Loading documentation..."
                     NotFoundText="Documentation page not found. Please check the URL or browse our documentation index." />
    </main>
</div>

@code {
    [Parameter] public string DocPath { get; set; } = string.Empty;
    
    private string docPath => $"/docs/{DocPath}";
    
    private string FormatDocCategoryUrl(string category)
    {
        return $"/docs/category/{category}";
    }
}
```

### News Article with Custom Layout

```razor
<div class="news-article">
    <ContentView Item="@newsArticle"
                 CategoryUrlFormatter="@FormatNewsCategory"
                 TagUrlFormatter="@FormatNewsTag"
                 CssClass="news-content" />
    
    @if (relatedArticles?.Any() == true)
    {
        <aside class="related-news">
            <h3>Related News</h3>
            <div class="related-grid">
                @foreach (var related in relatedArticles)
                {
                    <div class="related-item">
                        <h4><a href="/news/@related.Slug">@related.Title</a></h4>
                        <p>@related.Description</p>
                    </div>
                }
            </div>
        </aside>
    }
</div>

@code {
    [Parameter] public string NewsSlug { get; set; } = string.Empty;
    
    private ContentItem? newsArticle;
    private List<ContentItem>? relatedArticles;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(NewsSlug))
        {
            newsArticle = await NewsService.GetBySlugAsync(NewsSlug);
            if (newsArticle != null)
            {
                relatedArticles = await NewsService.GetRelatedAsync(newsArticle.Id, 3);
            }
        }
    }
    
    private string FormatNewsCategory(string category)
    {
        return $"/news/category/{category}";
    }
    
    private string FormatNewsTag(string tag)
    {
        return $"/news/tag/{tag}";
    }
}
```

### Product Documentation

```razor
<div class="product-docs">
    <header class="product-header">
        <h1>@productName Documentation</h1>
        <div class="version-selector">
            <select @onchange="HandleVersionChange">
                @foreach (var version in availableVersions)
                {
                    <option value="@version" selected="@(version == currentVersion)">
                        Version @version
                    </option>
                }
            </select>
        </div>
    </header>
    
    <ContentView Path="@GetVersionedPath()"
                 ShowNavigationLinks="true"
                 CategoryUrlFormatter="@FormatProductCategory" />
</div>

@code {
    [Parameter] public string ProductName { get; set; } = string.Empty;
    [Parameter] public string DocPath { get; set; } = string.Empty;
    
    private string currentVersion = "latest";
    private List<string> availableVersions = new() { "latest", "v2.0", "v1.9", "v1.8" };
    
    private string GetVersionedPath()
    {
        return $"/docs/{ProductName}/{currentVersion}/{DocPath}";
    }
    
    private void HandleVersionChange(ChangeEventArgs e)
    {
        currentVersion = e.Value?.ToString() ?? "latest";
    }
    
    private string FormatProductCategory(string category)
    {
        return $"/docs/{ProductName}/{currentVersion}/category/{category}";
    }
}
```

### Multi-Author Blog

```razor
<div class="multi-author-blog">
    <ContentView Item="@blogPost"
                 CategoryUrlFormatter="@FormatBlogCategory"
                 TagUrlFormatter="@FormatBlogTag"
                 ShowNavigationLinks="true" />
    
    @if (!string.IsNullOrEmpty(blogPost?.Author))
    {
        <section class="author-info">
            <h3>About the Author</h3>
            <div class="author-card">
                <img src="@GetAuthorAvatar(blogPost.Author)" alt="@blogPost.Author" class="author-avatar" />
                <div class="author-details">
                    <h4>@blogPost.Author</h4>
                    <p>@GetAuthorBio(blogPost.Author)</p>
                    <a href="/authors/@GetAuthorSlug(blogPost.Author)" class="author-link">
                        View all posts by @blogPost.Author
                    </a>
                </div>
            </div>
        </section>
    }
</div>

@code {
    [Parameter] public ContentItem? BlogPost { get; set; }
    
    private ContentItem? blogPost => BlogPost;
    
    private string FormatBlogCategory(string category)
    {
        return $"/blog/category/{category.ToLower().Replace(' ', '-')}";
    }
    
    private string FormatBlogTag(string tag)
    {
        return $"/blog/tag/{tag.ToLower().Replace(' ', '-')}";
    }
    
    private string GetAuthorAvatar(string author)
    {
        return $"/images/authors/{GetAuthorSlug(author)}.jpg";
    }
    
    private string GetAuthorSlug(string author)
    {
        return author.ToLower().Replace(' ', '-').Replace(".", "");
    }
    
    private string GetAuthorBio(string author)
    {
        // In a real app, this would come from a database or service
        return $"{author} is a software developer and technical writer.";
    }
}
```

### E-learning Course Content

```razor
<div class="course-content">
    <div class="course-progress">
        <div class="progress">
            <div class="progress-bar" style="width: @(progress)%"></div>
        </div>
        <span>@progress% Complete</span>
    </div>
    
    <ContentView Item="@currentLesson"
                 CategoryUrlFormatter="@FormatCourseCategory"
                 ShowNavigationLinks="true"
                 PreviousItem="@previousLesson"
                 NextItem="@nextLesson" />
    
    <div class="lesson-actions">
        @if (previousLesson != null)
        {
            <button class="btn btn-outline-primary" @onclick="NavigateToPrevious">
                ← Previous Lesson
            </button>
        }
        
        <button class="btn btn-primary" @onclick="MarkComplete" disabled="@isCompleted">
            @(isCompleted ? "✓ Completed" : "Mark as Complete")
        </button>
        
        @if (nextLesson != null)
        {
            <button class="btn btn-primary" @onclick="NavigateToNext">
                Next Lesson →
            </button>
        }
    </div>
</div>

@code {
    [Parameter] public string CourseId { get; set; } = string.Empty;
    [Parameter] public string LessonId { get; set; } = string.Empty;
    
    private ContentItem? currentLesson;
    private ContentItem? previousLesson;
    private ContentItem? nextLesson;
    private int progress = 0;
    private bool isCompleted = false;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(CourseId) && !string.IsNullOrEmpty(LessonId))
        {
            currentLesson = await CourseService.GetLessonAsync(CourseId, LessonId);
            var navigation = await CourseService.GetLessonNavigationAsync(CourseId, LessonId);
            previousLesson = navigation.Previous;
            nextLesson = navigation.Next;
            
            progress = await CourseService.GetProgressAsync(CourseId);
            isCompleted = await CourseService.IsLessonCompletedAsync(CourseId, LessonId);
        }
    }
    
    private string FormatCourseCategory(string category)
    {
        return $"/courses/{CourseId}/category/{category}";
    }
    
    private async Task MarkComplete()
    {
        await CourseService.MarkLessonCompleteAsync(CourseId, LessonId);
        isCompleted = true;
        progress = await CourseService.GetProgressAsync(CourseId);
    }
    
    private void NavigateToPrevious()
    {
        if (previousLesson != null)
        {
            Navigation.NavigateTo($"/courses/{CourseId}/lessons/{previousLesson.Slug}");
        }
    }
    
    private void NavigateToNext()
    {
        if (nextLesson != null)
        {
            Navigation.NavigateTo($"/courses/{CourseId}/lessons/{nextLesson.Slug}");
        }
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-content-view {
    /* Main container */
}

.osirion-content-loading {
    /* Loading state */
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 2rem;
}

.osirion-content-not-found {
    /* Not found state */
    text-align: center;
    padding: 3rem 1rem;
    color: var(--bs-text-muted);
}

.osirion-content-article {
    /* Article container */
    max-width: 800px;
    margin: 0 auto;
}

.osirion-content-header {
    /* Article header */
    margin-bottom: 2rem;
}

.osirion-content-title {
    /* Article title */
    font-size: 2.5rem;
    font-weight: 700;
    line-height: 1.2;
    margin-bottom: 1rem;
}

.osirion-content-meta {
    /* Metadata container */
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    align-items: center;
    color: var(--bs-text-muted);
    font-size: 0.875rem;
    margin-bottom: 1.5rem;
}

.osirion-content-meta-item {
    /* Individual meta items */
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

.osirion-content-featured-image-container {
    /* Featured image wrapper */
    margin: 2rem 0;
}

.osirion-content-featured-image {
    /* Featured image */
    width: 100%;
    height: auto;
    border-radius: 0.5rem;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.osirion-content-body {
    /* Article body */
    line-height: 1.6;
    font-size: 1.1rem;
}

.osirion-content-footer {
    /* Article footer */
    margin-top: 3rem;
    padding-top: 2rem;
    border-top: 1px solid var(--bs-border-color);
}

.osirion-content-tags {
    /* Tags container */
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
}

.osirion-content-tag {
    /* Individual tag */
    display: inline-block;
    padding: 0.25rem 0.75rem;
    background: var(--bs-light);
    color: var(--bs-dark);
    text-decoration: none;
    border-radius: 1rem;
    font-size: 0.875rem;
    transition: background-color 0.2s ease;
}

.osirion-content-tag:hover {
    background: var(--bs-primary);
    color: white;
}
```

### Responsive Design

```css
/* Mobile responsiveness */
@media (max-width: 768px) {
    .osirion-content-title {
        font-size: 2rem;
    }
    
    .osirion-content-meta {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
    }
    
    .osirion-content-body {
        font-size: 1rem;
    }
    
    .osirion-content-tags-inline {
        display: none; /* Hide inline tags on mobile */
    }
}

@media (max-width: 576px) {
    .osirion-content-article {
        padding: 0 1rem;
    }
    
    .osirion-content-title {
        font-size: 1.75rem;
    }
}
```

### Custom Styling Examples

```css
/* Modern article design */
.modern-article .osirion-content-article {
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    padding: 3rem;
}

/* Dark theme support */
@media (prefers-color-scheme: dark) {
    .osirion-content-article {
        background: var(--bs-dark);
        color: var(--bs-light);
    }
    
    .osirion-content-tag {
        background: var(--bs-secondary);
        color: var(--bs-light);
    }
}

/* Print styles */
@media print {
    .osirion-content-meta,
    .osirion-content-tags {
        display: none;
    }
    
    .osirion-content-featured-image {
        max-height: 300px;
        object-fit: cover;
    }
}
```

## SEO Integration

### Meta Tags Setup

```razor
<ContentView Item="@article" @oncontentloaded="HandleContentLoaded" />

@code {
    private async Task HandleContentLoaded(ContentItem content)
    {
        await JSRuntime.InvokeVoidAsync("updatePageMeta", new
        {
            title = content.Title,
            description = content.Description,
            author = content.Author,
            publishedTime = content.DateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            image = content.FeaturedImageUrl,
            keywords = string.Join(", ", content.Tags)
        });
    }
}
```

### Structured Data

```razor
<ContentView Item="@article" />

@if (article != null)
{
    <script type="application/ld+json">
    {
        "@context": "https://schema.org",
        "@type": "Article",
        "headline": "@article.Title",
        "description": "@article.Description",
        "author": {
            "@type": "Person",
            "name": "@article.Author"
        },
        "datePublished": "@article.DateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ")",
        "image": "@article.FeaturedImageUrl"
    }
    </script>
}
```

## Performance Optimization

### Lazy Loading

```razor
<ContentView Path="@articlePath" @ref="contentViewRef" />

@code {
    private ContentView? contentViewRef;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("observeImages", ".osirion-content-featured-image");
        }
    }
}
```

### Caching Strategy

```razor
@implements IDisposable

<ContentView Item="@cachedContent" />

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

## Common Use Cases

- **Blog Posts**: Full blog article display with metadata and navigation
- **Documentation**: Technical documentation with navigation links
- **News Articles**: News content with category and tag navigation
- **Product Documentation**: Versioned documentation with structured navigation
- **Course Content**: E-learning lessons with progress tracking
- **Portfolio Items**: Creative work showcases with detailed descriptions
- **Knowledge Base**: Help articles with related content suggestions

## Best Practices

1. **URL Formatting**: Implement consistent URL formatting functions
2. **SEO**: Include proper meta tags and structured data
3. **Performance**: Use lazy loading for images and content
4. **Accessibility**: Ensure proper heading structure and alt text
5. **Mobile**: Test responsive behavior across devices
6. **Navigation**: Provide clear previous/next navigation when appropriate
7. **Error Handling**: Implement graceful error states
8. **Caching**: Cache content appropriately for better performance

The ContentView component provides a comprehensive solution for displaying rich content with all the metadata and navigation features needed for professional content websites.
