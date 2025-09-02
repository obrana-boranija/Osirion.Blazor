# OsirionArticleMetdata

Lightweight article metadata renderer for author, publish date, and read time.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionArticleMetdata 
    Author="John Smith" 
    PublishDate="@DateTime.UtcNow.Date" 
    ReadTime="6 min" />
```

Note

- DateFormat controls date rendering (default "d").
