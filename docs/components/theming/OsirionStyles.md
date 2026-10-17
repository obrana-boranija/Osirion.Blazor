Purpose
-------
Injects the core and theme styles into the page and optionally emits CSS custom properties from configuration.

Parameters
----------
- `CustomVariables` (string): CSS variables to include inside a `<style>` tag. Every `<` is replaced by the CSS escape `\3C `, so a value cannot end the element.
- `GeneratedVariables` (string): CSS variables generated at runtime, written the same way.

Example
-------
```razor
<OsirionStyles />
```

Notes
-----
- Loads framework and theme stylesheets. For advanced scenarios, configure `ThemingOptions` and `IThemeService`.
