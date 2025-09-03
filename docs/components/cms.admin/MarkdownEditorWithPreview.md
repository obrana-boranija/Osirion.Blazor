# MarkdownEditorWithPreview

Purpose
A markdown editor with live preview used in the admin content editor.

Parameters
- `Content`: string - Markdown content
- `OnContentChanged`: EventCallback<string> - Fired when content changes
- `Height`: string - Editor height (default: "400px")
- `ShowToolbar`: bool - Show formatting toolbar (default: true)

Example

```razor
<MarkdownEditorWithPreview Content="@markdown" OnContentChanged="@HandleChange" Height="500px" />

@code {
    private string markdown = "# Hello";
    private void HandleChange(string content) { markdown = content; }
}
```
