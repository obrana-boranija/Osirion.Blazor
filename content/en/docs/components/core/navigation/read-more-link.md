---
id: 'osirion-read-more-link'
order: 2
layout: docs
title: OsirionReadMoreLink Component
permalink: /docs/components/core/navigation/read-more-link
description: Learn how to use the OsirionReadMoreLink component to create consistent read more and call-to-action links with icons and styling options.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Navigation
tags:
- blazor
- links
- navigation
- call-to-action
- icons
- user-interface
is_featured: true
published: true
slug: read-more-link
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionReadMoreLink Component - Action Links | Osirion.Blazor'
  description: 'Create consistent read more and call-to-action links with the OsirionReadMoreLink component. Features multiple variants, icons, and accessibility support.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/navigation/read-more-link'
  lang: en
  robots: index, follow
  og_title: 'OsirionReadMoreLink Component - Osirion.Blazor'
  og_description: 'Create consistent read more and call-to-action links with multiple variants and accessibility support.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionReadMoreLink Component - Osirion.Blazor'
  twitter_description: 'Create consistent read more and call-to-action links with icons and styling.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionReadMoreLink Component

The OsirionReadMoreLink component provides a consistent way to create "read more" links and call-to-action buttons throughout your application. It supports multiple visual variants, customizable icons, and various sizing options while maintaining accessibility standards.

## Component Overview

OsirionReadMoreLink is designed to provide consistent styling and behavior for action links across your application. Whether you need simple "read more" links in cards, external link indicators, or download buttons, this component handles the visual styling and accessibility requirements.

### Key Features

**Multiple Variants**: Choose from default, arrow, external, download, plain, and button styles
**Flexible Icons**: Built-in icons for common use cases or custom icon support
**Size Options**: Small, normal, and large sizing options
**Position Control**: Icons can be positioned left or right of text
**Accessibility Compliant**: Proper ARIA labels and semantic markup
**Animation Support**: Optional hover animations and transitions
**Framework Agnostic**: Works with Bootstrap, Tailwind, and other CSS frameworks
**Stretched Links**: Support for Bootstrap's stretched link functionality
**Block Display**: Option to display as block-level element
**Custom Content**: Support for additional child content

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Href` | `string` (Required) | `"#"` | The link URL destination. |
| `Text` | `string` | `"Read more"` | The text displayed in the link. |
| `AriaLabel` | `string?` | `null` | Custom aria-label for accessibility. Falls back to Text if not provided. |
| `ShowText` | `bool` | `true` | Whether to display the text. |
| `ShowIcon` | `bool` | `true` | Whether to display the icon. |
| `IconPosition` | `IconPosition` | `Right` | Position of the icon relative to text (Left or Right). |
| `IconContent` | `RenderFragment?` | `null` | Custom icon content. Overrides default icon when provided. |
| `Variant` | `ReadMoreVariant` | `Default` | Visual variant (Default, Arrow, External, Download, Plain, Button). |
| `Size` | `LinkSize` | `Normal` | Link size (Small, Normal, Large). |
| `Stretched` | `bool` | `false` | Whether the link should be stretched (useful in cards). |
| `Block` | `bool` | `false` | Whether to display as a block element. |
| `Target` | `string?` | `null` | Link target (_blank, _self, etc.). |
| `Animated` | `bool` | `true` | Whether the link should have hover animation. |
| `ChildContent` | `RenderFragment?` | `null` | Additional content to render inside the link. |

## Basic Usage

### Simple Read More Link

```razor
@using Osirion.Blazor.Components

<OsirionReadMoreLink 
    Href="/articles/getting-started"
    Text="Read more" />
```

### External Link

```razor
<OsirionReadMoreLink 
    Href="https://example.com"
    Text="Visit website"
    Variant="ReadMoreVariant.External"
    Target="_blank" />
```

### Download Link

```razor
<OsirionReadMoreLink 
    Href="/downloads/user-guide.pdf"
    Text="Download guide"
    Variant="ReadMoreVariant.Download" />
```

### Arrow Link

```razor
<OsirionReadMoreLink 
    Href="/next-page"
    Text="Continue"
    Variant="ReadMoreVariant.Arrow"
    Size="LinkSize.Large" />
