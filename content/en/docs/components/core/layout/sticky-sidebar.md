---
id: 'osirion-sticky-sidebar'
order: 4
layout: docs
title: OsirionStickySidebar Component
permalink: /docs/components/core/layout/sticky-sidebar
description: Learn how to use the OsirionStickySidebar component to create sticky sidebars that remain positioned while scrolling with customizable offset and styling.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Layout
tags:
- blazor
- sidebar
- layout
- sticky
- navigation
- scrolling
- responsive
is_featured: true
published: true
slug: components/core/layout/sticky-sidebar
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionStickySidebar Component - Sticky Sidebars | Osirion.Blazor'
  description: 'Create sticky sidebars with the OsirionStickySidebar component. Features customizable positioning, scrollbar hiding, and responsive behavior.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/layout/sticky-sidebar'
  lang: en
  robots: index, follow
  og_title: 'OsirionStickySidebar Component - Osirion.Blazor'
  og_description: 'Create sticky sidebars that remain positioned while scrolling with customizable styling.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionStickySidebar Component - Osirion.Blazor'
  twitter_description: 'Create sticky sidebars with customizable positioning and responsive behavior.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionStickySidebar Component

The OsirionStickySidebar component creates sidebars that remain positioned while users scroll through content. It provides customizable top offset positioning, scrollbar management, and responsive behavior for optimal user experience.

## Component Overview

OsirionStickySidebar is designed for creating navigation sidebars, table of contents, or supplementary content areas that need to remain visible as users scroll. It handles the complex CSS positioning and provides options for customizing the sticky behavior.

### Key Features

**Sticky Positioning**: Automatically sticks to the top of the viewport while scrolling
**Customizable Offset**: Adjustable top offset to account for fixed headers or navigation
**Scrollbar Management**: Option to hide scrollbars for cleaner appearance
**Responsive Design**: Adapts to different screen sizes and orientations
**Accessibility Compliant**: Proper semantic markup with aside element
**Performance Optimized**: Efficient CSS-based sticky positioning
**Framework Agnostic**: Works with any CSS framework or custom styles
**Flexible Content**: Supports any type of sidebar content
**Non-Sticky Mode**: Option to disable sticky behavior when needed

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | The content to display in the sidebar. |
| `TopOffset` | `int` | `40` (2.5rem) | The offset from the top of the viewport in pixels. |
| `HideScrollbar` | `bool` | `true` | Whether to hide the scrollbar for cleaner appearance. |
| `IsSticky` | `bool` | `true` | Whether the sidebar should stick to the top while scrolling. |

## Basic Usage

### Simple Sticky Sidebar

```razor
@using Osirion.Blazor.Components

<div class="container-fluid">
    <div class="row">
        <div class="col-md-3">
            <OsirionStickySidebar>
                <nav class="sidebar-nav">
                    <h5>Navigation</h5>
                    <ul class="nav flex-column">
                        <li class="nav-item">
                            <a class="nav-link" href="#section1">Getting Started</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#section2">Components</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#section3">Examples</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#section4">API Reference</a>
                        </li>
                    </ul>
                </nav>
            </OsirionStickySidebar>
        </div>
        
        <div class="col-md-9">
            <main class="main-content">
                <!-- Your main content here -->
                <section id="section1">
                    <h2>Getting Started</h2>
                    <p>Content for getting started...</p>
                </section>
                
                <section id="section2">
                    <h2>Components</h2>
                    <p>Content about components...</p>
                </section>
                
                <!-- More sections... -->
            </main>
        </div>
    </div>
</div>
```

### Sidebar with Custom Top Offset

```razor
<OsirionStickySidebar TopOffset="80">
    <div class="sidebar-content">
        <h4>Table of Contents</h4>
        <nav class="toc">
            <a href="#introduction" class="toc-link">Introduction</a>
            <a href="#installation" class="toc-link">Installation</a>
            <a href="#configuration" class="toc-link">Configuration</a>
            <a href="#usage" class="toc-link">Usage</a>
            <a href="#examples" class="toc-link">Examples</a>
        </nav>
    </div>
</OsirionStickySidebar>

<style>
.sidebar-content {
    padding: 1rem;
    background: #f8f9fa;
    border-radius: 0.5rem;
}

.toc {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.toc-link {
    padding: 0.5rem;
    text-decoration: none;
    color: #495057;
    border-radius: 0.25rem;
    transition: all 0.2s ease;
}

.toc-link:hover {
    background: #e9ecef;
    color: #007bff;
    text-decoration: none;
}
</style>
```

