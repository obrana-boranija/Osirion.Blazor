# OsirionHtmlRenderer

Purpose
Render HTML content with optional Prism-based code highlighting, copy button, and line highlighting.

Key parameters
- HtmlContent (string)
- EnableSyntaxHighlighting (default true)
- ShowLineNumbers, EnableCopyButton, EnableLineHighlighting
- UseAccessibleTheme (WCAG-friendly)
- LanguageClasses (per-language custom CSS classes)
- SanitizeHtml (default true): sanitize with the registered IHtmlContentSanitizer allow-list
- HtmlSanitizer (optional delegate that replaces the registered sanitizer)

Implementation notes
- Detects <pre><code> blocks and reads their language- class (on the code or pre element) to load the matching
  Prism language; the block markup itself is not rewritten
- Wraps blocks with header and copy button when enabled
- Adds line-numbers to <pre> if requested
- Provides initialization JS snippet with retry

Security
- Content is sanitized by default with an allow-list HTML sanitizer: script, event handlers, javascript:/data: URLs,
  forms, iframes, inline styles and style elements are removed; headings keep their id and code blocks keep their
  language class.
- Sanitization runs before code blocks are wrapped, so the copy button handler added by the component is kept.
- A custom HtmlSanitizer delegate must escape < and > inside attribute values, as the default sanitizer does:
  the copy button wrapper and ShowLineNumbers edit the sanitized markup with text patterns.
- Configure the allow-list with services.AddOsirionHtmlSanitizer(...), for example to allow a custom element.
- Set SanitizeHtml="false" only for markup the application itself produced.
