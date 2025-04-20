# GitHub CMS Components

Osirion.Blazor provides GitHub CMS components that allow you to create a content management system using markdown files in a GitHub repository. These components support features like categories, tags, search, and dynamic navigation.

## Features

- **File-based CMS**: Use markdown files in a GitHub repository as content
- **Frontmatter Support**: Enhanced YAML frontmatter parsing
- **Caching**: Built-in caching for better performance
- **Categories & Tags**: Organize content with categories and tags
- **Search**: Full-text search across content
- **Directory Navigation**: Automatic navigation based on directory structure
- **SSR Compatible**: Works with Server-Side Rendering
- **Customizable Styling**: Full control over component appearance via CSS variables

## Getting Started

1. Configure the service in `Program.cs`:

```csharp
using Osirion.Blazor.Extensions;

// Option 1: Using configuration
builder.Services.AddGitHubCms(builder.Configuration);

// Option 2: Using action delegate
builder.Services.AddGitHubCms(options =>
{
    options.Owner = "your-github-username";
    options.Repository = "your-repo-name";
    options.ContentPath = "content"; // Root directory for content
    options.Branch = "main";
    options.ApiToken = "your-github-token"; // Optional, for private repos
    options.CacheDurationMinutes = 30;
});
```

2. Add configuration to `appsettings.json`:

```json
{
  "GitHubCms": {
    "Owner": "your-github-username",
    "Repository": "your-repo-name",
    "ContentPath": "content",
    "Branch": "main",
    "ApiToken": "", // Optional
    "CacheDurationMinutes": 30,
    "SupportedExtensions": [".md", ".markdown"]
  }
}
```

3. Add the style sheet to your application:

```html
<!-- In App.razor or _Host.cshtml -->
<link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />

<!-- Optional: Override default variables -->
<style>
    :root {
        --osirion-primary-color: #0077cc;
        --osirion-border-radius: 0.25rem;
    }
</style>
```

Alternative: Use the OsirionStyles component:

```razor
<!-- In your layout -->
@using Osirion.Blazor.Components.GitHubCms
<OsirionStyles />
```

## Components

### ContentList

Displays a list of content items with optional filtering.

```razor
@using Osirion.Blazor.Components.GitHubCms

<!-- Show all content -->
<ContentList />

<!-- Show content from specific directory -->
<ContentList Directory="blog" />

<!-- Show content by category -->
<ContentList Category="tutorials" />

<!-- Show content by tag -->
<ContentList Tag="blazor" />

<!-- Show featured content -->
<ContentList FeaturedCount="3" />
```

### ContentView

Displays a single content item.

```razor
<!-- View content by path -->
<ContentView Path="blog/my-post.md" />
```

### CategoriesList

Displays a list of categories with content counts.

```razor
<CategoriesList />
```

### TagCloud

Displays a cloud of tags with content counts.

```razor
<!-- Show all tags -->
<TagCloud />

<!-- Limit number of tags -->
<TagCloud MaxTags="20" />
```

### SearchBox

Provides a search input for content.

```razor
<SearchBox Placeholder="Search articles..." />
```

### DirectoryNavigation

Creates a navigation menu based on directory structure.

```razor
<!-- Basic navigation -->
<DirectoryNavigation />

<!-- With current directory highlighted -->
<DirectoryNavigation CurrentDirectory="blog" />
```

## Content Structure

### Markdown Files

Create markdown files with frontmatter:

```markdown
---
title: "My Blog Post"
author: "John Doe"
date: "2025-04-20"
description: "A brief description of my post"
tags: [blazor, webassembly, dotnet]
keywords: [blazor components, web development]
categories: [tutorials, web]
slug: "my-blog-post"
is_featured: true
featured_image: "https://example.com/image.jpg"
---

# My Blog Post Content

Your markdown content here...
```

### Supported Frontmatter Properties

- `title`: Post title
- `author`: Author name
- `date`: Publication date
- `description`: Brief description
- `tags`: List of tags
- `keywords`: SEO keywords
- `categories`: List of categories
- `slug`: URL-friendly identifier
- `is_featured`: Whether the post is featured
- `featured_image`: URL to featured image

## Styling the Components

Osirion.Blazor components use CSS variables for easy styling customization.

### Basic CSS Integration

Add the styles to your application:

```html
<link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />
```

### Customizing Appearance

Override CSS variables to match your design:

```html
<style>
    :root {
        --osirion-primary-color: #0077cc;
        --osirion-border-radius: 0.25rem;
        --osirion-font-size: 1.1rem;
    }
</style>
```

See the [Styling Guide](./STYLING.md) for a complete reference of all available CSS variables and styling options.

## Advanced Usage

### Custom Routes

```razor
@page "/content/{*Path}"
@inject IGitHubCmsService CmsService

<ContentView Path="@Path" />

@code {
    [Parameter]
    public string Path { get; set; } = string.Empty;
}
```

### Search Results Page

```razor
@page "/search"
@inject IGitHubCmsService CmsService
@inject NavigationManager NavigationManager

@if (Results.Count > 0)
{
    <h2>Search Results for "@Query"</h2>
    <ContentList ContentItems="@Results" />
}
else
{
    <p>No results found for "@Query"</p>
}

@code {
    [Parameter]
    [SupplyParameterFromQuery]
    public string Query { get; set; } = string.Empty;
    
    private List<ContentItem> Results { get; set; } = new();
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Query))
        {
            Results = await CmsService.SearchContentItemsAsync(Query);
        }
    }
}
```

### Cache Management

```csharp
// Refresh cache manually
@inject IGitHubCmsService CmsService

await CmsService.RefreshCacheAsync();
```

## Best Practices

1. **Directory Structure**: Organize content in logical directories
2. **Consistent Frontmatter**: Use consistent frontmatter structure
3. **Optimized Images**: Host images on a CDN for better performance
4. **API Rate Limits**: Use an API token for higher rate limits
5. **Caching Strategy**: Adjust cache duration based on update frequency
6. **Styling Consistency**: Override CSS variables at the application level for consistent styling

## Troubleshooting

1. **Content Not Loading**: Check GitHub API configuration and permissions
2. **Rate Limiting**: Use API token for public repos, required for private repos
3. **Cache Issues**: Manually refresh cache if content appears outdated
4. **Invalid Markdown**: Validate frontmatter syntax and markdown structure
5. **Styling Issues**: Check CSS variable overrides and selector specificity