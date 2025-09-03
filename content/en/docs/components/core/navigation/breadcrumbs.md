---
id: 'osirion-breadcrumbs'
order: 1
layout: docs
title: OsirionBreadcrumbs Component
permalink: /docs/components/core/navigation/breadcrumbs
description: Learn how to use the OsirionBreadcrumbs component to create accessible navigation breadcrumbs with automatic path parsing and customization options.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Navigation
tags:
- blazor
- breadcrumbs
- navigation
- accessibility
- path-parsing
- user-interface
is_featured: true
published: true
slug: components/core/navigation/breadcrumbs
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionBreadcrumbs Component - Navigation Breadcrumbs | Osirion.Blazor'
  description: 'Create accessible navigation breadcrumbs with the OsirionBreadcrumbs component. Features automatic path parsing, customization, and responsive design.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/navigation/breadcrumbs'
  lang: en
  robots: index, follow
  og_title: 'OsirionBreadcrumbs Component - Osirion.Blazor'
  og_description: 'Create accessible navigation breadcrumbs with automatic path parsing and customization options.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionBreadcrumbs Component - Osirion.Blazor'
  twitter_description: 'Create accessible navigation breadcrumbs with automatic path parsing.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionBreadcrumbs Component

The OsirionBreadcrumbs component provides accessible navigation breadcrumbs with automatic path parsing, customizable formatting, and responsive design. It helps users understand their current location within your application's hierarchy and provides easy navigation back to parent pages.

## Component Overview

OsirionBreadcrumbs automatically parses URL paths to generate hierarchical navigation breadcrumbs. It converts URL segments into user-friendly labels, supports custom formatting, and maintains accessibility standards with proper ARIA labels and semantic markup.

### Key Features

**Automatic Path Parsing**: Converts URL paths into hierarchical breadcrumb trails
**Custom Formatting**: Customizable segment formatting with built-in slug-to-title conversion
**Home Link Integration**: Optional home link with customizable text and URL
**Accessibility Compliant**: Full ARIA support and semantic HTML structure
**Responsive Design**: Mobile-friendly layout with overflow handling
**URL Prefix Support**: Configurable URL prefixes for different routing scenarios
**Last Item Control**: Option to make the last breadcrumb item a link or plain text
**Framework Agnostic**: Compatible with all CSS frameworks and design systems
**SEO Friendly**: Structured data markup for search engine optimization

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Path` | `string` | `""` | The URL path to parse for breadcrumb generation. |
| `ShowHome` | `bool` | `true` | Whether to display the home link as the first breadcrumb. |
| `HomeText` | `string` | `"Home"` | Text displayed for the home link. |
| `HomeUrl` | `string` | `"/"` | URL for the home link. |
| `LinkLastItem` | `bool` | `false` | Whether to make the last breadcrumb item a clickable link. |
| `UrlPrefix` | `string` | `"/"` | URL prefix added to all breadcrumb links. |
| `SegmentFormatter` | `Func<string, string>?` | `null` | Custom function to format breadcrumb segment names. |

## Basic Usage

### Simple Breadcrumbs

```razor
@using Osirion.Blazor.Components
@inject NavigationManager Navigation

<OsirionBreadcrumbs Path="@GetCurrentPath()" />

@code {
    private string GetCurrentPath()
    {
        return Navigation.ToBaseRelativePath(Navigation.Uri);
    }
}
```

### Breadcrumbs without Home Link

```razor
<OsirionBreadcrumbs 
    Path="/products/electronics/smartphones"
    ShowHome="false" />
```

### Custom Home Link

```razor
<OsirionBreadcrumbs 
    Path="/docs/components/forms/contact-form"
    HomeText="Documentation"
    HomeUrl="/docs" />
```

### Breadcrumbs with Custom Formatting

```razor
<OsirionBreadcrumbs 
    Path="/user-profile/settings/notifications"
    SegmentFormatter="@FormatBreadcrumbSegment" />

@code {
    private string FormatBreadcrumbSegment(string segment)
    {
        // Custom formatting logic
        return segment switch
        {
            "user-profile" => "👤 User Profile",
            "settings" => "⚙️ Settings",
            "notifications" => "🔔 Notifications",
            _ => segment.Replace("-", " ").ToTitleCase()
        };
    }
}
```

## Advanced Usage

### Dynamic Breadcrumbs with Page Titles

```razor
@inject IPageTitleService PageTitleService
@inject NavigationManager Navigation

<OsirionBreadcrumbs 
    Path="@currentPath"
    SegmentFormatter="@FormatWithPageTitles"
    LinkLastItem="false"
    Class="dynamic-breadcrumbs" />

