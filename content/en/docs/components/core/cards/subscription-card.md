---
id: 'osirion-subscription-card'
order: 2
layout: docs
title: OsirionSubscriptionCard Component
permalink: /docs/components/core/cards/subscription-card
description: Learn how to use the OsirionSubscriptionCard component to create engaging subscription forms with validation, categories, and privacy options.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Cards
- Forms
tags:
- blazor
- subscription
- newsletter
- email-capture
- card-components
- forms
- validation
is_featured: true
published: true
slug: components/core/cards/subscription-card
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionSubscriptionCard Component - Newsletter Signup | Osirion.Blazor'
  description: 'Create engaging subscription forms with the OsirionSubscriptionCard component. Features validation, categories, privacy options, and customizable styling.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/cards/subscription-card'
  lang: en
  robots: index, follow
  og_title: 'OsirionSubscriptionCard Component - Osirion.Blazor'
  og_description: 'Create engaging subscription forms with validation, categories, and privacy options.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionSubscriptionCard Component - Osirion.Blazor'
  twitter_description: 'Create engaging subscription forms with validation and privacy options.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionSubscriptionCard Component

The OsirionSubscriptionCard component provides a comprehensive subscription form solution for newsletters, updates, and email capture. It features validation, privacy compliance, subscription categories, and customizable styling to maximize conversion rates while maintaining excellent user experience.

## Component Overview

OsirionSubscriptionCard combines form functionality with card presentation to create engaging subscription experiences. It handles email capture, optional name collection, subscription categories, privacy compliance, and provides feedback on submission status with built-in spam protection and accessibility features.

### Key Features

**Email Capture**: Professional email subscription with validation and verification
**Optional Name Field**: Configurable name collection for personalized communications
**Subscription Categories**: Allow users to choose their subscription preferences
**Privacy Compliance**: Built-in privacy policy agreement and GDPR compliance
**Spam Protection**: Honeypot field and advanced anti-spam measures
**Status Feedback**: Success, error, and processing states with clear messaging
**Responsive Design**: Mobile-first approach with adaptive layouts
**Accessibility**: Full ARIA support and keyboard navigation
**Framework Integration**: Compatible with all CSS frameworks and email services
**SSR Compatible**: Server-side rendering support with proper hydration

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | `"subscription-card"` | Unique identifier for the subscription card (required). |
| `Title` | `string` | `"Stay Updated"` | Title displayed at the top of the subscription card. |
| `Description` | `string` | `"Get the latest updates and insights delivered to your inbox."` | Description text displayed below the title. |
| `ShowNameField` | `bool` | `false` | Whether to display the optional name field. |
| `NamePlaceholder` | `string` | `"Your name (optional)"` | Placeholder text for the name field. |
| `EmailPlaceholder` | `string` | `"Enter your email address"` | Placeholder text for the email field. |
| `SubscribeButtonText` | `string` | `"Subscribe"` | Text displayed on the subscribe button. |
| `SubscribingText` | `string` | `"Subscribing..."` | Text displayed while form is submitting. |
| `ShowRequiredIndicators` | `bool` | `true` | Whether to show asterisk (*) for required fields. |
| `PrivacyPolicyText` | `string?` | `"I agree to the privacy policy"` | Privacy policy agreement text (supports HTML). |
| `ShowSubscriptionCategories` | `bool` | `false` | Whether to show subscription category selection. |
| `SubscriptionCategories` | `IReadOnlyList<SubscriptionCategory>?` | `null` | Available subscription categories. |
| `CategoriesText` | `string` | `"What would you like to receive?"` | Text displayed above subscription categories. |
| `Variant` | `SubscriptionCardVariant` | `Default` | Card variant: Default, Minimal, Hero, Inline. |
| `Size` | `SubscriptionCardSize` | `Medium` | Card size: Small, Medium, Large. |
| `OnSubscriptionSuccess` | `EventCallback<SubscriptionResult>` | - | Callback fired when subscription succeeds. |
| `OnSubscriptionError` | `EventCallback<string>` | - | Callback fired when subscription fails. |

## Basic Usage

### Simple Newsletter Signup

```razor
@using Osirion.Blazor.Components

<OsirionSubscriptionCard 
    Id="newsletter-signup"
    Title="Join Our Newsletter"
    Description="Get weekly updates on the latest features and improvements."
    EmailPlaceholder="Your email address"
    SubscribeButtonText="Join Newsletter" />
```

