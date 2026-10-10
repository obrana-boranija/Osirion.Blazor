---
title: "Changelog"
author: "Dejan Demonjić"
date: 2025-04-23
---
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Mega menu building blocks in Osirion.Blazor.Navigation:
  - `MenuPanel` + `MenuPanelLink`: high-contrast promotional aside with pill links ("Platform highlights", "Build your business case"); brandable via `--osirion-menu-panel-background` / `--osirion-menu-panel-color`
  - `MenuImageCard`: linked image card with overlay caption for mega menu asides
  - `MenuItem.SubmenuAside` render fragment: dedicated right-hand aside column for mega submenus
  - `MenuGroup.Description`: column subtitle with divider (mega menu header style)
  - `MenuGroup.FooterText`/`FooterHref`/`FooterTarget`: "Explore X →" footer links per group
- bUnit coverage for all new mega menu components and parameters
- Content HTML sanitizer in Osirion.Blazor.Core: `IHtmlContentSanitizer`, `HtmlContentSanitizer`, `HtmlContentSanitizerOptions` (`AllowedTags`, `AllowedAttributes`, `AllowedSchemes`, `AllowElement`, `AllowAriaAttributes`, `AllowTableCellAlignment`), `AddOsirionHtmlSanitizer`, `GetHtmlContentSanitizer` and `JsonLdNormalizer`
- Osirion.Blazor.Cms.Admin: `CmsAdminAuthorization.PolicyName`, `AuthGuard.Policy`, `IGitHubTokenStore`, `IGitHubOAuthStateStore`, `GitHubOAuthRequest`, and the `LoginViewModel` members `StartGitHubLoginAsync`, `LoginWithGitHubAsync(code, state, cancellationToken)`, `IsServerCredentialConfigured`, `IsGitHubOAuthConfigured` and `GitHubRedirectUri`
- `IGitHubTokenProvider.ExchangeCodeForTokenAsync(code, clientId, clientSecret, redirectUri, codeVerifier, cancellationToken)` and `IMarkdownProcessor.SanitizeHtml(string)`
- Osirion.Blazor.Core: `LocalUrl` in `Osirion.Blazor.Core.Utils` (`IsLocalPath`, `GetSafeLocalPath` and `DefaultPath`), the local return-URL check used by the cookie consent endpoint and the admin login
- Analytics consent gate, off by default: `IAnalyticsBuilder.RequireConsent()` or `Osirion:Analytics:Consent:RequireConsent` renders trackers only after the visitor accepted the analytics category in `OsirionCookieConsent`, and GA4 then starts with Google Consent Mode defaults set to denied
- Matomo and Yandex Metrica report page views after Blazor enhanced navigation through a static script (`_content/Osirion.Blazor.Analytics/js/osirion-analytics.js`) when `AutoTrackPageViews` is on
- `IGitHubApiClient.GetTreeAsync` (default implementation returns null), `GitHubTree`, `GitHubRateLimitException`, `BaseContentRepository.GenerateMarkdownWithFrontMatterAsync` and `LastRefreshSucceeded`, and `UseOsirionCmsAsync`

