# Breadcrumb

Purpose
Admin breadcrumb component used inside admin layouts to show location context.

Parameters
- `Segments`: IEnumerable<BreadcrumbSegment> - Breadcrumb segments
- `ShowHome`: bool - Whether to include a home link

Example

```razor
<Breadcrumb Segments="@segments" ShowHome="true" />

@code {
    private IEnumerable<BreadcrumbSegment> segments = new[] {
        new BreadcrumbSegment("Admin", "/admin"),
        new BreadcrumbSegment("Content", "/admin/content")
    };
}
```
