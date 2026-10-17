---
title: "Introduction"
description: "What this documentation site is made of, and where to change each part."
slug: "introduction"
order: 1
author: "My Docs"
date: "2026-10-17"
published: true
---

This site is a Blazor app that renders the Markdown files in `content/docs`. Every page arrives as complete HTML, with
the folder's pages beside it, a table of contents, and links to the previous and next page.

## Where things live

| What | Where |
| --- | --- |
| Pages | `content/docs`, one Markdown file each |
| Their order in the navigation | `order` in each page's front matter |
| Menu, footer and site name | `Osirion:Site` in `appsettings.json` |
| Colours and dark mode | `Osirion:Theming` in `appsettings.json` |
| The home page | `Components/Pages/Home.razor` |

## Next

Read [Writing pages](/docs/writing-pages) to add the next page.
