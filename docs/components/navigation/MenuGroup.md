# MenuGroup

Purpose
Groups related menu items under an optional label. In a mega submenu a group is a column with a title, an optional
description, its items and an optional footer link; in other menus it is a labelled section of a list.

Key parameters
- Label: the group label; nothing is rendered for it when blank
- Description: supporting text rendered below the label (used by mega menus to describe the column)
- FooterText: text of the footer link
- FooterHref: destination of the footer link. The footer link renders only when `FooterText` is not blank and
  `FooterHref` passes the link check (relative URLs and the `http`, `https`, `mailto` and `tel` schemes are allowed)
- FooterTarget: how to open the footer link, for example `_blank`
- Collapsible: makes the label a toggle for the items (default `false`); it needs a `Label`
- Expanded: default `true`; whether a collapsible group starts open, and only has an effect when `Collapsible` is `true`
- ChildContent: the items of the group, typically `MenuItem`
- Id: base identifier for the group (default: `osirion-menu-group-` plus a GUID generated once per instance)
- Class: extra CSS classes on the root element (inherited from `OsirionComponentBase`)
- Style: inherited from `OsirionComponentBase`, written on the root element
- Theme: `ThemeMode.System` (default), `Light` or `Dark`; a non-System value is written as `data-theme` (or
  `data-bs-theme` when the Bootstrap framework is configured) on the root element (inherited)
- Attributes: any other attribute, applied to the root element (inherited). An `id` supplied here wins over `Id`

Usage
```razor
<Menu BrandText="Contoso">
    <MenuItem Text="Platform" HasSubmenu SubmenuVariant="SubmenuVariant.Mega">
        <MenuGroup Label="Core platform"
                   Description="Measure and automate."
                   FooterText="Explore the platform"
                   FooterHref="/platform">
            <MenuItem Text="Tracking" Description="Measurement down to ad level." Href="/platform/tracking" />
            <MenuItem Text="Automation" Description="Protect margins as you scale." Href="/platform/automation" />
        </MenuGroup>
        <MenuGroup Label="Infrastructure">
            <MenuItem Text="Data and APIs" Href="/platform/data" />
        </MenuGroup>
    </MenuItem>
</Menu>
```

Notes
- Emits `div.osirion-menu-group` with an `id` (the `Id` value or the generated one). Added classes:
  `osirion-menu-group-collapsible` when `Collapsible`, `osirion-menu-group-expanded` when `Collapsible` and `Expanded`,
  then `Class`.
- Label: `div.osirion-menu-group-label` with `id="{id}-label"` and `span.osirion-menu-group-text`. When `Collapsible`
  with a `Label`, the group is wrapped in `details.osirion-menu-group-disclosure` (with `open` when `Expanded`), the label
  is its `summary.osirion-menu-group-label`, and it also holds `span.osirion-menu-group-toggle` (an `aria-hidden`
  chevron SVG).
- Description: `p.osirion-menu-group-description`.
- Items: `div.osirion-menu-group-items` with `id="{id}-items"` and `role="group"`. It carries
  `aria-labelledby="{id}-label"` only when `Label` is not blank. The generated ids stay the same across renders.
- Footer: `a.osirion-menu-group-footer` with `span.osirion-menu-group-footer-text` and
  `span.osirion-menu-group-footer-arrow` (`aria-hidden`). `rel="noopener noreferrer"` is added when `FooterTarget` is
  exactly `_blank`.
- A collapsible group needs no script and no click handler: the `summary` is focusable, Enter or Space toggles it, and
  the browser hides the items while the `details` is closed. Without a `Label` there is nothing to toggle, so the items
  always show.
- In a vertical menu, and in the drawer below the collapse breakpoint, the label becomes a small uppercase heading
  and the description is hidden (scoped CSS).
- Renders in static SSR and needs no script.
