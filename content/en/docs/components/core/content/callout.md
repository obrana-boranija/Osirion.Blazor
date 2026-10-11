---
title: "Callouts - Osirion Blazor"
description: "Highlight a note, tip, important point, warning or caution in content, written in Markdown with GitHub's alert syntax or with the OsirionCallout component."
author: Dejan Demonjić
date: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Content
tags:
- callout
- alert
- markdown
- accessibility
slug: callout
published: true
---

# Callouts

A callout highlights a note, tip, important point, warning or caution inside content. Write it in Markdown with
GitHub's alert syntax, or render it from a Razor page with `OsirionCallout`. Both look the same, follow the theme and
the CSS framework adapter, and need no script.

## In Markdown

Start a blockquote with the kind in brackets:

```markdown
> [!NOTE]
> Content is read from the `content` folder in Development.
```

The five kinds render like this:

> [!NOTE]
> Useful information the reader should notice even when skimming.

> [!TIP]
> Optional advice that helps the reader do something better or more easily.

> [!IMPORTANT]
> Key information the reader needs to succeed.

> [!WARNING]
> Urgent information that needs the reader's attention to avoid problems.

> [!CAUTION]
> Advice about the risks or negative outcomes of an action.

## In Razor

```razor
<OsirionCallout Type="CalloutType.Warning" Title="Back up first">
    <p>Back up the database before you run the migration.</p>
</OsirionCallout>
```

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Type` | `CalloutType` | `Note` | `Note`, `Tip`, `Important`, `Warning` or `Caution`; sets the color, icon and default title. |
| `Title` | `string?` | the type's name | Title shown next to the icon; set it for other languages or a more specific heading. |
| `ChildContent` | `RenderFragment?` | `null` | The callout's content. |
| `Class` | `string?` | `null` | Extra CSS classes. |

## Accessibility

- The component renders `role="note"`, so assistive technology announces it as a note without interrupting the
  reader the way an alert role would.
- The title is text, not color: the color and icon only reinforce it.
- The title keeps the normal text color; the kind's color is used for the icon and border, so light colors such as
  amber never carry text.

## Styling

Both forms share the `--osirion-callout-color` and `--osirion-callout-icon` custom properties. Each kind takes its
color from the theme's status tokens (`--osirion-status-info`, `--osirion-status-success`, `--osirion-action-primary`,
`--osirion-status-warning`, `--osirion-status-danger`), so a framework adapter recolors callouts with the rest of the
site. Override a kind with its class, for example `.osirion-callout-tip, .markdown-alert-tip { --osirion-callout-color: #0e7490; }`.
