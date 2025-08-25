---
title: "Getting Started with Osirion.Blazor v1.5"
author: "Dejan Demonjić"
date: "2025-01-25"
description: "Complete guide to getting started with Osirion.Blazor v1.5, featuring the new fluent API, core components, and comprehensive examples for building modern Blazor applications."
tags: [Getting Started, Tutorial, Components, Setup]
categories: [Documentation, Tutorial]
slug: "getting-started-with-osirion-blazor"
is_featured: true
featured_image: "https://images.unsplash.com/photo-1517077304055-6e89abbf09b0?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
seo_properties:
  title: "Getting Started with Osirion.Blazor v1.5 - Complete Tutorial"
  description: "Learn how to build modern Blazor applications with Osirion.Blazor v1.5, featuring the new fluent API and comprehensive component library."
  og_image_url: "https://images.unsplash.com/photo-1517077304055-6e89abbf09b0?ixlib=rb-4.0.3&auto=format&fit=crop&w=1200&q=80"
  type: "Article"
---

# Getting Started with Osirion.Blazor v1.5

Osirion.Blazor v1.5 represents a significant evolution in building modern Blazor applications. With a comprehensive component library, enhanced developer experience, and seamless CSS framework integration, you can build professional applications faster than ever before.

## What's New in v1.5

### 🚀 Fluent API for Service Registration

The new fluent API makes service registration intuitive and discoverable:

```csharp
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddGitHubCms(options => {
            options.Owner = "username";
            options.Repository = "content-repo";
        })
        .AddScrollToTop()
        .AddClarityTracker(options => {
            options.SiteId = "clarity-id";
        })
        .AddOsirionStyle(CssFramework.Bootstrap);
});
```

### 🎨 CSS Framework Integration

Automatic integration with popular CSS frameworks:

- **Bootstrap 5+**: Seamless integration with Bootstrap classes and components
- **Fluent UI**: Microsoft's design system integration
- **MudBlazor**: Material Design components integration
- **Radzen**: Radzen Blazor components integration

### 🧩 New Core Components

A comprehensive set of layout and interaction components:

- **HeroSection**: Versatile hero sections for landing pages
- **OsirionBreadcrumbs**: Automatic breadcrumb navigation
- **OsirionCookieConsent**: GDPR-compliant cookie consent
- **InfiniteLogoCarousel**: Self-contained logo carousel
- **OsirionPageLayout**: Complete page layout system

## Installation and Setup

### 1. Install the Package

Choose your installation approach:

```bash
# Complete package (recommended)
dotnet add package Osirion.Blazor

# Or individual packages
dotnet add package Osirion.Blazor.Core
dotnet add package Osirion.Blazor.Navigation
dotnet add package Osirion.Blazor.Analytics
dotnet add package Osirion.Blazor.Cms
dotnet add package Osirion.Blazor.Theming
```

### 2. Configure Services

Add services to your `Program.cs`:

```csharp
using Osirion.Blazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Osirion.Blazor with fluent API
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddGitHubCms(options => {
            options.Owner = "your-username";
            options.Repository = "your-content-repo";
            options.ContentPath = "content";
            options.Branch = "main";
        })
        .AddScrollToTop(options => {
            options.Position = Position.BottomRight;
            options.Behavior = ScrollBehavior.Smooth;
        })
        .AddClarityTracker(options => {
            options.SiteId = "your-clarity-site-id";
        })
        .AddOsirionStyle(CssFramework.Bootstrap);
});

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

### 3. Alternative: Configuration-Based Setup

Use `appsettings.json` for configuration:

```json
{
  "Osirion": {
    "GitHubCms": {
      "Owner": "your-username",
      "Repository": "your-content-repo",
      "ContentPath": "content",
      "Branch": "main",
      "CacheExpirationMinutes": 60
    },
    "Analytics": {
      "Clarity": {
        "SiteId": "your-clarity-site-id",
        "Enabled": true
      }
    },
    "Navigation": {
      "ScrollToTop": {
        "Position": "BottomRight",
        "Behavior": "Smooth",
        "VisibilityThreshold": 300
      }
    },
    "Style": {
      "FrameworkIntegration": "Bootstrap",
      "UseStyles": true
    }
  }
}
```

Then in `Program.cs`:

```csharp
builder.Services.AddOsirionBlazor(builder.Configuration);
```

### 4. Add Global Imports

Add to your `_Imports.razor`:

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Navigation.Components
@using Osirion.Blazor.Analytics.Components
@using Osirion.Blazor.Cms.Components
```

