# OsirionSubscriptionCard Component

A comprehensive subscription card component with validation, accessibility, and framework-agnostic styling. Perfect for newsletter signups, email subscriptions, and user engagement.

## Features

- ? **Email Validation** - Built-in email validation with custom error messages
- ? **Optional Name Field** - Configurable name field for personalized subscriptions
- ? **Subscription Categories** - Allow users to choose subscription preferences
- ? **Privacy Policy Agreement** - GDPR-compliant privacy policy checkbox
- ? **Spam Protection** - Honeypot field for anti-spam protection
- ? **Accessibility** - Full ARIA support and keyboard navigation
- ? **Responsive Design** - Mobile-first responsive layout
- ? **Framework Integration** - Works with Bootstrap, MudBlazor, and standalone
- ? **Custom Styling** - CSS custom properties for easy theming
- ? **Dark Theme Support** - Built-in dark theme variant
- ? **Compact Layout** - Space-saving compact variant
- ? **Status Messages** - Success/error messaging with icons
- ? **Event Callbacks** - Extensible event system for custom handling

## Basic Usage

### Simple Newsletter Signup

```razor
<OsirionSubscriptionCard Id="newsletter-signup"
                        Title="Stay Updated"
                        Description="Get the latest updates delivered to your inbox."
                        OnSubscribe="HandleSubscription" />

@code {
    private async Task HandleSubscription(SubscriptionModel subscription)
    {
        // Handle the subscription (save to database, call API, etc.)
        Logger.LogInformation("New subscription: {Email}", subscription.Email);
    }
}
```

### With Name Field and Categories (Custom Dropdown)

```razor
<OsirionSubscriptionCard Id="newsletter-signup"
                        Title="Join Our Community"
                        Description="Choose what you'd like to receive from us."
                        ShowNameField="true"
                        ShowSubscriptionCategories="true"
                        ShowCategoriesAsDropdown="true"
                        CategoriesDropdownText="Choose your interests"
                        SubscriptionCategories="Categories"
                        OnSubscribe="HandleSubscription" />

@code {
    private IReadOnlyList<SubscriptionCategory> Categories { get; set; } = new[]
    {
        new SubscriptionCategory
        {
            Id = "newsletter",
            Name = "Weekly Newsletter",
            Description = "Our weekly roundup of news and updates",
            IsDefault = true
        },
        new SubscriptionCategory
        {
            Id = "product-updates",
            Name = "Product Updates",
            Description = "New features and improvements",
            IsDefault = true,
            IsRequired = true // Cannot be unchecked
        },
        new SubscriptionCategory
        {
            Id = "tips",
            Name = "Tips & Tutorials",
            Description = "Best practices and how-to guides",
            IsDefault = false
        }
    };

    private async Task HandleSubscription(SubscriptionModel subscription)
    {
        // subscription.Name - optional name
        // subscription.Email - required email
        // subscription.SubscriptionCategories - selected category IDs
        // subscription.AgreeToPrivacyPolicy - privacy agreement
        
        await MySubscriptionService.SubscribeAsync(subscription);
    }
}
```

### With Name Field and Categories

```razor
<OsirionSubscriptionCard Id="newsletter-signup"
                        Title="Join Our Community"
                        Description="Choose what you'd like to receive from us."
                        ShowNameField="true"
                        ShowSubscriptionCategories="true"
                        SubscriptionCategories="Categories"
                        OnSubscribe="HandleSubscription" />

@code {
    private IReadOnlyList<SubscriptionCategory> Categories { get; set; } = new[]
    {
        new SubscriptionCategory
        {
            Id = "newsletter",
            Name = "Weekly Newsletter",
            Description = "Our weekly roundup of news and updates",
            IsDefault = true
        },
        new SubscriptionCategory
        {
            Id = "product-updates",
            Name = "Product Updates",
            Description = "New features and improvements",
            IsDefault = true,
            IsRequired = true // Cannot be unchecked
        },
        new SubscriptionCategory
        {
            Id = "tips",
            Name = "Tips & Tutorials",
            Description = "Best practices and how-to guides",
            IsDefault = false
        }
    };

    private async Task HandleSubscription(SubscriptionModel subscription)
    {
        // subscription.Name - optional name
        // subscription.Email - required email
        // subscription.SubscriptionCategories - selected category IDs
        // subscription.AgreeToPrivacyPolicy - privacy agreement
        
        await MySubscriptionService.SubscribeAsync(subscription);
    }
}
```

