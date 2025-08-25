---
title: "Enhanced Navigation Components in Osirion.Blazor v1.5"
author: "Dejan Demonjić"
date: "2025-01-20"
description: "Explore the new navigation components in Osirion.Blazor v1.5, including enhanced navigation with scroll restoration, breadcrumbs, and scroll-to-top functionality."
tags: [Navigation, Components, Blazor, UX, SSR]
categories: [Components, Navigation]
slug: "enhanced-navigation-interceptor"
is_featured: true
featured_image: "https://images.unsplash.com/photo-1551288049-bebda4e38f71?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
seo_properties:
  title: "Enhanced Navigation Components - Osirion.Blazor v1.5"
  description: "Discover the powerful navigation components in Osirion.Blazor v1.5 including breadcrumbs, scroll restoration, and scroll-to-top functionality."
  og_image_url: "https://images.unsplash.com/photo-1551288049-bebda4e38f71?ixlib=rb-4.0.3&auto=format&fit=crop&w=1200&q=80"
  type: "Article"
---

# Enhanced Navigation Components in Osirion.Blazor v1.5

Navigation is the backbone of user experience in web applications. With Osirion.Blazor v1.5, we've significantly enhanced our navigation components to provide a seamless, accessible, and performant navigation experience that works perfectly with SSR.

## What's New in Navigation

### 🧭 Enhanced Navigation with Scroll Restoration

The `EnhancedNavigation` component now provides intelligent scroll behavior management:

```razor
<EnhancedNavigation 
    Behavior="ScrollBehavior.Smooth"
    ResetScrollOnNavigation="true"
    PreserveScrollForSamePageNavigation="true" />
```

**Key Features:**
- Automatic scroll position restoration when navigating back
- Smooth scrolling animations
- Same-page navigation preservation
- SSR compatibility with progressive enhancement

### 🍞 Automatic Breadcrumbs

The new `OsirionBreadcrumbs` component automatically generates navigation breadcrumbs from URL paths:

```razor
<OsirionBreadcrumbs 
    Path="/blog/components/navigation/breadcrumbs"
    ShowHome="true"
    HomeText="🏠 Home"
    SegmentFormatter="@FormatSegment" />

@code {
    private string FormatSegment(string segment)
    {
        return segment.Replace("-", " ")
                     .Split(' ')
                     .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                     .Aggregate((a, b) => $"{a} {b}");
    }
}
```

**Benefits:**
- Zero configuration breadcrumbs
- Customizable segment formatting
- Full accessibility support
- SEO-friendly structured navigation

### ⬆️ Scroll to Top

The `ScrollToTop` component provides a smooth "back to top" experience:

```razor
<ScrollToTop 
    Position="Position.BottomRight"
    Behavior="ScrollBehavior.Smooth"
    VisibilityThreshold="300"
    Text="Top" />
```

**Features:**
- Configurable visibility threshold
- Multiple positioning options
- Smooth scroll animations
- Customizable appearance

## Implementation Patterns

### Blog Layout Navigation

```razor
@page "/blog/{category}/{slug}"
@inject NavigationManager Navigation

<!-- Breadcrumb navigation -->
<OsirionBreadcrumbs 
    Path="@Navigation.Uri"
    ShowHome="true"
    HomeText="Blog"
    HomeUrl="/blog"
    SegmentFormatter="@FormatBlogSegment" />

<!-- Enhanced navigation -->
<EnhancedNavigation Behavior="ScrollBehavior.Smooth" />

<!-- Article content -->
<article>
    <HeroSection 
        Title="@article.Title"
        Author="@article.Author"
        PublishDate="@article.Date"
        ShowMetadata="true"
        Variant="HeroVariant.Minimal" />
    
    <ContentView Path="@articlePath" />
</article>

<!-- Scroll to top -->
<ScrollToTop Position="Position.BottomRight" />

@code {
    [Parameter] public string Category { get; set; } = "";
    [Parameter] public string Slug { get; set; } = "";
    
    private string articlePath => $"blog/{Category}/{Slug}.md";
    private Article article = new();
    
    private string FormatBlogSegment(string segment)
    {
        return segment switch
        {
            "components" => "Components",
            "navigation" => "Navigation",
            "blazor" => "Blazor",
            _ => segment.Replace("-", " ").ToTitleCase()
        };
    }
}
```

### Documentation Site Navigation

