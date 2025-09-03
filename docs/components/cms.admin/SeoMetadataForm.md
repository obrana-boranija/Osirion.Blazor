# SeoMetadataForm

Purpose
Form for editing SEO-specific metadata (title, description, canonical, robots, social overrides).

Parameters
- `SeoData`: SeoMetadata - SEO metadata model
- `OnChanged`: EventCallback<SeoMetadata> - Fired when form changes

Example

```razor
<SeoMetadataForm SeoData="@seo" OnChanged="@HandleSeoChanged" />

@code {
    private SeoMetadata seo = new SeoMetadata();
    private void HandleSeoChanged(SeoMetadata s) { seo = s; }
}
```