### Subscription with Name Field

```razor
<OsirionSubscriptionCard 
    Id="full-subscription"
    Title="Stay in the Loop"
    Description="Subscribe to receive product updates, tips, and exclusive content."
    ShowNameField="true"
    NamePlaceholder="Your full name"
    EmailPlaceholder="Your work email"
    PrivacyPolicyText="I agree to receive emails and the <a href='/privacy' target='_blank'>privacy policy</a>" />
```

### Subscription with Categories

```razor
<OsirionSubscriptionCard 
    Id="category-subscription"
    Title="Choose Your Interests"
    Description="Select the topics you're most interested in receiving updates about."
    ShowNameField="true"
    ShowSubscriptionCategories="true"
    SubscriptionCategories="@GetSubscriptionCategories()"
    CategoriesText="What topics interest you?"
    OnSubscriptionSuccess="HandleSubscriptionSuccess"
    OnSubscriptionError="HandleSubscriptionError" />

@code {
    private IReadOnlyList<SubscriptionCategory> GetSubscriptionCategories()
    {
        return new List<SubscriptionCategory>
        {
            new("product-updates", "Product Updates", "Get notified about new features and releases", true),
            new("tech-blog", "Technical Blog", "In-depth technical articles and tutorials", false),
            new("company-news", "Company News", "Company announcements and news", false),
            new("events", "Events & Webinars", "Upcoming events and webinar invitations", false),
            new("special-offers", "Special Offers", "Exclusive discounts and promotions", false)
        };
    }
    
    private async Task HandleSubscriptionSuccess(SubscriptionResult result)
    {
        // Handle successful subscription
        Logger.LogInformation("Subscription successful for {Email}", result.Email);
        
        // Show success notification
        await NotificationService.ShowSuccessAsync($"Welcome aboard! Check your email for confirmation.");
        
        // Track conversion
        await AnalyticsService.TrackEvent("subscription_success", new { 
            email = result.Email,
            categories = result.Categories,
            source = "newsletter_card"
        });
    }
    
    private async Task HandleSubscriptionError(string error)
    {
        // Handle subscription error
        Logger.LogError("Subscription failed: {Error}", error);
        
        // Show error notification
        await NotificationService.ShowErrorAsync("Something went wrong. Please try again later.");
    }
}
```

## Advanced Usage

### Hero Subscription Card

```razor
<OsirionSubscriptionCard 
    Id="hero-subscription"
    Variant="SubscriptionCardVariant.Hero"
    Size="SubscriptionCardSize.Large"
    Title="🚀 Join 10,000+ Developers"
    Description="Get exclusive insights, early access to new features, and join a community of forward-thinking developers."
    ShowNameField="true"
    ShowSubscriptionCategories="true"
    SubscriptionCategories="@GetHeroCategories()"
    SubscribeButtonText="Start Your Journey"
    Class="hero-subscription-card"
    Style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
    
    @* Additional content can be added here *@
    <div class="hero-subscription-benefits">
        <div class="benefit-list">
            <div class="benefit-item">
                <span class="benefit-icon">📧</span>
                <span class="benefit-text">Weekly newsletter with curated content</span>
            </div>
            <div class="benefit-item">
                <span class="benefit-icon">🎯</span>
                <span class="benefit-text">Early access to new features</span>
            </div>
            <div class="benefit-item">
                <span class="benefit-icon">👥</span>
                <span class="benefit-text">Exclusive community access</span>
            </div>
            <div class="benefit-item">
                <span class="benefit-icon">🎁</span>
                <span class="benefit-text">Free resources and templates</span>
            </div>
        </div>
    </div>
</OsirionSubscriptionCard>

@code {
    private IReadOnlyList<SubscriptionCategory> GetHeroCategories()
    {
        return new List<SubscriptionCategory>
        {
            new("developer-weekly", "Developer Weekly", "Weekly roundup of development trends and tools", true),
            new("product-launches", "Product Launches", "Be first to know about new product releases", true),
            new("tutorials", "Tutorials & Guides", "Step-by-step tutorials and how-to guides", false),
            new("community-events", "Community Events", "Meetups, conferences, and networking events", false)
        };
    }
}

<style>
.hero-subscription-card {
    color: white;
    border-radius: 1rem;
    box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
}

.hero-subscription-card .osirion-subscription-card-title {
    font-size: 2.5rem;
    font-weight: 700;
    margin-bottom: 1rem;
}

.hero-subscription-card .osirion-subscription-card-description {
    font-size: 1.25rem;
    opacity: 0.9;
    margin-bottom: 2rem;
}

.hero-subscription-benefits {
    margin-top: 2rem;
    padding: 2rem;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 0.5rem;
    backdrop-filter: blur(10px);
}

.benefit-list {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1rem;
}

.benefit-item {
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.benefit-icon {
    font-size: 1.5rem;
}

.benefit-text {
    font-weight: 500;
}

@media (max-width: 768px) {
    .hero-subscription-card .osirion-subscription-card-title {
        font-size: 2rem;
    }
    
    .benefit-list {
        grid-template-columns: 1fr;
    }
}
</style>
```

