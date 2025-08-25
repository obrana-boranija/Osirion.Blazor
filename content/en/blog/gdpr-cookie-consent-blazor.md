---
title: "GDPR-Compliant Cookie Consent with Osirion.Blazor"
author: "Dejan Demonji?"
date: "2025-01-23"
description: "Implement GDPR-compliant cookie consent management with the new OsirionCookieConsent component, featuring customizable categories, server-side handling, and full accessibility support."
tags: [Privacy, GDPR, Compliance, Cookie Consent, Legal]
categories: [Components, Privacy]
slug: "gdpr-cookie-consent-blazor"
is_featured: true
featured_image: "https://images.unsplash.com/photo-1555949963-aa79dcee981c?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80"
seo_properties:
  title: "GDPR-Compliant Cookie Consent with Osirion.Blazor"
  description: "Learn how to implement comprehensive GDPR cookie consent management with customizable categories and server-side processing."
  og_image_url: "https://images.unsplash.com/photo-1555949963-aa79dcee981c?ixlib=rb-4.0.3&auto=format&fit=crop&w=1200&q=80"
  type: "Article"
---

# GDPR-Compliant Cookie Consent with Osirion.Blazor

Privacy compliance isn't just a legal requirement—it's about building trust with your users. With the new `OsirionCookieConsent` component in Osirion.Blazor v1.5, implementing comprehensive GDPR-compliant cookie consent management becomes straightforward while maintaining full control over the user experience.

## Why Cookie Consent Matters

The General Data Protection Regulation (GDPR) requires explicit consent for non-essential cookies. This means:

- Users must actively consent (no pre-checked boxes)
- Consent must be granular (category-specific)
- Users can withdraw consent easily
- Consent records must be maintained

The `OsirionCookieConsent` component handles all these requirements while providing a smooth user experience.

## Basic Implementation

Start with a simple implementation:

```razor
@using Osirion.Blazor.Components

<!-- Simple cookie consent banner -->
<OsirionCookieConsent />
```

This gives you a fully functional consent banner with default categories and GDPR-compliant behavior.

## Customized Implementation

For more control, customize the messaging and categories:

```razor
<OsirionCookieConsent 
    Title="Your Privacy Matters"
    Message="We use cookies to enhance your experience, analyze site usage, and provide personalized content. You can manage your preferences at any time."
    AcceptButtonText="Accept All"
    DeclineButtonText="Essential Only"
    CustomizeButtonText="Manage Preferences"
    PolicyLink="/privacy-policy"
    PolicyLinkText="Read our Privacy Policy"
    Position="bottom"
    Categories="@GetCookieCategories()" />

@code {
    private List<CookieCategory> GetCookieCategories()
    {
        return new List<CookieCategory>
        {
            new CookieCategory
            {
                Id = "necessary",
                Name = "Essential Cookies",
                Description = "These cookies are necessary for the website to function and cannot be disabled. They include session management, security, and core functionality.",
                IsRequired = true,
                IsEnabled = true
            },
            new CookieCategory
            {
                Id = "analytics",
                Name = "Analytics & Performance",
                Description = "These cookies help us understand how visitors interact with our website by collecting and reporting information anonymously. This helps us improve our services.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "marketing",
                Name = "Marketing & Advertising",
                Description = "These cookies are used to deliver personalized advertisements and measure the effectiveness of our marketing campaigns.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "personalization",
                Name = "Personalization",
                Description = "These cookies remember your preferences and settings to provide a customized experience tailored to your needs.",
                IsRequired = false,
                IsEnabled = false
            }
        };
    }
}
```

## E-commerce Implementation

For e-commerce sites, you might want different categories and messaging:

```razor
<OsirionCookieConsent 
    Title="Cookie Settings"
    Message="We use cookies to provide the best shopping experience, remember your preferences, and show you relevant products."
    AcceptButtonText="Accept & Continue Shopping"
    DeclineButtonText="Essential Only"
    CustomizeButtonText="Cookie Preferences"
    PolicyLink="/cookie-policy"
    Categories="@GetEcommerceCookieCategories()"
    ConsentEndpoint="/api/cookie-consent"
    ConsentExpiryDays="90" />

@code {
    private List<CookieCategory> GetEcommerceCookieCategories()
    {
        return new List<CookieCategory>
        {
            new CookieCategory
            {
                Id = "necessary",
                Name = "Essential",
                Description = "Required for basic site functionality, shopping cart, checkout, and security. These cannot be disabled.",
                IsRequired = true,
                IsEnabled = true
            },
            new CookieCategory
            {
                Id = "analytics",
                Name = "Analytics",
                Description = "Help us understand shopping behavior and improve our store experience through anonymous data collection.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "marketing",
                Name = "Marketing",
                Description = "Enable personalized product recommendations, targeted advertisements, and promotional communications.",
                IsRequired = false,
                IsEnabled = false
            },
            new CookieCategory
            {
                Id = "preferences",
                Name = "Shopping Preferences",
                Description = "Remember your shopping preferences, language settings, currency, and recently viewed products.",
                IsRequired = false,
                IsEnabled = true // Often enabled by default for better UX
            }
        };
    }
}
```

## Server-Side Consent Handling

The component submits consent via form POST to maintain SSR compatibility. Implement the server endpoint:

### ASP.NET Core Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class CookieConsentController : ControllerBase
{
    private readonly ICookieConsentService _consentService;
    private readonly ILogger<CookieConsentController> _logger;
    
    public CookieConsentController(
        ICookieConsentService consentService,
        ILogger<CookieConsentController> logger)
    {
        _consentService = consentService;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveConsent([FromForm] CookieConsentModel model)
    {
        try
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Get user identifier (IP, session, user ID, etc.)
            var userIdentifier = GetUserIdentifier();
            
            // Create consent record
            var consent = new UserCookieConsent
            {
                UserIdentifier = userIdentifier,
                ConsentDate = DateTime.UtcNow,
                ConsentVersion = model.ConsentVersion,
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                
                // Category consents
                NecessaryConsent = model.Necessary,
                AnalyticsConsent = model.Analytics,
                MarketingConsent = model.Marketing,
                PersonalizationConsent = model.Personalization,
                
                ExpiryDate = DateTime.UtcNow.AddDays(365)
            };
            
            // Save to database
            await _consentService.SaveConsentAsync(consent);
            
            // Set consent cookie
            SetConsentCookie(model);
            
            // Log for audit trail
            _logger.LogInformation("Cookie consent saved for user {UserIdentifier}: {Consent}", 
                userIdentifier, 
                System.Text.Json.JsonSerializer.Serialize(model));
            
            // Redirect back to referring page
            return Redirect(model.ReturnUrl ?? "/");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving cookie consent");
            return StatusCode(500, "An error occurred while saving your preferences");
        }
    }
    
    private void SetConsentCookie(CookieConsentModel model)
    {
        var cookieValue = System.Text.Json.JsonSerializer.Serialize(new
        {
            version = model.ConsentVersion,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            preferences = new
            {
                necessary = model.Necessary,
                analytics = model.Analytics,
                marketing = model.Marketing,
                personalization = model.Personalization
            }
        });
        
        Response.Cookies.Append("osirion_cookie_consent", cookieValue, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(365),
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            IsEssential = true // Exempt from consent requirements
        });
    }
    
    private string GetUserIdentifier()
    {
        // Use authenticated user ID if available
        if (User.Identity?.IsAuthenticated == true)
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        }
        
        // Otherwise use session ID or generate a pseudonymous identifier
        return HttpContext.Session.Id;
    }
}

public class CookieConsentModel
{
    public string ConsentVersion { get; set; } = "1.0";
    public bool Necessary { get; set; } = true;
    public bool Analytics { get; set; }
    public bool Marketing { get; set; }
    public bool Personalization { get; set; }
    public string? ReturnUrl { get; set; }
}