```

## Advanced Usage

### Custom Icon Content

```razor
<OsirionReadMoreLink 
    Href="/contact"
    Text="Get in touch"
    IconPosition="IconPosition.Left">
    
    <IconContent>
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
            <path d="M8 1a7 7 0 1 0 0 14A7 7 0 0 0 8 1M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8"/>
            <path d="M5.255 5.786a.237.237 0 0 0 .241.247h.825c.138 0 .248-.113.266-.25.09-.656.54-1.134 1.342-1.134.686 0 1.314.343 1.314 1.168 0 .635-.374.927-.965 1.371-.673.489-1.206 1.06-1.168 1.987l.003.217a.25.25 0 0 0 .25.246h.811a.25.25 0 0 0 .25-.25v-.105c0-.718.273-.927 1.01-1.486.609-.463 1.244-.977 1.244-2.056 0-1.511-1.276-2.241-2.673-2.241-1.267 0-2.655.59-2.75 2.286m1.557 5.763c0 .533.425.927 1.01.927.609 0 1.028-.394 1.028-.927 0-.552-.42-.94-1.029-.94-.584 0-1.009.388-1.009.94"/>
        </svg>
    </IconContent>
    
</OsirionReadMoreLink>
```

### Card with Stretched Link

```razor
<div class="card position-relative">
    <div class="card-body">
        <h5 class="card-title">Article Title</h5>
        <p class="card-text">This is a brief excerpt from the article...</p>
        
        <OsirionReadMoreLink 
            Href="/articles/detailed-article"
            Text="Read full article"
            Stretched="true"
            Variant="ReadMoreVariant.Arrow" />
    </div>
</div>
```

### Link Collection with Different Variants

```razor
<div class="link-collection">
    <div class="mb-3">
        <h6>Documentation Links</h6>
        <ul class="list-unstyled">
            <li class="mb-2">
                <OsirionReadMoreLink 
                    Href="/docs/getting-started"
                    Text="Getting Started Guide"
                    Variant="ReadMoreVariant.Default"
                    Size="LinkSize.Small" />
            </li>
            <li class="mb-2">
                <OsirionReadMoreLink 
                    Href="/docs/api-reference"
                    Text="API Reference"
                    Variant="ReadMoreVariant.Arrow"
                    Size="LinkSize.Small" />
            </li>
            <li class="mb-2">
                <OsirionReadMoreLink 
                    Href="/examples"
                    Text="View Examples"
                    Variant="ReadMoreVariant.External"
                    Target="_blank"
                    Size="LinkSize.Small" />
            </li>
        </ul>
    </div>
    
    <div class="mb-3">
        <h6>Downloads</h6>
        <ul class="list-unstyled">
            <li class="mb-2">
                <OsirionReadMoreLink 
                    Href="/downloads/quick-start.pdf"
                    Text="Quick Start Guide"
                    Variant="ReadMoreVariant.Download"
                    Size="LinkSize.Small" />
            </li>
            <li class="mb-2">
                <OsirionReadMoreLink 
                    Href="/downloads/templates.zip"
                    Text="Component Templates"
                    Variant="ReadMoreVariant.Download"
                    Size="LinkSize.Small" />
            </li>
        </ul>
    </div>
</div>

<style>
.link-collection h6 {
    color: #495057;
    font-weight: 600;
    margin-bottom: 0.75rem;
}

.link-collection .osirion-read-more {
    transition: transform 0.2s ease;
}

.link-collection .osirion-read-more:hover {
    transform: translateX(4px);
}
</style>
```

### Interactive Link Grid

```razor
<div class="row g-3">
    @foreach (var linkData in GetLinkData())
    {
        <div class="col-md-6 col-lg-4">
            <div class="link-card p-3 border rounded">
                <div class="link-card-icon mb-2">
                    @linkData.Icon
                </div>
                <h6 class="link-card-title">@linkData.Title</h6>
                <p class="link-card-description text-muted small">@linkData.Description</p>
                
                <OsirionReadMoreLink 
                    Href="@linkData.Url"
                    Text="@linkData.LinkText"
                    Variant="@linkData.Variant"
                    Target="@linkData.Target"
                    Block="true"
                    Class="mt-2" />
            </div>
        </div>
    }
</div>

@code {
    private List<LinkData> GetLinkData()
    {
        return new List<LinkData>
        {
            new("Documentation", 
                "Complete guides and API reference", 
                "/docs", 
                "Browse docs", 
                ReadMoreVariant.Arrow, 
                null,
                "📚"),
            new("Examples", 
                "Live examples and code samples", 
                "/examples", 
                "View examples", 
                ReadMoreVariant.External, 
                "_blank",
                "🎯"),
            new("Templates", 
                "Ready-to-use component templates", 
                "/downloads/templates.zip", 
                "Download", 
                ReadMoreVariant.Download, 
                null,
                "📦"),
            new("Support", 
                "Get help from our community", 
                "https://github.com/osirion/support", 
                "Get support", 
                ReadMoreVariant.External, 
                "_blank",
                "🤝"),
            new("Tutorial", 
                "Step-by-step learning path", 
                "/tutorial", 
                "Start tutorial", 
                ReadMoreVariant.Button, 
                null,
                "🎓"),
            new("Blog", 
                "Latest news and updates", 
                "/blog", 
                "Read posts", 
                ReadMoreVariant.Default, 
                null,
                "📰")
        };
    }
    
    public record LinkData(
        string Title, 
        string Description, 
        string Url, 
        string LinkText, 
        ReadMoreVariant Variant, 
        string? Target,
        string Icon);
}

