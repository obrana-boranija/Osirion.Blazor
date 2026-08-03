# Osirion.Blazor Core Components Documentation

This documentation covers the Core module of Osirion.Blazor, which provides essential UI components for building Blazor applications. The Core module is designed to be framework-agnostic and SSR-compatible, offering reusable components for common web development needs.

The Core module includes components for layout, navigation, forms, cards, sections, popups, rendering, and states. These components are built to integrate seamlessly with popular CSS frameworks like Bootstrap and Fluent UI.

## Component List

The Core module contains the following components:

- OsirionArticleMetadata
- OsirionBreadcrumbs
- OsirionContactForm
- OsirionContentNotFound
- OsirionCookieConsent
- OsirionFeatureCard
- OsirionFooter
- OsirionHtmlRenderer
- OsirionPageLayout
- OsirionPageLoading
- OsirionReadMoreLink
- OsirionResponsiveShowcaseSection
- OsirionStickySidebar
- OsirionSubscriptionCard
- OsirionTestimonialCarousel
- OsirionBaseSection
- OsirionContactInfoSection
- OsirionBackgroundPattern
- OsirionImageGallery
- OsirionMetricCard
- OsirionMetricGrid
- OsirionContextCardGrid
- OsirionReveal
- InfiniteLogoCarousel

Each component is described below with usage examples.

## OsirionArticleMetadata

The `OsirionArticleMetadata` component displays article metadata such as author, publish date, and read time.

### Parameters

- `Author`: string - The author name
- `PublishDate`: DateTime - The publication date
- `ReadTime`: string - Estimated read time

### Example

```razor
<OsirionArticleMetadata 
    Author="John Doe" 
    PublishDate="@DateTime.Now" 
    ReadTime="5 min read" />
```

## OsirionBreadcrumbs

The `OsirionBreadcrumbs` component automatically generates breadcrumb navigation from URL paths.

### Parameters

- `Path`: string - URL path to generate breadcrumbs from
- `ShowHome`: bool - Whether to show the home link (default: true)
- `HomeText`: string - Text for the home link (default: "Home")
- `HomeUrl`: string - URL for the home link (default: "/")
- `LinkLastItem`: bool - Make the last breadcrumb item clickable (default: false)
- `UrlPrefix`: string - Prefix for generated breadcrumb URLs (default: "/")
- `SegmentFormatter`: Func<string, string> - Function to format URL segments

### Example

```razor
<OsirionBreadcrumbs Path="/blog/web-development/blazor-components" />
```

For advanced usage with custom formatting:

```razor
<OsirionBreadcrumbs 
    Path="/blog/categories/web-development/articles/building-blazor-components"
    ShowHome="true"
    HomeText="Home"
    HomeUrl="/"
    LinkLastItem="false"
    UrlPrefix="/blog/"
    SegmentFormatter="@FormatSegment" />

@code {
    private string FormatSegment(string segment)
    {
        return segment.Replace("-", " ")
                     .Split(' ')
                     .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
                     .Aggregate((a, b) => $"{a} {b}");
    }
}
```

## OsirionContactForm

The `OsirionContactForm` component provides a contact form with validation.

### Parameters

- `Title`: string - Form title
- `SubmitButtonText`: string - Submit button text
- `OnSubmit`: EventCallback<ContactFormModel> - Submit event handler

### Example

```razor
<OsirionContactForm 
    Title="Contact Us" 
    SubmitButtonText="Send Message" 
    OnSubmit="@HandleSubmit" />

@code {
    private async Task HandleSubmit(ContactFormModel model)
    {
        // Handle form submission
    }
}
```

## OsirionContentNotFound

The `OsirionContentNotFound` component displays a 404 not found message.

### Parameters

- `Title`: string - Title text (default: "Content Not Found")
- `Message`: string - Message text
- `ShowHomeButton`: bool - Show home button (default: true)
- `HomeButtonText`: string - Home button text (default: "Go Home")
- `HomeUrl`: string - Home URL (default: "/")

### Example

```razor
<OsirionContentNotFound 
    Title="Page Not Found" 
    Message="The page you are looking for does not exist." />
```

## OsirionCookieConsent

The `OsirionCookieConsent` component displays a GDPR consent banner.

### Parameters

- `Title`: string - Banner title
- `Message`: string - Consent message
- `AcceptButtonText`: string - Accept button text
- `DeclineButtonText`: string - Decline button text
- `ShowCustomizeButton`: bool - Show customize button (default: true)
- `PolicyLink`: string - Privacy policy link
- `PolicyLinkText`: string - Privacy policy link text
- `Position`: string - Banner position ("bottom" or "top")
- `ConsentExpiryDays`: int - Consent expiry in days (default: 365)

### Example

```razor
<OsirionCookieConsent 
    Title="Cookie Consent" 
    Message="We use cookies to improve your experience." 
    PolicyLink="/privacy" 
    PolicyLinkText="Privacy Policy" />
```

