# LandingPage

Purpose
A marketing page template. It shows a hero, then the Markdown body of a content item as a `ContentSection`, an optional
block of custom HTML as a highlighted section, and `ChildContent` as a last section. The item comes from `Content`, or
is loaded from the default content provider by `ContentPath`. Without an item, the hero text comes from `HeroTitle` and
`HeroSubtitle`. It works for pages that are not a blog, a document or a home page; `HomePage` is the one for the site
root.

Key parameters
Used by the page:
- ContentPath: `string?`, the path of the content item, loaded with `GetItemByPathAsync` on the default provider when
  `Content` is null
- Content: `ContentItem?`, the item to show; when set, nothing is loaded
- ShowHero: `bool`, default `true`
- HeroTitle, HeroSubtitle: `string?`, used when there is no content item (the item's title and description win)
- HeroBackgroundImage: `string?`, passed to the hero as `ImageUrl` when the item has no featured image
- HeroHeight: `string`, default `"70vh"`, passed to the hero as `MinHeight`
- HeroPrimaryButtonText, HeroPrimaryButtonUrl, HeroSecondaryButtonText, HeroSecondaryButtonUrl: `string?`
- ContentWidth: `string`, default `"normal"`
- ContentAlign: `string`, default `"left"`, passed as `TextAlign`
- ContentVariant: `string`, default `"default"`
- ContentPadding: `string`, default `"normal"`
- CustomContent: `string?`, HTML shown in a `ContentSection` with width `wide`, variant `highlight` and padding `large`
- ChildContent: `RenderFragment?`, shown in a `ContentSection` with width `wide` and padding `normal`
- LoadingText: `string`, default `"Loading page..."`
- NotFoundText: `string`, default `"Page content not found."`
- Class: `string?`, extra CSS classes on the root, from `OsirionComponentBase`

Declared but not read by the component's markup or code: `HeroAlignment` (`string`, default `"center"`),
`ShowHeroButtons` (`bool`, default `true`), `ShowTestimonials` (default `false`), `TestimonialsSectionTitle` (default
`"What Our Customers Say"`), `TestimonialsLayout` (default `"grid"`), `ShowTestimonialImages` (default `true`),
`TestimonialItems` (`IReadOnlyList<TestimonialItem>?`), `ShowFeatures` (default `false`), `FeaturesSectionTitle`
(default `"Features"`), `FeaturesSectionDescription`, `FeaturesLayout` (default `"grid"`), `FeaturesColumns` (`int`,
default `3`), `FeatureItems` (`IReadOnlyList<FeatureItem>?`), `ShowCallToAction` (default `true`), `CtaTitle`,
`CtaDescription`, `CtaPrimaryButtonText`, `CtaPrimaryButtonUrl`, `CtaSecondaryButtonText`, `CtaSecondaryButtonUrl`,
`CtaBackgroundColor`, `CtaVariant` (default `"primary"`). `OsirionComponentBase` also declares `Style`, `Theme` and
`Attributes`, which the markup does not render.

Usage
```razor
@page "/launch"
@using Osirion.Blazor.Cms.Components

<LandingPage ContentPath="pages/launch.md"
             HeroPrimaryButtonText="Get started"
             HeroPrimaryButtonUrl="/docs"
             HeroSecondaryButtonText="See the code"
             HeroSecondaryButtonUrl="https://github.com/demonjic/Osirion.Blazor"
             ContentWidth="wide"
             ContentAlign="center"
             NotFoundText="This page is not published yet.">
    <p>Questions? Write to the team.</p>
</LandingPage>
```

Notes
- Not routable: the host page declares the route. Namespace `Osirion.Blazor.Cms.Components`.
- Renders `div.osirion-landing-page` (plus `Class`), then `HeadContent` with `SeoMetadataRenderer` for the item, the
  `HeroSection` when `ShowHero` is on, `ContentSection` for the item body (the body's leading title heading is dropped
  while the hero shows the title), the custom content section and the `ChildContent` section.
- Loading: the page loads only when `Content` is null and `ContentPath` is not blank. The `div.osirion-loading`
  spinner with `LoadingText` shows only while loading and the component is interactive. When `Content` is still null
  after loading and `ContentPath` is set (no default provider, nothing found at the path, or the provider threw), it
  shows `div.osirion-not-found` with `NotFoundText` instead of the sections. A provider exception is logged, not
  shown. The page does not signal an HTTP 404.
- Services the host must register: `IContentProviderManager` (`GetDefaultProvider()` is used) and the logging
  factory. The page has no render mode of its own and works in static SSR; the tests render it statically and
  interactively.
- `TestimonialItem` has `Name`, `Role`, `Company`, `Quote`, `ImageUrl` and `Rating` (default `5`); `FeatureItem` has
  `Title`, `Description`, `Icon`, `IconUrl` and `LinkUrl`. Both are defined in the same file, and no markup reads them
  yet.
