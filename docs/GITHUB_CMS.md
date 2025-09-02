# GitHub CMS Guide

[![Documentation](https://img.shields.io/badge/Documentation-GitHub_CMS-blue)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/GITHUB_CMS.md)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor.Cms)](https://www.nuget.org/packages/Osirion.Blazor.Cms)

Comprehensive guide for using GitHub as a content management system with Osirion.Blazor.

## Table of Contents

1. [Overview](#overview)
2. [Getting Started](#getting-started)
3. [Repository Setup](#repository-setup)
4. [Content Structure](#content-structure)
5. [Frontmatter Reference](#frontmatter-reference)
6. [Content Components](#content-components)
7. [Advanced Features](#advanced-features)
8. [SEO and Metadata](#seo-and-metadata)
9. [Multi-language Support](#multi-language-support)
10. [Admin Interface](#admin-interface)
11. [Webhooks and Automation](#webhooks-and-automation)
12. [Best Practices](#best-practices)

## Overview

The GitHub CMS provider transforms any GitHub repository into a powerful content management system for your Blazor application. This approach offers several advantages:

### Benefits of Git-Based CMS

- **Version Control**: Complete history of all content changes
- **Collaboration**: Multiple authors can contribute using Git workflows
- **Review Process**: Content reviews through pull requests
- **Backup**: Content is automatically backed up and distributed
- **Developer-Friendly**: Use familiar Git tools and workflows
- **Cost-Effective**: Leverage existing GitHub infrastructure

### Key Features

- **Markdown Support**: Write content in Markdown with frontmatter
- **Automatic Discovery**: Automatically discovers and indexes content
- **Search and Filtering**: Full-text search and metadata filtering
- **Category and Tag Support**: Organize content with categories and tags
- **SEO Optimization**: Automatic sitemap and metadata generation
- **Caching**: Built-in caching for optimal performance
- **Multi-language**: Support for localized content

## Getting Started

### Installation

Install the required packages:

```bash
# Core CMS functionality
dotnet add package Osirion.Blazor.Cms

# Or install the complete package
dotnet add package Osirion.Blazor
```

### Basic Configuration

Configure the GitHub CMS provider in your `Program.cs`:

```csharp
using Osirion.Blazor.Cms.Extensions;

builder.Services.AddOsirionBlazor(osirion => {
    osirion.AddGitHubCms(options => {
        options.Owner = "your-username";
        options.Repository = "your-content-repo";
        options.ContentPath = "content";
        options.Branch = "main";
        options.ApiToken = "your-github-token"; // Optional for public repos
    });
});
```

### Add Component Imports

Add to your `_Imports.razor`:

```razor
@using Osirion.Blazor.Cms.Components
```

## Repository Setup

### Create Content Repository

1. **Create a new GitHub repository** for your content
2. **Add a content directory** (default: `content`)
3. **Create initial content structure**

Example repository structure:

```
your-content-repo/
??? content/
?   ??? blog/                  # Blog posts directory
?   ?   ??? 2024/              # Year-based organization
?   ?   ?   ??? 01/            # Month subdirectories
?   ?   ?   ?   ??? getting-started.md
?   ?   ?   ?   ??? advanced-tips.md
?   ?   ?   ??? 02/
?   ?   ?       ??? new-features.md
?   ?   ??? categories/        # Category definitions
?   ?       ??? tutorials.md
?   ?       ??? announcements.md
?   ??? pages/                 # Static pages
?   ?   ??? about.md
?   ?   ??? contact.md
?   ?   ??? privacy-policy.md
?   ??? docs/                  # Documentation
?   ?   ??? getting-started.md
?   ?   ??? api-reference.md
?   ??? tags/                  # Tag definitions
?       ??? blazor.md
?       ??? web-development.md
??? assets/                    # Static assets
?   ??? images/
?   ?   ??? featured/          # Featured images
?   ?   ??? blog/              # Blog images
?   ??? documents/
??? README.md
```

### Authentication Setup

For private repositories or higher API limits, create a GitHub Personal Access Token:

1. Go to GitHub Settings ? Developer settings ? Personal access tokens
2. Generate a new token with `repo` scope
3. Add the token to your configuration:

```csharp
options.ApiToken = "ghp_your_token_here";
```

Or use environment variables:

```bash
export GITHUB_TOKEN="ghp_your_token_here"
```

```csharp
options.ApiToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
```

## Content Structure

### File Naming Conventions

- Use lowercase with hyphens: `getting-started.md`
- Include dates for blog posts: `2024-01-15-new-features.md`
- Use descriptive, SEO-friendly names
- Keep file names under 50 characters

### Directory Organization

#### By Category
```
content/
??? tutorials/
??? announcements/
??? case-studies/
??? product-updates/
```

#### By Date (for blogs)
```
content/blog/
??? 2024/
?   ??? 01/
?   ??? 02/
?   ??? 03/
??? 2023/
    ??? 11/
    ??? 12/
```

#### Hybrid Approach
```
content/
??? blog/
?   ??? 2024/01/
?   ??? 2024/02/
??? docs/
?   ??? getting-started/
?   ??? advanced/
??? pages/
    ??? legal/
    ??? company/
```

## Frontmatter Reference

Frontmatter is YAML metadata at the top of your Markdown files:

### Required Fields

```yaml
---
title: "Your Article Title"
date: "2024-01-15"
---
```

### Complete Example

```yaml
---
# Basic Information
title: "Complete Guide to Osirion.Blazor"
subtitle: "Building Modern Web Applications"
description: "Learn how to build powerful Blazor applications with Osirion.Blazor components and GitHub CMS."

# Publishing Information
date: "2024-01-15T10:00:00Z"
lastModified: "2024-01-20T15:30:00Z"
author: "Jane Developer"
authorEmail: "jane@example.com"
authorBio: "Senior Developer at Example Corp"

# Content Organization
categories: [tutorials, web-development]
tags: [blazor, cms, github, web]
series: "Osirion.Blazor Tutorial Series"
seriesOrder: 1

# Publishing Status
published: true
featured: true
draft: false

# SEO and Social
slug: "complete-guide-osirion-blazor"
excerpt: "A comprehensive guide covering everything you need to know about building applications with Osirion.Blazor."
keywords: [blazor, cms, github, tutorial]
canonicalUrl: "https://mysite.com/blog/complete-guide-osirion-blazor"

# Media
featuredImage: "/assets/images/featured/osirion-guide.jpg"
featuredImageAlt: "Osirion.Blazor Components Diagram"
socialImage: "/assets/images/social/osirion-guide-social.jpg"

# Content Metadata
readingTime: "15 min"
difficulty: "intermediate"
audience: "developers"

# Custom Fields
customField: "custom value"
metadata:
  version: "1.0"
  lastReviewed: "2024-01-20"
  reviewedBy: "Tech Team"
---
```

### Field Descriptions

| Field | Type | Description | Required |
|-------|------|-------------|----------|
| `title` | String | Article title | ? |
| `date` | DateTime | Publication date | ? |
| `description` | String | Meta description | Recommended |
| `author` | String | Author name | Recommended |
| `categories` | Array | Content categories | Optional |
| `tags` | Array | Content tags | Optional |
| `published` | Boolean | Publishing status | Optional (default: true) |
| `featured` | Boolean | Featured content flag | Optional |
| `draft` | Boolean | Draft status | Optional |
| `slug` | String | URL slug | Optional (auto-generated) |
| `excerpt` | String | Content summary | Optional |
| `featuredImage` | String | Featured image URL | Optional |
| `readingTime` | String | Estimated reading time | Optional |

## Content Components

### Displaying Content Lists

#### Basic Content List
```razor
<ContentList Directory="blog" Count="10" />
```

#### Filtered Content List
```razor
<ContentList 
    Directory="blog"
    Category="tutorials"
    Count="5"
    SortBy="SortField.Date"
    SortDirection="SortDirection.Descending"
    ShowExcerpt="true"
    ShowMetadata="true" />
```

#### Featured Content
```razor
<ContentList 
    FeaturedCount="3"
    ShowExcerpt="true"
    ShowMetadata="true" />
```

### Displaying Individual Content

#### Content View
```razor
<ContentView Path="blog/getting-started.md" />
```

#### Content with Custom Layout
```razor
<ContentView Path="blog/getting-started.md">
    <HeaderTemplate Context="content">
        <div class="custom-header">
            <h1>@content.Title</h1>
            <p class="lead">@content.Excerpt</p>
        </div>
    </HeaderTemplate>
    
    <FooterTemplate Context="content">
        <div class="custom-footer">
            <p>Published on @content.PublishDate?.ToString("MMMM dd, yyyy")</p>
        </div>
    </FooterTemplate>
</ContentView>
```

### Navigation and Discovery

#### Categories List
```razor
<CategoriesList />

<!-- With custom formatting -->
<CategoriesList>
    <CategoryTemplate Context="category">
        <div class="category-card">
            <h5>@category.Name</h5>
            <span class="badge">@category.Count</span>
        </div>
    </CategoryTemplate>
</CategoriesList>
```

#### Tag Cloud
```razor
<TagCloud MaxTags="20" />

<!-- With size variation -->
<TagCloud 
    MaxTags="30"
    ShowCounts="true"
    SizeVariation="true" />
```

#### Search Box
```razor
<SearchBox 
    Placeholder="Search articles..."
    MinSearchLength="3"
    MaxResults="10"
    OnSearch="@HandleSearch" />

@code {
    private void HandleSearch(string query)
    {
        // Handle search results
        NavigationManager.NavigateTo($"/search?q={Uri.EscapeDataString(query)}");
    }
}
```

#### Directory Navigation
```razor
<DirectoryNavigation CurrentDirectory="blog" />

<!-- With custom structure -->
<DirectoryNavigation CurrentDirectory="docs">
    <DirectoryTemplate Context="directory">
        <div class="nav-directory">
            <i class="fas fa-folder"></i>
            <a href="/docs/@directory.Path">@directory.Name</a>
            <span class="count">(@directory.ItemCount)</span>
        </div>
    </DirectoryTemplate>
</DirectoryNavigation>
```

## Advanced Features

### Content Querying

#### Programmatic Content Access
```csharp
@inject IContentProvider ContentProvider

@code {
    private List<ContentItem> articles = new();
    
    protected override async Task OnInitializedAsync()
    {
        var query = new ContentQuery
        {
            Directory = "blog",
            Category = "tutorials",
            Tag = "blazor",
            SearchQuery = "getting started",
            FromDate = new DateTime(2024, 1, 1),
            ToDate = DateTime.Now,
            Count = 10,
            Skip = 0,
            SortBy = SortField.Date,
            SortDirection = SortDirection.Descending,
            IsFeatured = true
        };
        
        articles = (await ContentProvider.GetItemsByQueryAsync(query)).ToList();
    }
}
```

#### Complex Queries
```csharp
// Get recent articles in multiple categories
var query = new ContentQuery
{
    Categories = new[] { "tutorials", "announcements" },
    FromDate = DateTime.Now.AddDays(-30),
    SortBy = SortField.Date,
    SortDirection = SortDirection.Descending
};

// Get popular articles by tags
var popularQuery = new ContentQuery
{
    Tags = new[] { "blazor", "popular" },
    IsFeatured = true,
    Count = 5
};
```

### Content Transformation

#### Custom Content Processors
```csharp
public class CodeHighlightProcessor : IContentTransformer
{
    public async Task<string> TransformAsync(string content, ContentItem item)
    {
        // Add syntax highlighting to code blocks
        return content.Replace("```csharp", "```csharp class=\"language-csharp\"");
    }
}

// Register the processor
builder.Services.AddScoped<IContentTransformer, CodeHighlightProcessor>();
```

#### Link Processing
```csharp
public class InternalLinkProcessor : IContentTransformer
{
    public async Task<string> TransformAsync(string content, ContentItem item)
    {
        // Convert relative links to absolute
        var pattern = @"\[([^\]]+)\]\((?!http)([^)]+)\)";
        return Regex.Replace(content, pattern, 
            match => $"[{match.Groups[1].Value}](/content/{match.Groups[2].Value})");
    }
}
```

### Caching Strategies

#### Configure Caching
```csharp
builder.Services.AddOsirionBlazor(osirion => {
    osirion.AddGitHubCms(options => {
        options.UseCache = true;
        options.CacheExpirationMinutes = 60;
        options.SlidingExpiration = TimeSpan.FromMinutes(15);
        options.CacheSize = 100; // Number of items
    });
});
```

#### Custom Cache Provider
```csharp
public class RedisCacheProvider : IContentCacheProvider
{
    private readonly IDistributedCache cache;
    
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var json = await cache.GetStringAsync(key);
        return json != null ? JsonSerializer.Deserialize<T>(json) : null;
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
    {
        var json = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });
    }
}
```

## SEO and Metadata

### Automatic SEO Metadata

```razor
<SeoMetadataRenderer 
    Content="@currentContent"
    SiteName="My Website"
    BaseUrl="https://mysite.com"
    DefaultImage="/images/default-social.jpg"
    TwitterHandle="@myhandle" />
```

### Custom Meta Tags

```csharp
public class CustomSeoProcessor : ISeoProcessor
{
    public void ProcessSeoData(ContentItem content, SeoData seoData)
    {
        // Add custom OpenGraph tags
        seoData.OpenGraph["article:author"] = content.Author;
        seoData.OpenGraph["article:published_time"] = content.PublishDate?.ToString("yyyy-MM-dd");
        
        // Add structured data
        seoData.StructuredData = new
        {
            Type = "Article",
            Headline = content.Title,
            Author = new { Name = content.Author },
            DatePublished = content.PublishDate,
            Image = content.FeaturedImage
        };
    }
}
```

### Sitemap Generation

```csharp
public class SitemapService
{
    public async Task<string> GenerateSitemapAsync()
    {
        var content = await contentProvider.GetAllItemsAsync();
        var sitemap = new XDocument(
            new XElement("urlset",
                new XAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9"),
                content.Select(item => new XElement("url",
                    new XElement("loc", $"https://mysite.com/{item.Slug}"),
                    new XElement("lastmod", item.LastModified?.ToString("yyyy-MM-dd")),
                    new XElement("changefreq", "weekly"),
                    new XElement("priority", item.IsFeatured ? "0.9" : "0.7")
                ))
            )
        );
        
        return sitemap.ToString();
    }
}
```

## Multi-language Support

### Enable Localization

```csharp
builder.Services.AddOsirionBlazor(osirion => {
    osirion.AddGitHubCms(options => {
        options.EnableLocalization = true;
        options.DefaultLocale = "en";
        options.SupportedLocales = new[] { "en", "es", "fr", "de" };
    });
});
```

### Content Structure for Localization

```
content/
??? en/                    # English content
?   ??? blog/
?   ?   ??? getting-started.md
?   ??? pages/
?       ??? about.md
??? es/                    # Spanish content
?   ??? blog/
?   ?   ??? comenzando.md
?   ??? pages/
?       ??? acerca-de.md
??? fr/                    # French content
    ??? blog/
    ?   ??? commencer.md
    ??? pages/
        ??? a-propos.md
```

### Localized Content Components

```razor
<LocalizedContentView 
    Path="@contentPath"
    CurrentLocale="@currentLocale"
    FallbackLocale="en" />

<LocalizedNavigation 
    CurrentLocale="@currentLocale"
    OnLocaleChanged="@HandleLocaleChange" />

@code {
    private string currentLocale = "en";
    
    private async Task HandleLocaleChange(string newLocale)
    {
        currentLocale = newLocale;
        await LocalStorage.SetItemAsync("locale", newLocale);
        NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
    }
}
```

## Admin Interface

### Enable Admin Interface

```csharp
// Add admin services
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddGitHubCms(options => { ... })
        .AddCmsAdmin(options => {
            options.EnableAdmin = true;
            options.AdminPath = "/admin";
            options.RequireAuthentication = true;
        });
});
```

### Admin Components

#### Markdown Editor
```razor
<MarkdownEditorPreview 
    Content="@content"
    ContentChanged="@OnContentChanged"
    ShowToolbar="true"
    SyncScroll="true"
    AutoSave="true"
    AutoSaveInterval="30000" />

@code {
    private string content = "";
    
    private async Task OnContentChanged(string newContent)
    {
        content = newContent;
        // Auto-save logic
        await SaveDraftAsync(newContent);
    }
}
```

#### Content Manager
```razor
<ContentManager 
    AllowCreate="true"
    AllowEdit="true"
    AllowDelete="true"
    ShowDrafts="true">
    
    <ContentListTemplate Context="items">
        @foreach (var item in items)
        {
            <ContentManagerCard 
                Item="@item"
                OnEdit="@(() => EditContent(item))"
                OnDelete="@(() => DeleteContent(item))" />
        }
    </ContentListTemplate>
</ContentManager>
```

### Authentication Integration

```csharp
// Configure authentication
builder.Services.AddAuthentication("GitHub")
    .AddGitHub(options => {
        options.ClientId = "your-github-app-id";
        options.ClientSecret = "your-github-app-secret";
    });

// Authorize admin area
app.MapRazorPages().RequireAuthorization();
app.MapBlazorHub().RequireAuthorization();
```

## Webhooks and Automation

### Setup GitHub Webhooks

1. **Configure Webhook URL** in your GitHub repository settings
2. **Set Content-Type** to `application/json`
3. **Select Events**: Push events, Pull request events
4. **Add Secret** for security

```csharp
// Configure webhook handling
builder.Services.AddOsirionBlazor(osirion => {
    osirion.AddGitHubCms(options => {
        options.EnableWebhooks = true;
        options.WebhookSecret = "your-webhook-secret";
        options.WebhookPath = "/api/github-webhook";
    });
});

// Add webhook endpoint
app.MapPost("/api/github-webhook", async (HttpContext context, IGitHubWebhookHandler handler) =>
{
    var result = await handler.HandleWebhookAsync(context.Request);
    return result ? Results.Ok() : Results.BadRequest();
});
```

### Webhook Handler

```csharp
public class CustomWebhookHandler : IGitHubWebhookHandler
{
    public async Task<bool> HandleWebhookAsync(HttpRequest request)
    {
        var eventType = request.Headers["X-GitHub-Event"].ToString();
        
        return eventType switch
        {
            "push" => await HandlePushEvent(request),
            "pull_request" => await HandlePullRequest(request),
            _ => true // Ignore other events
        };
    }
    
    private async Task<bool> HandlePushEvent(HttpRequest request)
    {
        // Clear cache, rebuild content index, etc.
        await contentCache.ClearAsync();
        await contentIndexer.RebuildIndexAsync();
        return true;
    }
}
```

### Automated Workflows

#### GitHub Actions for Content Processing

```yaml
# .github/workflows/content-processing.yml
name: Process Content Updates

on:
  push:
    paths:
      - 'content/**'

jobs:
  process-content:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Optimize Images
        run: |
          # Optimize images in content
          find content -name "*.jpg" -o -name "*.png" | xargs -I {} sh -c 'imagemin {} --out-dir={}'
      
      - name: Generate Thumbnails
        run: |
          # Generate thumbnails for featured images
          ./scripts/generate-thumbnails.sh
      
      - name: Trigger Cache Refresh
        run: |
          curl -X POST "https://mysite.com/api/refresh-cache" \
            -H "Authorization: Bearer ${{ secrets.API_TOKEN }}"
```

## Best Practices

### Content Organization

1. **Use Consistent Naming Conventions**
   - Lowercase with hyphens for files and directories
   - Date prefixes for time-sensitive content
   - Descriptive, SEO-friendly names

2. **Organize by Purpose and Audience**
   - Separate blogs, documentation, and pages
   - Use categories and tags consistently
   - Create clear content hierarchies

3. **Optimize for Performance**
   - Keep images optimized and appropriately sized
   - Use efficient directory structures
   - Implement appropriate caching strategies

### Content Writing

1. **Write Good Frontmatter**
   - Include all relevant metadata
   - Use consistent field names
   - Validate YAML syntax

2. **Structure Content Well**
   - Use clear headings and sections
   - Include table of contents for long articles
   - Add relevant internal and external links

3. **Optimize for SEO**
   - Write descriptive titles and meta descriptions
   - Use relevant keywords naturally
   - Include alt text for images

### Development Workflow

1. **Use Branching Strategy**
   - Create feature branches for content changes
   - Use pull requests for content review
   - Maintain staging and production branches

2. **Implement Content Review Process**
   - Require reviews for content changes
   - Use GitHub's review features
   - Test content in staging environment

3. **Monitor Performance**
   - Track content loading times
   - Monitor API rate limits
   - Set up alerts for failures

### Security Considerations

1. **Protect API Tokens**
   - Store tokens as environment variables
   - Use minimal required permissions
   - Rotate tokens regularly

2. **Validate Webhook Payloads**
   - Verify webhook signatures
   - Validate payload structure
   - Log security events

3. **Sanitize Content**
   - Enable HTML sanitization
   - Validate frontmatter data
   - Escape user-generated content

This comprehensive guide covers all aspects of using GitHub as a CMS with Osirion.Blazor. For additional help or advanced use cases, consult the [API Reference](API_REFERENCE.md) or reach out to the community.