<style>
.link-card {
    transition: all 0.3s ease;
    height: 100%;
    display: flex;
    flex-direction: column;
}

.link-card:hover {
    border-color: #007bff;
    box-shadow: 0 4px 8px rgba(0, 123, 255, 0.1);
    transform: translateY(-2px);
}

.link-card-icon {
    font-size: 2rem;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 3rem;
    height: 3rem;
    background: #f8f9fa;
    border-radius: 0.5rem;
}

.link-card-title {
    color: #495057;
    font-weight: 600;
    margin-bottom: 0.5rem;
}

.link-card-description {
    flex: 1;
    margin-bottom: 1rem;
}

.link-card .osirion-read-more-block {
    margin-top: auto;
    text-align: center;
    padding: 0.5rem;
    border-radius: 0.25rem;
    background: #f8f9fa;
    transition: background-color 0.2s;
}

.link-card .osirion-read-more-block:hover {
    background: #e9ecef;
    text-decoration: none;
}
</style>
```

### Animated Link Sequence

```razor
<div class="link-sequence">
    @for (int i = 0; i < sequenceLinks.Count; i++)
    {
        var link = sequenceLinks[i];
        var isActive = currentStep >= i;
        var isCompleted = currentStep > i;
        
        <div class="sequence-item @(isActive ? "active" : "") @(isCompleted ? "completed" : "")">
            <div class="sequence-number">
                @if (isCompleted)
                {
                    <span class="sequence-check">✓</span>
                }
                else
                {
                    <span>@(i + 1)</span>
                }
            </div>
            
            <div class="sequence-content">
                <h6 class="sequence-title">@link.Title</h6>
                <p class="sequence-description">@link.Description</p>
                
                @if (isActive && !isCompleted)
                {
                    <OsirionReadMoreLink 
                        Href="@link.Url"
                        Text="@link.ActionText"
                        Variant="ReadMoreVariant.Button"
                        Size="LinkSize.Small"
                        @onclick="() => CompleteStep(i)" />
                }
                else if (isCompleted)
                {
                    <OsirionReadMoreLink 
                        Href="@link.Url"
                        Text="Review"
                        Variant="ReadMoreVariant.Plain"
                        Size="LinkSize.Small" />
                }
            </div>
        </div>
        
        @if (i < sequenceLinks.Count - 1)
        {
            <div class="sequence-connector @(currentStep > i ? "completed" : "")"></div>
        }
    }
</div>

@code {
    private int currentStep = 0;
    
    private List<SequenceLink> sequenceLinks = new()
    {
        new("Setup Environment", "Install the required packages and dependencies", "/setup", "Get started"),
        new("Configure Project", "Set up your project configuration", "/configure", "Configure"),
        new("Create Components", "Build your first components", "/components", "Create"),
        new("Deploy Application", "Deploy to production", "/deploy", "Deploy")
    };
    
    private void CompleteStep(int stepIndex)
    {
        if (stepIndex == currentStep)
        {
            currentStep++;
        }
    }
    
    public record SequenceLink(string Title, string Description, string Url, string ActionText);
}

<style>
.link-sequence {
    max-width: 500px;
    margin: 2rem auto;
}

.sequence-item {
    display: flex;
    align-items: flex-start;
    gap: 1rem;
    padding: 1rem;
    border-radius: 0.5rem;
    transition: all 0.3s ease;
}

.sequence-item.active {
    background: #e3f2fd;
    border: 2px solid #2196f3;
}

.sequence-item.completed {
    background: #e8f5e8;
    opacity: 0.8;
}

.sequence-number {
    width: 2rem;
    height: 2rem;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: bold;
    font-size: 0.875rem;
    background: #e9ecef;
    color: #6c757d;
    flex-shrink: 0;
}

.sequence-item.active .sequence-number {
    background: #2196f3;
    color: white;
}

.sequence-item.completed .sequence-number {
    background: #4caf50;
    color: white;
}

.sequence-check {
    font-size: 0.75rem;
}

.sequence-content {
    flex: 1;
}

.sequence-title {
    margin: 0 0 0.25rem 0;
    color: #495057;
    font-weight: 600;
}

