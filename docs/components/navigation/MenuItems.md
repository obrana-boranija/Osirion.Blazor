# MenuItem / MenuGroup / MenuDivider / MenuPanel / MenuImageCard

## Purpose

Composable items for [Menu](Menu.md), including the mega menu building blocks.

## MenuItem

- Text, Description, Href, Target, IsActive, Disabled, Match, AutoDetectActive, OnClick
- Icon: a built-in icon name (see Menu.md); an unknown name renders no icon
- IconTemplate (fragment): your own icon. In a mega submenu it sits in a tinted tile
- HasSubmenu + ChildContent: the submenu items
- A parent with HasSubmenu and no Href (or "#") renders as the submenu toggle itself
- SubmenuVariant (Regular | Mega)
- SubmenuAside (fragment): right-hand column of a mega submenu, typically a MenuPanel or MenuImageCard. Left out of
  the drawer

## MenuGroup

- Label, Description (column subtitle under the label, with a divider, in mega submenus)
- FooterText + FooterHref (+ FooterTarget): an "Explore X" link at the bottom of the group
- Collapsible, Expanded, ChildContent
- In the drawer a group becomes a list with a small uppercase label; its description is left out

## MenuCallToAction

- Text, Description, Href, Target
- In a mega submenu it spans every column, below the groups

## MenuDivider

- Separator with an optional Label: a horizontal rule in lists and the drawer, a short vertical rule between the
  items of a horizontal bar

## MenuPanel

- High-contrast promotional aside for mega menus ("Platform highlights", "Build your business case")
- Title, Description, AriaLabel, ChildContent (MenuPanelLink items)
- Theme-aware: defaults to the inverted surface tokens; rebrand per site via
  --osirion-menu-panel-background and --osirion-menu-panel-color

## MenuPanelLink

- Pill-style featured link inside MenuPanel
- Text, Href, Target, IconTemplate; hover reveals a trailing arrow

## MenuImageCard

- Linked image card with an overlay caption (customer stories, newsroom features)
- ImageUrl, ImageAlt, Title, Description, Href, Target
- Caption sits on a dark scrim so contrast holds in both themes

## Notes

- Combine them to build multi-level horizontal or vertical menus.
- Mega submenus render a responsive column grid for groups; the panel centres under the whole bar on desktop and
  becomes one list in the drawer.
- Keep deep trees out of dropdowns: two levels read well, three rarely do. A mega submenu shows a large section
  better than a chain of flyouts.
