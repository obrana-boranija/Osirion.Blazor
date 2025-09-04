---
id: 'core-states-components-overview'
order: 1
layout: docs
title: Core States Components Overview
permalink: /docs/components/core/states
description: Complete overview of Osirion.Blazor Core States components including content not found states for error handling and user feedback.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- States
tags:
- blazor
- core-components
- states
- error-handling
- user-feedback
- 404-pages
is_featured: true
published: true
slug: components/core/states
lang: en
custom_fields: {}
seo_properties:
  title: 'Core States Components - Osirion.Blazor Error Handling'
  description: 'Explore Osirion.Blazor Core States components for error handling and user feedback states.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/states'
  lang: en
  robots: index, follow
  og_title: 'Core States Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core States components with error handling and feedback features.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core States Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core States components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core States Components Overview

The Osirion.Blazor Core States module provides essential components for handling various application states and providing meaningful user feedback. These components ensure users receive clear, helpful information when content is unavailable or errors occur.

## Available Components

### Content Not Found
A comprehensive component for handling 404 errors and missing content scenarios. Provides user-friendly error messages, navigation suggestions, and recovery options when requested content cannot be found.

## Key Features

- **User-Friendly Messaging**: Clear, helpful error communication
- **Navigation Recovery**: Suggested actions and alternative paths
- **SEO Optimization**: Proper HTTP status handling for search engines
- **Customizable Design**: Flexible styling and branding options
- **Analytics Integration**: Error tracking and monitoring capabilities

## Getting Started

To use state components in your project:

```razor
@using Osirion.Blazor.Core

<ContentNotFound Title="Page Not Found"
                 Message="The page you're looking for doesn't exist."
                 SuggestedActions="@recoveryOptions"
                 ShowSearchBox="true"
                 ReturnUrl="@homeUrl" />
```

## Content Not Found Features

The Content Not Found component includes:

- **Custom Messages**: Configurable error messages and descriptions
- **Search Integration**: Built-in search functionality for content discovery
- **Navigation Suggestions**: Helpful links to popular or related content
- **Visual Elements**: Icons, illustrations, or custom graphics
- **Return Actions**: Easy navigation back to safe pages

## Error Handling Patterns

Common error state patterns:

- **404 Not Found**: Missing pages and resources
- **403 Forbidden**: Access denied scenarios
- **500 Server Error**: Internal server error handling
- **Network Issues**: Connection and timeout problems
- **Empty States**: No content available scenarios

## Recovery Options

Built-in recovery mechanisms:

- **Search Functionality**: Help users find alternative content
- **Popular Content**: Suggest trending or popular pages
- **Category Navigation**: Guide users to relevant sections
- **Contact Support**: Direct access to help and support
- **Previous Page**: Safe navigation back to working content

## SEO Considerations

The state components handle SEO properly:

- **HTTP Status Codes**: Correct status code responses
- **Meta Tags**: Appropriate meta information for error pages
- **Canonical URLs**: Prevent indexing of error states
- **Structured Data**: Error page markup for search engines

## Customization Options

Extensive customization capabilities:

- **Visual Design**: Custom styling, colors, and layouts
- **Content**: Personalized messages and copy
- **Actions**: Custom recovery actions and buttons
- **Integrations**: Analytics, search, and support system connections
- **Animations**: Optional visual effects and transitions

## User Experience

The state components prioritize user experience:

- **Clear Communication**: Understandable error explanations
- **Helpful Actions**: Constructive next steps and options
- **Visual Appeal**: Professional, branded error page design
- **Accessibility**: Screen reader compatibility and keyboard navigation
- **Performance**: Fast loading even during error conditions

These state components ensure that even when things go wrong, users receive helpful, professional feedback that maintains trust and provides clear paths forward.
