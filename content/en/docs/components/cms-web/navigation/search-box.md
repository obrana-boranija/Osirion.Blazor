---
title: "SearchBox"
description: "A Blazor component for creating accessible search forms with customizable styling and configuration for content search functionality."
keywords: 
  - "search-box"
  - "search form"
  - "content search"
  - "blazor cms"
  - "navigation"
  - "accessibility"
seo:
  title: "SearchBox - Osirion Blazor CMS Web Component"
  description: "Learn how to implement search functionality in Osirion Blazor CMS with the SearchBox component featuring accessibility support and customizable styling."
  keywords:
    - "blazor search component"
    - "cms search form"
    - "content search box"
    - "osirion blazor"
date: "2024-12-29"
---

# SearchBox

The `SearchBox` component provides a fully accessible search form for content discovery with customizable styling, action URLs, and query parameters. It generates a standard HTML form that can integrate with any search backend.

## Features

- **Accessible Design**: Full accessibility support with proper ARIA labels and roles
- **Customizable Action**: Configurable form action URL and HTTP method
- **Flexible Parameters**: Customizable query parameter names and values
- **Themeable Styling**: Works with all supported UI frameworks
- **Form Integration**: Standard HTML form that works with any backend
- **Unique IDs**: Automatic generation of unique element IDs for multiple instances
- **Screen Reader Support**: Proper labeling and hidden text for assistive technologies

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ActionUrl` | `string` | `"/search"` | The URL where the search form will be submitted |
| `QueryParameterName` | `string` | `"q"` | The name of the query parameter for the search term |
| `SearchQuery` | `string` | `""` | The current search query value |
| `Placeholder` | `string` | `"Search content..."` | Placeholder text for the search input |
| `SearchButtonText` | `string` | `"Search"` | Text displayed on the search button |
| `Title` | `string?` | `null` | Optional title displayed above the search box |

## Basic Usage

### Simple Search Box

```razor
@using Osirion.Blazor.Cms.Web.Components

<SearchBox />
```

### Search Box with Custom Action

```razor
<SearchBox 
    ActionUrl="/api/search"
    QueryParameterName="query"
    Placeholder="Search articles..." />
```

### Search Box with Title

```razor
<SearchBox 
    Title="Find Content"
    Placeholder="Enter your search terms"
    SearchButtonText="Find" />
```

## Advanced Examples

### Custom Search Configuration

```razor
<SearchBox 
    ActionUrl="@searchEndpoint"
    QueryParameterName="@queryParam"
    SearchQuery="@currentQuery"
    Title="@Localizer["SearchTitle"]"
    Placeholder="@Localizer["SearchPlaceholder"]"
    SearchButtonText="@Localizer["SearchButton"]" />

@code {
    private string searchEndpoint = "/search/advanced";
    private string queryParam = "searchTerm";
    private string currentQuery = "";

    [Inject] private IStringLocalizer<SearchResources> Localizer { get; set; } = default!;
}
```

### Multiple Search Boxes

```razor
<!-- Global site search -->
<SearchBox 
    ActionUrl="/search"
    Title="Site Search"
    Placeholder="Search all content..."
    CssClass="global-search" />

<!-- Category-specific search -->
<SearchBox 
    ActionUrl="/search/blog"
    Title="Blog Search"
    Placeholder="Search blog posts..."
    QueryParameterName="blogQuery"
    CssClass="blog-search" />

<!-- Documentation search -->
<SearchBox 
    ActionUrl="/search/docs"
    Title="Documentation"
    Placeholder="Search documentation..."
    QueryParameterName="docQuery"
    CssClass="doc-search" />
```

### Pre-filled Search

```razor
<SearchBox 
    SearchQuery="@prefilledQuery"
    ActionUrl="/search/results"
    Title="Refine Your Search" />

@code {
    [Parameter] public string? Category { get; set; }
    
    private string prefilledQuery => !string.IsNullOrEmpty(Category) ? $"category:{Category}" : "";
}
```

### Integration with Search Results

```razor
@page "/search"
@using Osirion.Blazor.Cms.Web.Components

<h1>Search</h1>

<SearchBox 
    SearchQuery="@Query"
    ActionUrl="/search"
    Title="Search Content" />

@if (!string.IsNullOrEmpty(Query))
{
    <div class="search-results">
        <h2>Results for "@Query"</h2>
        @if (searchResults != null)
        {
            @foreach (var result in searchResults)
            {
                <div class="search-result">
                    <h3><a href="@result.Url">@result.Title</a></h3>
                    <p>@result.Summary</p>
                </div>
            }
        }
    </div>
}

@code {
    [Parameter, SupplyParameterFromQuery] public string? Query { get; set; }
    [Inject] private ISearchService SearchService { get; set; } = default!;
    
    private List<SearchResult>? searchResults;

    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Query))
        {
            searchResults = await SearchService.SearchAsync(Query);
        }
    }
}
```

### AJAX Search with Real-time Results

```razor
<div class="search-container">
    <SearchBox 
        ActionUrl="javascript:void(0)"
        Placeholder="Type to search..."
        SearchButtonText="🔍"
        @onkeyup="@HandleKeyUp" />
    
    @if (liveResults != null && liveResults.Any())
    {
        <div class="live-results">
            @foreach (var result in liveResults)
            {
                <a href="@result.Url" class="live-result-item">
                    <strong>@result.Title</strong>
                    <small>@result.Summary</small>
                </a>
            }
        </div>
    }
</div>

