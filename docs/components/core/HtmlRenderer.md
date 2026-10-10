# OsirionHtmlRenderer

Renders HTML, sanitized by default, with optional syntax highlighting, line numbers, copy button, and accessible theme.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionHtmlRenderer HtmlContent="@html" />

@code {
    private string html = "<h3>Intro</h3><p>Text</p>";
}
```

Code blocks

```razor
<OsirionHtmlRenderer 
    HtmlContent='@"<pre><code class=\"language-csharp\">Console.WriteLine(\"Hi\");</code></pre>"'
    EnableSyntaxHighlighting="true"
    ShowLineNumbers="true"
    EnableCopyButton="true"
    UseAccessibleTheme="true" />
```

Security

- SanitizeHtml defaults to true: the registered IHtmlContentSanitizer (or the default allow-list) removes script,
  event handlers, unsafe URLs, forms, iframes and styles before rendering.
- Provide an HtmlSanitizer delegate to replace it, or set SanitizeHtml="false" only for markup your application built.
  A delegate must escape < and > inside attribute values, as the default sanitizer does, because the copy button
  wrapper and line numbers edit the sanitized markup with text patterns.

```csharp
// Program.cs: allow a custom element and every attribute your script reads from it
builder.Services.AddOsirionHtmlSanitizer(options =>
    options.AllowElement("chart", "type", "title", "caption", "data", "colors"));
```

The script that reads those attributes must treat them as text (`textContent`, `setAttribute`) and never write
them through `innerHTML`: the browser decodes an escaped value back into markup.
