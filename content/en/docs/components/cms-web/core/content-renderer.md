---
title: "Content Renderer - Osirion Blazor CMS"
description: "Simple content rendering wrapper component for displaying CMS content with HTML rendering capabilities."
category: "CMS Web Components"
subcategory: "Core"
tags: ["cms", "content", "renderer", "html", "display"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "content-renderer"
section: "components"
layout: "component"
seo:
  title: "Content Renderer Component | Osirion Blazor CMS Documentation"
  description: "Learn how to render CMS content safely using the ContentRenderer component in Osirion Blazor CMS."
  keywords: ["Blazor", "CMS", "content renderer", "HTML rendering", "content display"]
  canonical: "/docs/components/cms.web/core/content-renderer"
  image: "/images/components/content-renderer-preview.jpg"
navigation:
  parent: "CMS Web Components"
  order: 3
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Core"
    link: "/docs/components/cms.web/core"
  - text: "Content Renderer"
    link: "/docs/components/cms.web/core/content-renderer"
---

# Content Renderer Component

The **ContentRenderer** component provides a lightweight wrapper for rendering CMS content using the OsirionHtmlRenderer. It's designed as a simple, focused component that safely displays HTML content from your CMS with minimal overhead.

## Overview

This component serves as a bridge between your ContentItem objects and the HTML rendering engine. It automatically handles content extraction and passes it to the OsirionHtmlRenderer for safe display with syntax highlighting and other advanced features.

## Key Features

- **Simple Integration**: Minimal setup required to display content
- **Safe HTML Rendering**: Built-in XSS protection through OsirionHtmlRenderer
- **Syntax Highlighting**: Automatic code block highlighting
- **Lightweight**: Minimal performance overhead
- **Flexible Styling**: Customizable CSS classes
- **Content-Aware**: Automatically handles null/empty content states

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Item` | `ContentItem?` | `null` | The content item to render |

## Basic Usage

### Simple Content Display

```razor
@using Osirion.Blazor.Cms.Web.Components

<ContentRenderer Item="@contentItem" />

@code {
    private ContentItem? contentItem;
    
    protected override async Task OnInitializedAsync()
    {
        contentItem = await ContentService.GetByIdAsync("article-123");
    }
}
```

### With Dynamic Loading

```razor
<ContentRenderer Item="@currentContent" />

@code {
    [Parameter] public string? ContentId { get; set; }
    
    private ContentItem? currentContent;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(ContentId))
        {
            currentContent = await ContentService.GetByIdAsync(ContentId);
        }
    }
}
```

## Advanced Examples

### Content Gallery

```razor
<div class="content-gallery">
    @foreach (var item in contentItems)
    {
        <div class="gallery-item">
            <h3>@item.Title</h3>
            <ContentRenderer Item="@item" CssClass="gallery-content" />
        </div>
    }
</div>

@code {
    private List<ContentItem> contentItems = new();
    
    protected override async Task OnInitializedAsync()
    {
        contentItems = await ContentService.GetFeaturedAsync(6);
    }
}
```

### Conditional Rendering

```razor
@if (showPreview)
{
    <div class="content-preview">
        <h4>Preview Mode</h4>
        <ContentRenderer Item="@draftContent" CssClass="preview-content" />
    </div>
}
else
{
    <ContentRenderer Item="@publishedContent" />
}

@code {
    private bool showPreview = false;
    private ContentItem? draftContent;
    private ContentItem? publishedContent;
    
    private void TogglePreview()
    {
        showPreview = !showPreview;
    }
}
```

### Content Comparison

```razor
<div class="content-comparison">
    <div class="comparison-column">
        <h3>Version A</h3>
        <ContentRenderer Item="@versionA" CssClass="version-a" />
    </div>
    
    <div class="comparison-column">
        <h3>Version B</h3>
        <ContentRenderer Item="@versionB" CssClass="version-b" />
    </div>
</div>

@code {
    private ContentItem? versionA;
    private ContentItem? versionB;
    
    protected override async Task OnInitializedAsync()
    {
        versionA = await ContentService.GetVersionAsync(contentId, 1);
        versionB = await ContentService.GetVersionAsync(contentId, 2);
    }
}
```

### Embedded Content

```razor
<div class="main-article">
    <h1>@mainArticle?.Title</h1>
    <ContentRenderer Item="@mainArticle" />
    
    @if (embeddedContent?.Any() == true)
    {
        <div class="embedded-content">
            <h2>Related Information</h2>
            @foreach (var embedded in embeddedContent)
            {
                <div class="embedded-item">
                    <ContentRenderer Item="@embedded" CssClass="embedded" />
                </div>
            }
        </div>
    }
</div>