### 5. Include Styles

Add CSS to your `_Host.cshtml` or `App.razor`:

```html
<link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion.css" />
```

Or include the OsirionStyles component:

```razor
<OsirionStyles FrameworkIntegration="CssFramework.Bootstrap" />
```

## Building Your First Page

### Create a Landing Page

Create a compelling landing page with the HeroSection component:

```razor
@page "/"
@using Osirion.Blazor.Components

<PageTitle>Welcome to My Application</PageTitle>

<!-- Hero Section -->
<HeroSection 
    Title="Build Amazing Applications"
    Subtitle="Faster, Better, Smarter"
    Summary="Osirion.Blazor provides everything you need to create modern, accessible web applications with minimal effort and maximum impact."
    ImageUrl="/images/hero-illustration.svg"
    ImageAlt="Modern web development illustration"
    UseBackgroundImage="false"
    ImagePosition="Alignment.Right"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/docs/getting-started"
    SecondaryButtonText="View Examples"
    SecondaryButtonUrl="/examples"
    BackgroundPattern="BackgroundPatternType.Dots" />

<!-- Features Section -->
<section class="py-5">
    <div class="container">
        <div class="row">
            <div class="col-lg-4 mb-4">
                <div class="card h-100">
                    <div class="card-body text-center">
                        <i class="bi bi-lightning-charge text-primary fs-1 mb-3"></i>
                        <h5 class="card-title">Lightning Fast</h5>
                        <p class="card-text">SSR-compatible components with zero JavaScript dependencies where possible.</p>
                    </div>
                </div>
            </div>
            <div class="col-lg-4 mb-4">
                <div class="card h-100">
                    <div class="card-body text-center">
                        <i class="bi bi-universal-access text-success fs-1 mb-3"></i>
                        <h5 class="card-title">Fully Accessible</h5>
                        <p class="card-text">WCAG 2.1 compliant components with comprehensive keyboard and screen reader support.</p>
                    </div>
                </div>
            </div>
            <div class="col-lg-4 mb-4">
                <div class="card h-100">
                    <div class="card-body text-center">
                        <i class="bi bi-palette text-info fs-1 mb-3"></i>
                        <h5 class="card-title">Framework Integration</h5>
                        <p class="card-text">Works seamlessly with Bootstrap, Fluent UI, MudBlazor, and Radzen.</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>

<!-- Partner Logos -->
<section class="py-5 bg-light">
    <div class="container">
        <InfiniteLogoCarousel 
            Title="Trusted by Developers Worldwide"
            CustomLogos="@partnerLogos"
            AnimationDuration="45"
            PauseOnHover="true"
            EnableGrayscale="true" />
    </div>
</section>

<!-- Footer -->
<OsirionFooter>
    <FooterContent>
        <div class="container">
            <div class="row">
                <div class="col-md-6">
                    <h5>Osirion.Blazor</h5>
                    <p>Modern components for modern applications.</p>
                </div>
                <div class="col-md-6 text-md-end">
                    <a href="/docs" class="me-3">Documentation</a>
                    <a href="/examples" class="me-3">Examples</a>
                    <a href="https://github.com/obrana-boranija/Osirion.Blazor">GitHub</a>
                </div>
            </div>
        </div>
    </FooterContent>
</OsirionFooter>

<!-- Navigation Enhancements -->
<EnhancedNavigation Behavior="ScrollBehavior.Smooth" />
<ScrollToTop Position="Position.BottomRight" />

@code {
    private List<LogoItem> partnerLogos = new()
    {
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/blazor/blazor-original.svg", "Blazor"),
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg", ".NET"),
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/bootstrap/bootstrap-original.svg", "Bootstrap"),
        new("https://cdn.jsdelivr.net/gh/devicons/devicon/icons/github/github-original.svg", "GitHub")
    };
}
```

### Add Navigation and Layout

Create a shared layout (`_Layout.razor` or `MainLayout.razor`):

