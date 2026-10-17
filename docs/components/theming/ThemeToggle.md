Purpose
-------
Render a theme toggle (light/dark) control and initialize client-side theme JavaScript. Provides accessible markup.

Parameters
----------
- `Id` (optional): id of the button. No id is written by default, so several toggles can share a page.
- `OnThemeChanged` (optional): raised with `light` or `dark` after a switch, only when the toggle is interactive. Under Static SSR it is never raised; listen for the `osirion-theme-update` browser event instead.
- State and configuration come from `IThemeService` and `ThemingOptions`.

Example
-------
```razor
<ThemeToggle />
```

Notes
-----
- The component loads a script file that handles toggling through a click listener; it writes no inline script and no `onclick` attribute, so a strict Content Security Policy does not block it. The button needs JavaScript; there is no server-side fallback form.
- For customization, update `ThemingOptions` or the theme service implementation.
