---
id: 'osirion-content-not-found'
order: 1
layout: docs
title: OsirionContentNotFound
permalink: /docs/components/core/states/content-not-found
description: OsirionContentNotFound renders a 404 or empty state with an error code, message, buttons, suggested links, a search slot and a contact address.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- States
- Error Handling
tags:
- blazor
- 404
- not-found
- empty-state
- static-ssr
is_featured: true
published: true
slug: content-not-found
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionContentNotFound - Osirion.Blazor'
  description: 'A 404 or empty state for Blazor with an error code, message, buttons, suggested links and a contact address, rendered without script.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/states/content-not-found'
  lang: en
  robots: index, follow
  og_title: 'OsirionContentNotFound - Osirion.Blazor'
  og_description: 'A 404 or empty state with a message, buttons and suggested links, rendered without script.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionContentNotFound - Osirion.Blazor'
  twitter_description: 'A 404 or empty state with a message, buttons and suggested links, rendered without script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionContentNotFound

`OsirionContentNotFound` renders the body of a not found page: an icon, an error code, a heading, a message and
buttons back into the site, with optional suggested links, a search slot and a contact address. With the error code
removed, it also works as an empty state for a list or search. The CMS page components show it when their content
is missing.

## Usage

With no parameters it renders "404", "Page Not Found", a short message and a "Go to Homepage" button:

```razor
<OsirionContentNotFound />
```

A not found page that points readers to likely destinations:

```razor
<OsirionContentNotFound Title="This page has moved"
                        Message="We reorganized the docs. One of these pages may have what you need."
                        ShowSuggestions="true"
                        Suggestions="@suggestions"
                        SecondaryButtonText="All docs"
                        SecondaryButtonUrl="/docs"
                        ShowContactInfo="true"
                        ContactEmail="hello@example.org"
                        Variant="centered" />

@code {
    private readonly ContentNotFoundSuggestion[] suggestions =
    [
        new() { Text = "Components", Url = "/docs/components", Description = "Every component, by package." },
        new() { Text = "Blog", Url = "/blog" },
    ];
}
```

An empty state: clear the error code and the default button, and make it smaller. The title is still an `<h1>`, so
use this where the empty state is the page's main content, such as a blog index with no posts yet:

```razor
<OsirionContentNotFound ErrorCode="@null"
                        Title="No posts yet"
                        Message="The first article is on its way."
                        PrimaryButtonText="@null"
                        Variant="minimal"
                        Size="small" />
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `ErrorCode` | `string?` | `"404"` | Large code above the title. Hidden when empty. |
| `Title` | `string` | `"Page Not Found"` | Heading, rendered as an `<h1>`. |
| `Message` | `string` | `"The page you're looking for doesn't exist or has been moved."` | Paragraph under the title. Hidden when empty. |
| `ShowIcon` | `bool` | `true` | Shows an icon above the text. |
| `Icon` | `RenderFragment?` | `null` | Your own icon markup. Takes precedence over `IconUrl`. |
| `IconUrl` | `string?` | `null` | An image used as the icon. |
| `IconAlt` | `string` | `"Not Found"` | Alternative text of the `IconUrl` image. |
| `UseDefaultIcon` | `bool` | `true` | Shows the built-in magnifier icon when neither `Icon` nor `IconUrl` is set. |
| `PrimaryButtonText` | `string?` | `"Go to Homepage"` | Text of the primary button. Hidden when empty. |
| `PrimaryButtonUrl` | `string` | `"/"` | Address of the primary button. |
| `SecondaryButtonText` | `string?` | `null` | Text of the outlined secondary button. Hidden when empty. |
| `SecondaryButtonUrl` | `string` | `"/"` | Address of the secondary button. |
| `Actions` | `RenderFragment?` | `null` | More buttons or links, after the two buttons. |
| `ShowSuggestions` | `bool` | `false` | Shows the `Suggestions` list. |
| `SuggestionsTitle` | `string` | `"Here are some helpful links:"` | Text above the suggestions. |
| `Suggestions` | `IReadOnlyList<ContentNotFoundSuggestion>?` | `null` | Links to suggest. |
| `ShowSearchBox` | `bool` | `false` | Renders `SearchContent`. |
| `SearchContent` | `RenderFragment?` | `null` | A search form of your choice, such as the CMS `SearchBox`. |
| `ShowContactInfo` | `bool` | `false` | Shows `ContactMessage` and a `mailto:` link when `ContactEmail` is set. |
| `ContactMessage` | `string` | `"Need help? Contact us at"` | Text before the email address. |
| `ContactEmail` | `string?` | `null` | Address for the contact line. |
| `ChildContent` | `RenderFragment?` | `null` | Any other content, after the search slot. |
| `Variant` | `string` | `"default"` | `default`, `minimal`, `centered` or `hero`; see Styling. |
| `Size` | `string` | `"medium"` | `small`, `medium`, `large` or `full`; sets the minimum height. |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | A decorative pattern behind the content. |
| `Class` | `string?` | `null` | Extra CSS classes on the root element. |

Unmatched attributes are written to the root `<div>`. A `ContentNotFoundSuggestion` has `Text`, `Url`, an optional
`Description` shown under the link, and `Target` (default `_self`).

## Behavior

- The component renders plain HTML and needs no script.
- All text is HTML-encoded. Button and suggestion addresses are written as given, so pass addresses you control.
- `Title` is an `<h1>`, so use the component as the main content of the page, not inside a page that already has
  one.
- The buttons take the configured CSS framework's primary and outlined secondary button classes.

### Status code

The component renders markup only; it does not set the HTTP status code. Without a 404, search engines can index the
page as real content.

On .NET 10, give the router a not found page and use the component there:

```razor
<Router AppAssembly="typeof(Program).Assembly" NotFoundPage="typeof(Pages.NotFound)">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
    </Found>
</Router>
```

```razor
@page "/not-found"

<PageTitle>Page not found</PageTitle>

<OsirionContentNotFound SecondaryButtonText="All docs" SecondaryButtonUrl="/docs" />
```

When a page finds that its own content is missing, it calls `NavigationManager.NotFound()` on .NET 10, or sets
`Response.StatusCode` on the cascading `HttpContext` during static rendering on .NET 9. The CMS `ArticlePage`,
`DocumentPage` and `HomePage` do this themselves.

## Styling

The root has `osirion-content-not-found`, `osirion-content-not-found-{Variant}` and
`osirion-content-not-found-{Size}`, plus `osirion-content-not-found-with-code` when there is an error code and
`osirion-content-not-found-with-background` with a pattern.

| Variant | Look |
| --- | --- |
| `default` | Plain, on the page background |
| `minimal` | Smaller code, title, message and icon |
| `centered` | On a rounded panel in `--osirion-background-secondary` |
| `hero` | Full window height below the header, with a gradient background and a large gradient error code |

The `full` size fills the window below the header, using `--osirion-header-height` (default `4rem`). Inner elements
use `osirion-content-not-found-code`, `-title`, `-message`, `-suggestions`, `-search`, `-custom`, `-actions` and
`-contact`, each prefixed with `osirion-content-not-found`. Colors and sizes come from the theme tokens
(`--osirion-text-primary`, `--osirion-text-secondary`, `--osirion-action-primary`, `--osirion-font-size-*`).

## Related

- [Core state components](/docs/components/core/states)
- [Page loading](/docs/components/core/layout/page-loading)
- [Background pattern](/docs/components/core/layout/background-pattern)
