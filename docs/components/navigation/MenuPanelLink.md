# MenuPanelLink

Purpose
Renders a pill-style featured link inside a `MenuPanel`. It works in static SSR and needs no script.

Key parameters
- Text: the visible link text (default: empty string)
- Href: destination URL (default: `"#"`). A value whose scheme is not `http`, `https`, `mailto` or `tel` (for example
  `javascript:`) is not rendered, so the anchor has no `href`. Relative URLs are kept as written
- Target: how to open the destination, for example `_blank`
- IconTemplate: optional leading icon, a `RenderFragment`
- Class: extra CSS classes, added after `osirion-menu-panel-link` (inherited from `OsirionComponentBase`)
- Style: inherited from `OsirionComponentBase`, written on the root element
- Theme: `ThemeMode.System` (default), `Light` or `Dark`; a non-System value is written as `data-theme` (or
  `data-bs-theme` when the Bootstrap framework is configured) on the anchor (inherited)
- Attributes: any other attribute, captured and applied to the anchor (inherited)

Usage
```razor
<MenuPanel Title="Build your business case">
    <MenuPanelLink Text="ROI assessment" Href="/roi">
        <IconTemplate>
            <i class="bi bi-graph-up"></i>
        </IconTemplate>
    </MenuPanelLink>
    <MenuPanelLink Text="API documentation" Href="https://docs.example.com" Target="_blank" />
</MenuPanel>
```

Notes
- Emits `a.osirion-menu-panel-link` containing, in order: `span.osirion-menu-panel-link-icon` (only when
  `IconTemplate` is set, with `aria-hidden="true"`), `span.osirion-menu-panel-link-text` and
  `span.osirion-menu-panel-link-arrow` (`aria-hidden="true"`). There is no `role` attribute.
- `rel="noopener noreferrer"` is added when `Target` is exactly `_blank`.
- The link inherits the text color of its container (`color: inherit`) and derives its background and border from
  `currentColor`, so it follows the colors of the `MenuPanel` it sits in.
- The arrow is hidden until the link is hovered or keyboard-focused, then fades in; the motion is switched off under
  `prefers-reduced-motion: reduce`.
- The component does not check where it is placed. The code describes it as a link for use inside `MenuPanel`.