### Changed
- Mega submenus now center under the whole navigation bar on desktop, use a responsive auto-fit column grid with column headers, and keep an invisible hover bridge so the pointer can travel from the trigger into the panel
- Mega submenu asides and group descriptions are hidden in the collapsed drawer and mobile views to keep the drawer scannable
- Collapsed drawer submenu toggle no longer paints an opaque hover background over the row label
- `OsirionHtmlRenderer.SanitizeHtml` now defaults to `true` and uses the registered `IHtmlContentSanitizer` instead of a regex sanitizer; `IMarkdownProcessor.RenderToHtml` and `RenderToHtmlAsync` now default `sanitizeHtml` to `true` and sanitize the rendered HTML
- `IGitHubApiClientFactory.GetClient` and `GetDefaultClient` return a new caller-owned client on every call
- `IAuthenticationService` and `IGitHubAdminService` are registered as scoped and replace any earlier registration
- `LoginViewModel`'s public constructor also takes `IGitHubTokenProvider`, `IGitHubTokenStore` and `IGitHubOAuthStateStore`
- `AuthGuard` enforces an authorization policy through `AuthorizeView`; `ShowLoginForm` and `OnAuthResult` still compile but do nothing
- The Cms.Admin event buses (`IEventPublisher`, `IEventSubscriber` and `CmsEventMediator`) are registered as scoped instead of singleton
- `AddOsirionCmsAdmin`, and `AddOsirion(configuration)` when an `Osirion:Cms:Admin` section exists, register authorization and the cascading authentication state
- A verified webhook delivery that matches no provider is answered with 202 instead of 400; tags and deleted branches no longer trigger a refresh
- `AddOsirionCookieConsent` registers the antiforgery services, and the banner posts the current path and query instead of the absolute URL
- Content files excluded from markdown rendering (`.txt`, `.json`, `.yml`, `.log`, `.bak`) are stored as sanitized HTML
- Osirion.Blazor.Cms.Infrastructure now references Osirion.Blazor.Core, which depends on HtmlSanitizer 9.2.1039 and AngleSharp 1.8.4
- Dependencies updated to their latest releases: .NET 9 packages to 9.0.20, .NET 10 packages to 10.0.12, Markdig 1.4.0, YamlDotNet 18.1.0, Scrutor 7.0.0, FluentValidation 12.2.0 and System.IO.Abstractions 22.3.0
- Osirion.Blazor.Cms.Domain, Cms.Core and Cms.Infrastructure use the `Microsoft.AspNetCore.App` framework reference instead of the legacy `Microsoft.AspNetCore.Http` 2.x packages and the `Microsoft.Extensions.*` packages that framework already provides
- Every package ships SourceLink information and a symbol package (`.snupkg`); the build is deterministic and treats warnings as errors
- Menu accessibility: the mobile toggle is keyboard-focusable and labelled with `aria-controls`; submenus follow the disclosure pattern (no `role="menu"`, `aria-haspopup` or constant `aria-expanded="false"`); a disabled `MenuItem` renders no `href`; `Menu` default ids are unique per instance (`osirion-menu-{orientation}-{suffix}`)
- `EnhancedNavigation` parameters are nullable, so configured `EnhancedNavigationOptions` apply unless a parameter is set; `INavigationService.ScrollToTopAsync` and `ScrollToElementAsync` are marked `[Obsolete]` because they only log
- `SeoMetadataRenderer`: canonical, og:url and structured-data URLs drop the query string and fragment (keep chosen parameters with the new `CanonicalQueryParameters`), collection pages declare a canonical, generated FAQ data is off by default (`EnableGeneratedFaq`) and valid when on, viewport and theme-color tags are opt-in (`EmitMobileMetaTags`), and dates are written as UTC
- `ArticlePage`, `DocumentPage` and `HomePage` no longer use streaming rendering and answer missing content with HTTP 404 (`NavigationManager.NotFound()` on .NET 10, the response status on .NET 9)
- `ArticlePage` tag badges are rendered locally with stable colours instead of loading images from img.shields.io
- The GitHub provider reads the repository with one Git Trees request and downloads only files whose SHA changed (about 80 requests per refresh for 50 files in 10 folders before, 3 to 13 now); branches are paged; rate-limit responses stop a refresh and keep the current content; GitHub clients get a 30-second timeout when the host left the default
- `UseOsirionCookieConsent` no longer sends a `Permissions-Policy` header: it used the obsolete Feature-Policy syntax, overwrote the host's header and has nothing to do with cookie consent
- Email providers use pooled `IHttpClientFactory` clients with a 30-second timeout and per-request authorization, with one shared mail body builder
- `OsirionComponentBase.IsServerSide` is obsolete because it is also true under Interactive Server; use `IsInteractive`, or the new `IsRunningInBrowser` for WebAssembly

