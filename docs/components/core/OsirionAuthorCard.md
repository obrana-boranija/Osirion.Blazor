# OsirionAuthorCard

Purpose
Introduces an author: avatar (or initials), name, role, short bio and links. Use it under an article or at the top
of an author's page. It renders without script.

Key parameters
- `Name` (required)
- `Role`, `Bio`: plain text
- `AvatarUrl`: decorative image next to the name (`alt=""`); without it the initials are shown
- `ProfileUrl`: the author's page; the name links there with `rel="author"`
- `Links`: `IReadOnlyList<AuthorLink>` (`AuthorLink(Label, Url)` in `Osirion.Blazor.Core.Models`), rendered with
  `rel="me"`
- `Class` and unmatched attributes go on the card

Usage
```razor
<OsirionAuthorCard Name="Jane Developer"
                   Role="Principal engineer"
                   Bio="Writes about Blazor and the web platform."
                   AvatarUrl="/img/authors/jane.jpg"
                   ProfileUrl="/authors/jane-developer"
                   Links="@([new AuthorLink("GitHub", "https://github.com/jane"), new AuthorLink("Mastodon", "https://mastodon.social/@jane")])" />
```

Notes
- Only http(s), `mailto:` and site-relative URLs are rendered; others (such as `javascript:`) are left out, so author
  details taken from content cannot inject script.
- For an author's page, combine it with `ContentList Author="Jane Developer"` (Osirion.Blazor.Cms.Web) and link
  article bylines there with `ContentView AuthorUrlFormatter`. See "Author pages" in GITHUB_CMS.md.
