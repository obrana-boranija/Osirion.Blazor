Purpose
-------
Render HTML produced from Markdown input. Used by preview panes and read-only content views.

Parameters
----------
- `Markdown` (string) — The markdown source to render.
- `Title` (string) — Optional header title shown above the preview.
- `ShowHeader` (bool) — Render the header. Default: true.
- `Placeholder` (string) — Displayed when the Markdown input is empty.
- `ContentCssClass` (string) — CSS class applied to the rendered content container.
- `Pipeline` (string) — Optional pipeline name to customize rendering.

Example
-------
```razor
<MarkdownPreview Markdown="@myMarkdown" Title="Preview" />
```

Notes
-----
- The rendered HTML is sanitized with the registered IHtmlContentSanitizer (or the default allow-list), including
  output from a custom `Pipeline`.
- The markdown is rendered and sanitized again only when `Markdown` or `Pipeline` changes, not on every render
  (for example after a scroll event).
- When rendering fails the preview shows a fixed error message and logs the exception through
  `ILogger<MarkdownPreview>`; the exception message is not shown in the page.
- The component exposes scroll events to enable sync with editors.
- See `src/Osirion.Blazor.Cms.Core/Components/Editor/MarkdownPreview.razor` for implementation details.
