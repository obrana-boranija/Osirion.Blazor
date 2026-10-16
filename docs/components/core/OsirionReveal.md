# OsirionReveal

Purpose
Wraps content and fades or slides it into view when it first scrolls into the viewport. The animation is a script
enhancement that can be turned off per instance.

Key parameters
- ChildContent: `RenderFragment`, required. The content to reveal
- Animate: bool, default true. Enables the viewport reveal; when false the content is rendered without the hidden
  starting state
- Animation: `OsirionReveal.RevealAnimation` (default `Up`): `Up`, `Down`, `Left`, `Right` or `Fade`
- Theme: `ThemeMode` (default `System`), inherited from `OsirionComponentBase`. `Light`, `Dark` and `Inverted` add a
  `data-theme` attribute (`data-bs-theme` when the Bootstrap framework is configured)
- Class and Style: strings, inherited from `OsirionComponentBase`. The component's markup does not read them
- Attributes: captures unmatched attributes (`CaptureUnmatchedValues`) and renders them on the root element

Usage
```razor
<OsirionReveal Animation="OsirionReveal.RevealAnimation.Left">
    <section>
        <h2>Pricing</h2>
        <p>Content that slides in from the left when it scrolls into view.</p>
    </section>
</OsirionReveal>
```

Notes
- Emits `div.osirion-reveal.osirion-reveal-{animation}` (lower case, for example `osirion-reveal-left`) with
  `data-osirion-reveal` set to the `Animate` value. The component adds no ARIA roles.
- The enum is nested in the component, so outside the component write `OsirionReveal.RevealAnimation.Left`.
- A collocated script (loaded through `BlazorJSComponents`) observes the root element and adds the class
  `osirion-reveal-visible` once at least 12% of it is visible, then stops observing. The stylesheet starts the
  element at `opacity: 0` with a 1.25rem offset (direction depends on `Animation`; `Fade` has no offset) and a 0.55s
  transition when `data-osirion-reveal` is true.
- Reduced motion: both the script (it does not observe) and the stylesheet (opacity 1, no transform, no transition)
  keep the content visible under `prefers-reduced-motion: reduce`.
- With `Animate="false"` the script does nothing and the stylesheet applies no starting state, so the content is
  visible.
