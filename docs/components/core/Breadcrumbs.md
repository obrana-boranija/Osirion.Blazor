# OsirionBreadcrumbs

A semantic, SSR-friendly breadcrumb generator from URL paths or custom strings. Supports custom formatting, home link, and flexible URL generation.

Basic usage

```razor
@using Osirion.Blazor.Components
@inject NavigationManager Nav

<!-- From current URL -->
<OsirionBreadcrumbs Path="@new Uri(Nav.Uri).AbsolutePath" />

<!-- Custom path -->
<OsirionBreadcrumbs Path="/docs/components/navigation/breadcrumbs" />
```

Key parameters

- Path: string. Source path to parse.
- ShowHome: bool = true. Show home item.
- HomeText: string = "Home".
- HomeUrl: string = "/".
- LinkLastItem: bool = false.
- UrlPrefix: string = "/". Prefix for generated links.
- SegmentFormatter: Func<string, string>? to format segment labels.

Custom formatting example

```razor
<OsirionBreadcrumbs 
    Path="/blog/web-development/blazor-components"
    UrlPrefix="/blog/"
    SegmentFormatter="@(s => string.Join(' ', s.Split('-').Select(p => char.ToUpper(p[0]) + p[1..])))" />
```

Styling

The root element renders nav.osirion-breadcrumbs with framework-friendly classes. Style via your CSS framework or custom CSS.
