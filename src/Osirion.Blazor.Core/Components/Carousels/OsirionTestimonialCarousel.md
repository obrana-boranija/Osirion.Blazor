# OsirionTestimonialCarousel Component

The `OsirionTestimonialCarousel` component is a sophisticated, infinite-scrolling testimonial carousel that showcases customer testimonials in a continuous animation. Built with modern UX/UI principles and designed to work seamlessly in SSR environments without JavaScript dependencies.

## Features

- **Infinite Scrolling**: Seamless continuous animation with perfect looping
- **SSR Compatible**: Works in Static Server-Side Rendering without JavaScript
- **Framework Agnostic**: Integrates with Bootstrap, FluentUI, MudBlazor, Radzen, and custom frameworks
- **Responsive Design**: Adapts to all screen sizes with optimized mobile experience
- **Accessibility**: Full ARIA support, screen reader friendly, and reduced motion compliance
- **Performance Optimized**: Hardware-accelerated animations and efficient rendering
- **Customizable**: Extensive customization options for speed, direction, and appearance

## Parameters

### Content Parameters
- `Title`: Optional title displayed above the carousel
- `SectionTitle`: ARIA label for the section (default: "Customer Testimonials")
- `CustomTestimonials`: List of testimonial items (uses samples if not provided)

### Animation Parameters
- `AnimationDuration`: Animation duration in seconds (default: 60)
- `Direction`: Animation direction - Right or Left (default: Right)
- `Speed`: Animation speed - Slow, Normal, Fast (default: Normal)
- `PauseOnHover`: Whether to pause animation on hover (default: true)

### Layout Parameters
- `CardWidth`: Width of each testimonial card in pixels (default: 400)
- `CardGap`: Gap between cards in pixels (default: 24)
- `MaxVisibleTestimonials`: Maximum number of testimonials to show (performance optimization)

### Card Style Parameters
- `CardVariant`: Testimonial card variant (Default, Minimal, Highlighted, Compact)
- `CardSize`: Testimonial card size (Small, Normal, Large)
- `CardElevated`: Whether cards have shadow elevation (default: true)
- `CardBorderless`: Whether cards are borderless (default: false)

## Usage Examples

### Basic Testimonial Carousel
```razor
<OsirionTestimonialCarousel Title="What Our Customers Say" />
```

### Custom Testimonials with Styling
```razor
<OsirionTestimonialCarousel 
    Title="Client Success Stories"
    CardVariant="TestimonialVariant.Highlighted"
    CardSize="TestimonialSize.Large"
    Speed="CarouselSpeed.Slow"
    Direction="AnimationDirection.Left"
    CustomTestimonials="@testimonialList" />
```

### Minimal Design for Sidebar
```razor
<OsirionTestimonialCarousel 
    CardVariant="TestimonialVariant.Minimal"
    CardSize="TestimonialSize.Small"
    CardBorderless="true"
    CardWidth="300"
    Speed="CarouselSpeed.Fast"
    PauseOnHover="false" />
```

### Performance Optimized for Large Lists
```razor
<OsirionTestimonialCarousel 
    Title="Featured Reviews"
    CustomTestimonials="@allTestimonials"
    MaxVisibleTestimonials="6"
    AnimationDuration="45"
    CardWidth="380" />
```

### Custom Testimonial Data
```csharp
private List<TestimonialItem> testimonialList = new()
{
    new("Sarah Johnson", 
        "Senior Developer", 
        "Tech Solutions Inc.",
        "This product has completely transformed our workflow.",
        "https://example.com/sarah.jpg",
        "https://linkedin.com/in/sarahjohnson",
        ShowRating: true,
        Rating: 5,
        ReadMoreHref: "/testimonials/sarah"),
        
    new("Michael Chen", 
        "CTO", 
        "Innovation Labs",
        "Exceptional quality and support. ROI was evident immediately.",
        "https://example.com/michael.jpg",
        ShowRating: true,
        Rating: 5)
};
```

## TestimonialItem Properties

The `TestimonialItem` record supports all the properties available in `OsirionTestimonialCard`:

### Required
- `Name`: Person's name

### Optional
- `Position`: Job title
- `Company`: Company name
- `TestimonialText`: The testimonial content
- `ProfileImageUrl`: Profile image URL
- `LinkedInUrl`: LinkedIn profile URL
- `ShowRating`: Whether to show star rating (default: false)
- `Rating`: Star rating 1-5 (default: 5)
- `ReadMoreHref`: URL for read more link
- `ReadMoreText`: Text for read more link (default: "Read more")
- `ReadMoreVariant`: Style variant for read more link
- `ReadMoreTarget`: Link target attribute
- `AdditionalCssClass`: Additional CSS classes for the card

