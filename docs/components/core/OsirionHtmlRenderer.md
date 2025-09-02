# OsirionHtmlRenderer

Purpose
Render HTML content with optional Prism-based code highlighting, copy button, and line highlighting.

Key parameters
- HtmlContent (string)
- EnableSyntaxHighlighting (default true)
- ShowLineNumbers, EnableCopyButton, EnableLineHighlighting
- UseAccessibleTheme (WCAG-friendly)
- LanguageClasses (per-language custom CSS classes)
- SanitizeHtml (basic sanitizer) or HtmlSanitizer delegate

Implementation notes
- Detects <pre><code> blocks and injects language- classes
- Wraps blocks with header and copy button when enabled
- Adds line-numbers to <pre> if requested
- Provides initialization JS snippet with retry

Security
- Prefer SanitizeHtml=true for untrusted content, or provide a sanitizer callback.