## OsirionFeatureCard

The `OsirionFeatureCard` component displays a feature card with icon, title, and description.

### Parameters

- `Title`: string - Card title
- `Description`: string - Card description
- `Icon`: string - Icon class or URL
- `Link`: string - Link URL
- `LinkText`: string - Link text

### Example

```razor
<OsirionFeatureCard 
    Title="Fast Performance" 
    Description="Optimized for speed and efficiency." 
    Icon="fas fa-rocket" 
    Link="/features" 
    LinkText="Learn More" />
```

## OsirionFooter

The `OsirionFooter` component provides a site footer with links and copyright.

### Parameters

- `CopyrightText`: string - Copyright text
- `Links`: IEnumerable<FooterLink> - Footer links
- `ShowSocialLinks`: bool - Show social media links (default: true)
- `SocialLinks`: IEnumerable<SocialLink> - Social media links

### Example

```razor
<OsirionFooter 
    CopyrightText="© 2023 My Company" 
    Links="@footerLinks" />
```

## OsirionHtmlRenderer

The `OsirionHtmlRenderer` component renders HTML content safely.

### Parameters

- `HtmlContent`: string - HTML content to render
- `AllowedTags`: string[] - Allowed HTML tags
- `AllowedAttributes`: string[] - Allowed HTML attributes

### Example

```razor
<OsirionHtmlRenderer 
    HtmlContent="<p>This is <strong>safe</strong> HTML.</p>" />
```

## OsirionPageLayout

The `OsirionPageLayout` component provides a basic page layout structure.

### Parameters

- `Title`: string - Page title
- `ShowSidebar`: bool - Show sidebar (default: false)
- `SidebarContent`: RenderFragment - Sidebar content

### Example

```razor
<OsirionPageLayout Title="My Page">
    <SidebarContent>
        <nav>Sidebar menu</nav>
    </SidebarContent>
    <p>Page content</p>
</OsirionPageLayout>
```

## OsirionPageLoading

The `OsirionPageLoading` component displays a loading indicator.

### Parameters

- `Message`: string - Loading message (default: "Loading...")
- `ShowSpinner`: bool - Show spinner (default: true)

### Example

```razor
<OsirionPageLoading Message="Loading content..." />
```

## OsirionReadMoreLink

The `OsirionReadMoreLink` component provides a "read more" link for content previews.

### Parameters

- `Text`: string - Link text (default: "Read More")
- `Url`: string - Link URL
- `CssClass`: string - Additional CSS class

### Example

```razor
<OsirionReadMoreLink 
    Text="Continue Reading" 
    Url="/article/123" />
```

## OsirionResponsiveShowcaseSection

The `OsirionResponsiveShowcaseSection` component displays a responsive showcase section.

### Parameters

- `Title`: string - Section title
- `Items`: IEnumerable<ShowcaseItem> - Showcase items
- `Columns`: int - Number of columns (default: 3)

### Example

```razor
<OsirionResponsiveShowcaseSection 
    Title="Our Products" 
    Items="@products" />
```

## OsirionStickySidebar

The `OsirionStickySidebar` component provides a sticky sidebar.

### Parameters

- `Content`: RenderFragment - Sidebar content
- `OffsetTop`: int - Top offset in pixels (default: 20)

### Example

```razor
<OsirionStickySidebar OffsetTop="30">
    <p>Sticky content</p>
</OsirionStickySidebar>
```

## OsirionSubscriptionCard

The `OsirionSubscriptionCard` component displays a subscription card.

### Parameters

- `Title`: string - Card title
- `Description`: string - Card description
- `Price`: string - Price text
- `ButtonText`: string - Button text
- `OnSubscribe`: EventCallback - Subscribe event

### Example

```razor
<OsirionSubscriptionCard 
    Title="Premium Plan" 
    Price="$9.99/month" 
    ButtonText="Subscribe" 
    OnSubscribe="@HandleSubscribe" />
```

## OsirionTestimonialCarousel

The `OsirionTestimonialCarousel` component displays testimonials in a carousel.

### Parameters

- `Testimonials`: IEnumerable<Testimonial> - List of testimonials
- `AutoPlay`: bool - Auto-play carousel (default: true)
- `Interval`: int - Auto-play interval in milliseconds (default: 5000)

### Example

```razor
<OsirionTestimonialCarousel Testimonials="@testimonials" />
```

## OsirionBaseSection

The `OsirionBaseSection` component provides a base section layout.

### Parameters

- `Title`: string - Section title
- `Subtitle`: string - Section subtitle
- `BackgroundClass`: string - Background CSS class

### Example

```razor
<OsirionBaseSection 
    Title="Welcome" 
    Subtitle="Welcome to our site" 
    BackgroundClass="bg-primary" />
```

