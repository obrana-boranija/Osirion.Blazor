---
title: "Privacy-Aware Analytics in Osirion"
author: "Osirion Team"
date: "2025-01-25"
description: "Add Clarity, GA4, Matomo, and Yandex Metrica with consent-aware, SSR-friendly components."
slug: "osirion-analytics"
categories: [Analytics]
tags: [Analytics, GDPR, Consent]
featured_image: "/images/docs/analytics.jpg"
seo_properties:
  title: "Analytics Integration in Osirion.Blazor"
  description: "Implement consent-gated analytics with Osirion.Blazor."
  og_image_url: "/images/social/analytics.jpg"
  type: "Article"
---

# Privacy-Aware Analytics in Osirion

Enable analytics while respecting user privacy and SSR constraints.

## Consent gating

```razor
@if (await ConsentService.HasConsentAsync("analytics"))
{
  <ClarityTracker SiteId="your-id" />
}
```

## Recommended settings

- Anonymize IP when available
- Defer loading until consent is present
- Document your data collection in Privacy Policy
