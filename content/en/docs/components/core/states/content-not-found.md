---
id: 'osirion-content-not-found'
order: 1
layout: docs
title: OsirionContentNotFound Component
permalink: /docs/components/core/states/content-not-found
description: Learn how to use the OsirionContentNotFound component to display elegant 404 and error pages with customizable content and actions.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- States
- Error Handling
tags:
- blazor
- 404
- error-pages
- content-not-found
- error-states
- user-experience
- navigation
is_featured: true
published: true
slug: content-not-found
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionContentNotFound Component - Elegant Error Pages | Osirion.Blazor'
  description: 'Create elegant 404 and error pages with the OsirionContentNotFound component. Features customizable content, actions, and suggestions.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/states/content-not-found'
  lang: en
  robots: index, follow
  og_title: 'OsirionContentNotFound Component - Osirion.Blazor'
  og_description: 'Create elegant 404 and error pages with customizable content and actions.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionContentNotFound Component - Osirion.Blazor'
  twitter_description: 'Create elegant 404 and error pages with customizable content and actions.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionContentNotFound Component

The OsirionContentNotFound component provides elegant and customizable 404 error pages and "content not found" states. It offers multiple variants, customizable messaging, action buttons, and helpful suggestions to guide users back to relevant content.

## Component Overview

OsirionContentNotFound displays user-friendly error states when content cannot be found or accessed. It transforms potentially frustrating error experiences into opportunities to engage users with helpful alternatives, clear messaging, and intuitive navigation options.

### Key Features

**Multiple Variants**: Support for default, minimal, centered, and hero layouts
**Customizable Content**: Flexible error codes, titles, messages, and icons
**Action Buttons**: Primary and secondary action buttons with custom URLs
**Helpful Suggestions**: Display related links and navigation options
**Search Integration**: Optional search functionality for content discovery
**Contact Information**: Easy access to support and contact details
**Background Patterns**: Optional decorative background patterns
**Responsive Design**: Mobile-first design that works on all screen sizes
**Accessibility**: Full ARIA support and keyboard navigation
**SEO Friendly**: Proper status codes and meta information

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ErrorCode` | `string?` | `"404"` | Error code displayed prominently (e.g., "404", "500"). |
| `Title` | `string` | `"Page Not Found"` | Main title for the error state. |
| `Message` | `string` | `"The page you're looking for doesn't exist or has been moved."` | Descriptive message explaining the error. |
| `ShowIcon` | `bool` | `true` | Whether to display an icon. |
| `UseDefaultIcon` | `bool` | `true` | Whether to use the default icon when no custom icon is provided. |
| `Icon` | `RenderFragment?` | `null` | Custom icon content. |
| `IconUrl` | `string?` | `null` | URL for a custom icon image. |
| `IconAlt` | `string` | `"Not Found"` | Alt text for the icon image. |
| `PrimaryButtonText` | `string?` | `"Go to Homepage"` | Text for the primary action button. |
| `PrimaryButtonUrl` | `string` | `"/"` | URL for the primary action button. |
| `SecondaryButtonText` | `string?` | `null` | Text for the secondary action button. |
| `SecondaryButtonUrl` | `string` | `"/"` | URL for the secondary action button. |
| `Actions` | `RenderFragment?` | `null` | Custom action buttons content. |
| `ShowSuggestions` | `bool` | `false` | Whether to display helpful suggestions. |
| `SuggestionsTitle` | `string` | `"Here are some helpful links:"` | Title for the suggestions section. |
| `Suggestions` | `IReadOnlyList<ContentNotFoundSuggestion>?` | `null` | List of suggested navigation links. |
| `ShowSearchBox` | `bool` | `false` | Whether to display a search box. |
| `SearchContent` | `RenderFragment?` | `null` | Custom search functionality content. |
| `ShowContactInfo` | `bool` | `false` | Whether to display contact information. |
| `ContactMessage` | `string` | `"Need help? Contact us at"` | Message preceding contact information. |
| `ContactEmail` | `string?` | `null` | Contact email address. |
| `Variant` | `string` | `"default"` | Component variant: "default", "minimal", "centered", "hero". |
| `Size` | `string` | `"medium"` | Component size: "small", "medium", "large", "full". |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | Optional background pattern. |
| `ChildContent` | `RenderFragment?` | `null` | Additional custom content. |

## Basic Usage

### Simple 404 Page

```razor
@using Osirion.Blazor.Components

