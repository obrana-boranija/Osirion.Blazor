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

Register the services and map the endpoint. The endpoint requires a valid antiforgery token, so the app must also call `UseAntiforgery()` after `UseRouting()` (when the app calls it) and before the endpoints, as the Blazor Web App template does. Registering the antiforgery services does not add the middleware. Without it, every post to the endpoint fails with `InvalidOperationException` ("contains anti-forgery metadata, but a middleware was not found that supports anti-forgery") and a 500 response.

```csharp
builder.Services.AddRazorComponents();
builder.Services.AddOsirionCookieConsent(); // also registers antiforgery services

var app = builder.Build();

app.UseAntiforgery();
app.MapOsirionCookieConsent(); // POST /api/cookie-consent
```

The banner posts to ConsentEndpoint (default /api/cookie-consent) with an antiforgery token and the current page as a local path and query. The endpoint:

- returns 400 without a body, and records nothing, when the antiforgery token is missing or invalid or the body is not a form post;
- writes the consent cookie and redirects back to the return URL only when it is a local path such as `/blog/post?page=2`; absolute, protocol-relative (`//host`) and other non-local values redirect to `/`;
- for Customize, redirects back with a single `customize-cookies=true` query flag, and removes a leftover flag after consent is saved.

If you post to the endpoint from your own markup, include `<AntiforgeryToken />` inside the form and post a local path as `returnUrl`.
