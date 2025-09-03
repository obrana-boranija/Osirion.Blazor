Purpose
-------
Layout used for documentation pages. Renders a two-column layout with a navigational tree in the sidebar and the documentation content in the main area.

Parameters
----------
- `Title` (string) — Page title.
- `Description` (string) — Meta description.
- `ShowHeader` (bool) — Render the site header. Default: true.
- `HeaderTemplate` (RenderFragment) — Custom header content.
- `SidebarContent` (RenderFragment) — Content for the documentation sidebar (usually a `DocumentTree`).
- `MainContent` (RenderFragment) — The documentation content.
- `DocTreeTitle` (string) — Sidebar title. Default: "Documentation".
- `SidebarOnRight` (bool) — Place sidebar on right. Default: false.

Example
-------
```razor
<DocumentationLayout Title="API docs">
    <SidebarContent>
        <DocumentTree Sections="@docSections" />
    </SidebarContent>
    <MainContent>
        <article>...doc content...</article>
    </MainContent>
</DocumentationLayout>
```

Notes
-----
- Ideal for documentation collections and long-form content with a table-of-contents.