public class UserCookieConsent
{
    public string UserIdentifier { get; set; } = string.Empty;
    public DateTime ConsentDate { get; set; }
    public string ConsentVersion { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool NecessaryConsent { get; set; }
    public bool AnalyticsConsent { get; set; }
    public bool MarketingConsent { get; set; }
    public bool PersonalizationConsent { get; set; }
    public DateTime ExpiryDate { get; set; }
}
```

### Minimal API Alternative

```csharp
app.MapPost("/api/cookie-consent", async (
    CookieConsentModel model,
    HttpContext context,
    ICookieConsentService consentService,
    ILogger<Program> logger) =>
{
    try
    {
        // Save consent to database
        var consent = new UserCookieConsent
        {
            UserIdentifier = context.Session.Id,
            ConsentDate = DateTime.UtcNow,
            ConsentVersion = model.ConsentVersion,
            NecessaryConsent = model.Necessary,
            AnalyticsConsent = model.Analytics,
            MarketingConsent = model.Marketing,
            PersonalizationConsent = model.Personalization,
            ExpiryDate = DateTime.UtcNow.AddDays(365)
        };
        
        await consentService.SaveConsentAsync(consent);
        
        // Set consent cookie
        var cookieValue = JsonSerializer.Serialize(new
        {
            version = model.ConsentVersion,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            preferences = new
            {
                necessary = model.Necessary,
                analytics = model.Analytics,
                marketing = model.Marketing,
                personalization = model.Personalization
            }
        });
        
        context.Response.Cookies.Append("osirion_cookie_consent", cookieValue, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(365),
            HttpOnly = true,
            Secure = context.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            IsEssential = true
        });
        
        logger.LogInformation("Cookie consent saved for session {SessionId}", context.Session.Id);
        
        return Results.Redirect(model.ReturnUrl ?? "/");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error saving cookie consent");
        return Results.Problem("An error occurred while saving your preferences");
    }
});
```

## Conditional Feature Loading

Use consent status to conditionally load analytics and marketing features:

### Service for Checking Consent

```csharp
public interface ICookieConsentService
{
    Task<bool> HasConsentAsync(string category);
    Task<CookieConsent?> GetConsentAsync();
    Task WithdrawConsentAsync();
}

public class CookieConsentService : ICookieConsentService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CookieConsentService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Task<bool> HasConsentAsync(string category)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return Task.FromResult(false);
        
        if (!context.Request.Cookies.TryGetValue("osirion_cookie_consent", out var cookieValue))
        {
            return Task.FromResult(false);
        }
        
        try
        {
            var consent = JsonSerializer.Deserialize<CookieConsent>(cookieValue);
            if (consent == null || consent.IsExpired()) return Task.FromResult(false);
            
            return Task.FromResult(category.ToLower() switch
            {
                "necessary" => true, // Always true
                "analytics" => consent.Preferences.Analytics,
                "marketing" => consent.Preferences.Marketing,
                "personalization" => consent.Preferences.Personalization,
                _ => false
            });
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
    
    public async Task WithdrawConsentAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Response.Cookies.Delete("osirion_cookie_consent");
        }
    }
}
```

### Conditional Component Loading

```razor
@inject ICookieConsentService ConsentService

<!-- Only load analytics if user has consented -->
@if (await ConsentService.HasConsentAsync("analytics"))
{
    <ClarityTracker />
    <GoogleAnalyticsTracker />
}

<!-- Only load marketing tracking if user has consented -->
@if (await ConsentService.HasConsentAsync("marketing"))
{
    <FacebookPixel />
    <GoogleAdsTracker />
    <LinkedInInsightTag />
}

<!-- Only show personalized content if user has consented -->
@if (await ConsentService.HasConsentAsync("personalization"))
{
    <PersonalizedRecommendations />
    <RecentlyViewedProducts />
}
```

### Layout Integration

```razor
@using Osirion.Blazor.Components

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - My Application</title>
    <base href="~/" />
    <link rel="stylesheet" href="css/site.css" />
</head>
<body>
    <!-- Main content -->
    <div id="blazor-error-ui">
        <!-- Error UI -->
    </div>
    
    <!-- Page content -->
    @RenderBody()
    
    <!-- Cookie consent at the bottom -->
    <OsirionCookieConsent 
        Title="Privacy Preferences"
        Message="We respect your privacy. Choose which cookies you're comfortable with."
        PolicyLink="/privacy-policy"
        Categories="@GetCookieCategories()"
        Position="bottom" />
    
    <script src="_framework/blazor.server.js"></script>