### Inline Subscription Form

```razor
<div class="content-section">
    <div class="blog-content">
        <h2>The Future of Web Development</h2>
        <p>As we look ahead to the next decade of web development, several trends are emerging that will shape how we build applications...</p>
        
        <!-- Inline subscription card -->
        <OsirionSubscriptionCard 
            Id="inline-subscription"
            Variant="SubscriptionCardVariant.Inline"
            Size="SubscriptionCardSize.Small"
            Title="Enjoying this article?"
            Description="Subscribe to get more insights like this delivered to your inbox."
            SubscribeButtonText="Subscribe"
            Class="inline-subscription"
            ShowNameField="false" />
        
        <p>Continuing with our analysis of emerging technologies...</p>
    </div>
</div>

<style>
.inline-subscription {
    margin: 3rem 0;
    padding: 2rem;
    background: #f8f9fa;
    border-left: 4px solid #007bff;
    border-radius: 0.5rem;
}

.inline-subscription .osirion-subscription-card-title {
    color: #007bff;
    font-size: 1.5rem;
    margin-bottom: 0.5rem;
}

.inline-subscription .osirion-subscription-form {
    display: flex;
    gap: 1rem;
    align-items: end;
}

.inline-subscription .osirion-form-group {
    flex: 1;
    margin-bottom: 0;
}

@media (max-width: 768px) {
    .inline-subscription .osirion-subscription-form {
        flex-direction: column;
        align-items: stretch;
    }
    
    .inline-subscription .osirion-form-group {
        margin-bottom: 1rem;
    }
}
</style>
```

### Multi-Step Subscription Flow

