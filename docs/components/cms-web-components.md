# Osirion.Blazor CMS Web Components Documentation

This documentation covers the Web module of Osirion.Blazor CMS, which provides components for displaying content managed through GitHub repositories. The Web module is designed for headless CMS scenarios, allowing content to be stored in GitHub and rendered in Blazor applications.

The Web module includes components for content rendering, navigation, pages, sections, and SEO metadata. These components integrate with the CMS domain models and services to provide a seamless content management experience.

## Component List

The Web module contains the following components:

- ContentRenderer
- ContentPage
- ContentList
- ContentView
- LocalizedContentView
- SeoMetadataRenderer
- OsirionContentListSection
- FeaturedPostsSection
- ContentSection
- CategoriesList
- ArticlePage
- DirectoryNavigation
- HomePage
- LocalizedNavigation
- OsirionContentNavigation
- LandingPage
- DocumentPage
- ContentBreadcrumbs

Each component is described below with usage examples.

## ContentRenderer

The `ContentRenderer` component renders the content of a content item using the HTML renderer.

### Parameters

- `Item`: ContentItem - The content item to render

### Example

```razor
<ContentRenderer Item="@contentItem" />
```

## ContentPage

The `ContentPage` component displays a full content page with breadcrumbs, navigation, and metadata.

### Parameters

- `Path`: string - Path of the content to display
- `ContentItem`: ContentItem - The content item to display
- `PreviousItem`: ContentItem - Previous content item for navigation
- `NextItem`: ContentItem - Next content item for navigation
- `RelatedItems`: IReadOnlyList<ContentItem> - Related content items
- `LoadingText`: string - Loading message (default: "Loading content...")
- `NotFoundTitle`: string - Not found title (default: "Content Not Found")
- `NotFoundText`: string - Not found message
- `ShowBackLink`: bool - Show back link when not found (default: true)
- `BackLinkUrl`: string - Back link URL (default: "/")
- `BackLinkText`: string - Back link text (default: "Back to Home")
- `ShowBreadcrumbs`: bool - Show breadcrumbs (default: true)
- `ShowBreadcrumbHome`: bool - Show home in breadcrumbs (default: true)
- `BreadcrumbHomeUrl`: string - Home URL in breadcrumbs (default: "/")
- `BreadcrumbHomeText`: string - Home text in breadcrumbs (default: "Home")
- `ShowNavigationLinks`: bool - Show previous/next links (default: true)
- `UseLocalizedView`: bool - Use localized content view (default: true)
- `SchemaType`: string - Schema.org type (default: "Article")

### Example

```razor
<ContentPage 
    Path="/blog/my-article" 
    ShowBreadcrumbs="true" 
    ShowNavigationLinks="true" />
```

## ContentList

The `ContentList` component displays a list of content items.

### Parameters

- `Items`: IEnumerable<ContentItem> - List of content items
- `ShowExcerpt`: bool - Show content excerpt (default: true)
- `ShowMetadata`: bool - Show metadata (default: true)
- `ItemsPerPage`: int - Items per page (default: 10)

### Example

```razor
<ContentList Items="@contentItems" ItemsPerPage="5" />
```

## ContentView

The `ContentView` component displays the main content view for a content item.

### Parameters

- `Item`: ContentItem - The content item
- `ShowTitle`: bool - Show title (default: true)
- `ShowMetadata`: bool - Show metadata (default: true)
- `ShowContent`: bool - Show content (default: true)

### Example

```razor
<ContentView Item="@contentItem" />
```

## LocalizedContentView

The `LocalizedContentView` component displays localized content with navigation.

### Parameters

- `Item`: ContentItem - The content item
- `PreviousItem`: ContentItem - Previous item
- `NextItem`: ContentItem - Next item
- `ShowNavigationLinks`: bool - Show navigation links (default: true)
- `AvailableTranslations`: IEnumerable<ContentItem> - Available translations

### Example

```razor
<LocalizedContentView 
    Item="@contentItem" 
    PreviousItem="@prevItem" 
    NextItem="@nextItem" />
```

## SeoMetadataRenderer

The `SeoMetadataRenderer` component renders SEO metadata tags.

### Parameters

- `Metadata`: SeoMetadata - SEO metadata object

### Example

```razor
<SeoMetadataRenderer Metadata="@seoMetadata" />
```

## OsirionContentListSection

The `OsirionContentListSection` component displays a section with a list of content items.

### Parameters

