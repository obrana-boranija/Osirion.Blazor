# AdvancedMetadataForm

Purpose
Full metadata editor including SEO, social, structured data and custom fields.

Parameters
- `Metadata`: AdvancedMetadata - Full metadata model
- `OnChanged`: EventCallback<AdvancedMetadata> - Fired when metadata changes

Example

```razor
<AdvancedMetadataForm Metadata="@adv" OnChanged="@HandleAdvChanged" />

@code {
    private AdvancedMetadata adv = new AdvancedMetadata();
    private void HandleAdvChanged(AdvancedMetadata a) { adv = a; }
}
```
