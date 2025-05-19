# GitHub CMS Provider for Osirion.Blazor

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms)](https://www.nuget.org/packages/Osirion.Blazor.Cms)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

## Overview

The GitHub CMS Provider allows you to use a GitHub repository as a content management system for your Blazor application. This enables a Git-based workflow for content creation and management, making it ideal for static sites, blogs, documentation, and other content-driven applications.

## Features

- **Git-Based Workflow**: Use Git for content versioning, collaboration, and review
- **Markdown Support**: Write content in Markdown with frontmatter for metadata
- **Frontmatter Parsing**: Extract and use metadata from your content files
- **Hierarchical Organization**: Organize content in directories with nested structures
- **Tagging & Categorization**: Easily tag and categorize content
- **Full-Text Search**: Search across all your content
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Caching**: Built-in caching for improved performance
- **Automatic Updates**: Webhook support for content updates

## Installation

```bash
dotnet add package Osirion.Blazor.Cms
```

## Basic Configuration

Add the GitHub CMS provider to your services in `Program.cs`:

```csharp
using Osirion.Blazor.Cms.Extensions;

builder.Services.AddOsirionContent(content => {
    content.AddGitHub(options => {
        options.Owner = "username";
        options.Repository = "content-repo";
        options.ContentPath = "content";
        options.Branch = "main";
        options.UseCache = true;
        options.CacheExpirationMinutes = 60;
    });
});
```

## Content Structure

The GitHub CMS Provider expects your content to be structured in a specific way:

```
repository-root/
├── content/              # Base content directory (configurable)
│   ├── blog/             # Content category
│   │   ├── post1.md      # Content file
│   │   └── post2.md
│   ├── docs/
│   │   ├── getting-started.md
│   │   └── advanced/
│   │       └── custom-providers.md
│   └── tags/             # Special directory for tag definitions
│       ├── blazor.md
│       └── web.md
```

## Content Format

Content files should be written in Markdown with YAML frontmatter:

```markdown
---
title: "Getting Started with Osirion.Blazor"
date: "2025-04-20"
author: "Jane Developer"
description: "A comprehensive guide to getting started with Osirion.Blazor"
categories: [tutorials, beginner]
tags: [blazor, webassembly, ssr]
featured_image: "/images/getting-started.jpg"
is_featured: true
slug: "getting-started-osirion-blazor"
---

# Getting Started with Osirion.Blazor

This is the content of your markdown file...
```

## Basic Usage

### Display a List of Content Items

```razor
@using Osirion.Blazor.Cms.Components

<ContentList 
    Directory="blog" 
    SortBy="SortField.Date" 
    SortDirection="SortDirection.Descending" 
    Count="10" />
```

### Display a Single Content Item

```razor
<ContentView Path="blog/my-post.md" />
```

### Adding Search

```razor
<SearchBox Placeholder="Search..." />

<!-- Display search results -->
<ContentList Query="@searchQuery" />

@code {
    private string? searchQuery;

    private void HandleSearch(string query)
    {
        searchQuery = query;
    }
}
```

### Displaying Categories and Tags

```razor
<CategoriesList />
<TagCloud MaxTags="20" />
```

### Navigation Based on Repository Structure

```razor
<DirectoryNavigation />
```

## Advanced Configuration

### Authentication

For private repositories, configure authentication:

```csharp
content.AddGitHub(options => {
    options.Owner = "username";
    options.Repository = "content-repo";
    options.Authentication = new GitHubAuthOptions {
        PersonalAccessToken = "your-pat-token"
    };
});
```

### Webhook Configuration

To automatically update content when changes are pushed to your repository:

```csharp
content.AddGitHub(options => {
    options.Owner = "username";
    options.Repository = "content-repo";
    options.EnableWebhooks = true;
    options.WebhookSecret = "your-webhook-secret"; // Set this in GitHub webhook settings
});
```

Then configure the webhook endpoint in your `app.UseEndpoints` configuration:

```csharp
app.UseEndpoints(endpoints => {
    endpoints.MapGitHubWebhook("/api/content-webhook");
});
```

### Content Transformation

You can register custom content transformers to modify content before rendering:

```csharp
content.AddGitHub(options => {
    options.Owner = "username";
    options.Repository = "content-repo";
    
    options.ContentTransformers.Add(new CustomLinkTransformer());
});
```

Implementing a custom transformer:

```csharp
public class CustomLinkTransformer : IContentTransformer
{
    public Task<string> TransformAsync(string content, ContentItem item)
    {
        // Modify content, for example, make all links open in a new tab
        return Task.FromResult(content.Replace("<a href", "<a target=\"_blank\" href"));
    }
}
```

## Using the ContentProvider Service

For programmatic access to content, inject `IContentProvider`:

```csharp
@inject IContentProvider ContentProvider

@code {
    private List<ContentItem> recentPosts = new();
    
    protected override async Task OnInitializedAsync()
    {
        var query = new ContentQuery
        {
            Directory = "blog",
            SortBy = SortField.Date,
            SortDirection = SortDirection.Descending,
            Count = 5
        };
        
        var result = await ContentProvider.GetItemsByQueryAsync(query);
        recentPosts = result.ToList();
    }
}
```

## Content Query API

The `ContentQuery` class provides a powerful way to filter and search content:

```csharp
var query = new ContentQuery
{
    // Filter by directory
    Directory = "blog",
    
    // Filter by metadata properties
    Category = "tutorials",
    Tag = "blazor",
    Author = "Jane Developer",
    
    // Full-text search
    SearchQuery = "blazor components",
    
    // Date filtering
    FromDate = new DateTime(2025, 1, 1),
    ToDate = DateTime.Now,
    
    // Limit and sort
    Count = 10,
    Skip = 0,
    SortBy = SortField.Date,
    SortDirection = SortDirection.Descending,
    
    // Special filters
    IsFeatured = true,
    HasFeaturedImage = true
};
```

## Rendering Markdown Content

Osirion.Blazor.Cms includes components for rendering Markdown:

```razor
<MarkdownRenderer Content="@markdownContent" />
```

To customize Markdown rendering:

```csharp
builder.Services.Configure<MarkdownOptions>(options => {
    options.UseEmphasisExtras = true;
    options.UseSmartyPants = true;
    options.UseTaskLists = true;
    options.UseMediaLinks = true;
    options.UseCustomContainers = true;
});
```

## SEO Optimization

The CMS includes components for SEO optimization:

```razor
<SeoMetadataRenderer 
    Content="@currentContent" 
    SiteName="My Website" 
    BaseUrl="https://mysite.com" 
    DefaultImage="/images/default-social.jpg" />
```

## Multi-tenant Support

For applications that serve content for multiple tenants:

```csharp
services.AddOsirionContent(content => {
    // Primary tenant
    content.AddGitHub(options => {
        options.Owner = "username";
        options.Repository = "main-content";
        options.ProviderId = "main";
    });
    
    // Secondary tenant
    content.AddGitHub(options => {
        options.Owner = "username";
        options.Repository = "secondary-content";
        options.ProviderId = "secondary";
    });
});
```

Then in your components, specify which provider to use:

```razor
<ContentList ProviderId="secondary" Directory="blog" />
```

## Repository Structure Recommendations

To get the most out of the GitHub CMS, we recommend the following structure:

```
repository-root/
├── content/
│   ├── blog/                  # Blog posts
│   │   ├── 2025/              # Organize by year
│   │   │   ├── 01/            # And month
│   │   │   │   ├── post1.md
│   │   │   │   └── post2.md
│   ├── pages/                 # Static pages
│   │   ├── about.md
│   │   ├── contact.md
│   │   └── legal/
│   │       ├── privacy.md
│   │       └── terms.md
│   ├── docs/                  # Documentation
│   │   ├── getting-started.md
│   │   └── advanced-topics/
│   │       └── custom-providers.md
│   ├── tags/                  # Tag definitions
│   │   ├── blazor.md
│   │   └── web.md
│   └── categories/            # Category definitions
│       ├── tutorials.md
│       └── news.md
├── assets/                    # Static assets
│   ├── images/
│   │   └── featured/
│   │       └── getting-started.jpg
```

## Performance Considerations

- **Use Caching**: Enable the built-in cache for optimal performance
- **Optimize Image Size**: Use appropriately sized images for your content
- **Limit Content**: Use pagination or limits when displaying content lists
- **Use Webhooks**: Set up webhooks to keep your content up-to-date without constant API calls

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.