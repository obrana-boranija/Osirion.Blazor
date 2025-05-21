---
title: "EnhancedNavigationInterceptor: Auto-Scrolling for Blazor Navigation"
author: "Dejan Demonjić"
date: "2025-04-15"
description: "Technical implementation of scroll position reset during Blazor navigation using the EnhancedNavigationInterceptor component."
tags: [Blazor, Navigation, Component, .NET]
categories: [Technical, Components]
slug: "enhanced-navigation-interceptor"
is_featured: false
featured_image: "https://images.unsplash.com/photo-1519389950473-47ba0277781c?auto=format&fit=crop&q=80&w=2070&ixlib=rb-4.0.3"
---

# EnhancedNavigationInterceptor: Auto-Scrolling for Blazor Navigation

One common issue with Blazor's enhanced navigation is that it doesn't automatically reset the scroll position when navigating between pages. This can lead to a confusing user experience where users land in the middle of a page after clicking a link.

## The Problem

Blazor's default navigation behavior preserves the scroll position when navigating between pages to optimize the user experience. While this is beneficial for some scenarios, it's often not the expected behavior for most websites.

Consider this scenario:
1. User scrolls down to the bottom of Page A
2. User clicks a link to Page B
3. Page B loads but the scroll position remains at the bottom
4. User needs to manually scroll to the top to see the beginning of Page B

## The Solution: EnhancedNavigationInterceptor

Osirion.Blazor provides the `EnhancedNavigationInterceptor` component that solves this problem elegantly. Here's how it works:

```razor
<EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
```

This component:
1. Listens for Blazor's navigation events
2. Detects when navigation to a new page has completed
3. Automatically scrolls to the top of the page using the specified behavior

## Scroll Behavior Options

The component supports three scroll behaviors:

- `ScrollBehavior.Auto`: Uses the browser's default scrolling behavior
- `ScrollBehavior.Instant`: Jumps immediately to the top without animation
- `ScrollBehavior.Smooth`: Smoothly animates the scroll to the top

## Technical Implementation

Under the hood, the `EnhancedNavigationInterceptor` component injects a small JavaScript snippet that:

1. Listens for Blazor's `enhancedload` event
2. Compares the current URL with the new URL
3. If the URL has changed, it calls `window.scrollTo()` with the appropriate behavior

Here's a simplified version of the implementation:

```csharp
private string GetScript()
{
    return $@"
        <script>
            (function() {{
                let currentUrl = window.location.href;
                Blazor.addEventListener('enhancedload', () => {{
                    let newUrl = window.location.href;
                    if (currentUrl != newUrl) {{
                        window.scrollTo({{ top: 0, left: 0, behavior: '{BehaviorString}' }});
                    }}
                    currentUrl = newUrl;
                }});
            }})();
        </script>
    ";
}
```

## Best Practices

Here are some best practices for using the `EnhancedNavigationInterceptor`:

1. Place it in your `App.razor` or main layout component
2. Use `ScrollBehavior.Smooth` for most websites for better UX
3. Consider using `ScrollBehavior.Instant` for admin dashboards or data-heavy applications
4. Only include one instance of the component in your application

## Conclusion

The `EnhancedNavigationInterceptor` component provides a simple solution to a common Blazor navigation issue. By automatically managing scroll position during navigation, it creates a more intuitive and user-friendly experience.

For more information, check out the [Osirion.Blazor documentation](https://github.com/obrana-boranija/Osirion.Blazor).