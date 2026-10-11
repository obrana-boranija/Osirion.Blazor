# OsirionReadingProgress

Purpose
A thin bar at the top of the window that fills as the reader scrolls through the page, for long articles. It is a
CSS scroll-driven animation (`animation-timeline: scroll(root block)`), so it needs no script and costs nothing on
the main thread.

Key parameters
- `Class` and unmatched attributes go on the bar

Usage
```razor
<OsirionReadingProgress />
```

```css
:root {
    --osirion-reading-progress-color: #16a34a;   /* default: --osirion-action-primary */
    --osirion-reading-progress-height: 4px;      /* default: 3px */
}
```

Notes
- Browsers without scroll-driven animations do not show the bar (it is hidden behind
  `@supports (animation-timeline: scroll())`), so it never sits empty or stuck.
- The bar is decorative (`aria-hidden="true"`) and ignores the pointer.
- It tracks the page's root scroller; it does not follow a scrolling container inside the page.
