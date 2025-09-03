Purpose
-------
Render Google Analytics 4 (GA4) script when a configured GA4 provider is enabled.

Parameters
----------
No component-level parameters. Configure GA4 options via the `GA4Provider` registered in analytics services.

Example
-------
```razor
<GA4Tracker />
```

Notes
-----
- See `src/Osirion.Blazor.Analytics/Providers/GA4Provider.cs` for configuration fields.
- This component inherits common behavior from the analytics base tracker.
