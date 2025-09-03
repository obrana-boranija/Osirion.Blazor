# DefaultNavigation

Purpose
Default admin navigation used in the CMS admin layout.

Parameters
- `MenuItems`: IEnumerable<MenuItem> - Menu items to display
- `CurrentPath`: string - Current path to highlight
- `Collapsed`: bool - Navigation collapsed state

Example

```razor
<DefaultNavigation MenuItems="@menuItems" CurrentPath="/content" />

@code {
    private IEnumerable<MenuItem> menuItems = new[] {
        new MenuItem("Dashboard", "/admin"),
        new MenuItem("Content", "/admin/content"),
        new MenuItem("Settings", "/admin/settings")
    };
}
```