## Layout Variants

### Compact Layout

```razor
<OsirionSubscriptionCard Id="compact-signup"
                        Title="Quick Subscribe"
                        CompactLayout="true"
                        ShowBorder="false"
                        OnSubscribe="HandleSubscription" />
```

### Dark Theme

```razor
<OsirionSubscriptionCard Id="dark-signup"
                        Title="Join Us"
                        UseDarkTheme="true"
                        ShowShadow="true"
                        OnSubscribe="HandleSubscription" />
```

### Custom Styling

```razor
<OsirionSubscriptionCard Id="custom-signup"
                        Title="Subscribe"
                        Class="my-custom-subscription"
                        Style="max-width: 350px; margin: 0 auto;"
                        OnSubscribe="HandleSubscription">
    <!-- Custom footer content -->
    <p>We respect your privacy and will never spam you.</p>
</OsirionSubscriptionCard>
```

## Advanced Configuration

### Custom Messages and Validation

```razor
<OsirionSubscriptionCard Id="advanced-signup"
                        Title="Premium Newsletter"
                        Description="Join thousands of subscribers getting exclusive content."
                        EmailPlaceholder="Enter your best email address"
                        SubscribeButtonText="Get Exclusive Access"
                        SuccessMessage="Welcome! Check your email to confirm your subscription."
                        ErrorMessage="Oops! Something went wrong. Please try again."
                        AlreadySubscribedMessage="You're already on our list!"
                        PrivacyPolicyText="I agree to the <a href='/privacy' target='_blank'>Privacy Policy</a> and <a href='/terms' target='_blank'>Terms of Service</a>"
                        OnSubscribe="HandleSubscription"
                        OnValidationFailed="HandleValidationError" />

@code {
    private async Task HandleValidationError(EditContext context)
    {
        Logger.LogWarning("Subscription validation failed");
        // Handle validation errors
    }
}
```

### Email Service Integration

```razor
<OsirionSubscriptionCard Id="auto-email-signup"
                        Title="Automated Signup"
                        UseBuiltInEmailService="true"
                        OnSubscribe="HandleSubscription" />
```

## Parameters

### Core Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | `"subscription-card"` | **Required.** Unique identifier for the subscription card |
| `Title` | `string` | `"Stay Updated"` | Title displayed at the top |
| `Description` | `string` | `"Get the latest updates..."` | Description text below title |

### Form Configuration

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowNameField` | `bool` | `false` | Whether to show the optional name field |
| `NamePlaceholder` | `string` | `"Your name (optional)"` | Placeholder for name field |
| `EmailPlaceholder` | `string` | `"Enter your email address"` | Placeholder for email field |
| `SubscribeButtonText` | `string` | `"Subscribe"` | Text for the subscribe button |
| `SubscribingText` | `string` | `"Subscribing..."` | Button text while submitting |
| `ShowRequiredIndicators` | `bool` | `true` | Whether to show asterisks for required fields |

### Subscription Categories

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowSubscriptionCategories` | `bool` | `false` | Whether to show category selection |
| `ShowCategoriesAsDropdown` | `bool` | `false` | Whether to show categories as dropdown instead of inline checkboxes |
| `SubscriptionCategories` | `IReadOnlyList<SubscriptionCategory>?` | `null` | Available subscription categories |
| `CategoriesText` | `string` | `"What would you like to receive?"` | Text above categories |
| `CategoriesDropdownText` | `string` | `"Select Categories"` | Text for dropdown button when no categories selected |

### Privacy and Compliance

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `PrivacyPolicyText` | `string?` | `"I agree to the..."` | Privacy policy agreement text (HTML supported) |

### Messages

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `SuccessMessage` | `string` | `"Thank you for subscribing!"` | Success message |
| `ErrorMessage` | `string` | `"Sorry, there was an error..."` | Error message |
| `SpamMessage` | `string` | `"Your submission appears to be spam..."` | Spam detection message |
| `AlreadySubscribedMessage` | `string` | `"This email address is already subscribed..."` | Already subscribed message |

