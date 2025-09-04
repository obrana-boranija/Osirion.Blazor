---
id: 'core-rendering-components-overview'
order: 1
layout: docs
title: Core Rendering Components Overview
permalink: /docs/components/core/rendering
description: Complete overview of Osirion.Blazor Core Rendering components including HTML renderer for dynamic content display and markup handling.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Rendering
tags:
- blazor
- core-components
- rendering
- html-renderer
- dynamic-content
is_featured: true
published: true
slug: rendering
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Rendering Components - Osirion.Blazor Dynamic Content'
  description: 'Explore Osirion.Blazor Core Rendering components for HTML rendering and dynamic content display.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/rendering'
  lang: en
  robots: index, follow
  og_title: 'Core Rendering Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Rendering components with HTML rendering capabilities.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Rendering Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Rendering components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Rendering Components Overview

The Osirion.Blazor Core Rendering module provides powerful components for dynamic content rendering and HTML processing. These components enable safe, flexible display of user-generated content and dynamic markup within Blazor applications.

## Available Components

### HTML Renderer
A secure HTML rendering component that safely displays HTML content while preventing XSS attacks and maintaining application security. Perfect for rendering user-generated content, markdown output, and dynamic HTML from external sources.

## Key Features

- **Security First**: Built-in XSS protection and content sanitization
- **Flexible Rendering**: Support for various HTML content types
- **Performance Optimized**: Efficient rendering with minimal overhead
- **Customizable Sanitization**: Configurable HTML filtering rules
- **CSS Integration**: Seamless styling and theme integration

## Getting Started

To use rendering components in your project:

```razor
@using Osirion.Blazor.Core

<HtmlRenderer Content="@htmlContent" 
              SanitizationLevel="@SanitizationLevel.Strict"
              AllowedTags="@allowedTags"
              AllowedAttributes="@allowedAttributes" />
```

## Security Features

The HTML Renderer provides comprehensive security:

- **XSS Prevention**: Automatic script injection protection
- **Content Sanitization**: Whitelist-based HTML filtering
- **Attribute Validation**: Safe attribute handling and validation
- **URL Filtering**: Protection against malicious links
- **Custom Rules**: Configurable security policies

## Sanitization Levels

Multiple security levels available:

- **Strict**: Maximum security with minimal HTML allowed
- **Standard**: Balanced security for typical content
- **Relaxed**: More permissive for trusted content sources
- **Custom**: User-defined sanitization rules

## Content Sources

Ideal for rendering content from:

- **Content Management Systems**: CMS-generated HTML content
- **Markdown Processors**: Converted markdown to HTML
- **User Comments**: Safely display user-generated content
- **External APIs**: Third-party content integration
- **Rich Text Editors**: WYSIWYG editor output

## Configuration Options

Extensive customization capabilities:

- **Tag Whitelist**: Define allowed HTML elements
- **Attribute Control**: Specify permitted attributes per tag
- **CSS Handling**: Style and class attribute management
- **Link Policies**: External link handling and security
- **Image Processing**: Safe image source validation

## Performance Considerations

The HTML Renderer is optimized for:

- **Minimal Processing**: Efficient content parsing and rendering
- **Caching Support**: Rendered content caching capabilities
- **Memory Efficiency**: Low memory footprint for large content
- **Server-Side Rendering**: Full SSR compatibility

This rendering component ensures that your application can safely display dynamic HTML content while maintaining security and performance standards.
