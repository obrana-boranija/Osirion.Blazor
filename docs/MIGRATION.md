# Migration Guide

## Upgrading to the next release (Unreleased)

### Breaking Changes

This release closes security holes in the CMS and Core modules. Most changes are behavioural. Skip the steps for features you do not use.

| Area | Change | Who must act |
| --- | --- | --- |
| CMS admin | Admin pages require the `Osirion.Cms.Admin` authorization policy | Hosts that map the admin pages (step 1) |
| CMS admin | The configured admin token no longer signs anyone in | Hosts that relied on that sign-in (step 2) |
| Webhooks | Deliveries must be signed with `WebhookSecret` | Hosts that call `UseGitHubWebhook` (step 3) |
| Content | HTML is sanitized by default, including `OsirionHtmlRenderer` | Every host that renders content (step 4) |
| Cookie consent | Antiforgery token required, local return URLs only | Hosts that call `MapOsirionCookieConsent` (step 5) |
| Content paths | Rooted paths and `.`/`..` segments are rejected | Code that builds content paths (step 6) |
| API | Interface, constructor and lifetime changes | Implementers and direct callers (step 7) |
| Content ids | Item, directory, localization and FileSystem provider ids are now stable hashes | Hosts that stored ids, and repositories with translations saved through the admin (step 8) |

### Migration Steps

