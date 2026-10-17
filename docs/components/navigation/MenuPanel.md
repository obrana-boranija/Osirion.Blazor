# MenuPanel

Purpose
Renders a high-contrast promotional panel inside a mega menu, typically as the aside for featured links such as
"Platform highlights" or "Build your business case". It works in static SSR and needs no script.

Key parameters
- Title: the panel heading
- Description: optional supporting text below the heading
- AriaLabel: the accessible name; falls back to `Title` when not set
- ChildContent: the panel content, typically `MenuPanelLink` items
- Class: extra CSS classes, added after `osirion-menu-panel` (inherited from `OsirionComponentBase`)
- Style: inherited from `OsirionComponentBase`, written on the root element
- Theme: `ThemeMode.System` (default), `Light` or `Dark`; a non-System value is written as `data-theme` (or
  `data-bs-theme` when the Bootstrap framework is configured) on the panel (inherited)
- Attributes: any other attribute, captured and applied to the panel (inherited)

Usage
```razor
<Menu BrandText="Contoso">
    <MenuItem Text="Solutions" HasSubmenu SubmenuVariant="SubmenuVariant.Mega">
        <ChildContent>
            <MenuGroup Label="By team">
                <MenuItem Text="Finance" Href="/solutions/finance" />
            </MenuGroup>
        </ChildContent>
        <SubmenuAside>
            <MenuPanel Title="Build your business case" Description="Hand-picked guides and tools.">
                <MenuPanelLink Text="ROI assessment" Href="/roi" />
                <MenuPanelLink Text="API documentation" Href="https://docs.example.com" Target="_blank" />
            </MenuPanel>
        </SubmenuAside>
    </MenuItem>
</Menu>
```

Notes
- Emits `aside.osirion-menu-panel` with `aria-label` set to `AriaLabel`, or `Title` when `AriaLabel` is null. Inside it:
  `div.osirion-menu-panel-title` (only when `Title` is not blank), `p.osirion-menu-panel-description` (only when
  `Description` is not blank) and `div.osirion-menu-panel-links` holding `ChildContent`.
- `MenuItem` renders its `SubmenuAside` fragment in `div.osirion-submenu-mega-aside`, and only when the item's
  `SubmenuVariant` is `Mega`.
- The panel is a flex column with `height: 100%`. Its background and text color come from
  `--osirion-menu-panel-background` (fallback `--osirion-background-inverted`) and `--osirion-menu-panel-color`
  (fallback `--osirion-text-inverted`), so it stays a contrast panel in light and dark themes unless a site overrides
  them.
- Pure markup and CSS: no interactivity, no JavaScript.
