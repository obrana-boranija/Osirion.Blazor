---
id: 'core-navigation-components-overview'
order: 1
layout: docs
title: Core Navigation Components Overview
permalink: /docs/components/core/navigation
description: Complete overview of Osirion.Blazor Core Navigation components including breadcrumbs and read more links for enhanced user navigation.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Navigation
tags:
- blazor
- core-components
- navigation
- breadcrumbs
- user-experience
is_featured: true
published: true
slug: navigation
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Navigation Components - Osirion.Blazor Navigation Tools'
  description: 'Explore Osirion.Blazor Core Navigation components for breadcrumbs and enhanced navigation experiences.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/navigation'
  lang: en
  robots: index, follow
  og_title: 'Core Navigation Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Navigation components with breadcrumbs and navigation tools.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Navigation Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Navigation components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Navigation Components Overview

The Osirion.Blazor Core Navigation module provides essential navigation components that enhance user experience and site wayfinding. These components help users understand their location within your application and navigate efficiently.

## Available Components

### Breadcrumbs
A hierarchical navigation component that shows the user's current location within the site structure, providing clear paths back to parent pages and improving overall navigation experience.

### Read More Link
An interactive link component designed for content truncation and expansion, allowing users to reveal additional content without navigating away from the current page.

## Key Features

- **Hierarchical Navigation**: Clear parent-child relationship display
- **SEO Benefits**: Structured data support for search engines
- **Responsive Design**: Mobile-friendly navigation patterns
- **Accessibility**: Screen reader support and keyboard navigation
- **Customizable Styling**: Flexible appearance and theming options

## Getting Started

To use navigation components in your project:

```razor
@using Osirion.Blazor.Core

<Breadcrumbs CurrentPage="@currentPage" 
             ShowHome="true" 
             Separator="/" 
             MaxItems="5" />

<ReadMoreLink Text="@longContent" 
              MaxLength="150" 
              ExpandText="Read more" 
              CollapseText="Show less" />
```

## Breadcrumbs Features

The Breadcrumbs component offers:

- **Automatic Generation**: Build breadcrumbs from route data
- **Custom Separators**: Choose from various separator styles
- **Truncation Options**: Handle deep navigation hierarchies
- **Rich Snippets**: Built-in structured data for SEO
- **Click Handling**: Programmatic navigation support

## Read More Link Features

The Read More Link component includes:

- **Content Truncation**: Intelligent text cutting at word boundaries
- **Smooth Animations**: Expand/collapse transitions
- **Custom Triggers**: Configurable expand/collapse text
- **Character Limits**: Flexible content length management
- **Accessibility**: Screen reader friendly content revelation

## Use Cases

These navigation components are ideal for:

- **E-commerce Sites**: Product category navigation
- **Documentation**: Section and subsection navigation
- **Blogs**: Article category and tag navigation
- **Corporate Sites**: Department and page hierarchies
- **Content Portals**: Multi-level content organization

The navigation components integrate seamlessly with Blazor routing and provide consistent navigation patterns across your application.