## OsirionContactInfoSection

The `OsirionContactInfoSection` component displays contact information.

### Parameters

- `Title`: string - Section title
- `ContactItems`: IEnumerable<ContactItem> - Contact items

### Example

```razor
<OsirionContactInfoSection 
    Title="Contact Us" 
    ContactItems="@contactItems" />
```

## OsirionBackgroundPattern

The `OsirionBackgroundPattern` component provides background patterns.

### Parameters

- `Pattern`: string - Pattern type
- `Color`: string - Pattern color

### Example

```razor
<OsirionBackgroundPattern 
    Pattern="dots" 
    Color="#f0f0f0" />
```

## InfiniteLogoCarousel

The `InfiniteLogoCarousel` component displays a carousel of logos.

### Parameters

- `Logos`: IEnumerable<string> - List of logo URLs
- `Speed`: int - Animation speed (default: 20)

### Example

```razor
<InfiniteLogoCarousel Logos="@logoUrls" />
```

## OsirionImageGallery

The `OsirionImageGallery` component renders an SSR-friendly responsive image grid with an optional lightbox enhancement.

### Parameters

- `Items`: IReadOnlyList<GalleryItem> - Images with source, alt text, caption, and dimensions
- `Columns`: int - Large-screen column count, clamped from 1 to 4 (default: 2)
- `GapSize`: int - Gap scale, clamped from 0 to 5 (default: 4)
- `Dark`: bool - Uses dark-surface caption styling

### Example

```razor
<OsirionImageGallery Items="@images" Columns="3" />

@code {
    private readonly IReadOnlyList<OsirionImageGallery.GalleryItem> images =
    [
        new("/images/overview.webp", "Product overview", "Overview")
    ];
}
```

## OsirionMetricCard

The `OsirionMetricCard` component displays a prominent value with optional supporting text and viewport-triggered count-up animation.

### Parameters

- `Value`: string - Display value such as `99.99%`, `18-32%`, or `<100ms`
- `Label`: string - Text below the value
- `Description`: string - Supporting text below the label
- `Accent`: string - Accent token used by the component styles
- `Kicker`: string - Optional text above the value
- `Elevated`: bool - Applies elevated styling
- `CssClass`: string - Additional CSS classes
- `Animate`: bool - Enables progressive count-up enhancement

### Example

```razor
<OsirionMetricCard Value="18-32%"
                   Label="Labor variance reduction"
                   Description="Measured against the approved baseline"
                   Accent="green"
                   Animate="true" />
```

## OsirionMetricGrid

The `OsirionMetricGrid` component renders metric cards as a responsive, semantic list. Place it in a parent section with a visible heading, provide an `AriaLabel` that identifies the metric group, and keep values and claims in the consuming application.

### Parameters

- `Metrics`: IReadOnlyList<MetricGridItem> - Metric values and optional card content
- `Columns`: MetricGridColumns - Two, three, or four large-screen columns
- `AriaLabel`: string - Accessible name for the metric list
- `CssClass`: string - Additional grid classes

### Example

```razor
<OsirionMetricGrid Columns="MetricGridColumns.Three"
                    AriaLabel="Service reliability metrics"
                    Metrics="@[
                        new("99.9%", "Availability"),
                        new("24/7", "Monitoring"),
                        new("15 min", "Response target")
                    ]" />
```

## OsirionContextCardGrid

The `OsirionContextCardGrid` component presents labeled detail cards in a responsive, accessible section.

### Parameters

- `Title`: string - Panel heading
- `Description`: string - Optional supporting text
- `Items`: ContextCardItem[] - Label and detail values to display
- `Id`: string - Optional section id for in-page navigation
- `HeadingId`: string - Id shared by the section landmark and heading
- `SectionClass`: string - Additional section classes
- `ContainerClass`: string - Content container classes

### Example

```razor
<OsirionContextCardGrid
    Title="Context by category"
    Description="Connect each category's priorities to the same operating view."
    Id="operating-context"
    HeadingId="operating-context-heading"
    Items="@RoleItems" />

@code {
    private readonly OsirionContextCardGrid.ContextCardItem[] RoleItems =
    [
        new("Finance", "Cost variance and forecast predictability.", "blue"),
        new("Operations", "Coverage reliability and exception response.", "green")
    ];
}
```

## OsirionReveal

The `OsirionReveal` component keeps content visible during SSR and adds an optional viewport reveal animation when browser enhancement is available.

### Parameters

- `ChildContent`: RenderFragment - Content to render
- `Animate`: bool - Enables the viewport enhancement (default: true)
- `Animation`: RevealAnimation - `Up`, `Down`, `Left`, `Right`, or `Fade`

### Example

```razor
<OsirionReveal Animation="RevealAnimation.Left">
    <section>Progressively enhanced content</section>
</OsirionReveal>
```
