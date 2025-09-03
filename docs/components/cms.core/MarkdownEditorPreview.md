Purpose
-------
Combined Markdown editor and preview layout used in the CMS admin UI. Shows an editor and a synchronized preview pane; optionally shows the preview area collapsed.

Parameters
----------
- `Content` (string) — The Markdown text to edit. (Two-way binding supported via `ContentChanged` events.)
- `Placeholder` (string) — Placeholder text shown when content is empty.
- `ShowPreview` (bool) — When true, the preview pane is visible. Default: true.
- `ShowActionsBar` (bool) — Show the toggle and other action buttons. Default: true.
- `AutoFocus` (bool) — Whether the editor should autofocus on load.
- `SyncScroll` (bool) — Enable synchronized scrolling between editor and preview.
- `ToolbarActions` (IEnumerable&lt;ToolbarAction&gt;) — Custom toolbar actions for the editor.

Example
-------
```razor
<MarkdownEditorPreview Content="@myContent" Placeholder="Start writing..." ShowPreview="true" />
```

Notes
-----
- The component composes a `MarkdownEditor` and `MarkdownPreview` and forwards many settings to both.
- For full behavior, review `src/Osirion.Blazor.Cms.Core/Components/Editor/MarkdownEditorPreview.razor`.
