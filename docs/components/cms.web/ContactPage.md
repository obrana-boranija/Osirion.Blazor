# ContactPage

Purpose
A whole contact page: the title as the page's `h1`, an optional Markdown introduction, and an `OsirionContactForm`
with the contact details beside it. It works without content, from `Title` and `Description`. With a content item
(`contact.md`, slug `contact`, by default) the title, description and introduction come from Markdown and the page
writes ContactPage structured data. It renders without script.

Usage
```razor
@page "/contact"
<ContactPage Description="Questions, bug reports and ideas."
             ContactInfo="@contact"
             OnSubmit="SendAsync" />
```

Key parameters
- Content: `ItemSlug` (default `contact`), `ItemPath`, `ItemUrl` or `Item`; when nothing is found the page falls back
  to `Title` (default `Contact`) and `Description`, writes them as the title tag and meta description, and does not
  answer 404
- Form: `FormId` (default `contact-page-form`, also the form name for static rendering), `FormTitle`,
  `FormDescription`, `ContactInfo` (shown beside the form when set), `UseBuiltInEmailService` (false; needs
  `AddOsirionEmailServices`), `OnSubmit`
- `FormContent` replaces the built-in form, for an `OsirionContactForm` configured beyond these parameters
- `ChildContent` is shown after the form
- Heading and SEO parameters as on `AboutPage`; `SchemaTypes` defaults to `ContactPage`