.sequence-description {
    margin: 0 0 0.75rem 0;
    font-size: 0.875rem;
    color: #6c757d;
}

.sequence-connector {
    width: 2px;
    height: 1.5rem;
    background: #e9ecef;
    margin-left: 2rem;
    transition: background-color 0.3s ease;
}

.sequence-connector.completed {
    background: #4caf50;
}
</style>
```

## Styling Examples

### Bootstrap Integration

```razor
<OsirionReadMoreLink 
    Href="/example"
    Text="Bootstrap Link"
    Class="btn btn-outline-primary btn-sm"
    Variant="ReadMoreVariant.Button" />

<style>
/* Bootstrap-compatible styling */
.osirion-read-more {
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    text-decoration: none;
    transition: all 0.2s ease;
}

.osirion-read-more:hover {
    text-decoration: none;
}

.osirion-read-more-small {
    font-size: 0.875rem;
}

.osirion-read-more-large {
    font-size: 1.125rem;
}

.osirion-read-more-default {
    color: #007bff;
}

.osirion-read-more-default:hover {
    color: #0056b3;
}

.osirion-read-more-arrow {
    color: #28a745;
}

.osirion-read-more-external {
    color: #17a2b8;
}

.osirion-read-more-download {
    color: #ffc107;
    font-weight: 500;
}

.osirion-read-more-button {
    background: #007bff;
    color: white;
    padding: 0.375rem 0.75rem;
    border-radius: 0.25rem;
    border: 1px solid #007bff;
}

.osirion-read-more-button:hover {
    background: #0056b3;
    border-color: #0056b3;
    color: white;
}

.osirion-read-more-block {
    display: block;
    text-align: center;
    width: 100%;
}

.osirion-read-more-animated .osirion-read-more-icon {
    transition: transform 0.2s ease;
}

.osirion-read-more-animated:hover .osirion-read-more-icon-right {
    transform: translateX(2px);
}

.osirion-read-more-animated:hover .osirion-read-more-icon-left {
    transform: translateX(-2px);
}
</style>
```

### Tailwind CSS Integration

```razor
<OsirionReadMoreLink 
    Href="/example"
    Text="Tailwind Link"
    Class="inline-flex items-center px-3 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
    Variant="ReadMoreVariant.Button" />

<style>
/* Tailwind-compatible classes */
.osirion-read-more {
    @apply inline-flex items-center gap-1 no-underline transition-all duration-200;
}

.osirion-read-more-small {
    @apply text-sm;
}

.osirion-read-more-large {
    @apply text-lg;
}

.osirion-read-more-default {
    @apply text-blue-600 hover:text-blue-800;
}

.osirion-read-more-arrow {
    @apply text-green-600 hover:text-green-800;
}

.osirion-read-more-external {
    @apply text-cyan-600 hover:text-cyan-800;
}

.osirion-read-more-download {
    @apply text-yellow-600 hover:text-yellow-800 font-medium;
}

.osirion-read-more-button {
    @apply bg-blue-600 text-white px-3 py-2 rounded border border-blue-600 hover:bg-blue-700 hover:border-blue-700;
}

.osirion-read-more-block {
    @apply block text-center w-full;
}

.osirion-read-more-animated .osirion-read-more-icon {
    @apply transition-transform duration-200;
}

.osirion-read-more-animated:hover .osirion-read-more-icon-right {
    @apply transform translate-x-0.5;
}

.osirion-read-more-animated:hover .osirion-read-more-icon-left {
    @apply transform -translate-x-0.5;
}
</style>
```

## Best Practices

### Accessibility Guidelines

1. **Descriptive Text**: Use clear, descriptive link text that indicates the action
2. **ARIA Labels**: Provide meaningful aria-labels for screen readers
3. **Focus States**: Ensure proper focus indication for keyboard navigation
4. **Target Attributes**: Use appropriate target attributes for external links
5. **Context**: Provide sufficient context for the link's purpose

### User Experience

1. **Consistency**: Use consistent styling and variants across your application
2. **Visual Hierarchy**: Use appropriate sizes and variants to show importance
3. **Hover States**: Provide clear visual feedback on hover and focus
4. **Loading States**: Consider loading indicators for actions that take time
5. **Mobile Design**: Ensure links are properly sized for touch interactions

### Performance Considerations

1. **Icon Optimization**: Use SVG icons for crisp rendering at any size
2. **Animation Performance**: Use CSS transforms for smooth animations
3. **Bundle Size**: Consider the impact of custom icons on bundle size
4. **Accessibility**: Ensure animations respect user's motion preferences
5. **Caching**: Cache commonly used icons and styles

The OsirionReadMoreLink component provides a flexible and accessible solution for creating consistent action links throughout your application.
