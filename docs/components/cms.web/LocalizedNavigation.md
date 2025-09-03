Purpose
-------
Render navigation for localized documentation directories. Shows a locale selector (optional) and lists directories for the selected locale.

Parameters
----------
- `Title` (string) — Optional navigation title.
- `IsLoading` (bool) — Show loading state.
- `Directories` (IEnumerable&lt;DirectoryModel&gt;) — The directories to render.
- `ShowLocaleSelector` (bool) — When true, render locale selector buttons.
- `AvailableLocales` (IEnumerable&lt;string&gt;) — Locales available for selection.
- `ShowLocaleSelector` (bool) — Default: false.
- `LoadingText` / `NoContentText` — Text for loading and empty states.

Example
-------
```razor
<LocalizedNavigation Title="Docs" Directories="@dirs" ShowLocaleSelector="true" />
```

Notes
-----
- The component exposes `SwitchLocale` callbacks to change the active locale.
