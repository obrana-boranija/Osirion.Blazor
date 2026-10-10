# OsirionCookieConsent

Purpose
GDPR consent banner with optional categories and a customization panel. SSR-compatible.

Key parameters
- Title, Message
- AcceptButtonText, DeclineButtonText, CustomizeButtonText, SavePreferencesButtonText
- ShowDeclineButton, ShowCustomizeButton, ShowCustomizationPanel
- CustomizePanelTitle
- PolicyLink, PolicyLinkText
- Categories: IReadOnlyList<CookieCategory> (defaults: necessary, analytics, marketing, preferences)
- Position: bottom or top
- ConsentExpiryDays (default 365)
- ConsentEndpoint (server endpoint to persist preferences)

Behavior
- Reads consent cookie "osirion_cookie_consent"; shows banner when absent
- Supports "?customize-cookies" query to open customization directly
- Renders an antiforgery token inside the form and posts the current path and query (never the absolute URL) as returnUrl

Notes
- Emits osirion-cookie-consent-{position} and -customizing classes
- Map the server endpoint with MapOsirionCookieConsent and call UseAntiforgery; the endpoint rejects posts without a valid token and only redirects to local paths. See CookieConsent.md.
