---
id: 'navigation-components-overview'
order: 1
layout: docs
title: Navigation
permalink: /docs/components/navigation
description: Scroll handling for Blazor enhanced navigation, a scroll-to-top button, and a responsive menu with submenus and mega menus that needs no JavaScript.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Navigation Components
- User Experience
tags:
- blazor
- navigation
- menu
- mega-menu
- enhanced-navigation
- scroll-to-top
- static-ssr
is_featured: true
published: true
slug: navigation
lang: en
custom_fields: {}
seo_properties:
  title: 'Navigation Components for Blazor - Osirion.Blazor'
  description: 'EnhancedNavigation, ScrollToTop and a responsive Menu with mega menus for Blazor, working under static SSR.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/navigation'
  lang: en
  robots: index, follow
  og_title: 'Navigation Components for Blazor - Osirion.Blazor'
  og_description: 'Scroll handling for enhanced navigation, a scroll-to-top button and a responsive menu without JavaScript.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Navigation Components for Blazor - Osirion.Blazor'
  twitter_description: 'EnhancedNavigation, ScrollToTop and a responsive Menu for Blazor.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# Navigation

`Osirion.Blazor.Navigation` contains three things:

- `EnhancedNavigation` controls where the page scrolls after a Blazor enhanced navigation.
- `ScrollToTop` and `ScrollToTopProvider` show a "back to top" button once the visitor scrolls down.
- `Menu` and its child components build a site menu with submenus and mega menus. The menu opens and closes with CSS
  only, so it works under Static SSR without JavaScript.

## Setup

```bash
dotnet add package Osirion.Blazor.Navigation
```

The `Osirion.Blazor` meta-package includes it, and `AddOsirion()` registers enhanced navigation and the scroll-to-top
button with their defaults. With this package alone:

```csharp
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Extensions;

builder.Services.AddOsirionNavigation(navigation => navigation
    .AddEnhancedNavigation(options => options.Behavior = ScrollBehavior.Smooth)
    .AddScrollToTop(options => options.VisibilityThreshold = 400));
```

Or from configuration, with `AddOsirionNavigation(builder.Configuration)` or `AddOsirion(builder.Configuration)`:

```json
{
  "Osirion": {
    "Navigation": {
      "Enhanced": {
        "Behavior": "Smooth",
        "ResetScrollOnNavigation": true,
        "PreserveScrollForSamePageNavigation": true
      },
      "ScrollToTop": {
        "Position": "BottomRight",
        "VisibilityThreshold": 300,
        "Title": "Back to top"
      }
    }
  }
}
```

Add `@using Osirion.Blazor.Navigation.Components` to `_Imports.razor`. The enums `ScrollBehavior` and `Position`
are in `Osirion.Blazor.Components`.

## EnhancedNavigation

Blazor enhanced navigation replaces the page content without a full load. `EnhancedNavigation` listens for Blazor's
`enhancedload` event and scrolls to the top of the new page. Place it in `App.razor` after the Blazor script, because
it looks for the `Blazor` object when it runs:

```razor
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
    <EnhancedNavigation />
</body>
```

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Behavior` | `ScrollBehavior?` | `null` | `Auto`, `Instant` or `Smooth`. Unset, the configured value applies (default `Auto`). |
| `ResetScrollOnNavigation` | `bool?` | `null` | Scroll to the top after an enhanced navigation. Configured default `true`. |
| `PreserveScrollForSamePageNavigation` | `bool?` | `null` | Keep the scroll position when an enhanced update leaves the URL unchanged. Configured default `true`. |

The component writes one inline script and needs no interactive render mode.

## ScrollToTop

`ScrollToTop` renders a fixed button that appears after the visitor scrolled past `VisibilityThreshold` pixels and
scrolls back to the top when clicked. `ScrollToTopProvider` renders the same button from the registered settings,
so you configure it once in `Program.cs` or `appsettings.json`:

```razor
@inherits LayoutComponentBase

@Body