```razor
@inherits LayoutView
@using Osirion.Blazor.Components

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <!-- Breadcrumb Navigation -->
        <div class="top-row px-4">
            <OsirionBreadcrumbs 
                Path="@GetCurrentPath()"
                ShowHome="true"
                HomeText="Home"
                SegmentFormatter="@FormatSegment" />
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

<!-- Global Components -->
<OsirionCookieConsent 
    Title="Privacy Notice"
    Message="We use cookies to enhance your experience and analyze site usage."
    PolicyLink="/privacy-policy"
    Categories="@GetCookieCategories()" />

<EnhancedNavigation Behavior="ScrollBehavior.Smooth" />
<ScrollToTop Position="Position.BottomRight" />

@code {
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private string GetCurrentPath()
    {
        var uri = new Uri(Navigation.Uri);
        return uri.AbsolutePath;
    }

    private string FormatSegment(string segment)
    {
        return segment.Replace("-", " ")
                     .Split(' ')
                     .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                     .Aggregate((a, b) => $"{a} {b}");
    }

    private List<CookieCategory> GetCookieCategories()
    {
        return new List<CookieCategory>
        {
            new() { Id = "necessary", Name = "Essential", IsRequired = true, IsEnabled = true },
            new() { Id = "analytics", Name = "Analytics", IsRequired = false, IsEnabled = false },
            new() { Id = "marketing", Name = "Marketing", IsRequired = false, IsEnabled = false }
        };
    }
}
```

## Building a Blog System

### Content Structure

Create your content repository structure:

```
content/
├── blog/
│   ├── 2025/
│   │   ├── 01/
│   │   │   ├── getting-started.md
│   │   │   └── advanced-components.md
│   │   └── 02/
│   │       └── new-features.md
│   └── tutorials/
│       ├── blazor-basics.md
│       └── component-development.md
├── docs/
│   ├── installation.md
│   ├── configuration.md
│   └── components/
│       ├── navigation.md
│       └── analytics.md
└── pages/
    ├── about.md
    ├── contact.md
    └── privacy-policy.md
```

### Blog Post Template

Create a blog post with frontmatter:

```markdown
---
title: "Building Your First Component"
author: "Jane Developer"
date: "2025-01-25"
description: "Learn how to create your first Blazor component with Osirion.Blazor."
tags: [Blazor, Components, Tutorial]
categories: [Tutorials]
slug: "building-first-component"
is_featured: true
featured_image: "/images/component-tutorial.jpg"
---

# Building Your First Component

Welcome to this comprehensive tutorial on building Blazor components...

## Getting Started

First, let's create a new component...

## Best Practices

Here are some best practices to follow...
```

### Blog Page Implementation

```razor
@page "/blog"
@page "/blog/{category?}"
@using Osirion.Blazor.Cms.Components

<PageTitle>@GetPageTitle()</PageTitle>

<!-- Blog Hero -->
<HeroSection 
    Title="@GetHeroTitle()"
    Subtitle="Insights, tutorials, and updates from the Osirion.Blazor team"
    Summary="Stay up to date with the latest developments, best practices, and community contributions."
    Variant="HeroVariant.Minimal"
    ShowPrimaryButton="false"
    ShowSecondaryButton="false" />

<div class="container my-5">
    <div class="row">
        <div class="col-lg-8">
            <!-- Featured Posts -->
            @if (string.IsNullOrEmpty(Category))
            {
                <section class="mb-5">
                    <h2 class="h4 mb-4">Featured Posts</h2>
                    <ContentList 
                        Directory="blog"
                        FeaturedCount="3"
                        SortBy="SortField.Date"
                        SortDirection="SortDirection.Descending" />
                </section>
            }

            <!-- All Posts or Category Posts -->
            <section>
                <h2 class="h4 mb-4">@GetSectionTitle()</h2>
                <ContentList 
                    Directory="blog"
                    Category="@Category"
                    Count="10"
                    SortBy="SortField.Date"
                    SortDirection="SortDirection.Descending" />
            </section>
        </div>

        <div class="col-lg-4">
            <aside>
                <!-- Search -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Search</h5>
                    </div>
                    <div class="card-body">
                        <SearchBox Placeholder="Search blog posts..." />
                    </div>
                </div>

                <!-- Categories -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Categories</h5>
                    </div>
                    <div class="card-body">
                        <CategoriesList />
                    </div>
                </div>

                <!-- Tags -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Popular Tags</h5>
                    </div>
                    <div class="card-body">
                        <TagCloud MaxTags="15" />
                    </div>
                </div>

                <!-- Recent Posts -->
                <div class="card">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Recent Posts</h5>
                    </div>
                    <div class="card-body">
                        <ContentList 
                            Directory="blog"
                            Count="5"
                            SortBy="SortField.Date"
                            SortDirection="SortDirection.Descending" />
                    </div>
                </div>
            </aside>
        </div>
    </div>
</div>

@code {
    [Parameter] public string? Category { get; set; }

    private string GetPageTitle()
    {
        return string.IsNullOrEmpty(Category) 
            ? "Blog - Osirion.Blazor" 
            : $"{Category} - Blog - Osirion.Blazor";
    }

    private string GetHeroTitle()
    {
        return string.IsNullOrEmpty(Category) 
            ? "Osirion.Blazor Blog" 
            : $"{Category} Posts";
    }

    private string GetSectionTitle()
    {
        return string.IsNullOrEmpty(Category) 
            ? "Latest Posts" 
            : $"{Category} Posts";
    }
}
```

