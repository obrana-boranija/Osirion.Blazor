---
id: 'components-overview'
order: 1
layout: docs
title: Components overview
permalink: /docs/components
description: Every Osirion.Blazor component by module and package (Core, CMS web, CMS admin, Navigation, Analytics, Theming) and what they share.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Documentation
- Blazor
tags:
- blazor
- components
- core
- cms
- navigation
- analytics
- theming
is_featured: true
published: true
slug: components
lang: en
custom_fields: {}
seo_properties:
  title: 'Osirion.Blazor components overview'
  description: 'The Osirion.Blazor components grouped by module and package, with the parameters and behavior they all share.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components'
  lang: en
  robots: index, follow
  og_title: 'Osirion.Blazor components overview'
  og_description: 'The Osirion.Blazor components grouped by module and package.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Osirion.Blazor components overview'
  twitter_description: 'The Osirion.Blazor components grouped by module and package.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Components overview

Osirion.Blazor is split into modules, and each module is its own NuGet package. The `Osirion.Blazor` package
references all of them and adds the `AddOsirion` registration; see [Getting started](/docs/getting-started) for the
setup. This page lists the components by module and links to their reference pages.

## What every component shares

- **Server-rendered markup.** Components render their HTML on the server, so pages work under static SSR. A few
  (for example the carousels, `OsirionImageGallery` and `OsirionPageLayout`) load a small script of their own for
  behavior that needs the browser; register `AddJSComponents()` so they can.
- **Common parameters.** Components inherit `OsirionComponentBase`, which adds `Class` and `Style` for the root
  element, `Attributes` (any unmatched attribute is passed through) and `Theme` (a `ThemeMode`: `Light`, `Dark`,
  `System` or `Inverted`, default `System`). The old `SetInteractive` parameter is obsolete and setting it is a
  compile error; interactivity is detected automatically.
- **Sanitized content.** Markdown is rendered to HTML and cleaned with an allow-list before it is shown, so script,
  event handler attributes and `javascript:` URLs in content are removed.

Components added in 4.1 are marked below and ship in the next release.

## Core

Package `Osirion.Blazor.Core`, namespace `Osirion.Blazor.Components`. Building blocks for any page.

- [Layout](/docs/components/core/layout): `OsirionPageLayout`, `OsirionFooter`, `OsirionStickySidebar`,
  `OsirionBackgroundPattern`.
- [Sections](/docs/components/core/sections): `HeroSection`, `OsirionBaseSection`, `OsirionContactInfoSection`,
  `OsirionResponsiveShowcaseSection`, `OsirionContextCardGrid`, `OsirionMetricGrid`. From 4.1:
  `OsirionPricingTable`, `OsirionFeatureComparison`, `OsirionSteps`.
- [Cards](/docs/components/core/cards): `OsirionFeatureCard`, `OsirionTestimonialCard`, `OsirionSubscriptionCard`,
  `OsirionMetricCard`. From 4.1: `OsirionAuthorCard`.
- [Carousels](/docs/components/core/carousels): `InfiniteLogoCarousel`, `OsirionTestimonialCarousel`.
- [Content](/docs/components/core/content): `OsirionArticleMetdata`. From 4.1: `OsirionCallout`, `OsirionTabs`,
  `OsirionAccordion`, `OsirionFaq`, `OsirionResponsiveImage`, `OsirionVideoEmbed`, `OsirionReadingProgress`.
- [Forms](/docs/components/core/forms): `OsirionContactForm`.
- [Navigation](/docs/components/core/navigation): `OsirionBreadcrumbs`, `OsirionReadMoreLink`.
- [Popups](/docs/components/core/popups): `OsirionCookieConsent`. From 4.1: `OsirionAnnouncementBar`.
- [Rendering](/docs/components/core/rendering): `OsirionHtmlRenderer`, `OsirionReveal`.
- [States](/docs/components/core/states): `OsirionPageLoading`, `OsirionContentNotFound`.
- `OsirionImageGallery`, a gallery with a lightbox.

## CMS web

Package `Osirion.Blazor.Cms.Web`. Components that read content from the configured provider (a local folder or a
GitHub repository).

- [Content](/docs/components/cms-web/core): `ContentList`, `ContentView`, `ContentPage`, `ContentRenderer`,
  `LocalizedContentView`, `SeoMetadataRenderer`.
- [Navigation](/docs/components/cms-web/navigation): `ContentBreadcrumbs`, `CategoriesList`, `TagCloud`,
  `SearchBox`, `TableOfContents`, `DirectoryNavigation`, `LocalizedNavigation`, `OsirionContentNavigation`. From
  4.1: `ContentSeriesNavigation`.
- Page-level components that combine the above: `ArticlePage`, `DocumentPage`, `LandingPage` and `HomePage`.

## CMS admin

Package `Osirion.Blazor.Cms.Admin`. Admin pages under `/osirion` for browsing and editing content in a GitHub
repository, with a Markdown editor and preview. Every admin page requires the `Osirion.Cms.Admin` authorization
policy, so the host must add its own sign-in first. The `Osirion.Blazor.Cms` package combines CMS web and CMS admin.

## Navigation

Package `Osirion.Blazor.Navigation`, namespace `Osirion.Blazor.Navigation.Components`.
[Navigation components](/docs/components/navigation): `Menu` with `MenuItem`, `MenuGroup`, `MenuDivider`,
`MenuCallToAction`, `MenuPanel`, `MenuPanelLink` and `MenuImageCard`; `EnhancedNavigation`, which handles scroll
position after Blazor's enhanced navigation; `ScrollToTop` and `ScrollToTopProvider`.

## Analytics

Package `Osirion.Blazor.Analytics`, namespace `Osirion.Blazor.Analytics.Components`.
[Analytics components](/docs/components/analytics): `ClarityTracker`, `MatomoTracker`, `GA4Tracker` and
`YandexMetricaTracker`, configured under `Osirion:Analytics`. From 4.1: `PlausibleTracker` and `UmamiTracker`.

## Theming

Package `Osirion.Blazor.Theming`, namespace `Osirion.Blazor.Theming.Components`.
[Theming components](/docs/components/theming): [OsirionStyles](/docs/components/theming/osirion-styles) loads the
stylesheets and the adapter for Bootstrap, Fluent UI, MudBlazor or Radzen;
[ThemeToggle](/docs/components/theming/theme-toggle) is a button that switches between the light and dark theme.
