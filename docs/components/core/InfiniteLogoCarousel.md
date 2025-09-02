# InfiniteLogoCarousel

CSS-only continuous logo carousel. SSR-safe, no JavaScript required. Supports dual light/dark logos, pause on hover, and performance controls.

Basic usage

```razor
@using Osirion.Blazor.Components

<InfiniteLogoCarousel Title="Our Partners" />
```

Custom logos

```razor
<InfiniteLogoCarousel 
    Title="Technology Stack"
    AnimationDuration="45"
    Direction="AnimationDirection.Left"
    CustomLogos="@logos" />

@code {
    private List<LogoItem> logos = new()
    {
        new("/img/a.svg", "Brand A", Url: "https://a.example"),
        new("/img/b.svg", "Brand B", Url: "https://b.example", EnableGrayscale: false)
    };
}
```

Key parameters

- AnimationDuration: int = 60.
- Direction: AnimationDirection (Right|Left).
- PauseOnHover: bool = true.
- LogoWidth, LogoHeight, LogoGap: sizing.
- MaxVisibleLogos: int? limit for performance.
- CustomLogos: List<LogoItem>.