<OsirionContentNotFound 
    ErrorCode="404"
    Title="Page Not Found"
    Message="The page you're looking for doesn't exist or has been moved."
    PrimaryButtonText="Go to Homepage"
    PrimaryButtonUrl="/"
    SecondaryButtonText="View Sitemap"
    SecondaryButtonUrl="/sitemap" />
```

### Minimal Error State

```razor
<OsirionContentNotFound 
    Variant="minimal"
    Size="small"
    Title="Content Unavailable"
    Message="This content is currently unavailable."
    PrimaryButtonText="Try Again"
    PrimaryButtonUrl="/refresh"
    ShowIcon="false" />
```

### Centered Layout with Suggestions

```razor
<OsirionContentNotFound 
    Variant="centered"
    Size="large"
    Title="Oops! Page Not Found"
    Message="The page you're looking for might have been moved or deleted."
    ShowSuggestions="true"
    SuggestionsTitle="Popular pages:"
    Suggestions="@GetSuggestions()"
    ShowContactInfo="true"
    ContactEmail="support@example.com" />

@code {
    private IReadOnlyList<ContentNotFoundSuggestion> GetSuggestions()
    {
        return new List<ContentNotFoundSuggestion>
        {
            new("Home", "/", "_self"),
            new("Products", "/products", "_self"),
            new("Services", "/services", "_self"),
            new("About Us", "/about", "_self"),
            new("Contact", "/contact", "_self")
        };
    }
}
```

## Advanced Usage

### Hero-Style Error Page

```razor
<OsirionContentNotFound 
    Variant="hero"
    Size="full"
    ErrorCode="404"
    Title="Page Not Found"
    Message="Don't worry, even the best explorers sometimes take a wrong turn. Let's get you back on track!"
    BackgroundPattern="BackgroundPatternType.Geometric"
    ShowSuggestions="true"
    Suggestions="@GetHelpfulSuggestions()"
    ShowSearchBox="true">
    
    <Icon>
        <div class="custom-404-icon">
            <svg width="200" height="200" viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg">
                <circle cx="100" cy="100" r="80" fill="#f3f4f6" stroke="#e5e7eb" stroke-width="2"/>
                <circle cx="75" cy="80" r="8" fill="#6b7280"/>
                <circle cx="125" cy="80" r="8" fill="#6b7280"/>
                <path d="M70 130 Q100 150 130 130" stroke="#6b7280" stroke-width="3" fill="none" stroke-linecap="round"/>
            </svg>
        </div>
    </Icon>
    
    <Actions>
        <div class="custom-actions">
            <button class="btn btn-primary btn-lg" @onclick="NavigateHome">
                🏠 Take Me Home
            </button>
            <button class="btn btn-outline-secondary btn-lg" @onclick="GoBack">
                ← Go Back
            </button>
            <button class="btn btn-link" @onclick="ReportIssue">
                🐛 Report Issue
            </button>
        </div>
    </Actions>
    
    <SearchContent>
        <div class="search-container">
            <h4>Search for what you need:</h4>
            <div class="search-form">
                <input type="text" 
                       @bind="searchQuery" 
                       @onkeypress="HandleSearchKeyPress"
                       placeholder="Search our site..." 
                       class="form-control form-control-lg" />
                <button class="btn btn-primary" @onclick="PerformSearch">
                    🔍 Search
                </button>
            </div>
            
            @if (searchSuggestions?.Any() == true)
            {
                <div class="search-suggestions">
                    <p>Quick suggestions:</p>
                    <div class="suggestion-tags">
                        @foreach (var suggestion in searchSuggestions)
                        {
                            <button class="suggestion-tag" @onclick="() => NavigateToSuggestion(suggestion)">
                                @suggestion
                            </button>
                        }
                    </div>
                </div>
            }
        </div>
    </SearchContent>