</body>
</html>
```

## Advanced Consent Management

### Consent Preferences Page

Create a dedicated page for managing consent preferences:

```razor
@page "/privacy/cookie-preferences"
@inject ICookieConsentService ConsentService

<div class="container">
    <div class="row justify-content-center">
        <div class="col-lg-8">
            <div class="card">
                <div class="card-header">
                    <h2 class="card-title">Cookie Preferences</h2>
                    <p class="card-subtitle text-muted">
                        Manage your cookie settings and privacy preferences
                    </p>
                </div>
                
                <div class="card-body">
                    <!-- Force show the customization panel -->
                    <OsirionCookieConsent 
                        Title="Update Your Preferences"
                        Message="You can change your cookie preferences at any time. Your choices will be remembered for future visits."
                        ShowCustomizationPanel="true"
                        Categories="@cookieCategories"
                        AcceptButtonText="Save Preferences"
                        ShowDeclineButton="false"
                        ConsentEndpoint="/api/cookie-consent" />
                </div>
                
                <div class="card-footer">
                    <div class="d-flex justify-content-between align-items-center">
                        <small class="text-muted">
                            Last updated: @(await GetLastConsentDate())
                        </small>
                        
                        <button class="btn btn-outline-danger btn-sm" 
                                @onclick="WithdrawAllConsent">
                            Withdraw All Consent
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private List<CookieCategory> cookieCategories = new();
    
    protected override async Task OnInitializedAsync()
    {
        cookieCategories = GetCookieCategories();
        
        // Update categories with current consent status
        var currentConsent = await ConsentService.GetConsentAsync();
        if (currentConsent != null)
        {
            foreach (var category in cookieCategories)
            {
                category.IsEnabled = category.Id switch
                {
                    "necessary" => true,
                    "analytics" => currentConsent.Preferences.Analytics,
                    "marketing" => currentConsent.Preferences.Marketing,
                    "personalization" => currentConsent.Preferences.Personalization,
                    _ => false
                };
            }
        }
    }
    
    private async Task<string> GetLastConsentDate()
    {
        var consent = await ConsentService.GetConsentAsync();
        return consent?.ConsentDate.ToString("MMM dd, yyyy") ?? "Never";
    }
    
    private async Task WithdrawAllConsent()
    {
        await ConsentService.WithdrawConsentAsync();
        // Redirect to show banner again
        Navigation.NavigateTo("/", forceLoad: true);
    }
}
```

### Audit and Compliance Reporting

```csharp
public class ConsentAuditService
{
    private readonly IDbContext _dbContext;
    
    public async Task<ConsentReport> GenerateConsentReportAsync(DateTime from, DateTime to)
    {
        var consents = await _dbContext.CookieConsents
            .Where(c => c.ConsentDate >= from && c.ConsentDate <= to)
            .ToListAsync();
            
        return new ConsentReport
        {
            Period = $"{from:yyyy-MM-dd} to {to:yyyy-MM-dd}",
            TotalConsents = consents.Count,
            ConsentsByCategory = new Dictionary<string, int>
            {
                ["Analytics"] = consents.Count(c => c.AnalyticsConsent),
                ["Marketing"] = consents.Count(c => c.MarketingConsent),
                ["Personalization"] = consents.Count(c => c.PersonalizationConsent)
            },
            ConsentRates = new Dictionary<string, double>
            {
                ["Analytics"] = (double)consents.Count(c => c.AnalyticsConsent) / consents.Count * 100,
                ["Marketing"] = (double)consents.Count(c => c.MarketingConsent) / consents.Count * 100,
                ["Personalization"] = (double)consents.Count(c => c.PersonalizationConsent) / consents.Count * 100
            }
        };
    }
}
```

## Styling and Customization

### Custom Styling

```css
/* Custom cookie consent styling */
.osirion-cookie-consent {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    border: none;
    border-radius: 12px;
    margin: 1rem;
    box-shadow: 0 10px 30px rgba(0,0,0,0.2);
}

.osirion-cookie-consent-title {
    font-size: 1.5rem;
    font-weight: 700;
    margin-bottom: 1rem;
}

