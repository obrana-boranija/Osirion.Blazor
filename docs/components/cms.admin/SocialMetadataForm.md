# SocialMetadataForm

Purpose
Form for editing social media metadata (OpenGraph, Twitter card overrides).

Parameters
- `Metadata`: SocialMetadata - Social metadata model
- `OnChanged`: EventCallback<SocialMetadata> - Fired when form changes

Example

```razor
<SocialMetadataForm Metadata="@social" OnChanged="@HandleSocialChanged" />

@code {
    private SocialMetadata social = new SocialMetadata();
    private void HandleSocialChanged(SocialMetadata s) { social = s; }
}
```
