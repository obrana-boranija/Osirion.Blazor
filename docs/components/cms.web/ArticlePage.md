Purpose
-------
Page-level component used to render article detail pages. Composes metadata, hero, content, and optional sidebar sections.

Parameters
----------
- Many parameters are forwarded from `OsirionContentDetailPageBase` such as `Item`, `IsLoading`, `ShowHero`, `ShowSidebar`, `ShowBreadcrumbs`, and more.

Example
-------
```razor
<ArticlePage Item="@article" ShowSidebar="true" />
```

Notes
-----
- See `src/Osirion.Blazor.Cms.Web/Components/PageLevel/ArticlePage.razor` for the full parameter list and behaviour.

- The component supports `ShowRelatedContent`, `ShowNavigation`, and slotting of `SidebarContent` for related widgets.