.osirion-cookie-consent-actions button {
    border-radius: 8px;
    font-weight: 600;
    padding: 0.75rem 1.5rem;
    transition: all 0.3s ease;
}

.osirion-cookie-consent-actions button[name="consent"][value="accept"] {
    background: rgba(255,255,255,0.2);
    border: 2px solid rgba(255,255,255,0.3);
    color: white;
}

.osirion-cookie-consent-actions button[name="consent"][value="accept"]:hover {
    background: rgba(255,255,255,0.3);
    transform: translateY(-2px);
}

/* Category styling */
.cookie-category {
    background: rgba(255,255,255,0.1);
    border: 1px solid rgba(255,255,255,0.2);
    border-radius: 8px;
    backdrop-filter: blur(10px);
}

.cookie-category-name {
    color: white;
    font-weight: 600;
}

.cookie-category-description {
    color: rgba(255,255,255,0.8);
}
```

### Framework Integration

```razor
<!-- Bootstrap themed consent -->
<OsirionCookieConsent 
    Class="shadow-lg"
    Categories="@categories" />

<!-- Material Design themed consent -->
<OsirionCookieConsent 
    Class="mdc-elevation--z8 mdc-theme--surface"
    Categories="@categories" />
```

## Best Practices

### 1. Clear, Honest Communication

```csharp
// Use plain language, avoid legal jargon
new CookieCategory
{
    Id = "analytics",
    Name = "Website Analytics",  // Not "Statistical Processing"
    Description = "Help us understand which pages are most popular and how visitors move around the site. All information is anonymous.",  // Clear benefit
    IsRequired = false,
    IsEnabled = false
}
```

### 2. Granular Control

```csharp
// Provide specific categories rather than broad ones
var categories = new List<CookieCategory>
{
    new() { Id = "necessary", Name = "Essential", IsRequired = true },
    new() { Id = "analytics", Name = "Analytics" },
    new() { Id = "marketing", Name = "Marketing" },
    new() { Id = "personalization", Name = "Personalization" },
    new() { Id = "social", Name = "Social Media" }
};
```

### 3. Easy Consent Withdrawal

```razor
<!-- Provide easy access to consent management -->
<footer>
    <div class="footer-links">
        <a href="/privacy-policy">Privacy Policy</a>
        <a href="/cookie-preferences">Cookie Settings</a>
        <a href="/contact">Contact</a>
    </div>
</footer>
```

### 4. Regular Consent Renewal

```csharp
// Set appropriate expiry periods
<OsirionCookieConsent 
    ConsentExpiryDays="365"  // Renew annually
    Categories="@categories" />
```

## Testing and Debugging

### Testing Consent Flow

```razor
@if (Environment.IsDevelopment())
{
    <div class="dev-tools">
        <button class="btn btn-sm btn-warning" @onclick="ClearConsent">
            Clear Consent (Dev)
        </button>
        
        <button class="btn btn-sm btn-info" @onclick="ShowConsentStatus">
            Show Consent Status
        </button>
    </div>
}

@code {
    private async Task ClearConsent()
    {
        // Clear consent cookie for testing
        await JSRuntime.InvokeVoidAsync("document.cookie", 
            "osirion_cookie_consent=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/");
        
        // Refresh page to show banner
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }
    
    private async Task ShowConsentStatus()
    {
        var status = new
        {
            Analytics = await ConsentService.HasConsentAsync("analytics"),
            Marketing = await ConsentService.HasConsentAsync("marketing"),
            Personalization = await ConsentService.HasConsentAsync("personalization")
        };
        
        await JSRuntime.InvokeVoidAsync("console.log", "Consent Status:", status);
    }
}
```

## Conclusion

The `OsirionCookieConsent` component provides a comprehensive solution for GDPR compliance while maintaining excellent user experience. With customizable categories, server-side processing, and full accessibility support, you can implement privacy-respectful cookie management that builds trust with your users.

Remember that compliance is not just about the technical implementation—it's about being transparent, respectful, and giving users meaningful control over their data. The component provides the tools; your content and policies complete the compliance picture.

Start building trust through transparent privacy practices with Osirion.Blazor v1.5!