# GA4Tracker

Renders Google Analytics 4 tag. Configure via GA4Options (MeasurementId, SendPageView, etc.).

# MatomoTracker

Renders Matomo script and configuration. Configure via MatomoOptions (SiteId, EndpointUrl, etc.).

# YandexMetricaTracker

Renders Yandex Metrica tag. Configure via YandexMetricaOptions.

# ClarityTracker

Renders Microsoft Clarity tag. Configure via ClarityOptions (ProjectId, etc.).

# AnalyticsTracker

Generic wrapper that selects the configured provider and renders its tracker.

Usage
- Register AddOsirionAnalytics in Program.cs
- Configure desired provider and options via configuration or code
- Place tracker component in MainLayout or document head/body per provider requirements.
