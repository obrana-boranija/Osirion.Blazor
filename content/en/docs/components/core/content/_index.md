---
id: 'core-content-components-overview'
order: 1
layout: docs
title: Core Content Components Overview
permalink: /docs/components/core/content
description: Complete overview of Osirion.Blazor Core Content components including article metadata handling for content-rich applications.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Content
tags:
- blazor
- core-components
- content
- metadata
- articles
is_featured: true
published: true
slug: components/core/content
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Content Components - Osirion.Blazor Content Handling'
  description: 'Explore Osirion.Blazor Core Content components for article metadata and content management features.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/content'
  lang: en
  robots: index, follow
  og_title: 'Core Content Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Content components with metadata and article handling.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Content Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Content components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Content Components Overview

The Osirion.Blazor Core Content module provides essential components for handling and displaying content metadata and article information. These components are designed to work seamlessly with content management systems and article-based applications.

## Available Components

### Article Metadata
A comprehensive component for displaying and managing article metadata including publication dates, authors, tags, categories, reading time estimates, and other content-related information.

## Key Features

- **Rich Metadata Support**: Display comprehensive article information
- **SEO Optimization**: Built-in structured data and metadata handling
- **Customizable Display**: Flexible formatting and presentation options
- **Accessibility**: Screen reader friendly metadata presentation
- **Integration Ready**: Works seamlessly with CMS and content systems

## Getting Started

To use content components in your project:

```razor
@using Osirion.Blazor.Core

<ArticleMetadata Article="@currentArticle" 
                 ShowAuthor="true" 
                 ShowPublishDate="true" 
                 ShowReadingTime="true" 
                 ShowTags="true" />
```

## Metadata Features

The Article Metadata component supports:

- **Author Information**: Display author names, profiles, and avatars
- **Publication Dates**: Show creation, modification, and publication timestamps
- **Content Classification**: Display categories, tags, and content types
- **Reading Metrics**: Estimated reading time and content statistics
- **Social Sharing**: Integration with social media metadata
- **Custom Fields**: Support for additional metadata properties

## Integration

These components integrate seamlessly with:

- Content Management Systems (CMS)
- Blog platforms and article repositories
- Documentation systems
- News and media websites
- Academic and research platforms

The content components are designed to be highly flexible while maintaining consistency in metadata presentation across your application.
