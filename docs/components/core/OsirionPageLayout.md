# OsirionPageLayout

Purpose
Three-slot page layout (Header, Body, Footer) with optional sticky footer and min-height strategies.

Key parameters
- Header, Body, Footer (RenderFragment)
- StickyFooter (default true)
- MinHeightStrategy: "viewport" or "content"
- EnableHeaderScrollBehavior (default true): hides the header while scrolling down in interactive modes
- ShowSkipLink (default true), SkipLinkText (default "Skip to main content"), MainContentId (default "osirion-main-content")

Notes
- Emits osirion-page-layout, osirion-sticky-footer-layout, osirion-min-height-{strategy}
- Use for shell layouts where footer should remain at bottom on short pages.
- Renders a skip link as the first focusable element. It stays off screen until it receives keyboard focus and moves focus to the main element, which carries `MainContentId` and `tabindex="-1"`. Its address is the current page plus the fragment, because a bare `#id` resolves against `<base href="/">`.