</OsirionContentNotFound>

@code {
    private string searchQuery = "";
    private readonly string[] searchSuggestions = { "Documentation", "API Reference", "Tutorials", "Examples", "Support" };
    
    private IReadOnlyList<ContentNotFoundSuggestion> GetHelpfulSuggestions()
    {
        return new List<ContentNotFoundSuggestion>
        {
            new("📚 Documentation", "/docs", "_self"),
            new("🎯 Getting Started", "/docs/getting-started", "_self"),
            new("💡 Examples", "/examples", "_self"),
            new("🆘 Support Center", "/support", "_self"),
            new("📧 Contact Us", "/contact", "_self")
        };
    }
    
    private void NavigateHome()
    {
        Navigation.NavigateTo("/");
    }
    
    private void GoBack()
    {
        JSRuntime.InvokeVoidAsync("history.back");
    }
    
    private void ReportIssue()
    {
        Navigation.NavigateTo("/report-issue");
    }
    
    private async Task HandleSearchKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await PerformSearch();
        }
    }
    
    private async Task PerformSearch()
    {
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            Navigation.NavigateTo($"/search?q={Uri.EscapeDataString(searchQuery)}");
        }
    }
    
    private void NavigateToSuggestion(string suggestion)
    {
        var url = suggestion.ToLower() switch
        {
            "documentation" => "/docs",
            "api reference" => "/docs/api",
            "tutorials" => "/tutorials",
            "examples" => "/examples",
            "support" => "/support",
            _ => "/search?q=" + Uri.EscapeDataString(suggestion)
        };
        
        Navigation.NavigateTo(url);
    }
}

<style>
.custom-404-icon {
    margin-bottom: 2rem;
    animation: bounce 2s infinite;
}

@keyframes bounce {
    0%, 20%, 50%, 80%, 100% {
        transform: translateY(0);
    }
    40% {
        transform: translateY(-10px);
    }
    60% {
        transform: translateY(-5px);
    }
}

.custom-actions {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    align-items: center;
    margin: 2rem 0;
}

@media (min-width: 768px) {
    .custom-actions {
        flex-direction: row;
        justify-content: center;
    }
}

.search-container {
    max-width: 500px;
    margin: 2rem auto;
    text-align: center;
}

.search-form {
    display: flex;
    gap: 0.5rem;
    margin: 1rem 0;
}

.search-form input {
    flex: 1;
}

.search-suggestions {
    margin-top: 1.5rem;
}

.suggestion-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    justify-content: center;
    margin-top: 0.5rem;
}

.suggestion-tag {
    background: #f3f4f6;
    border: 1px solid #d1d5db;
    border-radius: 9999px;
    padding: 0.375rem 0.75rem;
    font-size: 0.875rem;
    cursor: pointer;
    transition: all 0.2s;
}

.suggestion-tag:hover {
    background: #e5e7eb;
    border-color: #9ca3af;
}
</style>
```

### Custom Error State for Different Scenarios

```razor
@switch (errorType)
{
    case ErrorType.NotFound:
        <OsirionContentNotFound 
            ErrorCode="404"
            Title="Page Not Found"
            Message="The page you're looking for doesn't exist."
            Variant="default"
            ShowSuggestions="true"
            Suggestions="@notFoundSuggestions" />
        break;
        
    case ErrorType.Unauthorized:
        <OsirionContentNotFound 
            ErrorCode="401"
            Title="Access Denied"
            Message="You don't have permission to access this content."
            PrimaryButtonText="Sign In"
            PrimaryButtonUrl="/login"
            SecondaryButtonText="Go Home"
            SecondaryButtonUrl="/"
            Variant="centered"
            Size="medium">
            
            <Icon>
                <div class="auth-error-icon">
                    🔒
                </div>
            </Icon>
        </OsirionContentNotFound>
        break;
        
    case ErrorType.ServerError:
        <OsirionContentNotFound 
            ErrorCode="500"
            Title="Server Error"
            Message="Something went wrong on our end. We're working to fix it."
            PrimaryButtonText="Try Again"
            PrimaryButtonUrl="@currentUrl"
            SecondaryButtonText="Report Issue"
            SecondaryButtonUrl="/report-issue"
            Variant="minimal"
            ShowContactInfo="true"
            ContactEmail="support@example.com" />
        break;
        
    case ErrorType.Maintenance:
        <OsirionContentNotFound 
            ErrorCode="503"
            Title="Under Maintenance"
            Message="We're currently performing scheduled maintenance. Please check back soon."
            PrimaryButtonText="Check Status"
            PrimaryButtonUrl="/status"
            Variant="hero"
            Size="large"
            BackgroundPattern="BackgroundPatternType.Dots">
            
            <Icon>
                <div class="maintenance-icon">
                    <svg width="120" height="120" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                        <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
                    </svg>
                </div>
            </Icon>
            
            <ChildContent>
                <div class="maintenance-info">
                    <p><strong>Estimated completion:</strong> @estimatedCompletion</p>
                    <p>We apologize for any inconvenience.</p>
                </div>
            </ChildContent>
        </OsirionContentNotFound>
        break;
        
    default:
        <OsirionContentNotFound />
        break;
}

