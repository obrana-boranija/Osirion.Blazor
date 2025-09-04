---
id: 'cms-web-core-components-overview'
order: 1
layout: docs
title: CMS Web Core Components Overview
permalink: /docs/components/cms-web/core
description: Complete overview of Osirion.Blazor CMS Web Core components including content list, content page, content renderer, content view, and localized content view.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- CMS Web Components
- Core Components
tags:
- blazor
- cms
- content-management
- web-components
- content-rendering
is_featured: true
published: true
slug: core
lang: en
custom_fields: {}
seo_properties:
  title: 'CMS Web Core Components - Osirion.Blazor Content Management'
  description: 'Explore Osirion.Blazor CMS Web Core components for content management, rendering, and display.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/cms-web/core'
  lang: en
  robots: index, follow
  og_title: 'CMS Web Core Components - Osirion.Blazor'
  og_description: 'Complete documentation for CMS Web Core components with content management features.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'CMS Web Core Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor CMS Web Core components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# CMS Web Core Components Overview

The Osirion.Blazor CMS Web Core module provides fundamental content management components for building content-driven web applications. These components form the foundation of content display, management, and rendering functionality.

## Available Components

### Content List
A versatile component for displaying collections of content items with filtering, sorting, pagination, and various layout options. Perfect for blog listings, article archives, and content galleries.

### Content Page
A comprehensive page-level component that handles individual content item display with full metadata, navigation, and related content suggestions.

### Content Renderer
A flexible content rendering engine that processes and displays various content types including HTML, Markdown, rich text, and custom content formats with security and performance optimization.

### Content View
A configurable component for displaying content items in various formats and layouts, supporting different presentation styles and interaction patterns.

### Localized Content View
An internationalization-ready component that handles multi-language content display with automatic language detection, fallback mechanisms, and culture-specific formatting.

## Key Features

- **Flexible Content Display**: Multiple layout and presentation options
- **Multi-language Support**: Full internationalization capabilities
- **SEO Optimization**: Built-in metadata and search engine optimization
- **Performance**: Optimized rendering and caching mechanisms
- **Security**: Content sanitization and XSS protection

## Getting Started

To use CMS Web Core components in your project:

```razor
@using Osirion.Blazor.Cms.Web

<ContentList ContentType="@ContentType.Article" 
             PageSize="10" 
             ShowPagination="true"
             Layout="@ListLayout.Grid" />

<ContentPage ContentId="@pageId" 
             ShowMetadata="true" 
             EnableComments="true" />

<ContentRenderer Content="@contentHtml" 
                 SanitizationLevel="@SanitizationLevel.Standard" />

<ContentView Item="@contentItem" 
             Template="@ViewTemplate.Card" />

<LocalizedContentView ContentId="@contentId" 
                     Language="@currentLanguage" 
                     FallbackLanguage="en" />
```

## Content Management Features

### Content Organization
- **Categories**: Hierarchical content categorization
- **Tags**: Flexible content tagging and labeling
- **Collections**: Grouped content management
- **Workflows**: Content approval and publishing workflows

### Display Options
- **Templates**: Multiple content presentation templates
- **Layouts**: Flexible arrangement and styling options
- **Responsive**: Mobile-optimized content display
- **Accessibility**: WCAG compliant content presentation

### Performance Features
- **Lazy Loading**: Efficient content loading strategies
- **Caching**: Intelligent content caching mechanisms
- **Optimization**: Image and media optimization
- **CDN Support**: Content delivery network integration

## Localization Support

The localized components provide:

- **Multi-language Content**: Support for multiple content languages
- **Automatic Detection**: Browser language detection
- **Fallback Mechanisms**: Graceful handling of missing translations
- **Cultural Formatting**: Date, number, and currency localization
- **RTL Support**: Right-to-left language support

## Integration

These components integrate seamlessly with:

- Content Management Systems (CMS)
- Headless CMS platforms
- Database content storage
- External content APIs
- Static site generators

The CMS Web Core components provide a robust foundation for building content-rich applications with modern web standards and best practices.