## Advanced Usage

### Documentation Sidebar with Active State Tracking

```razor
@inject IJSRuntime JSRuntime

<OsirionStickySidebar TopOffset="@headerHeight" Class="docs-sidebar">
    <div class="sidebar-header">
        <h4>Documentation</h4>
        <div class="search-box">
            <input type="search" @bind="searchTerm" @oninput="FilterSections" 
                   placeholder="Search sections..." class="form-control form-control-sm" />
        </div>
    </div>
    
    <nav class="docs-nav" aria-label="Documentation navigation">
        @foreach (var section in filteredSections)
        {
            <div class="nav-section">
                <a href="@section.Anchor" 
                   class="nav-section-title @(activeSectionId == section.Id ? "active" : "")"
                   @onclick="() => SetActiveSection(section.Id)">
                    @section.Title
                </a>
                
                @if (section.Subsections.Any())
                {
                    <div class="nav-subsections @(activeSectionId == section.Id ? "expanded" : "")">
                        @foreach (var subsection in section.Subsections)
                        {
                            <a href="@subsection.Anchor" 
                               class="nav-subsection-link @(activeSubsectionId == subsection.Id ? "active" : "")"
                               @onclick="() => SetActiveSubsection(subsection.Id)">
                                @subsection.Title
                            </a>
                        }
                    </div>
                }
            </div>
        }
    </nav>
    
    <div class="sidebar-footer">
        <div class="progress-indicator">
            <div class="progress-label">Reading Progress</div>
            <div class="progress">
                <div class="progress-bar" style="width: @(readingProgress)%"></div>
            </div>
        </div>
    </div>
</OsirionStickySidebar>

@code {
    private int headerHeight = 80;
    private string searchTerm = "";
    private string activeSectionId = "";
    private string activeSubsectionId = "";
    private double readingProgress = 0;
    
    private List<DocSection> allSections = new();
    private List<DocSection> filteredSections = new();
    
    protected override async Task OnInitializedAsync()
    {
        allSections = GetDocumentationSections();
        filteredSections = allSections;
        
        // Set up intersection observer for active section tracking
        await SetupSectionObserver();
        
        // Set up scroll progress tracking
        await SetupScrollProgress();
    }
    
    private void FilterSections()
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            filteredSections = allSections;
        }
        else
        {
            filteredSections = allSections
                .Where(s => s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           s.Subsections.Any(sub => sub.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
        StateHasChanged();
    }
    
    private void SetActiveSection(string sectionId)
    {
        activeSectionId = sectionId;
        activeSubsectionId = "";
    }
    
    private void SetActiveSubsection(string subsectionId)
    {
        activeSubsectionId = subsectionId;
    }
    
    private async Task SetupSectionObserver()
    {
        await JSRuntime.InvokeVoidAsync("observeSections", DotNetObjectReference.Create(this));
    }
    
    private async Task SetupScrollProgress()
    {
        await JSRuntime.InvokeVoidAsync("trackScrollProgress", DotNetObjectReference.Create(this));
    }
    
    [JSInvokable]
    public void UpdateActiveSection(string sectionId)
    {
        activeSectionId = sectionId;
        StateHasChanged();
    }
    
    [JSInvokable]
    public void UpdateReadingProgress(double progress)
    {
        readingProgress = Math.Round(progress);
        StateHasChanged();
    }
    
    private List<DocSection> GetDocumentationSections()
    {
        return new List<DocSection>
        {
            new("introduction", "Introduction", "#introduction", new List<DocSubsection>
            {
                new("overview", "Overview", "#overview"),
                new("getting-started", "Getting Started", "#getting-started")
            }),
            new("installation", "Installation", "#installation", new List<DocSubsection>
            {
                new("prerequisites", "Prerequisites", "#prerequisites"),
                new("package-manager", "Package Manager", "#package-manager"),
                new("manual-setup", "Manual Setup", "#manual-setup")
            }),
            new("components", "Components", "#components", new List<DocSubsection>
            {
                new("core-components", "Core Components", "#core-components"),
                new("layout-components", "Layout Components", "#layout-components"),
                new("form-components", "Form Components", "#form-components")
            }),
            new("examples", "Examples", "#examples", new List<DocSubsection>
            {
                new("basic-usage", "Basic Usage", "#basic-usage"),
                new("advanced-patterns", "Advanced Patterns", "#advanced-patterns"),
                new("real-world-examples", "Real-world Examples", "#real-world-examples")
            })
        };
    }
    
    public record DocSection(string Id, string Title, string Anchor, List<DocSubsection> Subsections);
    public record DocSubsection(string Id, string Title, string Anchor);
}

<style>
.docs-sidebar {
    background: #ffffff;
    border-right: 1px solid #e9ecef;
    height: calc(100vh - var(--osirion-header-height, 80px));
    overflow-y: auto;
    padding: 0;
}

.sidebar-header {
    padding: 1.5rem 1rem 1rem;
    border-bottom: 1px solid #f1f3f4;
    background: #fafbfc;
    position: sticky;
    top: 0;
    z-index: 10;
}

.sidebar-header h4 {
    margin: 0 0 1rem 0;
    font-size: 1.125rem;
    font-weight: 600;
    color: #495057;
}

.search-box {
    position: relative;
}

.docs-nav {
    padding: 1rem 0;
}

.nav-section {
    margin-bottom: 0.5rem;
}

.nav-section-title {
    display: block;
    padding: 0.75rem 1rem;
    color: #495057;
    text-decoration: none;
    font-weight: 500;
    border-left: 3px solid transparent;
    transition: all 0.2s ease;
}

.nav-section-title:hover {
    color: #007bff;
    background: #f8f9fa;
    text-decoration: none;
}

.nav-section-title.active {
    color: #007bff;
    background: #e3f2fd;
    border-left-color: #007bff;
    font-weight: 600;
}

.nav-subsections {
    max-height: 0;
    overflow: hidden;
    transition: max-height 0.3s ease;
}

.nav-subsections.expanded {
    max-height: 500px;
}

.nav-subsection-link {
    display: block;
    padding: 0.5rem 1rem 0.5rem 2rem;
    color: #6c757d;
    text-decoration: none;
    font-size: 0.875rem;
    border-left: 3px solid transparent;
    transition: all 0.2s ease;
}

.nav-subsection-link:hover {
    color: #495057;
    background: #f8f9fa;
    text-decoration: none;
}

.nav-subsection-link.active {
    color: #007bff;
    background: #e3f2fd;
    border-left-color: #007bff;
}

.sidebar-footer {
    padding: 1rem;
    border-top: 1px solid #f1f3f4;
    background: #fafbfc;
    margin-top: auto;
}

.progress-indicator {
    margin-bottom: 0;
}

.progress-label {
    font-size: 0.75rem;
    color: #6c757d;
    margin-bottom: 0.5rem;
}

.progress {
    height: 4px;
    background: #e9ecef;
    border-radius: 2px;
    overflow: hidden;
}

.progress-bar {
    height: 100%;
    background: linear-gradient(90deg, #007bff, #0056b3);
    transition: width 0.3s ease;
    border-radius: 2px;
}

/* Hide scrollbar in sidebar */
.osirion-sticky-sidebar.osirion-no-scrollbar {
    scrollbar-width: none;
    -ms-overflow-style: none;
}

.osirion-sticky-sidebar.osirion-no-scrollbar::-webkit-scrollbar {
    display: none;
}

@media (max-width: 768px) {
    .docs-sidebar {
        position: fixed;
        top: var(--osirion-header-height, 80px);
        left: -100%;
        width: 280px;
        z-index: 1000;
        transition: left 0.3s ease;
        box-shadow: 2px 0 10px rgba(0, 0, 0, 0.1);
    }
    
    .docs-sidebar.mobile-open {
        left: 0;
    }
}
</style>

<script>
window.observeSections = (dotNetHelper) => {
    const sections = document.querySelectorAll('[id^="section"], h2[id], h3[id]');
    const options = {
        rootMargin: '-20% 0% -70% 0%',
        threshold: 0
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                dotNetHelper.invokeMethodAsync('UpdateActiveSection', entry.target.id);
            }
        });
    }, options);
    
    sections.forEach(section => observer.observe(section));
};

window.trackScrollProgress = (dotNetHelper) => {
    const updateProgress = () => {
        const winScroll = document.body.scrollTop || document.documentElement.scrollTop;
        const height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
        const scrolled = (winScroll / height) * 100;
        
        dotNetHelper.invokeMethodAsync('UpdateReadingProgress', scrolled || 0);
    };
    
    window.addEventListener('scroll', updateProgress);
    updateProgress(); // Initial call
};
</script>
```