@code {
    private string currentPath = "";
    private Dictionary<string, string> pageTitles = new();
    
    protected override async Task OnInitializedAsync()
    {
        currentPath = Navigation.ToBaseRelativePath(Navigation.Uri);
        
        // Load page titles for better breadcrumb labels
        pageTitles = await PageTitleService.GetPageTitlesAsync();
    }
    
    private string FormatWithPageTitles(string segment)
    {
        // Check if we have a custom title for this segment
        if (pageTitles.TryGetValue(segment, out var title))
        {
            return title;
        }
        
        // Fallback to default formatting
        return segment.Replace("-", " ")
                     .Split(' ')
                     .Select(word => char.ToUpperInvariant(word[0]) + word[1..])
                     .Aggregate((a, b) => $"{a} {b}");
    }
}
```

### Breadcrumbs with Icons and Metadata

```razor
<OsirionBreadcrumbs 
    Path="@currentPath"
    SegmentFormatter="@FormatWithIcons"
    Class="icon-breadcrumbs"
    HomeText="🏠 Home" />

@code {
    private string currentPath = "/dashboard/analytics/reports/monthly";
    
    private string FormatWithIcons(string segment)
    {
        var iconMap = new Dictionary<string, string>
        {
            { "dashboard", "📊 Dashboard" },
            { "analytics", "📈 Analytics" },
            { "reports", "📋 Reports" },
            { "monthly", "📅 Monthly Report" },
            { "products", "📦 Products" },
            { "orders", "🛒 Orders" },
            { "customers", "👥 Customers" },
            { "settings", "⚙️ Settings" },
            { "profile", "👤 Profile" }
        };
        
        return iconMap.TryGetValue(segment, out var formatted) 
            ? formatted 
            : $"📄 {FormatDefaultSegment(segment)}";
    }
    
    private string FormatDefaultSegment(string segment)
    {
        return string.Join(" ", segment.Split('-')
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }
}

<style>
.icon-breadcrumbs {
    font-size: 0.95rem;
}

.icon-breadcrumbs .osirion-breadcrumbs-link {
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.5rem;
    border-radius: 0.25rem;
    transition: background-color 0.2s;
}

.icon-breadcrumbs .osirion-breadcrumbs-link:hover {
    background-color: #f8f9fa;
    text-decoration: none;
}
</style>
```

### Breadcrumbs with Dropdown Menus

```razor
<nav aria-label="breadcrumb" class="enhanced-breadcrumbs">
    <ol class="breadcrumb-list">
        <li class="breadcrumb-item">
            <a href="/" class="breadcrumb-link">🏠 Home</a>
        </li>
        
        @foreach (var (segment, index) in GetBreadcrumbSegments().Select((s, i) => (s, i)))
        {
            <li class="breadcrumb-item @(segment.IsLast ? "current" : "")">
                @if (segment.HasChildren && !segment.IsLast)
                {
                    <div class="breadcrumb-dropdown">
                        <button class="breadcrumb-button" @onclick="() => ToggleDropdown(index)">
                            @segment.DisplayName
                            <span class="dropdown-arrow">▼</span>
                        </button>
                        
                        @if (dropdownStates.GetValueOrDefault(index, false))
                        {
                            <div class="dropdown-menu">
                                @foreach (var child in segment.Children)
                                {
                                    <a href="@child.Url" class="dropdown-item">
                                        @child.DisplayName
                                    </a>
                                }
                            </div>
                        }
                    </div>
                }
                else if (segment.IsLast)
                {
                    <span class="breadcrumb-text">@segment.DisplayName</span>
                }
                else
                {
                    <a href="@segment.Url" class="breadcrumb-link">@segment.DisplayName</a>
                }
            </li>
        }
    </ol>
</nav>

@code {
    private Dictionary<int, bool> dropdownStates = new();
    
    private void ToggleDropdown(int index)
    {
        dropdownStates[index] = !dropdownStates.GetValueOrDefault(index, false);
        
        // Close other dropdowns
        foreach (var key in dropdownStates.Keys.Where(k => k != index).ToList())
        {
            dropdownStates[key] = false;
        }
        
        StateHasChanged();
    }
    
    private List<BreadcrumbSegment> GetBreadcrumbSegments()
    {
        // Build breadcrumb segments with children for dropdown menus
        return new List<BreadcrumbSegment>
        {
            new("Documentation", "/docs", false, new[]
            {
                new ChildItem("Getting Started", "/docs/getting-started"),
                new ChildItem("Components", "/docs/components"),
                new ChildItem("API Reference", "/docs/api")
            }),
            new("Components", "/docs/components", false, new[]
            {
                new ChildItem("Core", "/docs/components/core"),
                new ChildItem("CMS", "/docs/components/cms"),
                new ChildItem("Navigation", "/docs/components/navigation")
            }),
            new("Core", "/docs/components/core", false, new[]
            {
                new ChildItem("Cards", "/docs/components/core/cards"),
                new ChildItem("Forms", "/docs/components/core/forms"),
                new ChildItem("Layout", "/docs/components/core/layout")
            }),
            new("Breadcrumbs", "/docs/components/core/navigation/breadcrumbs", true, Array.Empty<ChildItem>())
        };
    }
    
    public record BreadcrumbSegment(string DisplayName, string Url, bool IsLast, ChildItem[] Children)
    {
        public bool HasChildren => Children.Length > 0;
    }
    
    public record ChildItem(string DisplayName, string Url);
}

<style>
.enhanced-breadcrumbs {
    margin-bottom: 2rem;
}

.breadcrumb-list {
    display: flex;
    flex-wrap: wrap;
    list-style: none;
    margin: 0;
    padding: 0;
    gap: 0.5rem;
}

.breadcrumb-item {
    position: relative;
    display: flex;
    align-items: center;
}

.breadcrumb-item:not(:last-child)::after {
    content: "/";
    margin-left: 0.5rem;
    color: #6c757d;
}

.breadcrumb-link {
    color: #007bff;
    text-decoration: none;
    padding: 0.25rem 0.5rem;
    border-radius: 0.25rem;
    transition: all 0.2s;
}

.breadcrumb-link:hover {
    background-color: #e3f2fd;
    text-decoration: none;
}

.breadcrumb-button {
    background: none;
    border: none;
    color: #007bff;
    cursor: pointer;
    padding: 0.25rem 0.5rem;
    border-radius: 0.25rem;
    display: flex;
    align-items: center;
    gap: 0.25rem;
    transition: all 0.2s;
}

.breadcrumb-button:hover {
    background-color: #e3f2fd;
}

.breadcrumb-text {
    color: #6c757d;
    font-weight: 500;
}

.breadcrumb-dropdown {
    position: relative;
}

.dropdown-menu {
    position: absolute;
    top: 100%;
    left: 0;
    background: white;
    border: 1px solid #dee2e6;
    border-radius: 0.375rem;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    min-width: 200px;
    z-index: 1000;
    margin-top: 0.25rem;
}

.dropdown-item {
    display: block;
    padding: 0.5rem 1rem;
    color: #495057;
    text-decoration: none;
    transition: background-color 0.2s;
}

.dropdown-item:hover {
    background-color: #f8f9fa;
    text-decoration: none;
}

.dropdown-item:first-child {
    border-top-left-radius: 0.375rem;
    border-top-right-radius: 0.375rem;
}

.dropdown-item:last-child {
    border-bottom-left-radius: 0.375rem;
    border-bottom-right-radius: 0.375rem;
}

.dropdown-arrow {
    font-size: 0.75rem;
    transition: transform 0.2s;
}

.breadcrumb-dropdown[aria-expanded="true"] .dropdown-arrow {
    transform: rotate(180deg);
}

@media (max-width: 768px) {
    .breadcrumb-list {
        gap: 0.25rem;
    }
    
    .breadcrumb-item {
        font-size: 0.875rem;
    }
    
    .dropdown-menu {
        min-width: 150px;
    }
}
</style>
```

### Responsive Breadcrumbs with Collapsing

```razor
<nav aria-label="breadcrumb" class="responsive-breadcrumbs">
    <div class="breadcrumb-container" @ref="breadcrumbContainer">
        <ol class="breadcrumb-list">
            @if (ShowHome)
            {
                <li class="breadcrumb-item home-item">
                    <a href="/" class="breadcrumb-link">
                        <span class="breadcrumb-icon">🏠</span>
                        <span class="breadcrumb-text">Home</span>
                    </a>
                </li>
            }
            
            @if (shouldCollapse && segments.Count > maxVisibleSegments)
            {
                <!-- Show first segment -->
                <li class="breadcrumb-item">
                    <a href="@segments[0].Url" class="breadcrumb-link">@segments[0].DisplayName</a>
                </li>
                
                <!-- Collapsed indicator -->
                <li class="breadcrumb-item collapsed-item">
                    <button class="collapse-button" @onclick="ToggleCollapsed">
                        <span class="collapse-dots">...</span>
                        <span class="collapse-count">+@(segments.Count - maxVisibleSegments)</span>
                    </button>
                    
                    @if (isExpanded)
                    {
                        <div class="expanded-menu">
                            @for (int i = 1; i < segments.Count - 1; i++)
                            {
                                <a href="@segments[i].Url" class="expanded-item">
                                    @segments[i].DisplayName
                                </a>
                            }
                        </div>
                    }
                </li>
                
                <!-- Show last segment -->
                <li class="breadcrumb-item current-item">
                    <span class="breadcrumb-text">@segments.Last().DisplayName</span>
                </li>
            }
            else
            {
                @foreach (var (segment, index) in segments.Select((s, i) => (s, i)))
                {
                    <li class="breadcrumb-item @(segment.IsLast ? "current-item" : "")">
                        @if (segment.IsLast)
                        {
                            <span class="breadcrumb-text">@segment.DisplayName</span>
                        }
                        else
                        {
                            <a href="@segment.Url" class="breadcrumb-link">@segment.DisplayName</a>
                        }
                    </li>
                }
            }
        </ol>
    </div>
</nav>

@code {
    [Parameter] public bool ShowHome { get; set; } = true;
    [Parameter] public string Path { get; set; } = "";
    
    private ElementReference breadcrumbContainer;
    private bool shouldCollapse = false;
    private bool isExpanded = false;
    private int maxVisibleSegments = 3;
    private List<BreadcrumbSegment> segments = new();
    
    protected override async Task OnInitializedAsync()
    {
        segments = ParsePathToSegments(Path);
        await CheckIfShouldCollapse();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CheckIfShouldCollapse();
        }
    }
    
    private async Task CheckIfShouldCollapse()
    {
        // Check container width and determine if we should collapse
        try
        {
            var dimensions = await JSRuntime.InvokeAsync<BoundingClientRect>("getBoundingClientRect", breadcrumbContainer);
            shouldCollapse = dimensions.Width < 600 || segments.Count > 5;
            StateHasChanged();
        }
        catch
        {
            // Fallback for SSR or if JS is not available
            shouldCollapse = segments.Count > 4;
            StateHasChanged();
        }
    }
    
    private void ToggleCollapsed()
    {
        isExpanded = !isExpanded;
        StateHasChanged();
    }
    
    private List<BreadcrumbSegment> ParsePathToSegments(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries)
                          .Select((segment, index, array) => new BreadcrumbSegment(
                              FormatSegmentName(segment),
                              "/" + string.Join("/", array.Take(index + 1)),
                              index == array.Length - 1
                          ))
                          .ToList();
        return segments;
    }
    
    private string FormatSegmentName(string segment)
    {
        return string.Join(" ", segment.Split('-')
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }
    
    public record BreadcrumbSegment(string DisplayName, string Url, bool IsLast);
    public record BoundingClientRect(double Width, double Height);
}

