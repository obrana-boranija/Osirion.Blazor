---
title: "TableOfContents"
description: "A Blazor component for automatically extracting and displaying a hierarchical table of contents from HTML content with customizable levels and depth."
keywords: 
  - "table-of-contents"
  - "toc"
  - "content navigation"
  - "headings"
  - "blazor cms"
  - "content structure"
seo:
  title: "TableOfContents - Osirion Blazor CMS Web Component"
  description: "Learn how to implement automatic table of contents generation in Osirion Blazor CMS with the TableOfContents component featuring hierarchical navigation and customizable levels."
  keywords:
    - "blazor table of contents"
    - "cms navigation"
    - "content headings"
    - "osirion blazor"
date: "2024-12-29"
---

# TableOfContents

The `TableOfContents` component automatically extracts headings from HTML content and displays them as a hierarchical, navigable table of contents. It supports customizable heading levels, nesting depth, and automatic ID generation for seamless page navigation.

## Features

- **Automatic Extraction**: Parses HTML content to find heading elements (h1-h6)
- **Hierarchical Structure**: Builds nested navigation based on heading levels
- **Customizable Levels**: Configure minimum and maximum heading levels to include
- **Depth Control**: Limit nesting depth for better organization
- **ID Generation**: Automatically generates IDs for headings without them
- **Accessible Navigation**: Full accessibility support with proper ARIA labels
- **Empty State Handling**: Customizable message when no headings are found
- **Clean HTML Processing**: Strips HTML tags from heading text

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Content` | `string?` | `null` | The HTML content to extract headings from |
| `MinLevel` | `int` | `2` | Minimum heading level to include (1-6) |
| `MaxLevel` | `int` | `6` | Maximum heading level to include (1-6) |
| `MaxDepth` | `int` | `3` | Maximum nesting depth for hierarchical structure |
| `EmptyText` | `string` | `"No headings found."` | Text displayed when no headings are found |

## Data Structures

### HeadingItem Class

| Property | Type | Description |
|----------|------|-------------|
| `Level` | `int` | The heading level (1-6) |
| `Id` | `string` | The heading ID for anchor navigation |
| `Text` | `string` | The cleaned heading text |
| `Children` | `List<HeadingItem>` | Child headings for hierarchical structure |

## Basic Usage

### Simple Table of Contents

```razor
@using Osirion.Blazor.Cms.Web.Components

<TableOfContents Content="@htmlContent" />

@code {
    private string htmlContent = @"
        <h2 id='introduction'>Introduction</h2>
        <p>Some content...</p>
        <h3 id='getting-started'>Getting Started</h3>
        <p>More content...</p>
        <h3 id='advanced-usage'>Advanced Usage</h3>
        <p>Advanced content...</p>
        <h2 id='conclusion'>Conclusion</h2>
        <p>Final content...</p>
    ";
}
```

### Custom Heading Levels

```razor
<TableOfContents 
    Content="@content"
    MinLevel="1"
    MaxLevel="4"
    EmptyText="No headings available." />
```

### Limited Nesting Depth

```razor
<TableOfContents 
    Content="@content"
    MaxDepth="2"
    EmptyText="Content outline not available." />
```

## Advanced Examples

### Content Article with TOC

```razor
@page "/article/{slug}"
@using Osirion.Blazor.Cms.Web.Components

<div class="article-layout">
    <aside class="article-sidebar">
        <div class="toc-container">
            <h3>Contents</h3>
            <TableOfContents 
                Content="@article?.Content"
                MinLevel="2"
                MaxLevel="4"
                CssClass="article-toc" />
        </div>
    </aside>
    
    <main class="article-content">
        <h1>@article?.Title</h1>
        <div>@((MarkupString)(article?.Content ?? ""))</div>
    </main>
</div>

@code {
    [Parameter] public string Slug { get; set; } = "";
    [Inject] private IContentService ContentService { get; set; } = default!;
    
    private ContentItem? article;

    protected override async Task OnParametersSetAsync()
    {
        article = await ContentService.GetBySlugAsync(Slug);
    }
}
```

### Documentation with Collapsible TOC

```razor
<div class="documentation-page">
    <nav class="doc-nav">
        <details class="toc-details" open>
            <summary class="toc-summary">
                <h3>Table of Contents</h3>
            </summary>
            <TableOfContents 
                Content="@documentContent"
                MinLevel="2"
                MaxLevel="5"
                MaxDepth="3"
                CssClass="doc-toc" />
        </details>
    </nav>
    
    <article class="doc-content">
        @((MarkupString)documentContent)
    </article>
</div>

