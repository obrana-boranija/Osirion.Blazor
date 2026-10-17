---
order: 9
title: LandingPage
permalink: /components/landing-page
description: "LandingPage renders a marketing page from one content item or from parameters: a hero, a Markdown body in a configurable section, custom HTML and your own content."
author: "Osirion Team"
date: 2026-10-17
last_modified: 2026-10-17
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Page Components
tags:
- landing-page
- cms
- hero
- marketing
- static-ssr
slug: landing-page
lang: en
layout: "component"
published: true
custom_fields:
  group: cms-pages
seo_properties:
  title: 'LandingPage: a marketing page for Blazor'
  description: 'LandingPage renders a marketing page from one CMS content item or from parameters: a hero, a body section and your own content.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/landing-page'
  lang: en
  robots: index, follow
  og_title: 'LandingPage - Osirion.Blazor'
  og_description: 'A marketing page from one content item or from parameters: a hero, a body section and your own content.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'LandingPage - Osirion.Blazor'
  twitter_description: 'A marketing page from one content item or from parameters: a hero, a body section and your own content.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# LandingPage

`LandingPage` renders a marketing page: a tall hero, the Markdown body of a content item in a section you can size,
align and style, and optional HTML or markup of your own after it. Use it for a product or campaign page whose text
lives in your content repository, or whose hero you set from parameters.

Package `Osirion.Blazor.Cms.Web`, namespace `Osirion.Blazor.Cms.Components`. [Source on GitHub](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Cms.Web/Components/PageLevel/LandingPage.razor).

There is no live demo on this page. `LandingPage` writes the page's title and structured data into the head, and its
hero is 70 percent of the window tall by default, so it does not belong inside another page.

## Usage

Load the page from a content item by its source path, with a hero that links to the documentation and to the source:

```razor
@page "/launch"
@using Osirion.Blazor.Cms.Components

<LandingPage ContentPath="en/launch.md"
             HeroHeight="50vh"
             HeroPrimaryButtonText="Get started"
             HeroPrimaryButtonUrl="/docs"
             HeroSecondaryButtonText="Source on GitHub"
             HeroSecondaryButtonUrl="https://github.com/demonjic/Osirion.Blazor"
             ContentWidth="narrow" />
```

Without an item, the hero comes from `HeroTitle`, `HeroSubtitle` and `HeroBackgroundImage`, and `ChildContent` holds
the rest of the page:

```razor
<LandingPage HeroTitle="Launch week" HeroSubtitle="Everything that ships on Monday.">
    <p>Details follow.</p>
</LandingPage>
```

When an item is set, its title and description win over `HeroTitle` and `HeroSubtitle`, which only fill in for a
missing item or description. Pass an item you loaded yourself with `Content` instead of `ContentPath` when you also
need it elsewhere.

## Parameters

Many parameters of this page do nothing: `HeroAlignment`, `ShowHeroButtons`, the testimonial parameters
(`ShowTestimonials`, `TestimonialsSectionTitle`, `TestimonialsLayout`, `ShowTestimonialImages`, `TestimonialItems`),
the feature parameters (`ShowFeatures`, `FeaturesSectionTitle`, `FeaturesSectionDescription`, `FeaturesLayout`,
`FeaturesColumns`, `FeatureItems`) and the call to action parameters (`ShowCallToAction`, every `Cta` parameter).
The page renders no testimonials, features or call to action section; the types `TestimonialItem` and `FeatureItem`
are declared but nothing reads them. Put such sections in `ChildContent` instead. The table says which parameters
have no effect.