### E-commerce Sidebar with Filters

```razor
<div class="container-fluid">
    <div class="row">
        <div class="col-lg-3">
            <OsirionStickySidebar TopOffset="60" Class="filters-sidebar">
                <div class="filters-container">
                    <div class="filters-header">
                        <h4>Filters</h4>
                        <button class="btn btn-sm btn-outline-secondary" @onclick="ClearAllFilters">
                            Clear All
                        </button>
                    </div>
                    
                    <!-- Category Filter -->
                    <div class="filter-section">
                        <h5 class="filter-title">Category</h5>
                        <div class="filter-options">
                            @foreach (var category in categories)
                            {
                                <label class="filter-option">
                                    <input type="checkbox" @bind="category.IsSelected" 
                                           @onchange="() => UpdateFilters()" />
                                    <span class="filter-label">@category.Name</span>
                                    <span class="filter-count">(@category.Count)</span>
                                </label>
                            }
                        </div>
                    </div>
                    
                    <!-- Price Range Filter -->
                    <div class="filter-section">
                        <h5 class="filter-title">Price Range</h5>
                        <div class="price-range">
                            <div class="price-inputs">
                                <input type="number" @bind="minPrice" @onchange="UpdateFilters" 
                                       placeholder="Min" class="form-control form-control-sm" />
                                <span>to</span>
                                <input type="number" @bind="maxPrice" @onchange="UpdateFilters" 
                                       placeholder="Max" class="form-control form-control-sm" />
                            </div>
                            <div class="price-slider">
                                <input type="range" @bind="minPrice" min="0" max="1000" 
                                       class="form-range" @onchange="UpdateFilters" />
                                <input type="range" @bind="maxPrice" min="0" max="1000" 
                                       class="form-range" @onchange="UpdateFilters" />
                            </div>
                        </div>
                    </div>
                    
                    <!-- Rating Filter -->
                    <div class="filter-section">
                        <h5 class="filter-title">Rating</h5>
                        <div class="filter-options">
                            @for (int rating = 5; rating >= 1; rating--)
                            {
                                int currentRating = rating;
                                <label class="filter-option rating-option">
                                    <input type="radio" name="rating" value="@currentRating" 
                                           @onchange="() => SetRatingFilter(currentRating)" />
                                    <span class="rating-stars">
                                        @for (int i = 1; i <= 5; i++)
                                        {
                                            <span class="star @(i <= currentRating ? "filled" : "")">★</span>
                                        }
                                    </span>
                                    <span class="rating-text">& Up</span>
                                </label>
                            }
                        </div>
                    </div>
                    
                    <!-- Active Filters Summary -->
                    @if (GetActiveFiltersCount() > 0)
                    {
                        <div class="active-filters">
                            <h6>Active Filters (@GetActiveFiltersCount())</h6>
                            <div class="filter-tags">
                                @foreach (var category in categories.Where(c => c.IsSelected))
                                {
                                    <span class="filter-tag">
                                        @category.Name
                                        <button @onclick="() => RemoveCategoryFilter(category.Name)">×</button>
                                    </span>
                                }
                                @if (minPrice > 0 || maxPrice < 1000)
                                {
                                    <span class="filter-tag">
                                        $@minPrice - $@maxPrice
                                        <button @onclick="ClearPriceFilter">×</button>
                                    </span>
                                }
                                @if (selectedRating > 0)
                                {
                                    <span class="filter-tag">
                                        @selectedRating+ Stars
                                        <button @onclick="() => SetRatingFilter(0)">×</button>
                                    </span>
                                }
                            </div>
                        </div>
                    }
                </div>
            </OsirionStickySidebar>
        </div>
        
        <div class="col-lg-9">
            <div class="products-container">
                <div class="results-header">
                    <h3>Products (@filteredProductsCount)</h3>
                    <div class="sort-options">
                        <select @bind="sortBy" @onchange="UpdateFilters" class="form-select">
                            <option value="relevance">Sort by Relevance</option>
                            <option value="price-low">Price: Low to High</option>
                            <option value="price-high">Price: High to Low</option>
                            <option value="rating">Highest Rated</option>
                            <option value="newest">Newest</option>
                        </select>
                    </div>
                </div>
                
                <!-- Products grid would go here -->
                <div class="products-grid">
                    <!-- Product items -->
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private List<CategoryFilter> categories = new();
    private decimal minPrice = 0;
    private decimal maxPrice = 1000;
    private int selectedRating = 0;
    private string sortBy = "relevance";
    private int filteredProductsCount = 0;
    
    protected override void OnInitialized()
    {
        categories = new List<CategoryFilter>
        {
            new("Electronics", 156),
            new("Clothing", 89),
            new("Books", 234),
            new("Home & Garden", 67),
            new("Sports", 45),
            new("Toys", 78)
        };
        
        UpdateFilters();
    }
    
    private void UpdateFilters()
    {
        // Apply filters and update product count
        // This would typically call a service to filter products
        filteredProductsCount = CalculateFilteredCount();
        StateHasChanged();
    }
    
    private void ClearAllFilters()
    {
        foreach (var category in categories)
        {
            category.IsSelected = false;
        }
        minPrice = 0;
        maxPrice = 1000;
        selectedRating = 0;
        UpdateFilters();
    }
    
    private void RemoveCategoryFilter(string categoryName)
    {
        var category = categories.FirstOrDefault(c => c.Name == categoryName);
        if (category != null)
        {
            category.IsSelected = false;
            UpdateFilters();
        }
    }
    
    private void ClearPriceFilter()
    {
        minPrice = 0;
        maxPrice = 1000;
        UpdateFilters();
    }
    
    private void SetRatingFilter(int rating)
    {
        selectedRating = rating;
        UpdateFilters();
    }
    
    private int GetActiveFiltersCount()
    {
        int count = categories.Count(c => c.IsSelected);
        if (minPrice > 0 || maxPrice < 1000) count++;
        if (selectedRating > 0) count++;
        return count;
    }
    
    private int CalculateFilteredCount()
    {
        // Mock calculation - in real app, this would query your data source
        return 156;
    }
    
    public class CategoryFilter
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool IsSelected { get; set; }
        
        public CategoryFilter(string name, int count)
        {
            Name = name;
            Count = count;
            IsSelected = false;
        }
    }
}

<style>
.filters-sidebar {
    background: #ffffff;
    border: 1px solid #e9ecef;
    border-radius: 0.5rem;
    height: calc(100vh - 100px);
    overflow-y: auto;
}

.filters-container {
    padding: 1.5rem;
}

.filters-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid #f1f3f4;
}

.filters-header h4 {
    margin: 0;
    font-size: 1.25rem;
    font-weight: 600;
}

.filter-section {
    margin-bottom: 2rem;
}

.filter-title {
    font-size: 1rem;
    font-weight: 600;
    margin-bottom: 1rem;
    color: #495057;
}

.filter-options {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
}

.filter-option {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    cursor: pointer;
    font-size: 0.875rem;
}

.filter-option input[type="checkbox"],
.filter-option input[type="radio"] {
    margin: 0;
}

.filter-label {
    flex: 1;
    color: #495057;
}

.filter-count {
    color: #6c757d;
    font-size: 0.75rem;
}

.price-range {
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

.price-inputs {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.price-inputs input {
    flex: 1;
}

.price-slider {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.rating-option {
    align-items: center;
}

.rating-stars {
    display: flex;
    gap: 0.125rem;
}

.star {
    color: #e9ecef;
    font-size: 1rem;
}

.star.filled {
    color: #ffc107;
}

.rating-text {
    font-size: 0.875rem;
    color: #6c757d;
}

.active-filters {
    padding-top: 1rem;
    border-top: 1px solid #f1f3f4;
}

.active-filters h6 {
    font-size: 0.875rem;
    font-weight: 600;
    margin-bottom: 0.75rem;
    color: #495057;
}

.filter-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
}

.filter-tag {
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.5rem;
    background: #e3f2fd;
    color: #0d47a1;
    border-radius: 1rem;
    font-size: 0.75rem;
}

.filter-tag button {
    background: none;
    border: none;
    color: inherit;
    cursor: pointer;
    font-weight: bold;
    padding: 0;
    margin-left: 0.25rem;
}

.results-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
}

.results-header h3 {
    margin: 0;
    font-size: 1.5rem;
    font-weight: 600;
}

.sort-options select {
    min-width: 200px;
}

.products-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 1.5rem;
}

@media (max-width: 992px) {
    .filters-sidebar {
        position: fixed;
        top: 60px;
        left: -100%;
        width: 320px;
        z-index: 1000;
        transition: left 0.3s ease;
        box-shadow: 2px 0 10px rgba(0, 0, 0, 0.1);
        height: calc(100vh - 60px);
    }
    
    .filters-sidebar.mobile-open {
        left: 0;
    }
}
</style>
```

