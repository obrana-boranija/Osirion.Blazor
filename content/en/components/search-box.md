---
id: 'cms-web-search-box'
order: 6
title: SearchBox
permalink: /components/search-box
description: "SearchBox renders a search form that submits the query with GET to a results page, with no script, under static SSR and interactive modes."
author: Dejan Demonjić
date: "2024-12-29"
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- CMS Web Components
- Navigation Components
tags:
- cms
- search
- forms
- accessibility
published: true
slug: search-box
lang: en
redirect_from:
- /docs/components/cms-web/navigation/search-box
custom_fields:
  group: cms-navigation
seo_properties:
  title: "SearchBox Component - Osirion.Blazor CMS"
  description: "A search form for Blazor content sites that submits with GET to a results page built with ContentList. No script needed."
  image: /assets/images/blazor-cover.png
  canonical: "https://getosirion.com/components/search-box"
---

# SearchBox

`SearchBox` renders a search field and a submit button inside a plain HTML form. Submitting it sends the browser to
your results page with the query in the URL, for example `/search?q=routing`, so it works without script and in every
render mode.

## Usage

```razor
@using Osirion.Blazor.Cms.Web.Components

<SearchBox />
```

The results page reads the query and passes it to `ContentList`, which filters the content. Passing the query back
to `SearchBox` keeps it in the field:

```razor
@page "/search"
@using Osirion.Blazor.Cms.Web.Components

<SearchBox Title="Search the docs" SearchQuery="@(Query ?? string.Empty)" />

@if (!string.IsNullOrWhiteSpace(Query))
{
    <ContentList SearchQuery="@Query"
                 ShowPagination="true"
                 ItemsPerPage="12"
                 NoContentText="No results found." />
}

@code {
    [SupplyParameterFromQuery(Name = "q")]
    public string? Query { get; set; }
}
```

The example apps place a `SearchBox` in their navigation menu (`<SearchBox Class="p-3" />` in the Bootstrap app) and
have a `/search` page that passes the `q` query to `ContentList`.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `ActionUrl` | `string` | `"/search"` | Page the form submits to. |
| `QueryParameterName` | `string` | `"q"` | Name of the query string parameter that carries the search text. |
| `SearchQuery` | `string` | `""` | Text shown in the field, usually the current query. |
| `Placeholder` | `string` | `"Search content..."` | Placeholder of the field; also its accessible name. |
| `SearchButtonText` | `string` | `"Search"` | Visible text of the submit button. |
| `Title` | `string?` | `null` | Heading rendered as an `h2` above the form. No heading when null. |
| `Class` | `string?` | `null` | Extra CSS classes on the outer `div`. |

Other attributes on the tag are captured but not rendered.

## Behavior

- The form uses `method="get"` and `role="search"`. Nothing is posted and no antiforgery token is needed.
- `SearchBox` does not search. `ContentList` with a `SearchQuery` splits the text on spaces and keeps the published
  items where any word appears, ignoring case, in the title, description, rendered HTML, categories or tags. It is
  a substring match over the loaded content, not a ranked full-text index.
- The field has a visually hidden `label` with the text "Search", and an `aria-label` set to `Placeholder`, which
  screen readers announce instead. The button is announced as "Submit search"; its visible text is hidden from
  screen readers. Both of these fixed texts are English, so on other languages set `Placeholder` and
  `SearchButtonText` and expect the button name to stay "Submit search".
- The submitted text reaches your page through the query string. Render it with Razor (`@Query`), which encodes
  it; never write it out as raw markup.

## Styling

| Class | Element |
| --- | --- |
| `osirion-search-container` | Outer `div`, plus your `Class` |
| `osirion-search-title` | The `h2` heading |
| `osirion-search-box` | The `form` |
| `osirion-search-input` | The `input type="search"` |

The button takes the primary button class of the CSS framework configured in the theming options: `btn btn-primary`
for Bootstrap, `mud-button mud-button-filled mud-button-filled-primary` for MudBlazor, `rz-button rz-button-primary`
for Radzen, and `osirion-button osirion-button-primary` for Fluent UI and for no framework. The field uses
`--osirion-background-primary`, `--osirion-border-color`, `--osirion-text-primary` and
`--osirion-focus-ring-color`.

## Related

- [Content List](/components/content-list), which runs the search on the results page
- [CMS navigation components](/components/cms-navigation)
