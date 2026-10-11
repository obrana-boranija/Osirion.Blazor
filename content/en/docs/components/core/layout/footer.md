---
id: 'osirion-footer'
order: 3
layout: docs
title: OsirionFooter
permalink: /docs/components/core/layout/footer
description: A site footer with link sections, social links, a copyright line, legal links and free content slots, rendered on the server without script.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Layout
tags:
- blazor
- footer
- layout
- navigation
- static-ssr
- accessibility
is_featured: true
published: true
slug: footer
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionFooter: Blazor site footer with link sections | Osirion.Blazor'
  description: 'A Blazor site footer with link sections, social links, a copyright line, legal links and content slots, rendered without script.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/layout/footer'
  lang: en
  robots: index, follow
  og_title: 'OsirionFooter - Osirion.Blazor'
  og_description: 'Site footer with link sections, social links, copyright and legal links.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionFooter - Osirion.Blazor'
  twitter_description: 'Site footer with link sections, social links, copyright and legal links.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionFooter

`OsirionFooter` renders a site footer: a top row with your logo and description, columns of links and social links,
then a bottom row with the copyright line and legal links. Everything is plain markup, so it works the same under
static SSR and interactive rendering.

## Usage

With only a company name, the footer shows a copyright line for the current year:

```razor
@using Osirion.Blazor.Components

<OsirionFooter CompanyName="Example Ltd" CompanyUrl="https://example.com" />
```

A fuller footer passes link data from code:

```razor
<OsirionFooter CompanyName="Example Ltd"
               CompanyUrl="/"
               Description="Components for content sites built with Blazor."
               Links="links"
               SocialLinks="socialLinks"
               BottomLinks="legalLinks">
    <Logo>
        <img src="/logo.svg" width="40" height="40" alt="Example Ltd" />
    </Logo>
</OsirionFooter>

@code {
    private readonly IReadOnlyList<FooterSection> links =
    [
        new FooterSection
        {
            Title = "Documentation",
            Links =
            [
                new FooterLink { Text = "Getting started", Href = "/docs" },
                new FooterLink { Text = "Components", Href = "/docs/components" }
            ]
        },
        new FooterSection
        {
            Title = "Project",
            Links =
            [
                new FooterLink { Text = "Source code", Href = "https://github.com/obrana-boranija/Osirion.Blazor", OpenInNewTab = true }
            ]
        }
    ];

    private readonly IReadOnlyList<FooterSocialLink> socialLinks =
    [
        new FooterSocialLink { Text = "NuGet", Href = "https://www.nuget.org/packages/Osirion.Blazor", AriaLabel = "Osirion.Blazor on NuGet" }
    ];

    private readonly IReadOnlyList<FooterLink> legalLinks =
    [
        new FooterLink { Text = "Privacy", Href = "/privacy" },
        new FooterLink { Text = "Terms", Href = "/terms" }
    ];
}
```

