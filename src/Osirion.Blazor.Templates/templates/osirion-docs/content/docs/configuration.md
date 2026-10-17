---
title: "Configuration"
description: "The settings in appsettings.json that shape the site: name, menu, footer, colours and public address."
slug: "configuration"
order: 3
author: "My Docs"
date: "2026-10-17"
published: true
---

Everything that is not a page is in `appsettings.json`.

## Site name, menu and footer

`Osirion:Site` holds the name in the menu and the footer, the menu entries, and the footer's columns:

```json
"Site": {
  "Name": "My Docs",
  "Menu": [
    { "Text": "Documentation", "Href": "/docs/introduction", "MatchPrefix": true }
  ]
}
```

## Colours

`Osirion:Theming:DefaultPalette` picks the colours: `Osirion`, `Commit`, `Editor`, `Studio`, `StudioBlue` or
`Contrast`, each with a light and a dark variant. `DefaultMode` is `Light`, `Dark` or `System`.

## Public address

Before publishing, set `Feeds:SiteUrl` to the site's address, so the sitemap links to it, and the `Sitemap` line in
`wwwroot/robots.txt`. Then run `dotnet publish`; the `content` folder is copied with the app.
