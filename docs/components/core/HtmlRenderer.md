# OsirionHtmlRenderer

Renders trusted HTML with optional syntax highlighting, line numbers, copy button, and accessible theme.

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

- Set SanitizeHtml or provide HtmlSanitizer delegate for untrusted content.
