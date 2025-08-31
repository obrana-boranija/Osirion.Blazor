# OsirionContactInfoSection Component

[![Component](https://img.shields.io/badge/Component-Core-blue)](https://github.com/obrana-boranija/Osirion.Blazor/tree/master/src/Osirion.Blazor.Core)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor.Core)](https://www.nuget.org/packages/Osirion.Blazor.Core)

The `OsirionContactInfoSection` component is a dedicated component for displaying contact information with consistent styling and accessibility features. It's designed to be used as a standalone section or as part of contact forms and landing pages.

## Features

- **Complete Contact Display**: Address, phone, email, and website information
- **Customizable Labels**: All field labels can be customized with HTML markup support
- **Accessibility First**: Full ARIA support and semantic HTML
- **Responsive Design**: Automatically adapts to different screen sizes
- **Framework Agnostic**: Works with all CSS frameworks or standalone
- **Dark Theme Support**: Built-in dark theme styling
- **Icon Integration**: SVG icons for each contact type
- **Link Handling**: Automatic tel: and mailto: link generation
- **HTML Markup Support**: All text fields support HTML markup using MarkupString

## Basic Usage

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<OsirionContactInfoSection 
    Title="Contact Information"
    ContactInfo="@contactInfo" />

@code {
    private ContactInformation contactInfo = new()
    {
        Address = "123 Main Street, City, State 12345",
        Phone = "+1 (555) 123-4567",
        Email = "contact@example.com",
        Website = "https://example.com",
        Message = "We're here to help you succeed!"
    };
}
```

## Advanced Usage

### With Custom Labels and HTML Markup

```razor
<OsirionContactInfoSection 
    Title="Get in Touch"
    ContactInfo="@contactInfo"
    AddressLabel="<strong>Our Location:</strong>"
    PhoneLabel="<strong>Call Us:</strong>"
    EmailLabel="<strong>Email Us:</strong>"
    WebsiteLabel="<strong>Visit:</strong>"
    UseDarkTheme="true" />

@code {
    private ContactInformation contactInfo = new()
    {
        Address = "123 Business Plaza<br/>Suite 456<br/>Downtown District, NY 10001",
        Phone = "+1 (555) 123-4567",
        Email = "info@business.com",
        Website = "https://business.com",
        Message = "Office hours: <strong>Monday - Friday</strong>, 9 AM - 6 PM EST"
    };
}
```

### Multilingual Support

```razor
<OsirionContactInfoSection 
    Title="Kontaktinformationen"
    ContactInfo="@germanContact"
    AddressLabel="Adresse:"
    PhoneLabel="Telefon:"
    EmailLabel="E-Mail:"
    WebsiteLabel="Website:" />

@code {
    private ContactInformation germanContact = new()
    {
        Address = "Musterstraﬂe 123<br/>12345 Berlin, Deutschland",
        Phone = "+49 30 12345678",
        Email = "kontakt@beispiel.de",
        Website = "https://beispiel.de",
        Message = "Wir sind hier, um Ihnen zu helfen!"
    };
}
```

### Minimal Contact Info

```razor
@* Only showing phone and email *@
<OsirionContactInfoSection 
    Title="Quick Contact"
    ContactInfo="@minimalContact"
    PhoneLabel="?? Call:"
    EmailLabel="?? Email:" />

@code {
    private ContactInformation minimalContact = new()
    {
        Phone = "+1 (555) 123-4567",
        Email = "support@example.com",
        Message = "Available <em>Monday - Friday</em>, 9 AM - 5 PM"
    };
}
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string` | `"Contact information"` | Section title |
| `ContactInfo` | `ContactInformation?` | `null` | Contact information object (required) |
| `UseDarkTheme` | `bool` | `false` | Enable dark theme styling |
| `AddressLabel` | `string` | `"Address:"` | Label for address field (supports HTML) |
| `PhoneLabel` | `string` | `"Phone:"` | Label for phone field (supports HTML) |
| `EmailLabel` | `string` | `"Email:"` | Label for email field (supports HTML) |
| `WebsiteLabel` | `string` | `"Website:"` | Label for website field (supports HTML) |

### Inherited Parameters
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string?` | `null` | Additional CSS classes |
| `Style` | `string?` | `null` | Inline styles |
| `Attributes` | `Dictionary<string, object>?` | `null` | Additional HTML attributes |

## ContactInformation Model

The `ContactInformation` model supports the following properties (all support HTML markup):

```csharp
public class ContactInformation
{
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
```

### Field Behavior

- **Address**: Displays with location icon, supports line breaks with `<br/>`
- **Phone**: Displays with phone icon, creates `tel:` link, supports HTML formatting
- **Email**: Displays with email icon, creates `mailto:` link, supports HTML formatting
- **Website**: Displays with globe icon, creates external link with `target="_blank"`
- **Message**: Optional descriptive text displayed above contact items, supports full HTML

## HTML Markup Support

All text fields in the component support HTML markup through `MarkupString`:

```razor
@code {
    private ContactInformation richContact = new()
    {
        Address = "<strong>Main Office</strong><br/>123 Business Ave<br/><em>Suite 100</em><br/>City, State 12345",
        Phone = "+1 (555) <strong>CALL-NOW</strong>",
        Email = "<span style='color: blue;'>info@company.com</span>",
        Message = "We're <strong>always</strong> here to help!<br/><small>Response time: &lt; 24 hours</small>"
    };
}
```

## Styling and Themes

### Light Theme (Default)
- Clean, minimal design with subtle borders
- Uses system/framework colors
- High contrast for accessibility

### Dark Theme
- Dark background with light text
- Accent colors for better visibility
- Optimized for dark interfaces

### CSS Variables

Customize the component with CSS variables:

```css
:root {
    /* Background colors */
    --osirion-contact-bg: #f9fafb;
    --osirion-contact-text: #111827;
    --osirion-contact-text-muted: #6b7280;
    --osirion-contact-accent: #2563eb;
    
    /* Spacing */
    --osirion-contact-spacing: 1.5rem;
    
    /* Typography */
    --osirion-font-size-xl: 1.25rem;
    --osirion-font-size-sm: 0.875rem;
    --osirion-font-weight-semibold: 600;
    --osirion-font-weight-medium: 500;
}

/* Dark theme overrides */
.osirion-contact-info-dark {
    --osirion-contact-bg: #374151;
    --osirion-contact-text: #ffffff;
    --osirion-contact-text-muted: #9ca3af;
    --osirion-contact-accent: #f59e0b;
}
```

## Icons

The component includes built-in SVG icons for each contact type:

- **Address**: Location pin icon
- **Phone**: Phone icon
- **Email**: Envelope icon
- **Website**: Globe icon

Icons are styled with the accent color and scale appropriately.

## Accessibility Features

- **Semantic HTML**: Uses proper structure with headings and lists
- **ARIA Labels**: Screen reader friendly
- **Keyboard Navigation**: All links are keyboard accessible
- **Color Contrast**: High contrast mode support
- **Focus Indicators**: Clear focus states for interactive elements

## Framework Integration

### Bootstrap
```razor
<OsirionContactInfoSection 
    ContactInfo="@contactInfo"
    Class="bg-light border rounded p-4 shadow-sm" />
```

### Fluent UI
```razor
<OsirionContactInfoSection 
    ContactInfo="@contactInfo"
    Class="ms-depth-8 ms-bgColor-neutralLighter" />
```

### MudBlazor
```razor
<MudPaper Class="pa-4">
    <OsirionContactInfoSection ContactInfo="@contactInfo" />
</MudPaper>
```

## Responsive Behavior

The component automatically adapts to different screen sizes:

- **Desktop**: Full layout with icons and labels
- **Tablet**: Maintains structure with adjusted spacing
- **Mobile**: Stacked layout with optimized touch targets

## Real-World Examples

### Business Contact Section with Rich Formatting
```razor
<OsirionContactInfoSection 
    Title="Visit Our Office"
    ContactInfo="@businessContact"
    AddressLabel="<i class='fas fa-map-marker-alt'></i> Location:"
    PhoneLabel="<i class='fas fa-phone'></i> Call Us:"
    EmailLabel="<i class='fas fa-envelope'></i> Email:"
    WebsiteLabel="<i class='fas fa-globe'></i> Website:" />

@code {
    private ContactInformation businessContact = new()
    {
        Address = "<strong>Corporate Headquarters</strong><br/>123 Business Plaza<br/>Suite 456<br/>Downtown District, NY 10001",
        Phone = "+1 (555) <strong>123-4567</strong>",
        Email = "<span style='color: #0066cc;'>info@business.com</span>",
        Website = "https://business.com",
        Message = "<strong>Office hours:</strong><br/>Monday - Friday: 9 AM - 6 PM EST<br/><em>Emergency support available 24/7</em>"
    };
}
```

### Event Contact Information
```razor
<OsirionContactInfoSection 
    Title="Event Information"
    ContactInfo="@eventContact"
    AddressLabel="?? Venue:"
    PhoneLabel="?? Hotline:"
    EmailLabel="?? Event Email:"
    Class="border-start border-primary border-4 ps-3" />

@code {
    private ContactInformation eventContact = new()
    {
        Address = "<strong>Convention Center</strong><br/>Hall A - Main Floor<br/>789 Event Blvd<br/>Event City, EC 54321",
        Phone = "+1 (555) <strong>EVENT-01</strong>",
        Email = "<strong>events@example.com</strong>",
        Message = "??? <strong>Registration opens at 8:00 AM</strong><br/><small>Please arrive 30 minutes early</small>"
    };
}
```

### Support Contact with Styling
```razor
<OsirionContactInfoSection 
    Title="Need Help?"
    ContactInfo="@supportContact"
    UseDarkTheme="true"
    PhoneLabel="<span style='color: #10b981;'>?? Emergency:</span>"
    EmailLabel="<span style='color: #3b82f6;'>?? Support:</span>"
    WebsiteLabel="<span style='color: #8b5cf6;'>?? Help Center:</span>" />

@code {
    private ContactInformation supportContact = new()
    {
        Phone = "+1 (555) <strong style='color: #ef4444;'>987-6543</strong>",
        Email = "<strong>support@example.com</strong>",
        Website = "https://help.example.com",
        Message = "<strong style='color: #10b981;'>24/7 support</strong> available for all customers<br/><small>Average response time: &lt; 2 hours</small>"
    };
}
```

## Best Practices

1. **Complete Information**: Provide at least one contact method
2. **Consistent Formatting**: Use standard phone number and address formats
3. **Accessible Links**: Ensure all links have meaningful text
4. **Mobile Optimization**: Test contact links work on mobile devices
5. **Update Regularly**: Keep contact information current
6. **Clear Messaging**: Use the Message field for important context
7. **HTML Safety**: Be cautious with user-generated HTML content
8. **Label Consistency**: Keep label formatting consistent across your application

## Performance Considerations

- **Lightweight**: Minimal CSS and no JavaScript dependencies
- **SSR Compatible**: Renders completely on the server
- **Icon Optimization**: SVG icons are inline for performance
- **Responsive Images**: No images required, uses SVG icons
- **MarkupString**: Efficient HTML rendering through Blazor's MarkupString

## Integration with OsirionContactForm

This component was extracted from the `OsirionContactForm` component and can be used independently or as part of the contact form:

```razor
<!-- Standalone usage with custom labels -->
<OsirionContactInfoSection 
    ContactInfo="@contactInfo"
    AddressLabel="Our Office:"
    PhoneLabel="Call Us:"
    EmailLabel="Email Us:"
    WebsiteLabel="Visit Us:" />

<!-- As part of contact form -->
<OsirionContactForm 
    ContactInfo="@contactInfo"
    ShowContactInformation="true" />
```

The contact form automatically uses this component internally when contact information is provided, but uses the default labels.