### Deprecated
- `LoginViewModel.LoginWithGitHubAsync(string code)` always fails, because a code without its OAuth state cannot be verified; use `StartGitHubLoginAsync()` and `LoginWithGitHubAsync(code, state)`
- `IAuthenticationService.AuthenticateWithGitHubAsync(string code)` is marked `[Obsolete]`: it exchanges a code without a state check, a PKCE verifier or a redirect URI. It still works; check the state, exchange the code with `IGitHubTokenProvider` and call `SetAccessTokenAsync` instead
- `AuthGuard.ShowLoginForm`, `AuthGuard.OnAuthResult` and `AdminPage.ShowLoginForm` are marked `[Obsolete]`, because they have no effect
- `DomainEntityBase<TId>` and the unused Cms.Admin types `CommandDispatcher` (Application.Core), `ContentRepositoryAdapter`, `StatePersistenceService`, `StateManager`, `ErrorHandlingService` and `CmsAdminStartupService`; they will be removed in the next major release
- `BaseContentRepository.GenerateMarkdownWithFrontMatter`; use `GenerateMarkdownWithFrontMatterAsync`

### Fixed
- `MenuGroup` without an `Id` generated a new identifier on every read, so the items container's `aria-labelledby` pointed at an element that did not exist and the label and items ids changed on every render; the generated identifier is now created once per instance
- Navigation tests updated to the current `Menu`, `MenuItem` and `MenuGroup` markup, and `NavigationService` log assertions no longer try to match the `LogInformation` extension method with NSubstitute
- `/osirion/login`, `/osirion/settings` and the `Login`, `AdminLayout`, `AdminPage`, `CmsAdminDashboard` and `CmsLayoutEditor` components threw on render ("more than one parameter matching the name 'theme'"); their duplicate `string Theme` parameter is removed
- The default GitHub OAuth redirect URI is now `{BaseUri}osirion/login`; it was `/osirion/auth/callback`, which has no route, and `GitHubTokenProvider` sent a hard-coded `https://localhost:5001/auth/callback`
- `IGitHubAdminService` could not be constructed from the registrations of `AddGitHubProvidersFromConfiguration` and `AddGitHubContentProvider`
- GitHub files and folders whose names contain `#`, `?`, `%`, `+` or `&` now load
- Content ingestion kept `OriginalMarkdown` null, so a save could write a lossy conversion of the rendered HTML back to the repository
- Webhook deliveries for branch names that contain `/` now refresh the matching provider
- `MarkdownPreview` no longer renders and sanitizes the whole document on every render (every scroll event), and shows a fixed message instead of exception text when rendering fails
- The cookie consent Customize redirect adds its flag correctly to URLs that already have a query string
- Signing out from the admin also removes the stored GitHub token
- `ContentView`, `LocalizedContentView` and `DocumentPage` sanitize their content once per content change instead of on every render
- Osirion.Blazor.Core references AngleSharp directly. An application that references the Osirion projects from source (a `ProjectReference`) resolved HtmlSanitizer's lower AngleSharp version, and every sanitizer call failed with `FileNotFoundException` for AngleSharp 1.7.3; package consumers were not affected
- `IThemeService` is registered per scope instead of as a singleton. One user's theme choice was served to other users and re-rendered every user's `OsirionStyles` and `ThemeToggle`
- `OsirionStyles` now unsubscribes from theme changes when it is disposed; every rendered instance used to stay in memory for the life of the application
- `ScrollToTopManager` is registered per scope instead of as a singleton, so changing it for one user no longer changes the button for every user, and `ScrollToTopProvider` re-renders through its dispatcher
- Each GitHub content provider and the FileSystem provider now keep their own directory cache; with several providers, one provider could return another provider's directories
- The admin `Dashboard` page now unsubscribes from `CmsState` when it is disposed
- The FileSystem provider could not be resolved from DI because `System.IO.Abstractions.IFileSystem` was never registered; it is now registered with `TryAdd`, so a host's own registration wins
- `FileSystemContentRepository` returned nothing until a refresh, because its lazy load treated the empty cache as loaded
- `FileSystemDirectoryRepository` found no directories (it scanned the content root as a directory and dropped every nested directory as a circular reference), and `MoveAsync` never checked whether the destination exists
- Custom directory metadata is written to `_index.md` again; it was dropped on every save. `DirectoryItem.CustomMetadata` exposes the values, and only plain keys (letters, digits, `_`, `-`) are written
- `DirectoryCacheManager.InvalidateCacheAsync` and `ContentCacheManager` did nothing: invalidation never forced a reload and the content cache never returned a hit
- `ExceptionHandlingContentProviderDecorator` rejects a null inner provider and no longer wraps a `ContentProviderException` in another one
- `CreateDirectoryCommandHandler` threw `System.IO.DirectoryNotFoundException` instead of the domain exception for a missing parent
- `DirectoryItem.AddChild` threw for a child already under the directory, which also broke `CloneWithChildren`
- `LocalizationInfo.GetBestMatchingLocale` returns the declared locale code instead of the requested casing, and `LocalizationOptions.SetDefaultCulture` adds a culture that is not yet supported instead of ignoring it
- `AddOsirionGitHubContentProvider(Action<GitHubOptions>)` ignored its configuration
- CMS admin: a title ending in `.md` produced `titlemd.md`, a new post in a subfolder was saved with a backslash in its path on Windows, and `ErrorHandlingService` handled and logged each error twice
- `LocalizedContentView` threw `KeyNotFoundException` for any content with more than one translation, so `ContentPage` failed to render translated content; translation links now point to the right locale
- The `OsirionImageGallery` script did not parse, so the lightbox never opened
- Commands without a FluentValidation validator, such as `DeleteContentCommand`, could not be dispatched: resolving the open-generic validator adapter threw. Adapters are now registered only for validated commands
- `FrontMatter.Clone` dropped `Id`, `Order`, `Permalink` and `Lang` and shared `SeoProperties`, and `SeoMetadata.Clone` dropped `Image`, `Lang` and `MetaTags`, so editing metadata in the admin reset them (a translated post was saved as `lang: en`)
- `ContentItem.IsPublished` is now a view of `Status`. The two disagreed, so content created through `CreateContentCommand` was saved as a draft
- `CreateContentCommand` and `UpdateContentCommand` keep their markdown as the original source, so saving no longer converts the content back from HTML
- Front matter is found line by line: files with LF line endings lost the first character of their body (often the `#` of a heading), `---` inside a value ended the front matter early, and a file containing only `---` threw
- `IOsirionBuilder.UseCmsAdmin(Action<ICmsAdminBuilder>)` registered nothing; it now registers the admin services and applies the callback
- The markdown editor and preview wrote the scroll position with the current culture, which broke scroll sync in cultures with a decimal comma
- Content item ids, default localization ids, FileSystem directory ids and the default FileSystem provider id came from `string.GetHashCode()`, which is randomized per process, so they changed on every restart, differed between server instances and could collide. They are now SHA-256 based and stable; see MIGRATION for translations saved through the admin
- GA4 outbound link tracking no longer blocks navigation (links did nothing when gtag was blocked) and leaves new-tab, modified and `mailto:`/`tel:` clicks alone
- GA4 `SendPageView=false` is honoured
- Tracker component parameters (`Enabled`, `SiteId`, `MeasurementId`, `DebugMode`, `CounterId`, `WebVisor`, `ClickMap`, `TrackLinks`, `AccurateTrackBounce`) were ignored; they now override the configured options for that render
- Yandex Metrica options bind from `Osirion:Analytics:YandexMetrica` like the other providers (the old `YandexMetrica` key is still read), and each analytics provider is registered once with `IAnalyticsProvider` returning the same instance
- `MenuItem` renders `target` and `rel="noopener noreferrer"` on the link instead of putting `rel` on the wrapper; its active state no longer overwrites the `IsActive` parameter, follows navigation when interactive, ignores query and fragment, and `Prefix` matches on a segment boundary
- `Menu` and `MenuGroup` keep their classes, styles and ids when a parent renders again
- `ScrollToTopOptions.Enabled` is honoured, the scroll listener is registered once, and the button id is unique per instance
- `SeoMetadataRenderer` recomputes when its parameters change; the Event schema no longer invents an end date; `keywords` and `article:section` are no longer duplicated; malformed hreflang entries are skipped instead of throwing; the article check ignores case
- `ContentList` pagination keeps the search query and other parameters, navigates on click, clamps invalid page numbers and fetches the list once
- `ContentView`, `ContentPage` and the detail pages reload when their path, url or slug changes instead of keeping the previous item, and `ContentPage` share links use the real page URL
- Tag and category URLs come from one invariant, URL-encoded slug helper (`C#` no longer turns into a fragment), and `ContentPage` passes its locale and URL formatters to `LocalizedContentView`
- Cms.Web components log failures through `ILogger` instead of the console or empty catch blocks; `CategoriesList` and `TagCloud` fetch once; `DocumentPage` writes a valid schema.org `itemtype`
- `DirectoryItem` and `ContentItem` keep the directory tree consistent from both sides: moving a child no longer lists it under two parents, and `SetParent` accepts moving a directory up to a higher ancestor
- `GenerateSlug` strips diacritics and transliterates Serbian Latin and Serbian and Russian Cyrillic; titles with no Latin form get a stable short hash instead of `untitled`. Stored slugs are unchanged
- Entity equality compares the runtime type, and entities without an id are equal only to themselves
- `DomainEventDispatcher` runs handlers registered for the event's concrete type
- CMS admin: `AddOsirionCmsAdmin(Action<...>)` no longer builds a second service provider; the edit page loads a file once per path change; background failures are logged; error messages shown to users no longer contain exception text; view models unsubscribe from shared state when disposed
- CMS admin: the GitHub sign-in is restored on any admin page after a reload when the admin layout renders interactively
- Refreshing a CMS cache cleared the host application's whole memory cache (`Compact(1.0)`); only this library's entries are evicted now, including `IContentCacheService.ClearAsync` and `RemoveByPrefixAsync`
- Cold-start readers waited for nothing and got empty results; they now await the first load, a refresh requested during a refresh runs once more afterwards, and a failed refresh keeps the previous content instead of emptying it
- The fallback front matter writer escapes `id`, `slug`, `lang`, `featured_image` and control characters, so a value can no longer add YAML keys
- Carousels run their animation work only while visible and animating, no longer block vertical page scrolling, pause on keyboard focus, get a pause button, show every testimonial under reduced motion and make their looping copies inert
- `OsirionComponentBase` applies the theme attribute on every parameter update without changing the caller's `Attributes` dictionary
- `OsirionContactForm` no longer tells visitors they were flagged as spam when the email service is missing, and no longer shows placeholder New York contact details
- `OsirionCookieConsent` honours `ConsentExpiryDays`, applies extra attributes once and renders nothing instead of throwing without an `HttpContext`
- `ThemeToggle` drops a form that posted to an unmapped endpoint and its fixed ids; the image gallery uses a link trigger that works without JavaScript, traps focus in the viewer and has valid caption markup; the last breadcrumb carries `aria-current="page"`; `OsirionHtmlRenderer` no longer duplicates the line-numbers class or wraps identical code blocks twice

