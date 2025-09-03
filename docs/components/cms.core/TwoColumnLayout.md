Purpose
-------
Simple two-column layout used by documentation pages: a sidebar and a main content area.

Parameters
----------
- `SidebarContent` (RenderFragment) — Content rendered in the sidebar.
- `MainContent` (RenderFragment) — Content rendered in the main area.
- `SidebarOnRight` (bool) — When true, sidebar is displayed on the right. Default: false.
- `StickyNavigation` (bool) — When true, sidebar navigation is sticky. Default: true.

Example
-------
```razor
<TwoColumnLayout>
    <SidebarContent>...nav...</SidebarContent>
    <MainContent>...content...</MainContent>
</TwoColumnLayout>
```

Notes
-----
- Adds utility CSS classes based on parameters; pass `Class` to append additional CSS.
