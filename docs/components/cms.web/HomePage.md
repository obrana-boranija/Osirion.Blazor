# HomePage

Purpose
A home page template. It shows a hero built from a content item (title, description, featured image), the item's
Markdown body, optional `ChildContent`, a featured posts section, optional category and tag lists, and an optional
block of custom HTML. It writes the page's SEO head tags from the item. With neither `Content` nor `ChildContent` it
renders the 404 "content not found" component and asks the framework for a 404 response. `AboutPage` and `ContactPage`
are the single-purpose pages in this folder; `HomePage` is the one for the site root.

Key parameters
Declared by `HomePage`:
- Content: `ContentItem?`, the item shown in the hero and as the body. The markup renders `Content`, not `Item`.
- ContentPath: `string?`, declared; the component's code does not read it.
- ChildContent: `RenderFragment?`, shown after the body in a second `div.osirion-homepage-content`
- ShowFeaturedContent: `bool`, default `true`. Renders an `OsirionContentListSection`.
- FeaturedSectionTitle: `string`, default `"Featured Content"`
- FeaturedSectionDescription: `string?`
- FeaturedLayout: `string`, default `"grid"`
- FeaturedColumns: `int`, default `3`
- FeaturedCount: `int`, default `3`. Passed to the section as `PostCount`.
- ShowFeaturedExcerpt, ShowFeaturedAuthor, ShowFeaturedCategories: `bool`, default `true`
- FeaturedViewAllUrl: `string?`
- ShowFeaturedViewAll: `bool`, default `true`
- ShowRecentContent: `bool`, default `true`
- RecentSectionTitle: `string`, default `"Recent Posts"`
- RecentCount: `int`, default `6`; declared, the component's code does not read it
- ReadMoreText: `string`, default `"Read more"`. Passed to the recent posts `ContentList`.
- ShowCategories: `bool`, default `false`. Renders a `CategoriesList` in the browse section.
- ShowTags: `bool`, default `false`. Renders a `TagCloud` in the browse section.
- BrowseSectionTitle: `string`, default `"Browse Content"`
- MaxCategories: `int`, default `8`
- MaxTags: `int`, default `15`
- ShowCategoryCounts, ShowTagCounts: `bool`, default `true`
- CustomContent: `string?`, HTML rendered in a last `ContentSection` through `OsirionHtmlRenderer`
- ContentUrlFormatter: `Func<ContentItem, string>?`, declared; the component's code does not read it

Used by `HomePage` from its base classes (`OsirionContentPageBase`, `OsirionContentItemPageBase`):
- Hero: `ShowHero` (default `true`), `HeroBackgroundPattern` (`BackgroundPatternType?`, default `Grid`),
  `UseHeroBackgroundImage`, `HeroImagePosition` (`Alignment`, default `Right`), `HeroAlignment` (`Alignment`, default
  `Left`), `HeroVariant` (default `Hero`), `HeroTheme` (`ThemeMode`, default `System`), `HeroPrimaryButtonText`,
  `HeroPrimaryButtonUrl`, `HeroSecondaryButtonText`, `HeroSecondaryButtonUrl`
- SEO: `SchemaTypes` (`SchemaType[]?`), `WebsiteName` (passed as the site name), `CustomBreadcrumbs`
  (`List<(string Name, string Url)>?`), `PrevPageUrl`, `NextPageUrl`, `AlternateLanguageUrls` (`List<string>?`)
- Browse links: `CategoryUrlFormatter` and `TagUrlFormatter` (`Func<string, string>?`, given the category or tag
  name), `CategoriesSectionTitle` (default `"Categories"`), `TagsSectionTitle` (default `"Tags"`)
- Loading: `LoadingText` (default `"Loading article..."`), `ShowLoadingText` (default `true`)
- Item lookup (the base loads an `Item` from these; the markup does not render it): `Item`, `ItemPath`, `ItemUrl`,
  `ItemSlug`, `Locale` (default `"en"`), `Query`, `DirectoryName`

Inherited and not read by the `HomePage` markup: `ShowNavigation`, `PreviousItem`, `NextItem`, `RelatedItems`,
`ItemUrlFormatter`, `ShowRelatedContent`, `RelatedContentTitle`, `RelatedItemsCount`, `GenerateMultipleSchemas`,
`SiteDescription`, `SiteLogoUrl`, `TwitterSite`, `TwitterCreator`, `FacebookAppId`, `AllowAiDiscovery`,
`AllowAiTraining`, `AllowSearchIndexing`, `EnableGeoOptimization`, `EnableAeoOptimization`, `DefaultImageUrl`,
`DefaultImageWidth`, `DefaultImageHeight`, `OrganizationName`, `NotFoundTitle`, `NotFoundText`, `ShowBackLink`,
`BackLinkUrl`, `BackLinkText`, `HasOverlay`, `HeroImageUrl`, `ShowHeroButtons`, `ShowBreadcrumbs`,
`ShowBreadcrumbHome`, `BreadcrumbHomeText`, `BreadcrumbHomeUrl`, `DirectoryUrlFormatter`, `ShowCategoriesSidebar`,
`ShowTagsSidebar`.
From `OsirionComponentBase`: `Class`, `Style`, `Theme` and `Attributes`; the first, second and last are rendered on the
root element.

Usage
```razor
@page "/"
@using Osirion.Blazor.Cms.Components
@using Osirion.Blazor.Cms.Domain.Entities
@using Osirion.Blazor.Cms.Domain.Services
@inject IContentProviderManager Providers

<HomePage Content="home"
          WebsiteName="My Site"
          FeaturedSectionTitle="Latest writing"
          FeaturedCount="3"
          HeroPrimaryButtonText="Read the blog"
          HeroPrimaryButtonUrl="/blog"
          ShowCategories="true"
          CategoryUrlFormatter="@(name => $"/category/{name}")" />

@code {
    private ContentItem? home;

    protected override async Task OnInitializedAsync() =>
        home = await Providers.GetDefaultProvider()!.GetItemByPathAsync("pages/home.md");
}
```

Notes
- Not routable: the host page declares the route (`@page "/"` above). Namespace `Osirion.Blazor.Cms.Components`.
- Renders `div.osirion-homepage` (plus `Class`), then, in order: a `HeroSection` when `ShowHero` is on, a
  `div.osirion-homepage-content` holding the body (`OsirionHtmlRenderer`; the body's leading title heading is dropped
  while the hero shows the title), the `ChildContent`, the featured section, the recent section, the browse section
  (`ContentSection.osirion-homepage-browse` with `div.osirion-browse-content` and one `div.osirion-browse-section`
  per list) and the custom block (`ContentSection.osirion-homepage-custom`).
- The SEO tags are written into `HeadContent` by `SeoMetadataRenderer`, so the host needs a `HeadOutlet`.
- While the base class loads its `Item`, the page shows `OsirionPageLoading`. When `Content` and `ChildContent` are both
  null and loading has finished, it shows `OsirionContentNotFound` with code 404 and calls the not-found signal
  (`NavigationManager.NotFound()` on .NET 10, a 404 status code on the cascading `HttpContext` otherwise).
- The recent posts section renders only when the page has recent items, and the component's code never assigns any, so
  `ShowRecentContent`, `RecentSectionTitle` and `ReadMoreText` currently have no visible effect.
- The featured section gets no items from this page (it passes null), so `OsirionContentListSection` loads its own.
- Services the host must register: `IContentProviderManager` (the base class and the sections use it) and the logging
  factory. The page works in static SSR and has no render mode of its own; the tests render it statically.
