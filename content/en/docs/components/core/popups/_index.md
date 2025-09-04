---
id: 'core-popups-components-overview'
order: 1
layout: docs
title: Core Popups Components Overview
permalink: /docs/components/core/popups
description: Complete overview of Osirion.Blazor Core Popups components including cookie consent dialogs for user interaction and compliance.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Popups
tags:
- blazor
- core-components
- popups
- cookie-consent
- privacy
- compliance
is_featured: true
published: true
slug: popups
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Popups Components - Osirion.Blazor User Interaction'
  description: 'Explore Osirion.Blazor Core Popups components for cookie consent and user interaction dialogs.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/popups'
  lang: en
  robots: index, follow
  og_title: 'Core Popups Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Popups components with cookie consent and privacy features.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Popups Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Popups components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Popups Components Overview

The Osirion.Blazor Core Popups module provides essential user interaction components for displaying important information, obtaining consent, and managing user preferences. These components ensure compliance with privacy regulations while maintaining excellent user experience.

## Available Components

### Cookie Consent
A comprehensive cookie consent dialog component that helps websites comply with GDPR, CCPA, and other privacy regulations while providing users with clear choices about data collection and tracking.

## Key Features

- **Legal Compliance**: GDPR, CCPA, and international privacy law compliance
- **Granular Controls**: Detailed cookie category management
- **Customizable Design**: Flexible styling and branding options
- **Multi-language Support**: Localization for international audiences
- **Accessibility**: Screen reader compatible and keyboard navigable
- **Storage Management**: Automatic consent preference storage

## Getting Started

To use popup components in your project:

```razor
@using Osirion.Blazor.Core

<CookieConsent OnConsentChanged="@HandleConsentChange"
               RequiredCategories="@requiredCookies"
               OptionalCategories="@optionalCookies"
               Position="@ConsentPosition.Bottom"
               ShowDetailedOptions="true" />
```

## Cookie Consent Features

The Cookie Consent component includes:

- **Essential Cookies**: Always-allowed required functionality cookies
- **Analytics Cookies**: Optional tracking and analytics consent
- **Marketing Cookies**: Advertising and targeting cookie controls
- **Functional Cookies**: Enhanced functionality and personalization
- **Custom Categories**: Define application-specific cookie types

## Compliance Features

Built-in compliance capabilities:

- **Consent Recording**: Timestamp and choice logging
- **Withdrawal Options**: Easy consent modification and withdrawal
- **Clear Information**: Detailed cookie purpose explanations
- **Opt-out Support**: Respect user privacy choices
- **Legal Text**: Customizable privacy policy and terms links

## Configuration Options

Extensive customization options:

- **Position**: Top, bottom, overlay, or custom positioning
- **Styling**: Colors, fonts, and layout customization
- **Behavior**: Auto-hide, persistent, or interaction-based display
- **Content**: Custom messaging and legal text
- **Integration**: Cookie management service connections

## Privacy by Design

The popup components follow privacy-by-design principles:

- **Minimal Data Collection**: Only necessary consent tracking
- **User Control**: Clear and accessible preference management
- **Transparency**: Open about data usage and sharing
- **Security**: Secure consent storage and transmission

These popup components ensure your application meets privacy requirements while providing users with clear, accessible control over their data preferences.
