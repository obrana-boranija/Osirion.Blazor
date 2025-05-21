---
title: "Building a Blog with GitHub CMS and Blazor"
author: "Dejan Demonjić"
date: "2025-04-15"
description: "Learn how to create a fast, SEO-friendly blog using Osirion.Blazor's GitHub CMS components and static Blazor."
tags: [blazor, github, cms, static-site, tutorial]
categories: [tutorials, blazor]
slug: "github-cms-with-blazor"
is_featured: true
featured_image: "https://example.com/images/github-cms-blazor.jpg"
---

# Building a Blog with GitHub CMS and Blazor

In this tutorial, we'll build a fast, SEO-friendly blog using Osirion.Blazor's GitHub CMS components and static Blazor. This approach gives you the benefits of a static site (speed, security, SEO) with the development experience of Blazor.

## Why GitHub as a CMS?

Using GitHub as a content management system offers several advantages:

1. **Version Control**: Your content benefits from Git's version control
2. **Markdown Support**: Write content in Markdown with frontmatter
3. **Collaboration**: Use GitHub's collaboration features (PRs, issues)
4. **No Database**: No need for a separate database
5. **Free Hosting**: Host content for free on GitHub
6. **Content as Code**: Treat your content the same way you treat your code

## Prerequisites

- .NET 8 SDK or newer
- A GitHub account
- Basic knowledge of Blazor
- Visual Studio or VS Code

## Step 1: Create a New Blazor Project

Start by creating a new Blazor Web App with the .NET CLI:

```bash
dotnet new blazor -o GitHubCmsBlog
cd GitHubCmsBlog
```

## Step 2: Add Osirion.Blazor Package

Add the Osirion.Blazor package to your project:

```bash
dotnet add package Osirion.Blazor --version 1.3.0
```

## Step 3: Configure GitHub CMS

1. Create a GitHub repository for your content
2. Add some Markdown files with frontmatter (see example below)
3. Configure the GitHub CMS service in `Program.cs`:

```csharp
builder.Services.AddGitHubCms(options =>
{
    options.Owner = "your-github-username";
    options.Repository = "your-content-repo";
    options.ContentPath = "content";
    options.Branch = "main";
    // Add an API token for private repos
    // options.ApiToken = "your-github-token";
});
```

## Step 4: Update _Imports.razor

Add the necessary using statements to your `_Imports.razor` file:

```razor
@using Osirion.Blazor.Components.GitHubCms
@using Osirion.Blazor.Services.GitHub
@using Osirion.Blazor.Models.Cms
```

## Step 5: Create Blog Layout

Create a new layout for your blog in `Components/Layout/BlogLayout.razor`:

```razor
@inherits LayoutComponentBase

<div class="blog-layout">
    <header>
        <h1><a href="/">My GitHub CMS Blog</a></h1>
        <SearchBox Placeholder="Search posts..." />
    </header>

    <div class="content">
        <main>
            @Body
        </main>
        
        <aside>
            <h2>Categories</h2>
            <CategoriesList />
            
            <h2>Tags</h2>
            <TagCloud MaxTags="20" />
            
            <h2>Navigation</h2>
            <DirectoryNavigation />
        </aside>
    </div>
    
    <footer>
        <p>&copy; 2025 My GitHub CMS Blog</p>
    </footer>
</div>

<!-- Add default Osirion styling -->
<OsirionStyles CustomVariables="
    --osirion-primary-color: #3b82f6;
    --osirion-color-background: #f8fafc;
" />
```

## Step 6: Create Blog Home Page

Update your `Components/Pages/Home.razor` file:

```razor
@page "/"
@using Osirion.Blazor.Components.GitHubCms

<PageTitle>My GitHub CMS Blog</PageTitle>

<h1>Latest Posts</h1>

<ContentList Directory="blog" />
```

## Step 7: Create Blog Post Page

Create a new file `Components/Pages/BlogPost.razor`:

```razor
@page "/blog/{Slug}"
@inject IGitHubCmsService CmsService
@using Osirion.Blazor.Components.GitHubCms
@using Osirion.Blazor.Models.Cms

<PageTitle>@(ContentItem?.Title ?? "Loading...")</PageTitle>

@if (IsLoading)
{
    <p>Loading post...</p>
}
else if (ContentItem == null)
{
    <p>Post not found.</p>
}
else
{
    <!-- Set metadata for SEO -->
    <HeadContent>
        <meta name="description" content="@ContentItem.Description" />
        <meta property="og:title" content="@ContentItem.Title" />
        <meta property="og:description" content="@ContentItem.Description" />
        @if (!string.IsNullOrEmpty(ContentItem.FeaturedImageUrl))
        {
            <meta property="og:image" content="@ContentItem.FeaturedImageUrl" />
        }
    </HeadContent>
    
    <ContentView Path="@($"blog/{Slug}.md")" />
}

@code {
    [Parameter]
    public string Slug { get; set; } = string.Empty;
    
    private bool IsLoading { get; set; } = true;
    private ContentItem? ContentItem { get; set; }
    
    protected override async Task OnParametersSetAsync()
    {
        IsLoading = true;
        
        try
        {
            var path = $"blog/{Slug}.md";
            ContentItem = await CmsService.GetContentItemByPathAsync(path);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading post: {ex.Message}");
            ContentItem = null;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

## Step 8: Create Category and Tag Pages

Create a `Components/Pages/Category.razor` file:

```razor
@page "/category/{CategorySlug}"
@using Osirion.Blazor.Components.GitHubCms