@code {
    [Parameter] public ErrorType errorType { get; set; } = ErrorType.NotFound;
    
    private string currentUrl => Navigation.Uri;
    private DateTime estimatedCompletion = DateTime.Now.AddHours(2);
    
    private readonly IReadOnlyList<ContentNotFoundSuggestion> notFoundSuggestions = new List<ContentNotFoundSuggestion>
    {
        new("🏠 Home", "/", "_self"),
        new("📱 Products", "/products", "_self"),
        new("📞 Contact", "/contact", "_self"),
        new("🔍 Search", "/search", "_self")
    };
    
    public enum ErrorType
    {
        NotFound,
        Unauthorized,
        ServerError,
        Maintenance
    }
}

<style>
.auth-error-icon {
    font-size: 4rem;
    margin-bottom: 1rem;
    opacity: 0.7;
}

.maintenance-icon {
    color: #f59e0b;
    margin-bottom: 1rem;
}

.maintenance-info {
    background: #fef3c7;
    border: 1px solid #f59e0b;
    border-radius: 0.5rem;
    padding: 1rem;
    margin-top: 1rem;
    text-align: center;
}

.maintenance-info strong {
    color: #d97706;
}
</style>
```

### Interactive Error Page with Analytics

```razor
<OsirionContentNotFound 
    @ref="contentNotFoundRef"
    Title="@GetDynamicTitle()"
    Message="@GetDynamicMessage()"
    ShowSuggestions="true"
    Suggestions="@GetPersonalizedSuggestions()"
    ShowSearchBox="true"
    Variant="centered"
    Size="large">
    
    <Actions>
        <div class="interactive-actions">
            <button class="btn btn-primary" @onclick="HandlePrimaryAction">
                @primaryActionText
            </button>
            
            @if (showFeedback)
            {
                <div class="feedback-section">
                    <p>Was this page helpful?</p>
                    <div class="feedback-buttons">
                        <button class="btn btn-sm btn-outline-success" @onclick="() => SubmitFeedback(true)">
                            👍 Yes
                        </button>
                        <button class="btn btn-sm btn-outline-danger" @onclick="() => SubmitFeedback(false)">
                            👎 No
                        </button>
                    </div>
                </div>
            }
            
            @if (feedbackSubmitted)
            {
                <div class="feedback-thanks">
                    <p>✅ Thank you for your feedback!</p>
                </div>
            }
        </div>
    </Actions>
    
    <SearchContent>
        <div class="smart-search">
            <input type="text" 
                   @bind="searchQuery" 
                   @oninput="HandleSearchInput"
                   @onkeypress="HandleSearchKeyPress"
                   placeholder="What were you looking for?" 
                   class="form-control" />
            
            @if (searchSuggestions?.Any() == true)
            {
                <div class="live-suggestions">
                    @foreach (var suggestion in searchSuggestions.Take(5))
                    {
                        <button class="suggestion-item" @onclick="() => SelectSuggestion(suggestion)">
                            <span class="suggestion-text">@suggestion.Text</span>
                            <span class="suggestion-meta">@suggestion.Category</span>
                        </button>
                    }
                </div>
            }
        </div>
    </SearchContent>
