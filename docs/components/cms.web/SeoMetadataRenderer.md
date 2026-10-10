# SeoMetadataRenderer

Emit meta tags (title, description, og:*, twitter:*) from ContentItem metadata.

Structured data
- Custom JSON-LD from front matter (`seo_properties.json_ld`) is parsed as JSON and written back with `<`, `>`
  and `&` escaped, so it cannot end the surrounding script element.
- A value that is not a valid JSON object or array, including one whose text is not valid Unicode (for example
  half of a surrogate pair), is ignored with a warning log entry, and the generated Schema.org data is rendered
  instead.

## Canonical URL, FAQ data and mobile tags

- The canonical URL, `og:url` and the structured data URLs are the current address without its query string and fragment, so tracking parameters never create a second canonical URL. Collection pages get a canonical link too. Use `CanonicalQueryParameters` to keep named parameters (for example `page` for paginated lists); a canonical declared in front matter always wins.
- `EnableGeneratedFaq` (default `false`, also available on `SeoMetadataOptions`) turns on FAQ JSON-LD and `answers-to` meta tags derived from titles and descriptions. It requires `EnableAeoOptimization`. An explicit `SchemaType.FAQ` emits nothing on its own.
- `EmitMobileMetaTags` (default `false`) emits viewport, theme-color and the mobile web app tags. Leave it off when the host document already declares them.
- Metadata is recomputed whenever the parameters are set, so a reused instance never keeps the previous page's values.
- `AlternateLanguageUrls` entries must have the form `language|url`; malformed entries are skipped and logged.