- `Title`: string - Section title
- `Items`: IEnumerable<ContentItem> - Content items
- `ShowMoreLink`: string - Show more link URL
- `ShowMoreText`: string - Show more link text

### Example

```razor
<OsirionContentListSection 
    Title="Latest Posts" 
    Items="@posts" 
    ShowMoreLink="/blog" 
    ShowMoreText="View All" />
```

## FeaturedPostsSection

The `FeaturedPostsSection` component displays featured posts.

### Parameters

- `Posts`: IEnumerable<ContentItem> - Featured posts
- `Title`: string - Section title
- `Subtitle`: string - Section subtitle

### Example

```razor
<FeaturedPostsSection 
    Posts="@featuredPosts" 
    Title="Featured Articles" />
```

## ContentSection

The `ContentSection` component displays a content section.

### Parameters

- `Content`: string - Section content
- `Title`: string - Section title
- `Background`: string - Background style

### Example

```razor
<ContentSection 
    Title="About Us" 
    Content="<p>Our company...</p>" />
```

## CategoriesList

The `CategoriesList` component displays a list of content categories.

### Parameters

- `Categories`: IEnumerable<Category> - List of categories
- `ShowCounts`: bool - Show item counts (default: true)

### Example

```razor
<CategoriesList Categories="@categories" />
```

## ArticlePage

The `ArticlePage` component displays an article page layout.

### Parameters

- `Article`: ContentItem - The article content item
- `ShowAuthor`: bool - Show author info (default: true)
- `ShowDate`: bool - Show publication date (default: true)

### Example

```razor
<ArticlePage Article="@article" />
```

## DirectoryNavigation

The `DirectoryNavigation` component provides navigation for content directories.

### Parameters

- `Directory`: ContentDirectory - The content directory
- `CurrentPath`: string - Current path
- `ShowRoot`: bool - Show root directory (default: true)

### Example

```razor
<DirectoryNavigation Directory="@directory" CurrentPath="/blog" />
```

## HomePage

The `HomePage` component displays the home page layout.

### Parameters

- `FeaturedContent`: IEnumerable<ContentItem> - Featured content
- `RecentPosts`: IEnumerable<ContentItem> - Recent posts
- `HeroContent`: ContentItem - Hero content

### Example

```razor
<HomePage 
    FeaturedContent="@featured" 
    RecentPosts="@recent" 
    HeroContent="@hero" />
```

## LocalizedNavigation

The `LocalizedNavigation` component provides localized navigation.

### Parameters

- `CurrentCulture`: string - Current culture
- `AvailableCultures`: IEnumerable<string> - Available cultures
- `ContentPath`: string - Content path

### Example

```razor
<LocalizedNavigation 
    CurrentCulture="en" 
    AvailableCultures="@cultures" 
    ContentPath="/about" />
```

## OsirionContentNavigation

The `OsirionContentNavigation` component provides content navigation.

### Parameters

- `Items`: IEnumerable<ContentItem> - Navigation items
- `CurrentItem`: ContentItem - Current item
- `ShowIcons`: bool - Show icons (default: true)

### Example

```razor
<OsirionContentNavigation 
    Items="@navItems" 
    CurrentItem="@current" />
```

## LandingPage

The `LandingPage` component displays a landing page layout.

### Parameters

- `HeroSection`: ContentItem - Hero section content
- `Features`: IEnumerable<ContentItem> - Feature items
- `CallToAction`: ContentItem - Call to action content

### Example

```razor
<LandingPage 
    HeroSection="@hero" 
    Features="@features" 
    CallToAction="@cta" />
```

## DocumentPage

The `DocumentPage` component displays a document page.

### Parameters

- `Document`: ContentItem - The document content item
- `ShowToc`: bool - Show table of contents (default: true)
- `TocTitle`: string - TOC title (default: "Table of Contents")

### Example

```razor
<DocumentPage Document="@document" ShowToc="true" />
```

## ContentBreadcrumbs

The `ContentBreadcrumbs` component displays breadcrumbs for content navigation.

### Parameters

- `Content`: ContentItem - The content item
- `Directory`: ContentDirectory - The content directory
- `ShowHome`: bool - Show home link (default: true)
- `HomeUrl`: string - Home URL (default: "/")
- `HomeText`: string - Home text (default: "Home")

### Example

```razor
<ContentBreadcrumbs 
    Content="@contentItem" 
    Directory="@directory" />
```
