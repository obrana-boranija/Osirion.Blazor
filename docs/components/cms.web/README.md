# CMS Web Components

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Web)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Web)

The Osirion.Blazor.Cms.Web package provides frontend components for content display, navigation, and user interaction within the Osirion CMS ecosystem. These components are designed for content consumption and provide a complete toolkit for building content-driven websites.

## Installation

```bash
dotnet add package Osirion.Blazor.Cms.Web
```

## Component Categories

### Content Display Components
Components for rendering and displaying content items:
- [ContentRenderer](ContentRenderer.md) - Low-level content rendering with OsirionHtmlRenderer
- [ContentView](ContentView.md) - Rich content display with metadata and navigation
- [ContentPage](ContentPage.md) - Complete page layout for content with SEO and sharing
- [LocalizedContentView](LocalizedContentView.md) - Multi-language content display

### Content Collection Components
Components for displaying multiple content items:
- [ContentList](ContentList.md) - Lists and collections of content with pagination

### Navigation Components
Components for content discovery and navigation:
- [DirectoryNavigation](DirectoryNavigation.md) - Hierarchical content navigation
- [ContentBreadcrumbs](ContentBreadcrumbs.md) - Breadcrumb navigation for content
- [CategoriesList](CategoriesList.md) - Category-based navigation
- [TagCloud](TagCloud.md) - Tag-based content discovery  
- [TableOfContents](TableOfContents.md) - Document navigation and outline
- [SearchBox](SearchBox.md) - Content search interface
- [OsirionContentNavigation](OsirionContentNavigation.md) - Previous/next navigation
- [LocalizedNavigation](LocalizedNavigation.md) - Multi-language navigation

### Page Level Components
High-level page components for specific content types:
- [ArticlePage](ArticlePage.md) - Article/blog post pages
- [DocumentPage](DocumentPage.md) - Documentation pages  
- [HomePage](HomePage.md) - Home page layout
- [LandingPage](LandingPage.md) - Landing page layout

### Section Components
Components for page sections and layouts:
- [ContentSection](ContentSection.md) - Generic content sections
- [FeaturedPostsSection](FeaturedPostsSection.md) - Featured content display
- [OsirionContentListSection](OsirionContentListSection.md) - Content list sections

### SEO and Metadata Components
Components for search engine optimization:
- [SeoMetadataRenderer](SeoMetadataRenderer.md) - Complete SEO metadata generation

## Quick Start

### Basic Content Display

```razor
@using Osirion.Blazor.Cms.Web.Components

<!-- Display a single content item -->
<ContentView Path="blog/my-post.md" />

<!-- Display content with full page layout -->
<ContentPage Path="blog/my-post.md" />

<!-- Display a list of content items -->
<ContentList Directory="blog" Count="10" />

<!-- Add navigation -->
<DirectoryNavigation CurrentDirectory="blog" />
<CategoriesList />
<TagCloud />
```

### Complete Blog Post Page

```razor
<ContentPage 
    Path="@Path"
    ShowBreadcrumbs="true"
    ShowMetadata="true"
    ShowNavigationLinks="true"
    ShowRelatedContent="true"
    ShowShareLinks="true" />
```

### Search and Discovery

```razor
<SearchBox 
    Placeholder="Search content..."
    OnSearch="@HandleSearch" />

<CategoriesList>
    <CategoryTemplate Context="category">
        <div class="category-card">
            <h4>@category.Name</h4>
            <span class="badge">@category.Count</span>
        </div>
    </CategoryTemplate>
</CategoriesList>
```

### SEO Optimization

```razor
<SeoMetadataRenderer 
    Content="@currentContent"
    SiteNameOverride="My Website"
    TwitterSite="@myhandle"
    SchemaType="SchemaType.Article" />
```

## Configuration

### Service Registration

```csharp
// Configure in Program.cs
builder.Services.AddOsirionBlazor(osirion => {
    osirion.AddGitHubCms(options => {
        options.Owner = "your-username";
        options.Repository = "content-repo";
        options.ContentPath = "content";
        options.Branch = "main";
        options.EnableLocalization = true;
        options.DefaultLocale = "en";
    });
});
```

### Component Imports

Add to `_Imports.razor`:

```razor
@using Osirion.Blazor.Cms.Web.Components
@using Osirion.Blazor.Cms.Web.Components.Navigation
@using Osirion.Blazor.Cms.Web.Components.Sections
@using Osirion.Blazor.Cms.Web.Components.PageLevel
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Domain.Enums
```

