# SeoMetadataRenderer

Emit meta tags (title, description, og:*, twitter:*) from ContentItem metadata.

Structured data
- Custom JSON-LD from front matter (`seo_properties.json_ld`) is parsed as JSON and written back with `<`, `>`
  and `&` escaped, so it cannot end the surrounding script element.
- A value that is not a valid JSON object or array, including one whose text is not valid Unicode (for example
  half of a surrogate pair), is ignored with a warning log entry, and the generated Schema.org data is rendered
  instead.