</OsirionContentNotFound>

@code {
    private OsirionContentNotFound? contentNotFoundRef;
    private string searchQuery = "";
    private bool showFeedback = true;
    private bool feedbackSubmitted = false;
    private string primaryActionText = "Go to Homepage";
    private List<SearchSuggestion> searchSuggestions = new();
    private Timer? searchTimer;
    
    protected override async Task OnInitializedAsync()
    {
        // Track 404 error for analytics
        await TrackErrorEvent();
        
        // Get user's previous pages for personalized suggestions
        await LoadPersonalizedContent();
    }
    
    private string GetDynamicTitle()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            >= 5 and < 12 => "Good Morning! Page Not Found",
            >= 12 and < 17 => "Good Afternoon! Page Not Found",
            >= 17 and < 22 => "Good Evening! Page Not Found",
            _ => "Page Not Found"
        };
    }
    
    private string GetDynamicMessage()
    {
        var referer = Request.Headers["Referer"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(referer))
        {
            return $"The page you're looking for from {new Uri(referer).Host} doesn't exist or has been moved.";
        }
        
        return "The page you're looking for doesn't exist or has been moved.";
    }
    
    private IReadOnlyList<ContentNotFoundSuggestion> GetPersonalizedSuggestions()
    {
        // Return suggestions based on user's history or popular pages
        return new List<ContentNotFoundSuggestion>
        {
            new("🏠 Home", "/", "_self"),
            new("📚 Documentation", "/docs", "_self"),
            new("🛍️ Products", "/products", "_self"),
            new("📞 Support", "/support", "_self")
        };
    }
    
    private async Task HandleSearchInput(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString() ?? "";
        
        // Debounce search
        searchTimer?.Dispose();
        searchTimer = new Timer(async _ => await PerformLiveSearch(), null, 300, Timeout.Infinite);
    }
    
    private async Task PerformLiveSearch()
    {
        if (string.IsNullOrWhiteSpace(searchQuery) || searchQuery.Length < 2)
        {
            searchSuggestions.Clear();
            await InvokeAsync(StateHasChanged);
            return;
        }
        
        // Simulate API call for search suggestions
        await Task.Delay(100);
        
        searchSuggestions = new List<SearchSuggestion>
        {
            new($"Search for '{searchQuery}'", "Pages", $"/search?q={searchQuery}"),
            new($"'{searchQuery}' in Documentation", "Docs", $"/docs/search?q={searchQuery}"),
            new($"'{searchQuery}' in Products", "Products", $"/products/search?q={searchQuery}"),
            new($"'{searchQuery}' in Support", "Support", $"/support/search?q={searchQuery}")
        };
        
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task HandleSearchKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SelectSuggestion(searchSuggestions.FirstOrDefault() ?? 
                new SearchSuggestion($"Search for '{searchQuery}'", "General", $"/search?q={searchQuery}"));
        }
    }
    
    private async Task SelectSuggestion(SearchSuggestion suggestion)
    {
        await TrackSearchEvent(suggestion.Text);
        Navigation.NavigateTo(suggestion.Url);
    }
    
    private async Task HandlePrimaryAction()
    {
        await TrackActionEvent("primary_action");
        Navigation.NavigateTo("/");
    }
    
    private async Task SubmitFeedback(bool helpful)
    {
        feedbackSubmitted = true;
        showFeedback = false;
        
        await TrackFeedbackEvent(helpful);
        StateHasChanged();
        
        // Auto-hide feedback thanks after 3 seconds
        await Task.Delay(3000);
        feedbackSubmitted = false;
        StateHasChanged();
    }
    
    private async Task TrackErrorEvent()
    {
        // Track with analytics service
        await AnalyticsService.TrackEvent("404_error", new
        {
            url = Navigation.Uri,
            referrer = Request.Headers["Referer"].FirstOrDefault(),
            user_agent = Request.Headers["User-Agent"].FirstOrDefault(),
            timestamp = DateTime.UtcNow
        });
    }
    
    private async Task TrackSearchEvent(string query)
    {
        await AnalyticsService.TrackEvent("404_search", new { query, timestamp = DateTime.UtcNow });
    }
    
    private async Task TrackActionEvent(string action)
    {
        await AnalyticsService.TrackEvent("404_action", new { action, timestamp = DateTime.UtcNow });
    }
    
    private async Task TrackFeedbackEvent(bool helpful)
    {
        await AnalyticsService.TrackEvent("404_feedback", new { helpful, timestamp = DateTime.UtcNow });
    }
    
    private async Task LoadPersonalizedContent()
    {
        // Load user-specific suggestions based on history
        await Task.CompletedTask;
    }
    
    public record SearchSuggestion(string Text, string Category, string Url);
    
    public void Dispose()
    {
        searchTimer?.Dispose();
    }
}

