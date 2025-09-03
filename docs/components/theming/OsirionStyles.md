Purpose
-------
Injects the core and theme styles into the page and optionally emits CSS custom properties from configuration.

Parameters
----------
- `CustomVariables` (string) — Raw CSS variables to include inside a `<style>` tag.
- `GeneratedVariables` (string) — CSS variables generated at runtime.

Example
-------
```razor
<OsirionStyles />
```

Notes
-----
- Loads framework and theme stylesheets. For advanced scenarios, configure `ThemingOptions` and `IThemeService`.
