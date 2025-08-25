# OsirionBreadcrumbs Component

[![Component](https://img.shields.io/badge/Component-Core-blue)](https://github.com/obrana-boranija/Osirion.Blazor/tree/master/src/Osirion.Blazor.Core)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor.Core)](https://www.nuget.org/packages/Osirion.Blazor.Core)

The `OsirionBreadcrumbs` component automatically generates breadcrumb navigation from URL paths, providing users with clear navigation context and improving site usability. It's fully SSR-compatible and includes extensive customization options.

## Features

- **Automatic Path Parsing**: Generates breadcrumbs from URL paths
- **Customizable Formatting**: Transform URL segments into user-friendly text
- **Home Link Integration**: Optional home breadcrumb with custom text and URL
- **Flexible URL Generation**: Configurable URL prefixes and patterns
- **SSR Compatible**: Works perfectly with server-side rendering
- **Accessibility**: Full ARIA support and semantic markup
- **Framework Integration**: Adapts to Bootstrap, Fluent UI, and other frameworks

## Basic Usage

```razor
@using Osirion.Blazor.Components

<!-- Simple breadcrumbs from current URL -->
<OsirionBreadcrumbs Path="@Navigation.Uri" />

<!-- Custom path -->
<OsirionBreadcrumbs Path="/blog/web-development/blazor-components" />
```

## Advanced Usage

### Blog Breadcrumbs with Custom Formatting

```razor
<OsirionBreadcrumbs 
    Path="/blog/categories/web-development/articles/building-blazor-components"
    ShowHome="true"
    HomeText="?? Home"
    HomeUrl="/"
    LinkLastItem="false"
    UrlPrefix="/blog/"
    SegmentFormatter="@FormatBlogSegment" />

@code {
    private string FormatBlogSegment(string segment)
    {
        // Convert "web-development" to "Web Development"
        return segment.Replace("-", " ")
                     .Split(' ')
                     .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
                     .Aggregate((a, b) => $"{a} {b}");
    }
}
```

### E-commerce Product Breadcrumbs

```razor
<OsirionBreadcrumbs 
    Path="/products/electronics/computers/laptops/gaming-laptop-xyz"
    ShowHome="true"
    HomeText="Store"
    HomeUrl="/store"
    LinkLastItem="true"
    UrlPrefix="/products/"
    SegmentFormatter="@FormatProductCategory" />

@code {
    private string FormatProductCategory(string segment)
    {
        // Handle special cases for product categories
        return segment switch
        {
            "electronics" => "Electronics",
            "computers" => "Computers & Tablets",
            "laptops" => "Laptops",
            var s when s.StartsWith("gaming-") => s.Replace("gaming-", "").Replace("-", " "),
            _ => segment.Replace("-", " ").ToTitleCase()
        };
    }
}
```

### Documentation Site Breadcrumbs

```razor
<OsirionBreadcrumbs 
    Path="/docs/components/navigation/breadcrumbs"
    ShowHome="true"
    HomeText="Documentation"
    HomeUrl="/docs"
    LinkLastItem="false"
    UrlPrefix="/docs/"
    SegmentFormatter="@FormatDocsSegment" />

@code {
    private string FormatDocsSegment(string segment)
    {
        // Format documentation segments
        var specialCases = new Dictionary<string, string>
        {
            { "api", "API Reference" },
            { "ui", "User Interface" },
            { "ux", "User Experience" },
            { "css", "CSS Styling" },
            { "js", "JavaScript" }
        };
        
        return specialCases.TryGetValue(segment.ToLower(), out var formatted) 
            ? formatted 
            : segment.Replace("-", " ").ToTitleCase();
    }
}
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Path` | `string` | `""` | URL path to generate breadcrumbs from |
| `ShowHome` | `bool` | `true` | Whether to show the home link |
| `HomeText` | `string` | `"Home"` | Text for the home link |
| `HomeUrl` | `string` | `"/"` | URL for the home link |
| `LinkLastItem` | `bool` | `false` | Make the last breadcrumb item clickable |
| `UrlPrefix` | `string` | `"/"` | Prefix for generated breadcrumb URLs |
| `SegmentFormatter` | `Func<string, string>?` | `null` | Function to format URL segments |

## Path Processing

The component automatically processes URL paths by:

1. **Splitting**: Divides the path into segments by `/`
2. **Filtering**: Removes empty segments and common prefixes
3. **Formatting**: Applies the `SegmentFormatter` function (or default formatting)
4. **URL Generation**: Creates clickable links with the `UrlPrefix`

### Default Formatting

When no `SegmentFormatter` is provided, the component uses this default logic:

```csharp
private string FormatSegmentName(string segmentName)
{
    // Replace hyphens with spaces and convert to Title Case
    return Regex.Replace(segmentName, "-", " ", RegexOptions.Compiled)
        .Split(' ')
        .Select(word => word.Length > 0
            ? char.ToUpperInvariant(word[0]) + word[1..]
            : word)
        .Aggregate((a, b) => $"{a} {b}");
}
```

Examples:
- `"web-development"` ? `"Web Development"`
- `"api-reference"` ? `"Api Reference"`
- `"getting-started"` ? `"Getting Started"`

## URL Generation

Breadcrumb URLs are generated using this pattern:
```
{HomeUrl} -> {UrlPrefix}{segment1} -> {UrlPrefix}{segment1}/{segment2} -> ...
```

Example with `UrlPrefix="/blog/"`:
- Home: `/`
- Category: `/blog/web-development`
- Subcategory: `/blog/web-development/frameworks`
- Article: `/blog/web-development/frameworks/blazor-guide`

## Styling and CSS Classes

The component generates semantic HTML with proper CSS classes:

```html
<nav aria-label="Breadcrumb" class="osirion-breadcrumbs">
    <ol class="breadcrumb">
        <li class="breadcrumb-item">
            <a href="/">Home</a>
        </li>
        <li class="breadcrumb-item">
            <a href="/blog/web-development">Web Development</a>
        </li>
        <li class="breadcrumb-item active" aria-current="page">
            Blazor Components
        </li>
    </ol>
</nav>
```

### Custom Styling

Override the default styling with CSS:

```css
.osirion-breadcrumbs {
    padding: 1rem 0;
    background-color: #f8f9fa;
    border-radius: 0.375rem;
}

.osirion-breadcrumbs .breadcrumb {
    margin: 0;
    background: transparent;
}

.osirion-breadcrumbs .breadcrumb-item + .breadcrumb-item::before {
    content: "›";
    color: #6c757d;
}

.osirion-breadcrumbs .breadcrumb-item a {
    color: #0d6efd;
    text-decoration: none;
}

.osirion-breadcrumbs .breadcrumb-item a:hover {
    text-decoration: underline;
}

.osirion-breadcrumbs .breadcrumb-item.active {
    color: #495057;
    font-weight: 500;
}
```

## Framework Integration

### Bootstrap Integration

```razor
<OsirionBreadcrumbs 
    Path="@currentPath"
    Class="bg-light p-3 rounded" />
```

### Fluent UI Integration

```razor
<OsirionBreadcrumbs 
    Path="@currentPath"
    Class="ms-bgColor-neutralLighter ms-p-3" />
```

## Accessibility Features

- **ARIA Labels**: Proper `aria-label` for navigation
- **Current Page**: `aria-current="page"` for the active item
- **Semantic HTML**: Uses `<nav>`, `<ol>`, and `<li>` elements
- **Keyboard Navigation**: Full keyboard support for links
- **Screen Reader**: Descriptive text for assistive technologies

## Dynamic Breadcrumbs

### From Current URL

```razor
@inject NavigationManager Navigation

<OsirionBreadcrumbs 
    Path="@GetCurrentPath()"
    SegmentFormatter="@FormatSegment" />

@code {
    private string GetCurrentPath()
    {
        var uri = new Uri(Navigation.Uri);
        return uri.AbsolutePath;
    }
}
```

### From Route Parameters

```razor
@page "/blog/{category}/{subcategory}/{slug}"

<OsirionBreadcrumbs 
    Path="@($"/blog/{Category}/{Subcategory}/{Slug}")"
    UrlPrefix="/blog/" />

@code {
    [Parameter] public string Category { get; set; } = "";
    [Parameter] public string Subcategory { get; set; } = "";
    [Parameter] public string Slug { get; set; } = "";
}
```

### From Content Management System

```razor
@inject IContentService ContentService

<OsirionBreadcrumbs 
    Path="@currentContent?.Path"
    SegmentFormatter="@FormatContentSegment" />

@code {
    private ContentItem? currentContent;
    
    protected override async Task OnInitializedAsync()
    {
        currentContent = await ContentService.GetCurrentContentAsync();
    }
    
    private string FormatContentSegment(string segment)
    {
        // Use content metadata for better formatting
        return currentContent?.Categories
            .FirstOrDefault(c => c.Slug == segment)?.Name ?? 
            segment.Replace("-", " ").ToTitleCase();
    }
}
```

## Best Practices

1. **Consistent URL Structure**: Maintain consistent URL patterns across your site
2. **Meaningful Segments**: Use descriptive URL segments that make sense as breadcrumbs
3. **Custom Formatting**: Implement `SegmentFormatter` for domain-specific terminology
4. **Performance**: Cache formatted segment names when possible
5. **Mobile Design**: Ensure breadcrumbs work well on mobile devices
6. **SEO Benefits**: Breadcrumbs improve site structure for search engines

## Common Patterns

### Multi-level Categories

```razor
<OsirionBreadcrumbs 
    Path="/products/electronics/smartphones/android/samsung-galaxy"
    SegmentFormatter="@(segment => GetCategoryName(segment))" />
```

### Date-based Archives

```razor
<OsirionBreadcrumbs 
    Path="/archive/2025/01/blazor-updates"
    SegmentFormatter="@FormatArchiveSegment" />

@code {
    private string FormatArchiveSegment(string segment)
    {
        return segment switch
        {
            "2025" => "2025",
            "01" => "January",
            _ => segment.Replace("-", " ").ToTitleCase()
        };
    }
}
```

### User Profiles

```razor
<OsirionBreadcrumbs 
    Path="/users/john-doe/projects/my-blazor-app"
    SegmentFormatter="@FormatUserSegment" />

@code {
    private string FormatUserSegment(string segment)
    {
        if (segment == "john-doe") return "John Doe";
        return segment.Replace("-", " ").ToTitleCase();
    }
}
```

## Troubleshooting

### Common Issues

1. **Missing Breadcrumbs**: Ensure the `Path` parameter is correctly set
2. **Incorrect Formatting**: Check your `SegmentFormatter` function logic
3. **Broken Links**: Verify the `UrlPrefix` matches your routing pattern
4. **Accessibility Issues**: Test with screen readers and keyboard navigation

### Debug Output

```razor
<OsirionBreadcrumbs 
    Path="@currentPath"
    SegmentFormatter="@DebugFormatter" />

@code {
    private string DebugFormatter(string segment)
    {
        Console.WriteLine($"Formatting segment: '{segment}'");
        return segment.Replace("-", " ").ToTitleCase();
    }
}
```