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