### Security
- Osirion.Blazor.Cms.Admin: every routed admin page (`/osirion`, `/osirion/dashboard`, `/osirion/content`, `/osirion/content/edit`, `/osirion/settings`, `/osirion/login`) requires the authorization policy `"Osirion.Cms.Admin"` (`CmsAdminAuthorization.PolicyName`). The default policy requires an authenticated user; a host policy with that name replaces it. These pages, and the repository writes behind them, were open to anonymous visitors
- Osirion.Blazor.Cms.Admin: the configured `Osirion:Cms:Admin:Authentication:PersonalAccessToken` no longer signs in every visitor of `/osirion/login` and is never sent to the browser. Admin services use it on the server only and never use a content provider's `ApiToken`
- Osirion.Blazor.Cms.Admin: a user's own GitHub token is kept in `ProtectedLocalStorage`, encrypted with Data Protection under a purpose bound to the signed-in host user, instead of plain-text localStorage (`github_auth_token`); plain-text copies left by earlier versions are removed
- Osirion.Blazor.Cms.Admin: GitHub OAuth sends a single-use `state`, compared in constant time, and a PKCE S256 `code_challenge`, which closes login CSRF through a planted `?code=`. Login `returnUrl` values are limited to local paths, and login errors no longer show exception text
- Osirion.Blazor.Cms.Infrastructure: GitHub API clients are no longer shared and send their headers per request, so a token, repository or branch set in one admin session can no longer reach another user or the public site. Cookies are turned off on the GitHub HTTP clients' primary handlers
- Osirion.Blazor.Cms.Infrastructure: GitHub paths, branch names, owner and repository are percent-encoded segment by segment, and empty, `.` or `..` segments are rejected with `ArgumentException` before any request is sent. `GitHubApiClient.DeleteFileAsync` throws instead of sending a request when the owner, repository or token is missing
- Osirion.Blazor.Cms.Infrastructure: the GitHub webhook (`UseGitHubWebhook`, `POST /api/github/webhook`) verifies `X-Hub-Signature-256` (HMAC-SHA256 over the raw body, constant-time comparison) against the providers' `WebhookSecret`, rejects every delivery while no provider has a secret, refuses bodies over 5 MiB and refreshes only providers whose owner, repository, branch and secret match exactly
- Osirion.Blazor.Cms.Infrastructure: `FileSystemContentRepository` and `FileSystemDirectoryRepository` no longer write, delete, create or move anything outside `FileSystemOptions.BasePath`. Osirion.Blazor.Cms.Application: `CreateContentCommandValidator` and `UpdateContentCommandValidator` reject rooted paths, `.`, `..` and empty segments, and control characters
- Content HTML is sanitized with an allow-list (HtmlSanitizer on the AngleSharp HTML5 parser) when markdown is rendered or ingested, and again where it is rendered as raw markup (`OsirionHtmlRenderer`, `ContentView`, `LocalizedContentView`, `DocumentPage`, `MarkdownPreview` and the admin preview). Script, event handler attributes, `javascript:`, `vbscript:` and `data:` URLs, forms, iframes, inline styles, `<style>`, `svg` and the `name` attribute are removed; `class`, `id`, `role`, `aria-*` and table cell `text-align` are kept
- Front matter JSON-LD is parsed and re-serialized with `<`, `>` and `&` escaped before `SeoMetadataRenderer` writes it into a script element; invalid JSON-LD is dropped with a warning
- Osirion.Blazor.Core: the cookie consent endpoint (`MapOsirionCookieConsent`) requires an antiforgery token, which the banner form now carries, and redirects only to local paths
- Osirion.Blazor.Cms.Admin: the admin event buses are scoped to one circuit or request. As singletons they delivered one admin's events to every other admin's editor, so opening or creating content in one session could replace another admin's unsaved draft, and authentication and error events crossed sessions
- Osirion.Blazor.Cms.Admin: `AddGitHubAdminServices` turns cookies off on the GitHub HTTP clients itself, through the same registration as Osirion.Blazor.Cms.Infrastructure, instead of relying on another registration method to do it
- Osirion.Blazor.Core: `HeroSection` CSS-escapes `ImageUrl` when it writes the background image, so a front matter `featured_image` (passed by `ArticlePage`, `HomePage` and `LandingPage`) can no longer close the `url()` value and add style declarations such as a full-page overlay
- Analytics trackers (Clarity, Matomo, GA4, Yandex Metrica) JSON-encode every configured value they write into scripts and attributes; invalid GA4 ids, Yandex counters and tracker or CDN URLs fail at startup with a message naming the setting
- Navigation links (`MenuItem`, `MenuGroup` footer, `MenuPanelLink`, `MenuImageCard`, `MenuCallToAction` and the `Menu` brand link) render no `href` for schemes other than relative, http, https, mailto and tel, so a `javascript:` URL from CMS-driven menu data cannot run
- `ScrollToTop.CustomIcon` is sanitized; the new `IconCssClass` is the way to use a custom icon
- The CMS query cache key includes every query field, so a draft query (`IncludeUnpublished`) and a public query can no longer share a cached result and leak drafts to the public site
- `OsirionHtmlRenderer` loads the pinned Prism 1.29.0 from cdnjs with Subresource Integrity (new `PrismBaseUrl` to self-host), `OsirionBaseSection` CSS-escapes `BackgroundImageUrl` like `HeroSection`, and `OsirionContactInfoSection` validates website URLs, encodes phone, email and website, and sanitizes its markup fields

