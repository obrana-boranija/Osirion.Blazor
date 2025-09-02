# OsirionContentNotFound

Configurable empty-state component for 404, 403, empty search, or missing content.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionContentNotFound 
    ErrorCode="404"
    Title="Page Not Found"
    Message="The page you requested does not exist."
    PrimaryButtonText="Go Home"
    PrimaryButtonUrl="/" />
```

Presets

```razor
@* Static helpers *@
@* OsirionContentNotFound.Create404(); OsirionContentNotFound.Create403(); *@
```

Notes

- Optional Suggestions, SearchContent, Actions slots.
- BackgroundPattern, Variant, Size options.
