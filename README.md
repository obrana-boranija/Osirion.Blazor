# Osirion.Blazor

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor)
[![CI](https://github.com/demonjic/Osirion.Blazor/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/demonjic/Osirion.Blazor/actions/workflows/ci.yml)
[![License](https://img.shields.io/github/license/demonjic/Osirion.Blazor)](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=demonjic_Osirion.Blazor&metric=security_rating)](https://sonarcloud.io/dashboard?id=demonjic_Osirion.Blazor)
[![codecov](https://codecov.io/gh/demonjic/Osirion.Blazor/branch/master/graph/badge.svg)](https://codecov.io/gh/demonjic/Osirion.Blazor)

Components, a Markdown CMS and SEO for Blazor Web Apps. Pages render with static server-side rendering and work
without JavaScript; where a component loads a script, the script only enhances what the server already rendered.

## 📦 Packages

`Osirion.Blazor` brings in every module and the `AddOsirion` registration method. Each module is also published on
its own.

| Package | What it adds |
|---|---|
| [Osirion.Blazor](https://www.nuget.org/packages/Osirion.Blazor) | Every module below, `AddOsirion` and `OsirionSiteLayout` |
| [Osirion.Blazor.Core](https://www.nuget.org/packages/Osirion.Blazor.Core) | Layout, section, card and form components, the HTML renderer and its sanitizer, cookie consent, email services ([README](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Core/README.md)) |
| [Osirion.Blazor.Cms](https://www.nuget.org/packages/Osirion.Blazor.Cms) | The content management modules, Web and Admin, in one reference ([README](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Cms/README.md)) |
| [Osirion.Blazor.Cms.Web](https://www.nuget.org/packages/Osirion.Blazor.Cms.Web) | Content lists, article and page templates, table of contents, SEO metadata and structured data, sitemap and feeds ([README](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Cms.Web/README.md)) |
| [Osirion.Blazor.Cms.Admin](https://www.nuget.org/packages/Osirion.Blazor.Cms.Admin) | An admin interface that edits content in a GitHub repository ([README](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Cms.Admin/README.md)) |
| [Osirion.Blazor.Cms.Core](https://www.nuget.org/packages/Osirion.Blazor.Cms.Core), [Domain](https://www.nuget.org/packages/Osirion.Blazor.Cms.Domain), [Application](https://www.nuget.org/packages/Osirion.Blazor.Cms.Application), [Infrastructure](https://www.nuget.org/packages/Osirion.Blazor.Cms.Infrastructure) | The CMS layers the two modules share: content model, queries, and the file system and GitHub providers |
| [Osirion.Blazor.Navigation](https://www.nuget.org/packages/Osirion.Blazor.Navigation) | Menus, enhanced navigation and scroll to top ([README](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Navigation/README.md)) |
| [Osirion.Blazor.Analytics](https://www.nuget.org/packages/Osirion.Blazor.Analytics) | Trackers for Microsoft Clarity, Google Analytics 4, Matomo, Plausible, Umami and Yandex Metrica ([README](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Analytics/README.md)) |
| [Osirion.Blazor.Theming](https://www.nuget.org/packages/Osirion.Blazor.Theming) | `OsirionStyles`, light and dark themes, named palettes, `ThemePicker`, and adapters for Bootstrap, FluentUI, MudBlazor and Radzen ([README](https://github.com/demonjic/Osirion.Blazor/blob/master/src/Osirion.Blazor.Theming/README.md)) |

## 🚀 Quick start

These steps put a Markdown blog into a new Blazor Web App. The
[getting started guide](https://github.com/demonjic/Osirion.Blazor/blob/master/content/en/docs/getting-started.md)
covers each step in full, including a GitHub repository as the content source.

```bash
dotnet new blazor -n MySite
cd MySite
dotnet add package Osirion.Blazor
```

Register the services in `Program.cs`:

```csharp
using Osirion.Blazor.Extensions;

builder.Services.AddOsirion(builder.Configuration);
builder.Services.AddJSComponents();
```

`AddOsirion(builder.Configuration)` reads the `Osirion` section. Point it at a folder of Markdown files in
`appsettings.json`:

```json
{
  "Osirion": {
    "Theming": { "Framework": "Bootstrap" },
    "Cms": {
      "Web": {
        "FileSystem": { "BasePath": "content", "IsDefault": true }
      }
    }
  }
}
```

Add the namespaces to `Components/_Imports.razor` and render `OsirionStyles` once in the layout:

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Cms.Components
@using Osirion.Blazor.Cms.Web.Components
@using Osirion.Blazor.Theming.Components
```

```razor
@inherits LayoutComponentBase

@Body

<OsirionStyles />
```

Then a list page and an article page in one component:

```razor
@page "/blog"
@page "/blog/{Slug}"

@if (string.IsNullOrWhiteSpace(Slug))
{
    <ContentList Directory="blog" ShowPagination />
}
else
{
    <ArticlePage DirectoryName="blog" ItemSlug="@Slug" SchemaTypes="new[] { SchemaType.BlogPosting }" />
}

@code {
    [Parameter]
    public string? Slug { get; set; }
}
```

Posts are Markdown files with front matter in `content/blog`. To serve `/sitemap.xml`, `/feed.xml` and `/atom.xml`,
add `app.MapOsirionFeeds(feeds => feeds.SiteUrl = "https://example.com");` after `builder.Build()`.

## 🆕 What's new in 4.1

- **Theming**: named palettes (`ThemePalette`) with light and dark variants, and `ThemePicker` for visitors. In the
  dark theme, the active navigation item and the danger and info messages now reach a 4.5:1 contrast ratio.
- **Table of contents**: `TableOfContents` marks the section being read as the page scrolls.
- **Content lists**: `ItemTemplate` on `ContentList`, `ContentListPage` and `OsirionContentListSection` renders each
  item through your own markup; `ArticlePage.HeaderContent` replaces the hero.
- **SEO**: the AI, indexing and schema switches in `SeoMetadataOptions` apply unless a page sets them; titles are no
  longer cut at 60 characters; `AuthorUrl` names the author's page in structured data.
- **Accessibility**: code blocks and tables in rendered content can be scrolled from the keyboard.
- **Content queries**: a category or tag matches by its name or its slug, exactly.
- **Last modified dates**: a file's write time no longer stands in for `last_modified`;
  `FileSystemOptions.UseFileWriteTimeAsLastModified` restores the old behaviour.

The [changelog](https://github.com/demonjic/Osirion.Blazor/blob/master/CHANGELOG.md) lists every change.

## 📚 Documentation

- [Getting started](https://github.com/demonjic/Osirion.Blazor/blob/master/content/en/docs/getting-started.md)
- [Component reference](https://github.com/demonjic/Osirion.Blazor/tree/master/docs/components), one page per component
- [Analytics](https://github.com/demonjic/Osirion.Blazor/blob/master/docs/ANALYTICS.md),
  [Navigation](https://github.com/demonjic/Osirion.Blazor/blob/master/docs/NAVIGATION.md),
  [GitHub CMS](https://github.com/demonjic/Osirion.Blazor/blob/master/docs/GITHUB_CMS.md) and
  [Styling](https://github.com/demonjic/Osirion.Blazor/blob/master/docs/STYLING.md)
- [Migration guide](https://github.com/demonjic/Osirion.Blazor/blob/master/docs/MIGRATION.md)
- [Examples](https://github.com/demonjic/Osirion.Blazor/tree/master/examples): the same site built with Bootstrap,
  FluentUI and MudBlazor

## 📋 Requirements

- .NET 9 or .NET 10
- A Blazor Web App. Content pages need no interactive render mode; the admin interface runs on Interactive Server.

## 🤝 Contributing

Contributions are welcome. [CONTRIBUTING.md](https://github.com/demonjic/Osirion.Blazor/blob/master/CONTRIBUTING.md)
explains how to build, test and measure coverage, and how a release is made. Report bugs and ask questions in
[issues](https://github.com/demonjic/Osirion.Blazor/issues).

## 📄 License

MIT. See [LICENSE.txt](https://github.com/demonjic/Osirion.Blazor/blob/master/LICENSE.txt).

## 🙏 Acknowledgments

- Built with [Blazor](https://blazor.net/) and [ASP.NET Core](https://asp.net/)
- Markdown processing by [Markdig](https://github.com/xoofx/markdig)
- Testing with [bUnit](https://bunit.dev/) and [Shouldly](https://shouldly.io/)