### Individual Blog Post Page

```razor
@page "/blog/{year:int}/{month:int}/{slug}"
@using Osirion.Blazor.Cms.Components
@inject IContentProvider ContentProvider

<PageTitle>@(article?.Title ?? "Loading...") - Blog</PageTitle>

@if (article != null)
{
    <!-- Article Hero -->
    <HeroSection 
        Title="@article.Title"
        Summary="@article.Description"
        Author="@article.Author"
        PublishDate="@article.Date"
        ReadTime="@($"{article.ReadTimeMinutes} min read")"
        ShowMetadata="true"
        Variant="HeroVariant.Minimal"
        ShowPrimaryButton="false"
        ShowSecondaryButton="false"
        ImageUrl="@article.FeaturedImageUrl"
        UseBackgroundImage="@(!string.IsNullOrEmpty(article.FeaturedImageUrl))" />

    <div class="container my-5">
        <div class="row">
            <div class="col-lg-8">
                <article>
                    <!-- Article Content -->
                    <ContentView Path="@articlePath" />

                    <!-- Article Tags -->
                    @if (article.Tags.Any())
                    {
                        <div class="mt-4 pt-4 border-top">
                            <h5>Tags</h5>
                            <div class="d-flex flex-wrap gap-2">
                                @foreach (var tag in article.Tags)
                                {
                                    <a href="/blog?tag=@tag" class="badge bg-secondary text-decoration-none">
                                        @tag
                                    </a>
                                }
                            </div>
                        </div>
                    }
                </article>
            </div>

            <div class="col-lg-4">
                <aside>
                    <!-- Related Posts -->
                    <div class="card">
                        <div class="card-header">
                            <h5 class="card-title mb-0">Related Posts</h5>
                        </div>
                        <div class="card-body">
                            <ContentList 
                                Category="@article.Categories.FirstOrDefault()"
                                Count="5"
                                ExcludePath="@articlePath" />
                        </div>
                    </div>
                </aside>
            </div>
        </div>
    </div>
}
else
{
    <!-- Loading State -->
    <OsirionPageLoading Message="Loading article..." />
}

@code {
    [Parameter] public int Year { get; set; }
    [Parameter] public int Month { get; set; }
    [Parameter] public string Slug { get; set; } = "";

    private ContentItem? article;
    private string articlePath => $"blog/{Year:0000}/{Month:00}/{Slug}.md";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            article = await ContentProvider.GetItemByPathAsync(articlePath);
        }
        catch (Exception)
        {
            // Article not found - this will be handled by the NotFound component
        }
    }
}
```

## Advanced Scenarios

### Custom Component Development

Create custom components that integrate with Osirion.Blazor:

```csharp
// Custom component inheriting from OsirionComponentBase
public partial class CustomCard : OsirionComponentBase
{
    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Subtitle { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? ImageUrl { get; set; }

    private string GetCardClass()
    {
        var classes = new List<string> { "custom-card" };
        
        if (!string.IsNullOrWhiteSpace(ImageUrl))
            classes.Add("custom-card-with-image");
            
        return CombineCssClasses(string.Join(" ", classes));
    }
}
```

