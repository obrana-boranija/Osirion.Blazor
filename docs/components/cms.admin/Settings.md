# Settings

Purpose
Admin page that shows the CMS state and lets the user pick a theme and sign out. It shows the GitHub sign-in status,
the selected repository and branch, the content settings the application defines (display only), the theme buttons,
a Sign Out button, and an "About Osirion CMS" card with version text and links.

Key parameters
- Route: `/osirion/settings` (no route parameters)
- ThemeChanged: `EventCallback<string>`, invoked with the current theme (`"light"` or `"dark"`) after every render
  except the first, so a click on the Light or Dark button reports the new value.
- Theme is not a parameter here: the page declares `Theme` as a plain `string` property (default `"light"`, reset
  to `"light"` when empty) that hides the `Theme` parameter of `OsirionComponentBase`, which has a different type. The
  code comment says a parameter of that name made Blazor refuse to render the page.
- The page does not apply the chosen theme itself; the host reacts to `ThemeChanged`.

Usage
```razor
<a href="/osirion/settings">Admin settings</a>
```

Notes
- Declares `@rendermode InteractiveServer`, `@layout OsirionAdminLayout` and
  `[Authorize(Policy = CmsAdminAuthorization.PolicyName)]` (policy `Osirion.Cms.Admin`), the last two from the
  `_Imports.razor` of its folder. A host needs an authentication scheme; `AddOsirionCmsAdmin` registers a default policy
  of that name that requires an authenticated user unless the host already defined one.
- Renders `div.container-fluid.py-4` with Bootstrap cards (`card shadow-sm`) in a three column row: GitHub
  Configuration (shows "Authenticated as {username}" or "Not authenticated", the selected repository name, the selected
  branch name), Content Settings, and Theme Settings with Session Settings.
- Content Settings is fixed markup: the extensions `.md` and `.markdown` are shown as allowed, `.yaml`, `.yml` and
  `.json` struck through, and the three switches (file deletion, branch creation, pull requests) are checked and
  disabled. They do not read configuration. The version text and the GitHub, NuGet, documentation and license links in
  the "About" card are also fixed in the markup.
- Sign Out calls `IAuthenticationService.SignOutAsync`, then `IGitHubTokenStore.RemoveTokenAsync`, then navigates to
  `/osirion/login`. When either call throws, the exception is logged and the page shows the fixed message "Signing out
  failed. Please try again." through `CmsState.SetErrorMessage`, never the exception text.
- Injected services: `IAuthenticationService` (admin feature), `IGitHubTokenStore`, `ILogger<Settings>`, and from the
  admin base component `CmsState`, `NavigationManager`. `AddOsirionCmsAdminDI` registers `IGitHubTokenStore` and
  `CmsState`.
- Subscribes to `CmsState.StateChanged` and unsubscribes in `Dispose`.
- `SettingsPage.md` in this folder describes this page (same `ThemeChanged` callback and sign-out behavior). No
  component named `SettingsPage` exists in the source.
