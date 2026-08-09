# MenuItem / MenuGroup / MenuDivider / MenuPanel / MenuImageCard

Purpose
Composable items for Menu, including Personio-style mega menu building blocks.

MenuItem
- Text, Href, Target, Active, Disabled, Description
- Icon (fragment), ChildContent for dropdowns/submenus
- SubmenuVariant (Regular | Mega)
- SubmenuAside (fragment): dedicated right-hand column for mega submenus,
  typically a MenuPanel or MenuImageCard. Hidden automatically in the
  collapsed mobile/drawer view.

MenuGroup
- Label, Description (column subtitle shown under the label with a divider in mega menus)
- FooterText + FooterHref (+ FooterTarget): renders an "Explore X →" link at the bottom of the group
- Collapsible, Expanded, ChildContent

MenuDivider
- Visual separator; optional Label

MenuPanel
- High-contrast promotional aside for mega menus ("Platform highlights", "Build your business case")
- Title, Description, AriaLabel, ChildContent (MenuPanelLink items)
- Theme-aware: defaults to the inverted surface tokens; rebrand per site via
  --osirion-menu-panel-background and --osirion-menu-panel-color

MenuPanelLink
- Pill-style featured link inside MenuPanel
- Text, Href, Target, IconTemplate; hover reveals a trailing arrow

MenuImageCard
- Linked image card with an overlay caption (customer stories, newsroom features)
- ImageUrl, ImageAlt, Title, Description, Href, Target
- Caption sits on a dark scrim so contrast holds in both themes

Notes
- Combine to build multi-level horizontal or vertical menus.
- Mega submenus render a responsive auto-fit column grid for groups; the panel
  centers under the whole navigation bar on desktop and stacks vertically in
  the collapsed drawer and on mobile.