## Styling Examples

### Bootstrap Integration

```razor
<OsirionStickySidebar Class="d-none d-lg-block bg-light border-end">
    <div class="p-3">
        <h5 class="border-bottom pb-2 mb-3">Quick Navigation</h5>
        <nav class="nav flex-column">
            <a class="nav-link" href="#overview">Overview</a>
            <a class="nav-link" href="#features">Features</a>
            <a class="nav-link" href="#installation">Installation</a>
            <a class="nav-link" href="#usage">Usage</a>
        </nav>
    </div>
</OsirionStickySidebar>

<style>
/* Bootstrap-compatible styling */
.osirion-sticky-sidebar {
    position: sticky;
    top: var(--osirion-header-height, 80px);
    height: calc(100vh - var(--osirion-header-height, 80px));
    overflow-y: auto;
}

.osirion-sticky-sidebar.osirion-non-sticky {
    position: static;
    height: auto;
}

.osirion-sticky-sidebar .nav-link {
    color: #495057;
    padding: 0.5rem 0.75rem;
    border-radius: 0.25rem;
    transition: all 0.2s ease;
}

.osirion-sticky-sidebar .nav-link:hover {
    color: #007bff;
    background-color: rgba(0, 123, 255, 0.1);
}

.osirion-sticky-sidebar .nav-link.active {
    color: #007bff;
    background-color: rgba(0, 123, 255, 0.1);
    font-weight: 500;
}
</style>
```

