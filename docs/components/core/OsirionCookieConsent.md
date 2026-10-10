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
- ConsentExpiryDays (default 365, posted with the form; the endpoint accepts 1 to 3650)
- ConsentEndpoint (server endpoint to persist preferences)

Behavior
- Reads consent cookie "osirion_cookie_consent"; shows banner when absent
- Supports "?customize-cookies" query to open customization directly
- Renders an antiforgery token inside the form and posts the current path and query (never the absolute URL) as returnUrl

Rendering modes
- The banner needs the HTTP request (static server rendering or prerendering). Without an HttpContext, for example under WebAssembly or in an interactive server circuit, it renders nothing and does not throw

Notes
- Emits osirion-cookie-consent-{position} and -customizing classes
- Map the server endpoint with MapOsirionCookieConsent and call UseAntiforgery; the endpoint rejects posts without a valid token and only redirects to local paths. See CookieConsent.md.
