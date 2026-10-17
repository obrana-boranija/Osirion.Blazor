# Osirion.Blazor.Templates

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Templates)](https://www.nuget.org/packages/Osirion.Blazor.Templates)
[![License](https://img.shields.io/github/license/demonjic/Osirion.Blazor)](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt)

Project templates for [Osirion.Blazor](https://getosirion.com).

```bash
dotnet new install Osirion.Blazor.Templates
dotnet new osirion-blog -n MyBlog
cd MyBlog
dotnet run
```

## Templates

| Short name | What you get |
| --- | --- |
| `osirion-blog` | A Markdown blog on .NET 10 with static server-side rendering: home page with the newest posts, a blog list, post pages, an About page, a 404 page, RSS, Atom and a sitemap. The menu and footer come from `appsettings.json`. |
| `osirion-docs` | A Markdown documentation site: a home page, pages with side navigation in front matter order, a table of contents, previous and next links, a 404 page and a sitemap. |
| `osirion-site` | A product or company site: a home page with a hero, three steps and the latest news, news from Markdown, About and Contact pages, RSS and a sitemap. |

## Options

| Option | Default | Description |
| --- | --- | --- |
| `--SiteName` | `My Blog`, `My Docs`, `My Product` | The name in the menu, the footer, the feeds and the page titles |
| `--SiteUrl` | `https://example.com` | The public address the feeds, the sitemap and `robots.txt` link to |
| `--skipRestore` | `false` | Skips the restore after the project is created |

Each template references the Osirion.Blazor version this package was released with.

## Documentation

The [getting started guide](https://getosirion.com/docs/getting-started) walks through the blog the template creates.
