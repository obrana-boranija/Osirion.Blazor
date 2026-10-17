---
title: "Write your first post"
description: "Add a Markdown file to content/blog and it appears on the home page, in the blog list, in the feeds and in the sitemap."
slug: "write-your-first-post"
featured_image: "/covers/write-your-first-post.svg"
author: "My Blog"
date: "2026-10-17T12:00:00Z"
categories:
  - "Guides"
tags:
  - "markdown"
published: true
---

A post is a Markdown file in `content/blog` that starts with front matter between two `---` lines:

```markdown
---
title: "My second post"
description: "One or two sentences for the list, the feeds and search results."
slug: "my-second-post"
date: "2026-10-18"
tags:
  - "notes"
published: true
---

The text of the post, in Markdown.
```

Save the file and reload the page: the folder is watched, so the post appears without a restart.

- `slug` is the address: `/blog/my-second-post`.
- `date` orders the posts, newest first, and is shown on each one.
- `published: false` keeps a draft out of every list and feed.

When the blog is ready, set `SiteUrl` to its public address in `appsettings.json` (under `Feeds`), so the feeds and
the sitemap link to the right place, and publish it with `dotnet publish`.
