Purpose
-------
Render a documentation navigation tree composed of sections and nested items. Used inside documentation sidebars.

Parameters
----------
- `Sections` (IEnumerable&lt;DocSection&gt;) — Sections containing `Title` and `Items` to render.

Example
-------
```razor
<DocumentTree Sections="@sections" />
```

Notes
-----
- Items can have `Children` for nested navigation.
- The component applies an `active` class to the current page item.
