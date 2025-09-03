# TagInput

Purpose
Reusable tag input component with suggestions and free-form tags.

Parameters
- `Tags`: IEnumerable<string> - Current tags
- `Suggestions`: IEnumerable<string> - Optional tag suggestions
- `OnTagsChanged`: EventCallback<IEnumerable<string>> - Called when tags change

Example

```razor
<TagInput Tags="@tags" Suggestions="@suggestions" OnTagsChanged="@HandleTagsChanged" />

@code {
    private IEnumerable<string> tags = new[] { "news", "blazor" };
    private IEnumerable<string> suggestions = new[] { "news", "tutorial", "release" };
    private void HandleTagsChanged(IEnumerable<string> t) { tags = t; }
}
```
