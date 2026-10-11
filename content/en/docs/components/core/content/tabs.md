---
id: 'osirion-tabs'
order: 4
layout: docs
title: "Tabs - Osirion Blazor"
permalink: /docs/components/core/content/tabs
description: "OsirionTabs and OsirionTab show alternative content behind tabs that switch without script and work with the keyboard, under static SSR."
author: Dejan Demonjić
date: 2026-10-11
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Content
tags:
- tabs
- static-ssr
- accessibility
- code-samples
slug: tabs
lang: en
published: true
---

# Tabs

`OsirionTabs` shows one of several panels at a time, with a row of tabs to switch between them. Use it for
alternative versions of the same content, such as one install command per package manager. The tabs switch without
script, so they work under static server-side rendering.

Available from 4.1.

## Usage

```razor
@using Osirion.Blazor.Components

<OsirionTabs AriaLabel="Install command">
    <OsirionTab Title=".NET CLI">
        <pre><code>dotnet add package Osirion.Blazor</code></pre>
    </OsirionTab>
    <OsirionTab Title="Package Manager">
        <pre><code>Install-Package Osirion.Blazor</code></pre>
    </OsirionTab>
</OsirionTabs>
```

The first tab is selected when the page renders. To start on another one and give the ids a stable prefix:

```razor
<OsirionTabs AriaLabel="Install command" Id="install" SelectedIndex="2">
    <OsirionTab Title=".NET CLI">
        <pre><code>dotnet add package Osirion.Blazor</code></pre>
    </OsirionTab>
    <OsirionTab Title="Package Manager">
        <pre><code>Install-Package Osirion.Blazor</code></pre>
    </OsirionTab>
    <OsirionTab Title="PackageReference">
        <pre><code>&lt;PackageReference Include="Osirion.Blazor" Version="4.0.0" /&gt;</code></pre>
    </OsirionTab>
</OsirionTabs>
```

With `Id="install"`, the third tab's radio button is `install-2`, its label `install-2-label` and its panel
`install-2-panel`.

## Parameters

### OsirionTabs

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `ChildContent` | `RenderFragment?` | `null` | The `OsirionTab` components. |
| `SelectedIndex` | `int` | `0` | Zero-based index of the tab selected when the page renders. |
| `Id` | `string?` | generated | Radio group name and prefix of every tab's ids. Without it, a unique value is generated per instance. |
| `AriaLabel` | `string?` | `null` | Accessible name of the tab group, for example "Package manager". |
| `Class` | `string?` | `null` | Extra classes on the container. |

Unmatched attributes go on the container `<div>`.

### OsirionTab

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Title` | `string` | `""` | The tab's label. Marked `[EditorRequired]`. |
| `ChildContent` | `RenderFragment?` | `null` | The panel content shown while the tab is selected. |
| `Class` | `string?` | `null` | Extra classes on the panel. |

Unmatched attributes go on the panel `<div>`.

## Behavior

- Each `OsirionTab` renders a radio button, its `<label>` and its panel. The radio buttons share one group name,
  and CSS shows the panel that follows the checked one. No script runs, in static SSR or in an interactive render
  mode.
- Keyboard: Tab moves focus into the group and the arrow keys move between tabs, which is the native radio button
  behavior. The radio button itself is visually hidden; its label shows a focus ring.
- Screen readers announce each tab as a radio button with its position ("2 of 3"). Each panel has `role="region"`
  and is labelled by its tab's label. The container has `role="group"` with `AriaLabel` as its name, so set
  `AriaLabel` to describe the choice.
- Tabs take their index in the order they first render. Ids stay the same when the parent renders again.
- Two `OsirionTabs` on one page get different generated group names. If you set `Id` yourself, use a different value
  for each instance.
- Printed pages show every panel under the tab row, because paper cannot switch tabs.
- An `OsirionTab` placed outside an `OsirionTabs` throws `InvalidOperationException`.

## Styling

| Class | Element |
| --- | --- |
| `osirion-tabs` | The container, a wrapping flex row |
| `osirion-tabs-input` | The visually hidden radio button |
| `osirion-tabs-label` | The visible tab |
| `osirion-tabs-panel` | A panel |

The selected tab's label is underlined in `--osirion-action-primary`. Labels use `--osirion-text-secondary`, and
`--osirion-text-primary` when selected or hovered; the line under the tab row is `--osirion-border-color`, and the
focus ring uses `--osirion-focus-ring-width` and `--osirion-focus-ring-color`. Every token has a fallback value, and
a CSS framework adapter that sets them restyles the tabs with the rest of the site. Color transitions are turned off
under `prefers-reduced-motion: reduce`.

## Related

- [Accordion and FAQ](/docs/components/core/content/accordion-and-faq)
- [Callouts](/docs/components/core/content/callout)
- [Content components](/docs/components/core/content)
