# OsirionCookieConsent

GDPR-ready cookie consent banner with categories, form-based submission, and SSR support.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionCookieConsent />
```

Customized

```razor
<OsirionCookieConsent 
    Title="Privacy Preferences"
    Message="We use cookies to improve your experience."
    PolicyLink="/privacy"
    PolicyLinkText="Privacy Policy"
    ShowCustomizeButton="true"
    Categories="@Categories" />

@code {
    private static readonly List<CookieCategory> Categories = new()
    {
        new() { Id = "necessary", Name = "Necessary", Description = "Required", IsRequired = true, IsEnabled = true },
        new() { Id = "analytics", Name = "Analytics", Description = "Usage metrics" }
    };
}
```

Server endpoint

Post preferences to ConsentEndpoint (default /api/cookie-consent). Set a consent cookie and redirect back to ReturnUrl.
