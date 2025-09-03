---
title: "Localized Navigation - Osirion Blazor CMS"
description: "Multi-language navigation component with locale switching, hierarchical content organization, and customizable URL formatting for international websites."
category: "CMS Web Components"
subcategory: "Navigation"
tags: ["cms", "navigation", "localization", "i18n", "multilingual", "locale-switching"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "localized-navigation"
section: "components"
layout: "component"
seo:
  title: "Localized Navigation Component | Osirion Blazor CMS Documentation"
  description: "Learn how to implement multi-language navigation with locale switching and hierarchical content organization."
  keywords: ["Blazor", "CMS", "navigation", "localization", "i18n", "multilingual", "internationalization"]
  canonical: "/docs/components/cms.web/navigation/localized-navigation"
  image: "/images/components/localized-navigation-preview.jpg"
navigation:
  parent: "CMS Web Navigation"
  order: 4
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Navigation"
    link: "/docs/components/cms.web/navigation"
  - text: "Localized Navigation"
    link: "/docs/components/cms.web/navigation/localized-navigation"
---

# Localized Navigation Component

The **LocalizedNavigation** component provides comprehensive multi-language navigation functionality with locale switching, hierarchical content organization, and customizable URL formatting. It's designed for international websites that need to serve content in multiple languages while maintaining consistent navigation structure.

## Overview

This component creates a fully localized navigation experience that dynamically loads content based on the selected locale. It supports hierarchical directory structures, content items within directories, and seamless language switching with proper URL routing for each locale.

## Key Features

- **Multi-Language Support**: Automatic locale detection and switching
- **Hierarchical Navigation**: Nested directories and content items
- **Locale Selector**: Built-in language switching interface
- **Content Item Display**: Shows content within directories
- **Custom URL Formatting**: Flexible URL generation for different locales
- **Event Handling**: Click events for custom navigation logic
- **Expandable Structure**: Collapsible directory navigation
- **Loading States**: Built-in loading and empty state handling
- **SEO Optimized**: Proper URL structure for each language

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | Optional title displayed above the navigation |
| `LoadingText` | `string` | `"Loading navigation..."` | Text shown during loading |
| `NoContentText` | `string` | `"No navigation available."` | Text shown when no content exists |
| `CurrentLocale` | `string?` | `null` | Currently selected locale code |
| `OnLocaleChanged` | `EventCallback<string>` | - | Event fired when locale changes |
| `OnDirectorySelected` | `EventCallback<DirectoryItem>` | - | Event fired when directory is clicked |
| `OnContentSelected` | `EventCallback<ContentItem>` | - | Event fired when content item is clicked |
| `ExpandAllDirectories` | `bool` | `false` | Whether to expand all directories by default |
| `ExpandedDirectoryId` | `string?` | `null` | ID of directory to expand |
| `ShowContentItems` | `bool` | `true` | Whether to display content items within directories |
| `DirectoryUrlFormatter` | `Func<DirectoryItem, string>?` | `null` | Custom function to format directory URLs |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Custom function to format content URLs |
| `LocaleNameFormatter` | `Func<string, string>?` | `null` | Custom function to format locale display names |
| `EnableLocalization` | `bool` | `true` | Whether to enable localization features |

## Basic Usage

### Simple Localized Navigation

```razor
@using Osirion.Blazor.Cms.Web.Components

<LocalizedNavigation CurrentLocale="@currentLocale"
                     OnLocaleChanged="@HandleLocaleChange" />

@code {
    private string currentLocale = "en";
    
    private async Task HandleLocaleChange(string newLocale)
    {
        currentLocale = newLocale;
        await Navigation.NavigateToAsync($"/{newLocale}");
    }
}
```

### Navigation with Custom URL Formatting

```razor
<LocalizedNavigation CurrentLocale="@userLocale"
                     OnLocaleChanged="@HandleLocaleChange"
                     DirectoryUrlFormatter="@FormatDirectoryUrl"
                     ContentUrlFormatter="@FormatContentUrl"
                     LocaleNameFormatter="@FormatLocaleName" />

@code {
    private string userLocale = "en";
    
    private string FormatDirectoryUrl(DirectoryItem directory)
    {
        return $"/{userLocale}/browse/{directory.Path}";
    }
    
    private string FormatContentUrl(ContentItem item)
    {
        return $"/{userLocale}/content/{item.Slug}";
    }
    
    private string FormatLocaleName(string locale)
    {
        return locale switch
        {
            "en" => "English",
            "es" => "Español",
            "fr" => "Français",
            "de" => "Deutsch",
            "zh" => "中文",
            _ => locale.ToUpper()
        };
    }
}
```

### Event-Driven Navigation

```razor
<LocalizedNavigation CurrentLocale="@currentLocale"
                     OnDirectorySelected="@HandleDirectorySelected"
                     OnContentSelected="@HandleContentSelected"
                     ExpandedDirectoryId="@expandedDirectory" />

@code {
    private string? expandedDirectory;
    
    private async Task HandleDirectorySelected(DirectoryItem directory)
    {
        expandedDirectory = directory.Id;
        await LoadDirectoryContentsAsync(directory.Path);
    }
    
    private async Task HandleContentSelected(ContentItem item)
    {
        await Navigation.NavigateToAsync($"/{currentLocale}/content/{item.Slug}");
    }
}
```

## Advanced Examples

### Multi-Language Documentation Site

```razor
@page "/{locale}/docs/{*path}"

<div class="docs-layout">
    <aside class="docs-sidebar">
        <LocalizedNavigation Title="Documentation"
                           CurrentLocale="@Locale"
                           OnLocaleChanged="@HandleLocaleChange"
                           OnContentSelected="@HandleDocumentSelected"
                           DirectoryUrlFormatter="@FormatDocDirectoryUrl"
                           ContentUrlFormatter="@FormatDocContentUrl"
                           LocaleNameFormatter="@GetLocaleName"
                           ExpandedDirectoryId="@GetExpandedDirectory()"
                           ShowContentItems="true" />
    </aside>
    
    <main class="docs-content">
        @if (currentDocument != null)
        {
            <article class="documentation-article">
                <header>
                    <h1>@currentDocument.Title</h1>
                    @if (!string.IsNullOrEmpty(currentDocument.Description))
                    {
                        <p class="lead">@currentDocument.Description</p>
                    }
                </header>
                
                <div class="article-content">
                    @((MarkupString)currentDocument.Content)
                </div>
                
                <footer class="article-footer">
                    <div class="article-meta">
                        <small class="text-muted">
                            Last updated: @currentDocument.LastModified?.ToString("MMMM dd, yyyy")
                        </small>
                    </div>
                </footer>
            </article>
        }
    </main>
</div>

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string Path { get; set; } = string.Empty;
    
    private ContentItem? currentDocument;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Path))
        {
            currentDocument = await DocumentationService.GetLocalizedAsync($"/docs/{Path}", Locale);
        }
    }
    
    private async Task HandleLocaleChange(string newLocale)
    {
        var newPath = string.IsNullOrEmpty(Path) 
            ? $"/{newLocale}/docs" 
            : $"/{newLocale}/docs/{Path}";
        await Navigation.NavigateToAsync(newPath);
    }
    
    private async Task HandleDocumentSelected(ContentItem document)
    {
        await Navigation.NavigateToAsync($"/{Locale}/docs/{document.Path}");
    }
    
    private string FormatDocDirectoryUrl(DirectoryItem directory)
    {
        return $"/{Locale}/docs/{directory.Path}";
    }
    
    private string FormatDocContentUrl(ContentItem item)
    {
        return $"/{Locale}/docs/{item.Path}";
    }
    
    private string GetLocaleName(string locale)
    {
        return locale switch
        {
            "en" => "🇺🇸 English",
            "es" => "🇪🇸 Español",
            "fr" => "🇫🇷 Français",
            "de" => "🇩🇪 Deutsch",
            "ja" => "🇯🇵 日本語",
            "ko" => "🇰🇷 한국어",
            "zh" => "🇨🇳 中文",
            _ => $"🌍 {locale.ToUpper()}"
        };
    }
    
    private string? GetExpandedDirectory()
    {
        if (string.IsNullOrEmpty(Path)) return null;
        
        var segments = Path.Split('/');
        return segments.Length > 1 
            ? string.Join('/', segments.Take(segments.Length - 1))
            : segments[0];
    }
}
```

### E-commerce Product Navigation

```razor
@page "/{locale}/products/{*categoryPath}"

<div class="ecommerce-layout">
    <nav class="product-navigation">
        <LocalizedNavigation Title="@GetCatalogTitle(Locale)"
                           CurrentLocale="@Locale"
                           OnLocaleChanged="@HandleLocaleChange"
                           OnDirectorySelected="@HandleCategorySelected"
                           OnContentSelected="@HandleProductSelected"
                           DirectoryUrlFormatter="@FormatCategoryUrl"
                           ContentUrlFormatter="@FormatProductUrl"
                           LocaleNameFormatter="@GetCurrencyLocaleName"
                           ExpandedDirectoryId="@CategoryPath"
                           ShowContentItems="true" />
    </nav>
    
    <main class="product-content">
        @if (selectedProducts?.Any() == true)
        {
            <div class="products-grid">
                @foreach (var product in selectedProducts)
                {
                    <div class="product-card">
                        <img src="@product.ImageUrl" alt="@product.Title" class="product-image" />
                        <h3 class="product-title">@product.Title</h3>
                        <p class="product-price">@FormatPrice(product.Price, Locale)</p>
                        <button class="btn btn-primary">
                            @GetLocalizedText("add_to_cart", Locale)
                        </button>
                    </div>
                }
            </div>
        }
    </main>
</div>

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string CategoryPath { get; set; } = string.Empty;
    
    private IReadOnlyList<ContentItem>? selectedProducts;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(CategoryPath))
        {
            selectedProducts = await ProductService.GetByCategoryAsync(CategoryPath, Locale);
        }
    }
    
    private async Task HandleLocaleChange(string newLocale)
    {
        var newPath = string.IsNullOrEmpty(CategoryPath)
            ? $"/{newLocale}/products"
            : $"/{newLocale}/products/{CategoryPath}";
        await Navigation.NavigateToAsync(newPath);
    }
    
    private async Task HandleCategorySelected(DirectoryItem category)
    {
        await Navigation.NavigateToAsync($"/{Locale}/products/{category.Path}");
    }
    
    private async Task HandleProductSelected(ContentItem product)
    {
        await Navigation.NavigateToAsync($"/{Locale}/products/{CategoryPath}/{product.Slug}");
    }
    
    private string GetCatalogTitle(string locale)
    {
        return locale switch
        {
            "en" => "Product Catalog",
            "es" => "Catálogo de Productos",
            "fr" => "Catalogue de Produits",
            "de" => "Produktkatalog",
            _ => "Products"
        };
    }
    
    private string FormatCategoryUrl(DirectoryItem category)
    {
        return $"/{Locale}/products/{category.Path}";
    }
    
    private string FormatProductUrl(ContentItem product)
    {
        return $"/{Locale}/products/{CategoryPath}/{product.Slug}";
    }
    
    private string GetCurrencyLocaleName(string locale)
    {
        var currency = GetCurrency(locale);
        var name = GetLocaleName(locale);
        return $"{name} ({currency})";
    }
    
    private string GetCurrency(string locale)
    {
        return locale switch
        {
            "en" => "USD",
            "en-GB" => "GBP",
            "de" => "EUR",
            "fr" => "EUR",
            "es" => "EUR",
            "ja" => "JPY",
            _ => "USD"
        };
    }
    
    private string FormatPrice(decimal price, string locale)
    {
        var culture = CultureInfo.GetCultureInfo(locale);
        return price.ToString("C", culture);
    }
}
```

### News Portal with Regional Navigation

```razor
@page "/{locale}/news/{region?}"

<div class="news-layout">
    <header class="news-header">
        <div class="region-selector">
            <LocalizedNavigation Title="@GetNewsTitle(Locale, Region)"
                               CurrentLocale="@Locale"
                               OnLocaleChanged="@HandleLocaleChange"
                               OnDirectorySelected="@HandleNewsSection"
                               OnContentSelected="@HandleArticleSelected"
                               DirectoryUrlFormatter="@FormatNewsSectionUrl"
                               ContentUrlFormatter="@FormatNewsArticleUrl"
                               LocaleNameFormatter="@GetRegionalLocaleName"
                               ExpandAllDirectories="false"
                               ShowContentItems="true" />
        </div>
    </header>
    
    <main class="news-content">
        @if (featuredArticles?.Any() == true)
        {
            <section class="featured-news">
                <h2>@GetLocalizedText("featured_news", Locale)</h2>
                <div class="featured-grid">
                    @foreach (var article in featuredArticles.Take(3))
                    {
                        <article class="featured-article">
                            <h3><a href="@FormatNewsArticleUrl(article)">@article.Title</a></h3>
                            <p>@article.Description</p>
                            <time datetime="@article.PublishDate.ToString("yyyy-MM-dd")">
                                @article.PublishDate.ToString(GetDateFormat(Locale))
                            </time>
                        </article>
                    }
                </div>
            </section>
        }
        
        @if (recentArticles?.Any() == true)
        {
            <section class="recent-news">
                <h2>@GetLocalizedText("recent_news", Locale)</h2>
                <div class="news-list">
                    @foreach (var article in recentArticles)
                    {
                        <article class="news-item">
                            <h4><a href="@FormatNewsArticleUrl(article)">@article.Title</a></h4>
                            <p class="news-summary">@article.Description</p>
                            <div class="news-meta">
                                <span class="news-category">@article.Categories.FirstOrDefault()</span>
                                <time datetime="@article.PublishDate.ToString("yyyy-MM-dd")">
                                    @article.PublishDate.ToString(GetDateFormat(Locale))
                                </time>
                            </div>
                        </article>
                    }
                </div>
            </section>
        }
    </main>
</div>

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string? Region { get; set; }
    
    private IReadOnlyList<ContentItem>? featuredArticles;
    private IReadOnlyList<ContentItem>? recentArticles;
    
    protected override async Task OnParametersSetAsync()
    {
        featuredArticles = await NewsService.GetFeaturedAsync(Locale, Region, 6);
        recentArticles = await NewsService.GetRecentAsync(Locale, Region, 10);
    }
    
    private async Task HandleLocaleChange(string newLocale)
    {
        var newPath = string.IsNullOrEmpty(Region)
            ? $"/{newLocale}/news"
            : $"/{newLocale}/news/{Region}";
        await Navigation.NavigateToAsync(newPath);
    }
    
    private async Task HandleNewsSection(DirectoryItem section)
    {
        var path = string.IsNullOrEmpty(Region)
            ? $"/{Locale}/news/{section.Path}"
            : $"/{Locale}/news/{Region}/{section.Path}";
        await Navigation.NavigateToAsync(path);
    }
    
    private async Task HandleArticleSelected(ContentItem article)
    {
        await Navigation.NavigateToAsync($"/{Locale}/news/{article.Path}");
    }
    
    private string GetNewsTitle(string locale, string? region)
    {
        var baseTitle = locale switch
        {
            "en" => "News",
            "es" => "Noticias",
            "fr" => "Actualités",
            "de" => "Nachrichten",
            _ => "News"
        };
        
        if (!string.IsNullOrEmpty(region))
        {
            var regionName = GetRegionName(region, locale);
            return $"{baseTitle} - {regionName}";
        }
        
        return baseTitle;
    }
    
    private string FormatNewsSectionUrl(DirectoryItem section)
    {
        return string.IsNullOrEmpty(Region)
            ? $"/{Locale}/news/{section.Path}"
            : $"/{Locale}/news/{Region}/{section.Path}";
    }
    
    private string FormatNewsArticleUrl(ContentItem article)
    {
        return $"/{Locale}/news/{article.Path}";
    }
    
    private string GetRegionalLocaleName(string locale)
    {
        var flag = GetFlagEmoji(locale);
        var name = GetLocaleName(locale);
        var region = !string.IsNullOrEmpty(Region) ? $" ({GetRegionName(Region, locale)})" : "";
        return $"{flag} {name}{region}";
    }
    
    private string GetRegionName(string region, string locale)
    {
        return (region.ToLower(), locale) switch
        {
            ("us", "en") => "United States",
            ("us", "es") => "Estados Unidos",
            ("europe", "en") => "Europe",
            ("europe", "es") => "Europa",
            ("europe", "fr") => "Europe",
            ("asia", "en") => "Asia",
            ("asia", "zh") => "亚洲",
            _ => region.ToUpper()
        };
    }
    
    private string GetDateFormat(string locale)
    {
        return CultureInfo.GetCultureInfo(locale).DateTimeFormat.ShortDatePattern;
    }
}
```

### Learning Management System Navigation

```razor
@page "/{locale}/courses/{*coursePath}"

<div class="lms-layout">
    <aside class="course-navigation">
        <LocalizedNavigation Title="@GetCoursesTitle(Locale)"
                           CurrentLocale="@Locale"
                           OnLocaleChanged="@HandleLocaleChange"
                           OnDirectorySelected="@HandleCourseSelected"
                           OnContentSelected="@HandleLessonSelected"
                           DirectoryUrlFormatter="@FormatCourseUrl"
                           ContentUrlFormatter="@FormatLessonUrl"
                           LocaleNameFormatter="@GetEducationLocaleName"
                           ExpandedDirectoryId="@GetCurrentCourseId()"
                           ShowContentItems="true" />
        
        @if (currentCourse != null)
        {
            <div class="course-progress">
                <h4>@GetLocalizedText("progress", Locale)</h4>
                <div class="progress">
                    <div class="progress-bar" style="width: @(courseProgress)%"></div>
                </div>
                <small>@courseProgress% @GetLocalizedText("complete", Locale)</small>
            </div>
        }
    </aside>
    
    <main class="lesson-content">
        @if (currentLesson != null)
        {
            <article class="lesson">
                <header class="lesson-header">
                    <h1>@currentLesson.Title</h1>
                    <div class="lesson-meta">
                        <span class="lesson-duration">
                            @GetLocalizedText("duration", Locale): @currentLesson.Metadata?.GetValue("duration", "N/A")
                        </span>
                        <span class="lesson-difficulty">
                            @GetLocalizedText("difficulty", Locale): @currentLesson.Metadata?.GetValue("difficulty", "Beginner")
                        </span>
                    </div>
                </header>
                
                <div class="lesson-content">
                    @((MarkupString)currentLesson.Content)
                </div>
                
                <footer class="lesson-actions">
                    @if (previousLesson != null)
                    {
                        <a href="@FormatLessonUrl(previousLesson)" class="btn btn-outline-primary">
                            ← @GetLocalizedText("previous", Locale)
                        </a>
                    }
                    
                    @if (nextLesson != null)
                    {
                        <a href="@FormatLessonUrl(nextLesson)" class="btn btn-primary">
                            @GetLocalizedText("next", Locale) →
                        </a>
                    }
                    
                    <button @onclick="MarkLessonComplete" class="btn btn-success">
                        @GetLocalizedText("mark_complete", Locale)
                    </button>
                </footer>
            </article>
        }
    </main>
</div>

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string CoursePath { get; set; } = string.Empty;
    
    private ContentItem? currentCourse;
    private ContentItem? currentLesson;
    private ContentItem? previousLesson;
    private ContentItem? nextLesson;
    private double courseProgress = 0;
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(CoursePath))
        {
            var pathSegments = CoursePath.Split('/');
            if (pathSegments.Length >= 2)
            {
                var courseId = pathSegments[0];
                var lessonId = pathSegments[1];
                
                currentCourse = await LearningService.GetCourseAsync(courseId, Locale);
                currentLesson = await LearningService.GetLessonAsync(courseId, lessonId, Locale);
                
                var navigation = await LearningService.GetLessonNavigationAsync(courseId, lessonId, Locale);
                previousLesson = navigation.Previous;
                nextLesson = navigation.Next;
                
                courseProgress = await LearningService.GetCourseProgressAsync(courseId);
            }
        }
    }
    
    private async Task HandleLocaleChange(string newLocale)
    {
        var newPath = string.IsNullOrEmpty(CoursePath)
            ? $"/{newLocale}/courses"
            : $"/{newLocale}/courses/{CoursePath}";
        await Navigation.NavigateToAsync(newPath);
    }
    
    private async Task HandleCourseSelected(DirectoryItem course)
    {
        await Navigation.NavigateToAsync($"/{Locale}/courses/{course.Path}");
    }
    
    private async Task HandleLessonSelected(ContentItem lesson)
    {
        await Navigation.NavigateToAsync($"/{Locale}/courses/{lesson.Path}");
    }
    
    private string GetCoursesTitle(string locale)
    {
        return locale switch
        {
            "en" => "Courses",
            "es" => "Cursos",
            "fr" => "Cours",
            "de" => "Kurse",
            "zh" => "课程",
            _ => "Courses"
        };
    }
    
    private string FormatCourseUrl(DirectoryItem course)
    {
        return $"/{Locale}/courses/{course.Path}";
    }
    
    private string FormatLessonUrl(ContentItem lesson)
    {
        return $"/{Locale}/courses/{lesson.Path}";
    }
    
    private string GetEducationLocaleName(string locale)
    {
        var flag = GetFlagEmoji(locale);
        var name = GetLocaleName(locale);
        var level = GetLanguageLevel(locale);
        return $"{flag} {name} {level}";
    }
    
    private string GetLanguageLevel(string locale)
    {
        // This could be based on user's proficiency in each language
        return locale switch
        {
            "en" => "(Native)",
            "es" => "(Intermediate)",
            "fr" => "(Beginner)",
            _ => ""
        };
    }
    
    private string? GetCurrentCourseId()
    {
        if (string.IsNullOrEmpty(CoursePath)) return null;
        
        var segments = CoursePath.Split('/');
        return segments.Length > 0 ? segments[0] : null;
    }
    
    private async Task MarkLessonComplete()
    {
        if (currentLesson != null)
        {
            await LearningService.MarkLessonCompleteAsync(currentLesson.Id);
            courseProgress = await LearningService.GetCourseProgressAsync(GetCurrentCourseId());
            StateHasChanged();
        }
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-localized-navigation {
    /* Main navigation container */
}

.osirion-nav-title {
    /* Navigation title */
    margin-bottom: 1rem;
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--bs-heading-color);
}

.osirion-locale-selector {
    /* Locale selector container */
    display: flex;
    flex-wrap: wrap;
    gap: 0.25rem;
    margin-bottom: 1rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid var(--bs-border-color);
}

.osirion-locale-button {
    /* Individual locale button */
    background: var(--bs-light);
    border: 1px solid var(--bs-border-color);
    border-radius: 0.25rem;
    padding: 0.25rem 0.75rem;
    font-size: 0.875rem;
    cursor: pointer;
    transition: all 0.2s ease;
}

.osirion-locale-button:hover {
    background: var(--bs-primary);
    color: white;
    border-color: var(--bs-primary);
}

.osirion-locale-button.osirion-active {
    background: var(--bs-primary);
    color: white;
    border-color: var(--bs-primary);
}

.osirion-nav-list {
    /* Main navigation list */
    list-style: none;
    padding: 0;
    margin: 0;
}

.osirion-nav-item {
    /* Navigation item */
    margin-bottom: 0.25rem;
}

.osirion-nav-link {
    /* Directory/section links */
    display: block;
    padding: 0.5rem 0.75rem;
    color: var(--bs-body-color);
    text-decoration: none;
    border-radius: 0.25rem;
    font-weight: 500;
    transition: all 0.2s ease;
}

.osirion-nav-link:hover {
    background: var(--bs-light);
    color: var(--bs-primary);
}

.osirion-nav-sublist {
    /* Nested navigation list */
    list-style: none;
    padding-left: 1rem;
    margin: 0.25rem 0 0 0;
    border-left: 2px solid var(--bs-border-color);
}

.osirion-nav-content-item {
    /* Individual content item */
    margin-bottom: 0.125rem;
}

.osirion-nav-content-link {
    /* Content item links */
    display: block;
    padding: 0.375rem 0.5rem;
    color: var(--bs-text-muted);
    text-decoration: none;
    border-radius: 0.25rem;
    font-size: 0.875rem;
    transition: all 0.2s ease;
}

.osirion-nav-content-link:hover {
    background: var(--bs-light);
    color: var(--bs-primary);
}

.osirion-loading,
.osirion-no-directories {
    /* Loading and empty states */
    text-align: center;
    padding: 2rem;
    color: var(--bs-text-muted);
    font-style: italic;
}
```

### Custom Styling Examples

#### Modern Language Selector
```css
.osirion-locale-selector {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    gap: 0.5rem;
    margin-bottom: 1.5rem;
}

.osirion-locale-button {
    text-align: center;
    padding: 0.5rem;
    border-radius: 0.5rem;
    font-weight: 500;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.osirion-locale-button:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}
```

#### Hierarchical Visual Design
```css
.osirion-nav-sublist {
    position: relative;
    padding-left: 1.5rem;
    border-left: none;
}

.osirion-nav-sublist::before {
    content: "";
    position: absolute;
    left: 0;
    top: 0;
    bottom: 0;
    width: 2px;
    background: linear-gradient(to bottom, var(--bs-primary), transparent);
}

.osirion-nav-content-link {
    position: relative;
    padding-left: 1.5rem;
}

.osirion-nav-content-link::before {
    content: "📄";
    position: absolute;
    left: 0.25rem;
    font-size: 0.75rem;
}
```

## Localization Best Practices

### URL Structure Recommendations

```razor
<!-- Good URL patterns for localized navigation -->
/{locale}/docs/{section}/{page}           <!-- Documentation -->
/{locale}/products/{category}/{product}   <!-- E-commerce -->
/{locale}/news/{region}/{article}         <!-- News -->
/{locale}/courses/{course}/{lesson}       <!-- Learning -->
```

### SEO-Friendly Implementation

```razor
<LocalizedNavigation CurrentLocale="@currentLocale" />

<!-- Add hreflang attributes for SEO -->
@if (availableLocales?.Any() == true)
{
    @foreach (var locale in availableLocales)
    {
        <link rel="alternate" hreflang="@locale" href="@GetLocalizedUrl(locale)" />
    }
    <link rel="alternate" hreflang="x-default" href="@GetDefaultUrl()" />
}

@code {
    private string GetLocalizedUrl(string locale)
    {
        return $"{NavigationManager.BaseUri.TrimEnd('/')}/{locale}{GetCurrentPath()}";
    }
}
```

### Performance Optimization

#### Locale-Specific Caching

```razor
@implements IDisposable

<LocalizedNavigation CurrentLocale="@currentLocale" />

@code {
    private readonly MemoryCache navigationCache = new MemoryCache(new MemoryCacheOptions());
    
    private async Task<IReadOnlyList<DirectoryItem>> GetCachedNavigation(string locale)
    {
        var cacheKey = $"navigation_{locale}";
        
        if (!navigationCache.TryGetValue(cacheKey, out IReadOnlyList<DirectoryItem>? navigation))
        {
            navigation = await NavigationService.GetForLocaleAsync(locale);
            navigationCache.Set(cacheKey, navigation, TimeSpan.FromMinutes(30));
        }
        
        return navigation ?? new List<DirectoryItem>();
    }
    
    public void Dispose()
    {
        navigationCache.Dispose();
    }
}
```

## Common Use Cases

- **Documentation Sites**: Multi-language technical documentation
- **E-commerce Platforms**: International product catalogs
- **News Portals**: Regional and international news navigation
- **Educational Platforms**: Multi-language course navigation
- **Corporate Websites**: Global company site navigation
- **Government Portals**: Multi-language public services
- **Tourism Sites**: International destination guides

## Best Practices

1. **Consistent URL Structure**: Use consistent patterns across all locales
2. **Fallback Handling**: Provide fallbacks when content isn't available in selected locale
3. **SEO Optimization**: Implement proper hreflang attributes
4. **User Experience**: Remember user's locale preference
5. **Performance**: Cache navigation data per locale
6. **Accessibility**: Ensure proper language attributes
7. **Cultural Considerations**: Respect cultural differences in navigation patterns
8. **Testing**: Test navigation with different locale combinations

The LocalizedNavigation component provides a comprehensive solution for building truly international navigation experiences with proper localization support and cultural awareness.
