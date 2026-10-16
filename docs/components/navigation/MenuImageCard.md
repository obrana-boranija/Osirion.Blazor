# MenuImageCard

Purpose
Renders a linked image card with an overlay caption inside a mega menu. It is typically used as the aside of a mega
submenu to feature customer stories or news. It works in static SSR and needs no script.

Key parameters
- ImageUrl: the image URL (default: empty string)
- ImageAlt: the image alternative text (default: `null`, which leaves the `alt` attribute off)
- Title: the caption title (default: empty string)
- Description: optional caption supporting text
- Href: destination URL (default: `"#"`). A value whose scheme is not `http`, `https`, `mailto` or `tel` (for example
  `javascript:`) is not rendered, so the anchor has no `href`. Relative URLs are kept as written
- Target: how to open the destination, for example `_blank`
- Class: extra CSS classes, added after `osirion-menu-image-card` (inherited from `OsirionComponentBase`)
- Style: inherited from `OsirionComponentBase`; this component's markup does not render it
- Theme: `ThemeMode.System` (default), `Light` or `Dark`; a non-System value is written as `data-theme` (or
  `data-bs-theme` when the Bootstrap framework is configured) on the anchor (inherited)
- Attributes: any other attribute, captured and applied to the anchor (inherited)

Usage
```razor
<Menu BrandText="Contoso">
    <MenuItem Text="Customers" HasSubmenu SubmenuVariant="SubmenuVariant.Mega">
        <ChildContent>
            <MenuGroup Label="Stories">
                <MenuItem Text="All stories" Href="/customers" />
            </MenuGroup>
        </ChildContent>
        <SubmenuAside>
            <MenuImageCard ImageUrl="/images/customer.jpg"
                           ImageAlt="A customer team at work"
                           Title="Customer stories"
                           Description="Trusted by teams across Europe"
                           Href="/customers" />
        </SubmenuAside>
    </MenuItem>
</Menu>
```

Notes
- Emits `a.osirion-menu-image-card` containing `img.osirion-menu-image-card-image` (`loading="lazy"`),
  `span.osirion-menu-image-card-scrim` (`aria-hidden="true"`) and `span.osirion-menu-image-card-caption` with
  `span.osirion-menu-image-card-title` and, when `Description` is not blank, `span.osirion-menu-image-card-description`.
  There is no `role` attribute.
- `rel="noopener noreferrer"` is added when `Target` is exactly `_blank`.
- The caption sits on a dark gradient scrim over the image with white text, so contrast does not depend on the theme.
  The card has a minimum height of `12rem`; the image is `object-fit: cover`.
- `MenuItem` renders its `SubmenuAside` fragment in `div.osirion-submenu-mega-aside`, and only when the item's
  `SubmenuVariant` is `Mega`.
- Hover and keyboard focus scale the image slightly; this is switched off under `prefers-reduced-motion: reduce`.
