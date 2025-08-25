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
- `ImageUrl`: Image URL for hero section
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
Safely renders HTML content with built-in sanitization.

```razor
<OsirionHtmlRenderer 
    HtmlContent="@htmlString"
    SanitizeHtml="true" />
```

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
- SSR-compatible form submission
- Configurable consent expiry
- Privacy policy integration

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