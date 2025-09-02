# ContentRenderer

Renders content item HTML using the OsirionHtmlRenderer component. This is a low-level component that focuses purely on content rendering without additional metadata or navigation features.

## Basic Usage

```razor
<ContentRenderer Item="@contentItem" />
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Item` | `ContentItem?` | `null` | The content item to render |

## Features

- **Pure Content Rendering**: Focuses solely on rendering the HTML content
- **OsirionHtmlRenderer Integration**: Uses the core HTML renderer for consistent output
- **CSS Isolation**: Scoped styling with CSS variables
- **Responsive Design**: Mobile-first responsive layout
- **Accessibility**: Semantic HTML structure with proper heading hierarchy

## Examples

### Basic Content Rendering

```razor
@inject IContentProviderManager ContentProviderManager

<ContentRenderer Item="@currentItem" />

@code {
    private ContentItem? currentItem;
    
    protected override async Task OnInitializedAsync()
    {
        var provider = ContentProviderManager.GetDefaultProvider();
        currentItem = await provider.GetItemByPathAsync("blog/my-post.md");
    }
}
```

### With Custom CSS Classes

```razor
<ContentRenderer 
    Item="@contentItem" 
    Class="custom-content-wrapper" />
```

### In a Card Layout

```razor
<div class="card">
    <div class="card-header">
        <h2>@contentItem.Title</h2>
    </div>
    <div class="card-body">
        <ContentRenderer Item="@contentItem" />
    </div>
</div>
```

## CSS Customization

The component uses CSS variables for customization:

```css
.osirion-content-view {
    /* Content wrapper */
    line-height: var(--osirion-line-height-relaxed, 1.7);
}

.osirion-content-body {
    /* Content body styles */
    line-height: var(--osirion-line-height-relaxed, 1.7);
    color: var(--osirion-text-primary, #374151);
    margin-bottom: var(--osirion-spacing-8, 2rem);
}

/* Heading styles */
.osirion-content-body h2 {
    margin-top: 1.8em;
    margin-bottom: 0.8em;
    font-size: var(--osirion-font-size-2xl, 1.5rem);
    font-weight: var(--osirion-font-weight-bold, 700);
}

.osirion-content-body h3 {
    margin-top: 1.5em;
    margin-bottom: 0.6em;
    font-size: var(--osirion-font-size-xl, 1.25rem);
    font-weight: var(--osirion-font-weight-bold, 700);
}

/* Paragraph and image styles */
.osirion-content-body p {
    margin-bottom: 1.2em;
}

.osirion-content-body img {
    max-width: 100%;
    height: auto;
    border-radius: var(--osirion-radius-md, 0.375rem);
    margin: var(--osirion-spacing-4, 1rem) 0;
}

/* Link styles */
.osirion-content-body a {
    color: var(--osirion-action-primary, #0369a1);
    text-decoration: none;
    transition: color var(--osirion-transition-duration-200, 0.2s) ease;
}

.osirion-content-body a:hover {
    text-decoration: underline;
    color: var(--osirion-action-primary-hover, #0284c7);
}
```

## Accessibility Features

- **Semantic HTML**: Uses proper HTML structure for content
- **Reduced Motion**: Respects `prefers-reduced-motion` settings
- **Print Styles**: Optimized for printing
- **Screen Reader**: Compatible with assistive technologies

## When to Use

Use `ContentRenderer` when you need:

- **Pure Content Display**: Just render the content without metadata
- **Custom Layouts**: Building your own content page structure
- **Embedded Content**: Including content within other components
- **Simple Integration**: Minimal overhead for content rendering

## When Not to Use

Consider other components for:

- **Full Page Display**: Use `ContentPage` for complete page layouts
- **Rich Metadata**: Use `ContentView` for content with metadata display
- **Navigation Features**: Use page-level components for navigation
- **SEO Requirements**: Use components that include SEO metadata

## Related Components

- [ContentView](ContentView.md) - Rich content display with metadata
- [ContentPage](ContentPage.md) - Complete page layout for content
- [OsirionHtmlRenderer](../../core/OsirionHtmlRenderer.md) - Underlying HTML renderer

## Implementation Details

The component is implemented as a simple wrapper around `OsirionHtmlRenderer`:

```razor
<div class="@GetContentViewClass()">
    <div class="osirion-content-body">
        @if (Item?.Content is not null)
        {
            <OsirionHtmlRenderer HtmlContent="@Item.Content" />
        }
    </div>
</div>
```

This ensures consistent content rendering across all CMS components while providing the flexibility to build custom layouts around the core content display.