```razor
@page "/docs/{*path}"

<!-- Documentation breadcrumbs -->
<OsirionBreadcrumbs 
    Path="@($"/docs/{Path}")"
    HomeText="Documentation"
    HomeUrl="/docs"
    SegmentFormatter="@FormatDocsSegment" />

<!-- Sticky sidebar with navigation -->
<OsirionPageLayout HasSidebar="true">
    <SidebarContent>
        <OsirionStickySidebar Position="Position.Left">
            <DirectoryNavigation CurrentDirectory="@currentDir" />
        </OsirionStickySidebar>
    </SidebarContent>
    
    <MainContent>
        <ContentView Path="@contentPath" />
    </MainContent>
</OsirionPageLayout>

@code {
    [Parameter] public string Path { get; set; } = "";
    
    private string contentPath => $"docs/{Path}.md";
    private string currentDir => System.IO.Path.GetDirectoryName(Path) ?? "";
    
    private string FormatDocsSegment(string segment)
    {
        var specialCases = new Dictionary<string, string>
        {
            { "api", "API Reference" },
            { "components", "Components" },
            { "getting-started", "Getting Started" },
            { "advanced", "Advanced Topics" }
        };
        
        return specialCases.TryGetValue(segment, out var formatted) 
            ? formatted 
            : segment.Replace("-", " ").ToTitleCase();
    }
}
```

## Advanced Navigation Patterns

### Hierarchical Menu System

```razor
<Menu>
    <MenuGroup Title="Getting Started">
        <MenuItem Text="Installation" Url="/docs/installation" />
        <MenuItem Text="Quick Start" Url="/docs/quick-start" />
        <MenuDivider />
        <MenuItem Text="Configuration" Url="/docs/configuration" />
    </MenuGroup>
    
    <MenuGroup Title="Components">
        <MenuItem Text="Core Components" Url="/docs/components/core" />
        <MenuItem Text="Navigation" Url="/docs/components/navigation" />
        <MenuItem Text="Content Management" Url="/docs/components/cms" />
    </MenuGroup>
    
    <MenuGroup Title="Advanced">
        <MenuItem Text="Custom Providers" Url="/docs/advanced/providers" />
        <MenuItem Text="Performance" Url="/docs/advanced/performance" />
        <MenuItem Text="Theming" Url="/docs/advanced/theming" />
    </MenuGroup>
</Menu>
```

### Contextual Navigation

```razor
@inject IContentProvider ContentProvider

<!-- Context-aware breadcrumbs -->
<OsirionBreadcrumbs 
    Path="@currentContent?.Path"
    SegmentFormatter="@FormatContentSegment" />

<!-- Related content navigation -->
<nav class="related-navigation">
    <h3>Related Articles</h3>
    <ContentList 
        Category="@currentContent?.Categories.FirstOrDefault()"
        Count="5"
        ExcludePath="@currentContent?.Path" />
</nav>

@code {
    private ContentItem? currentContent;
    
    protected override async Task OnInitializedAsync()
    {
        currentContent = await ContentProvider.GetItemByPathAsync(currentPath);
    }
    
    private string FormatContentSegment(string segment)
    {
        // Use content metadata for better formatting
        var category = currentContent?.Categories
            .FirstOrDefault(c => c.Slug == segment);
        
        return category?.Name ?? segment.Replace("-", " ").ToTitleCase();
    }
}
```

## Performance Optimizations

### Scroll Event Optimization

The navigation components use optimized scroll event handling:

```csharp
// Debounced scroll events prevent excessive updates
private async Task HandleScroll()
{
    if (_scrollTimer != null)
    {
        _scrollTimer.Dispose();
    }
    
    _scrollTimer = new Timer(async _ => {
        await UpdateScrollPosition();
    }, null, 100, Timeout.Infinite);
}
```

### Memory-Efficient Breadcrumbs

Breadcrumb segments are cached and reused:

```csharp
private readonly Dictionary<string, BreadcrumbPath> _pathCache = new();

private BreadcrumbPath GetBreadcrumbPath(string path)
{
    if (_pathCache.TryGetValue(path, out var cached))
    {
        return cached;
    }
    
    var breadcrumbPath = ParsePath(path);
    _pathCache[path] = breadcrumbPath;
    return breadcrumbPath;
}
```

## Accessibility Features

All navigation components include comprehensive accessibility support:

### ARIA Labels and Roles

```html
<nav aria-label="Breadcrumb" class="osirion-breadcrumbs">
    <ol class="breadcrumb" role="list">
        <li class="breadcrumb-item" role="listitem">
            <a href="/">Home</a>
        </li>
        <li class="breadcrumb-item active" aria-current="page" role="listitem">
            Current Page
        </li>
    </ol>
</nav>
```

### Keyboard Navigation

```csharp
// Full keyboard support for navigation elements
[Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

private async Task HandleKeyDown(KeyboardEventArgs e)
{
    switch (e.Key)
    {
        case "Enter":
        case " ":
            await NavigateToUrl();
            break;
        case "Escape":
            await CloseMenu();
            break;
    }
}
```