### Layout and Styling

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `CompactLayout` | `bool` | `false` | Use compact spacing and sizing |
| `UseDarkTheme` | `bool` | `false` | Apply dark theme styling |
| `ShowBorder` | `bool` | `true` | Show card border |
| `ShowShadow` | `bool` | `true` | Show card shadow |

### Integration

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `UseBuiltInEmailService` | `bool` | `false` | Use registered email service for handling |

### Events

| Parameter | Type | Description |
|-----------|------|-------------|
| `OnSubscribe` | `EventCallback<SubscriptionModel>` | Triggered when form is submitted with valid data |
| `OnValidationFailed` | `EventCallback<EditContext>` | Triggered when validation fails |

## Models

### SubscriptionModel

```csharp
public class SubscriptionModel
{
    public string? Name { get; set; }                           // Optional name
    public string Email { get; set; } = string.Empty;          // Required email
    public bool AgreeToPrivacyPolicy { get; set; }             // Privacy agreement
    public List<string> SubscriptionCategories { get; set; }   // Selected categories
    public string Website { get; set; } = string.Empty;        // Honeypot (should be empty)
}
```

### SubscriptionCategory

```csharp
public class SubscriptionCategory
{
    public string Id { get; set; } = string.Empty;           // Unique identifier
    public string Name { get; set; } = string.Empty;         // Display name
    public string Description { get; set; } = string.Empty;  // Category description
    public bool IsDefault { get; set; }                      // Selected by default
    public bool IsRequired { get; set; }                     // Cannot be unselected
}
```

### SubscriptionSubmissionResult

```csharp
public enum SubscriptionSubmissionResult
{
    None = 0,
    Success = 1,
    ValidationError = 2,
    ServerError = 3,
    SpamDetected = 4,
    Processing = 5,
    AlreadySubscribed = 6
}
```

## CSS Customization

The component uses CSS custom properties for easy theming:

```css
.osirion-subscription-card {
    --osirion-subscription-card-padding: 1.5rem;
    --osirion-subscription-card-border-radius: 0.5rem;
    --osirion-subscription-card-background: var(--osirion-color-background, #ffffff);
    --osirion-subscription-card-border-color: var(--osirion-color-border, #e5e7eb);
    --osirion-subscription-card-shadow: 0 1px 3px 0 rgb(0 0 0 / 0.1);
}

/* Custom theme example */
.my-custom-subscription {
    --osirion-subscription-card-background: #f8fafc;
    --osirion-subscription-card-border-color: #3b82f6;
    border-width: 2px;
}
```

## Examples

### Newsletter Signup with Social Proof

```razor
<OsirionSubscriptionCard Id="newsletter"
                        Title="Join 10,000+ Subscribers"
                        Description="Get weekly insights delivered to your inbox."
                        ShowNameField="true"
                        SuccessMessage="Welcome to the community! Check your email to confirm."
                        OnSubscribe="HandleNewsletterSignup">
    <p><small>Join developers from Google, Microsoft, and Amazon</small></p>
</OsirionSubscriptionCard>
```

### Product Updates with Categories

```razor
<OsirionSubscriptionCard Id="product-updates"
                        Title="Product Updates"
                        Description="Stay informed about new features and improvements."
                        ShowSubscriptionCategories="true"
                        SubscriptionCategories="UpdateCategories"
                        CompactLayout="true"
                        OnSubscribe="HandleProductUpdates" />

@code {
    private IReadOnlyList<SubscriptionCategory> UpdateCategories { get; set; } = new[]
    {
        new SubscriptionCategory
        {
            Id = "features",
            Name = "New Features",
            Description = "Major feature releases and announcements",
            IsDefault = true,
            IsRequired = true
        },
        new SubscriptionCategory
        {
            Id = "bugfixes",
            Name = "Bug Fixes",
            Description = "Important bug fixes and patches",
            IsDefault = false
        },
        new SubscriptionCategory
        {
            Id = "security",
            Name = "Security Updates",
            Description = "Security patches and advisories",
            IsDefault = true
        }
    };
}
```

