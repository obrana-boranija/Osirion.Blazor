# MetadataPreview

Purpose
Preview of content metadata with optional compact/full modes used before committing or publishing.

Parameters
- `Metadata`: ContentMetadata - Metadata to preview
- `ShowAll`: bool - Show all metadata fields (default: false)

Example

```razor
<MetadataPreview Metadata="@metadata" ShowAll="true" />

@code {
    private ContentMetadata metadata = new ContentMetadata { Title = "Post" };
}
```