<style>
.responsive-breadcrumbs {
    margin-bottom: 1.5rem;
}

.breadcrumb-container {
    display: flex;
    align-items: center;
    min-height: 2rem;
}

.breadcrumb-list {
    display: flex;
    flex-wrap: wrap;
    align-items: center;
    list-style: none;
    margin: 0;
    padding: 0;
    gap: 0.5rem;
}

.breadcrumb-item {
    position: relative;
    display: flex;
    align-items: center;
}

.breadcrumb-item:not(:last-child)::after {
    content: "›";
    margin-left: 0.5rem;
    color: #6c757d;
    font-weight: bold;
}

.breadcrumb-link {
    color: #007bff;
    text-decoration: none;
    padding: 0.375rem 0.5rem;
    border-radius: 0.25rem;
    transition: all 0.2s;
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

.breadcrumb-link:hover {
    background-color: #e3f2fd;
    text-decoration: none;
}

.breadcrumb-text {
    color: #6c757d;
    font-weight: 500;
    padding: 0.375rem 0.5rem;
}

.home-item .breadcrumb-icon {
    font-size: 1rem;
}

.collapsed-item {
    position: relative;
}

.collapse-button {
    background: none;
    border: none;
    color: #6c757d;
    cursor: pointer;
    padding: 0.375rem 0.5rem;
    border-radius: 0.25rem;
    display: flex;
    align-items: center;
    gap: 0.25rem;
    transition: all 0.2s;
}

.collapse-button:hover {
    background-color: #f8f9fa;
    color: #495057;
}

.collapse-dots {
    font-weight: bold;
    letter-spacing: 0.1em;
}

.collapse-count {
    font-size: 0.75rem;
    background: #6c757d;
    color: white;
    padding: 0.125rem 0.25rem;
    border-radius: 0.25rem;
}

.expanded-menu {
    position: absolute;
    top: 100%;
    left: 0;
    background: white;
    border: 1px solid #dee2e6;
    border-radius: 0.375rem;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    min-width: 200px;
    z-index: 1000;
    margin-top: 0.25rem;
}

.expanded-item {
    display: block;
    padding: 0.5rem 1rem;
    color: #495057;
    text-decoration: none;
    transition: background-color 0.2s;
}

.expanded-item:hover {
    background-color: #f8f9fa;
    text-decoration: none;
}

@media (max-width: 768px) {
    .breadcrumb-list {
        gap: 0.25rem;
    }
    
    .breadcrumb-link,
    .breadcrumb-text {
        font-size: 0.875rem;
        padding: 0.25rem 0.375rem;
    }
    
    .home-item .breadcrumb-text {
        display: none;
    }
    
    .collapsed-item {
        display: block;
    }
}

@media (max-width: 480px) {
    .breadcrumb-link,
    .breadcrumb-text {
        font-size: 0.75rem;
        padding: 0.25rem;
    }
}
</style>

<script>
window.getBoundingClientRect = (element) => {
    if (!element) return { width: 0, height: 0 };
    const rect = element.getBoundingClientRect();
    return { width: rect.width, height: rect.height };
};
</script>
```

## Styling Examples

### Bootstrap Integration

```razor
<OsirionBreadcrumbs 
    Path="@currentPath"
    Class="breadcrumb bg-light rounded p-3"
    HomeText="Home" />

<style>
/* Custom Bootstrap breadcrumb styling */
.osirion-breadcrumbs.breadcrumb {
    background: #f8f9fa;
}

.osirion-breadcrumbs .osirion-breadcrumbs-list {
    display: flex;
    flex-wrap: wrap;
    padding: 0;
    margin: 0;
    list-style: none;
}

.osirion-breadcrumbs .osirion-breadcrumbs-item + .osirion-breadcrumbs-item::before {
    content: "/";
    color: #6c757d;
    padding: 0 0.5rem;
}

.osirion-breadcrumbs .osirion-breadcrumbs-link {
    color: #007bff;
    text-decoration: none;
}

.osirion-breadcrumbs .osirion-breadcrumbs-link:hover {
    color: #0056b3;
    text-decoration: underline;
}

.osirion-breadcrumbs .osirion-breadcrumbs-current {
    color: #6c757d;
}
</style>
```

### Tailwind CSS Integration

```razor
<OsirionBreadcrumbs 
    Path="@currentPath"
    Class="flex flex-wrap items-center space-x-2 text-sm text-gray-500 bg-gray-50 px-4 py-2 rounded-lg" />

<style>
/* Tailwind-compatible styles */
.osirion-breadcrumbs {
    @apply flex flex-wrap items-center space-x-2;
}

.osirion-breadcrumbs-list {
    @apply flex flex-wrap items-center space-x-2 list-none p-0 m-0;
}

.osirion-breadcrumbs-link {
    @apply text-blue-600 hover:text-blue-800 hover:underline transition-colors duration-200;
}

.osirion-breadcrumbs-text {
    @apply text-gray-600 font-medium;
}

.osirion-breadcrumbs-item:not(:last-child)::after {
    @apply text-gray-400 mx-2;
    content: ">";
}
</style>
```

## Best Practices

### Navigation Guidelines

1. **Clear Hierarchy**: Ensure breadcrumbs reflect the actual site structure
2. **Consistent Formatting**: Use consistent naming conventions across all breadcrumbs
3. **Mobile Responsiveness**: Implement collapsing or truncation for mobile devices
4. **Accessibility**: Include proper ARIA labels and semantic markup
5. **Performance**: Optimize breadcrumb generation for large site hierarchies

### User Experience

1. **Logical Structure**: Make breadcrumb hierarchy intuitive and logical
2. **Visual Clarity**: Use clear separators and hover states
3. **Current Page Indication**: Clearly indicate the current page in the breadcrumb trail
4. **Click Targets**: Ensure adequate click target sizes for touch devices
5. **Loading States**: Show skeleton or placeholder breadcrumbs during navigation

### Technical Implementation

1. **URL Parsing**: Handle complex URL structures and query parameters appropriately
2. **Caching**: Cache breadcrumb data for frequently accessed pages
3. **SEO Optimization**: Include structured data markup for search engines
4. **Error Handling**: Gracefully handle invalid or malformed paths
5. **Testing**: Test breadcrumbs across different browsers and screen sizes

The OsirionBreadcrumbs component provides a robust foundation for implementing navigation breadcrumbs with excellent accessibility and user experience.
