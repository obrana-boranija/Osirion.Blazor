# MenuCallToAction

Purpose
Renders a prominent link at the end of a menu or mega menu: a bordered row with the link text, optional supporting
text and a trailing arrow. It works in static SSR and needs no script.

Key parameters
- Text: the visible link text (default: empty string)
- Description: optional supporting text under the link text
- Href: destination URL (default: `"#"`). A value whose scheme is not `http`, `https`, `mailto` or `tel` (for example
  `javascript:`) is not rendered, so the anchor has no `href`. Relative URLs are kept as written
- Target: how to open the destination, for example `_blank`
- Class: extra CSS classes, added after `osirion-menu-call-to-action` (inherited from `OsirionComponentBase`)
- Style: inherited from `OsirionComponentBase`, written on the root element
- Theme: `ThemeMode.System` (default), `Light` or `Dark`; a non-System value is written as `data-theme` (or
  `data-bs-theme` when the Bootstrap framework is configured) on the anchor (inherited)
- Attributes: any other attribute, captured and applied to the anchor (inherited)

Usage
```razor
<Menu BrandText="Contoso">
    <MenuItem Text="Platform" HasSubmenu SubmenuVariant="SubmenuVariant.Mega">
        <MenuGroup Label="Core platform">
            <MenuItem Text="Tracking" Href="/platform/tracking" />
        </MenuGroup>
        <MenuCallToAction Text="Platform overview"
                          Description="Everything in one tour."
                          Href="/platform" />
    </MenuItem>
</Menu>
```

Notes
- Emits `a.osirion-menu-call-to-action` containing `span.osirion-menu-call-to-action-content` (with
  `span.osirion-menu-call-to-action-text` and, when `Description` is not blank,
  `span.osirion-menu-call-to-action-description`)
  and `span.osirion-menu-call-to-action-arrow` with `aria-hidden="true"`. There is no `role` attribute.
- `rel="noopener noreferrer"` is added when `Target` is exactly `_blank`.
- Inside a mega submenu (`.osirion-submenu-mega-content`) the scoped CSS makes it span every grid column, below the
  groups.
- Colors come from `--osirion-menu-color`, `--osirion-menu-hover-background`, `--osirion-menu-border-color`,
  `--osirion-menu-accent` and `--osirion-menu-muted-color`, each with a theme fallback.
- Pure markup and CSS: no interactivity, no JavaScript. The arrow motion is switched off under
  `prefers-reduced-motion: reduce`.
