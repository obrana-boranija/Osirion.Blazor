# NotFoundPage

Purpose
A whole "page not found" page that asks search engines not to index it (`<meta name="robots" content="noindex">`) and
offers a way home, an optional search box and suggested pages. It renders without script.

Usage
```razor
@* .NET 10: Routes.razor *@
<Router AppAssembly="typeof(Program).Assembly" NotFoundPage="typeof(NotFound)">

@* NotFound.razor *@
@page "/not-found"
<NotFoundPage Suggestions="@suggestions" ShowSearch />
```

Key parameters
- `Title` (default `Page not found`, the `h1` and title tag), `Message`
- `HomeUrl` and `HomeText`
- `Suggestions` (`ContentNotFoundSuggestion` items) and `SuggestionsTitle`; the list is shown only when it has items
- `ShowSearch` (false), `SearchUrl` (default `/search`), `SearchPlaceholder`
- `ChildContent` below

Status code
- .NET 10: the router answers 404 when it shows its `NotFoundPage`. A request to the page's own route answers 200;
  the noindex tag keeps that page out of search results.
- .NET 9: the page sets status 404 itself under static server-side rendering, for use inside the router's `NotFound`
  template.