```razor
<!-- CustomCard.razor -->
<div class="@GetCardClass()" style="@Style" @attributes="Attributes">
    @if (!string.IsNullOrWhiteSpace(ImageUrl))
    {
        <div class="custom-card-image">
            <img src="@ImageUrl" alt="@Title" />
        </div>
    }
    
    <div class="custom-card-content">
        @if (!string.IsNullOrWhiteSpace(Title))
        {
            <h3 class="custom-card-title">@Title</h3>
        }
        
        @if (!string.IsNullOrWhiteSpace(Subtitle))
        {
            <p class="custom-card-subtitle">@Subtitle</p>
        }
        
        <div class="custom-card-body">
            @ChildContent
        </div>
    </div>
</div>
```

### Analytics Integration

```razor
@inject ICookieConsentService ConsentService

<!-- Conditional analytics loading based on consent -->
@if (await ConsentService.HasConsentAsync("analytics"))
{
    <ClarityTracker />
    
    @if (Environment.IsProduction())
    {
        <script async src="https://www.googletagmanager.com/gtag/js?id=GA_MEASUREMENT_ID"></script>
        <script>
            window.dataLayer = window.dataLayer || [];
            function gtag(){dataLayer.push(arguments);}
            gtag('js', new Date());
            gtag('config', 'GA_MEASUREMENT_ID');
        </script>
    }
}
```

### Performance Optimization

```csharp
// Implement caching for content-heavy pages
public class CachedContentService : IContentService
{
    private readonly IContentProvider _contentProvider;
    private readonly IMemoryCache _cache;
    
    public async Task<ContentItem?> GetContentAsync(string path)
    {
        var cacheKey = $"content:{path}";
        
        if (_cache.TryGetValue(cacheKey, out ContentItem? cached))
        {
            return cached;
        }
        
        var content = await _contentProvider.GetItemByPathAsync(path);
        
        if (content != null)
        {
            _cache.Set(cacheKey, content, TimeSpan.FromMinutes(30));
        }
        
        return content;
    }
}
```

## Best Practices

### 1. SSR-First Development

```razor
<!-- Design components to work without JavaScript -->
<nav class="navigation">
    <a href="/docs">Documentation</a>
    <a href="/blog">Blog</a>
    <a href="/contact">Contact</a>
</nav>

<!-- Enhance with JavaScript when available -->
<EnhancedNavigation @rendermode="@RenderMode.InteractiveServer" />
```

### 2. Progressive Enhancement

```csharp
// Use progressive enhancement patterns
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender && JSRuntime != null)
    {
        // Add JavaScript enhancements after initial render
        await JSRuntime.InvokeVoidAsync("enhanceComponent", DotNetObjectReference.Create(this));
    }
}
```

### 3. Accessibility First

```razor
<!-- Always include proper ARIA labels and semantic HTML -->
<nav aria-label="Main navigation">
    <ul role="list">
        <li role="listitem">
            <a href="/docs" aria-current="@(currentPage == "docs" ? "page" : null)">
                Documentation
            </a>
        </li>
    </ul>
</nav>
```

### 4. Performance Monitoring

```csharp
// Monitor component performance
public class PerformanceMonitor
{
    private readonly ILogger<PerformanceMonitor> _logger;
    
    public async Task<T> MeasureAsync<T>(string operation, Func<Task<T>> action)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await action();
            _logger.LogInformation("Operation {Operation} completed in {Duration}ms", 
                operation, stopwatch.ElapsedMilliseconds);
            return result;
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
```

## Next Steps

### Learn More

- 📖 [Component Documentation](https://github.com/obrana-boranija/Osirion.Blazor/tree/master/docs)
- 🎯 [Quick Reference Guide](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/QUICK_REFERENCE.md)
- 🔄 [Migration Guide](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/MIGRATION.md)

### Component Guides

- [🦸 Hero Section](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/HERO_SECTION.md)
- [🍞 Breadcrumbs](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/BREADCRUMBS.md)
- [🍪 Cookie Consent](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/COOKIE_CONSENT.md)
- [🎠 Logo Carousel](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/INFINITE_LOGO_CAROUSEL.md)

### Community

- 💬 [GitHub Discussions](https://github.com/obrana-boranija/Osirion.Blazor/discussions)
- 🐛 [Issue Tracker](https://github.com/obrana-boranija/Osirion.Blazor/issues)
- 📧 [Contact](https://github.com/obrana-boranija/Osirion.Blazor)

## Conclusion

Osirion.Blazor v1.5 provides everything you need to build modern, accessible, and performant Blazor applications. With comprehensive components, seamless framework integration, and excellent developer experience, you can focus on building features that matter to your users.

Start building amazing applications today with Osirion.Blazor v1.5!