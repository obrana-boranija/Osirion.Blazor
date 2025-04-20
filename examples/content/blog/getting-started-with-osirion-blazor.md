// content/blog/getting-started-with-osirion-blazor.md
---
title: "Getting Started with Osirion.Blazor"
author: "Dejan Demonjić"
date: "2025-04-19"
description: "A comprehensive guide to getting started with Osirion.Blazor components and tools."
tags: [Blazor, .NET, Web Development, Tutorial]
categories: [Tutorials, Blazor]
slug: "getting-started-with-osirion-blazor"
is_featured: true
featured_image: "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2070&q=80"
---

# Getting Started with Osirion.Blazor

Osirion.Blazor is a powerful library of components and utilities designed to enhance your Blazor development experience. In this tutorial, we'll explore how to set up and use Osirion.Blazor in your projects.

## Prerequisites

Before we begin, ensure you have the following:

- .NET 8.0 SDK or newer
- Visual Studio 2022 or VS Code
- Basic knowledge of Blazor

## Installation

To install Osirion.Blazor, add the package to your project using NuGet:

```bash
dotnet add package Osirion.Blazor
```

Alternatively, you can use the NuGet Package Manager in Visual Studio.

## Basic Setup

After installing the package, update your `Program.cs` file to register Osirion.Blazor services:

```csharp
using Osirion.Blazor.Extensions;

// Add Osirion.Blazor services
builder.Services.AddOsirionBlazor();

// Add GitHub CMS if needed
builder.Services.AddGitHubCms(options =>
{
    options.Owner = "your-github-username";
    options.Repository = "your-content-repo";
    options.Branch = "main";
});
```

## Using Navigation Components

Osirion.Blazor provides enhanced navigation components for Blazor. Add the following to your `_Imports.razor`:

```razor
@using Osirion.Blazor.Components.Navigation
```

Then, in your `App.razor` or layout file:

```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

This will ensure smooth scrolling behavior during navigation.

## Analytics Integration

To add analytics tracking:

```razor
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options

<ClarityTracker Options="new ClarityOptions { 
    TrackerUrl = 'https://www.clarity.ms/tag/', 
    SiteId = 'your-clarity-id',
    Track = true 
}" />

<MatomoTracker Options="new MatomoOptions {
    TrackerUrl = '//analytics.example.com/',
    SiteId = '1',
    Track = true
}" />
```

## Using GitHub CMS

To implement a content management system using GitHub:

```razor
@using Osirion.Blazor.Components.GitHubCms

<!-- Display a list of posts -->
<ContentList Directory="blog" />

<!-- Display posts by category -->
<ContentList Category="tutorials" />

<!-- Display posts by tag -->
<ContentList Tag="blazor" />

<!-- Show featured posts -->
<ContentList FeaturedCount="3" />

<!-- View a single post -->
<ContentView Path="blog/my-post.md" />

<!-- Display categories and tags -->
<CategoriesList />
<TagCloud MaxTags="15" />

<!-- Add search functionality -->
<SearchBox Placeholder="Search blog..." />
```

## Conclusion

Osirion.Blazor provides a rich set of components and utilities to enhance your Blazor applications. By following this guide, you've learned how to set up and use some of the key features.

For more information, check out the [GitHub repository](https://github.com/obrana-boranija/Osirion.Blazor) and the detailed documentation.

// content/blog/enhanced-navigation-interceptor.md
---
title: "EnhancedNavigationInterceptor: Auto-Scrolling for Blazor Navigation"
author: "Dejan Demonjić"
date: "2025-04-15"
description: "Technical implementation of scroll position reset during Blazor navigation using the EnhancedNavigationInterceptor component."
tags: [Blazor, Navigation, Component, .NET]
categories: [Technical, Components]
slug: "enhanced-navigation-interceptor"
is_featured: false
featured_image: "https://images.unsplash.com/photo-1519389950473-47ba0277781c?auto=format&fit=crop&q=80&w=2070&ixlib=rb-4.0.3"
---

# EnhancedNavigationInterceptor: Auto-Scrolling for Blazor Navigation

One common issue with Blazor's enhanced navigation is that it doesn't automatically reset the scroll position when navigating between pages. This can lead to a confusing user experience where users land in the middle of a page after clicking a link.

## The Problem

Blazor's default navigation behavior preserves the scroll position when navigating between pages to optimize the user experience. While this is beneficial for some scenarios, it's often not the expected behavior for most websites.

Consider this scenario:
1. User scrolls down to the bottom of Page A
2. User clicks a link to Page B
3. Page B loads but the scroll position remains at the bottom
4. User needs to manually scroll to the top to see the beginning of Page B

## The Solution: EnhancedNavigationInterceptor

Osirion.Blazor provides the `EnhancedNavigationInterceptor` component that solves this problem elegantly. Here's how it works:

```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

