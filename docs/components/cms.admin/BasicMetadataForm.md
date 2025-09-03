# BasicMetadataForm

Purpose
A compact metadata form for title, description and primary fields used in simple pages.

Parameters
- `Metadata`: BasicMetadata - Basic fields model
- `OnChanged`: EventCallback<BasicMetadata> - Fired when metadata changes

Example

```razor
<BasicMetadataForm Metadata="@meta" OnChanged="@HandleMetaChanged" />

@code {
    private BasicMetadata meta = new BasicMetadata();
    private void HandleMetaChanged(BasicMetadata m) { meta = m; }
}
```