`FooterSection`, `FooterLink` and `FooterSocialLink` are in the `Osirion.Blazor.Components` namespace.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `CompanyName` | `string` | `"Company"` | Name in the default copyright line. |
| `CompanyUrl` | `string` | `"/"` | Link in the default copyright line; an address starting with `http` opens in a new tab. |
| `Description` | `string?` | `null` | Text under the logo, rendered as HTML. Shown only when the top row renders (see Behavior). |
| `Copyright` | `string?` | `null` | Replaces the default copyright line; rendered as HTML. |
| `ShowCopyright` | `bool` | `true` | Shows the default copyright line when `Copyright` is empty. |
| `Logo` | `RenderFragment?` | `null` | Logo in the top row. |
| `LeftContent` | `RenderFragment?` | `null` | Free content in the left column of the top row, above the logo. |
| `RightContent` | `RenderFragment?` | `null` | Free content in the right column of the top row. |
| `Links` | `IReadOnlyList<FooterSection>?` | `null` | Link columns, each with an optional `Title` and a list of `FooterLink`. |
| `SocialLinks` | `IReadOnlyList<FooterSocialLink>?` | `null` | Social links; each shows its `Icon` fragment, or its `Text` when there is no icon. |
| `ShowSocialLinks` | `bool` | `true` | Shows the social links. |
| `SocialTitle` | `string` | `"Follow Us"` | Heading above the social links; empty hides it. |
| `BottomLinks` | `IReadOnlyList<FooterLink>?` | `null` | Legal links in the bottom row. |
| `ShowDivider` | `bool` | `true` | Line between the top and bottom rows. |
| `ShowTopSection` | `bool` | `true` | Renders the top row. |
| `ShowBottomSection` | `bool` | `true` | Renders the bottom row. |
| `Variant` | `string` | `"default"` | Adds `osirion-footer-{Variant}`; the stylesheet styles `minimal` and `centered`. |
| `GridLayout` | `string` | `"auto"` | Adds `osirion-footer-grid-{GridLayout}`; `2-column`, `3-column` and `4-column` set fixed link columns. |
| `Docked` | `bool` | `false` | Docks the footer with `DockingMode`. |
| `DockingMode` | `string` | `"sticky"` | `"sticky"` pushes the footer down with `margin-top: auto`; `"fixed"` fixes it to the bottom of the viewport. |
| `ChildContent` | `RenderFragment?` | `null` | Extra content below the bottom row. |
| `Theme` | `ThemeMode` | `System` | `Light` or `Dark` forces that palette on the footer; `System` follows the site. |

`Class`, `Style` and unmatched attributes go on the outer element.

`FooterLink` has `Text`, `Href` (default `"#"`) and `OpenInNewTab`, which adds `target="_blank"` and
`rel="noopener noreferrer"`. `FooterSocialLink` adds `Icon` and `AriaLabel`; social links always open in a new tab.

## Behavior

- **Landmark.** On its own the footer is a `footer` element. Inside the `Footer` slot of
  [OsirionPageLayout](/docs/components/core/layout/page-layout) it renders a `div` with the same classes, because the
  layout already provides the `footer` landmark. Available from 4.0; in 3.x the footer nested a second `footer`.
- **Headings.** Section titles and the social title are `h2` elements, so the outline does not skip a level after a
  page's `h1`.
- **Legal links** are wrapped in a `nav` labelled "Footer legal navigation".
- **Top row.** It renders when at least one of `Logo`, `Links`, `SocialLinks`, `LeftContent` or `RightContent` is set.
  `Description` alone does not make it render.
- **Social links without text.** When a link has only an icon, set `AriaLabel` so screen reader users hear where it
  goes.

> [!WARNING]
> `Description` and `Copyright` are rendered as raw HTML. Pass only text you control, never visitor or CMS input.

## Styling

The outer element has `osirion-footer`, `osirion-footer-{Variant}` and `osirion-footer-grid-{GridLayout}`, plus
`osirion-footer-docked-{DockingMode}` when docked. Inner parts include `osirion-footer-main`, `osirion-footer-brand`,
`osirion-footer-links`, `osirion-footer-section-title`, `osirion-footer-link`, `osirion-footer-social-link`,
`osirion-footer-divider`, `osirion-footer-bottom`, `osirion-footer-copyright` and `osirion-footer-bottom-link`.

Colors come from the theme tokens `--osirion-background-primary`, `--osirion-text-primary`,
`--osirion-text-secondary`, `--osirion-border-color` and `--osirion-action-primary`, so the CSS framework adapters
recolor the footer. With `Theme` set to `Light` or `Dark`, the footer overrides those tokens on itself.

A fixed footer (`Docked="true"` with `DockingMode="fixed"`) covers the end of the page. Give your main content a
bottom padding equal to the footer's height in your own stylesheet; the component's scoped stylesheet does not reach
the `body`.

## Related

- [OsirionPageLayout](/docs/components/core/layout/page-layout)
- [OsirionContactInfoSection](/docs/components/core/sections/contact-info-section)
- [OsirionSubscriptionCard](/docs/components/core/cards/subscription-card)
- [Layout overview](/docs/components/core/layout)
