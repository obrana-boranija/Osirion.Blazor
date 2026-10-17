# HomePage

Purpose
A home page template. It shows a hero built from a content item (title, description, featured image), the item's
Markdown body, optional `ChildContent`, a featured posts section, a recent posts section, optional category and
tag lists, and an optional
block of custom HTML. It writes the page's SEO head tags from the item. With neither `Content` nor `ChildContent` it
renders the 404 "content not found" component and asks the framework for a 404 response. `AboutPage` and `ContactPage`
are the single-purpose pages in this folder; `HomePage` is the one for the site root.

Key parameters
Declared by `HomePage`:
- Content: `ContentItem?`, the item shown in the hero and as the body. The markup renders `Content`, not `Item`.
- ContentPath: `string?`, declared; the component's code does not read it.
- ChildContent: `RenderFragment?`, shown after the body in a second `div.osirion-homepage-content`
- ShowFeaturedContent: `bool`, default `true`. Renders an `OsirionContentListSection` of the newest items of the
  page's `Locale` marked `featured: true`; without any the section is left out.
- FeaturedSectionTitle: `string`, default `"Featured Content"`
- FeaturedSectionDescription: `string?`
- FeaturedLayout: `string`, default `"grid"`
- FeaturedColumns: `int`, default `3`
- FeaturedCount: `int`, default `3`. Passed to the section as `PostCount`.
- ShowFeaturedExcerpt, ShowFeaturedAuthor, ShowFeaturedCategories: `bool`, default `true`
- FeaturedViewAllUrl: `string?`
- ShowFeaturedViewAll: `bool`, default `true`
- ShowRecentContent: `bool`, default `true`. The newest items of the page's `Locale`, featured or not; without any
  items the section is left out.
- RecentSectionTitle: `string`, default `"Recent Posts"`
- RecentCount: `int`, default `6`. The number of items the recent section loads and shows.
- ReadMoreText: `string`, default `"Read more"`. The link under each recent item; none when empty.
- ShowCategories: `bool`, default `false`. Renders a `CategoriesList` in the browse section.
- ShowTags: `bool`, default `false`. Renders a `TagCloud` in the browse section.
- BrowseSectionTitle: `string`, default `"Browse Content"`
- MaxCategories: `int`, default `8`
- MaxTags: `int`, default `15`
- ShowCategoryCounts, ShowTagCounts: `bool`, default `true`
- CustomContent: `string?`, HTML rendered in a last `ContentSection` through `OsirionHtmlRenderer`
- ContentUrlFormatter: `Func<ContentItem, string>?`, builds the link of each item of the featured and recent sections

Used by `HomePage` from its base classes (`OsirionContentPageBase`, `OsirionContentItemPageBase`):
- Hero: `ShowHero` (default `true`), `HeroBackgroundPattern` (`BackgroundPatternType?`, default `Grid`),
  `UseHeroBackgroundImage`, `HeroImageUrl` (falls back to the item's featured image), `HeroImagePosition` (`Alignment`, default `Right`), `HeroAlignment` (`Alignment`, default
  `Left`), `HeroVariant` (default `Hero`), `HeroTheme` (`ThemeMode`, default `System`), `HeroPrimaryButtonText`,
  `HeroPrimaryButtonUrl`, `HeroSecondaryButtonText`, `HeroSecondaryButtonUrl`
- SEO: `SchemaTypes` (`SchemaType[]?`), `WebsiteName` (passed as the site name), `CustomBreadcrumbs`
  (`List<(string Name, string Url)>?`), `PrevPageUrl`, `NextPageUrl`, `AlternateLanguageUrls` (`List<string>?`)
- Browse links: `CategoryUrlFormatter` and `TagUrlFormatter` (`Func<string, string>?`, given the category or tag
  name), `CategoriesSectionTitle` (default `"Categories"`), `TagsSectionTitle` (default `"Tags"`)
- Loading: `LoadingText` (default `"Loading article..."`), `ShowLoadingText` (default `true`)
- `Locale` (default `"en"`): the locale of the featured and recent items. The item lookup parameters of the base
  class (`Item`, `ItemPath`, `ItemUrl`, `ItemSlug`, `Query`, `DirectoryName`) are not used: the page runs no lookup.

Inherited and not read by the `HomePage` markup: `ShowNavigation`, `PreviousItem`, `NextItem`, `RelatedItems`,
`ItemUrlFormatter`, `ShowRelatedContent`, `RelatedContentTitle`, `RelatedItemsCount`, `GenerateMultipleSchemas`,
`SiteDescription`, `SiteLogoUrl`, `TwitterSite`, `TwitterCreator`, `FacebookAppId`, `AllowAiDiscovery`,
`AllowAiTraining`, `AllowSearchIndexing`, `EnableGeoOptimization`, `EnableAeoOptimization`, `DefaultImageUrl`,
`DefaultImageWidth`, `DefaultImageHeight`, `OrganizationName`, `NotFoundTitle`, `NotFoundText`, `ShowBackLink`,
`BackLinkUrl`, `BackLinkText`, `HasOverlay`, `ShowHeroButtons`, `ShowBreadcrumbs`,
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
- While the page loads its featured and recent items, it shows `OsirionPageLoading`. When `Content` and `ChildContent`
  are both null and loading has finished, it shows `OsirionContentNotFound` with code 404 and calls the not-found signal
  (`NavigationManager.NotFound()` on .NET 10, a 404 status code on the cascading `HttpContext` otherwise).
- The page loads the items of both sections itself, through the default provider, and again only when `Locale`,
  `ShowFeaturedContent`, `FeaturedCount`, `ShowRecentContent` or `RecentCount` change. A failed query is logged and
  leaves its section out.
- Services the host must register: `IContentProviderManager` (the base class and the sections use it) and the logging
  factory. The page works in static SSR and has no render mode of its own; the tests render it statically.
