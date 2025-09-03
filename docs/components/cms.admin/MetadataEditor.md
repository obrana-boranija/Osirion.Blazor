# MetadataEditor

Purpose
Editor for content metadata fields (title, description, tags, SEO fields) used inside the content editor.

Parameters
- `Metadata`: ContentMetadata - Metadata model to edit
- `OnMetadataChanged`: EventCallback<ContentMetadata> - Fired when metadata changes

Example

```razor
<MetadataEditor Metadata="@metadata" OnMetadataChanged="@HandleMetadataChanged" />

@code {
    private ContentMetadata metadata = new ContentMetadata();
    private void HandleMetadataChanged(ContentMetadata m) { metadata = m; }
}
```
