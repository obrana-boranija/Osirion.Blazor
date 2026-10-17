---
id: 'osirion-read-more-link'
order: 2
layout: docs
title: OsirionReadMoreLink
permalink: /docs/components/core/navigation/read-more-link
description: OsirionReadMoreLink renders a read more or call-to-action link with an optional icon, six variants, three sizes and a stretched mode for cards.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Navigation
tags:
- blazor
- links
- call-to-action
- cards
- accessibility
is_featured: true
published: true
slug: read-more-link
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionReadMoreLink - Osirion.Blazor'
  description: 'A read more or call-to-action link for Blazor with an optional icon, six variants, three sizes and a stretched mode for cards.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/navigation/read-more-link'
  lang: en
  robots: index, follow
  og_title: 'OsirionReadMoreLink - Osirion.Blazor'
  og_description: 'A read more or call-to-action link with an optional icon, variants, sizes and a stretched mode for cards.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionReadMoreLink - Osirion.Blazor'
  twitter_description: 'A read more or call-to-action link with an optional icon, variants, sizes and a stretched mode for cards.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionReadMoreLink

`OsirionReadMoreLink` renders an `<a>` with text and an optional icon, for "read more" links in cards and lists and
for small calls to action. The feature and testimonial cards use it for their own links.

## Usage

The smallest form needs only `Href`. The text defaults to "Read more" with a chevron on the right:

```razor
<OsirionReadMoreLink Href="/blog/static-ssr" />
```

In a list of posts every link would read "Read more", so give each one an accessible name that starts with the
visible text. With `Stretched`, the whole card becomes clickable; the card needs `position: relative` because the
link covers its nearest positioned ancestor:

```razor
<article class="post-card" style="position: relative;">
    <h3>Static SSR with Blazor</h3>
    <p>What renders on the server, and what needs interactivity.</p>
    <OsirionReadMoreLink Href="/blog/static-ssr"
                         AriaLabel="Read more: Static SSR with Blazor"
                         Stretched="true" />
</article>
```

Other variants change the icon or the look:

```razor
<OsirionReadMoreLink Href="https://github.com/demonjic/Osirion.Blazor"
                     Text="Source on GitHub"
                     Variant="ReadMoreVariant.External"
                     Target="_blank"
                     rel="noopener" />

<OsirionReadMoreLink Href="/docs"
                     Text="Read the docs"
                     Variant="ReadMoreVariant.Button"
                     Size="LinkSize.Large" />
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Href` | `string` | `"#"` | The link address. Required. Written as given, so pass only addresses you trust. |
| `Text` | `string` | `"Read more"` | The visible text. |
| `AriaLabel` | `string?` | `null` | The accessible name. When empty, the component sets it to `Text`. |
| `ShowText` | `bool` | `true` | Renders the text. Turn it off for an icon-only link, and set `AriaLabel`. |
| `ShowIcon` | `bool` | `true` | Renders the icon. |
| `IconPosition` | `IconPosition` | `Right` | `Left` or `Right` of the text. |
| `IconContent` | `RenderFragment?` | `null` | Your own icon in place of the built-in one. |
| `Variant` | `ReadMoreVariant` | `Default` | `Default`, `Arrow`, `External`, `Download`, `Plain` or `Button`; see below. |
| `Size` | `LinkSize` | `Normal` | `Small`, `Normal` or `Large`. |
| `Stretched` | `bool` | `false` | Makes the link cover its nearest positioned ancestor, such as a card. |
| `Block` | `bool` | `false` | Displays the link as a full-width block. |
| `Target` | `string?` | `null` | The `target` attribute, for example `_blank`. |
| `Animated` | `bool` | `true` | Moves the icon slightly on hover. |
| `ChildContent` | `RenderFragment?` | `null` | Extra content rendered inside the link, after the text and icon. |
| `Class` | `string?` | `null` | Extra CSS classes on the link. |
| `Style` | `string?` | `null` | Inline style on the link. |

Unmatched attributes, such as `rel` or `data-*`, are written to the `<a>` element.

### Variants

| Variant | Icon | Look |
| --- | --- | --- |
| `Default` | Chevron | Primary color |
| `Arrow` | Arrow | Primary color, semibold |
| `External` | Box with an outgoing arrow | Primary color |
| `Download` | Tray with a downward arrow | Primary color |
| `Plain` | Chevron | Body text color, normal weight; combine with `ShowIcon="false"` for text only |
| `Button` | Chevron | Filled with the primary color, padded like a button |

## Behavior

- The component renders a plain link and needs no script; it works the same under static SSR, Interactive Server
  and WebAssembly.
- The built-in icons are inline SVG inside a `<span aria-hidden="true">`, so screen readers read only the accessible
  name.
- `aria-label` replaces the link's content as its accessible name. Keep the visible text at the start of it ("Read
  more: Static SSR with Blazor") so speech input users can say what they see.
- `Target` is written as given and no `rel` is added; pass `rel` as an attribute when you need one.
- With `prefers-reduced-motion: reduce`, the hover animation and transitions are off. In print, the icon is hidden.

## Styling

The link always has `osirion-read-more` plus a class for its variant (`osirion-read-more-default`,
`osirion-read-more-arrow` and so on) and its size (`osirion-read-more-small`, `-normal`, `-large`). Depending on the
parameters it also gets `osirion-read-more-stretched`, `osirion-read-more-block`, `osirion-read-more-animated` and
`osirion-read-more-icon-left` or `osirion-read-more-icon-right`. Inside, the text is `osirion-read-more-text` and the
icon `osirion-read-more-icon`.

The color is `--osirion-action-primary` and the hover color `--osirion-read-more-hover-color`, which falls back to
`--osirion-color-primary-700`; when the browser prefers a dark color scheme, the hover color is
`--osirion-action-primary-hover` instead. Set them on the link (through `Style`) or a container to recolor it:

```razor
<OsirionReadMoreLink Href="/pricing"
                     Text="See pricing"
                     Style="--osirion-action-primary: #7c3aed; --osirion-read-more-hover-color: #5b21b6;" />
```

The link also takes the configured CSS framework's link class:

| Framework | Link class | Stretched class |
| --- | --- | --- |
| Bootstrap | `icon-link icon-link-hover` (not when stretched) | `stretched-link` |
| Fluent UI | `osirion-fluent-link` | `osirion-fluent-stretched` |
| MudBlazor | `mud-link` | `mud-stretched` |
| Radzen | `rz-link` | `rz-stretched` |
| None | `osirion-link` | `osirion-stretched` |

## Related

- [Breadcrumbs](/docs/components/core/navigation/breadcrumbs)
- [Feature card](/docs/components/core/cards/feature-card)
- [Testimonial card](/docs/components/core/cards/testimonial-card)
- [Core navigation components](/docs/components/core/navigation)
