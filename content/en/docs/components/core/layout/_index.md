---
id: 'core-layout-components-overview'
order: 1
layout: docs
title: Core Layout Components Overview
permalink: /docs/components/core/layout
description: Complete overview of Osirion.Blazor Core Layout components including background patterns, footers, page layouts, loading states, and sticky sidebars.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Layout
tags:
- blazor
- core-components
- layout
- page-structure
- ui-components
is_featured: true
published: true
slug: components/core/layout
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Layout Components - Osirion.Blazor Page Structure'
  description: 'Explore Osirion.Blazor Core Layout components for page structure, backgrounds, footers, and loading states.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/layout'
  lang: en
  robots: index, follow
  og_title: 'Core Layout Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Layout components with page structure and UI patterns.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Layout Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Layout components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Layout Components Overview

The Osirion.Blazor Core Layout module provides essential structural components for building modern web application layouts. These components offer the foundation for creating consistent, responsive, and visually appealing page structures.

## Available Components

### Background Pattern
Decorative background component that adds visual interest with customizable patterns, gradients, and textures to enhance page aesthetics.

### Footer
Comprehensive footer component with support for links, contact information, social media icons, and multi-column layouts for site-wide navigation and information.

### Page Layout
Main layout wrapper component that provides consistent page structure, responsive grid systems, and content organization patterns.

### Page Loading
Loading state component with customizable animations and indicators to provide user feedback during page transitions and data loading.

### Sticky Sidebar
Advanced sidebar component with sticky positioning, collapsible sections, and responsive behavior for navigation and secondary content.

## Key Features

- **Responsive Design**: All layout components adapt to different screen sizes
- **Flexible Grid System**: CSS Grid and Flexbox-based layouts
- **Performance Optimized**: Efficient rendering and minimal layout shifts
- **Accessibility**: WCAG compliant with proper semantic structure
- **Customizable Theming**: Extensive styling and branding options

## Getting Started

To use layout components in your project:

```razor
@using Osirion.Blazor.Core

<PageLayout>
    <Header>
        <!-- Navigation content -->
    </Header>
    
    <MainContent>
        <BackgroundPattern Pattern="@PatternType.Dots" />
        
        <StickySidebar Position="@SidebarPosition.Left">
            <!-- Sidebar content -->
        </StickySidebar>
        
        <!-- Main page content -->
    </MainContent>
    
    <Footer Links="@footerLinks" SocialMedia="@socialLinks" />
</PageLayout>

<PageLoading IsVisible="@isLoading" Message="Loading content..." />
```

## Layout Patterns

The layout components support various common patterns:

- **Single Column**: Simple, focused content layout
- **Two Column**: Main content with sidebar
- **Three Column**: Content with dual sidebars
- **Full Width**: Edge-to-edge content presentation
- **Centered**: Contained content with maximum width

## Responsive Behavior

All layout components include:

- **Mobile-First Design**: Optimized for mobile devices
- **Breakpoint Management**: Automatic layout adjustments
- **Touch-Friendly**: Mobile gesture and interaction support
- **Performance**: Optimized for all device types

These layout components provide the structural foundation for building professional, responsive web applications with consistent design patterns.
