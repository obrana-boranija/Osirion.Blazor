# AboutPage

Purpose
A whole about page from one content item, `about.md` (slug `about`) by default. The title is the page's `h1`, in an
`OsirionPageHeading` or, with `ShowHero`, a hero, with the description below it, then the Markdown body and whatever `ChildContent`
adds. It writes AboutPage structured data with the site's organization as its main entity, and answers 404 when the
content is missing. It renders without script.

Usage
```razor
@page "/about"
<AboutPage>
    <OsirionAuthorCard Name="Ana Author" Role="Maintainer" ProfileUrl="/authors/ana" />
</AboutPage>
```

Key parameters
- Content: `ItemSlug` (default `about`), `ItemPath`, `ItemUrl` or `Item`; `Locale`
- Heading: an `OsirionPageHeading` by default; `ShowHero` (false here) shows a `HeroSection` styled by the hero
  parameters of the page base instead
- `ChildContent`: shown after the body, for example author cards
- SEO: `SchemaTypes` (default `AboutPage`, plus a BreadcrumbList while `GenerateMultipleSchemas` is on),
  `WebsiteName`, `OrganizationName`, `SiteLogoUrl`, `AlternateLanguageUrls`

Notes
- An opening `# Title` in the Markdown is left out, since the heading already shows the title.
