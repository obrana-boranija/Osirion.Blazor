# Osirion.Blazor.Cms.Web

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Web)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Web)
[![License](https://img.shields.io/github/license/demonjic/Osirion.Blazor)](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt)

Frontend components and web interfaces for the Osirion.Blazor CMS ecosystem, providing content display, navigation, and search capabilities.

## Features

- **Content Display Components**: Render content items with rich formatting
- **Navigation Components**: Directory, category, and tag-based navigation
- **Search Interface**: Full-text content search with filtering
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Responsive Design**: Mobile-friendly content display
- **Metadata Rendering**: SEO-optimized content rendering with OpenGraph support
- **Content List Views**: Customizable list displays for content collections
- **Custom Item Markup**: `ItemTemplate` on `ContentList`, `ContentListPage` and `OsirionContentListSection` renders each item through your own markup
- **Article Header**: `ArticlePage.HeaderContent` replaces the hero with your own article header
- **Table of Contents**: `TableOfContents` marks the section being read as the page scrolls; `TrackActiveHeading="false"` turns it off
- **Author URL**: `SeoMetadataOptions.AuthorUrl` (or the `AuthorUrl` parameter of `SeoMetadataRenderer`) sets the author's `url` in structured data
- **User Interface Elements**: User-friendly components for content consumption

## Installation

```bash
dotnet add package Osirion.Blazor.Cms.Web
```

## Usage

### Basic Content Display

```razor
@using Osirion.Blazor.Cms.Domain.Enums
@using Osirion.Blazor.Cms.Web.Components

<!-- Display a specific content item -->
<ContentView Path="blog/my-post.md" />

<!-- Display a list of content items -->
<ContentList
    Directory="blog"
    SortBy="SortField.Date"
    SortDirection="SortDirection.Descending" />

<!-- Add search functionality (a GET form that submits to /search) -->
<SearchBox Placeholder="Search..." />

<!-- Add directory navigation (pass the directories to show) -->
<DirectoryNavigation Directories="@directories" CurrentDirectory="blog" />

<!-- Display categories and tags -->
<CategoriesList />
<TagCloud MaxTags="20" />
```

### Content View Customization

```razor
<ContentView
    Path="blog/my-post.md"
    ShowNavigationLinks="true"
    PreviousItem="@previousItem"
    NextItem="@nextItem"
    CategoryUrlFormatter="@(category => $"/categories/{category}")"
    TagUrlFormatter="@(tag => $"/tags/{tag}")" />
```

### Custom Content List Templates

```razor
<ContentList Directory="blog">
    <ItemTemplate>
        <div class="custom-content-card">
            <h2>@context.Title</h2>
            <p class="date">@context.DateCreated.ToString("MMMM d, yyyy")</p>
            <p class="author">By: @context.Author</p>
            <p class="description">@context.Description</p>
            <a href="@($"/content/{context.Path}")">Read more...</a>
        </div>
    </ItemTemplate>
</ContentList>
```

### SEO Metadata

```razor
<SeoMetadataRenderer
    Content="@currentContent"
    SiteNameOverride="My Website"
    DefaultImageUrl="/images/default-social.jpg" />
```

The base URL comes from the current request. Site-wide defaults can be set with `AddOsirionSeoMetadata` (in
`Osirion.Blazor.Cms.Web.DependencyInjection`), which configures `SeoMetadataOptions`.

### Sitemap, RSS and Atom

```csharp
using Osirion.Blazor.Cms.Web.Feeds;

// Maps /sitemap.xml, /feed.xml (RSS 2.0) and /atom.xml from the published content
app.MapOsirionFeeds(feeds =>
{
    feeds.SiteUrl = "https://mysite.com";
    feeds.FeedDirectory = "blog";
    feeds.AdditionalSitemapUrls = ["/", "/blog"];
});
```

See "Sitemap and Feeds" in [GITHUB_CMS.md](https://github.com/demonjic/Osirion.Blazor/blob/master/docs/GITHUB_CMS.md)
for every option.

## Components

### ContentView

The primary component for displaying a single content item.

```razor
<ContentView Path="blog/my-post.md" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Path` | string | | The path to the content item |
| `Item` | ContentItem? | null | An item to render instead of loading one by `Path` |
| `PreviousItem` | ContentItem? | null | The previous item, for the navigation links |
| `NextItem` | ContentItem? | null | The next item, for the navigation links |
| `ShowNavigationLinks` | bool | false | Whether to show links to the previous and next items |
| `LoadingText` | string | "Loading content..." | Text shown while loading |
| `NotFoundText` | string | "Content not found." | Text shown when the item does not exist |
| `CategoryUrlFormatter` | Func<string, string>? | null | Builds the address of a category |
| `TagUrlFormatter` | Func<string, string>? | null | Builds the address of a tag |
| `AuthorUrlFormatter` | Func<string, string>? | null | Builds the address of an author |
| `ContentUrlFormatter` | Func<ContentItem, string>? | null | Builds the address of an item |

### ContentList

A component for displaying a list of content items.

```razor
<ContentList
    Directory="blog"
    SortBy="SortField.Date"
    SortDirection="SortDirection.Descending"
    ShowPagination="true"
    ItemsPerPage="10" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Directory` | string? | null | Filter by directory |
| `Category` | string? | null | Filter by category |
| `Tag` | string? | null | Filter by tag |
| `Author` | string? | null | Filter by author (ignoring case) |
| `SearchQuery` | string? | null | Search query |
| `OnlyFeatured` | bool? | null | Show only featured items |
| `FeaturedCount` | int? | null | Number of featured items to show |
| `Locale` | string? | null | Filter by locale |
| `SortBy` | SortField | Date | Field to sort by |
| `SortDirection` | SortDirection | Descending | Sort direction |
| `ShowPagination` | bool | false | Whether to page the list |
| `ItemsPerPage` | int | 10 | Items per page |
| `CurrentPage` | int | 1 | The page to show |
| `PageChanged` | EventCallback<int> | | Raised when the page changes |
| `PaginationUrlFormatter` | Func<int, string>? | null | Builds the address of a page |
| `ContentUrlFormatter` | Func<ContentItem, string>? | null | Builds the address of an item |
| `ItemTemplate` | RenderFragment<ContentItem>? | null | Renders each item through your own markup |
| `ReadMoreText` | string | "Read more" | Text of the link on each card |
| `LoadingText` | string | "Loading content..." | Text shown while loading |
| `NoContentText` | string | "No content available." | Text shown when the list is empty |
| `ShowTitle` | bool | false | Opens the list with its title as the page `h1` |
| `PageTitle` | string? | null | Title for the list and its structured data |
| `PageDescription` | string? | null | Description for the list and its structured data |
| `EnableSeoMetadata` | bool | true | Whether to render SEO metadata |

### SearchBox

A component for searching content. It renders a GET form; the results page handles the query.

```razor
<SearchBox
    ActionUrl="/search"
    QueryParameterName="q"
    Placeholder="Search..." />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ActionUrl` | string | "/search" | Address the form submits to |
| `QueryParameterName` | string | "q" | Name of the query string key |
| `SearchQuery` | string | "" | Initial value of the input |
| `Placeholder` | string | "Search content..." | Placeholder text |
| `SearchButtonText` | string | "Search" | Text for the search button |
| `Title` | string? | null | Optional heading above the form |

### DirectoryNavigation

A component for navigating content directories. It renders the directories you pass in.

```razor
<DirectoryNavigation
    Directories="@directories"
    CurrentDirectory="blog"
    ShowItemCount="true" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Directories` | IReadOnlyList<DirectoryItem>? | null | The directories to show |
| `CurrentDirectory` | string? | null | Currently active directory |
| `ExpandedDirectory` | string? | null | Directory to show expanded |
| `ExpandAllSubdirectories` | bool | false | Whether to expand every directory |
| `ShowItemCount` | bool | true | Whether to show item counts |
| `ShowSubdirectories` | bool | true | Whether to show subdirectories |
| `DirectoryUrlFormatter` | Func<DirectoryItem, string>? | null | Builds the address of a directory |
| `DirectoryClicked` | EventCallback<DirectoryItem> | | Raised when a directory is selected |
| `Title` | string? | null | Optional heading |
| `IsLoading` | bool | false | Shows `LoadingText` instead of the list |
| `LoadingText` | string | "Loading navigation..." | Text shown while loading |
| `NoContentText` | string | "No directories available." | Text shown when there are none |

### CategoriesList and TagCloud

Components for displaying content organization. Both load their data from the registered content provider.

```razor
<CategoriesList
    ShowCount="true"
    SortByCount="true" />

<TagCloud
    MaxTags="20"
    ShowCount="true" />
```

`CategoriesList` parameters: `Title`, `LoadingText`, `NoContentText`, `CategoryUrlFormatter`, `ActiveCategory`,
`ShowCount` (true), `SortByCount` (true) and `MaxCategories`.

`TagCloud` parameters: `Title`, `LoadingText`, `NoContentText`, `TagUrlFormatter`, `ActiveTag`, `ShowCount` (true),
`SortByCount` (true), `MaxTags` and `Tags` (a list to show instead of loading one).

## Service-Based Content Retrieval

For advanced scenarios, you can directly inject and use the content provider:

```razor
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Domain.Enums
@using Osirion.Blazor.Cms.Domain.Repositories
@using Osirion.Blazor.Cms.Domain.Services
@inject IContentProvider ContentProvider

@code {
    private List<ContentItem> featuredContent = new();

    protected override async Task OnInitializedAsync()
    {
        var query = new ContentQuery
        {
            IsFeatured = true,
            SortBy = SortField.Date,
            SortDirection = SortDirection.Descending,
            Take = 5
        };

        var result = await ContentProvider.GetItemsByQueryAsync(query);
        featuredContent = result?.ToList() ?? new();
    }
}
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt) file for details.