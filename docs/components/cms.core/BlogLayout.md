Purpose
-------
Wrapper layout for blog article pages. Provides featured image, title, content area and optional footer content.

Parameters
----------
- `Title` (string) — Page title.
- `Description` (string) — Short description or meta description.
- `ImageUrl` (string) — Featured image URL.
- `ImageAlt` (string) — Alt text for the image.
- `ImageCaption` (string) — Optional caption displayed under the image.
- `ShowHeader` (bool) — Whether to render the site header. Default: true.
- `HeaderTemplate` (RenderFragment) — Custom header fragment.
- `ChildContent` (RenderFragment) — Main article content.
- `FooterContent` (RenderFragment) — Optional footer fragment.

Example
-------
```razor
<BlogLayout Title="My post" ImageUrl="/images/post.jpg">
    <p>Article content here</p>
</BlogLayout>
```

Notes
-----
- The component composes a `PageLayout` and should be used for single-article pages.