@code {
    private ContentItem? mainArticle;
    private List<ContentItem>? embeddedContent;
    
    protected override async Task OnInitializedAsync()
    {
        mainArticle = await ContentService.GetBySlugAsync(Slug);
        if (mainArticle != null)
        {
            embeddedContent = await ContentService.GetEmbeddedAsync(mainArticle.Id);
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

.osirion-content-body {
    /* Content body wrapper */
    line-height: 1.6;
    color: var(--bs-body-color);
}

/* Custom styling examples */
.osirion-content-body h1,
.osirion-content-body h2,
.osirion-content-body h3 {
    margin-top: 2rem;
    margin-bottom: 1rem;
    color: var(--bs-heading-color);
}

.osirion-content-body p {
    margin-bottom: 1rem;
}

.osirion-content-body img {
    max-width: 100%;
    height: auto;
    border-radius: 0.375rem;
}
```

### Custom Styling Examples

```css
/* Gallery content styling */
.gallery-content {
    border: 1px solid var(--bs-border-color);
    border-radius: 0.5rem;
    padding: 1rem;
    background: var(--bs-light);
}

/* Preview mode styling */
.preview-content {
    border-left: 4px solid var(--bs-warning);
    padding-left: 1rem;
    background: rgba(255, 193, 7, 0.1);
}

/* Comparison styling */
.version-a {
    border-right: 2px solid var(--bs-primary);
}

.version-b {
    border-left: 2px solid var(--bs-success);
}

/* Embedded content styling */
.embedded {
    font-size: 0.9rem;
    padding: 0.75rem;
    background: var(--bs-gray-50);
    border-radius: 0.25rem;
    margin-bottom: 1rem;
}
```

## Integration Patterns

### With State Management

```razor
@using Fluxor
@inherits FluxorComponent

<ContentRenderer Item="@ContentState.Value.CurrentItem" />

@code {
    [Inject] private IState<ContentState> ContentState { get; set; } = null!;
}
```

### With Error Boundaries

```razor
<ErrorBoundary>
    <ChildContent>
        <ContentRenderer Item="@contentItem" />
    </ChildContent>
    <ErrorContent>
        <div class="alert alert-danger">
            <h4>Content Rendering Error</h4>
            <p>There was an error rendering this content. Please try again later.</p>
        </div>
    </ErrorContent>
</ErrorBoundary>
```

### With Loading States

```razor
@if (isLoading)
{
    <div class="content-skeleton">
        <div class="skeleton-line skeleton-title"></div>
        <div class="skeleton-line"></div>
        <div class="skeleton-line"></div>
        <div class="skeleton-line skeleton-short"></div>
    </div>
}
else
{
    <ContentRenderer Item="@loadedContent" />
}

@code {
    private bool isLoading = true;
    private ContentItem? loadedContent;
    
    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(100); // Simulate loading
        loadedContent = await ContentService.GetAsync(ContentId);
        isLoading = false;
    }
}
```

## Performance Considerations

### Memory Management

```razor
@implements IDisposable

<ContentRenderer Item="@contentItem" />

@code {
    private ContentItem? contentItem;
    private Timer? refreshTimer;
    
    protected override async Task OnInitializedAsync()
    {
        contentItem = await ContentService.GetAsync(Id);
        
        // Optional: Set up content refresh
        refreshTimer = new Timer(async _ => 
        {
            var updated = await ContentService.GetAsync(Id);
            if (updated?.DateModified > contentItem?.DateModified)
            {
                contentItem = updated;
                await InvokeAsync(StateHasChanged);
            }
        }, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    public void Dispose()
    {
        refreshTimer?.Dispose();
    }
}
```

### Virtualization for Large Lists

```razor
<Virtualize Items="@contentItems" Context="item">
    <div class="virtualized-item">
        <ContentRenderer Item="@item" />
    </div>
</Virtualize>

@code {
    private List<ContentItem> contentItems = new();
}
```

## Common Use Cases

### Blog Content Display

```razor
<div class="blog-post">
    <header class="blog-header">
        <h1>@blogPost?.Title</h1>
        <div class="blog-meta">
            <span>By @blogPost?.Author</span>
            <span>@blogPost?.DateCreated.ToString("MMMM dd, yyyy")</span>
        </div>
    </header>
    
    <ContentRenderer Item="@blogPost" CssClass="blog-content" />
</div>
```

### Documentation Rendering

```razor
<div class="documentation">
    <nav class="doc-nav">
        <!-- Navigation -->
    </nav>
    
    <main class="doc-content">
        <ContentRenderer Item="@docPage" CssClass="documentation-content" />
    </main>
</div>
```

### Product Description

```razor
<div class="product-details">
    <div class="product-images">
        <!-- Product images -->
    </div>
    
    <div class="product-info">
        <h2>@product?.Name</h2>
        <ContentRenderer Item="@productDescription" CssClass="product-description" />
        
        <div class="product-actions">
            <button class="btn btn-primary">Add to Cart</button>
        </div>
    </div>
</div>
```

## Best Practices

1. **Content Validation**: Always check for null content items before rendering
2. **Performance**: Use virtualization for large content lists
3. **Error Handling**: Implement error boundaries for robust content display
4. **Accessibility**: Ensure rendered content maintains semantic structure
5. **SEO**: Combine with proper meta tag components for search optimization
6. **Caching**: Cache content items when appropriate to reduce API calls
7. **Responsive**: Test content rendering across different screen sizes

## Accessibility Features

- **Semantic HTML**: Preserves the semantic structure of rendered content
- **Screen Reader Support**: Works with assistive technologies
- **Keyboard Navigation**: Maintains proper tab order in rendered content
- **Focus Management**: Ensures focusable elements are accessible

## Browser Compatibility

- **Modern Browsers**: Full support for Chrome, Firefox, Safari, Edge
- **HTML Rendering**: Relies on OsirionHtmlRenderer capabilities
- **Progressive Enhancement**: Graceful fallback for unsupported features

The ContentRenderer component provides a simple yet powerful way to display CMS content with all the benefits of the underlying HTML rendering system, making it perfect for scenarios where you need clean, efficient content display without the overhead of full page components.