### Tailwind CSS Integration

```razor
<OsirionStickySidebar Class="hidden lg:block bg-gray-50 border-r border-gray-200">
    <div class="p-4">
        <h5 class="text-lg font-semibold border-b border-gray-200 pb-2 mb-3">Navigation</h5>
        <nav class="space-y-1">
            <a href="#section1" class="nav-item">Getting Started</a>
            <a href="#section2" class="nav-item">Components</a>
            <a href="#section3" class="nav-item">Examples</a>
        </nav>
    </div>
</OsirionStickySidebar>

<style>
/* Tailwind-compatible classes */
.osirion-sticky-sidebar {
    @apply sticky overflow-y-auto;
    top: var(--osirion-header-height, 80px);
    height: calc(100vh - var(--osirion-header-height, 80px));
}

.osirion-sticky-sidebar.osirion-non-sticky {
    @apply static h-auto;
}

.nav-item {
    @apply block px-3 py-2 text-gray-700 rounded-md transition-colors duration-200 hover:text-blue-600 hover:bg-blue-50;
}

.nav-item.active {
    @apply text-blue-600 bg-blue-50 font-medium;
}
</style>
```

## Best Practices

### Layout Guidelines

1. **Consistent Positioning**: Use consistent top offset values across your application
2. **Content Organization**: Group related navigation items logically
3. **Visual Hierarchy**: Use proper heading levels and visual emphasis
4. **Responsive Design**: Consider mobile behavior and collapsible sidebars
5. **Performance**: Avoid heavy content in sticky sidebars

### Accessibility

1. **Semantic Markup**: Use proper navigation elements and landmarks
2. **Keyboard Navigation**: Ensure all interactive elements are keyboard accessible
3. **Focus Management**: Provide clear focus indicators
4. **Screen Readers**: Use appropriate ARIA labels and roles
5. **Skip Links**: Consider adding skip navigation options

### User Experience

1. **Visual Feedback**: Indicate current page/section in navigation
2. **Loading States**: Show loading indicators for dynamic content
3. **Error Handling**: Handle cases where content fails to load
4. **Mobile Optimization**: Provide alternative navigation for mobile devices
5. **Performance**: Optimize for smooth scrolling and interactions

The OsirionStickySidebar component provides an efficient and accessible way to create sticky navigation sidebars that enhance user experience and site navigation.
