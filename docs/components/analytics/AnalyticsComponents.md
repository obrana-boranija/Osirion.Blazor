# Analytics Components

Overview
Use provider-specific components to inject tracking tags. Services configure runtime provider selection.

Components
- GA4Tracker: Google Analytics 4
- MatomoTracker: Matomo
- YandexMetricaTracker: Yandex Metrica
- ClarityTracker: Microsoft Clarity
- AnalyticsTracker: generic wrapper that renders active provider

Common options
- Respect DoNotTrack, anonymize IP where supported
- Load only on Production

Notes
- Configure with AddOsirionAnalytics() and provider options. Components are markup-only and SSR-safe.
