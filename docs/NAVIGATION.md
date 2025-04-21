# Navigation Components

Osirion.Blazor provides navigation components that enhance the user experience during page transitions, particularly in Blazor applications with enhanced navigation enabled.

## Features

- **SSR Compatible**: Works with Server-Side Rendering and Static SSR
- **No JavaScript Interop**: Pure Blazor implementation without JS dependencies
- **Enhanced Navigation Support**: Built for Blazor's enhanced navigation feature
- **Configurable Scroll Behavior**: Choose between different scrolling behaviors

## EnhancedNavigationInterceptor

The EnhancedNavigationInterceptor component automatically scrolls to the top of the page after enhanced navigation completes. This solves the common issue where enhanced navigation preserves scroll position when you want it to reset.

### Usage

```razor
@using Osirion.Blazor.Components.Navigation

<!-- Basic usage with smooth scrolling -->
<EnhancedNavigationInterceptor />

<!-- Customize scroll behavior -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Instant" />

<!-- Available behaviors -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Auto" />    <!-- Browser default -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Instant" /> <!-- Immediate scroll -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />  <!-- Animated scroll -->
```

### Configuration

Place the component in your `App.razor` in `body` just below `_framework/blazor.web.js`:

```razor
<script src="_framework/blazor.web.js"></script>

<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

### How It Works
AddScrollToTopProvider
The component injects a small JavaScript snippet that:
1. Listens for Blazor's `enhancedload` event
2. Compares the current URL with the new URL
3. Scrolls to the top if the URL has changed

This approach ensures compatibility with:
- Server-Side Rendering (SSR)
- Static SSR
- All Blazor hosting models

## ScrollToTop

The ScrollToTop component adds a button that allows users to quickly scroll back to the top of the page when they've scrolled down.

### Usage Options

#### Option 1: Global Integration with ScrollToTopProvider (Recommended)

Add the ScrollToTop button globally by:

1. Configuring the service in Program.cs:
```csharp
// In Program.cs - Basic usage with defaults
builder.Services.AddScrollToTop();

// In Program.cs - With customization
builder.Services.AddScrollToTop(
    position: ButtonPosition.BottomRight,
    behavior: ScrollBehavior.Smooth,
    visibilityThreshold: 300,
    text: "Top"
);

// In Program.cs - With full configuration
builder.Services.AddScrollToTop(manager => {
    manager.Position = ButtonPosition.BottomRight;
    manager.Behavior = ScrollBehavior.Smooth;
    manager.VisibilityThreshold = 300;
    manager.Text = "Back to top";
    manager.CssClass = "custom-scroll";
    manager.CustomIcon = "<svg>...</svg>";
    manager.Title = "Return to top";
});
```

2. Adding the ScrollToTopProvider component to your App.razor or MainLayout:
```razor
@using Osirion.Blazor.Components.Navigation

<ScrollToTopProvider />
```

This approach allows you to configure the ScrollToTop button once in Program.cs while only adding a single line to your markup.

#### Option 2: Manual Component Placement

Add the component explicitly in your layout or pages without using the global manager:

```razor
@using Osirion.Blazor.Components.Navigation

<!-- Basic usage with default settings -->
<ScrollToTop />

<!-- Customize position -->
<ScrollToTop Position="ButtonPosition.BottomLeft" />

<!-- Add text label -->
<ScrollToTop Text="Back to top" />

<!-- Customize behavior -->
<ScrollToTop Behavior="ScrollBehavior.Instant" VisibilityThreshold="500" />

<!-- Custom styling -->
<ScrollToTop CssClass="custom-button" />

<!-- Custom icon -->
<ScrollToTop CustomIcon="<svg>...</svg>" />
```

### Modifying Global ScrollToTop Settings at Runtime

If you need to change the ScrollToTop settings at runtime:

```razor
@page "/settings"
@using Osirion.Blazor.Services
@inject ScrollToTopManager ScrollToTopManager

<h1>ScrollToTop Settings</h1>

<div>
    <label>
        <input type="checkbox" @bind="ScrollToTopManager.IsEnabled" />
        Enable ScrollToTop Button
    </label>
</div>

<div>
    <label>Position:</label>
    <select @bind="ScrollToTopManager.Position">
        <option value="@ButtonPosition.BottomRight">Bottom Right</option>
        <option value="@ButtonPosition.BottomLeft">Bottom Left</option>
        <option value="@ButtonPosition.TopRight">Top Right</option>
        <option value="@ButtonPosition.TopLeft">Top Left</option>
    </select>
</div>

<!-- Other settings... -->
```

### ScrollToTop Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Position | ButtonPosition | BottomRight | Position of the button on the screen |
| Behavior | ScrollBehavior | Smooth | Scrolling animation behavior |
| VisibilityThreshold | int | 300 | Pixels scrolled before button appears |
| Title | string | "Scroll to top" | Button tooltip/title |
| Text | string | null | Optional text to display on button |
| CssClass | string | null | Additional CSS classes |
| CustomIcon | string | null | Custom SVG markup |
| Options | ScrollToTopOptions | null | Direct options object |

### Button Positions

- `ButtonPosition.BottomRight` - Bottom right corner
- `ButtonPosition.BottomLeft` - Bottom left corner
- `ButtonPosition.TopRight` - Top right corner
- `ButtonPosition.TopLeft` - Top left corner

### Styling the ScrollToTop Button

The ScrollToTop button can be styled using CSS variables:

```css
:root {
  /* Colors */
  --osirion-scroll-background: rgba(0, 0, 0, 0.3);
  --osirion-scroll-hover-background: rgba(0, 0, 0, 0.5);
  --osirion-scroll-color: #ffffff;
  --osirion-scroll-hover-color: #ffffff;
  --osirion-scroll-border-color: transparent;
  --osirion-scroll-hover-border-color: transparent;
  
  /* Sizes and spacing */
  --osirion-scroll-size: 40px;
  --osirion-scroll-margin: 20px;
  --osirion-scroll-border-radius: 4px;
  --osirion-scroll-z-index: 1000;
  
  /* Animation */
  --osirion-scroll-transition: all 0.3s ease;
}
```

### Best Practices

1. **Single Instance**: Only include one instance of the component in your application
2. **Layout Placement**: Place the component in your main layout file
3. **Behavior Selection**: Choose the scroll behavior that best fits your application's UX

## Troubleshooting

1. **Components Not Working**: Ensure you're using Blazor with enhanced navigation enabled
2. **Scroll Not Happening**: Check browser console for errors and verify the component is properly placed
3. **Multiple Instances**: Remove duplicate instances of the components
4. **Button Not Appearing**: Check that the VisibilityThreshold value isn't too high for your page content