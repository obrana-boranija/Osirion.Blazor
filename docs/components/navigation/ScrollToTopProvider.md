# ScrollToTopProvider

Purpose
Renders the `ScrollToTop` button from the settings held by the registered `ScrollToTopManager`, so the button is
configured once (in `Program.cs` or configuration) instead of per page. It re-renders when the manager's settings
change.

Key parameters
- None of its own. The component declares no parameters; all settings come from `ScrollToTopManager`:
  `IsEnabled`, `Position`, `Behavior`, `VisibilityThreshold`, `Text`, `Title`, `CssClass`, `CustomIcon` and
  `IconCssClass`.
- Class, Style, Theme and Attributes are inherited from `OsirionComponentBase`, but this component's markup uses none
  of them (the inner button gets `Manager.CssClass` as its class).

Usage
```razor
@inherits LayoutComponentBase

@Body

<ScrollToTopProvider />
```

Configure the settings once at startup, with the navigation builder method `AddScrollToTop`:

```csharp
builder.Services.AddOsirionNavigation(navigation => navigation.AddScrollToTop(options =>
{
    options.Position = Position.BottomLeft;
    options.Text = "Top";
}));
```

Notes
- Injects the scoped `ScrollToTopManager`. The navigation builder registers it from the `ScrollToTopOptions` given to
  `AddScrollToTop`, with `IsEnabled` taken from `Enabled` (default `true` in `ScrollToTopOptions`).
- Renders nothing while `Manager.IsEnabled` is `false`. When it is `true`, it renders one `ScrollToTop` with the
  manager's `Position`, `Behavior`, `VisibilityThreshold`, `Text`, `Title`, `CustomIcon` and `IconCssClass`, and
  `Manager.CssClass` as its `Class`.
- The button is `button.osirion-scroll-to-top` with a position class (`top-left`, `top-right`, `bottom-left` or
  `bottom-right`), `type="button"`, an `aria-label` from the title, and `style="display: none;"` until the script
  shows it. An optional `span.scroll-text` holds `Text`.
- `ScrollToTop` treats a value equal to its own default (`BottomRight`, `Smooth`, `300`, `"Scroll to top"`) as unset and
  then uses the registered `ScrollToTopOptions` value. It also checks `ScrollToTopOptions.Enabled`, so the button
  shows only if both that option and `Manager.IsEnabled` allow it.
- It subscribes to `ScrollToTopManager.ConfigurationChanged` when initialized and re-renders through
  `InvokeAsync(StateHasChanged)`. `Dispose` unsubscribes.
- Works in static SSR: `ScrollToTop` writes an inline `<script>` that shows the button when `window.scrollY` is
  greater than the threshold and scrolls to the top on click. The visibility logic runs in the browser, not through a
  Blazor interactive render mode.
- `OsirionSiteLayout` renders `<ScrollToTopProvider />` when `ShowScrollToTop` is `true` (the default).
