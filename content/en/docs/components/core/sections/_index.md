---
id: 'core-sections-components-overview'
order: 1
layout: docs
title: Core Sections Components Overview
permalink: /docs/components/core/sections
description: Complete overview of Osirion.Blazor Core Sections components including base sections, contact info, hero sections, and responsive showcase sections.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Sections
tags:
- blazor
- core-components
- sections
- hero-sections
- layout-sections
is_featured: true
published: true
slug: components/core/sections
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Sections Components - Osirion.Blazor Page Sections'
  description: 'Explore Osirion.Blazor Core Sections components for hero sections, contact info, and responsive layouts.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/sections'
  lang: en
  robots: index, follow
  og_title: 'Core Sections Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Sections components with hero sections and layout patterns.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Sections Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Sections components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Sections Components Overview

The Osirion.Blazor Core Sections module provides essential page section components for building structured, visually appealing web layouts. These components offer pre-built section patterns that accelerate development while maintaining design consistency.

## Available Components

### Base Section
A foundational section component that provides the structure and styling foundation for all other section types. Includes padding, margins, background options, and responsive behavior.

### Contact Info Section
A specialized section component for displaying contact information including addresses, phone numbers, email addresses, social media links, and business hours in an organized, accessible format.

### Hero Section
A prominent, eye-catching section component typically used at the top of pages to showcase key messages, calls-to-action, background images, and primary content with strong visual impact.

### Responsive Showcase Section
A flexible section component designed for showcasing products, services, portfolios, or features with responsive grid layouts, image galleries, and interactive elements.

## Key Features

- **Responsive Design**: All sections adapt seamlessly to different screen sizes
- **Flexible Content**: Support for various content types and layouts
- **Consistent Styling**: Unified design patterns across sections
- **Accessibility**: WCAG compliant with proper semantic structure
- **Performance Optimized**: Efficient rendering and loading

## Getting Started

To use section components in your project:

```razor
@using Osirion.Blazor.Core

<HeroSection Title="Welcome to Our Platform"
             Subtitle="Build amazing applications with Osirion.Blazor"
             BackgroundImage="@heroImage"
             CallToAction="@ctaButton" />

<ContactInfoSection Address="@companyAddress"
                   Phone="@phoneNumber"
                   Email="@contactEmail"
                   SocialMedia="@socialLinks" />

<ResponsiveShowcaseSection Items="@showcaseItems"
                          Layout="@ShowcaseLayout.Grid"
                          Columns="3" />

<BaseSection BackgroundColor="@SectionColor.Light"
             Padding="@SectionPadding.Large">
    <!-- Custom section content -->
</BaseSection>
```

## Section Patterns

Common section layout patterns:

- **Full Width**: Edge-to-edge content spanning the viewport
- **Contained**: Centered content with maximum width constraints
- **Split Layout**: Two-column arrangements for content and media
- **Card Grid**: Grid-based layouts for multiple content items
- **Feature Blocks**: Structured feature presentation

## Hero Section Features

The Hero Section component includes:

- **Background Options**: Images, videos, gradients, or solid colors
- **Typography Control**: Flexible title and subtitle styling
- **Call-to-Action**: Prominent button and link placements
- **Overlay Support**: Text readability enhancement options
- **Animation**: Optional entrance animations and transitions

## Showcase Section Features

The Responsive Showcase Section offers:

- **Grid Layouts**: Flexible column configurations
- **Image Handling**: Optimized image loading and display
- **Interactive Elements**: Hover effects and click behaviors
- **Filtering**: Category-based content filtering
- **Pagination**: Large dataset handling

## Contact Section Features

The Contact Info Section provides:

- **Structured Data**: Schema.org markup for local SEO
- **Multiple Formats**: Various contact information layouts
- **Map Integration**: Optional map display capabilities
- **Social Media**: Social platform link integration
- **Business Hours**: Structured schedule display

These section components provide the building blocks for creating professional, responsive web pages with consistent design patterns and optimal user experience.
