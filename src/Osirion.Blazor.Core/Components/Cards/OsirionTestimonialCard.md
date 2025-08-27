# OsirionTestimonialCard Component

The `OsirionTestimonialCard` component is a professional, framework-agnostic testimonial card designed for showcasing customer testimonials, reviews, and recommendations with modern UX/UI principles.

## Features

- **Professional Design**: Clean, modern layout with optimal typography and spacing
- **Framework Agnostic**: Seamless integration with Bootstrap, FluentUI, MudBlazor, Radzen, and custom frameworks
- **Flexible Content**: Support for custom testimonial content and various layout options
- **Social Integration**: LinkedIn profile linking with professional icons
- **Rating System**: Optional 5-star rating display with accessible markup
- **Responsive Design**: Mobile-first approach with adaptive layouts
- **Accessibility**: Full ARIA support, screen reader friendly, and keyboard navigation
- **Performance Optimized**: Lazy loading images and CSS transitions with reduced motion support

## Parameters

### Required Parameters
- `Name`: Person's name (required)

### Profile Parameters
- `ProfileImageUrl`: Profile image URL (displays as circular avatar)
- `Company`: Person's company/organization
- `Position`: Person's job title/position
- `LinkedInUrl`: LinkedIn profile URL (adds clickable LinkedIn icon)
- `ImageSize`: Profile image size in pixels (default: 64, auto-adjusts by size variant)

### Content Parameters
- `TestimonialText`: The testimonial text content
- `TestimonialContent`: Custom testimonial content (overrides TestimonialText)
- `ChildContent`: Additional content area

### Rating Parameters
- `ShowRating`: Whether to display star rating (default: false)
- `Rating`: Star rating 1-5 (default: 5)

