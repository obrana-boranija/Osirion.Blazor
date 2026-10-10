# SettingsPage

Purpose
Administration settings page for configuring repository adapters and app options.

Parameters
- `ThemeChanged` (`EventCallback<string>`): raised with the theme chosen on the page. `Theme` itself is page state, not a parameter: a parameter of that name clashed with the `Theme` parameter of `OsirionComponentBase` and stopped the page from rendering.

Sign out
- The Sign Out button signs the user out of GitHub, drops the user's token from the admin services of the circuit and removes the token kept in protected browser storage, then opens `/osirion/login`.
- It works in hosts that make only the admin pages interactive, where the static admin layout cannot show its own sign-out control.
- When sign-out fails, the page shows a fixed message and the exception is logged, never shown.

Example

Composed of forms and toggles for feature flags, repository settings, and integrations.
