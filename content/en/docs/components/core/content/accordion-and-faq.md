---
id: 'osirion-accordion-and-faq'
order: 5
layout: docs
title: "Accordion and FAQ - Osirion Blazor"
permalink: /docs/components/core/content/accordion-and-faq
description: "OsirionAccordion builds collapsible sections on the native details element; OsirionFaq renders questions and answers with FAQPage structured data."
author: Dejan Demonjić
date: 2026-10-11
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Content
tags:
- accordion
- faq
- structured-data
- json-ld
- static-ssr
- accessibility
slug: accordion-and-faq
lang: en
published: true
---

# Accordion and FAQ

`OsirionAccordion` and `OsirionAccordionItem` render collapsible sections on the native `<details>` element, so they
open and close without script and work with the keyboard as they are. `OsirionFaq` uses them to render questions and
answers, and also writes FAQPage structured data so search engines can read the answers.

Available from 4.1.

## Usage

### Accordion

```razor
@using Osirion.Blazor.Components

<OsirionAccordion>
    <OsirionAccordionItem Title="Requirements" Open="true">
        <p>.NET 9 or .NET 10.</p>
    </OsirionAccordionItem>
    <OsirionAccordionItem Title="Installation">
        <pre><code>dotnet add package Osirion.Blazor</code></pre>
    </OsirionAccordionItem>
</OsirionAccordion>
```

Set `AllowMultiple="false"` to keep at most one item open: opening one closes the others. An `OsirionAccordionItem`
also works on its own, outside an accordion, as a single collapsible section.

### FAQ

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<OsirionFaq Items="faq" AllowMultiple="false" />

@code {
    private readonly FaqItem[] faq =
    [
        new("Does Osirion.Blazor need JavaScript?",
            "No. Every component renders with static server-side rendering; script only adds enhancements."),
        new("Where does the content come from?",
            "Markdown files in a GitHub repository or in a local folder."),
    ];
}
```

`FaqItem` is a record in `Osirion.Blazor.Core.Models` with two values, `Question` and `Answer`.

## Parameters

### OsirionAccordion

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `ChildContent` | `RenderFragment?` | `null` | The `OsirionAccordionItem` components. |
| `AllowMultiple` | `bool` | `true` | Whether several items can be open at once. When `false`, the items share a `name` and the browser closes the others when one opens. |
| `Class` | `string?` | `null` | Extra classes on the container `<div>`. |

### OsirionAccordionItem

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Title` | `string` | `""` | The `<summary>` text the visitor clicks, or presses Enter on, to open and close the section. Marked `[EditorRequired]`. |
| `Open` | `bool` | `false` | Whether the section is open when the page renders. |
| `ChildContent` | `RenderFragment?` | `null` | The section's content. |
| `Class` | `string?` | `null` | Extra classes on the `<details>` element. |

### OsirionFaq

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Items` | `IReadOnlyList<FaqItem>` | empty | The questions and answers. With no items the component renders nothing. |
| `IncludeStructuredData` | `bool` | `true` | Whether FAQPage structured data (JSON-LD) is written after the questions. |
| `AllowMultiple` | `bool` | `true` | Whether several answers can be open at once. |
| `Class` | `string?` | `null` | Extra classes on the accordion container, next to `osirion-faq`. |

On all three components, unmatched attributes go on the root element (`OsirionFaq` passes them to its accordion).

## Behavior

- Everything is native HTML: `<details>` and `<summary>` open and close in the browser with no script, in static SSR
  and in interactive render modes.
- One-at-a-time accordions rely on the `name` attribute of `<details>`. Browsers that do not support it let several
  items stay open; nothing else breaks.
- Answers in `OsirionFaq` are plain text, encoded like any Razor text, so HTML in an answer shows as text. Line breaks
  you write in an answer (`\n`) are kept on screen.
- The structured data is a `<script type="application/ld+json">` element with an FAQPage whose `mainEntity` lists each
  question with its accepted answer. It is built with System.Text.Json, whose default encoder escapes `<`, `>` and
  `&`, so a question or answer cannot close the script element. Text that cannot be written as JSON (half of a
  surrogate pair) leaves the structured data out, and the questions still render.
- Turn `IncludeStructuredData` off when the page already describes the same questions in its own structured data,
  and use one FAQPage per page.

## Styling

| Class | Element |
| --- | --- |
| `osirion-accordion` | The container |
| `osirion-accordion-item` | Each `<details>` |
| `osirion-accordion-summary` | The `<summary>`, with a chevron on the right in place of the native marker |
| `osirion-accordion-content` | The content below the summary |
| `osirion-faq` | Added to the container by `OsirionFaq` |
| `osirion-faq-answer` | The paragraph holding an answer |

The styles use the theme tokens `--osirion-border-color` (the lines between items), `--osirion-text-primary`,
`--osirion-text-secondary` (answers), `--osirion-action-primary` (summary hover color) and `--osirion-focus-ring-width`
with `--osirion-focus-ring-color` for the keyboard focus ring. Each has a fallback value, and a CSS framework adapter
that sets them restyles the accordion with the site. The chevron's rotation is not animated under
`prefers-reduced-motion: reduce`.

## Related

- [Tabs](/docs/components/core/content/tabs)
- [Callouts](/docs/components/core/content/callout)
- [Content components](/docs/components/core/content)
