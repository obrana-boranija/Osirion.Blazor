---
id: 'osirion-html-renderer'
order: 8
layout: "component"
title: OsirionHtmlRenderer
permalink: /components/html-renderer
description: OsirionHtmlRenderer sanitizes and renders HTML in Blazor, with Prism syntax highlighting loaded with SRI, line numbers and copy buttons.
author: "Osirion Team"
date: 2025-01-26
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Rendering
tags:
- html
- sanitization
- syntax-highlighting
- prism
- security
published: true
slug: html-renderer
lang: en
redirect_from:
- /docs/components/core/rendering/html-renderer
custom_fields:
  group: content
seo_properties:
  title: 'OsirionHtmlRenderer - Osirion.Blazor'
  description: 'Render sanitized HTML in Blazor with optional Prism syntax highlighting loaded with Subresource Integrity, line numbers and copy buttons.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/html-renderer'
  lang: en
  robots: index, follow
  og_title: 'OsirionHtmlRenderer - Osirion.Blazor'
  og_description: 'Sanitized HTML rendering for Blazor with optional syntax highlighting for code blocks.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionHtmlRenderer - Osirion.Blazor'
  twitter_description: 'Sanitized HTML rendering for Blazor with optional syntax highlighting for code blocks.'
  twitter_image_url: /assets/images/blazor-cover.png
---

# OsirionHtmlRenderer

`OsirionHtmlRenderer` sanitizes an HTML string and writes it into the page. When the HTML contains code blocks, it
also loads Prism to highlight them, with optional line numbers and copy buttons. Use it for HTML you hold outside
the CMS, such as Markdown you converted yourself or a field from an API.

## Usage

```razor
<OsirionHtmlRenderer HtmlContent="@html" />

@code {
    private string html = "<h2>Release notes</h2><p>Version 4.0 sanitizes content by default.</p>";
}
```

Code blocks are `<pre><code class="language-...">` elements, the form Markdown processors produce. With line numbers
and copy buttons:

```razor
<OsirionHtmlRenderer HtmlContent="@articleHtml"
                     ShowLineNumbers="true"
                     EnableCopyButton="true" />

@code {
    private string articleHtml =
        """
        <p>Register the services:</p>
        <pre><code class="language-csharp">builder.Services.AddOsirion(builder.Configuration);</code></pre>
        """;
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `HtmlContent` | `string?` | `null` | The HTML to render. When empty, the component renders nothing. |
| `SanitizeHtml` | `bool` | `true` | Sanitizes the HTML before it is rendered. Turn it off only for markup your application produced itself. |
| `HtmlSanitizer` | `Func<string, string>?` | `null` | Your own sanitizer, used instead of the registered `IHtmlContentSanitizer` while `SanitizeHtml` is on. |
| `EnableSyntaxHighlighting` | `bool` | `true` | Loads Prism and highlights code blocks. When `false`, line numbers and copy buttons are not added either. |
| `ShowLineNumbers` | `bool` | `false` | Adds Prism's line numbers to every code block. |
| `EnableCopyButton` | `bool` | `false` | Wraps each code block with a header showing the language and a Copy button. |
| `EnableLineHighlighting` | `bool` | `false` | Written to the page as `data-enable-line-highlighting`. The bundled script does not read it: line numbers are clickable whenever `ShowLineNumbers` is on. |
| `UseAccessibleTheme` | `bool` | `true` | Uses the built-in high-contrast dark theme. When `false`, Prism's Okaidia theme is loaded instead. |
| `PrismBaseUrl` | `string?` | `null` | Loads Prism from your own address instead of cdnjs. Available since 4.0. |
| `Class` | `string?` | `null` | Extra CSS classes on the `<article>`. |

`LanguageClasses` is obsolete: the component never read it. Unmatched attributes are written to the `<article>`.

## Behavior

### Sanitizing

With the default `SanitizeHtml="true"`, the HTML goes through the registered `IHtmlContentSanitizer`, or the
built-in allow-list when none is registered. Script, event handler attributes, `javascript:`, `vbscript:` and
`data:` URLs, forms, iframes, inline styles, `<style>` and SVG are removed; `class`, `id`, `role` and `aria-*` are
kept, so `language-*` classes survive. `AddOsirion` registers the sanitizer; to change the allow-list, call
`AddOsirionHtmlSanitizer` with your changes:

```csharp
using Osirion.Blazor.Core.Extensions;

builder.Services.AddOsirionHtmlSanitizer(options =>
    options.AllowElement("iframe", "src", "title", "allow", "allowfullscreen"));
```

Sanitizing runs before the copy button wrapper is added, so the button's own handler is not stripped.

### Syntax highlighting

Highlighting loads only when the HTML contains a code block. The component then requests Prism 1.29.0 from cdnjs,
plus one language file for each language it finds, and every file carries its Subresource Integrity hash. A language
that is not in that release is skipped. Short names are mapped to Prism's: `cs` to `csharp`, `js` to `javascript`,
`ts`, `py`, `rb`, `yml`, `sh`, `ps1`, and `razor` or `blazor` to `cshtml`; `html` and `xml` use Prism's built-in
`markup`.

To serve Prism yourself, set `PrismBaseUrl`, for example `/lib/prism/1.29.0/`. Keep the release layout
(`prism.min.js`, `components/prism-{language}.min.js`, `themes/prism-okaidia.min.css`, `plugins/line-numbers/...`).
Files from your own address load without an integrity check.

The scripts are rendered as part of the page rather than called through JavaScript interop, so highlighting also
works under static SSR, and it runs again after enhanced navigation. Without JavaScript, code blocks show as plain
preformatted text.

### Copy button and line numbers

- The Copy button copies the code's text with the Clipboard API, or an older fallback outside secure contexts, and
  briefly reads "Copied!". Its accessible name includes the language, for example "Copy C# code to clipboard".
- With line numbers on, each number becomes a toggle button (Enter or Space with the keyboard). Clicking one
  highlights that line; Ctrl, Cmd or Shift adds lines to the selection, and a polite live region announces the change.

### Content Security Policy

The accessible theme is an inline `<style>` element and the Copy button uses an inline `onclick` attribute. Under a
policy without `'unsafe-inline'`, set `UseAccessibleTheme="false"` (and provide your theme as a file) and leave
`EnableCopyButton` off.

## Styling

The output is wrapped in `<article class="osirion-html-renderer osirion-content">`, with `osirion-wcag-theme` added
when the accessible theme is in use. The component also links
`_content/Osirion.Blazor.Core/css/osirion-html-renderer.min.css`, which styles headings, lists, tables, links and code
inside the article from the theme tokens (`--osirion-text-primary`, `--osirion-font-family-base`,
`--osirion-font-family-mono` and others).

Two custom properties are yours to set:

- `--osirion-syntax-background-color`: background of the copy button wrapper, its header and the code block inside
  it.
- `--osirion-code-background`: background of inline `<code>`.

The copy button wrapper uses `osirion-code-wrapper`, `osirion-code-header`, `osirion-language-label` and
`osirion-copy-button` (with `copied` after a copy); the button also takes the CSS framework's button class. A
highlighted line has `osirion-line-highlighted`.

## Related

- [Core rendering components](/components/content)
- [Callouts](/components/callout)
- [CMS Web components](/docs/packages/cms-web)
