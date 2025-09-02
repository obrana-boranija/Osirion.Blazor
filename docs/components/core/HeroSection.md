# HeroSection

Purpose
A flexible hero/jumbotron section with optional image (background or side), metadata, and call-to-action buttons. SSR-compatible and framework-agnostic.

Key parameters
- Title, Subtitle, Summary or TitleContent/SubtitleContent/SummaryContent
- ImageUrl, ImageAlt, UseBackgroundImage
- BackgroundPattern, BackgroundColor, TextColor
- Alignment (Left, Center, Right, Justify), ImagePosition (Left, Right)
- Variant: Hero, Jumbotron, Minimal
- ShowPrimaryButton/Secondary + PrimaryButtonText/Url, SecondaryButtonText/Url
- MinHeight (default 60vh), HasDivider
- ShowMetadata + Author, PublishDate, ReadTime

Usage
- Background image: set ImageUrl and UseBackgroundImage=true
- Side image: set ImageUrl and UseBackgroundImage=false; choose ImagePosition
- Provide Title/Subtitle/Summary or render fragments for full control

Notes
- CSS classes emitted: osirion-hero-section, -variant-{variant}, -align-{alignment}, -with-background or -with-side-image, -image-{left|right}
- Works without JS; supply accessible alt text and use metadata when hero represents article header.