<ScrollToTopProvider />
```

To set it per layout instead, use the component directly:

```razor
<ScrollToTop Position="Position.BottomLeft" Text="Top" IconCssClass="bi bi-arrow-up" />
```

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Position` | `Position` | `BottomRight` | `TopLeft`, `TopRight`, `BottomLeft` or `BottomRight`. |
| `Behavior` | `ScrollBehavior` | `Smooth` | How the page scrolls up. |
| `VisibilityThreshold` | `int` | `300` | Pixels scrolled before the button appears. |
| `Title` | `string` | `"Scroll to top"` | The button's `aria-label`. |
| `Text` | `string?` | `null` | Visible text next to the icon; hidden at 768px and below. |
| `IconCssClass` | `string?` | `null` | A class for an icon font, rendered on an `aria-hidden` span instead of the default arrow. |
| `CustomIcon` | `string?` | `null` | Icon markup. It is sanitized, which removes inline SVG; prefer `IconCssClass`. |
| `Enabled` | `bool?` | `null` | `false` renders nothing. Unset, the configured `Enabled` applies. |
| `Class` | `string?` | `null` | Extra classes on the button. |

A parameter left at its default value lets the configured value apply. The button is hidden until its inline script
runs, so it needs JavaScript; each button gets a unique `id` and registers its scroll listener once.

## Menu

`Menu` renders a `<nav>` with an optional brand link and your items. Below the collapse breakpoint, a hamburger opens
the items. The toggle and every submenu are visually hidden checkboxes with labels, so they work from the keyboard
and without JavaScript.

```razor
<Menu AriaLabel="Main navigation" BrandText="My site" BrandLogo="/favicon.png"
      Alignment="MenuAlignment.Right">
    <MenuItem Text="Home" Href="/" Icon="home" />
    <MenuItem Text="Docs" Href="/docs" Icon="docs" HasSubmenu="true">
        <MenuItem Text="Getting started" Href="/docs/getting-started" />
        <MenuItem Text="Components" Href="/docs/components" />
    </MenuItem>
    <MenuItem Text="GitHub" Href="https://github.com/obrana-boranija/Osirion.Blazor" Target="_blank" />
</Menu>
```

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `ChildContent` | `RenderFragment?` | `null` | The menu items. |
| `BrandText` | `string?` | `null` | Brand name, linked to `Href`. |
| `BrandLogo` | `string?` | `null` | Logo image URL, shown before `BrandText`. |
| `BrandingTemplate` | `RenderFragment?` | `null` | Custom brand markup; replaces `BrandLogo` and `BrandText`. |
| `Href` | `string?` | `"/"` | Target of the brand link. |
| `Orientation` | `MenuOrientation` | `Horizontal` | `Horizontal` or `Vertical`. |
| `Alignment` | `MenuAlignment` | `Center` | `Left`, `Center` or `Right`; horizontal menus only. |
| `Sticky` | `bool` | `true` | Keeps a horizontal menu at the top while scrolling. |
| `StickyZIndex` | `int` | `1000` | `z-index` of a sticky menu. |
| `CollapseOnMobile` | `bool` | `true` | Collapses the items behind a hamburger toggle. |
| `CollapseBreakpoint` | `MenuCollapseBreakpoint` | `Mobile` | `Mobile` collapses at 768px and below, `ExtraLarge` below 1440px. |
| `AutoExpandActive` | `bool` | `true` | Vertical menus: adds the `osirion-menu-auto-expand` class to the container. |
| `AriaLabel` | `string` | `"Navigation menu"` | Accessible name of the `<nav>`. |
| `ToggleAriaLabel` | `string` | `"Toggle navigation menu"` | Accessible name of the mobile toggle. |
| `Id` | `string?` | `null` | Container id. Each menu gets a unique default id. |

### Menu items

