# OsirionFeatureCard

Flexible feature card with image, title, description, read-more link, and pillow buttons. Rich visual controls for borders, shadows, transforms.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionFeatureCard 
    Title="Analytics"
    Description="Real-time insights"
    ImageUrl="/img/analytics.png"
    ShowReadMoreLink="true"
    ReadMoreHref="/features/analytics" />
```

Layout and effects

- ImagePosition: Top|Right|Bottom|Left.
- CardSize: XXS..XXL.
- ContentAlignment: grid-like alignment options.
- BorderTiming, ShadowTiming, TransformTiming with ThemeColor accents.
