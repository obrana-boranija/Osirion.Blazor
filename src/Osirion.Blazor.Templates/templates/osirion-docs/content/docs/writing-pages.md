---
title: "Writing pages"
description: "A page is a Markdown file in content/docs with a title, a slug and its place in the navigation."
slug: "writing-pages"
order: 2
author: "My Docs"
date: "2026-10-17"
published: true
---

Create a file in `content/docs` that starts with front matter between two `---` lines:

```markdown
---
title: "Deployment"
description: "One or two sentences for search results and link previews."
slug: "deployment"
order: 4
published: true
---

The text of the page, in Markdown.
```

Save it and reload: the folder is watched, so the page appears in the navigation without a restart.

## Front matter

| Key | Meaning |
| --- | --- |
| `title` | The page's heading and the text in the navigation |
| `slug` | The address: `/docs/deployment` |
| `order` | The position in the navigation and in the previous and next links |
| `description` | The meta description |
| `last_modified` | Shown as "Last updated"; without it, `date` is shown |
| `published: false` | Keeps a draft off the site |

## Headings

Second, third and fourth level headings (`##` to `####`) make up the table of contents beside the page.
