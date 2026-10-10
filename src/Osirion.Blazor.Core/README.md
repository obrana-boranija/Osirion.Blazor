# Osirion.Blazor.Core

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Core)](https://www.nuget.org/packages/Osirion.Blazor.Core)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Core components and utilities for Osirion.Blazor ecosystem. This package provides the foundation for building SSR-compatible Blazor components with a comprehensive set of layout, navigation, and content components.

## Components

### Layout Components

#### `HeroSection`
A comprehensive hero section component with support for various layouts, background images, and call-to-action buttons.

```razor
<HeroSection 
    Title="Welcome to Our Platform"
    Subtitle="Build amazing applications with ease"
    Summary="Experience the power of modern web development with our comprehensive toolkit."
    ImageUrl="/images/hero-image.jpg"
    UseBackgroundImage="true"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/getting-started"
    SecondaryButtonText="Learn More"
    SecondaryButtonUrl="/docs"
    Variant="HeroVariant.Hero"
    Alignment="Alignment.Center" />
```

**Parameters:**
- `Title`: Main hero title
- `Subtitle`: Hero subtitle
- `Summary`: Hero description/summary
- `ImageUrl`: Image URL for hero section. As a background image it is written into `url('...')` with quotes, backslashes, parentheses and control characters CSS-escaped, so a URL from content front matter cannot add style declarations of its own
- `UseBackgroundImage`: Display image as background vs. side image
- `BackgroundPattern`: Optional background pattern
- `Alignment`: Text alignment (Left, Center, Right, Justify)
- `Variant`: Hero variant (Hero, Jumbotron, Minimal)
- `ShowPrimaryButton`/`ShowSecondaryButton`: Button visibility
- `PrimaryButtonText`/`SecondaryButtonText`: Button text
- `PrimaryButtonUrl`/`SecondaryButtonUrl`: Button URLs
- `MinHeight`: Minimum height of hero section
- `ShowMetadata`: Display author/date metadata
- `Author`, `PublishDate`, `ReadTime`: Metadata properties

#### `OsirionPageLayout`
A comprehensive page layout component with header, footer, sidebar, and content areas.

```razor
<OsirionPageLayout 
    HasSidebar="true"
    SidebarPosition="Position.Left"
    HasFooter="true">
    
    <HeaderContent>
        <!-- Your header content -->
    </HeaderContent>
    
    <SidebarContent>
        <!-- Your sidebar content -->
    </SidebarContent>
    
    <MainContent>
        <!-- Your main content -->
    </MainContent>
    
    <FooterContent>
        <!-- Your footer content -->
    </FooterContent>
</OsirionPageLayout>
```

#### `OsirionFooter`
A flexible footer component with multiple sections and customizable content.

```razor
<OsirionFooter>
    <FooterContent>
        <!-- Custom footer content -->
    </FooterContent>
</OsirionFooter>
```

#### `OsirionStickySidebar`
A sidebar component that sticks to the viewport during scrolling.

```razor
<OsirionStickySidebar Position="Position.Left">
    <!-- Sidebar content -->
</OsirionStickySidebar>
```

#### `OsirionBackgroundPattern`
A component for adding decorative background patterns to any section.

```razor
<OsirionBackgroundPattern 
    Pattern="BackgroundPatternType.Dots"
    Opacity="0.1" />
```

### Navigation Components

#### `OsirionBreadcrumbs`
Automatically generates breadcrumb navigation from URL paths.

```razor
<OsirionBreadcrumbs 
    Path="/blog/category/post-title"
    ShowHome="true"
    HomeText="Home"
    HomeUrl="/"
    LinkLastItem="false"
    UrlPrefix="/blog/"
    SegmentFormatter="@((segment) => segment.Replace("-", " "))" />
```

**Parameters:**
- `Path`: URL path to generate breadcrumbs from
- `ShowHome`: Include home link in breadcrumbs
- `HomeText`: Text for home link
- `HomeUrl`: URL for home link
- `LinkLastItem`: Make the last breadcrumb item clickable
- `UrlPrefix`: Prefix for generated URLs
- `SegmentFormatter`: Function to format URL segments

### Content Components

#### `OsirionArticleMetadata`
Displays article metadata including author, publish date, and read time.

```razor
<OsirionArticleMetadata 
    Author="John Doe"
    PublishDate="@DateTime.Now"
    DateFormat="MMM dd, yyyy"
    ReadTime="5 min read" />
```

**Parameters:**
- `Author`: Article author name
- `PublishDate`: Publication date
- `DateFormat`: Date formatting string
- `ReadTime`: Estimated reading time

#### `OsirionHtmlRenderer`
Renders HTML content, sanitized by default (`SanitizeHtml` defaults to `true`), with optional code highlighting.

```razor
<OsirionHtmlRenderer HtmlContent="@htmlString" />
```

Set `SanitizeHtml="false"` only for markup your application built itself. See [HTML sanitization](#html-sanitization).

### State Components

#### `OsirionContentNotFound`
A user-friendly 404 error page component.

```razor
<OsirionContentNotFound 
    Title="Page Not Found"
    Message="The page you're looking for doesn't exist."
    ShowBackButton="true"
    BackButtonText="Go Back"
    BackButtonUrl="/" />
```

#### `OsirionPageLoading`
A loading indicator component for page transitions.

```razor
<OsirionPageLoading 
    Message="Loading content..."
    ShowSpinner="true" />
```

### Interactive Components

#### `OsirionCookieConsent`
A GDPR-compliant cookie consent banner with customizable categories.

```razor
<OsirionCookieConsent 
    Title="Cookie Consent"
    Message="We use cookies to improve your experience."
    ShowDeclineButton="true"
    ShowCustomizeButton="true"
    PolicyLink="/privacy-policy"
    Categories="@cookieCategories"
    Position="bottom" />
```

**Features:**
- GDPR-compliant cookie consent management
- Customizable cookie categories (Necessary, Analytics, Marketing, Preferences)
- SSR-compatible form submission, protected by an antiforgery token (map the endpoint with `MapOsirionCookieConsent()` and call `UseAntiforgery()`)
- Redirects back only to local paths after consent is saved
- Configurable consent expiry
- Privacy policy integration

The return-URL check is public as `Osirion.Blazor.Core.Utils.LocalUrl`, for your own redirects:

```csharp
using Osirion.Blazor.Core.Utils;

// true only for a root-relative path such as "/blog?page=2"; false for "//evil.example", "/\evil.example",
// absolute URLs and values with control characters
var isLocal = LocalUrl.IsLocalPath(returnUrl);

// returnUrl when it is local, otherwise the fallback (LocalUrl.DefaultPath, "/", unless you pass another local path)
return Results.LocalRedirect(LocalUrl.GetSafeLocalPath(returnUrl));
```

#### `InfiniteLogoCarousel`
Self-contained logo carousel with zero external dependencies.

```razor
<InfiniteLogoCarousel 
    Title="Our Partners"
    CustomLogos="@logoList"
    AnimationDuration="60"
    Direction="AnimationDirection.Right"
    PauseOnHover="true"
    EnableGrayscale="true" />
```

Detailed documentation: [InfiniteLogoCarousel.md](Components/InfiniteLogoCarousel.md)

## Installation

```bash
dotnet add package Osirion.Blazor.Core
```

## Usage

Add import to your `_Imports.razor`:

```razor
@using Osirion.Blazor.Components
```

Inherit from `OsirionComponentBase` when creating components:

```csharp
public class MyComponent : OsirionComponentBase
{
    // Your component code here
}
```

## HTML Sanitization

Content HTML (rendered markdown, CMS items, `OsirionHtmlRenderer` input) is cleaned by `IHtmlContentSanitizer`,
an allow-list sanitizer built on the HtmlSanitizer library. It parses markup the way a browser does and keeps only
allowed elements, attributes and URL schemes.

Default allow-list:

- Keeps common content markup (headings, paragraphs, lists, tables, links, images, code, details) plus `class`
  and `id`, so styling, code highlighting and table of contents anchors keep working.
- Keeps `role` and every `aria-*` attribute on allowed elements. Set `AllowAriaAttributes = false` to remove them.
- Keeps markdown table column alignment: `th` and `td` keep the `text-align` declaration of their `style`
  attribute and every other CSS property is removed from them. Set `AllowTableCellAlignment = false` to remove
  `style` from table cells too.
- URL schemes `http`, `https`, `mailto`, `tel` and relative URLs, including relative URLs with a colon in the
  query or fragment such as `page.html?t=10:30`.
- Removes forms and form controls, iframes, `name` attributes, `<style>` elements and `style` attributes on every
  element other than table cells. In rendered markdown this also removes task-list checkboxes (`<input>`).
- Keeps only attributes on the HtmlSanitizer default list plus `class`, `id`, `role` and `aria-*`: `data-*`,
  `srcset`, `sizes`, `loading` and `decoding`, and the `video`, `audio`, `source`, `track` and `picture` elements,
  are removed unless the host allows them.
- Never allows `script`, `svg`, `math`, `template`, `object`, `embed`, `applet`, `meta`, `base`, `link`, `frame`,
  `frameset`, `noscript`, event handler attributes (`on*`) or `javascript:`, `vbscript:` and `data:` URLs,
  whatever the options say.
- Unknown elements are unwrapped and their text is kept; script, style, iframe, svg and similar elements are
  removed together with their content.

`AddOsirion()` and the CMS registrations add the sanitizer with this allow-list. Change it with
`AddOsirionHtmlSanitizer`; the call can be repeated and every configuration is applied:

```csharp
using Osirion.Blazor.Core.Extensions;

builder.Services.AddOsirionHtmlSanitizer(options =>
{
    // A custom element read by client-side script, with attributes kept only on that element
    options.AllowElement("chart", "type", "title", "caption", "data", "colors");

    // Opt in to inline styles on every element (CSS properties are still filtered, and table cells
    // get the same general filter instead of keeping text-align only)
    options.AllowedAttributes.Add("style");

    // Opt in to video embeds; src is still limited to the allowed URL schemes
    options.AllowElement("iframe", "src", "width", "height", "title", "allow", "allowfullscreen", "frameborder");

    // Opt in to responsive image attributes; srcset candidates are still URL-screened
    options.AllowedAttributes.UnionWith(["loading", "decoding", "srcset", "sizes"]);
});
```

To go back to the stricter defaults of earlier builds, which removed `role`, `aria-*` and table cell alignment,
set `options.AllowAriaAttributes = false` and `options.AllowTableCellAlignment = false`.

Attributes allowed through `AllowElement` are made inert as HTML only. Client-side script that reads them must
treat them as text (`textContent`, `setAttribute`, `createElement`) and never write them through `innerHTML`: the
browser decodes a value such as `caption="&lt;img src=x onerror=...&gt;"` back into markup, and the handler runs.

Components that render content resolve the registered sanitizer and fall back to `HtmlContentSanitizer.Default`
when none is registered, so content is never written unsanitized.

`JsonLdNormalizer.TryNormalize` makes author-supplied JSON-LD safe to place inside a
`<script type="application/ld+json">` element by re-serializing it with `<`, `>` and `&` escaped.

## Base Component Features

`OsirionComponentBase` provides:

- Common parameter handling for CSS classes and styles
- Automatic handling of additional attributes
- Environment detection (browser vs server)
- Consistent rendering behavior
- Accessibility support
- Framework integration support

## Enums and Models

### Position
```csharp
public enum Position
{
    Top, Bottom, Left, Right,
    TopLeft, TopRight, BottomLeft, BottomRight
}
```

### Alignment
```csharp
public enum Alignment
{
    Left, Center, Right, Justify
}
```

### HeroVariant
```csharp
public enum HeroVariant
{
    Hero, Jumbotron, Minimal
}
```

### BackgroundPatternType
```csharp
public enum BackgroundPatternType
{
    Dots, Grid, Lines, Waves, Geometric
}
```

### ScrollBehavior
```csharp
public enum ScrollBehavior
{
    Auto, Smooth, Instant
}
```

### ButtonVariant, ButtonSize, ButtonShape
Comprehensive button styling enums for consistent UI design.

## Models

### BreadcrumbSegment
```csharp
public class BreadcrumbSegment
{
    public string Name { get; set; }
    public string Url { get; set; }
    public bool IsLast { get; set; }
}
```

### BreadcrumbPath
```csharp
public class BreadcrumbPath
{
    public List<BreadcrumbSegment> Segments { get; set; }
    public string FullPath { get; set; }
}
```

### CookieCategory
```csharp
public class CookieCategory
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsRequired { get; set; }
    public bool IsEnabled { get; set; }
}
```

## CSS Framework Integration

The core components integrate seamlessly with popular CSS frameworks:

- Bootstrap 5+
- Fluent UI
- MudBlazor
- Radzen

Components automatically adapt their styling based on the detected framework.

## Accessibility

All components include:

- Proper ARIA labels and roles
- Keyboard navigation support
- Screen reader compatibility
- High contrast mode support
- Reduced motion preferences

## Performance

- **SSR Optimized**: Components render efficiently on the server
- **Minimal JavaScript**: Progressive enhancement approach
- **Caching Support**: Built-in caching for expensive operations
- **Lazy Loading**: Components load resources as needed

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.