## [1.5.0] - 2025-04-21

### Added
- Improved service registration with fluent API through `AddOsirionBlazor` method
- New `IOsirionBlazorBuilder` interface for better extensibility
- `OsirionStyleOptions` class separated from `GitHubCmsOptions` for better separation of concerns
- CSS framework integration support for Bootstrap, FluentUI, MudBlazor, and Radzen
- CssFramework enum for specifying framework integration options
- Integration classes for mapping Osirion variables to CSS framework variables
- Updated OsirionStyles component with FrameworkIntegration parameter
- Framework integration documentation with examples for each supported framework
- Comprehensive unit tests for framework integration

### Changed
- Refactored service collection extensions for better organization
- Renamed `osirion-cms.css` to `osirion.css` for better clarity on its broader purpose
- Enhanced the styling documentation to include framework integration examples
- Updated the README.md and Quick Reference with CSS framework integration examples and fluent API usage
- Styling options moved from `GitHubCmsOptions` to dedicated `OsirionStyleOptions` class

## [1.4.0] - 2025-04-20

### Added
- ScrollToTop component for easy navigation back to the top of the page
- ButtonPosition enum for positioning UI elements
- ScrollToTopOptions for DI-based configuration
- Extension methods for registering ScrollToTop configuration
- Comprehensive unit tests for ScrollToTop component
- Updated documentation for navigation components
- CSS variables for styling the ScrollToTop button