### Read More Integration
- `ReadMoreHref`: URL for read more link (optional - if null, link won't show)
- `ReadMoreText`: Text for read more link (default: "Read full testimonial")
- `ReadMoreVariant`: Style variant for read more link
- `ReadMoreTarget`: Link target (_blank, _self, etc.)

### Style Parameters
- `Variant`: Card style variant (Default, Minimal, Highlighted, Compact)
- `Size`: Card size (Small, Normal, Large)
- `Elevated`: Whether card should have shadow/elevation effect (default: true)
- `Borderless`: Whether card should be borderless (default: false)

## Usage Examples

### Basic Testimonial
```razor
<OsirionTestimonialCard 
    Name="Sarah Johnson"
    Position="Senior Developer"
    Company="Tech Solutions Inc."
    TestimonialText="This product has completely transformed our development workflow. The team's responsiveness and attention to detail is outstanding."
    ProfileImageUrl="/images/sarah-johnson.jpg" />
```

### With Rating and LinkedIn
```razor
<OsirionTestimonialCard 
    Name="Michael Chen"
    Position="CTO"
    Company="Innovation Labs"
    TestimonialText="Exceptional quality and support. Our team productivity increased by 40% after implementation."
    ProfileImageUrl="/images/michael-chen.jpg"
    LinkedInUrl="https://linkedin.com/in/michaelchen"
    ShowRating="true"
    Rating="5" />
```

### With Read More Link
```razor
<OsirionTestimonialCard 
    Name="Lisa Rodriguez"
    Position="Product Manager"
    Company="Digital Dynamics"
    TestimonialText="The integration was seamless and the results exceeded our expectations..."
    ProfileImageUrl="/images/lisa-rodriguez.jpg"
    ReadMoreHref="/testimonials/lisa-rodriguez"
    ReadMoreText="Read full case study"
    ReadMoreVariant="ReadMoreVariant.Arrow" />
```

### Custom Content with Rich Text
```razor
<OsirionTestimonialCard 
    Name="David Park"
    Position="Founder & CEO"
    Company="StartupCo"
    ProfileImageUrl="/images/david-park.jpg"
    LinkedInUrl="https://linkedin.com/in/davidpark"
    ShowRating="true"
    Rating="5">
    <TestimonialContent>
        <p>This solution has been a <strong>game-changer</strong> for our startup.</p>
        <p>The ROI was evident within the first month, and the ongoing support has been phenomenal.</p>
    </TestimonialContent>
</OsirionTestimonialCard>
```

### Different Variants
```razor
<!-- Minimal Style -->
<OsirionTestimonialCard 
    Name="Anna Thompson"
    TestimonialText="Clean, efficient, and reliable."
    Variant="TestimonialVariant.Minimal"
    Borderless="true" />

<!-- Highlighted Style -->
<OsirionTestimonialCard 
    Name="Robert Kim"
    Position="Lead Engineer"
    Company="Enterprise Corp"
    TestimonialText="Outstanding platform with excellent developer experience."
    Variant="TestimonialVariant.Highlighted"
    ShowRating="true"
    Rating="5" />

<!-- Compact Style -->
<OsirionTestimonialCard 
    Name="Maria Garcia"
    Position="UX Designer"
    TestimonialText="Intuitive design and powerful features."
    Variant="TestimonialVariant.Compact"
    Size="TestimonialSize.Small" />
```

### Different Sizes
```razor
<!-- Small Size -->
<OsirionTestimonialCard 
    Name="Tom Wilson"
    TestimonialText="Great product!"
    Size="TestimonialSize.Small" />

<!-- Large Size -->
<OsirionTestimonialCard 
    Name="Jennifer Adams"
    Position="VP of Engineering"
    Company="MegaCorp Industries"
    TestimonialText="This platform has revolutionized how we approach development challenges."
    Size="TestimonialSize.Large"
    ShowRating="true"
    Rating="5" />
```

## Variants

### TestimonialVariant.Default
Standard card appearance with border, shadow, and full padding.

### TestimonialVariant.Minimal
Clean, borderless design with reduced visual emphasis, perfect for content-focused layouts.

### TestimonialVariant.Highlighted
Eye-catching design with primary color accents and gradient background, ideal for featured testimonials.

### TestimonialVariant.Compact
Horizontal layout with reduced spacing, perfect for sidebar or dense layouts.

## Sizes

### TestimonialSize.Small
- Reduced padding and font sizes
- 48px profile image
- Ideal for sidebars or card grids

### TestimonialSize.Normal (Default)
- Standard spacing and typography
- 64px profile image
- Perfect for main content areas

### TestimonialSize.Large
- Increased padding and font sizes
- 80px profile image
- Great for hero sections or featured testimonials

## CSS Classes

The component generates semantic CSS classes:

- `.osirion-testimonial-card`: Base component class
- `.osirion-testimonial-{variant}`: Variant-specific styling
- `.osirion-testimonial-{size}`: Size-specific styling
- `.osirion-testimonial-elevated`: Shadow/elevation effects
- `.osirion-testimonial-borderless`: Borderless appearance

## Framework Integration

Automatically detects and integrates with your CSS framework:

- **Bootstrap**: Uses `card` classes and Bootstrap design tokens
- **FluentUI**: Integrates with Fluent design system colors and spacing
- **MudBlazor**: Uses `mud-paper` and elevation classes
- **Radzen**: Applies Radzen card styling
- **Custom**: Falls back to framework-agnostic classes

## Accessibility Features

- **ARIA Labels**: Comprehensive labeling for screen readers
- **Semantic HTML**: Uses proper `<blockquote>` and `<cite>` elements
- **Keyboard Navigation**: Full keyboard support for interactive elements
- **Rating Accessibility**: Star ratings include descriptive ARIA labels
- **Focus Management**: Visible focus indicators and logical tab order
- **Alt Text**: Dynamic, context-aware alt text for profile images

## Responsive Behavior

- **Mobile**: Automatic font size reduction and optimized spacing
- **Compact Variant**: Switches to vertical layout on mobile devices
- **Touch Targets**: All interactive elements meet minimum touch target sizes
- **Flexible Images**: Profile images scale appropriately across devices

## Performance Features

- **Lazy Loading**: Profile images load with `loading="lazy"` attribute
- **Optimized Icons**: Inline SVG icons for better performance
- **CSS Transitions**: Smooth animations with reduced motion support
- **Minimal DOM**: Efficient rendering with conditional elements

## Advanced Features

### LinkedIn Integration
When `LinkedInUrl` is provided, the name becomes a clickable link with a LinkedIn icon, opening in a new tab with proper security attributes.

### Star Ratings
The rating system displays filled and empty stars with proper ARIA labeling for accessibility.

### Read More Integration
Seamlessly integrates with the `OsirionReadMoreLink` component for consistent link styling.

### Custom Content
Use `TestimonialContent` parameter for rich HTML content while maintaining proper quote semantics.

## Browser Support

Compatible with all modern browsers. Uses CSS custom properties with comprehensive fallbacks for older browsers.

## Best Practices

1. **Image Optimization**: Use optimized, square images for best results
2. **Content Length**: Keep testimonials concise for better readability
3. **Consistent Sizing**: Use the same size variant for testimonials in the same section
4. **Alt Text**: The component automatically generates descriptive alt text
5. **Performance**: Consider lazy loading for pages with many testimonial cards