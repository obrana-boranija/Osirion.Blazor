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