<style>
.interactive-actions {
    text-align: center;
    margin: 2rem 0;
}

.feedback-section {
    margin-top: 2rem;
    padding: 1rem;
    background: #f8f9fa;
    border-radius: 0.5rem;
}

.feedback-buttons {
    display: flex;
    gap: 1rem;
    justify-content: center;
    margin-top: 0.5rem;
}

.feedback-thanks {
    color: #28a745;
    font-weight: 500;
    margin-top: 1rem;
}

.smart-search {
    position: relative;
    max-width: 400px;
    margin: 0 auto;
}

.live-suggestions {
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    background: white;
    border: 1px solid #dee2e6;
    border-top: none;
    border-radius: 0 0 0.375rem 0.375rem;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    z-index: 1000;
}

.suggestion-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    padding: 0.75rem 1rem;
    border: none;
    background: white;
    text-align: left;
    cursor: pointer;
    transition: background 0.2s;
}

.suggestion-item:hover {
    background: #f8f9fa;
}

.suggestion-item:not(:last-child) {
    border-bottom: 1px solid #e9ecef;
}

.suggestion-text {
    font-weight: 500;
}

.suggestion-meta {
    font-size: 0.875rem;
    color: #6c757d;
}
</style>
```

## Variants

### Default Variant

```razor
<OsirionContentNotFound 
    Variant="default"
    Title="Page Not Found"
    Message="The requested page could not be found." />
```

### Minimal Variant

```razor
<OsirionContentNotFound 
    Variant="minimal"
    Size="small"
    Title="Not Found"
    ShowIcon="false" />
```

### Centered Variant

```razor
<OsirionContentNotFound 
    Variant="centered"
    Size="large"
    BackgroundPattern="BackgroundPatternType.Subtle" />
```

### Hero Variant

```razor
<OsirionContentNotFound 
    Variant="hero"
    Size="full"
    BackgroundPattern="BackgroundPatternType.Geometric" />
```

## Best Practices

### Error Page Guidelines

1. **Clear Communication**: Use clear, non-technical language to explain the error
2. **Helpful Actions**: Provide relevant action buttons and navigation options
3. **Search Functionality**: Include search capability for content discovery
4. **Analytics Tracking**: Track 404 errors to identify broken links and improve content
5. **SEO Considerations**: Return proper HTTP status codes (404, 500, etc.)

### User Experience

1. **Empathetic Messaging**: Use friendly, helpful tone rather than technical jargon
2. **Visual Hierarchy**: Make the most important actions prominent and easy to find
3. **Loading Performance**: Ensure error pages load quickly even when other content fails
4. **Mobile Optimization**: Design error pages to work well on mobile devices
5. **Accessibility**: Ensure error pages are fully accessible to all users

### Technical Implementation

1. **Status Codes**: Return appropriate HTTP status codes for different error types
2. **Error Logging**: Log errors with sufficient detail for debugging
3. **Graceful Degradation**: Ensure error pages work even when JavaScript fails
4. **Caching Strategy**: Implement appropriate caching for error page assets
5. **Monitoring**: Monitor error page usage and user behavior for improvements

The OsirionContentNotFound component provides a comprehensive solution for handling error states while maintaining excellent user experience and providing helpful recovery options.
