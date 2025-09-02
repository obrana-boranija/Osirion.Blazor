# OsirionResponsiveShowcaseSection

Responsive preview wrapper to demonstrate components in desktop, tablet, and mobile widths. Works without JavaScript for basic scenarios.

Basic usage

```razor
@using Osirion.Blazor.Core.Components.Sections

<OsirionResponsiveShowcaseSection Title="Preview">
    <ChildContent>
        <!-- Place any component here -->
        <p class="lead">Hello</p>
    </ChildContent>
</OsirionResponsiveShowcaseSection>
```

Key parameters

- InitialViewport: Desktop|Tablet|Mobile.
- ShowDimensions: bool. Show size hints.
- ShowBrowserChrome: bool. Render chrome bar.
- ShowCode, CodeSnippet: optional code area.