<!-- osirion:parameters LandingPage -->

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `ContentPath` | `string?` | `null` | The source path of the item to load from the default provider while `Content` is not set. When nothing is found, the page shows `NotFoundText` and does not signal 404. |
| `Content` | `ContentItem?` | `null` | The item that makes the page: its title and description are the hero's, its featured image the hero's image and its body follows the hero. Without an item the page shows what the other parameters give it. |
| `ShowHero` | `bool` | `true` | Whether to show a hero with the title, the description, the image and up to two buttons. |
| `HeroTitle` | `string?` | `null` | The hero's title when there is no `Content`. The item's title wins when both are set. |
| `HeroSubtitle` | `string?` | `null` | The hero's subtitle when there is no `Content`, or when the item has no description. |
| `HeroBackgroundImage` | `string?` | `null` | The hero's image address when the item has no featured image. |
| `HeroAlignment` | `string` | `"center"` | Has no effect: the hero is not given the alignment. |
| `HeroHeight` | `string` | `"70vh"` | The minimum height of the hero, as a CSS length such as `70vh` or `30rem`. |
| `ShowHeroButtons` | `bool` | `true` | Has no effect: a hero button shows whenever its text and address are both set. |
| `HeroPrimaryButtonText` | `string?` | `null` | The text of the hero's primary button. The button needs `HeroPrimaryButtonUrl` too. |
| `HeroPrimaryButtonUrl` | `string?` | `null` | The address of the hero's primary button. |
| `HeroSecondaryButtonText` | `string?` | `null` | The text of the hero's secondary button. The button needs `HeroSecondaryButtonUrl` too. |
| `HeroSecondaryButtonUrl` | `string?` | `null` | The address of the hero's secondary button. |
| `ContentWidth` | `string` | `"normal"` | The width of the section that holds the item's body: `narrow`, `normal`, `wide` or `full`. |
| `ContentAlign` | `string` | `"left"` | The text alignment of the body: `left`, `center` or `right`. |
| `ContentVariant` | `string` | `"default"` | The look of the section that holds the body: `default`, `highlight`, `bordered` or `background`. |
| `ContentPadding` | `string` | `"normal"` | The padding of the section that holds the body: `none`, `small`, `normal` or `large`. |
| `ShowTestimonials` | `bool` | `false` | Has no effect: the page renders no testimonials section. |
| `TestimonialsSectionTitle` | `string` | `"What Our Customers Say"` | Has no effect: the page renders no testimonials section. |
| `TestimonialsLayout` | `string` | `"grid"` | Has no effect: the page renders no testimonials section. |
| `ShowTestimonialImages` | `bool` | `true` | Has no effect: the page renders no testimonials section. |
| `TestimonialItems` | `IReadOnlyList<TestimonialItem>?` | `null` | Has no effect: the page renders no testimonials section. |
| `ShowFeatures` | `bool` | `false` | Has no effect: the page renders no features section. |
| `FeaturesSectionTitle` | `string` | `"Features"` | Has no effect: the page renders no features section. |
| `FeaturesSectionDescription` | `string?` | `null` | Has no effect: the page renders no features section. |
| `FeaturesLayout` | `string` | `"grid"` | Has no effect: the page renders no features section. |
| `FeaturesColumns` | `int` | `3` | Has no effect: the page renders no features section. |
| `FeatureItems` | `IReadOnlyList<FeatureItem>?` | `null` | Has no effect: the page renders no features section. |
| `ShowCallToAction` | `bool` | `true` | Has no effect: the page renders no call to action section. |
| `CtaTitle` | `string?` | `null` | Has no effect: the page renders no call to action section. |
| `CtaDescription` | `string?` | `null` | Has no effect: the page renders no call to action section. |
| `CtaPrimaryButtonText` | `string?` | `null` | Has no effect: the page renders no call to action section. |
| `CtaPrimaryButtonUrl` | `string?` | `null` | Has no effect: the page renders no call to action section. |
| `CtaSecondaryButtonText` | `string?` | `null` | Has no effect: the page renders no call to action section. |
| `CtaSecondaryButtonUrl` | `string?` | `null` | Has no effect: the page renders no call to action section. |
| `CtaBackgroundColor` | `string?` | `null` | Has no effect: the page renders no call to action section. |
| `CtaVariant` | `string` | `"primary"` | Has no effect: the page renders no call to action section. |
| `CustomContent` | `string?` | `null` | HTML shown in a wide, highlighted section after the body. It is written through `OsirionHtmlRenderer`, which sanitizes it with the registered HTML sanitizer. |
| `LoadingText` | `string` | `"Loading page..."` | The text shown while the item loads, in interactive render modes only. A statically rendered response is sent once the item has loaded. |
| `NotFoundText` | `string` | `"Page content not found."` | The message shown when `ContentPath` finds no item. The response keeps its status code. |
| `ChildContent` | `RenderFragment?` | `null` | Content shown in a wide section after the body and the custom content. |
| `Theme` | `ThemeMode` | `ThemeMode.System` | The component's color mode. `Light` or `Dark` writes it to `data-theme` (`data-bs-theme` under Bootstrap) on the root element; `System` writes nothing. |
| `Class` | `string?` | `null` | Extra CSS classes for the component's root element. |
| `Style` | `string?` | `null` | An inline style for the component's root element. |

<!-- /osirion:parameters -->

The `Theme` and `Style` rows come from the base class. `LandingPage` does not write them, or unmatched attributes, so
only `Class` reaches the root element.

## Behavior

- **Rendering.** The page renders on the server and needs no script. The loading text appears only in interactive
  render modes.
- **Order.** The page writes, in this order: the head metadata, the hero (`ShowHero`), the body section, `CustomContent`
  in a wide highlighted section and `ChildContent` in a wide section.
- **Head.** The page writes a `SeoMetadataRenderer` for the item and passes nothing else to it, so all of the
  renderer's other settings take their defaults.
- **Hero.** The hero shows the item's title and description (or `HeroTitle` and `HeroSubtitle`), the item's featured
  image (or `HeroBackgroundImage`) and up to two buttons, each shown when its text and its address are both set.
  `HeroHeight` sets its minimum height. While the hero is shown, the body leaves out its own leading `h1`, so the page
  keeps a single `h1`.
- **Body section.** The body is written by `ContentSection` with `ContentWidth` (`narrow`, `normal`, `wide`, `full`),
  `ContentAlign` (`left`, `center`, `right`), `ContentVariant` (`default`, `highlight`, `bordered`, `background`) and
  `ContentPadding` (`none`, `small`, `normal`, `large`).
- **Not found.** With a `ContentPath` that finds nothing, the page shows `NotFoundText` in a plain block. The response
  keeps its status code, so under static SSR it is a 200; use `ArticlePage` or `DocumentPage` when a missing page must
  answer 404.
- **Security.** The body and `CustomContent` pass through `OsirionHtmlRenderer`, which sanitizes them.

## Styling

The root element has the class `osirion-landing-page`. The hero is a `HeroSection` and the body, custom content and
child content are `ContentSection` elements (`osirion-content-section`), which the page's stylesheet spaces and sizes.
Colors come from theme tokens such as `--osirion-text-primary`, `--osirion-text-secondary`, `--osirion-action-primary`
and `--osirion-background-primary`.

## Related

- [CMS pages](/components/cms-pages): the group overview
- [HeroSection](/components/hero-section): the hero and its parameters
- [HomePage](/components/home-page): a home page from one content item
- [Sections](/components/sections): pricing, steps, metrics and testimonials to use in `ChildContent`
