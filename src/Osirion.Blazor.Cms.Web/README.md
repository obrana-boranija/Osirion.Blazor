# Osirion.Blazor.Cms.Web

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Web)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Web)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Frontend components and web interfaces for the Osirion.Blazor CMS ecosystem, providing content display, navigation, and search capabilities.

## Features

- **Content Display Components**: Render content items with rich formatting
- **Navigation Components**: Directory, category, and tag-based navigation
- **Search Interface**: Full-text content search with filtering
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Responsive Design**: Mobile-friendly content display
- **Metadata Rendering**: SEO-optimized content rendering with OpenGraph support
- **Content List Views**: Customizable list displays for content collections
- **User Interface Elements**: User-friendly components for content consumption

## Installation

```bash
dotnet add package Osirion.Blazor.Cms.Web
```

## Usage

### Basic Content Display

```razor
@using Osirion.Blazor.Cms.Web.Components

<!-- Display a specific content item -->
<ContentView Path="blog/my-post.md" />

<!-- Display a list of content items -->
<ContentList 
    Directory="blog" 
    SortBy="SortField.Date" 
    SortDirection="SortDirection.Descending" />

<!-- Add search functionality -->
<SearchBox Placeholder="Search..." />

<!-- Add directory navigation -->
<DirectoryNavigation CurrentDirectory="blog" />

<!-- Display categories and tags -->
<CategoriesList />
<TagCloud MaxTags="20" />
```

### Content View Customization

```razor
<ContentView 
    Path="blog/my-post.md"
    ShowTitle="true"
    ShowMetadata="true"
    ShowDate="true"
    ShowAuthor="true"
    ShowTags="true"
    ShowCategories="true"
    ShowImage="true"
    ImageClass="my-featured-image" />
```

### Custom Content List Templates

```razor
<ContentList Directory="blog">
    <ItemTemplate>
        <div class="custom-content-card">
            <h2>@context.Title</h2>
            <p class="date">@context.Date?.ToString("MMMM d, yyyy")</p>
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
    SiteName="My Website" 
    BaseUrl="https://mysite.com" 
    DefaultImage="/images/default-social.jpg" />
```

## Components

### ContentView

The primary component for displaying a single content item.

```razor
<ContentView 
    Path="blog/my-post.md"
    ProviderId="github" 
    RenderOptions="@renderOptions" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Path` | string | | The path to the content item |
| `ProviderId` | string | null | Optional provider ID |
| `ShowTitle` | bool | true | Whether to show the content title |
| `ShowMetadata` | bool | true | Whether to show metadata |
| `ShowDate` | bool | true | Whether to show the publication date |
| `ShowAuthor` | bool | true | Whether to show the author |
| `ShowTags` | bool | true | Whether to show tags |
| `ShowCategories` | bool | true | Whether to show categories |
| `ShowImage` | bool | true | Whether to show the featured image |
| `TitleClass` | string | null | CSS class for the title |
| `ContentClass` | string | null | CSS class for the content |
| `MetadataClass` | string | null | CSS class for the metadata |
| `ImageClass` | string | null | CSS class for the image |
| `RenderOptions` | MarkdownRenderOptions | null | Options for markdown rendering |

### ContentList

A component for displaying a list of content items.

```razor
<ContentList 
    Directory="blog" 
    SortBy="SortField.Date" 
    SortDirection="SortDirection.Descending" 
    Count="10" 
    ProviderId="github" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Directory` | string | null | Filter by directory |
| `Tag` | string | null | Filter by tag |
| `Category` | string | null | Filter by category |
| `Author` | string | null | Filter by author |
| `Query` | string | null | Search query |
| `Count` | int? | null | Maximum number of items |
| `Skip` | int | 0 | Number of items to skip |
| `SortBy` | SortField | Date | Field to sort by |
| `SortDirection` | SortDirection | Descending | Sort direction |
| `ProviderId` | string | null | Optional provider ID |
| `IsFeatured` | bool? | null | Filter by featured status |
| `HasFeaturedImage` | bool? | null | Filter by presence of featured image |
| `FromDate` | DateTime? | null | Filter by date range (start) |
| `ToDate` | DateTime? | null | Filter by date range (end) |
| `ListClass` | string | null | CSS class for the list |
| `ItemClass` | string | null | CSS class for list items |

### SearchBox

A component for searching content.

```razor
<SearchBox 
    Placeholder="Search..." 
    MinLength="3" 
    DebounceTime="300" 
    OnSearch="HandleSearch" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Placeholder` | string | "Search..." | Placeholder text |
| `MinLength` | int | 3 | Minimum search query length |
| `DebounceTime` | int | 300 | Debounce time in milliseconds |
| `OnSearch` | EventCallback<string> | | Event callback when search is performed |
| `CssClass` | string | null | CSS class for the search box |
| `ButtonText` | string | "Search" | Text for the search button |
| `ShowButton` | bool | true | Whether to show the search button |

### DirectoryNavigation

A component for navigating content directories.

```razor
<DirectoryNavigation 
    CurrentDirectory="blog" 
    ShowItemCount="true" 
    MaxDepth="2" />
```

#### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `CurrentDirectory` | string | null | Currently active directory |
| `ProviderId` | string | null | Optional provider ID |
| `ShowItemCount` | bool | true | Whether to show item counts |
| `MaxDepth` | int? | null | Maximum directory depth to display |
| `CssClass` | string | null | CSS class for the navigation |
| `ActiveClass` | string | "active" | CSS class for active items |

### CategoriesList and TagCloud

Components for displaying content organization.

```razor
<CategoriesList 
    ShowItemCount="true" 
    ProviderId="github" />

<TagCloud 
    MaxTags="20" 
    ShowItemCount="true" 
    ProviderId="github" />
```

## Service-Based Content Retrieval

For advanced scenarios, you can directly inject and use the content service:

```razor
@using Osirion.Blazor.Cms.Core.Providers
@using Osirion.Blazor.Cms.Core.Models
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
            Count = 5
        };
        
        var result = await ContentProvider.GetItemsByQueryAsync(query);
        featuredContent = result.ToList();
    }
}
```

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.