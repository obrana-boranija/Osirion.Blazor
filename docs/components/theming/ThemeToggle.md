Purpose
-------
Render a theme toggle (light/dark) control and initialize client-side theme JavaScript. Provides accessible markup.

Parameters
----------
- `Id` (optional): id of the button. No id is written by default, so several toggles can share a page.
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