This component:
1. Listens for Blazor's navigation events
2. Detects when navigation to a new page has completed
3. Automatically scrolls to the top of the page using the specified behavior

## Scroll Behavior Options

The component supports three scroll behaviors:

- `ScrollBehavior.Auto`: Uses the browser's default scrolling behavior
- `ScrollBehavior.Instant`: Jumps immediately to the top without animation
- `ScrollBehavior.Smooth`: Smoothly animates the scroll to the top

## Technical Implementation

Under the hood, the `EnhancedNavigationInterceptor` component injects a small JavaScript snippet that:

1. Listens for Blazor's `enhancedload` event
2. Compares the current URL with the new URL
3. If the URL has changed, it calls `window.scrollTo()` with the appropriate behavior

Here's a simplified version of the implementation:

```csharp
private string GetScript()
{
    return $@"
        <script>
            (function() {{
                let currentUrl = window.location.href;
                Blazor.addEventListener('enhancedload', () => {{
                    let newUrl = window.location.href;
                    if (currentUrl != newUrl) {{
                        window.scrollTo({{ top: 0, left: 0, behavior: '{BehaviorString}' }});
                    }}
                    currentUrl = newUrl;
                }});
            }})();
        </script>
    ";
}
```

## Best Practices

Here are some best practices for using the `EnhancedNavigationInterceptor`:

1. Place it in your `App.razor` or main layout component
2. Use `ScrollBehavior.Smooth` for most websites for better UX
3. Consider using `ScrollBehavior.Instant` for admin dashboards or data-heavy applications
4. Only include one instance of the component in your application

## Conclusion

The `EnhancedNavigationInterceptor` component provides a simple solution to a common Blazor navigation issue. By automatically managing scroll position during navigation, it creates a more intuitive and user-friendly experience.

