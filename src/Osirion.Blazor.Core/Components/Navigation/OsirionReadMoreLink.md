# OsirionReadMoreLink Component

The `OsirionReadMoreLink` component is a versatile, framework-agnostic navigation component for creating "read more" links, call-to-action buttons, and other directional links with optional icons.

## Features

- **Framework Agnostic**: Works seamlessly with Bootstrap, FluentUI, MudBlazor, Radzen, and custom CSS frameworks
- **Multiple Variants**: Default, Arrow, External, Download, Plain, and Button styles
- **Flexible Icon Support**: Built-in icons or custom icon content with left/right positioning
- **Responsive Design**: Mobile-friendly with proper touch targets
- **Accessibility**: Proper ARIA labels, focus states, and screen reader support
- **Animation Support**: Optional hover animations with reduced motion support
- **SSR Compatible**: Works with both server-side rendering and WebAssembly

## Parameters

### Required Parameters
- `Href`: The link URL (required)

### Content Parameters
- `Text`: Link text (default: "Read more")
- `AriaLabel`: Accessibility label (auto-generated if not provided)
- `ShowText`: Whether to show the text (default: true)
- `ChildContent`: Additional content to render inside the link

### Icon Parameters
- `ShowIcon`: Whether to show an icon (default: true)
- `IconPosition`: Icon position - Left or Right (default: Right)
- `IconContent`: Custom icon content (overrides default icons)

### Style Parameters
- `Variant`: Link style variant (Default, Arrow, External, Download, Plain, Button)
- `Size`: Link size (Small, Normal, Large)
- `Animated`: Whether to show hover animations (default: true)

### Layout Parameters
- `Stretched`: Whether the link should stretch to fill container (useful in cards)
- `Block`: Whether to display as block element
- `Target`: Link target (_blank, _self, etc.)

## Usage Examples

### Basic Read More Link
```razor
<OsirionReadMoreLink Href="/article/details" Text="Read full article" />
```

### Arrow Style with Custom Text
```razor
<OsirionReadMoreLink 
    Href="/learn-more" 
    Text="Learn More" 
    Variant="ReadMoreVariant.Arrow"
    Size="LinkSize.Large" />
```

### External Link
```razor
<OsirionReadMoreLink 
    Href="https://external-site.com" 
    Text="Visit External Site"
    Variant="ReadMoreVariant.External"
    Target="_blank" />
```

### Download Link
```razor
<OsirionReadMoreLink 
    Href="/files/document.pdf" 
    Text="Download PDF"
    Variant="ReadMoreVariant.Download"
    Target="_blank" />
```

### Button Style
```razor
<OsirionReadMoreLink 
    Href="/get-started" 
    Text="Get Started"
    Variant="ReadMoreVariant.Button"
    Size="LinkSize.Large" />
```

### Stretched Link in Card
```razor
<div class="card">
    <div class="card-body">
        <h5 class="card-title">Article Title</h5>
        <p class="card-text">Article excerpt...</p>
        <OsirionReadMoreLink 
            Href="/article/123" 
            Text="Read More"
            Stretched="true" />
    </div>
</div>
```

### Custom Icon
```razor
<OsirionReadMoreLink Href="/details" Text="View Details">
    <IconContent>
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
            <path d="M8 4a.5.5 0 0 1 .5.5v3h3a.5.5 0 0 1 0 1h-3v3a.5.5 0 0 1-1 0v-3h-3a.5.5 0 0 1 0-1h3v-3A.5.5 0 0 1 8 4"/>
        </svg>
    </IconContent>
</OsirionReadMoreLink>
```

### Icon on Left
```razor
<OsirionReadMoreLink 
    Href="/back" 
    Text="Go Back"
    IconPosition="IconPosition.Left"
    Variant="ReadMoreVariant.Arrow" />
```

### Plain Text Link
```razor
<OsirionReadMoreLink 
    Href="/details" 
    Text="More details"
    Variant="ReadMoreVariant.Plain"
    ShowIcon="false" />
```

### Block Link
```razor
<OsirionReadMoreLink 
    Href="/full-width-link" 
    Text="Full Width Link"
    Block="true"
    Size="LinkSize.Large" />
```

## Variants

### ReadMoreVariant.Default
Standard chevron right icon with primary color styling.

### ReadMoreVariant.Arrow
Arrow right icon with semi-bold text weight.

### ReadMoreVariant.External
External link icon with blue color scheme, perfect for links that open in new windows.

### ReadMoreVariant.Download
Download icon with green color scheme, ideal for file downloads.

### ReadMoreVariant.Plain
Plain text without default styling, inherits parent colors.

### ReadMoreVariant.Button
Button-like appearance with background, padding, and border.

## CSS Classes

The component automatically generates appropriate CSS classes:

- `.osirion-read-more`: Base component class
- `.osirion-read-more-{variant}`: Variant-specific styling
- `.osirion-read-more-{size}`: Size-specific styling
- `.osirion-read-more-stretched`: For stretched links
- `.osirion-read-more-block`: For block display
- `.osirion-read-more-animated`: For animated links

## Framework Integration

The component automatically detects your CSS framework and applies appropriate classes:

- **Bootstrap**: `icon-link icon-link-hover`, `stretched-link`
- **FluentUI**: `osirion-fluent-link`, `osirion-fluent-stretched`
- **MudBlazor**: `mud-link`, `mud-stretched`
- **Radzen**: `rz-link`, `rz-stretched`
- **Default**: `osirion-link`, `osirion-stretched`

## Accessibility Features

- **ARIA Labels**: Automatic or custom aria-label support
- **Focus Management**: Proper focus indicators and keyboard navigation
- **Screen Reader Support**: Icons marked with `aria-hidden="true"`
- **Color Contrast**: High contrast mode support
- **Touch Targets**: Mobile-friendly touch target sizes

## Responsive Behavior

- **Mobile**: Smaller font sizes and better touch targets
- **Text Wrapping**: Responsive text handling for longer labels
- **Icon Scaling**: Icons scale appropriately with text size

## Animation & Motion

- **Hover Effects**: Icon translation on hover (can be disabled)
- **Reduced Motion**: Respects `prefers-reduced-motion` user preference
- **Smooth Transitions**: CSS transitions for all interactive states

## Browser Support

Compatible with all modern browsers. Uses CSS custom properties with fallbacks for older browsers.

## Examples in Different Frameworks

### Bootstrap
```razor
<!-- Automatically gets Bootstrap classes -->
<OsirionReadMoreLink Href="/article" Text="Read More" />
<!-- Renders with: class="osirion-read-more icon-link icon-link-hover ..." -->
```

### Custom Framework
```razor
<!-- Works with any CSS framework -->
<OsirionReadMoreLink 
    Href="/article" 
    Text="Read More"
    Class="my-custom-link-class" />
```