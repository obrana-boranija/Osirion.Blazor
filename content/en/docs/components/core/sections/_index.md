---
id: 'core-sections-components-overview'
order: 1
layout: docs
title: Core Section Components
permalink: /docs/components/core/sections
description: Page sections from Osirion.Blazor Core, from heroes and content sections to pricing tables and numbered steps, rendering under static SSR.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Sections
tags:
- blazor
- sections
- hero
- pricing
- static-ssr
is_featured: true
published: true
slug: sections
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Section Components - Osirion.Blazor'
  description: 'Hero, content section, contact details, pricing table, feature comparison and steps components for Blazor pages, rendering under static SSR.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/sections'
  lang: en
  robots: index, follow
  og_title: 'Core Section Components - Osirion.Blazor'
  og_description: 'Full-width page sections for Blazor landing pages and articles, rendering under static SSR.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Section Components - Osirion.Blazor'
  twitter_description: 'Full-width page sections for Blazor landing pages and articles, rendering under static SSR.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Core Section Components

Section components build the blocks of a landing page or the top of an article: a hero, a titled content band,
contact details, pricing and the steps of a process. Each renders complete HTML under static SSR and needs no script,
except the responsive showcase, which is a demo tool for interactive pages.

Most of them live in the `Osirion.Blazor.Components` namespace of the Osirion.Blazor.Core package, and their data
types in `Osirion.Blazor.Core.Models`:

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models
```

## Components

| Component | What it renders | Script | Available |
| --- | --- | --- | --- |
| [Hero section](/docs/components/core/sections/hero-section) | A page header with title, subtitle, buttons and a side or background image | None | 4.0 |
| [Base section](/docs/components/core/sections/base-section) | A `<section>` with heading, description, container and background options | None | 4.0 |
| [Contact info section](/docs/components/core/sections/contact-info-section) | Address, phone numbers, email addresses and websites as links | None | 4.0 |
| [Pricing table and feature comparison](/docs/components/core/sections/pricing-table) | Plans side by side, and a table comparing their features | None | 4.1 |
| [Steps](/docs/components/core/sections/steps) | Numbered "how it works" steps as an ordered list | None | 4.1 |
| [Responsive showcase](/docs/components/core/sections/responsive-showcase-section) | A preview frame that switches between desktop, tablet and mobile widths | Interactive render mode | 4.0 |

The same folder holds `OsirionMetricGrid` and `OsirionContextCardGrid`, which do not have pages yet.

## Related

- [Background pattern](/docs/components/core/layout/background-pattern)
- [Cards](/docs/components/core/cards)
- [Core components](/docs/components/core)
