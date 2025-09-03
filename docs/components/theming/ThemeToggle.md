Purpose
-------
Render a theme toggle (light/dark) control and initialize client-side theme JavaScript. Provides accessible markup and a non-JS fallback.

Parameters
----------
- No direct component parameters — state and configuration come from `IThemeService` and `ThemingOptions`.

Example
-------
```razor
<ThemeToggle />
```

Notes
-----
- The component wires client-side scripts that handle theme initialization and toggling. It also renders a hidden form fallback for environments without JavaScript.
- For customization, update `ThemingOptions` or the theme service implementation.
