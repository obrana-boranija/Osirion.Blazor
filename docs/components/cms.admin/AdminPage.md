# AdminPage

Purpose
Page wrapper for admin pages that provides consistent header, navigation, and content areas.

Parameters
- `Title`: string - Page title
- `ShowSidebar`: bool - Show sidebar (default: true)
- `SidebarContent`: RenderFragment - Sidebar content

Example

```razor
<AdminPage Title="Content Management">
    <SidebarContent>
        <nav>Admin menu</nav>
    </SidebarContent>

    <p>Main admin page content goes here.</p>
</AdminPage>
```
