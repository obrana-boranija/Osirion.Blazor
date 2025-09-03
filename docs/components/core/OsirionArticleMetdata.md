Purpose
-------
Render author, publish date and read time metadata for articles. Compact UI for article header metadata.

Parameters
----------
- `Author` (string)
- `PublishDate` (DateTime?)
- `ReadTime` (string) — Read time in minutes.

Example
-------
```razor
<OsirionArticleMetdata Author="John" PublishDate="@DateTime.Now" ReadTime="5" />
```

Notes
-----
- The component only renders when at least one field is present.
