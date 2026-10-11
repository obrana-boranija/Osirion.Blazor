---
id: 'core-carousels-components-overview'
order: 1
layout: docs
title: Core Carousel Components
permalink: /docs/components/core/carousels
description: The two Osirion.Blazor Core carousels, a looping logo strip and a looping testimonial rail, both rendered in full under static SSR.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Carousels
tags:
- blazor
- carousel
- logos
- testimonials
- static-ssr
- accessibility
is_featured: true
published: true
slug: carousels
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Carousel Components - Osirion.Blazor'
  description: 'A looping logo strip and a looping testimonial rail for Blazor, with CSS animation, a pause button and reduced motion support.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/carousels'
  lang: en
  robots: index, follow
  og_title: 'Core Carousel Components - Osirion.Blazor'
  og_description: 'A looping logo strip and a looping testimonial rail for Blazor, rendered under static SSR.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Carousel Components - Osirion.Blazor'
  twitter_description: 'A looping logo strip and a looping testimonial rail for Blazor.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Core Carousel Components

Osirion.Blazor Core has two carousels for marketing pages: a strip of client or partner logos, and a rail of
testimonial cards. Both render every item as HTML under static server-side rendering and move with a CSS animation.
A small script adds dragging, horizontal wheel scrolling and a pause button.

| Component | What it shows | Moves by default |
| --- | --- | --- |
| [InfiniteLogoCarousel](/docs/components/core/carousels/infinite-logo-carousel) | Logos, optionally linked, with light and dark variants | No, set `EnableAutoScroll` |
| [OsirionTestimonialCarousel](/docs/components/core/carousels/testimonial-carousel) | Testimonial cards with name, role, quote and rating | Yes |

## What they share

- **Setup.** The enhancement script loads through the BlazorJSComponents package, which Osirion.Blazor.Core
  references. Register its services once in `Program.cs`: `builder.Services.AddJSComponents();`.
- **Your own items.** Pass your logos or testimonials. Without them, both components show built-in sample content
  meant for demos.
- **Motion.** The animation pauses while the pointer is over the carousel (unless `PauseOnHover` is off) and while
  keyboard focus is inside it. While the carousel animates, the script adds a pause button, and it runs its
  per-frame work only while the carousel is on screen and moving. Vertical wheel scrolling always reaches the page.
- **Reduced motion.** Under `prefers-reduced-motion: reduce` the animation stops; the testimonial carousel then shows
  its cards as a static wrapped grid.
- **Looping copies.** The items repeated to close the loop are `aria-hidden` and `inert`, so screen readers
  read each item once and their links are not in the tab order.
- **Labels.** Each carousel is a `<section>` named by `SectionTitle`. The pause button's labels default to "Pause
  animation" and "Play animation"; set `data-pause-label` and `data-play-label` attributes on the component to
  translate them.

## Related

- [Testimonial card](/docs/components/core/cards/testimonial-card)
- [Core components](/docs/components/core)