<PageTitle>Category: @CategorySlug</PageTitle>

<h1>Posts in @CategorySlug</h1>

<ContentList Category="@CategorySlug" />

@code {
    [Parameter]
    public string CategorySlug { get; set; } = string.Empty;
}
```

Create a `Components/Pages/Tag.razor` file:

```razor
@page "/tag/{TagSlug}"
@using Osirion.Blazor.Components.GitHubCms

<PageTitle>Tag: @TagSlug</PageTitle>

<h1>Posts tagged with @TagSlug</h1>

<ContentList Tag="@TagSlug" />

@code {
    [Parameter]
    public string TagSlug { get; set; } = string.Empty;
}
```

## Step 9: Create Search Results Page

Create a `Components/Pages/Search.razor` file:

```razor
@page "/search"
@inject IGitHubCmsService CmsService
@using Osirion.Blazor.Components.GitHubCms
@using Osirion.Blazor.Models.Cms

<PageTitle>Search Results: @Query</PageTitle>

<h1>Search Results for "@Query"</h1>

@if (IsLoading)
{
    <p>Searching...</p>
}
else if (Results.Count == 0)
{
    <p>No results found for "@Query"</p>
}
else
{
    <ContentList ContentItems="@Results" />
}

@code {
    [Parameter]
    [SupplyParameterFromQuery(Name = "q")]
    public string Query { get; set; } = string.Empty;
    
    private bool IsLoading { get; set; } = true;
    private List<ContentItem> Results { get; set; } = new();
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Query))
        {
            IsLoading = true;
            Results = await CmsService.SearchContentItemsAsync(Query);
            IsLoading = false;
        }
    }
}
```

## Step 10: Add Styling

Add some CSS to `wwwroot/app.css`:

```css
.blog-layout {
    max-width: 1200px;
    margin: 0 auto;
    padding: 1rem;
}

.content {
    display: grid;
    grid-template-columns: 3fr 1fr;
    gap: 2rem;
}

header {
    margin-bottom: 2rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid var(--osirion-color-border);
    display: flex;
    justify-content: space-between;
    align-items: center;
}

aside {
    padding: 1rem;
    background-color: var(--osirion-color-background-light);
    border-radius: var(--osirion-border-radius);
}

footer {
    margin-top: 2rem;
    padding-top: 1rem;
    border-top: 1px solid var(--osirion-color-border);
    text-align: center;
}

@media (max-width: 768px) {
    .content {
        grid-template-columns: 1fr;
    }
    
    header {
        flex-direction: column;
        gap: 1rem;
    }
}
```

## Step 11: Run the Application

Start the application:

```bash
dotnet run
```

Visit https://localhost:5001 to see your blog!

## Example Content Format

Here's an example of a blog post markdown file with frontmatter:

```markdown
---
title: "Getting Started with Blazor"
author: "John Doe"
date: "2025-04-10"
description: "A beginner's guide to getting started with Blazor web development."
tags: [blazor, webassembly, dotnet]
categories: [tutorials, web]
slug: "getting-started-with-blazor"
is_featured: true
featured_image: "https://example.com/images/blazor.jpg"
---

# Getting Started with Blazor

Blazor is a framework for building interactive web UIs using C# instead of JavaScript.

## What is Blazor?

Blazor lets you build interactive web UIs using C#...
```

## Conclusion

In this tutorial, we've built a blog using Osirion.Blazor's GitHub CMS components. This approach provides several benefits:

- **Performance**: Static Blazor apps are fast and SEO-friendly
- **Simplicity**: No database to manage
- **Developer Experience**: Full C# and Blazor development experience
- **Content Management**: Easy content creation and editing with markdown
- **Customization**: Full control over styling and layout

The GitHub CMS approach is particularly well-suited for developer blogs, documentation sites, and small to medium-sized content websites. For larger sites with many editors, you might want to consider adding a custom editor UI or integrating with a headless CMS.

Happy coding!