@code {
    [Parameter] public string DocumentId { get; set; } = "";
    
    private string documentContent = "";

    protected override async Task OnInitializedAsync()
    {
        documentContent = await LoadDocumentationAsync(DocumentId);
    }
}
```

### Interactive TOC with Highlighting

```razor
<div class="content-with-toc">
    <nav class="toc-nav">
        <TableOfContents 
            Content="@content"
            CssClass="interactive-toc"
            @ref="tocComponent" />
    </nav>
    
    <div class="content-area" @onscroll="@HandleScroll">
        @((MarkupString)content)
    </div>
</div>

@code {
    private TableOfContents? tocComponent;
    private string content = "";

    private async Task HandleScroll(EventArgs e)
    {
        // Find the currently visible heading and highlight it in TOC
        await JSRuntime.InvokeVoidAsync("highlightCurrentTocItem");
    }
}

<script>
    window.highlightCurrentTocItem = () => {
        const headings = document.querySelectorAll('h2, h3, h4, h5, h6');
        const tocLinks = document.querySelectorAll('.osirion-toc-link');
        
        let currentHeading = null;
        for (const heading of headings) {
            const rect = heading.getBoundingClientRect();
            if (rect.top <= 100) {
                currentHeading = heading;
            }
        }
        
        tocLinks.forEach(link => link.classList.remove('active'));
        if (currentHeading) {
            const activeLink = document.querySelector(`a[href="#${currentHeading.id}"]`);
            activeLink?.classList.add('active');
        }
    };
</script>
```

### Multi-Column TOC

```razor
<div class="blog-post">
    <header class="post-header">
        <h1>@post.Title</h1>
        <div class="post-meta">
            <span>@post.PublishedDate.ToString("MMMM dd, yyyy")</span>
            <span>•</span>
            <span>@estimatedReadTime minutes read</span>
        </div>
        
        @if (showToc)
        {
            <div class="post-toc-wrapper">
                <TableOfContents 
                    Content="@post.Content"
                    MinLevel="2"
                    MaxLevel="4"
                    CssClass="post-toc-columns" />
            </div>
        }
    </header>
    
    <article class="post-content">
        @((MarkupString)post.Content)
    </article>
</div>

@code {
    private bool showToc => GetHeadingCount() >= 3;
    private int estimatedReadTime => CalculateReadTime(post.Content);

    private int GetHeadingCount()
    {
        return System.Text.RegularExpressions.Regex.Matches(
            post.Content ?? "", 
            @"<h[2-6][^>]*>.*?</h[2-6]>").Count;
    }
}
```

### Custom TOC with Progress Tracking

```razor
<div class="tutorial-container">
    <aside class="tutorial-sidebar">
        <div class="progress-toc">
            <h3>Tutorial Progress</h3>
            <TableOfContents 
                Content="@tutorialContent"
                MinLevel="2"
                MaxLevel="3"
                CssClass="progress-toc-list" />
            <div class="progress-bar">
                <div class="progress-fill" style="width: @progressPercentage%"></div>
            </div>
        </div>
    </aside>
    
    <main class="tutorial-content">
        @((MarkupString)tutorialContent)
    </main>
</div>

@code {
    private string tutorialContent = "";
    private double progressPercentage = 0;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("initializeProgressTracking");
        }
    }
}
```

## CSS Customization

### Default Styling

```css
.osirion-table-of-contents {
    border: 1px solid var(--border-color);
    border-radius: 0.5rem;
    padding: 1rem;
    background: var(--background-color);
}

.osirion-toc-list {
    list-style: none;
    margin: 0;
    padding: 0;
}

.osirion-toc-item {
    margin-bottom: 0.25rem;
}

.osirion-toc-link {
    display: block;
    padding: 0.25rem 0;
    color: var(--text-color);
    text-decoration: none;
    border-left: 2px solid transparent;
    padding-left: 0.5rem;
    transition: all 0.2s ease;
}

.osirion-toc-link:hover {
    color: var(--primary-color);
    border-left-color: var(--primary-color);
}

.osirion-toc-link.active {
    color: var(--primary-color);
    border-left-color: var(--primary-color);
    background-color: var(--primary-color-light);
}
```

### Level-specific Styling

```css
.osirion-toc-level-2 .osirion-toc-link {
    font-weight: 600;
    font-size: 1rem;
}

.osirion-toc-level-3 .osirion-toc-link {
    font-size: 0.875rem;
    padding-left: 1rem;
}

.osirion-toc-level-4 .osirion-toc-link {
    font-size: 0.8125rem;
    padding-left: 1.5rem;
    color: var(--text-color-secondary);
}

.osirion-toc-level-5 .osirion-toc-link,
.osirion-toc-level-6 .osirion-toc-link {
    font-size: 0.75rem;
    padding-left: 2rem;
    color: var(--text-color-tertiary);
}
```

### Compact TOC

```css
.osirion-table-of-contents.compact {
    padding: 0.5rem;
    border: none;
    background: transparent;
}