1. **CMS admin: add host authentication.** Every routed admin page (`/osirion/...`, `/osirion/login` included) requires the policy `CmsAdminAuthorization.PolicyName` (`"Osirion.Cms.Admin"`). The default policy lets in any authenticated user. The library never adds an authentication scheme, so a host that maps `typeof(Osirion.Blazor.Cms.Admin._Imports).Assembly` must add one:
   ```csharp
   builder.Services.AddRazorComponents().AddInteractiveServerComponents();
   builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options => options.LoginPath = "/account/login"); // your own sign-in page

   // Recommended where anyone can register: only editors may use the admin
   builder.Services.AddAuthorization(options =>
       options.AddPolicy(CmsAdminAuthorization.PolicyName, policy => policy.RequireRole("Editor")));

   var app = builder.Build();

   app.UseAuthentication();
   app.UseAuthorization(); // after UseRouting() when you call it
   app.UseAntiforgery();
   ```
   - Without a scheme, an admin request fails with `InvalidOperationException` ("Unable to find the required 'IAuthenticationService' service"). With `UseRouting()` but no `UseAuthorization()`, it fails with "contains authorization metadata, but a middleware was not found".
   - A host policy with that name replaces the default, whatever the registration order.
   - `/osirion/login` is the GitHub credential step, not a host sign-in page; point the cookie `LoginPath` at your own sign-in page.
   - Keep `AddInteractiveServerComponents()`: the GitHub credential stores use protected browser storage. When the whole app is interactive, route with `AuthorizeRouteView`.
   - The example app (`examples/Osirion.Blazor.Example.Bootstrap`) registers no authentication, so its `/osirion` routes now end in an error. This is intended: add your own sign-in.
   - Hosts that never map the admin need no change. `AddOsirionCmsAdmin`, and `AddOsirion(configuration)` when an `Osirion:Cms:Admin` section exists, now also call `AddAuthorization()` and `AddCascadingAuthenticationState()`.
   - Details: [Securing the Admin](../src/Osirion.Blazor.Cms.Admin/README.md#securing-the-admin).

2. **CMS admin: GitHub credentials.**
   - `Osirion:Cms:Admin:Authentication:PersonalAccessToken` no longer signs anyone in and is never sent to the browser. Admin services still use it on the server. **Rotate it**: earlier versions applied it to every visitor of `/osirion/login` and wrote it to their localStorage in plain text. Ask users who typed their own tokens to rotate them too.
   - Admin services never use a content provider's `ApiToken`. If admin writes relied on it, set `PersonalAccessToken` or have users sign in with their own token.
   - A user's own token is stored in localStorage under `osirion_cms_admin_github_token`, encrypted with Data Protection under a purpose bound to the host user (`ClaimTypes.NameIdentifier`, else `Identity.Name`; with neither, it lasts for the session only). Plain-text `github_auth_token` copies are removed when the user opens the login page, signs in or signs out, so those users sign in once more. Multi-instance deployments need a shared Data Protection key ring; a key ring change signs users out of GitHub.
   - GitHub OAuth sends a single-use `state` and a PKCE S256 `code_challenge`. The redirect URI defaults to `{BaseUri}osirion/login`: register `https://<your host>/osirion/login` as the OAuth app callback URL, or set `GitHubRedirectUri` to the URL you registered. A sign-in must finish in the browser tab that started it.
   - A `returnUrl` that is not a local path is replaced by `/osirion`.
   - Known limitations: a stored per-user token is restored only when `/osirion/login` is opened, so without a server token a user who reloads another admin page opens `/osirion/login` once to sign in again. Where only the admin pages are interactive, the admin layout renders statically and shows no sign-out control; use Sign Out on `/osirion/settings`.

3. **GitHub webhook: configure the secret.** Deliveries are rejected with 400 unless `X-Hub-Signature-256` verifies against the `WebhookSecret` of a configured provider, and every delivery is rejected while no provider has one.
   ```bash
   dotnet user-secrets set "Osirion:Cms:Web:GitHub:<Name>:WebhookSecret" "<long random value>"
   ```
   - Set the same value as the webhook secret in GitHub, with content type `application/json` and the exact URL `https://<your host>/api/github/webhook` (no trailing slash, no redirect).
   - Provider options are read at startup: restart after changing the secret, then check GitHub's Recent Deliveries.
   - Bodies over 5 MiB are rejected. The middleware lowers the request body limit of the webhook path to 10 MiB and keeps a stricter host limit, so a host limit below 5 MiB rejects larger deliveries.
   - Refresh matches the provider id exactly (`ProviderId`, or `github-{Owner}-{Repository}`) and the branch exactly and case-sensitively. Tags and deleted branches no longer refresh.
   - A custom `IGitHubWebhookHandler` must verify the signature itself.
   - Details: [Setup GitHub Webhooks](GITHUB_CMS.md#setup-github-webhooks).

4. **Content: review HTML sanitization.** Rendered markdown, ingested content, `ContentView`, `LocalizedContentView`, `DocumentPage`, `MarkdownPreview` and `OsirionHtmlRenderer` now sanitize HTML. `OsirionHtmlRenderer.SanitizeHtml` defaults to `true`, which also covers HTML you pass yourself (for example `HomePage.CustomContent` and `ContentSection.Content`).
   - Removed: `script`, `on*` event handlers, `javascript:`, `vbscript:` and `data:` URLs, forms and form controls (task-list checkboxes too), iframes (Markdig media embeds too), inline `style` (table cells keep `text-align`), `<style>`, `svg`, `name`, `data-*`, `srcset`, `sizes`, `loading`, `decoding`, and `video`, `audio`, `source`, `track` and `picture`. Unknown elements are unwrapped and their text is kept.
   - Kept: common content markup, `class`, `id` (heading anchors, code language classes), `role`, `aria-*`, table cell `text-align`, and `http`, `https`, `mailto`, `tel` and relative URLs.
   - Extend the allow-list instead of turning sanitization off. `AddOsirionHtmlSanitizer` can be called more than once; every configuration is applied:
     ```csharp
     using Osirion.Blazor.Core.Extensions;

     builder.Services.AddOsirionHtmlSanitizer(options =>
     {
         // A custom element read by your own script, with attributes kept only on that element
         options.AllowElement("chart", "type", "title", "caption", "data", "colors");

         // Opt-ins
         options.AllowedAttributes.Add("style"); // inline styles everywhere, CSS still filtered
         options.AllowElement("iframe", "src", "width", "height", "title", "allow", "allowfullscreen");
         options.AllowElement("input", "type", "checked", "disabled"); // task-list checkboxes
     });
     ```
   - `AllowElement` keeps only the attributes you list, on that element only. List every attribute your client script reads from the element: an attribute that is not listed is removed, and the script then sees no value (for example a chart without its labels, series names or colors). Collect the names from your content before you deploy, and add new ones when your content starts using them.
   - `script`, `svg`, `math`, `template`, `object`, `embed`, `applet`, `meta`, `base`, `link`, `frame`, `frameset`, `noscript`, `on*` attributes and `javascript:`, `vbscript:` and `data:` URLs are never allowed, whatever the options say.
   - **An element you allow is your responsibility.** The sanitizer only makes attribute values inert as HTML. If your JavaScript writes such a value into `innerHTML`, the browser turns `&lt;img onerror=...&gt;` back into markup and runs it. Read the values with `textContent`, `setAttribute` or `createElement`.
   - Set `SanitizeHtml="false"` only for markup your application builds itself, never for repository or user content. A custom `HtmlSanitizer` delegate must escape `<` and `>` inside attribute values.
   - Front matter `json_ld` is re-serialized with `<`, `>` and `&` escaped. JSON-LD that is invalid, or not an object or array, is dropped with a warning and the generated schema is rendered instead.
   - Details: [HTML Sanitization](../src/Osirion.Blazor.Core/README.md#html-sanitization).

5. **Cookie consent: add the antiforgery middleware.** `MapOsirionCookieConsent` now requires an antiforgery token, and the banner form carries one. Call `app.UseAntiforgery()` after `UseAuthentication()` and `UseAuthorization()` (and after `UseRouting()` when you call it), before the endpoints. The Blazor Web App template already does; Razor Pages and MVC hosts that render the banner must add it. Without it, every Accept, Decline or Customize post returns 500 ("contains anti-forgery metadata, but a middleware was not found") and the banner cannot be dismissed.
   - Custom forms that post to the endpoint must include `<AntiforgeryToken />` and post a local path as `returnUrl`. A missing or invalid token gets 400 with no body; a non-local return URL redirects to `/`.
   - Pages that render the banner now carry the antiforgery cookie and `Cache-Control: no-cache, no-store`; review CDN caching of first-visit pages. Persist Data Protection keys across restarts and instances.

6. **Content paths.**
   - FileSystem provider: paths for content save and directory save, move and delete must be relative to `BasePath`. Rooted paths, `.` or `..` segments, and stored `ProviderSpecificId` values outside the root throw `ContentProviderException` (inner `ContentValidationException` for `Path`). An empty `BasePath` fails fast.
   - `CreateContentCommandValidator` and `UpdateContentCommandValidator`: paths that start with `/`, or contain `.`, `..` or empty segments (`a//b.md`, `blog/`) or control characters, are invalid. `v1..2.md` is now valid.
   - GitHub: a path or branch name with an empty, `.` or `..` segment throws `ArgumentException` before any request is sent. Reserved characters are percent-encoded, so `a%41.md` addresses the file of that literal name, and `\` is read as `/`. URLs for plain names are unchanged.
   - `GitHubApiClient.DeleteFileAsync` throws `InvalidOperationException` when the owner, repository or token is missing, instead of sending the request.

7. **API changes for implementers and callers.**
   - `IGitHubTokenProvider`: implement the new `ExchangeCodeForTokenAsync(code, clientId, clientSecret, redirectUri, codeVerifier, cancellationToken)`. The three-argument member now forwards to it. An implementation compiled against an earlier version fails to load with `TypeLoadException`, even without recompiling. Callers are unaffected.
   - `LoginViewModel`: the constructor now also takes `IGitHubTokenProvider`, `IGitHubTokenStore` and `IGitHubOAuthStateStore`. DI resolution is unaffected; `new LoginViewModel(...)` must pass them. `LoginWithGitHubAsync(string code)` is obsolete and always fails; use `StartGitHubLoginAsync()` and `LoginWithGitHubAsync(code, state)`.
   - `IGitHubApiClientFactory.GetClient` and `GetDefaultClient` return a new instance on every call. Code that changed one client with `SetRepository`, `SetBranch` or `SetAccessToken` and expected other callers to see it must configure the client it uses.
   - `IAuthenticationService` and `IGitHubAdminService` are scoped, and the Osirion registration methods remove earlier registrations. Register a custom implementation after the Osirion calls.
   - `IMarkdownProcessor.RenderToHtml` and `RenderToHtmlAsync` default `sanitizeHtml` to `true`; callers compiled against the old default keep `false` until they recompile. Implementers get a new `SanitizeHtml(string)` member whose default implementation HTML-encodes; override it to sanitize.
   - `LoginPage`, `Login`, `Settings`, `AdminLayout`, `AdminPage`, `CmsAdminDashboard` and `CmsLayoutEditor` no longer have a `string Theme` parameter (it made them throw on render, so no host can have passed it successfully). The inherited `ThemeMode Theme` remains.
   - `AuthGuard` shows its content only to users who satisfy `Policy` (default: the admin policy) and needs the cascading authentication state. `ShowLoginForm` and `OnAuthResult`, and `AdminPage.ShowLoginForm`, no longer do anything and are marked `[Obsolete]`: code that sets them still compiles, with a warning (an error where warnings are treated as errors). Remove them.
   - `IAuthenticationService.AuthenticateWithGitHubAsync(string code)` is marked `[Obsolete]` and still works as before. It exchanges the code without a `state` check, a PKCE verifier or a redirect URI, so a host callback built on it accepts a planted `?code=` unless the host checks the state itself. It sends no code verifier, so it is not meant to complete a sign-in started by `LoginViewModel.StartGitHubLoginAsync()`, which always sends a PKCE challenge. Check the state, exchange the code with `IGitHubTokenProvider.ExchangeCodeForTokenAsync(code, clientId, clientSecret, redirectUri, codeVerifier, cancellationToken)`, then call `SetAccessTokenAsync`, as `LoginViewModel.LoginWithGitHubAsync(code, state)` does.
   - The admin event buses (`IEventPublisher`, `IEventSubscriber` and `CmsEventMediator`) are scoped, so an event reaches only subscribers of the same circuit or request. Resolve them inside a scope; resolving them from the root provider fails where scope validation is on (Development).
   - `IThemeService` and `ScrollToTopManager` are scoped instead of singletons, so each circuit or request gets its own. Inject them only into components or scoped services; a singleton that takes either fails where scope validation is on (Development).

8. **Content ids are stable.** Ids used to come from `string.GetHashCode()`, which .NET randomizes per process, so they changed on every restart and differed between server instances. They are now SHA-256 based, 16 lowercase hex characters, and the same everywhere:
   - Content item `Id`: a hash of the item's path and its provider id.
   - Localization id (`ContentId`) when the front matter has no `id`: a hash of the path without the locale segment and the extension, so translations share it.
   - FileSystem directory ids and ids from `CreateDirectoryCommand`: a hash of path, name and provider id, the scheme GitHub directories already used.
   - FileSystem provider id when `ProviderId` is not configured: `filesystem-` and a hash of `BasePath`. Set `FileSystemOptions.ProviderId` if you want a readable, fixed id.

   What to check:
   - Ids the library computes are not persisted, except one: when the admin saves content it writes the localization id into the front matter as `id:`. Files saved that way before this release carry a legacy id of 1 to 8 hex digits. It still works as an explicit id, but translations of the same content that were never saved now compute a different id, so the group splits and the language switcher stops linking them. Find such files with `grep -rEn "^id: ['\"]?[0-9a-f]{1,8}['\"]?\s*$" <content folder>`, then either give every translation of that content the same `id:` (any stable value, for example a slug) or remove the legacy `id:` line from all of them so they compute the same id again. Review matches by hand: an id you chose yourself, such as `42`, matches the pattern too.
   - Hosts that stored content, directory or FileSystem provider ids outside the library (bookmarks, database rows, cache keys) must look them up again. Those values never survived a restart, so nothing that worked before breaks.
   - No data migration runs and none is needed. Rolling back restores the old, per-process ids.

## Upgrading to v1.5.0

### Breaking Changes

Three breaking changes:

1. `osirion-cms.css` has been renamed to `osirion.css` to better reflect its broader purpose as it now includes styles for all components, not just CMS components.

2. Styling options have been moved from `GitHubCmsOptions` to a new dedicated `OsirionStyleOptions` class. If you previously used `GitHubCmsOptions` for styling configuration, you'll need to update your code to use the new class.

3. Service registration extensions have been refactored for better organization and maintainability. The previous extension methods still work but using the new fluent API is recommended.

### New Features

This version introduces CSS framework integration and an improved service registration API:

1. **Framework Integration**: Seamless integration with Bootstrap, Fluent UI, MudBlazor, and Radzen
2. **CssFramework enum**: For specifying which framework to integrate with
3. **Framework-specific CSS variable mappings**: Automatic mapping of Osirion variables to framework variables
4. **Enhanced OsirionStyles component**: Now supports framework integration parameter
5. **New OsirionStyleOptions class**: Dedicated class for styling configuration
6. **Fluent API for service registration**: New `AddOsirionBlazor` method for fluent configuration
7. **Configuration-based registration**: Easily register all services from appsettings.json

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.5.0
   ```

2. Update CSS reference in your `_Host.cshtml` or `App.razor`:
   ```html
   <!-- Before -->
   <link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />
   
   <!-- After -->
   <link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion.css" />
   ```

3. If you previously configured styling in `GitHubCmsOptions`, update to the new `OsirionStyleOptions`:
   ```csharp
   // Before
   builder.Services.Configure<GitHubCmsOptions>(options => {
       options.UseStyles = true;
       options.CustomVariables = "--osirion-primary-color: #0077cc;";
       options.FrameworkIntegration = CssFramework.Bootstrap;
   });
   
   // After
   builder.Services.AddOsirionStyle(options => {
       options.UseStyles = true;
       options.CustomVariables = "--osirion-primary-color: #0077cc;";
       options.FrameworkIntegration = CssFramework.Bootstrap;
   });
   
   // Or even simpler for framework integration
   builder.Services.AddOsirionStyle(CssFramework.Bootstrap);
   ```

4. Consider updating to the new fluent API for service registration:
   ```csharp
   // Before - individual service registration
   builder.Services.AddGitHubCms(options => { ... });
   builder.Services.AddScrollToTop(options => { ... });
   builder.Services.AddClarityTracker(options => { ... });
   
   // After - fluent API
   builder.Services.AddOsirionBlazor(osirion => {
       osirion
           .AddGitHubCms(options => { ... })
           .AddScrollToTop(options => { ... })
           .AddOsirionStyle(CssFramework.Bootstrap)
           .AddClarityTracker(options => { ... });
   });
   
   // Or from configuration
   builder.Services.AddOsirionBlazor(builder.Configuration);
   ```

5. (Optional) Update appsettings.json if used:
   ```json
   // Before
   {
     "GitHubCms": {
       "UseStyles": true,
       "CustomVariables": "...",
       "FrameworkIntegration": "Bootstrap"
     }
   }
   
   // After
   {
     "OsirionStyle": {
       "UseStyles": true,
       "CustomVariables": "...",
       "FrameworkIntegration": "Bootstrap"
     }
   }
   ```

6. (Optional) Apply framework integration via HTML class:
   ```html
   <html class="osirion-bootstrap-integration">
       <!-- Your app content -->
   </html>
   ```

## Upgrading to v1.4.0

### Breaking Changes

None. Version 1.4.0 is fully backward compatible with v1.3.0.

### New Features

This version introduces the ScrollToTop component:

1. **ScrollToTop**: A customizable button component that allows users to quickly scroll back to the top of the page
2. **ButtonPosition enum**: For positioning UI elements (BottomRight, BottomLeft, TopRight, TopLeft)
3. **ScrollToTopOptions**: Configuration options for DI-based setup

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.4.0
   ```

2. Add the ScrollToTop component to your layout:
   ```razor
   @using Osirion.Blazor.Components.Navigation
   
   <ScrollToTop />
   ```

3. (Optional) Configure ScrollToTop via DI in Program.cs:
   ```csharp
   using Osirion.Blazor.Extensions;
   
   builder.Services.AddScrollToTop(options =>
   {
       options.Position = ButtonPosition.BottomRight;
       options.Behavior = ScrollBehavior.Smooth;
       options.VisibilityThreshold = 300;
       options.Text = "Top"; // Optional text label
   });
   ```

4. (Optional) Configure via appsettings.json:
   ```json
   {
     "ScrollToTop": {
       "Position": "BottomRight",
       "Behavior": "Smooth",
       "VisibilityThreshold": 300,
       "Text": "Top",
       "UseStyles": true
     }
   }
   ```

### Configuration Options

The `ScrollToTopOptions` class provides the following configuration options:

```csharp
public class ScrollToTopOptions
{
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Smooth;
    public int VisibilityThreshold { get; set; } = 300;
    public ButtonPosition Position { get; set; } = ButtonPosition.BottomRight;
    public string Title { get; set; } = "Scroll to top";
    public string? Text { get; set; }
    public string? CssClass { get; set; }
    public string? CustomIcon { get; set; }
    public bool UseStyles { get; set; } = true;
    public string? CustomVariables { get; set; }
}
```

### CSS Styling

The ScrollToTop component can be styled using CSS variables:

```css
:root {
  /* Colors */
  --osirion-scroll-background: rgba(0, 0, 0, 0.3);
  --osirion-scroll-hover-background: rgba(0, 0, 0, 0.5);
  --osirion-scroll-color: #ffffff;
  --osirion-scroll-hover-color: #ffffff;
  
  /* Sizes */
  --osirion-scroll-size: 40px;
  --osirion-scroll-margin: 20px;
  --osirion-scroll-border-radius: 4px;
  --osirion-scroll-z-index: 1000;
}
```

## Upgrading to v1.3.0

### Breaking Changes

None. Version 1.3.0 is fully backward compatible with v1.2.0.

### New Features

This version introduces GitHub CMS components for markdown-based content management:

1. **ContentList**: Display lists of content with filtering options
2. **ContentView**: Display a single content item
3. **CategoriesList**: List content categories
4. **TagCloud**: Show content tags
5. **SearchBox**: Provide search functionality
6. **DirectoryNavigation**: Create navigation based on repository structure

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.3.0
   ```

2. Add new using statements to `_Imports.razor`:
   ```razor
   @using Osirion.Blazor.Components.GitHubCms
   @using Osirion.Blazor.Services.GitHub
   @using Osirion.Blazor.Models.Cms
   ```

3. Configure GitHub CMS service in `Program.cs`:
   ```csharp
   builder.Services.AddGitHubCms(options =>
   {
       options.Owner = "your-github-username";
       options.Repository = "your-content-repo";
       options.ContentPath = "content";
       options.Branch = "main";
   });
   ```

4. Add GitHub CMS components to your pages:
   ```razor
   <ContentList Directory="blog" />
   <ContentView Path="blog/my-post.md" />
   <CategoriesList />
   <TagCloud />
   <SearchBox />
   <DirectoryNavigation />
   ```

## Upgrading to v1.2.0

### Breaking Changes

None. Version 1.2.0 is fully backward compatible with v1.1.0.

### New Features

This version introduces analytics components:

1. **ClarityTracker**: Microsoft Clarity integration
2. **MatomoTracker**: Matomo analytics integration
3. **AnalyticsServiceCollectionExtensions**: DI configuration helpers

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.2.0
   ```

2. Add new using statements to `_Imports.razor`:
   ```razor
   @using Osirion.Blazor.Components.Analytics
   @using Osirion.Blazor.Components.Analytics.Options
   ```

3. Configure analytics services in `Program.cs` (optional):
   ```csharp
   builder.Services.AddClarityTracker(builder.Configuration);
   builder.Services.AddMatomoTracker(builder.Configuration);
   ```

4. Add analytics components to your layout:
   ```razor
   @inject IOptions<ClarityOptions>? ClarityOptions
   @inject IOptions<MatomoOptions>? MatomoOptions

   @if (ClarityOptions?.Value is not null)
   {
       <ClarityTracker Options="@ClarityOptions.Value" />
   }

   @if (MatomoOptions?.Value is not null)
   {
       <MatomoTracker Options="@MatomoOptions.Value" />
   }
   ```

## Upgrading from v1.0.0 to v1.1.0

### Breaking Changes

None. Version 1.1.0 is fully backward compatible with v1.0.0.

### New Features

This version introduces the EnhancedNavigationInterceptor component.

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.1.0
   ```

2. Add the navigation component to your layout:
   ```razor
   <EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
   ```