---
id: 'osirion-testimonial-carousel'
order: 8
layout: docs
title: OsirionTestimonialCarousel
permalink: /components/testimonial-carousel
description: OsirionTestimonialCarousel shows testimonial cards in a continuously looping rail, driven by CSS, with a pause button and a static grid under reduced motion.
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
- testimonials
- reviews
- accessibility
- static-ssr
is_featured: true
published: true
slug: testimonial-carousel
lang: en
redirect_from:
- /docs/components/core/carousels/testimonial-carousel
custom_fields:
  group: sections
seo_properties:
  title: 'OsirionTestimonialCarousel Component - Osirion.Blazor'
  description: 'Show testimonial cards in a looping rail with ratings, LinkedIn links and read more links; it pauses on focus and respects reduced motion.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/testimonial-carousel'
  lang: en
  robots: index, follow
  og_title: 'OsirionTestimonialCarousel Component - Osirion.Blazor'
  og_description: 'Testimonial cards in a looping rail, driven by CSS, with a pause button and reduced motion support.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionTestimonialCarousel Component - Osirion.Blazor'
  twitter_description: 'Testimonial cards in a looping rail, driven by CSS.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionTestimonialCarousel

`OsirionTestimonialCarousel` shows testimonials as a rail of cards that loops continuously. Each card is an
[OsirionTestimonialCard](/components/testimonial-card) with the person's name, role, company, quote
and optionally a photo, star rating and links. The loop is a CSS animation, so it runs under static server-side
rendering without script.

## Usage

The enhancement script (dragging, wheel scrolling, pause button) loads through the BlazorJSComponents package.
Register its services once in `Program.cs`:

```csharp
builder.Services.AddJSComponents();
```

Always pass your own testimonials. Without `CustomTestimonials`, the component shows built-in sample testimonials.

```razor
@using Osirion.Blazor.Components

<OsirionTestimonialCarousel Title="What our customers say" CustomTestimonials="testimonials" />

@code {
    private readonly List<TestimonialItem> testimonials =
    [
        new("Jane Doe", "CTO", "Northwind Traders", "We moved our docs site to static SSR in a week."),
        new("Sam Lee", "Lead Developer", "Fabrikam", "The components render the same with and without script."),
        new("Ana Ruiz", "Product Manager", "Contoso", "Our editors publish from GitHub without touching code."),
    ];
}
```

With photos, ratings and links to case studies:

```razor
<OsirionTestimonialCarousel Title="Customer stories"
                            SectionTitle="Customer testimonials"
                            CustomTestimonials="testimonials"
                            ShowRating="true"
                            CardVariant="TestimonialVariant.Highlighted"
                            CardHeight="340"
                            Speed="CarouselSpeed.Slow" />

@code {
    private readonly List<TestimonialItem> testimonials =
    [
        new("Jane Doe", "CTO", "Northwind Traders", "We moved our docs site to static SSR in a week.",
            ProfileImageUrl: "/images/people/jane.jpg",
            ShowRating: true, Rating: 5,
            ReadMoreHref: "/customers/northwind", ReadMoreText: "Read the case study"),
        new("Sam Lee", "Lead Developer", "Fabrikam", "The components render the same with and without script.",
            ProfileImageUrl: "/images/people/sam.jpg",
            LinkedInUrl: "https://www.linkedin.com/in/your-profile",
            ShowRating: true, Rating: 4),
        new("Ana Ruiz", "Product Manager", "Contoso", "Our editors publish from GitHub without touching code.",
            ShowRating: true, Rating: 5),
    ];
}
```

A star rating shows only when both the carousel's `ShowRating` and the item's `ShowRating` are `true`.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `CustomTestimonials` | `List<TestimonialItem>?` | `null` | The testimonials. When `null`, built-in samples are shown. |
| `Title` | `string?` | `null` | Heading shown above the rail, as an `<h2>`. |
| `SectionTitle` | `string` | `"Customer Testimonials"` | Accessible name (`aria-label`) of the `<section>`. |
| `AnimationDuration` | `int` | `60` | Seconds for one full loop at `Normal` speed. Lower is faster. |
| `Speed` | `CarouselSpeed` | `Normal` | `Slow` lengthens the loop, `Fast` shortens it. |
| `Direction` | `AnimationDirection` | `Right` | `Right` moves the cards from right to left; `Left` reverses the direction. |
| `PauseOnHover` | `bool` | `true` | Pauses the loop while the pointer is over the carousel. |
| `CardWidth` | `int` | `400` | Card width in pixels. |
| `CardHeight` | `int` | `300` | Fixed card height in pixels; size it for your longest quote. |
| `CardGap` | `int` | `24` | Gap between cards in pixels. |
| `MaxVisibleTestimonials` | `int?` | `null` | Shows only the first N testimonials. |
| `CardVariant` | `TestimonialVariant` | `Default` | `Default`, `Minimal`, `Highlighted` or `Compact`. |
| `CardSize` | `TestimonialSize` | `Normal` | `Small`, `Normal` or `Large`. |
| `CardElevated` | `bool?` | `null` | Elevation for every card whose item does not set `Elevated`. |
| `CardBorderless` | `bool` | `false` | Renders the cards without a border. |
| `ShowRating` | `bool` | `false` | Allows star ratings on the cards. |
| `InheritTheme` | `bool` | `false` | Gives the cards the carousel's `Theme` instead of `System`. |
| `Class` | `string?` | `null` | Extra classes on the `<section>`. |
| `Style` | `string?` | `null` | Inline style on the `<section>`. |
| `Theme` | `ThemeMode` | `System` | The carousel's theme; reaches the cards only with `InheritTheme`. |

