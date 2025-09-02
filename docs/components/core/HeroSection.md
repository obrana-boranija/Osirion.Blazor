# HeroSection

Versatile hero section with background or side images, CTA buttons, metadata, and multiple layout variants. SSR-ready and framework-agnostic.

Basic usage

```razor
@using Osirion.Blazor.Components

<HeroSection 
    Title="Welcome"
    Subtitle="Build faster with Osirion"
    Summary="Modern, SSR-first Blazor components"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/docs"
    SecondaryButtonText="GitHub"
    SecondaryButtonUrl="https://github.com/obrana-boranija/Osirion.Blazor" />
```

Background image

```razor
<HeroSection 
    Title="Transform Your Workflow"
    ImageUrl="/images/hero.jpg"
    UseBackgroundImage="true"
    TextColor="#fff"
    MinHeight="80vh"
    Alignment="Alignment.Center" />
```

Notes

- Variants: Hero, Jumbotron, Minimal.
- Alignment controls text alignment. ImagePosition applies when not using background images.
- Optional metadata: Author, PublishDate, ReadTime, ShowMetadata.
