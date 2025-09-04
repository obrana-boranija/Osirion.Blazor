---
id: 'core-cards-components-overview'
order: 1
layout: docs
title: Core Cards Components Overview
permalink: /docs/components/core/cards
description: Complete overview of Osirion.Blazor Core Cards components including feature cards, subscription cards, and testimonial cards for modern UI layouts.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Cards
tags:
- blazor
- core-components
- cards
- ui-components
- feature-cards
- testimonials
is_featured: true
published: true
slug: cards
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Cards Components - Osirion.Blazor UI Cards'
  description: 'Explore Osirion.Blazor Core Cards components for feature cards, subscription cards, and testimonial displays.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/cards'
  lang: en
  robots: index, follow
  og_title: 'Core Cards Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Cards components with feature, subscription, and testimonial cards.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Cards Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Cards components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Cards Components Overview

The Osirion.Blazor Core Cards module provides a collection of card components for building modern, responsive UI layouts. These components offer flexible, reusable card designs for various content types.

## Available Components

### Feature Card
A versatile card component for showcasing features, services, or product highlights with icons, titles, and descriptions.

### Subscription Card
Specialized card component for displaying subscription plans, pricing tiers, and service offerings with clear call-to-action elements.

### Testimonial Card
Dedicated card component for customer testimonials, reviews, and user feedback with author attribution and styling options.

## Key Features

- **Responsive Design**: All cards adapt to different screen sizes
- **Customizable Styling**: Flexible theming and appearance options
- **Accessibility**: Built-in accessibility features and ARIA support
- **Performance Optimized**: Lightweight and efficient rendering

## Getting Started

To use card components in your project:

```razor
@using Osirion.Blazor.Core

<FeatureCard Title="Amazing Feature" 
             Description="This feature will revolutionize your workflow" 
             Icon="star" />

<SubscriptionCard Title="Pro Plan" 
                  Price="$29/month" 
                  Features="@features" />

<TestimonialCard Quote="This product changed everything for us!" 
                 Author="John Doe" 
                 Company="Tech Corp" />
```

Each card component is designed to be highly customizable while maintaining consistent design patterns throughout your application.
