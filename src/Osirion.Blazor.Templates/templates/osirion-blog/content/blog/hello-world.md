---
title: "Hello, world"
description: "The first post of this blog: what the site is made of and where each part lives."
slug: "hello-world"
featured_image: "/covers/hello-world.svg"
author: "My Blog"
date: "2026-10-17"
categories:
  - "Notes"
tags:
  - "blazor"
  - "osirion"
published: true
---

This blog is a Blazor app that renders Markdown files on the server. Every page arrives as complete HTML, so people,
search engines and feed readers all see the same thing.

## Where things live

| What | Where |
| --- | --- |
| Posts | `content/blog`, one Markdown file each |
| The About page | `content/about.md` |
| Menu, footer and site name | `Osirion:Site` in `appsettings.json` |
| Colors and dark mode | `Osirion:Theming` in `appsettings.json` |
| Your own styles | `wwwroot/app.css` |

## A code block

Code is highlighted in the colors of the site's palette:

```csharp
builder.Services.AddOsirion(builder.Configuration);
```

Write the next post in [Write your first post](/blog/write-your-first-post).
