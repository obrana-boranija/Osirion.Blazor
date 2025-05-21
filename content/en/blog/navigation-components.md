---
title: "Navigation Components"
author: "Dejan Demonjić"
date: "2025-04-14"
description: "Documentation for navigation components in Osirion.Blazor."
tags: [Navigation, Components, Documentation]
categories: [Documentation]
slug: "navigation-components"
is_featured: false
featured_image: "https://images.unsplash.com/photo-1545987796-200677ee1011?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
---

# Navigation Components

This document provides information about the navigation components available in Osirion.Blazor.

## EnhancedNavigationInterceptor

The `EnhancedNavigationInterceptor` component improves Blazor's navigation by automatically scrolling to the top of the page after navigation.

### Usage

```razor
@using Osirion.Blazor.Components.Navigation

<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

### Parameters

- `Behavior`: Controls the scrolling behavior
  - `ScrollBehavior.Auto`: Browser default
  - `ScrollBehavior.Instant`: Immediate scroll (no animation)
  - `ScrollBehavior.Smooth`: Animated scroll (recommended)

### Best Practices

1. Place the component in your `App.razor` or main layout component
2. Only include one instance in your application
3. Use `ScrollBehavior.Smooth` for most websites
4. Use `ScrollBehavior.Instant` for data-heavy applications or admin dashboards

### How It Works

The component injects a small JavaScript snippet that:
1. Listens for Blazor's `enhancedload` event
2. Detects when navigation has completed
3. Scrolls to the top of the page if the URL has changed

### Example

```razor
@using Osirion.Blazor.Components.Navigation

<Router AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>

<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

## Troubleshooting

### Component Not Working

If the component doesn't work as expected, check:

1. Make sure you have Blazor with enhanced navigation enabled
2. Verify the component is placed in your App.razor or main layout
3. Check for JavaScript errors in the browser console
4. Ensure you only have one instance of the component in your application

### Browser Compatibility

The component should work in all modern browsers. If you encounter issues in specific browsers:

1. For older browsers, use `ScrollBehavior.Instant` instead of `ScrollBehavior.Smooth`
2. Check browser console for any JavaScript errors
3. Verify that the browser supports the Scroll Behavior API

## Conclusion

The `EnhancedNavigationInterceptor` component solves a common issue with Blazor's navigation by providing automatic scrolling to the top of the page. It enhances the user experience and makes navigation feel more natural.

For more information, refer to the [complete documentation](https://github.com/obrana-boranija/Osirion.Blazor).