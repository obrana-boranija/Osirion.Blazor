---
title: "Styling and Theming Osirion Components"
author: "Osirion Team"
date: "2025-01-25"
description: "Integrate Bootstrap, Fluent UI, MudBlazor, or Radzen and customize components using CSS variables."
slug: "osirion-styling-theming"
categories: [Design]
tags: [Styling, Theming, CSS]
featured_image: "/images/docs/styling.jpg"
seo_properties:
  title: "Osirion.Blazor Styling and Theming"
  description: "Learn framework integration, dark mode, and CSS variable customization for Osirion components."
  og_image_url: "/images/social/styling.jpg"
  type: "Article"
---

# Styling and Theming Osirion Components

This guide shows how to add OsirionStyles, switch CSS frameworks, and override styles with CSS variables.

## Quick setup

```razor
<OsirionStyles FrameworkIntegration="CssFramework.Bootstrap" UseStyles="true" />
```

## CSS variables

- Use global variables like --osirion-hero-background
- Prefer .razor.css for scoped overrides

## Dark mode

- Respect prefers-color-scheme or framework toggles

## Examples

```css
:root { --osirion-logo-grayscale: 1; }
@media (prefers-color-scheme: dark) { :root { --osirion-hero-text: #eee; } }
```
