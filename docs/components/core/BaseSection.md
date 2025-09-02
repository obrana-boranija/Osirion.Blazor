# OsirionBaseSection

Foundational section wrapper with title, description, alignment, background image, pattern, and padding.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionBaseSection Id="features" Title="Features" Description="What you get">
    <p>Content goes here.</p>
</OsirionBaseSection>
```

Notes

- ContainerClass auto-adjusts per CSS framework.
- BackgroundImageUrl, BackgroundColor, TextColor, ShowOverlay.
- Padding: None, Small, Medium, Large. HasDivider optional.
