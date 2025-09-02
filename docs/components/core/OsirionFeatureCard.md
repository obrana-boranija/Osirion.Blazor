# OsirionFeatureCard

Purpose
Feature card with image, headings, description, read-more and category-style pillow buttons. Highly themeable.

Key parameters
- Title, Description, ShowDescription, ChildContent
- Url (card-level link)
- ImageUrl, ImageAlt, ImageLazyLoading, ImageTransformOnHover, ImagePosition (Top, Bottom, Left, Right)
- CardSize (XXS..XXL), ContentAlignment, ContentFontSize
- BorderTiming, BorderColor; ShadowTiming, ShadowColor; TransformTiming
- ShowReadMoreLink, ReadMoreText, ReadMoreHref, ReadMoreStretched
- ShowPillowButtons, PillowButtons

Notes
- Emits osirion-feature-card plus many BEM modifiers for size/effects
- ReadMoreHref takes precedence over Url for the CTA
- Provides sensible image placeholders when ImageUrl is missing.
