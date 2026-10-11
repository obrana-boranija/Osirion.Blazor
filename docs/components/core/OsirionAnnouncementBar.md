# OsirionAnnouncementBar

Purpose
A site-wide announcement above the page that visitors can close. Closing posts a small form; the endpoint remembers
the choice in a cookie and redirects back, so it works without script. The bar stays hidden until the cookie
expires or the announcement gets a new `Id`.

Setup
```csharp
app.UseAntiforgery();
app.MapOsirionAnnouncements();   // POST /api/announcement/dismiss
```

Key parameters
- `Id` (required): 1 to 40 lowercase letters, digits or hyphens, such as `release-4-0`; a dismissal is remembered
  per ID
- `ChildContent`: the message, which may contain links
- `Dismissible`: show the close button (default true)
- `RememberDays`: how long a dismissal lasts, 1 to 365 (default 30)
- `DismissText`: the close button's accessible name (default "Dismiss announcement")
- `RegionLabel`: the region's accessible name (default "Announcement")
- `DismissEndpoint`: where the form posts; match the pattern passed to `MapOsirionAnnouncements`

Usage
```razor
<OsirionAnnouncementBar Id="release-4-0">
    Osirion.Blazor 4.0 is out. <a href="/blog/release-4-0">Read what's new</a>
</OsirionAnnouncementBar>
```

Notes
- The cookie (`osirion_announcement_{Id}`) is read from the request, so the bar renders under static server
  rendering and while prerendering. In an interactive render without an HTTP request it renders nothing, rather than
  show a dismissed announcement again; place it in a statically rendered layout.
- The endpoint requires an antiforgery token, accepts only valid IDs, bounds the lifetime and redirects only to local
  paths. The cookie is HttpOnly and SameSite=Lax, and Secure when the request is HTTPS.
- Text on the bar uses `--osirion-action-primary` and `--osirion-action-primary-text`.
