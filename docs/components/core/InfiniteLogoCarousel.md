# InfiniteLogoCarousel

Purpose
Showcase partner/client logos in an infinite, CSS-only marquee with optional grayscale and direction.

Key parameters
- Title, SectionTitle (aria-label)
- CustomLogos: List<LogoItem> with ImageUrl, AltText, optional LightImageUrl/DarkImageUrl, Url, Target, rel flags
- AnimationDuration (seconds), Direction (Left/Right), PauseOnHover
- EnableGrayscale (default true)
- LogoWidth, LogoHeight, LogoGap
- MaxVisibleLogos (performance)

Notes
- Emits osirion-infinite-carousel plus modifiers for grayscale, direction and hover pause
- Provide dual logos for light/dark themes using LightImageUrl/DarkImageUrl
- Link attributes default to nofollow and noopener for external URLs.