### Screen Reader Support

```razor
<!-- Descriptive text for screen readers -->
<span class="sr-only">
    Navigate to @item.Text. @(item.IsActive ? "Current page" : "")
</span>
```

## SSR Compatibility

All navigation components work seamlessly with SSR:

### Progressive Enhancement

```razor
<!-- Works without JavaScript -->
<nav class="navigation">
    <a href="/docs">Documentation</a>
    <a href="/blog">Blog</a>
</nav>

<!-- Enhanced with JavaScript when available -->
<EnhancedNavigation 
    @rendermode="@RenderMode.InteractiveServer" />
```

### Static Site Generation

```csharp
// Generate static navigation during build
public class NavigationStaticRenderer
{
    public string GenerateStaticNavigation(IEnumerable<ContentItem> items)
    {
        var nav = new StringBuilder();
        nav.AppendLine("<nav class='static-navigation'>");
        
        foreach (var item in items)
        {
            nav.AppendLine($"<a href='{item.Url}'>{item.Title}</a>");
        }
        
        nav.AppendLine("</nav>");
        return nav.ToString();
    }
}
```

## Framework Integration

Navigation components integrate seamlessly with CSS frameworks:

### Bootstrap Navigation

```razor
<nav class="navbar navbar-expand-lg navbar-dark bg-primary">
    <div class="container">
        <a class="navbar-brand" href="/">My Site</a>
        
        <!-- Enhanced navigation with Bootstrap classes -->
        <EnhancedNavigation 
            Class="navbar-nav"
            Behavior="ScrollBehavior.Smooth" />
        
        <!-- Breadcrumbs with Bootstrap styling -->
        <OsirionBreadcrumbs 
            Path="@currentPath"
            Class="breadcrumb bg-light" />
    </div>
</nav>
```

### Fluent UI Navigation

```razor
<nav class="ms-Nav">
    <Menu CssClass="ms-Nav-compositeLink">
        <MenuGroup Title="Navigation">
            <MenuItem Text="Home" Url="/" CssClass="ms-Nav-link" />
            <MenuItem Text="About" Url="/about" CssClass="ms-Nav-link" />
        </MenuGroup>
    </Menu>
</nav>
```

## Best Practices

### 1. Consistent Navigation Structure

```csharp
// Define navigation structure consistently
public class NavigationStructure
{
    public static readonly Dictionary<string, string> SegmentMappings = new()
    {
        { "docs", "Documentation" },
        { "api", "API Reference" },
        { "components", "Components" },
        { "getting-started", "Getting Started" }
    };
    
    public static string FormatSegment(string segment)
    {
        return SegmentMappings.TryGetValue(segment, out var mapped) 
            ? mapped 
            : segment.Replace("-", " ").ToTitleCase();
    }
}
```

### 2. Mobile-First Navigation

```css
/* Mobile-first navigation styles */
.navigation {
    display: flex;
    flex-direction: column;
}

@media (min-width: 768px) {
    .navigation {
        flex-direction: row;
        align-items: center;
    }
}

/* Responsive breadcrumbs */
.osirion-breadcrumbs {
    font-size: 0.875rem;
}

@media (max-width: 576px) {
    .osirion-breadcrumbs .breadcrumb-item:not(:last-child) {
        display: none;
    }
    
    .osirion-breadcrumbs .breadcrumb-item:nth-last-child(2) {
        display: inline;
    }
    
    .osirion-breadcrumbs .breadcrumb-item:nth-last-child(2)::before {
        content: "...";
    }
}
```

### 3. Performance Monitoring

```csharp
// Monitor navigation performance
public class NavigationMetrics
{
    private readonly ILogger<NavigationMetrics> _logger;
    
    public async Task<TimeSpan> MeasureNavigationTime(Func<Task> navigationAction)
    {
        var stopwatch = Stopwatch.StartNew();
        await navigationAction();
        stopwatch.Stop();
        
        _logger.LogInformation("Navigation completed in {Duration}ms", 
            stopwatch.ElapsedMilliseconds);
        
        return stopwatch.Elapsed;
    }
}
```

## Conclusion

The enhanced navigation components in Osirion.Blazor v1.5 provide a comprehensive foundation for building intuitive, accessible, and performant navigation experiences. With automatic breadcrumbs, scroll restoration, and smooth scroll-to-top functionality, you can create navigation that delights users while maintaining excellent performance and accessibility standards.

The components work seamlessly together and integrate with any CSS framework, making them perfect for both simple websites and complex applications. Best of all, they maintain full SSR compatibility while providing progressive enhancement when JavaScript is available.

Start building better navigation experiences today with Osirion.Blazor v1.5!