## Features

### Content Management
- Markdown content rendering with syntax highlighting via OsirionHtmlRenderer
- Rich metadata support (SEO, OpenGraph, Twitter Cards, JSON-LD)
- Multi-language content support with locale fallbacks
- Category and tag organization with URL formatting
- Featured content highlighting and filtering
- Content relationships and navigation (previous/next)
- Share link generation for social media

### User Experience
- Responsive design for all screen sizes
- Accessibility compliance (WCAG AA)
- Search functionality with debounced input
- Pagination support for content lists
- Loading states and error handling
- Progressive enhancement for SSR compatibility

### Developer Experience
- Template customization with RenderFragments
- URL formatting and routing flexibility
- Event callbacks for custom behaviors
- CSS customization and theming
- Framework integration (Bootstrap, MudBlazor, etc.)
- Base classes for common functionality

## Theming and Customization

### CSS Variables

```css
:root {
    /* Content display */
    --osirion-content-padding: 1.5rem;
    --osirion-content-background: #ffffff;
    --osirion-content-border-radius: 0.5rem;
    --osirion-line-height-relaxed: 1.7;
    
    /* Navigation */
    --osirion-nav-background: #f8f9fa;
    --osirion-nav-hover-background: #e9ecef;
    --osirion-nav-active-background: #dee2e6;
    
    /* Colors */
    --osirion-text-primary: #374151;
    --osirion-action-primary: #0369a1;
    --osirion-action-primary-hover: #0284c7;
    
    /* Typography */
    --osirion-font-size-xl: 1.25rem;
    --osirion-font-size-2xl: 1.5rem;
    --osirion-font-weight-bold: 700;
}
```

### Component Templates

Many components support custom templates:

```razor
<ContentList Directory="blog">
    <ItemTemplate Context="item">
        <article class="blog-post-card">
            <header>
                <h2><a href="@GetContentUrl(item)">@item.Title</a></h2>
                <time datetime="@item.PublishDate?.ToString("yyyy-MM-dd")">
                    @item.PublishDate?.ToString("MMMM d, yyyy")
                </time>
            </header>
            <div class="content">
                <p>@item.Description</p>
                @if (item.Tags.Any())
                {
                    <div class="tags">
                        @foreach (var tag in item.Tags)
                        {
                            <span class="tag">@tag</span>
                        }
                    </div>
                }
            </div>
        </article>
    </ItemTemplate>
</ContentList>
```

## Base Classes

### Page-Level Base Classes
All page-level components inherit from these base classes:

- `OsirionContentPageBase` - Base for all content pages with common properties
- `OsirionContentDetailPageBase` - Base for detailed content views  
- `OsirionContentListPageBase` - Base for content list pages

These provide:
- Content loading and state management
- URL formatting and navigation
- Error handling and loading states
- Common parameters and events

## Architecture

### Component Design Patterns

All components follow these design principles:

- **Inheritance**: Components inherit from `OsirionComponentBase`
- **CSS Isolation**: Each component has scoped CSS styles
- **Parameter Validation**: Required parameters are validated
- **SSR Compatible**: All components work with server-side rendering
- **Accessibility**: ARIA labels and semantic HTML structure
- **Responsive**: Mobile-first responsive design
- **Event-Driven**: EventCallback parameters for custom behaviors

### Service Dependencies

Components rely on:
- `IContentProviderManager` - Content retrieval and management
- `NavigationManager` - URL navigation and routing
- Various domain services for specialized functionality

## Best Practices

### Performance
- Use pagination for large content lists
- Implement caching strategies for frequently accessed content
- Optimize images and media files in content
- Leverage SSR for better initial page load

### SEO
- Always include `SeoMetadataRenderer` on content pages
- Use semantic HTML structure in custom templates
- Provide descriptive alt text for images
- Implement structured data with appropriate schema types

### Accessibility
- Ensure keyboard navigation works for all interactive elements
- Provide appropriate ARIA labels and descriptions
- Maintain WCAG AA color contrast ratios
- Test with screen readers

### Content Organization
- Use consistent URL patterns with formatters
- Implement breadcrumb navigation for deep content
- Provide search functionality for content discovery
- Create logical category and tag hierarchies

## Related Documentation

- [Content Management Guide](../../GITHUB_CMS.md)
- [Theming Guide](../../STYLING.md)
- [Architecture Overview](../../ARCHITECTURE.md)
- [API Reference](../../API_REFERENCE.md)
- [Quick Reference](../../QUICK_REFERENCE.md)