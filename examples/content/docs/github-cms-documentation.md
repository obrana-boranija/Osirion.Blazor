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