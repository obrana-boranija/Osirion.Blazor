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

The static `Create404`, `Create403`, `CreateEmptySearch` and `CreateEmptyContent` methods return a component
instance that only carries the preset values. Blazor creates component instances itself, so such an instance cannot be
rendered; set the same parameters in markup instead (see Basic usage). `Create403` leads to the home page.

Notes

- Optional Suggestions, SearchContent, Actions slots.
- BackgroundPattern, Variant, Size options.