@code {
    private List<SearchResult>? liveResults;
    private Timer? searchTimer;

    [Inject] private ISearchService SearchService { get; set; } = default!;

    private async Task HandleKeyUp(KeyboardEventArgs e)
    {
        var target = e.Target as IHtmlInputElement;
        var query = target?.Value;
        
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            liveResults = null;
            StateHasChanged();
            return;
        }

        // Debounce search requests
        searchTimer?.Dispose();
        searchTimer = new Timer(async _ =>
        {
            liveResults = await SearchService.SearchAsync(query, limit: 5);
            await InvokeAsync(StateHasChanged);
        }, null, 300, Timeout.Infinite);
    }
}
```

## CSS Customization

### Default Styling

```css
.osirion-search-container {
    margin-bottom: 1rem;
}

.osirion-search-title {
    margin-bottom: 0.5rem;
    font-size: 1.25rem;
    font-weight: 600;
}

.osirion-search-box {
    display: flex;
    gap: 0.5rem;
    align-items: stretch;
}

.osirion-search-input {
    flex: 1;
    padding: 0.5rem 0.75rem;
    border: 1px solid var(--border-color);
    border-radius: 0.375rem;
    font-size: 1rem;
}

.osirion-search-input:focus {
    outline: 2px solid var(--primary-color);
    outline-offset: 2px;
    border-color: var(--primary-color);
}

.osirion-search-button {
    padding: 0.5rem 1rem;
    background-color: var(--primary-color);
    color: white;
    border: none;
    border-radius: 0.375rem;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.2s ease;
}

.osirion-search-button:hover {
    background-color: var(--primary-color-dark);
}
```

### Compact Search

```css
.osirion-search-container.compact .osirion-search-input {
    padding: 0.375rem 0.5rem;
    font-size: 0.875rem;
}

.osirion-search-container.compact .osirion-search-button {
    padding: 0.375rem 0.75rem;
    font-size: 0.875rem;
}
```

### Full-width Search

```css
.osirion-search-container.full-width {
    width: 100%;
}

.osirion-search-container.full-width .osirion-search-box {
    width: 100%;
}

.osirion-search-container.full-width .osirion-search-input {
    min-width: 0;
}
```

### Inline Search

```css
.osirion-search-container.inline {
    display: inline-block;
    margin: 0;
}

.osirion-search-container.inline .osirion-search-box {
    display: inline-flex;
    width: auto;
}
```

## Framework-Specific Styling

### Bootstrap Integration

```razor
<SearchBox CssClass="mb-3">
    <div class="input-group">
        <input class="form-control" />
        <button class="btn btn-primary" type="submit">
            <i class="fas fa-search"></i>
        </button>
    </div>
</SearchBox>
```

### Tailwind CSS Integration

```razor
<SearchBox CssClass="mb-4">
    <div class="flex rounded-md shadow-sm">
        <input class="flex-1 rounded-l-md border-gray-300 focus:border-indigo-500 focus:ring-indigo-500" />
        <button class="bg-indigo-600 text-white px-4 py-2 rounded-r-md hover:bg-indigo-700">
            Search
        </button>
    </div>
</SearchBox>
```

## Accessibility Features

- **Semantic HTML**: Uses proper form elements and structure
- **ARIA Labels**: Comprehensive labeling for screen readers
- **Keyboard Navigation**: Full keyboard accessibility
- **Focus Management**: Proper focus indicators and behavior
- **Screen Reader Support**: Hidden labels and announcements
- **Unique IDs**: Automatic generation prevents ID conflicts

## Best Practices

1. **Search Endpoints**: Ensure your search endpoint can handle the form submission
2. **Query Validation**: Validate and sanitize search queries on the server
3. **Performance**: Implement debouncing for real-time search features
4. **Error Handling**: Provide feedback for failed searches
5. **Results Display**: Show meaningful results and empty states
6. **SEO Considerations**: Use proper URLs for search results pages

## Integration Examples

### With Search Service

```razor
@page "/search"
@using Osirion.Blazor.Cms.Web.Components

<SearchBox 
    SearchQuery="@Query"
    ActionUrl="/search"
    OnSearch="@HandleSearch" />

@if (loading)
{
    <div class="loading">Searching...</div>
}
else if (results != null)
{
    <div class="results">
        <p>Found @results.TotalCount results for "@Query"</p>
        @foreach (var item in results.Items)
        {
            <ContentView Item="@item" ShowSummary="true" />
        }
    </div>
}

@code {
    [Parameter, SupplyParameterFromQuery] public string? Query { get; set; }
    [Inject] private IContentSearchService SearchService { get; set; } = default!;
    
    private SearchResults? results;
    private bool loading;

    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Query))
        {
            await PerformSearch();
        }
    }

    private async Task HandleSearch(string query)
    {
        NavigationManager.NavigateTo($"/search?q={Uri.EscapeDataString(query)}");
    }

    private async Task PerformSearch()
    {
        loading = true;
        try
        {
            results = await SearchService.SearchContentAsync(Query!);
        }
        finally
        {
            loading = false;
        }
    }
}
```

### Global Navigation Integration

```razor
<!-- In MainLayout.razor -->
<nav class="navbar">
    <div class="navbar-brand">
        <a href="/">My Site</a>
    </div>
    
    <div class="navbar-search">
        <SearchBox 
            Placeholder="Search site..."
            CssClass="compact inline" />
    </div>
    
    <div class="navbar-menu">
        <!-- Other navigation items -->
    </div>
</nav>
```

## Related Components

- [`ContentView`](../core/content-view.md) - For displaying search results
- [`ContentList`](../core/content-list.md) - For listing search results
- [`TagCloud`](tag-cloud.md) - For tag-based search discovery
- [`CategoriesList`](categories-list.md) - For category-based navigation
