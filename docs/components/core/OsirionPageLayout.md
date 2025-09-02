# OsirionPageLayout

Purpose
Three-slot page layout (Header, Body, Footer) with optional sticky footer and min-height strategies.

Key parameters
- Header, Body, Footer (RenderFragment)
- StickyFooter (default true)
- MinHeightStrategy: "viewport" or "content"

Notes
- Emits osirion-page-layout, osirion-sticky-footer-layout, osirion-min-height-{strategy}
- Use for shell layouts where footer should remain at bottom on short pages.