Unmatched attributes go on the `<section>`.

### TestimonialItem

`TestimonialItem` is a record: `new TestimonialItem(Name, Position, Company, TestimonialText, ...)` with the other
values named.

| Value | Type | Default | Description |
| --- | --- | --- | --- |
| `Name` | `string` | required | The person's name, rendered in a `<cite>`. |
| `Position` | `string?` | `null` | Job title. With `Company`, shown as "Position at Company". |
| `Company` | `string?` | `null` | Company name. |
| `TestimonialText` | `string?` | `null` | The quote, as plain text. |
| `ProfileImageUrl` | `string?` | `null` | Photo; its alt text is built from the name, position and company. |
| `LinkedInUrl` | `string?` | `null` | Turns the name into a link that opens in a new tab. |
| `ShowRating` | `bool` | `false` | Shows this item's rating (when the carousel's `ShowRating` is on too). |
| `Rating` | `int` | `5` | Stars out of five. |
| `ReadMoreHref` | `string?` | `null` | Adds a read more link at the bottom of the card. |
| `ReadMoreText` | `string?` | `"Read more"` | Text of that link. |
| `ReadMoreVariant` | `ReadMoreVariant` | `Default` | Style of that link. |
| `ReadMoreTarget` | `string?` | `null` | `target` of that link. |
| `AdditionalCssClass` | `string?` | `null` | Extra classes on this card. |
| `Elevated` | `bool?` | `null` | Elevation for this card; overrides `CardElevated`. |

## Behavior

- **Without script** the rail loops with a CSS animation. The cards are rendered twice so the loop has no seam; the
  second set is `aria-hidden` and `inert`, so screen readers read each testimonial once and its links are not in
  the tab order.
- **With script** the visitor can drag the rail or move it with a horizontal wheel or trackpad gesture, a pause button
  appears in the top right corner, and the photos, stars and LinkedIn icons are shown in color only on the cards
  nearest the center (and on the card under the pointer).
- **Pausing.** Hover pauses the loop when `PauseOnHover` is on; keyboard focus inside the carousel always pauses it.
  Translate the pause button with `data-pause-label` and `data-play-label` attributes on the component.
- **Reduced motion.** Under `prefers-reduced-motion: reduce` the animation stops and every testimonial is shown once,
  in a wrapped grid of two columns (one on narrow screens).
- **Fewer than three items.** The carousel repeats items until there are three, and it does so by adding to the list
  you passed in. Pass at least three testimonials, or a list you do not reuse elsewhere. An empty list throws.
- **MaxVisibleTestimonials.** When it is set to a positive number, the visible items must still number three or more:
  a value of 1 or 2, or a list of fewer than three items combined with this parameter, keeps the component from
  finishing its first render. Leave it unset unless you pass more testimonials than you want to show.
- **Images.** The photos of the first two cards load eagerly with high fetch priority; the rest load lazily.
- **Text and URLs.** Names, roles and quotes are rendered as text, so Razor encodes them. `LinkedInUrl`,
  `ReadMoreHref` and `ProfileImageUrl` are written as given, so pass only URLs you trust.

## Styling

| Class or property | Purpose |
| --- | --- |
| `osirion-testimonial-carousel` | The `<section>` |
| `osirion-testimonial-carousel-title` | The heading |
| `osirion-testimonial-carousel-track` | The moving rail |
| `osirion-testimonial-carousel-slide` | The slot around each card |
| `osirion-carousel-pause` | The pause button the script adds |
| `--osirion-testimonial-carousel-duration`, `--osirion-testimonial-card-width`, `--osirion-testimonial-card-height`, `--osirion-testimonial-card-gap` | Set on the rail from the parameters |

Spacing, font sizes and colors come from the theme tokens (`--osirion-spacing-*`, `--osirion-font-size-*`,
`--osirion-text-primary`), so a CSS framework adapter restyles the carousel with the rest of the site. The cards have
their own classes; see the [testimonial card](/components/testimonial-card) page.

## Related

- [Testimonial card](/components/testimonial-card)
- [InfiniteLogoCarousel](/components/infinite-logo-carousel)
- [Carousels](/components/sections)