```razor
<OsirionSubscriptionCard 
    Id="multi-step-subscription"
    Title="@GetCurrentStepTitle()"
    Description="@GetCurrentStepDescription()"
    ShowNameField="@(currentStep >= 1)"
    ShowSubscriptionCategories="@(currentStep >= 2)"
    SubscriptionCategories="@availableCategories"
    SubscribeButtonText="@GetStepButtonText()"
    OnSubscriptionSuccess="HandleMultiStepSuccess"
    Class="multi-step-subscription @GetStepClass()">
    
    @if (currentStep > 0)
    {
        <div class="step-progress">
            <div class="progress-bar">
                <div class="progress-fill" style="width: @(GetProgressPercentage())%"></div>
            </div>
            <div class="step-indicator">
                Step @(currentStep + 1) of @totalSteps
            </div>
        </div>
    }
    
    @if (subscriptionComplete)
    {
        <div class="subscription-success">
            <div class="success-icon">✅</div>
            <h3>Welcome aboard!</h3>
            <p>Thank you for subscribing. We've sent a confirmation email to <strong>@submittedEmail</strong>.</p>
            <div class="next-steps">
                <h4>What's next?</h4>
                <ul>
                    <li>Check your email for confirmation</li>
                    <li>Add us to your contacts to ensure delivery</li>
                    <li>Browse our <a href="/resources">free resources</a></li>
                </ul>
            </div>
        </div>
    }
</OsirionSubscriptionCard>

@code {
    private int currentStep = 0;
    private int totalSteps = 3;
    private bool subscriptionComplete = false;
    private string submittedEmail = "";
    
    private readonly IReadOnlyList<SubscriptionCategory> availableCategories = new List<SubscriptionCategory>
    {
        new("weekly-digest", "Weekly Digest", "Curated weekly content roundup", true),
        new("breaking-news", "Breaking News", "Important updates as they happen", false),
        new("tutorials", "Tutorials", "Step-by-step learning content", false),
        new("case-studies", "Case Studies", "Real-world implementation examples", false)
    };
    
    private string GetCurrentStepTitle()
    {
        return currentStep switch
        {
            0 => "Join Our Community",
            1 => "Tell Us About Yourself",
            2 => "Choose Your Interests",
            _ => "Complete Your Subscription"
        };
    }
    
    private string GetCurrentStepDescription()
    {
        return currentStep switch
        {
            0 => "Start by providing your email address to join thousands of developers.",
            1 => "Help us personalize your experience with your name.",
            2 => "Select the topics you're most interested in receiving updates about.",
            _ => "Review and complete your subscription preferences."
        };
    }
    
    private string GetStepButtonText()
    {
        return currentStep switch
        {
            0 => "Continue",
            1 => "Next Step",
            2 => "Complete Subscription",
            _ => "Subscribe"
        };
    }
    
    private string GetStepClass()
    {
        return $"step-{currentStep}";
    }
    
    private double GetProgressPercentage()
    {
        return ((double)(currentStep + 1) / totalSteps) * 100;
    }
    
    private async Task HandleMultiStepSuccess(SubscriptionResult result)
    {
        submittedEmail = result.Email;
        subscriptionComplete = true;
        
        // Track multi-step completion
        await AnalyticsService.TrackEvent("multi_step_subscription_complete", new { 
            email = result.Email,
            steps_completed = totalSteps,
            categories = result.Categories
        });
        
        StateHasChanged();
    }
    
    protected override async Task OnInitializedAsync()
    {
        // Start with a delay to create anticipation
        await Task.Delay(500);
        StateHasChanged();
    }
}

<style>
.multi-step-subscription {
    max-width: 500px;
    margin: 0 auto;
    transition: all 0.3s ease;
}

.step-progress {
    margin-bottom: 2rem;
}

.progress-bar {
    width: 100%;
    height: 8px;
    background: #e9ecef;
    border-radius: 4px;
    overflow: hidden;
    margin-bottom: 0.5rem;
}

.progress-fill {
    height: 100%;
    background: linear-gradient(90deg, #007bff, #0056b3);
    border-radius: 4px;
    transition: width 0.3s ease;
}

.step-indicator {
    text-align: center;
    font-size: 0.875rem;
    color: #6c757d;
    font-weight: 500;
}

.subscription-success {
    text-align: center;
    padding: 2rem;
    background: #d4edda;
    border: 1px solid #c3e6cb;
    border-radius: 0.5rem;
    color: #155724;
}

.success-icon {
    font-size: 3rem;
    margin-bottom: 1rem;
}

.subscription-success h3 {
    color: #155724;
    margin-bottom: 1rem;
}

.next-steps {
    margin-top: 2rem;
    text-align: left;
}

.next-steps h4 {
    color: #155724;
    margin-bottom: 0.5rem;
}

.next-steps ul {
    margin: 0;
    padding-left: 1.25rem;
}

.next-steps li {
    margin-bottom: 0.25rem;
}

.next-steps a {
    color: #155724;
    font-weight: 600;
}

.step-0 {
    transform: scale(1);
}

.step-1, .step-2 {
    transform: scale(1.02);
    box-shadow: 0 8px 25px rgba(0, 123, 255, 0.15);
}
</style>
```

### A/B Testing Integration

