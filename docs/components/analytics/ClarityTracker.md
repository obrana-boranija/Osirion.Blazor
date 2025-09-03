Purpose
-------
Render Clarity analytics tracking script when a configured Clarity provider is enabled.

Parameters
----------
This component inherits behavior from the analytics base tracker and does not accept component-level parameters — configuration is provided via registered analytics providers.

Example
-------
```razor
<ClarityTracker />
```

Notes
-----
- The actual script and whether it renders is determined by the registered `ClarityProvider` (implements `IAnalyticsProvider`).
- See `src/Osirion.Blazor.Analytics/Providers/ClarityProvider.cs` for provider options.
