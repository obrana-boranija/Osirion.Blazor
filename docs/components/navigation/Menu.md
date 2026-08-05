# Menu

`Menu` supports simple navigation and grouped, descriptive mega menus without JavaScript. It remains compatible with static SSR, interactive server rendering, and WebAssembly.

Use `MenuItem.Description` to add supporting text to an item. Place `MenuGroup` and `MenuCallToAction` instances in a submenu. Add `SubmenuClass="osirion-submenu-mega"` to arrange the groups in a responsive desktop grid; it becomes a single-column list on mobile.

```razor
<Menu BrandText="Contoso">
    <MenuItem Text="Platform" Href="/platform" HasSubmenu="true"
              SubmenuClass="osirion-submenu-mega">
        <MenuGroup Label="Core platform">
            <MenuItem Text="Tracking & attribution"
                      Description="Reliable measurement down to ad level."
                      Href="/platform/tracking" />
            <MenuItem Text="Automation"
                      Description="Protect margins while campaigns scale."
                      Href="/platform/automation" />
        </MenuGroup>
        <MenuGroup Label="Infrastructure">
            <MenuItem Text="Data & APIs"
                      Description="Integrate your existing stack."
                      Href="/platform/data" />
        </MenuGroup>
        <MenuCallToAction Text="Platform overview"
                          Description="Explore the complete platform."
                          Href="/platform" />
    </MenuItem>
</Menu>
```# Menu

Purpose
Responsive navigation menu with optional brand, orientation, alignment and sticky behavior.

Key parameters
- ChildContent (MenuItem/MenuGroup/MenuDivider)
- Href (brand link), BrandLogo, BrandText, BrandingTemplate
- Sticky (default true), StickyZIndex
- Orientation: Horizontal (default) or Vertical
- Alignment (for horizontal): Left, Center, Right
- CollapseOnMobile (default true)
- AutoExpandActive (vertical mode)
- AriaLabel, ToggleAriaLabel, Id

Notes
- Emits orientation/alignment classes and sticky styles
- Works with Osirion CSS variables and any framework.
