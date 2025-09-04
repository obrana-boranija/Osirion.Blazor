---
id: 'core-carousels-components-overview'
order: 1
layout: docs
title: Core Carousels Components Overview
permalink: /docs/components/core/carousels
description: Complete overview of Osirion.Blazor Core Carousel components including infinite logo carousels and testimonial carousels for dynamic content display.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Carousels
tags:
- blazor
- core-components
- carousels
- ui-components
- infinite-scroll
- testimonials
is_featured: true
published: true
slug: carousels
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Carousels Components - Osirion.Blazor Dynamic Content Display'
  description: 'Explore Osirion.Blazor Core Carousel components for infinite logo displays and testimonial rotations.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/carousels'
  lang: en
  robots: index, follow
  og_title: 'Core Carousels Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Carousel components with infinite scrolling and testimonial features.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Carousels Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Carousel components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Carousels Components Overview

The Osirion.Blazor Core Carousels module provides dynamic content display components that create engaging, interactive content rotations. These components are perfect for showcasing logos, testimonials, and other rotating content.

## Available Components

### Infinite Logo Carousel
A smooth, continuously scrolling carousel designed specifically for displaying partner logos, client brands, or technology stack representations with seamless infinite looping.

### Testimonial Carousel
An interactive carousel component for rotating through customer testimonials, reviews, and user feedback with navigation controls and automatic progression.

## Key Features

- **Smooth Animations**: CSS-powered smooth transitions and animations
- **Touch/Swipe Support**: Mobile-friendly touch navigation
- **Infinite Scrolling**: Seamless looping for continuous content display
- **Auto-play Options**: Configurable automatic progression timing
- **Responsive Design**: Adapts to all screen sizes and orientations
- **Accessibility**: Keyboard navigation and screen reader support

## Getting Started

To use carousel components in your project:

```razor
@using Osirion.Blazor.Core

<InfiniteLogoCarousel Logos="@logoList" 
                      Speed="@CarouselSpeed.Medium" 
                      Direction="@ScrollDirection.Left" />

<TestimonialCarousel Testimonials="@testimonialList" 
                     AutoPlay="true" 
                     Interval="5000" 
                     ShowNavigation="true" />
```

## Configuration Options

Both carousel components offer extensive customization options:

- **Speed Control**: Adjust animation and transition speeds
- **Direction Settings**: Configure scroll direction and flow
- **Auto-play Configuration**: Set timing and interaction behavior
- **Visual Styling**: Customize appearance and theming
- **Navigation Controls**: Enable/disable user interaction elements

Each carousel component is optimized for performance and provides smooth, engaging user experiences across all devices.