### Changed
- Enhanced the navigation documentation to include ScrollToTop
- Updated the README.md and Quick Reference with ScrollToTop examples
- Improved the usability of the navigation components

## [1.3.0] - 2025-04-20

### Added
- GitHub CMS service and components for markdown-based content management
- Support for frontmatter parsing in markdown files
- Content caching with configurable duration
- Category and tag management components
- Full-text search functionality
- Directory-based navigation
- Default styling with customizable CSS variables
- OsirionStyles component for theme customization
- Comprehensive unit tests for GitHub CMS
- Detailed documentation and examples

### Changed
- Updated package metadata to include CMS capabilities
- Enhanced documentation with GitHub CMS examples
- Added Markdig dependency for markdown processing

## [1.2.0] - 2025-04-20

### Added
- ClarityTracker component for Microsoft Clarity analytics integration
- MatomoTracker component for Matomo analytics integration
- TrackerBaseOptions abstract class for common analytics properties
- AnalyticsServiceCollectionExtensions for easy dependency injection configuration
- Comprehensive unit tests for all analytics components
- Detailed documentation for analytics components
- Quick reference guide for component usage
- Migration guide for upgrading

### Changed
- Updated package description to include analytics components
- Added analytics-related tags to package metadata
- Improved README with proper usage examples for all components

## [1.1.0] - 2025-01-18

### Added
- EnhancedNavigationInterceptor component for automatic scroll behavior
- SSR compatibility for all components
- Support for .NET 8, .NET 9, and future versions
- Navigation documentation
- Enhanced examples and usage patterns

### Changed
- Improved package metadata and documentation
- Added navigation-related tags to package metadata

## [1.0.0] - 2025-01-15

### Added
- Core package structure
- Basic documentation and licensing
- Service collection extensions
- Initial project setup