## Animation Directions

### AnimationDirection.Right (Default)
Testimonials scroll from left to right, creating a natural reading flow.

### AnimationDirection.Left
Testimonials scroll from right to left, useful for RTL layouts or design variety.

## Speed Options

### CarouselSpeed.Slow
Extends animation duration by 30 seconds for a more relaxed pace.

### CarouselSpeed.Normal (Default)
Uses the specified `AnimationDuration` without modification.

### CarouselSpeed.Fast
Reduces animation duration by 30 seconds for a more dynamic feel.

## CSS Classes

The component generates semantic CSS classes for customization:

- `.osirion-testimonial-carousel`: Base container
- `.osirion-testimonial-carousel-pausable`: When hover pause is enabled
- `.osirion-testimonial-carousel-reverse`: When direction is left
- `.osirion-testimonial-carousel-{speed}`: Speed-specific classes
- `.osirion-testimonial-carousel-track`: The scrolling container
- `.osirion-testimonial-carousel-slide`: Individual testimonial wrapper

## Responsive Behavior

### Desktop (1200px+)
- Card width: 450px
- Gap: 32px
- Full mask gradient

### Tablet (768px - 1199px)
- Card width: 400px (default)
- Gap: 24px (default)
- Standard layout

### Mobile (480px - 767px)
- Card width: 320px
- Gap: 16px
- Adjusted mask gradient
- Smaller title font

### Small Mobile (<480px)
- Card width: 280px
- Gap: 12px
- Optimized spacing

## Accessibility Features

### ARIA Support
- Proper `role="list"` and `role="listitem"` semantics
- Section labeled with `aria-label`
- Duplicate items marked with `aria-hidden="true"`

### Screen Reader Friendly
- Logical content structure
- Descriptive section titles
- Proper heading hierarchy

### Reduced Motion
- Respects `prefers-reduced-motion` setting
- Converts to static grid layout when motion is reduced
- Shows limited number of testimonials in static mode

### Keyboard Navigation
- Focus management for interactive elements
- Logical tab order through testimonials

## Performance Features

### Hardware Acceleration
- Uses `transform` for animations (GPU accelerated)
- `will-change: transform` for optimal performance
- `backface-visibility: hidden` prevents flickering

### Efficient Rendering
- CSS-only animations (no JavaScript required)
- Minimal DOM manipulation
- Optimized for SSR environments

### Memory Management
- `MaxVisibleTestimonials` parameter for large datasets
- Duplicate testimonials only when needed for smooth looping
- CSS containment for performance isolation

## Advanced Customization

### Custom Animation Duration
```razor
<OsirionTestimonialCarousel 
    AnimationDuration="90"
    Speed="CarouselSpeed.Slow" />
<!-- Results in 120-second animation (90 + 30) -->
```

### Responsive Card Sizing
```razor
<OsirionTestimonialCarousel 
    CardWidth="400"
    CardGap="32"
    Class="custom-testimonial-carousel" />
```

### Framework-Specific Styling
The component automatically applies appropriate classes based on your CSS framework and inherits all framework-specific styling from `OsirionTestimonialCard`.

## Browser Support

- **Modern Browsers**: Full support with hardware acceleration
- **Legacy Browsers**: Graceful degradation with standard animations
- **Safari**: Full support including mask gradients
- **Mobile**: Optimized for touch devices with proper scaling

## Best Practices

1. **Content Length**: Keep testimonials concise for better readability in carousel format
2. **Image Optimization**: Use optimized, consistent-sized profile images
3. **Performance**: Use `MaxVisibleTestimonials` for large datasets (>10 items)
4. **Accessibility**: Always provide meaningful `Title` for context
5. **Responsive**: Test carousel on various screen sizes during development

## Integration with Sections

The carousel works perfectly within `OsirionBaseSection`:

```razor
<OsirionBaseSection 
    Id="testimonials"
    Title="Customer Success Stories"
    Description="See what our clients say about working with us"
    Padding="SectionPadding.Large">
    
    <OsirionTestimonialCarousel 
        CardVariant="TestimonialVariant.Highlighted"
        CustomTestimonials="@featuredTestimonials" />
</OsirionBaseSection>
```