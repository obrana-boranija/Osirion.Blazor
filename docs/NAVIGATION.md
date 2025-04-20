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

The component injects a small JavaScript snippet that:
1. Listens for Blazor's `enhancedload` event
2. Compares the current URL with the new URL
3. Scrolls to the top if the URL has changed

This approach ensures compatibility with:
- Server-Side Rendering (SSR)
- Static SSR
- All Blazor hosting models

### Best Practices

1. **Single Instance**: Only include one instance of the component in your application
2. **Layout Placement**: Place the component in your main layout file
3. **Behavior Selection**: Choose the scroll behavior that best fits your application's UX

### ScrollBehavior Enum

- `Auto`: Uses the browser's default scroll behavior
- `Instant`: Scrolls immediately without animation
- `Smooth`: Provides animated scrolling for a better user experience

### Example Scenarios

#### E-commerce Website
```razor
<!-- Use smooth scrolling for a polished feel -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

#### Documentation Site
```razor
<!-- Use instant scrolling for immediate feedback -->
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Instant" />
```

#### Mixed Application
```razor
@code {
    private ScrollBehavior currentBehavior = ScrollBehavior.Auto;

    private void SetBehavior(ScrollBehavior behavior)
    {
        currentBehavior = behavior;
    }
}

<EnhancedNavigationInterceptor Behavior="@currentBehavior" />
```

## Troubleshooting

1. **Component Not Working**: Ensure you're using Blazor with enhanced navigation enabled
2. **Scroll Not Happening**: Check browser console for errors and verify the component is properly placed
3. **Multiple Instances**: Remove duplicate instances of the component