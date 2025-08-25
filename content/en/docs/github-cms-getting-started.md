---
title: "GitHub CMS for Blazor: Getting Started"
author: "Osirion Team"
date: "2025-01-25"
description: "Set up a GitHub-backed content repository for your Blazor site with frontmatter, localization, and SSR-first rendering."
slug: "github-cms-getting-started"
categories: [CMS]
tags: [GitHub, CMS, Markdown, Frontmatter]
featured_image: "/images/docs/github-cms.jpg"
seo_properties:
  title: "Getting Started with GitHub CMS for Blazor"
  description: "Learn repository structure, frontmatter, queries, and SSR patterns for Osirion.Blazor CMS."
  og_image_url: "/images/social/github-cms.jpg"
  type: "Article"
---

# GitHub CMS for Blazor: Getting Started

Learn how to structure your content repo, write frontmatter, and connect your Blazor app.

## Repository structure

```
content/
??? en/
?   ??? blog/
?   ??? docs/
?   ??? pages/
??? fr/
    ??? ...
```

## Frontmatter template

```markdown
---
title: "Page Title"
author: "Author Name"
date: "2025-01-25"
description: "Short SEO description."
slug: "page-title"
categories: [Docs]
tags: [Blazor, CMS]
featured_image: "/images/feature.jpg"
seo_properties:
  title: "SEO Title"
  description: "SEO Description"
  og_image_url: "/images/social.jpg"
  type: "Article"
---
```

## App configuration

```csharp
builder.Services.AddOsirionContent(c => c.AddGitHub(o => {
  o.Owner = "username";
  o.Repository = "content-repo";
  o.ContentPath = "content";
  o.Branch = "main";
  o.UseCache = true;
}));
```

## Rendering content

```razor
<ContentView Path="blog/my-post.md" />
```

## Queries

```razor
<ContentList Directory="blog" Count="10" SortBy="SortField.Date" />
```

## SEO & SSR

- Use SeoMetadataRenderer with current content
- Keep pages functional without JS; enhance progressively

## Next steps

- [Content Structure & Frontmatter](/en/docs/content-structure-and-frontmatter)
- [Localization](/en/docs/localization-in-osirion)
