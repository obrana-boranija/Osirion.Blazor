# HeroSection Component

[![Component](https://img.shields.io/badge/Component-Core-blue)](https://github.com/obrana-boranija/Osirion.Blazor/tree/master/src/Osirion.Blazor.Core)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor.Core)](https://www.nuget.org/packages/Osirion.Blazor.Core)

The `HeroSection` component is a versatile, feature-rich component for creating compelling hero sections with various layout options, background images, and call-to-action buttons. It's fully SSR-compatible and requires no JavaScript dependencies.

## Features

- **Multiple Variants**: Hero, Jumbotron, and Minimal layouts
- **Flexible Image Handling**: Background images or side-positioned images
- **Background Patterns**: Built-in decorative patterns
- **Call-to-Action Buttons**: Primary and secondary buttons with customizable styling
- **Article Metadata**: Author, publication date, and read time display
- **Responsive Design**: Automatically adapts to different screen sizes
- **Accessibility**: Full ARIA support and keyboard navigation
- **Framework Integration**: Works with Bootstrap, Fluent UI, MudBlazor, and Radzen

## Basic Usage

```razor
@using Osirion.Blazor.Components

<HeroSection 
    Title="Welcome to Our Platform"
    Subtitle="Build amazing applications with ease"
    Summary="Experience the power of modern web development with our comprehensive toolkit."
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/getting-started"
    SecondaryButtonText="Learn More"
    SecondaryButtonUrl="/docs" />
```

## Advanced Usage

### Hero with Background Image

```razor
<HeroSection 
    Title="Transform Your Business"
    Subtitle="Digital Solutions for Modern Enterprises"
    Summary="Leverage cutting-edge technology to drive growth and innovation in your organization."
    ImageUrl="/images/business-hero.jpg"
    UseBackgroundImage="true"
    TextColor="#ffffff"
    MinHeight="80vh"
    Alignment="Alignment.Center"
    PrimaryButtonText="Start Free Trial"
    PrimaryButtonUrl="/trial"
    SecondaryButtonText="Watch Demo"
    SecondaryButtonUrl="/demo" />
```

### Hero with Side Image

```razor
<HeroSection 
    Title="Innovative Technology Solutions"
    Subtitle="Leading the Future of Development"
    Summary="Our platform combines the latest technologies with proven methodologies to deliver exceptional results."
    ImageUrl="/images/technology-illustration.png"
    ImageAlt="Technology illustration"
    UseBackgroundImage="false"
    ImagePosition="Alignment.Right"
    Variant="HeroVariant.Hero"
    BackgroundPattern="BackgroundPatternType.Dots" />
```

### Article Hero with Metadata

```razor
<HeroSection 
    Title="Understanding Modern Web Architecture"
    Subtitle="A Deep Dive into Scalable Systems"
    Summary="Explore the principles and practices that power today's most successful web applications."
    Author="Jane Developer"
    PublishDate="@DateTime.Parse("2025-01-15")"
    ReadTime="12 min read"
    ShowMetadata="true"
    Variant="HeroVariant.Minimal"
    ShowPrimaryButton="false"
    ShowSecondaryButton="false" />
```

### Hero with Custom Content

```razor
<HeroSection Variant="HeroVariant.Jumbotron">
    <TitleContent>
        <h1 class="display-3 fw-bold">
            Build <span class="text-primary">Better</span> Applications
        </h1>
    </TitleContent>
    
    <SubtitleContent>
        <p class="lead">
            Empower your development team with our comprehensive toolkit
        </p>
    </SubtitleContent>
    
    <SummaryContent>
        <div class="mt-4">
            <ul class="list-unstyled fs-5">
                <li>? SSR-Compatible Components</li>
                <li>? Zero External Dependencies</li>
                <li>? Framework Integration</li>
                <li>? Accessibility First</li>
            </ul>
        </div>
    </SummaryContent>
    
    <ChildContent>
        <div class="mt-4">
            <div class="d-flex gap-3 align-items-center">
                <span class="badge bg-success fs-6">New</span>
                <span>Now supporting .NET 9</span>
            </div>
        </div>
    </ChildContent>
</HeroSection>
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string?` | `null` | Main hero title |
| `TitleContent` | `RenderFragment?` | `null` | Custom title content (overrides Title) |
| `Subtitle` | `string?` | `null` | Hero subtitle |
| `SubtitleContent` | `RenderFragment?` | `null` | Custom subtitle content (overrides Subtitle) |
| `Summary` | `string?` | `null` | Hero description/summary |
| `SummaryContent` | `RenderFragment?` | `null` | Custom summary content (overrides Summary) |
| `ImageUrl` | `string?` | `null` | Image URL for hero section |
| `ImageAlt` | `string?` | `null` | Alt text for the image |
| `UseBackgroundImage` | `bool` | `false` | Display image as background vs. side image |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | Optional background pattern |
| `Alignment` | `Alignment` | `Left` | Text alignment |
| `ImagePosition` | `Alignment` | `Right` | Image position when not using background |
| `Variant` | `HeroVariant` | `Hero` | Layout variant |
| `ShowPrimaryButton` | `bool` | `true` | Show primary button |
| `ShowSecondaryButton` | `bool` | `true` | Show secondary button |
| `PrimaryButtonText` | `string?` | `null` | Primary button text |
| `PrimaryButtonUrl` | `string?` | `null` | Primary button URL |
| `SecondaryButtonText` | `string?` | `null` | Secondary button text |
| `SecondaryButtonUrl` | `string?` | `null` | Secondary button URL |
| `BackgroundColor` | `string?` | `null` | Background color override |
| `TextColor` | `string?` | `null` | Text color override |
| `MinHeight` | `string` | `"60vh"` | Minimum height of hero section |
| `Author` | `string?` | `null` | Author name for metadata |
| `PublishDate` | `DateTime?` | `null` | Publication date for metadata |
| `ReadTime` | `string?` | `null` | Reading time for metadata |
| `ShowMetadata` | `bool` | `false` | Display metadata section |
| `HasDivider` | `bool` | `true` | Show section divider |
| `ChildContent` | `RenderFragment?` | `null` | Additional custom content |

## Variants

### Hero (Default)
- Balanced layout with side image support
- Suitable for landing pages and product features
- Image size: 600x315px

### Jumbotron
- Large, prominent display
- Center-aligned content emphasis
- Image size: 600x600px
- Perfect for major announcements

### Minimal
- Clean, focused design
- Ideal for article headers and documentation
- Image size: 315x315px
- Reduced visual complexity

## Alignment Options

- **Left**: Standard left-aligned layout
- **Center**: Centered content alignment
- **Right**: Right-aligned content
- **Justify**: Full-width justified text

## Background Patterns

When `BackgroundPattern` is specified, decorative patterns are overlaid:

- `Dots`: Subtle dot pattern
- `Grid`: Grid line pattern
- `Lines`: Diagonal line pattern
- `Waves`: Wave pattern
- `Geometric`: Geometric shapes

## CSS Customization

Override the component styling with CSS variables:

```css
:root {
    /* Hero section colors */
    --osirion-hero-background: #ffffff;
    --osirion-hero-text: #333333;
    --osirion-hero-accent: #007bff;
    
    /* Hero section spacing */
    --osirion-hero-padding: 4rem 0;
    --osirion-hero-gap: 2rem;
    
    /* Button styling */
    --osirion-hero-button-primary: #007bff;
    --osirion-hero-button-secondary: transparent;
    
    /* Image styling */
    --osirion-hero-image-border-radius: 0.5rem;
    --osirion-hero-image-shadow: 0 10px 25px rgba(0,0,0,0.1);
}
```

## Accessibility Features

- **Semantic HTML**: Uses proper heading hierarchy
- **ARIA Labels**: Screen reader friendly
- **Keyboard Navigation**: Full keyboard support for buttons
- **Color Contrast**: High contrast mode support
- **Reduced Motion**: Respects `prefers-reduced-motion`

## Framework Integration

The HeroSection automatically adapts to different CSS frameworks:

### Bootstrap
```razor
<HeroSection 
    Title="Bootstrap Integration"
    Class="bg-primary text-white"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/start" />
```

### Fluent UI
```razor
<HeroSection 
    Title="Fluent Design"
    Class="ms-bgColor-themePrimary ms-fontColor-white"
    Variant="HeroVariant.Jumbotron" />
```

## Performance Considerations

- **SSR Optimized**: Renders completely on the server
- **Image Optimization**: Supports responsive images
- **Lazy Loading**: Images load efficiently
- **Minimal JavaScript**: No JS dependencies for core functionality

## Best Practices

1. **Image Optimization**: Use optimized images (WebP when possible)
2. **Accessible Alt Text**: Provide meaningful alt text for images
3. **Button Hierarchy**: Use primary button for main action, secondary for alternatives
4. **Content Length**: Keep titles concise, summaries under 200 characters
5. **Color Contrast**: Ensure sufficient contrast especially with background images
6. **Mobile First**: Test on mobile devices for responsive behavior

## Real-World Examples

### Landing Page Hero
```razor
<HeroSection 
    Title="Transform Your Workflow"
    Subtitle="Boost Productivity by 40%"
    Summary="Our platform streamlines your development process with intelligent automation and seamless integrations."
    ImageUrl="/images/productivity-hero.jpg"
    UseBackgroundImage="true"
    Alignment="Alignment.Center"
    MinHeight="100vh"
    PrimaryButtonText="Start Free Trial"
    PrimaryButtonUrl="/trial"
    SecondaryButtonText="Schedule Demo"
    SecondaryButtonUrl="/demo"
    TextColor="#ffffff" />
```

### Blog Post Header
```razor
<HeroSection 
    Title="@article.Title"
    Summary="@article.Description"
    Author="@article.Author"
    PublishDate="@article.PublishDate"
    ReadTime="@article.ReadTime"
    ImageUrl="@article.FeaturedImage"
    ShowMetadata="true"
    Variant="HeroVariant.Minimal"
    ShowPrimaryButton="false"
    ShowSecondaryButton="false" />
```

### Product Feature Section
```razor
<HeroSection 
    Title="Advanced Analytics Dashboard"
    Subtitle="Data-Driven Insights"
    Summary="Get real-time insights into your application performance with our comprehensive analytics suite."
    ImageUrl="/images/dashboard-preview.png"
    ImageAlt="Analytics dashboard preview"
    UseBackgroundImage="false"
    ImagePosition="Alignment.Right"
    BackgroundPattern="BackgroundPatternType.Dots"
    PrimaryButtonText="Explore Features"
    PrimaryButtonUrl="/features/analytics" />
```