.osirion-table-of-contents.compact .osirion-toc-link {
    padding: 0.125rem 0;
    font-size: 0.875rem;
}
```

### Sticky TOC

```css
.osirion-table-of-contents.sticky {
    position: sticky;
    top: 2rem;
    max-height: calc(100vh - 4rem);
    overflow-y: auto;
}
```

## Framework-Specific Styling

### Bootstrap Integration

```css
.osirion-table-of-contents.bootstrap {
    @extend .card;
}

.osirion-table-of-contents.bootstrap .osirion-toc-list {
    @extend .list-unstyled;
}

.osirion-table-of-contents.bootstrap .osirion-toc-link {
    @extend .text-decoration-none;
}

.osirion-table-of-contents.bootstrap .osirion-toc-link:hover {
    @extend .text-primary;
}
```

### Tailwind CSS

```css
.osirion-table-of-contents {
    @apply border border-gray-200 rounded-lg p-4 bg-white;
}

.osirion-toc-list {
    @apply list-none m-0 p-0;
}

.osirion-toc-link {
    @apply block py-1 text-gray-700 no-underline border-l-2 border-transparent pl-2 transition-all duration-200;
}

.osirion-toc-link:hover {
    @apply text-blue-600 border-blue-600;
}

.osirion-toc-level-3 .osirion-toc-link {
    @apply pl-4 text-sm;
}

.osirion-toc-level-4 .osirion-toc-link {
    @apply pl-6 text-xs text-gray-600;
}
```

## Accessibility Features

- **Semantic Navigation**: Uses proper `nav` element with ARIA label
- **Keyboard Navigation**: Full keyboard accessibility for all links
- **Screen Reader Support**: Proper structure and labeling
- **Focus Management**: Clear focus indicators
- **Hierarchical Structure**: Maintains semantic heading hierarchy

## Best Practices

1. **Heading Structure**: Ensure your content uses proper heading hierarchy
2. **ID Generation**: Provide meaningful IDs for headings when possible
3. **Content Length**: Consider showing TOC only for longer content
4. **Mobile Experience**: Make TOC collapsible on smaller screens
5. **Performance**: Cache extracted headings for static content
6. **Visual Hierarchy**: Use styling to show heading relationships clearly

## Integration Examples

### CMS Content Pages

```razor
@page "/page/{slug}"
@using Osirion.Blazor.Cms.Web.Components

<div class="page-layout">
    @if (ShouldShowToc())
    {
        <aside class="page-sidebar">
            <TableOfContents 
                Content="@page?.Content"
                MinLevel="2"
                MaxLevel="4"
                CssClass="page-toc" />
        </aside>
    }
    
    <main class="page-content @(ShouldShowToc() ? "with-toc" : "")">
        <h1>@page?.Title</h1>
        @if (!string.IsNullOrEmpty(page?.Summary))
        {
            <div class="page-summary">@page.Summary</div>
        }
        <div class="page-body">
            @((MarkupString)(page?.Content ?? ""))
        </div>
    </main>
</div>

@code {
    [Parameter] public string Slug { get; set; } = "";
    [Inject] private IContentService ContentService { get; set; } = default!;
    
    private ContentItem? page;

    protected override async Task OnParametersSetAsync()
    {
        page = await ContentService.GetPageBySlugAsync(Slug);
    }

    private bool ShouldShowToc()
    {
        if (string.IsNullOrEmpty(page?.Content)) return false;
        
        var headingCount = System.Text.RegularExpressions.Regex
            .Matches(page.Content, @"<h[2-6][^>]*>.*?</h[2-6]>").Count;
        
        return headingCount >= 3;
    }
}
```

### Print-Friendly TOC

```razor
<div class="printable-document">
    <div class="print-toc">
        <h2>Table of Contents</h2>
        <TableOfContents 
            Content="@documentContent"
            CssClass="print-friendly-toc" />
    </div>
    
    <div class="document-content">
        @((MarkupString)documentContent)
    </div>
</div>

<style>
    @media print {
        .print-toc {
            page-break-after: always;
        }
        
        .osirion-toc-link:after {
            content: leader(dotted) target-counter(attr(href), page);
        }
    }
</style>
```

## Related Components

- [`ContentView`](../core/content-view.md) - For displaying content with TOC
- [`ContentBreadcrumbs`](content-breadcrumbs.md) - For hierarchical navigation
- [`DirectoryNavigation`](directory-navigation.md) - For directory-based navigation
- [`OsirionContentNavigation`](osirion-content-navigation.md) - For previous/next navigation