`MenuItem` is a link. It marks itself active (`aria-current="page"`) when the current path matches `Href`; the
comparison ignores the query string, the fragment and surrounding slashes.

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Text` | `string` | `""` | Link text. |
| `Href` | `string` | `"#"` | Link target. |
| `Description` | `string?` | `null` | Supporting text below the link text. |
| `Icon` | `string?` | `null` | A built-in icon: `home`, `blog`, `document`, `docs`, `documentation`, `user`, `profile`, `contact`, `about`, `info`, `settings`, `gear` or `dashboard`. |
| `IconTemplate` | `RenderFragment?` | `null` | Your own icon markup; replaces `Icon`. |
| `Target` | `string?` | `null` | Link target; `_blank` adds `rel="noopener noreferrer"`. |
| `HasSubmenu` | `bool` | `false` | Renders `ChildContent` as a submenu with a disclosure toggle. |
| `ChildContent` | `RenderFragment?` | `null` | The submenu content. |
| `SubmenuVariant` | `SubmenuVariant` | `Regular` | `Regular` for a list, `Mega` for a grid of groups. |
| `SubmenuAside` | `RenderFragment?` | `null` | A right-hand column in a `Mega` submenu. |
| `IsActive` | `bool` | `false` | Forces the active state. |
| `AutoDetectActive` | `bool` | `true` | Detects the active state from the URL. |
| `Match` | `NavLinkMatch` | `All` | `All` matches the exact path; `Prefix` also matches child paths on a segment boundary. |
| `Disabled` | `bool` | `false` | Renders no `href` and sets `aria-disabled`. |
| `OnClick` | `EventCallback<MouseEventArgs>` | none | Raised on click in interactive render modes. |
| `Id` | `string?` | `null` | Base for the submenu ids. |

Under Static SSR the active state is computed once per request. In an interactive render mode the item also follows
later navigations.

The other building blocks:

- `MenuDivider` draws a separator (`role="separator"`), with an optional `Label`.
- `MenuGroup` groups items in a submenu column: `Label`, `Description`, and a footer link from `FooterText`,
  `FooterHref` and `FooterTarget`. `Collapsible` with `Expanded="false"` hides the items; there is no built-in control
  that opens the group again.
- `MenuCallToAction` is a highlighted link with `Text`, `Description`, `Href` and `Target`.
- `MenuPanel` (`Title`, `Description`, `AriaLabel`) holds `MenuPanelLink` items (`Text`, `Href`, `Target`,
  `IconTemplate`) as a promotional aside.
- `MenuImageCard` is a linked image with a caption: `ImageUrl`, `ImageAlt`, `Title`, `Description`, `Href`, `Target`.

A mega menu with an aside:

```razor
<MenuItem Text="Product" Href="/product" HasSubmenu="true" SubmenuVariant="SubmenuVariant.Mega">
    <ChildContent>
        <MenuGroup Label="Build" FooterText="All features" FooterHref="/features">
            <MenuItem Text="Content" Description="Markdown from GitHub or the file system." Href="/features/content" />
            <MenuItem Text="SEO" Description="Metadata, sitemaps and feeds." Href="/features/seo" />
        </MenuGroup>
        <MenuCallToAction Text="Get started" Description="Install the package." Href="/docs" />
    </ChildContent>
    <SubmenuAside>
        <MenuPanel Title="New in this release">
            <MenuPanelLink Text="Changelog" Href="/changelog" />
        </MenuPanel>
    </SubmenuAside>
</MenuItem>
```

### Security

`MenuItem`, `MenuGroup` footers, `MenuPanelLink`, `MenuImageCard`, `MenuCallToAction` and the brand link render an
`href` only for relative URLs and the `http`, `https`, `mailto` and `tel` schemes. With a `javascript:` URL from
CMS-driven menu data the element is rendered without an `href` (a `MenuGroup` footer is left out), so nothing can run.

## Styling

The menu reads the theme tokens (`--osirion-text-primary`, `--osirion-background-primary`, `--osirion-border-color`
and others), so it follows the light and dark theme. Useful hooks:

- `osirion-menu-container`, `osirion-menu-horizontal` or `osirion-menu-vertical`, `osirion-menu-align-*`,
  `osirion-menu-sticky` on the container;
- `osirion-menu-item`, `osirion-menu-item-active`, `osirion-submenu`, `osirion-submenu-mega` on items and submenus;
- `--osirion-menu-panel-background` and `--osirion-menu-panel-color` for `MenuPanel`.

`ScrollToTop` gets the framework's primary button class (`btn btn-primary` with Bootstrap) plus
`osirion-scroll-to-top` and its position class (`bottom-right` and so on). `--osirion-scroll-size` sets its size
(default `45px`, `40px` at 768px and below).

## Related

- [Theming](/docs/components/theming): put a [`ThemeToggle`](/docs/components/theming/theme-toggle) in the menu.
- [CMS navigation](/docs/components/cms-web/navigation): menus, breadcrumbs and a table of contents built from your
  content.
- [Core navigation](/docs/components/core/navigation): breadcrumbs and read-more links.
- [Osirion.Blazor.Navigation on NuGet](https://www.nuget.org/packages/Osirion.Blazor.Navigation)
