# MarkdownEditor

Purpose
A plain Markdown text editor: a toolbar, an optional header and a monospace `textarea`. It edits a string and reports
every change through `ContentChanged`. `MarkdownEditorPreview` composes it next to a `MarkdownPreview`; use
`MarkdownEditor` alone when no preview is wanted.

Key parameters
- Content: `string`, default empty. The Markdown text. When the parent passes a different value, the textarea shows it.
- ContentChanged: `EventCallback<string>`, raised on every input event with the new text, when it differs from `Content`
  (use `@bind-Content` for two-way binding).
- Placeholder: `string`, default `"Enter markdown here..."`
- ShowToolbar: `bool`, default `true`
- ShowHeader: `bool`, default `true`. Shows an `h3` with `Title`.
- Title: `string`, default `"Editor"`
- AutoFocus: `bool`, default `false`. Focuses the textarea after the first render.
- SyncScroll: `bool`, default `true`. While `true` and `OnScroll` has a handler, scroll events are reported; while
  `false`, `SetScrollPositionAsync` does nothing.
- OnScroll: `EventCallback<double>`. Raised on scroll with the position as a fraction from 0 to 1.
- ToolbarActions: `List<ToolbarAction>?`. When null, six default actions are used (Heading, Bold, Italic, Link,
  Bullet List, Code Block).
- Class: `string?`, extra CSS classes on the root element (from `OsirionComponentBase`). The component also inherits
  `Style`, `Theme` and `Attributes` from that base; the markup does not render them.

Usage
```razor
@using Osirion.Blazor.Cms

<MarkdownEditor @bind-Content="markdown"
                Title="Post body"
                Placeholder="Write in Markdown"
                ToolbarActions="actions"
                OnScroll="HandleScroll" />

@code {
    private string markdown = "# Hello";

    private readonly List<ToolbarAction> actions =
    [
        new("B", ToolbarActionType.Wrap, "**|**|bold text", "Bold"),
        new("List", ToolbarActionType.Insert, "\n- Item 1\n- Item 2", "Bullet List"),
    ];

    private void HandleScroll(double position)
    {
        // position is 0 at the top and 1 at the bottom
    }
}
```

Notes
- Namespace `Osirion.Blazor.Cms`. Not routable.
- Renders `div.osirion-markdown-editor` containing, in order: `div.osirion-markdown-editor-toolbar` with one
  `button.osirion-markdown-toolbar-button` per action (the icon HTML when `Icon` is set, otherwise the label), then
  `div.osirion-markdown-editor-header` with `h3.osirion-markdown-editor-title`, then
  `div.osirion-markdown-editor-content` with `textarea.osirion-markdown-editor-textarea`. The textarea binds on
  `oninput`.
- `ToolbarAction(label, actionType, action, title, icon = null)` has a `Label`, an `ActionType`, an `Action` string, a
  `Title` (the button tooltip) and an optional `Icon` that is rendered as raw HTML (`MarkupString`), so it must come
  from trusted code, not from user input.
- `ToolbarActionType.Insert` inserts the `Action` text at the cursor. `Wrap` reads `Action` as
  `prefix|suffix|defaultText` (at least three parts) and wraps the selection, or `defaultText` when nothing is
  selected. `Custom` does nothing in the editor; it is left to the consumer.
- The Tab key inserts a tab character, or indents and outdents (Shift) a multi-line selection.
- Public methods: `FocusAsync()`, `InsertTextAsync(string text)`, `WrapTextAsync(prefix, suffix, defaultText)` and
  `SetScrollPositionAsync(double position)`. When the JavaScript call fails, insert and wrap fall back to appending
  the text to the end of the content.
- Needs an interactive render mode: the binding, toolbar, Tab handling and scroll reporting use Blazor events, and the
  insert, wrap, focus, Tab and scroll code runs through `IJSRuntime` with `eval`. The component injects `IJSRuntime`.
- Styles are in `MarkdownEditor.razor.css` and use `--osirion-*` CSS variables with fallback values; the root shows a
  focus ring through `:focus-within`.
