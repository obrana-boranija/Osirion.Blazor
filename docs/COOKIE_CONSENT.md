# OsirionCookieConsent Component

[![Component](https://img.shields.io/badge/Component-Core-blue)](https://github.com/obrana-boranija/Osirion.Blazor/tree/master/src/Osirion.Blazor.Core)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor.Core)](https://www.nuget.org/packages/Osirion.Blazor.Core)

The `OsirionCookieConsent` component provides a comprehensive, GDPR-compliant cookie consent solution for Blazor applications. It's fully SSR-compatible and includes customizable consent categories, privacy policy integration, and form-based consent management.

## Features

- **GDPR Compliant**: Meets European privacy regulation requirements
- **Customizable Categories**: Necessary, Analytics, Marketing, and Preferences cookies
- **SSR Compatible**: Works perfectly with server-side rendering
- **Form-based Consent**: Server-side form submission for consent management
- **Privacy Policy Integration**: Link to your privacy policy
- **Consent Persistence**: Configurable consent expiry (default 365 days)
- **Customization Panel**: Detailed cookie category management
- **Accessibility**: Full ARIA support and keyboard navigation
- **Position Flexible**: Top or bottom positioning

## Basic Usage

```razor
@using Osirion.Blazor.Components

<!-- Simple cookie consent banner -->
<OsirionCookieConsent />

<!-- With custom messaging -->
<OsirionCookieConsent 
    Title="Privacy Preferences"
    Message="We use cookies to enhance your browsing experience and analyze our traffic. Please choose your preferences."
    AcceptButtonText="Accept All Cookies"
    DeclineButtonText="Reject All"
    PolicyLink="/privacy-policy"
    PolicyLinkText="Privacy Policy" />
```

## Advanced Usage

### Full Customization with Custom Categories

```razor
<OsirionCookieConsent 
    Title="Cookie Preferences"
    Message="We use various types of cookies to provide and improve our services. Please review and customize your preferences."
    AcceptButtonText="Accept Selected"
    DeclineButtonText="Reject All"
    CustomizeButtonText="Manage Preferences"
    SavePreferencesButtonText="Save My Choices"
    ShowDeclineButton="true"
    ShowCustomizeButton="true"
    ShowCustomizationPanel="true"
    PolicyLink="/privacy-policy"
    PolicyLinkText="Learn More About Cookies"
    Position="bottom"
    ConsentExpiryDays="90"
    Categories="@GetCustomCookieCategories()"
    ConsentEndpoint="/api/consent/cookies" />

@code {
    private List<CookieCategory> GetCustomCookieCategories()
    {
        return new List<CookieCategory>
        {
            new CookieCategory
            {
                Id = "necessary",
                Name = "Essential Cookies",
                Description = "These cookies are essential for the website to function properly and cannot be disabled.",
                IsRequired = true,
                IsEnabled = true
            },
            new CookieCategory
            {
                Id = "analytics",
                Name = "Analytics & Performance",
                Description = "These cookies help us understand how visitors interact with our website by collecting and reporting information anonymously.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "marketing",
                Name = "Marketing & Advertising",
                Description = "These cookies are used to show you relevant advertisements based on your interests and browsing behavior.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "personalization",
                Name = "Personalization",
                Description = "These cookies remember your preferences and settings to provide a personalized experience.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "social",
                Name = "Social Media",
                Description = "These cookies enable social media features and allow you to share content from our website.",
                IsRequired = false,
                IsEnabled = false
            }
        };
    }
}
```

### E-commerce Site Configuration

```razor
<OsirionCookieConsent 
    Title="Cookie Notice"
    Message="We use cookies to provide personalized shopping experiences, targeted advertising, and website analytics."
    AcceptButtonText="Accept & Continue Shopping"
    DeclineButtonText="Essential Only"
    CustomizeButtonText="Cookie Settings"
    PolicyLink="/cookie-policy"
    Categories="@GetEcommerceCookieCategories()"
    ConsentEndpoint="/api/user/cookie-consent"
    Position="bottom" />

@code {
    private List<CookieCategory> GetEcommerceCookieCategories()
    {
        return new List<CookieCategory>
        {
            new CookieCategory
            {
                Id = "necessary",
                Name = "Necessary",
                Description = "Required for basic site functionality, shopping cart, and checkout process.",
                IsRequired = true,
                IsEnabled = true
            },
            new CookieCategory
            {
                Id = "analytics",
                Name = "Analytics",
                Description = "Help us understand shopping behavior and improve our store experience.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "marketing",
                Name = "Marketing",
                Description = "Used for personalized product recommendations and targeted advertising.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "preferences",
                Name = "Preferences",
                Description = "Remember your shopping preferences, language, and currency settings.",
                IsRequired = false,
                IsEnabled = true // Enable by default for better UX
            }
        };
    }
}
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string` | `"Cookie Consent"` | Title displayed in the consent banner |
| `Message` | `string` | Default message | Main consent message text |
| `AcceptButtonText` | `string` | `"Accept All"` | Text for the accept button |
| `DeclineButtonText` | `string` | `"Decline"` | Text for the decline button |
| `CustomizeButtonText` | `string` | `"Customize"` | Text for the customize button |
| `SavePreferencesButtonText` | `string` | `"Save Preferences"` | Text for saving preferences |
| `ShowDeclineButton` | `bool` | `true` | Whether to show the decline button |
| `ShowCustomizeButton` | `bool` | `true` | Whether to show the customize button |
| `ShowCustomizationPanel` | `bool` | `true` | Whether to show the customization panel |
| `CustomizePanelTitle` | `string` | `"Cookie Preferences"` | Title for the customization panel |
| `PolicyLink` | `string?` | `null` | Link to privacy/cookie policy |
| `PolicyLinkText` | `string` | `"Learn more"` | Text for the policy link |
| `Categories` | `IReadOnlyList<CookieCategory>` | Default categories | Cookie categories configuration |
| `ConsentEndpoint` | `string` | `"/api/cookie-consent"` | Endpoint for form submission |
| `Position` | `string` | `"bottom"` | Banner position ("top" or "bottom") |
| `ConsentExpiryDays` | `int` | `365` | Number of days before consent expires |
| `Icon` | `RenderFragment?` | `null` | Custom icon content |

## CookieCategory Model

```csharp
public class CookieCategory
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsEnabled { get; set; }
}
```

## Server-Side Consent Handling

The component submits consent preferences via form POST to the specified endpoint. Create a server endpoint to handle consent:

### ASP.NET Core Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class CookieConsentController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SaveConsent([FromForm] CookieConsentModel model)
    {
        // Process consent preferences
        var preferences = new UserCookiePreferences
        {
            UserId = GetCurrentUserId(),
            ConsentDate = DateTime.UtcNow,
            ConsentVersion = model.ConsentVersion,
            Necessary = model.Necessary,
            Analytics = model.Analytics,
            Marketing = model.Marketing,
            Preferences = model.Preferences,
            ExpiryDate = DateTime.UtcNow.AddDays(365)
        };
        
        await _consentService.SaveUserPreferencesAsync(preferences);
        
        // Set consent cookie
        Response.Cookies.Append("osirion_cookie_consent", model.ConsentVersion, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(365),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        
        // Redirect back to the referring page
        return Redirect(model.ReturnUrl ?? "/");
    }
}

public class CookieConsentModel
{
    public string ConsentVersion { get; set; } = "1.0";
    public bool Necessary { get; set; } = true;
    public bool Analytics { get; set; }
    public bool Marketing { get; set; }
    public bool Preferences { get; set; }
    public string? ReturnUrl { get; set; }
}
```

### Minimal API (Alternative)

```csharp
app.MapPost("/api/cookie-consent", async (
    CookieConsentModel model,
    HttpContext context,
    ICookieConsentService consentService) =>
{
    // Save user preferences
    await consentService.SaveConsentAsync(model);
    
    // Set consent cookie
    context.Response.Cookies.Append("osirion_cookie_consent", "1.0", new CookieOptions
    {
        Expires = DateTimeOffset.UtcNow.AddDays(365),
        HttpOnly = true,
        Secure = true
    });
    
    return Results.Redirect(model.ReturnUrl ?? "/");
});
```

## Styling and CSS Classes

The component generates semantic HTML with proper CSS classes:

```html
<section class="osirion-cookie-consent osirion-cookie-consent-bottom">
    <div class="osirion-cookie-consent-content">
        <div class="osirion-cookie-consent-header">
            <h2 class="osirion-cookie-consent-title">Cookie Consent</h2>
        </div>
        <div class="osirion-cookie-consent-body">
            <p class="osirion-cookie-consent-message">...</p>
            <div class="osirion-cookie-consent-actions">
                <button type="submit" name="consent" value="accept">Accept All</button>
                <button type="submit" name="consent" value="decline">Decline</button>
                <button type="button" class="customize-button">Customize</button>
            </div>
        </div>
    </div>
    
    <!-- Customization panel (when active) -->
    <div class="osirion-cookie-consent-panel">
        <!-- Category checkboxes and descriptions -->
    </div>
</section>
```

### Custom Styling

```css
.osirion-cookie-consent {
    position: fixed;
    left: 0;
    right: 0;
    z-index: 1050;
    background: #ffffff;
    border: 1px solid #dee2e6;
    box-shadow: 0 -2px 10px rgba(0,0,0,0.1);
    padding: 1.5rem;
}

.osirion-cookie-consent-bottom {
    bottom: 0;
}

.osirion-cookie-consent-top {
    top: 0;
}

.osirion-cookie-consent-title {
    font-size: 1.25rem;
    font-weight: 600;
    margin-bottom: 0.5rem;
}

.osirion-cookie-consent-message {
    margin-bottom: 1rem;
    color: #495057;
}

.osirion-cookie-consent-actions {
    display: flex;
    gap: 0.75rem;
    flex-wrap: wrap;
}

.osirion-cookie-consent-actions button {
    padding: 0.5rem 1rem;
    border: 1px solid #dee2e6;
    border-radius: 0.375rem;
    background: #f8f9fa;
    cursor: pointer;
}

.osirion-cookie-consent-actions button[name="consent"][value="accept"] {
    background: #0d6efd;
    color: white;
    border-color: #0d6efd;
}

.osirion-cookie-consent-panel {
    margin-top: 1rem;
    padding-top: 1rem;
    border-top: 1px solid #dee2e6;
}

.cookie-category {
    margin-bottom: 1rem;
    padding: 1rem;
    border: 1px solid #dee2e6;
    border-radius: 0.375rem;
}

.cookie-category-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 0.5rem;
}

.cookie-category-name {
    font-weight: 600;
}

.cookie-category-description {
    font-size: 0.875rem;
    color: #6c757d;
}
```

## Accessibility Features

- **ARIA Labels**: Proper labels for form controls and buttons
- **Keyboard Navigation**: Full keyboard support for all interactive elements
- **Screen Reader**: Descriptive text and proper headings
- **Focus Management**: Logical tab order and focus indicators
- **Semantic HTML**: Uses proper form elements and structure

## Legal Compliance

### GDPR Requirements

The component helps meet GDPR requirements by:

1. **Explicit Consent**: Clear, affirmative action required
2. **Granular Control**: Category-specific consent options
3. **Easy Withdrawal**: Simple way to change preferences
4. **Consent Records**: Server-side consent logging
5. **Information Transparency**: Clear descriptions of cookie usage

### Implementation Checklist

- [ ] **Privacy Policy**: Link to comprehensive privacy policy
- [ ] **Cookie Categories**: Define all cookie types used
- [ ] **Consent Logging**: Record user consent decisions
- [ ] **Consent Withdrawal**: Provide way to change preferences
- [ ] **Regular Review**: Update categories and descriptions regularly

## Integration with Analytics

### Conditional Analytics Loading

```razor
@inject ICookieConsentService ConsentService

@if (await ConsentService.HasConsentAsync("analytics"))
{
    <ClarityTracker />
    <GoogleAnalyticsTracker />
}

@if (await ConsentService.HasConsentAsync("marketing"))
{
    <FacebookPixel />
    <GoogleAdsTracker />
}
```

### Consent-Based Feature Flags

```csharp
public class ConsentBasedFeatureFlags
{
    private readonly ICookieConsentService _consentService;
    
    public async Task<bool> CanUsePersonalizationAsync()
    {
        return await _consentService.HasConsentAsync("preferences");
    }
    
    public async Task<bool> CanShowTargetedAdsAsync()
    {
        return await _consentService.HasConsentAsync("marketing");
    }
}
```

## Best Practices

1. **Clear Communication**: Use plain language to explain cookie usage
2. **Granular Control**: Provide category-specific options
3. **Easy Access**: Make consent preferences easily accessible
4. **Regular Updates**: Review and update cookie categories regularly
5. **Performance**: Load the component early but don't block page rendering
6. **Testing**: Test consent flow on different devices and browsers
7. **Documentation**: Maintain detailed records of consent mechanisms

## Troubleshooting

### Common Issues

1. **Banner Not Showing**: Check if consent cookie already exists
2. **Form Submission Errors**: Verify ConsentEndpoint URL
3. **Styling Issues**: Ensure CSS classes are properly applied
4. **Accessibility**: Test with screen readers and keyboard navigation

### Debug Mode

```razor
<OsirionCookieConsent 
    @key="@debugKey"
    Categories="@debugCategories" />

@code {
    private string debugKey = Guid.NewGuid().ToString(); // Force re-render
    
    // Clear consent cookie for testing
    protected override void OnInitialized()
    {
        // For testing only - remove consent cookie
        if (Environment.IsDevelopment())
        {
            HttpContextAccessor.HttpContext?.Response.Cookies.Delete("osirion_cookie_consent");
        }
    }
}
```