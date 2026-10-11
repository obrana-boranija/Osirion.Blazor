# OsirionCallout

Purpose
Highlights a note, tip, important point, warning or caution inside content. Shares its look with the GitHub-style
alerts (`> [!NOTE]` and the others) that Markdown content renders, and needs no script.

Key parameters
- Type: `CalloutType.Note` (default), `Tip`, `Important`, `Warning` or `Caution`
- Title: defaults to the type's name ("Note", "Warning" and so on)
- ChildContent: the callout's content
- Class: extra CSS classes

Usage
```razor
<OsirionCallout Type="CalloutType.Warning" Title="Back up first">
    <p>Back up the database before you run the migration.</p>
</OsirionCallout>
```

Notes
- Emits `div.osirion-callout.osirion-callout-{type}` with `role="note"`, a `p.osirion-callout-title` and a
  `div.osirion-callout-body`.
- Styles live in the Core stylesheet (`css/utilities/callouts.scss`), not in scoped CSS, because the same rules style
  Markdown alerts (`div.markdown-alert.markdown-alert-{type}`), which scoped CSS cannot reach.
- Icons are CSS masks (GitHub Octicons, MIT License), so the HTML sanitizer can keep removing inline SVG from content.
- Colors come from `--osirion-status-info`, `--osirion-status-success`, `--osirion-action-primary`,
  `--osirion-status-warning` and `--osirion-status-danger`; override `--osirion-callout-color` per kind if needed.
