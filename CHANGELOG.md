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
- Osirion.Blazor.Cms.Infrastructure now references Osirion.Blazor.Core, which depends on HtmlSanitizer 9.2.1039 and AngleSharp 1.7.3

### Deprecated
- `LoginViewModel.LoginWithGitHubAsync(string code)` always fails, because a code without its OAuth state cannot be verified; use `StartGitHubLoginAsync()` and `LoginWithGitHubAsync(code, state)`
- `IAuthenticationService.AuthenticateWithGitHubAsync(string code)` is marked `[Obsolete]`: it exchanges a code without a state check, a PKCE verifier or a redirect URI. It still works; check the state, exchange the code with `IGitHubTokenProvider` and call `SetAccessTokenAsync` instead
- `AuthGuard.ShowLoginForm`, `AuthGuard.OnAuthResult` and `AdminPage.ShowLoginForm` are marked `[Obsolete]`, because they have no effect

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