For more information, check out the [Osirion.Blazor documentation](https://github.com/obrana-boranija/Osirion.Blazor).

// content/blog/github-cms-with-blazor.md
---
title: "Building a Markdown-Based CMS with GitHub and Blazor"
author: "Dejan Demonjić"
date: "2025-04-18"
description: "Learn how to create a content management system using markdown files in GitHub repositories with Osirion.Blazor's GitHub CMS components."
tags: [CMS, GitHub, Markdown, Blazor]
categories: [Tutorials, CMS]
slug: "github-cms-with-blazor"
is_featured: true
featured_image: "https://images.unsplash.com/photo-1556075798-4825dfaaf498?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
---

# Building a Markdown-Based CMS with GitHub and Blazor

Content management doesn't have to be complicated. With Osirion.Blazor's GitHub CMS components, you can create a powerful content management system using markdown files stored in a GitHub repository.

## Why Use GitHub as a CMS?

GitHub offers several advantages as a content repository:

1. **Version Control**: Every change is tracked and can be reverted
2. **Markdown Support**: Write content in markdown with frontmatter metadata
3. **Collaboration**: Use pull requests for content reviews
4. **Free Hosting**: Store your content on GitHub at no cost
5. **Content API**: Access content through GitHub's API

## Setting Up GitHub CMS

First, add the Osirion.Blazor package to your project:

```bash
dotnet add package Osirion.Blazor
```

Then, configure the GitHub CMS service in your `Program.cs`:

```csharp
using Osirion.Blazor.Extensions;

builder.Services.AddGitHubCms(options =>
{
    options.Owner = "your-github-username";
    options.Repository = "your-content-repo";
    options.ContentPath = "content"; // Optional, defaults to root
    options.Branch = "main";
    options.ApiToken = builder.Configuration["GitHub:ApiToken"]; // Optional
    options.CacheDurationMinutes = 30;
});
```

## Creating Content

GitHub CMS uses markdown files with YAML frontmatter for content:

```markdown
---
title: "My Blog Post"
author: "Your Name"
date: "2025-04-18"
description: "A brief description of the post"
tags: [blazor, webassembly, dotnet]
categories: [tutorials, web]
slug: "my-blog-post"
is_featured: true
featured_image: "https://example.com/image.jpg"
---

# My Blog Post Content

Your markdown content here...
```

## Displaying Content

Osirion.Blazor provides several components for displaying content:

### Content List

Display a list of content items:

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

### Single Content View

Display a single content item:

```razor
<ContentView Path="blog/my-post.md" />
```

### Categories and Tags

Display categories and tags:

```razor
<CategoriesList />
<TagCloud MaxTags="20" />
```

### Search Box

Add search functionality:

```razor
<SearchBox Placeholder="Search articles..." />
```

## Advanced Features

### Directory Navigation

Create navigation based on your repository structure:

```razor
<DirectoryNavigation CurrentDirectory="@currentDir" />
```

### Custom Routing

Implement custom routes for your content:

```razor
@page "/blog/{*Path}"

<ContentView Path="@Path" />

@code {
    [Parameter]
    public string Path { get; set; } = string.Empty;
}
```

### Cache Management

Manage content caching for better performance:

```razor
@inject IGitHubCmsService CmsService

// Refresh cache manually
await CmsService.RefreshCacheAsync();
```

## Conclusion

With Osirion.Blazor's GitHub CMS components, you can create a powerful content management system using just GitHub and Blazor. This approach offers:

- Simplicity: No database setup required
- Version control: Track all content changes
- Flexibility: Use markdown with full HTML support
- Performance: Built-in caching for fast response times

Start building your GitHub-based CMS today with Osirion.Blazor!

// content/blog/analytics-integration-in-blazor.md
---
title: "Analytics Integration for Blazor Applications"
author: "Dejan Demonjić"
date: "2025-04-12"
description: "How to integrate analytics tools like Microsoft Clarity and Matomo into your Blazor applications using Osirion.Blazor components."
tags: [Analytics, Tracking, Blazor, Privacy]
categories: [Analytics, Integration]
slug: "analytics-integration-in-blazor"
is_featured: false
featured_image: "https://images.unsplash.com/photo-1551288049-bebda4e38f71?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
---

# Analytics Integration for Blazor Applications

Adding analytics to your Blazor application can provide valuable insights into user behavior. Osirion.Blazor offers easy integration with popular analytics platforms like Microsoft Clarity and Matomo.

## Why Use Analytics in Blazor?

Analytics tools can help you:

1. Understand user engagement
2. Identify usability issues
3. Track conversion rates
4. Optimize performance
5. Make data-driven decisions

## Available Analytics Components

Osirion.Blazor provides components for two popular analytics platforms:

### Microsoft Clarity

Clarity is Microsoft's free analytics tool that provides heatmaps, session recordings, and insights.

```razor
@using Osirion.Blazor.Components.Analytics
@using Osirion.Blazor.Components.Analytics.Options

<ClarityTracker Options="new ClarityOptions { 
    TrackerUrl = 'https://www.clarity.ms/tag/', 
    SiteId = 'your-clarity-id',
    Track = true 
}" />
```

### Matomo (formerly Piwik)

Matomo is an open-source analytics platform that gives you full control over your data.

```razor
<MatomoTracker Options="new MatomoOptions {
    TrackerUrl = '//analytics.example.com/',
    SiteId = '1',
    Track = true
}" />
```

## Configuration Options

Both trackers support the following options:

- `TrackerUrl`: The URL of the analytics script
- `SiteId`: Your site ID for the analytics platform
- `Track`: Controls whether tracking is enabled

## Setting Up with Dependency Injection

You can also configure analytics via dependency injection:

```csharp
// Program.cs
using Osirion.Blazor.Extensions;

// Using configuration
builder.Services.AddClarityTracker(builder.Configuration);
builder.Services.AddMatomoTracker(builder.Configuration);

// Or programmatically
builder.Services.AddClarityTracker(options =>
{
    options.TrackerUrl = "https://www.clarity.ms/tag/";
    options.SiteId = "your-clarity-id";
    options.Track = true;
});
```

Then in your layout:

```razor
@inject IOptions<ClarityOptions> ClarityOptions
@inject IOptions<MatomoOptions> MatomoOptions

@if (ClarityOptions?.Value != null)
{
    <ClarityTracker Options="@ClarityOptions.Value" />
}

@if (MatomoOptions?.Value != null)
{
    <MatomoTracker Options="@MatomoOptions.Value" />
}
```

## Environment-Specific Configuration

It's a good practice to disable analytics in development environments:

```csharp
if (builder.Environment.IsProduction())
{
    builder.Services.AddClarityTracker(builder.Configuration);
    builder.Services.AddMatomoTracker(builder.Configuration);
}
else
{
    builder.Services.AddClarityTracker(options => options.Track = false);
    builder.Services.AddMatomoTracker(options => options.Track = false);
}
```

## Privacy Considerations

When implementing analytics, consider these privacy best practices:

1. Obtain user consent before enabling tracking
2. Provide a clear privacy policy
3. Allow users to opt out
4. Consider data retention policies
5. Be transparent about what data is collected

## Conditional Tracking

You can conditionally enable tracking based on user consent:

```razor
@if (ConsentGiven)
{
    <ClarityTracker Options="@ClarityOptions" />
}
```

## Conclusion

Osirion.Blazor makes it easy to integrate analytics into your Blazor applications. By following the examples in this article, you can start tracking user behavior and gaining valuable insights to improve your application.

Remember to respect user privacy and comply with regulations like GDPR when implementing analytics.

// content/docs/github-cms.md
---
title: "GitHub CMS Documentation"
author: "Dejan Demonjić"
date: "2025-04-20"
description: "Comprehensive documentation for the GitHub CMS components in Osirion.Blazor."
tags: [Documentation, GitHub, CMS, Reference]
categories: [Documentation]
slug: "github-cms-documentation"
is_featured: false
featured_image: "https://images.unsplash.com/photo-1614332287897-cdc485fa562d?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
---

# GitHub CMS Documentation

This document provides comprehensive documentation for the GitHub CMS components in Osirion.Blazor.

## Table of Contents

1. [Introduction](#introduction)
2. [Components](#components)
3. [Models](#models)
4. [Services](#services)
5. [Configuration](#configuration)
6. [Examples](#examples)

## Introduction

GitHub CMS is a content management system that uses markdown files stored in a GitHub repository as the content source. It provides:

- File-based content management
- Frontmatter support for metadata
- Caching for better performance
- Categories, tags, and directory-based organization
- Search functionality

## Components

### ContentList

Displays a list of content items with optional filtering.

```razor
<!-- Basic usage -->
<ContentList />

<!-- With filters -->
<ContentList Directory="blog" />
<ContentList Category="tutorials" />
<ContentList Tag="blazor" />
<ContentList FeaturedCount="3" />

<!-- With custom items -->
<ContentList ContentItems="@myItems" />
```

Parameters:
- `Directory`: Filter by directory
- `Category`: Filter by category
- `Tag`: Filter by tag
- `FeaturedCount`: Show featured items with limit
- `ContentItems`: Use custom items instead of fetching

### ContentView

Displays a single content item.

```razor
<ContentView Path="blog/my-post.md" />
```

Parameters:
- `Path`: The path to the content item

### CategoriesList

Shows a list of categories with content counts.

```razor
<CategoriesList />
```

### TagCloud

Shows a cloud of tags with content counts.

```razor
<TagCloud MaxTags="20" />
```

Parameters:
- `MaxTags`: Maximum number of tags to display

### SearchBox

Provides a search input for content.

```razor
<SearchBox Placeholder="Search content..." />
```

Parameters:
- `Placeholder`: Placeholder text for the search input

### DirectoryNavigation

Creates a navigation menu based on directory structure.

```razor
<DirectoryNavigation CurrentDirectory="blog" />
```

Parameters:
- `CurrentDirectory`: The currently active directory

## Models

### ContentItem

Represents a content item from the repository.

Properties:
- `Id`: Unique identifier
- `Title`: Content title
- `Author`: Content author
- `Date`: Publication date
- `Description`: Brief description
- `Content`: HTML content converted from markdown
- `Tags`: List of tags
- `Categories`: List of categories
- `Slug`: URL-friendly identifier
- `IsFeatured`: Whether the content is featured
- `FeaturedImageUrl`: URL to featured image
- `GitHubFilePath`: Path to file in GitHub
- `Directory`: Directory containing the file
- `CreatedDate`: Date the content was created
- `LastUpdatedDate`: Date the content was last updated
- `ReadTimeMinutes`: Estimated reading time

### ContentCategory

Represents a content category.

Properties:
- `Name`: Category name
- `SlugUrl`: URL-friendly identifier
- `ContentCount`: Number of items in category

### ContentTag

Represents a content tag.

Properties:
- `Name`: Tag name
- `SlugUrl`: URL-friendly identifier
- `ContentCount`: Number of items with tag

## Services

### IGitHubCmsService

Interface for the GitHub CMS service.

Methods:
- `GetAllContentItemsAsync()`: Gets all content items
- `GetContentItemByPathAsync(string path)`: Gets a specific content item
- `GetContentItemsByDirectoryAsync(string directory)`: Gets items by directory
- `GetCategoriesAsync()`: Gets all categories
- `GetContentItemsByCategoryAsync(string category)`: Gets items by category
- `GetTagsAsync()`: Gets all tags
- `GetContentItemsByTagAsync(string tag)`: Gets items by tag
- `GetFeaturedContentItemsAsync(int count)`: Gets featured items
- `SearchContentItemsAsync(string query)`: Searches content items
- `RefreshCacheAsync()`: Refreshes the cache

## Configuration

Configure the GitHub CMS service in your `Program.cs`:

```csharp
builder.Services.AddGitHubCms(options =>
{
    options.Owner = "your-github-username";
    options.Repository = "your-content-repo";
    options.ContentPath = "content"; // Optional, defaults to root
    options.Branch = "main";
    options.ApiToken = "your-github-token"; // Optional, for private repos
    options.CacheDurationMinutes = 30;
    options.SupportedExtensions = new List<string> { ".md", ".markdown" };
});
```

Or using configuration:

```csharp
builder.Services.AddGitHubCms(builder.Configuration);
```

With corresponding `appsettings.json`:

```json
{
  "GitHubCms": {
    "Owner": "your-github-username",
    "Repository": "your-content-repo",
    "ContentPath": "content",
    "Branch": "main",
    "ApiToken": "your-github-token",
    "CacheDurationMinutes": 30
  }
}
```

## Examples

### Blog Layout

```razor
<div class="blog-layout">
    <header>
        <h1>My Blog</h1>
        <SearchBox Placeholder="Search blog..." />
    </header>
    
    <div class="content">
        <main>
            <ContentList Directory="blog" />
        </main>
        
        <aside>
            <h2>Categories</h2>
            <CategoriesList />
            
            <h2>Tags</h2>
            <TagCloud MaxTags="15" />
            
            <h2>Featured</h2>
            <ContentList FeaturedCount="3" />
        </aside>
    </div>
</div>
```

### Content Item Page

```razor
@page "/blog/{Slug}"

<ContentView Path="@($"blog/{Slug}.md")" />

@code {
    [Parameter]
    public string Slug { get; set; } = string.Empty;
}
```

### Search Results Page

```razor
@page "/search"
@inject IGitHubCmsService CmsService

<h1>Search Results</h1>

<SearchBox Placeholder="Search again..." />

@if (!string.IsNullOrEmpty(Query))
{
    <p>Results for "@Query":</p>
    <ContentList ContentItems="@Results" />
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

For more examples and detailed usage, refer to the [GitHub repository](https://github.com/obrana-boranija/Osirion.Blazor).

// content/docs/navigation.md
---
title: "Navigation Components"
author: "Dejan Demonjić"
date: "2025-04-14"
description: "Documentation for navigation components in Osirion.Blazor."
tags: [Navigation, Components, Documentation]
categories: [Documentation]
slug: "navigation-components"
is_featured: false
featured_image: "https://images.unsplash.com/photo-1545987796-200677ee1011?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
---

# Navigation Components

This document provides information about the navigation components available in Osirion.Blazor.

## EnhancedNavigationInterceptor

The `EnhancedNavigationInterceptor` component improves Blazor's navigation by automatically scrolling to the top of the page after navigation.

### Usage

```razor
@using Osirion.Blazor.Components.Navigation

<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

### Parameters

- `Behavior`: Controls the scrolling behavior
  - `ScrollBehavior.Auto`: Browser default
  - `ScrollBehavior.Instant`: Immediate scroll (no animation)
  - `ScrollBehavior.Smooth`: Animated scroll (recommended)

### Best Practices

1. Place the component in your `App.razor` or main layout component
2. Only include one instance in your application
3. Use `ScrollBehavior.Smooth` for most websites
4. Use `ScrollBehavior.Instant` for data-heavy applications or admin dashboards

### How It Works

The component injects a small JavaScript snippet that:
1. Listens for Blazor's `enhancedload` event
2. Detects when navigation has completed
3. Scrolls to the top of the page if the URL has changed

### Example

```razor
@using Osirion.Blazor.Components.Navigation

<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>

<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

## Troubleshooting

### Component Not Working

If the component doesn't work as expected, check:

1. Make sure you have Blazor with enhanced navigation enabled
2. Verify the component is placed in your App.razor or main layout
3. Check for JavaScript errors in the browser console
4. Ensure you only have one instance of the component in your application

### Browser Compatibility

The component should work in all modern browsers. If you encounter issues in specific browsers:

1. For older browsers, use `ScrollBehavior.Instant` instead of `ScrollBehavior.Smooth`
2. Check browser console for any JavaScript errors
3. Verify that the browser supports the Scroll Behavior API

## Conclusion

The `EnhancedNavigationInterceptor` component solves a common issue with Blazor's navigation by providing automatic scrolling to the top of the page. It enhances the user experience and makes navigation feel more natural.

For more information, refer to the [complete documentation](https://github.com/obrana-boranija/Osirion.Blazor).