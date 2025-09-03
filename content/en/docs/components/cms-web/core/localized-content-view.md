---
title: "Localized Content View - Osirion Blazor CMS"
description: "Multi-language content display component with translation links and locale-aware rendering for international Blazor CMS applications."
category: "CMS Web Components"
subcategory: "Core"
tags: ["cms", "content", "localization", "i18n", "multilingual", "translations"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "localized-content-view"
section: "components"
layout: "component"
seo:
  title: "Localized Content View Component | Osirion Blazor CMS Documentation"
  description: "Learn how to display multi-language content with translation links using LocalizedContentView component."
  keywords: ["Blazor", "CMS", "localization", "i18n", "multilingual", "translations", "content view"]
  canonical: "/docs/components/cms.web/core/localized-content-view"
  image: "/images/components/localized-content-view-preview.jpg"
navigation:
  parent: "CMS Web Components"
  order: 5
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Core"
    link: "/docs/components/cms.web/core"
  - text: "Localized Content View"
    link: "/docs/components/cms.web/core/localized-content-view"
---

# Localized Content View Component

The **LocalizedContentView** component provides comprehensive multi-language content display capabilities with automatic translation detection, language switching, and locale-aware formatting. It's perfect for international websites that need to serve content in multiple languages.

## Overview

This component extends the standard content view with internationalization features, including automatic translation link generation, locale-specific URL formatting, and cultural date/time display. It seamlessly integrates with the Osirion CMS localization system to provide a complete multilingual content experience.

## Key Features

- **Multi-Language Support**: Automatic detection and display of available translations
- **Translation Switching**: Easy language switching with customizable URL patterns
- **Locale-Aware Formatting**: Cultural formatting for dates, numbers, and text
- **SEO Optimized**: Proper hreflang attributes and locale-specific URLs
- **Fallback Handling**: Graceful fallback to default language when translations are missing
- **Custom Locale Names**: Configurable display names for languages
- **Navigation Links**: Localized previous/next navigation with proper URL formatting
- **Content Discovery**: Automatic translation discovery based on content relationships

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `LocalizationId` | `string?` | `null` | Content localization identifier |
| `Path` | `string?` | `null` | Content path to load and display |
| `CurrentLocale` | `string` | `"en"` | Current locale/language code |
| `OnLocaleChanged` | `EventCallback<string>` | - | Callback when locale changes |
| `LoadingText` | `string` | `"Loading content..."` | Text shown during loading |
| `NotFoundText` | `string` | `"Content not found."` | Text when content is not found |
| `LocaleNameFormatter` | `Func<string, string>?` | `null` | Custom locale name formatting function |
| `TranslationUrlFormatter` | `Func<string, string, string>?` | `null` | Custom translation URL formatter |
| `CategoryUrlFormatter` | `Func<string, string>?` | `null` | Custom category URL formatter |
| `TagUrlFormatter` | `Func<string, string>?` | `null` | Custom tag URL formatter |
| `ContentUrlFormatter` | `Func<ContentItem, string>?` | `null` | Custom content URL formatter |
| `Item` | `ContentItem?` | `null` | Pre-loaded content item to display |
| `PreviousItem` | `ContentItem?` | `null` | Previous content item for navigation |
| `NextItem` | `ContentItem?` | `null` | Next content item for navigation |
| `ShowNavigationLinks` | `bool` | `false` | Enable previous/next navigation |
| `ShowJumbotron` | `bool` | `true` | Show header with title and metadata |
| `EnableLocalization` | `bool` | `true` | Enable localization features |

## Basic Usage

### Simple Localized Content

```razor
@using Osirion.Blazor.Cms.Web.Components

<LocalizedContentView Path="/articles/getting-started"
                      CurrentLocale="@currentLocale"
                      OnLocaleChanged="HandleLocaleChange" />

@code {
    private string currentLocale = "en";
    
    private async Task HandleLocaleChange(string newLocale)
    {
        currentLocale = newLocale;
        await Navigation.NavigateToAsync($"/{newLocale}/articles/getting-started");
    }
}
```

### Pre-loaded Content with Custom Formatters

```razor
<LocalizedContentView Item="@localizedArticle"
                      CurrentLocale="@userLocale"
                      LocaleNameFormatter="@FormatLocaleName"
                      TranslationUrlFormatter="@FormatTranslationUrl"
                      CategoryUrlFormatter="@FormatCategoryUrl"
                      ShowNavigationLinks="true" />

@code {
    private ContentItem? localizedArticle;
    private string userLocale = "en";
    
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
    
    private string FormatTranslationUrl(string locale, string contentId)
    {
        return $"/{locale}/articles/{contentId}";
    }
    
    private string FormatCategoryUrl(string category)
    {
        return $"/{userLocale}/category/{category.ToLower().Replace(' ', '-')}";
    }
}
```

## Advanced Examples

### Multi-Language Blog

```razor
@page "/{locale}/blog/{slug}"

<LocalizedContentView Path="@contentPath"
                      CurrentLocale="@Locale"
                      OnLocaleChanged="HandleLocaleChange"
                      LocaleNameFormatter="@GetLocaleName"
                      TranslationUrlFormatter="@FormatBlogTranslationUrl"
                      CategoryUrlFormatter="@FormatBlogCategoryUrl"
                      TagUrlFormatter="@FormatBlogTagUrl"
                      ShowNavigationLinks="true" />

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private string contentPath => $"/{Locale}/blog/{Slug}";
    
    private readonly Dictionary<string, string> localeNames = new()
    {
        { "en", "English" },
        { "es", "Español" },
        { "fr", "Français" },
        { "de", "Deutsch" },
        { "it", "Italiano" },
        { "pt", "Português" }
    };
    
    private async Task HandleLocaleChange(string newLocale)
    {
        await Navigation.NavigateToAsync($"/{newLocale}/blog/{Slug}");
    }
    
    private string GetLocaleName(string locale)
    {
        return localeNames.GetValueOrDefault(locale, locale.ToUpper());
    }
    
    private string FormatBlogTranslationUrl(string locale, string contentId)
    {
        return $"/{locale}/blog/{contentId}";
    }
    
    private string FormatBlogCategoryUrl(string category)
    {
        return $"/{Locale}/blog/category/{category.ToLower().Replace(' ', '-')}";
    }
    
    private string FormatBlogTagUrl(string tag)
    {
        return $"/{Locale}/blog/tag/{tag.ToLower().Replace(' ', '-')}";
    }
}
```

### Documentation with Version and Language Support

```razor
@page "/docs/{locale}/{version}/{*path}"

<div class="documentation-layout">
    <div class="doc-header">
        <div class="version-selector">
            <select @onchange="HandleVersionChange" value="@Version">
                @foreach (var version in availableVersions)
                {
                    <option value="@version">Version @version</option>
                }
            </select>
        </div>
        
        <div class="language-selector">
            <LocalizedContentView Path="@documentationPath"
                                  CurrentLocale="@Locale"
                                  OnLocaleChanged="HandleLocaleChange"
                                  LocaleNameFormatter="@GetDocLocaleName"
                                  TranslationUrlFormatter="@FormatDocTranslationUrl"
                                  ShowJumbotron="true"
                                  ShowNavigationLinks="true" />
        </div>
    </div>
</div>

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string Version { get; set; } = "latest";
    [Parameter] public string Path { get; set; } = string.Empty;
    
    private string documentationPath => $"/docs/{Locale}/{Version}/{Path}";
    private readonly List<string> availableVersions = new() { "latest", "v2.0", "v1.9", "v1.8" };
    
    private async Task HandleVersionChange(ChangeEventArgs e)
    {
        var newVersion = e.Value?.ToString() ?? "latest";
        await Navigation.NavigateToAsync($"/docs/{Locale}/{newVersion}/{Path}");
    }
    
    private async Task HandleLocaleChange(string newLocale)
    {
        await Navigation.NavigateToAsync($"/docs/{newLocale}/{Version}/{Path}");
    }
    
    private string GetDocLocaleName(string locale)
    {
        return locale switch
        {
            "en" => "🇺🇸 English",
            "es" => "🇪🇸 Español",
            "fr" => "🇫🇷 Français",
            "de" => "🇩🇪 Deutsch",
            "ja" => "🇯🇵 日本語",
            "ko" => "🇰🇷 한국어",
            _ => $"🌍 {locale.ToUpper()}"
        };
    }
    
    private string FormatDocTranslationUrl(string locale, string contentId)
    {
        return $"/docs/{locale}/{Version}/{contentId}";
    }
}
```

### E-commerce Product Pages

```razor
@page "/products/{locale}/{category}/{slug}"

<div class="product-page">
    <LocalizedContentView Path="@productPath"
                          CurrentLocale="@Locale"
                          OnLocaleChanged="HandleLocaleChange"
                          LocaleNameFormatter="@GetProductLocaleName"
                          TranslationUrlFormatter="@FormatProductTranslationUrl"
                          ShowJumbotron="false"
                          EnableLocalization="true" />
    
    @if (currentProduct != null)
    {
        <div class="product-actions">
            <div class="price-section">
                <span class="price">@FormatPrice(currentProduct.Price, Locale)</span>
                <span class="currency">@GetCurrency(Locale)</span>
            </div>
            
            <button class="btn btn-primary btn-lg">
                @GetLocalizedText("add_to_cart", Locale)
            </button>
        </div>
    }
</div>

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string Category { get; set; } = string.Empty;
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private string productPath => $"/products/{Locale}/{Category}/{Slug}";
    private ProductItem? currentProduct;
    
    protected override async Task OnParametersSetAsync()
    {
        currentProduct = await ProductService.GetBySlugAsync(Slug, Locale);
    }
    
    private async Task HandleLocaleChange(string newLocale)
    {
        await Navigation.NavigateToAsync($"/products/{newLocale}/{Category}/{Slug}");
    }
    
    private string GetProductLocaleName(string locale)
    {
        return CultureInfo.GetCultureInfo(locale).DisplayName;
    }
    
    private string FormatProductTranslationUrl(string locale, string contentId)
    {
        return $"/products/{locale}/{Category}/{contentId}";
    }
    
    private string FormatPrice(decimal price, string locale)
    {
        var culture = CultureInfo.GetCultureInfo(locale);
        return price.ToString("C", culture);
    }
    
    private string GetCurrency(string locale)
    {
        var region = new RegionInfo(locale);
        return region.ISOCurrencySymbol;
    }
    
    private string GetLocalizedText(string key, string locale)
    {
        // Implementation would depend on your localization system
        return Localizer[key];
    }
}
```

### News Portal with Regional Content

```razor
@page "/news/{locale}/{region?}/{slug}"

<div class="news-article">
    <div class="news-header">
        <div class="region-selector">
            @if (!string.IsNullOrEmpty(Region))
            {
                <span class="region-badge">@GetRegionName(Region)</span>
            }
        </div>
        
        <div class="share-tools">
            <button @onclick="ShareArticle" class="btn btn-outline-primary btn-sm">
                @GetLocalizedText("share", Locale)
            </button>
        </div>
    </div>
    
    <LocalizedContentView Path="@newsPath"
                          CurrentLocale="@Locale"
                          OnLocaleChanged="HandleLocaleChange"
                          LocaleNameFormatter="@GetNewsLocaleName"
                          TranslationUrlFormatter="@FormatNewsTranslationUrl"
                          CategoryUrlFormatter="@FormatNewsCategoryUrl"
                          ShowNavigationLinks="true" />
    
    @if (relatedNews?.Any() == true)
    {
        <section class="related-news">
            <h3>@GetLocalizedText("related_news", Locale)</h3>
            <div class="related-grid">
                @foreach (var related in relatedNews)
                {
                    <article class="related-item">
                        <h4><a href="@FormatNewsUrl(related)">@related.Title</a></h4>
                        <time datetime="@related.DateCreated.ToString("yyyy-MM-dd")">
                            @related.DateCreated.ToString(GetDateFormat(Locale))
                        </time>
                    </article>
                }
            </div>
        </section>
    }
</div>

@code {
    [Parameter] public string Locale { get; set; } = "en";
    [Parameter] public string? Region { get; set; }
    [Parameter] public string Slug { get; set; } = string.Empty;
    
    private string newsPath => $"/news/{Locale}/{(string.IsNullOrEmpty(Region) ? "" : $"{Region}/")}{Slug}";
    private List<ContentItem>? relatedNews;
    
    protected override async Task OnParametersSetAsync()
    {
        relatedNews = await NewsService.GetRelatedAsync(Slug, Locale, Region, 5);
    }
    
    private async Task HandleLocaleChange(string newLocale)
    {
        var path = string.IsNullOrEmpty(Region) 
            ? $"/news/{newLocale}/{Slug}"
            : $"/news/{newLocale}/{Region}/{Slug}";
        await Navigation.NavigateToAsync(path);
    }
    
    private string GetNewsLocaleName(string locale)
    {
        return $"{GetRegionFlag(locale)} {CultureInfo.GetCultureInfo(locale).NativeName}";
    }
    
    private string FormatNewsTranslationUrl(string locale, string contentId)
    {
        return string.IsNullOrEmpty(Region)
            ? $"/news/{locale}/{contentId}"
            : $"/news/{locale}/{Region}/{contentId}";
    }
    
    private string FormatNewsCategoryUrl(string category)
    {
        return $"/news/{Locale}/category/{category}";
    }
    
    private string FormatNewsUrl(ContentItem item)
    {
        return string.IsNullOrEmpty(Region)
            ? $"/news/{Locale}/{item.Slug}"
            : $"/news/{Locale}/{Region}/{item.Slug}";
    }
    
    private string GetRegionName(string region)
    {
        return region switch
        {
            "us" => "United States",
            "uk" => "United Kingdom", 
            "eu" => "Europe",
            "asia" => "Asia Pacific",
            _ => region.ToUpper()
        };
    }
    
    private string GetRegionFlag(string locale)
    {
        return locale switch
        {
            "en-US" => "🇺🇸",
            "en-GB" => "🇬🇧",
            "es-ES" => "🇪🇸",
            "fr-FR" => "🇫🇷",
            "de-DE" => "🇩🇪",
            "ja-JP" => "🇯🇵",
            _ => "🌍"
        };
    }
    
    private string GetDateFormat(string locale)
    {
        return CultureInfo.GetCultureInfo(locale).DateTimeFormat.ShortDatePattern;
    }
    
    private async Task ShareArticle()
    {
        await JSRuntime.InvokeVoidAsync("navigator.share", new
        {
            title = "Article Title",
            url = Navigation.Uri
        });
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-content-view {
    /* Main container */
}

.osirion-translations {
    /* Translation links container */
    display: flex;
    gap: 1rem;
    margin-bottom: 1rem;
    padding: 1rem;
    background: var(--bs-light);
    border-radius: 0.375rem;
    border-left: 4px solid var(--bs-primary);
}

.osirion-translations a {
    /* Translation link */
    color: var(--bs-primary);
    text-decoration: none;
    font-weight: 500;
    padding: 0.25rem 0.75rem;
    border-radius: 0.25rem;
    transition: background-color 0.2s ease;
}

.osirion-translations a:hover {
    background: var(--bs-primary);
    color: white;
}

.osirion-content-header {
    /* Localized content header */
    margin-bottom: 2rem;
}

.osirion-content-meta {
    /* Metadata with locale formatting */
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    color: var(--bs-text-muted);
    margin-bottom: 1.5rem;
}

.osirion-content-navigation {
    /* Previous/Next navigation */
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
    margin-top: 2rem;
    padding-top: 2rem;
    border-top: 1px solid var(--bs-border-color);
}

.osirion-content-previous,
.osirion-content-next {
    /* Navigation links */
    display: block;
    padding: 1rem;
    border: 1px solid var(--bs-border-color);
    border-radius: 0.5rem;
    text-decoration: none;
    color: inherit;
    transition: border-color 0.2s ease;
}

.osirion-content-next {
    text-align: right;
}

.osirion-content-navigation-label {
    /* Navigation labels */
    display: block;
    font-size: 0.875rem;
    color: var(--bs-text-muted);
    margin-bottom: 0.25rem;
}

.osirion-content-navigation-title {
    /* Navigation titles */
    display: block;
    font-weight: 600;
}
```

### RTL (Right-to-Left) Support

```css
/* RTL language support */
[dir="rtl"] .osirion-translations {
    flex-direction: row-reverse;
}

[dir="rtl"] .osirion-content-meta {
    flex-direction: row-reverse;
}

[dir="rtl"] .osirion-content-navigation {
    grid-template-columns: 1fr 1fr;
}

[dir="rtl"] .osirion-content-previous {
    text-align: right;
}

[dir="rtl"] .osirion-content-next {
    text-align: left;
}

/* Language-specific typography */
[lang="ar"] .osirion-content-view,
[lang="he"] .osirion-content-view {
    direction: rtl;
    text-align: right;
}

[lang="zh"] .osirion-content-view,
[lang="ja"] .osirion-content-view,
[lang="ko"] .osirion-content-view {
    line-height: 1.8;
    font-family: -apple-system, "Noto Sans CJK", sans-serif;
}
```

### Responsive Localization

```css
/* Mobile responsiveness for translations */
@media (max-width: 768px) {
    .osirion-translations {
        flex-direction: column;
        gap: 0.5rem;
    }
    
    .osirion-content-navigation {
        grid-template-columns: 1fr;
        gap: 0.5rem;
    }
    
    .osirion-content-next {
        text-align: left;
    }
    
    [dir="rtl"] .osirion-content-next {
        text-align: right;
    }
}
```

## SEO and Internationalization

### Hreflang Implementation

```razor
<LocalizedContentView Item="@article" CurrentLocale="@currentLocale" />

@if (article != null && availableTranslations?.Any() == true)
{
    @foreach (var translation in availableTranslations)
    {
        <link rel="alternate" hreflang="@translation.Key" href="@GetCanonicalUrl(translation.Value)" />
    }
    <link rel="alternate" hreflang="x-default" href="@GetCanonicalUrl(defaultTranslation)" />
}

@code {
    private string GetCanonicalUrl(string path)
    {
        return $"{Navigation.BaseUri.TrimEnd('/')}{path}";
    }
}
```

### Cultural Formatting

```razor
<LocalizedContentView Item="@article" 
                      CurrentLocale="@currentLocale"
                      LocaleNameFormatter="@FormatCulturalName" />

@code {
    private string FormatCulturalName(string locale)
    {
        var culture = CultureInfo.GetCultureInfo(locale);
        return $"{GetFlagEmoji(locale)} {culture.NativeName}";
    }
    
    private string GetFlagEmoji(string locale)
    {
        var region = locale.Split('-').LastOrDefault()?.ToUpper();
        return region switch
        {
            "US" => "🇺🇸",
            "GB" => "🇬🇧", 
            "FR" => "🇫🇷",
            "DE" => "🇩🇪",
            "ES" => "🇪🇸",
            "IT" => "🇮🇹",
            "JP" => "🇯🇵",
            "KR" => "🇰🇷",
            "CN" => "🇨🇳",
            _ => "🌍"
        };
    }
}
```

## Performance Optimization

### Translation Caching

```razor
@implements IDisposable

<LocalizedContentView Path="@contentPath" CurrentLocale="@currentLocale" />

@code {
    private readonly MemoryCache translationCache = new MemoryCache(new MemoryCacheOptions());
    
    private async Task<Dictionary<string, string>> GetCachedTranslations(string contentId)
    {
        var cacheKey = $"translations_{contentId}";
        
        if (!translationCache.TryGetValue(cacheKey, out Dictionary<string, string>? translations))
        {
            translations = await ContentService.GetTranslationsAsync(contentId);
            translationCache.Set(cacheKey, translations, TimeSpan.FromMinutes(15));
        }
        
        return translations ?? new Dictionary<string, string>();
    }
    
    public void Dispose()
    {
        translationCache.Dispose();
    }
}
```

### Lazy Translation Loading

```razor
<LocalizedContentView Item="@content" 
                      CurrentLocale="@currentLocale"
                      EnableLocalization="@enableTranslations" />

<button @onclick="LoadTranslations" class="btn btn-outline-secondary btn-sm">
    @(enableTranslations ? "Hide" : "Show") Translations
</button>

@code {
    private bool enableTranslations = false;
    
    private void LoadTranslations()
    {
        enableTranslations = !enableTranslations;
    }
}
```

## Common Use Cases

- **Multi-Language Blogs**: International blog platforms with multiple language versions
- **Global Documentation**: Technical documentation available in multiple languages
- **E-commerce Sites**: Product pages with localized content and pricing
- **News Portals**: Regional news sites with local language support
- **Educational Platforms**: Course content available in multiple languages
- **Corporate Websites**: Company information localized for different markets

## Best Practices

1. **URL Structure**: Use consistent URL patterns for different languages
2. **SEO**: Implement proper hreflang attributes and canonical URLs
3. **Cultural Formatting**: Respect cultural differences in date, time, and number formatting
4. **Fallbacks**: Always provide fallback content when translations are missing
5. **Performance**: Cache translations and use lazy loading when appropriate
6. **Accessibility**: Ensure proper language attributes and RTL support
7. **User Experience**: Provide clear language switching interfaces
8. **Content Management**: Maintain translation consistency and quality

The LocalizedContentView component provides a comprehensive solution for building truly international content experiences with proper localization support and cultural awareness.
