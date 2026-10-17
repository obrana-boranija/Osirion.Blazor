---
title: "Introducing My Product"
description: "The first news item: what My Product is, and where this site keeps its pages and news."
slug: "introducing-my-product"
featured_image: "/covers/introducing-my-product.svg"
author: "My Product"
date: "2026-10-17"
categories:
  - "Announcements"
published: true
---

This is the first news item. Replace it with your own announcement.

## Where things live

| What | Where |
| --- | --- |
| News | `content/news`, one Markdown file each |
| The About and Contact pages | `content/about.md` and `content/contact.md` |
| The home page | `Components/Pages/Home.razor` |
| Menu, footer and site name | `Osirion:Site` in `appsettings.json` |
| Colours and dark mode | `Osirion:Theming` in `appsettings.json` |

A news item needs front matter with a `title`, a `slug`, a `date` and a `description`; the date orders the list,
newest first, and the item appears on the home page, in the RSS feed and in the sitemap.
