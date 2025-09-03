# CmsLayoutEditor

Purpose
Editor for composing CMS page layouts in the admin UI. Allows arranging components and saving layout definitions.

Parameters
- `Layout`: PageLayout - Current page layout model
- `OnLayoutChanged`: EventCallback<PageLayout> - Fired when layout is modified
- `AvailableComponents`: IEnumerable<ComponentInfo> - Components available to place in layout

Example

```razor
<CmsLayoutEditor Layout="@layout" AvailableComponents="@components" OnLayoutChanged="@HandleLayoutChanged" />

@code{
    private PageLayout layout = new PageLayout();
    private IEnumerable<ComponentInfo> components = new[] { new ComponentInfo("Hero"), new ComponentInfo("ContentList") };

    private void HandleLayoutChanged(PageLayout updated)
    {
        layout = updated;
        // persist layout
    }
}
```
