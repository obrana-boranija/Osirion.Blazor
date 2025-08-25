---
title: "Localization with Osirion CMS"
author: "Osirion Team"
date: "2025-01-25"
description: "Organize multilingual content and deliver localized pages with Osirion.Blazor CMS."
slug: "localization-in-osirion"
categories: [Localization]
tags: [i18n, Localization, Multilingual]
featured_image: "/images/docs/localization.jpg"
seo_properties:
  title: "Localization in Osirion.Blazor"
  description: "Use locale folders, frontmatter, and components to build multilingual sites."
  og_image_url: "/images/social/localization.jpg"
  type: "Article"
---

# Localization with Osirion CMS

## Structure

- content/en/..., content/fr/...
- Link translations via localization_id in frontmatter

## Components

```razor
<LocalizedNavigation CurrentLocale="en" />
<LocalizedContentView Path="@path" CurrentLocale="en" />
```