```razor
@inject IABTestingService ABTestingService

<OsirionSubscriptionCard 
    Id="ab-test-subscription"
    Title="@subscriptionVariant.Title"
    Description="@subscriptionVariant.Description"
    SubscribeButtonText="@subscriptionVariant.ButtonText"
    Variant="@subscriptionVariant.CardVariant"
    ShowNameField="@subscriptionVariant.ShowNameField"
    PrivacyPolicyText="@subscriptionVariant.PrivacyText"
    OnSubscriptionSuccess="HandleABTestSuccess"
    Class="@subscriptionVariant.CssClass" />

@code {
    private SubscriptionVariant subscriptionVariant = new();
    
    protected override async Task OnInitializedAsync()
    {
        // Get A/B test variant
        var testResult = await ABTestingService.GetVariantAsync("subscription_card_test");
        
        subscriptionVariant = testResult.VariantName switch
        {
            "control" => new SubscriptionVariant
            {
                Title = "Join Our Newsletter",
                Description = "Get updates delivered to your inbox.",
                ButtonText = "Subscribe",
                CardVariant = SubscriptionCardVariant.Default,
                ShowNameField = false,
                PrivacyText = "I agree to the privacy policy",
                CssClass = "ab-test-control"
            },
            "variant_a" => new SubscriptionVariant
            {
                Title = "🚀 Join 5,000+ Developers",
                Description = "Get exclusive insights and early access to new features. Join our growing community!",
                ButtonText = "Join the Community",
                CardVariant = SubscriptionCardVariant.Hero,
                ShowNameField = true,
                PrivacyText = "I want to receive emails and agree to the <a href='/privacy'>privacy policy</a>",
                CssClass = "ab-test-variant-a"
            },
            "variant_b" => new SubscriptionVariant
            {
                Title = "Never Miss an Update",
                Description = "Stay ahead with the latest trends, tips, and tutorials.",
                ButtonText = "Get Updates",
                CardVariant = SubscriptionCardVariant.Minimal,
                ShowNameField = false,
                PrivacyText = null,
                CssClass = "ab-test-variant-b"
            },
            _ => subscriptionVariant
        };
    }
    
    private async Task HandleABTestSuccess(SubscriptionResult result)
    {
        // Track A/B test conversion
        await ABTestingService.TrackConversionAsync("subscription_card_test", new { 
            email = result.Email,
            variant = subscriptionVariant.GetType().Name
        });
        
        await AnalyticsService.TrackEvent("subscription_success_ab_test", new { 
            variant = subscriptionVariant.GetType().Name,
            email = result.Email
        });
    }
    
    public class SubscriptionVariant
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ButtonText { get; set; } = "";
        public SubscriptionCardVariant CardVariant { get; set; }
        public bool ShowNameField { get; set; }
        public string? PrivacyText { get; set; }
        public string CssClass { get; set; } = "";
    }
}

<style>
.ab-test-control {
    /* Standard styling */
}

.ab-test-variant-a {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    border-radius: 1rem;
    box-shadow: 0 10px 30px rgba(102, 126, 234, 0.3);
}

.ab-test-variant-b {
    border: 2px solid #007bff;
    background: #f8f9ff;
}
</style>
```

## Variants and Sizes

### Default Variant

```razor
<OsirionSubscriptionCard 
    Variant="SubscriptionCardVariant.Default"
    Title="Standard Newsletter"
    Description="Regular updates and insights." />
```

### Minimal Variant

```razor
<OsirionSubscriptionCard 
    Variant="SubscriptionCardVariant.Minimal"
    Size="SubscriptionCardSize.Small"
    Title="Quick Subscribe"
    ShowNameField="false" />
```

### Hero Variant

```razor
<OsirionSubscriptionCard 
    Variant="SubscriptionCardVariant.Hero"
    Size="SubscriptionCardSize.Large"
    Title="Join Our Community"
    Description="Become part of something bigger." />
```

### Inline Variant

```razor
<OsirionSubscriptionCard 
    Variant="SubscriptionCardVariant.Inline"
    Title="Like this content?"
    Description="Get more delivered to your inbox." />
```

## Best Practices

### Conversion Optimization

1. **Clear Value Proposition**: Clearly communicate what subscribers will receive
2. **Minimal Friction**: Only ask for essential information initially
3. **Social Proof**: Include subscriber counts or testimonials
4. **Mobile Optimization**: Ensure forms work perfectly on mobile devices
5. **A/B Testing**: Test different variants to optimize conversion rates

### Privacy and Compliance

1. **GDPR Compliance**: Include clear privacy policy agreements
2. **Double Opt-in**: Implement email confirmation for better deliverability
3. **Unsubscribe Options**: Provide easy unsubscribe mechanisms
4. **Data Protection**: Securely handle and store subscriber data
5. **Transparency**: Be clear about email frequency and content types

### User Experience

1. **Immediate Feedback**: Show clear success/error states
2. **Progressive Enhancement**: Build functionality that works without JavaScript
3. **Accessibility**: Ensure forms are accessible to all users
4. **Loading States**: Show progress during form submission
5. **Error Recovery**: Provide helpful error messages and recovery options

The OsirionSubscriptionCard component provides a comprehensive solution for email capture and newsletter subscriptions with excellent conversion rates and user experience.