### Marketing Campaign with Custom Styling

```razor
<div class="campaign-container">
    <OsirionSubscriptionCard Id="campaign"
                            Title="?? Early Access"
                            Description="Be the first to try our new features!"
                            EmailPlaceholder="Your email for early access"
                            SubscribeButtonText="Get Early Access"
                            UseDarkTheme="true"
                            ShowShadow="true"
                            Class="campaign-signup"
                            OnSubscribe="HandleCampaignSignup">
        <div class="campaign-footer">
            <p>? Exclusive features</p>
            <p>? Priority support</p>
            <p>? Cancel anytime</p>
        </div>
    </OsirionSubscriptionCard>
</div>

<style>
    .campaign-container {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        padding: 2rem;
        border-radius: 1rem;
    }
    
    .campaign-signup {
        margin: 0 auto;
        max-width: 400px;
    }
    
    .campaign-footer {
        text-align: left;
        font-size: 0.875rem;
    }
    
    .campaign-footer p {
        margin: 0.25rem 0;
        color: var(--osirion-color-success);
    }
</style>
```

## Integration Examples

### With Custom Validation

```razor
<OsirionSubscriptionCard Id="validated-signup"
                        Title="Premium Subscription"
                        ShowNameField="true"
                        OnSubscribe="HandleValidatedSubscription"
                        OnValidationFailed="HandleValidationFailed" />

@code {
    private async Task HandleValidatedSubscription(SubscriptionModel subscription)
    {
        // Custom validation
        if (await IsEmailAlreadySubscribed(subscription.Email))
        {
            // Navigate to already subscribed state
            NavigationManager.NavigateTo(
                NavigationManager.GetUriWithQueryParameter("SubmissionResultValue", 
                (int)SubscriptionSubmissionResult.AlreadySubscribed), 
                forceLoad: true);
            return;
        }
        
        // Process subscription
        await SubscriptionService.SubscribeAsync(subscription);
    }
    
    private async Task HandleValidationFailed(EditContext context)
    {
        // Log validation errors
        foreach (var message in context.GetValidationMessages())
        {
            Logger.LogWarning("Validation error: {Message}", message);
        }
    }
    
    private async Task<bool> IsEmailAlreadySubscribed(string email)
    {
        return await SubscriptionService.EmailExistsAsync(email);
    }
}
```

### With Email Service Integration

```razor
<!-- Configure email service in Program.cs -->
@* 
builder.Services.AddOsirion()
    .AddEmailService(builder.Configuration);
*@

<OsirionSubscriptionCard Id="auto-email"
                        Title="Automated Newsletter"
                        UseBuiltInEmailService="true"
                        OnSubscribe="LogSubscription" />

@code {
    private async Task LogSubscription(SubscriptionModel subscription)
    {
        // Email service handles the subscription automatically
        // This is just for additional logging/tracking
        Logger.LogInformation("New subscription processed: {Email}", subscription.Email);
        
        // Track in analytics
        await AnalyticsService.TrackEventAsync("newsletter_subscription", new 
        {
            email = subscription.Email,
            categories = subscription.SubscriptionCategories
        });
    }
}
```

## Accessibility Features

- **ARIA Labels**: Proper labeling for screen readers
- **Keyboard Navigation**: Full keyboard support
- **Focus Management**: Logical tab order
- **Error Announcements**: Validation errors announced to screen readers
- **Semantic HTML**: Uses proper form elements and fieldsets
- **Color Contrast**: Meets WCAG AA standards

## Browser Support

- Chrome 88+
- Firefox 85+
- Safari 14+
- Edge 88+

## Related Components

- [`OsirionContactForm`](./CONTACT_FORM.md) - Full contact form with message
- [`OsirionButton`](./BUTTON.md) - Standalone button component
- [`OsirionAlert`](./ALERT.md) - Status messages and notifications

## Static SSR Considerations

When using the OsirionSubscriptionCard in Static SSR mode:

### EnhancedNavigation Compatibility

If you're using the `EnhancedNavigation` component from `Osirion.Blazor.Navigation`, it may cause the page to scroll to the top when the subscription form is submitted. In Static SSR, we cannot prevent this behavior with JavaScript.

**Solutions:**