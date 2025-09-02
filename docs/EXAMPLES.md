# Examples and Tutorials

[![Documentation](https://img.shields.io/badge/Documentation-Examples-green)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/EXAMPLES.md)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor)

This document provides practical examples and step-by-step tutorials for building real-world applications with Osirion.Blazor.

## Table of Contents

1. [Quick Start Examples](#quick-start-examples)
2. [Landing Page Tutorial](#landing-page-tutorial)
3. [Blog System Tutorial](#blog-system-tutorial)
4. [Documentation Site Tutorial](#documentation-site-tutorial)
5. [E-commerce Product Catalog](#e-commerce-product-catalog)
6. [Portfolio Website](#portfolio-website)
7. [Corporate Website](#corporate-website)
8. [Integration Examples](#integration-examples)
9. [Advanced Patterns](#advanced-patterns)
10. [Real-World Applications](#real-world-applications)

## Quick Start Examples

### Basic Landing Page

```razor
@page "/"
@using Osirion.Blazor.Components

<PageTitle>Welcome - Your Company</PageTitle>

<!-- Hero Section -->
<HeroSection 
    Title="Transform Your Business Digital Experience"
    Subtitle="Modern Solutions for Tomorrow's Challenges"
    Summary="Leverage cutting-edge technology to drive growth and innovation in your industry."
    ImageUrl="/images/hero-business.jpg"
    UseBackgroundImage="true"
    PrimaryButtonText="Get Started"
    PrimaryButtonUrl="/contact"
    SecondaryButtonText="Learn More"
    SecondaryButtonUrl="/about"
    Variant="HeroVariant.Hero"
    Alignment="Alignment.Center" />

<!-- Features Section -->
<section class="py-5 bg-light">
    <div class="container">
        <div class="row text-center mb-5">
            <div class="col-12">
                <h2 class="display-5 fw-bold">Why Choose Us</h2>
                <p class="lead">Delivering excellence through innovation and expertise</p>
            </div>
        </div>
        <div class="row g-4">
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="bg-primary text-white rounded-circle d-inline-flex align-items-center justify-content-center mb-3" style="width: 60px; height: 60px;">
                            <i class="fas fa-rocket fa-lg"></i>
                        </div>
                        <h5 class="card-title">Fast Performance</h5>
                        <p class="card-text">Lightning-fast load times with optimized SSR and progressive enhancement.</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="bg-success text-white rounded-circle d-inline-flex align-items-center justify-content-center mb-3" style="width: 60px; height: 60px;">
                            <i class="fas fa-shield-alt fa-lg"></i>
                        </div>
                        <h5 class="card-title">Secure by Design</h5>
                        <p class="card-text">Built with security best practices and GDPR compliance out of the box.</p>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card h-100 border-0 shadow-sm">
                    <div class="card-body text-center p-4">
                        <div class="bg-info text-white rounded-circle d-inline-flex align-items-center justify-content-center mb-3" style="width: 60px; height: 60px;">
                            <i class="fas fa-cogs fa-lg"></i>
                        </div>
                        <h5 class="card-title">Easy Integration</h5>
                        <p class="card-text">Seamless integration with your existing infrastructure and workflows.</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>

<!-- Logo Carousel -->
<InfiniteLogoCarousel 
    Title="Trusted by Industry Leaders"
    CustomLogos="@clientLogos"
    AnimationDuration="60"
    PauseOnHover="true"
    EnableGrayscale="true" />

<!-- CTA Section -->
<section class="py-5 bg-primary text-white">
    <div class="container text-center">
        <h2 class="display-6 mb-3">Ready to Get Started?</h2>
        <p class="lead mb-4">Join thousands of satisfied customers who trust our solutions.</p>
        <a href="/contact" class="btn btn-light btn-lg me-3">Contact Us</a>
        <a href="/demo" class="btn btn-outline-light btn-lg">Request Demo</a>
    </div>
</section>

@code {
    private List<LogoItem> clientLogos = new()
    {
        new LogoItem { Name = "Microsoft", ImageUrl = "/images/clients/microsoft.png", Link = "https://microsoft.com" },
        new LogoItem { Name = "Google", ImageUrl = "/images/clients/google.png", Link = "https://google.com" },
        new LogoItem { Name = "Amazon", ImageUrl = "/images/clients/amazon.png", Link = "https://amazon.com" },
        new LogoItem { Name = "Netflix", ImageUrl = "/images/clients/netflix.png", Link = "https://netflix.com" },
        new LogoItem { Name = "Spotify", ImageUrl = "/images/clients/spotify.png", Link = "https://spotify.com" }
    };
}
```

### Simple Blog List

```razor
@page "/blog"
@using Osirion.Blazor.Cms.Components

<PageTitle>Blog - Your Company</PageTitle>

<div class="container mt-4">
    <!-- Page Header -->
    <div class="row mb-5">
        <div class="col-12 text-center">
            <h1 class="display-4">Our Blog</h1>
            <p class="lead">Insights, tutorials, and updates from our team</p>
        </div>
    </div>
    
    <!-- Content Area -->
    <div class="row">
        <!-- Main Content -->
        <div class="col-lg-8">
            <ContentList 
                Directory="blog"
                Count="10"
                SortBy="SortField.Date"
                SortDirection="SortDirection.Descending"
                ShowExcerpt="true"
                ShowMetadata="true" />
        </div>
        
        <!-- Sidebar -->
        <div class="col-lg-4">
            <div class="sticky-top" style="top: 2rem;">
                <!-- Search -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="mb-0">Search</h5>
                    </div>
                    <div class="card-body">
                        <SearchBox Placeholder="Search articles..." />
                    </div>
                </div>
                
                <!-- Categories -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="mb-0">Categories</h5>
                    </div>
                    <div class="card-body">
                        <CategoriesList />
                    </div>
                </div>
                
                <!-- Popular Tags -->
                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0">Popular Tags</h5>
                    </div>
                    <div class="card-body">
                        <TagCloud MaxTags="20" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
```

## Landing Page Tutorial

Let's build a complete landing page for a SaaS product using Osirion.Blazor components.

### Step 1: Project Setup

```csharp
// Program.cs
using Osirion.Blazor.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Osirion.Blazor
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddOsirionStyle(CssFramework.Bootstrap)
        .AddScrollToTop()
        .AddClarityTracker(options => {
            options.SiteId = "your-clarity-id";
        });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### Step 2: Layout Configuration

```razor
<!-- Components/Layout/MainLayout.razor -->
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>SaaS Product - @(ViewData["Title"] ?? "Home")</title>
    
    <!-- Bootstrap CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <!-- Font Awesome -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet">
    
    <!-- Osirion Styles -->
    <OsirionStyles FrameworkIntegration="CssFramework.Bootstrap" />
    
    <!-- Custom Styles -->
    <link href="css/app.css" rel="stylesheet" />
</head>
<body>
    <!-- Enhanced Navigation -->
    <EnhancedNavigation Behavior="ScrollBehavior.Smooth" />
    
    <!-- Navigation Bar -->
    <nav class="navbar navbar-expand-lg navbar-light bg-white shadow-sm fixed-top">
        <div class="container">
            <a class="navbar-brand fw-bold" href="/">
                <img src="/images/logo.svg" alt="Logo" height="32" class="me-2">
                SaaS Product
            </a>
            
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav me-auto">
                    <li class="nav-item">
                        <a class="nav-link" href="/">Home</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="/features">Features</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="/pricing">Pricing</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="/blog">Blog</a>
                    </li>
                </ul>
                <div class="d-flex">
                    <a href="/login" class="btn btn-outline-primary me-2">Log In</a>
                    <a href="/signup" class="btn btn-primary">Start Free Trial</a>
                </div>
            </div>
        </div>
    </nav>
    
    <!-- Main Content -->
    <main style="margin-top: 76px;">
        @Body
    </main>
    
    <!-- Footer -->
    <OsirionFooter>
        <FooterContent>
            <footer class="bg-dark text-white py-5 mt-5">
                <div class="container">
                    <div class="row">
                        <div class="col-lg-4 mb-4">
                            <h5>SaaS Product</h5>
                            <p>The ultimate solution for modern businesses looking to scale efficiently.</p>
                            <div class="d-flex">
                                <a href="#" class="text-white me-3"><i class="fab fa-twitter"></i></a>
                                <a href="#" class="text-white me-3"><i class="fab fa-facebook"></i></a>
                                <a href="#" class="text-white me-3"><i class="fab fa-linkedin"></i></a>
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-6 mb-4">
                            <h6>Product</h6>
                            <ul class="list-unstyled">
                                <li><a href="/features" class="text-white-50">Features</a></li>
                                <li><a href="/pricing" class="text-white-50">Pricing</a></li>
                                <li><a href="/integrations" class="text-white-50">Integrations</a></li>
                            </ul>
                        </div>
                        <div class="col-lg-2 col-md-6 mb-4">
                            <h6>Company</h6>
                            <ul class="list-unstyled">
                                <li><a href="/about" class="text-white-50">About</a></li>
                                <li><a href="/careers" class="text-white-50">Careers</a></li>
                                <li><a href="/contact" class="text-white-50">Contact</a></li>
                            </ul>
                        </div>
                        <div class="col-lg-2 col-md-6 mb-4">
                            <h6>Resources</h6>
                            <ul class="list-unstyled">
                                <li><a href="/blog" class="text-white-50">Blog</a></li>
                                <li><a href="/docs" class="text-white-50">Documentation</a></li>
                                <li><a href="/support" class="text-white-50">Support</a></li>
                            </ul>
                        </div>
                        <div class="col-lg-2 col-md-6 mb-4">
                            <h6>Legal</h6>
                            <ul class="list-unstyled">
                                <li><a href="/privacy" class="text-white-50">Privacy</a></li>
                                <li><a href="/terms" class="text-white-50">Terms</a></li>
                                <li><a href="/security" class="text-white-50">Security</a></li>
                            </ul>
                        </div>
                    </div>
                    <hr class="my-4">
                    <div class="row align-items-center">
                        <div class="col-md-6">
                            <p class="mb-0">&copy; 2024 SaaS Product. All rights reserved.</p>
                        </div>
                        <div class="col-md-6 text-md-end">
                            <small class="text-white-50">Built with Osirion.Blazor</small>
                        </div>
                    </div>
                </div>
            </footer>
        </FooterContent>
    </OsirionFooter>
    
    <!-- Scroll to Top -->
    <ScrollToTop Position="Position.BottomRight" />
    
    <!-- Analytics -->
    <ClarityTracker />
    
    <!-- Cookie Consent -->
    <OsirionCookieConsent 
        PolicyLink="/privacy-policy"
        ShowCustomizeButton="true" />
    
    <!-- Bootstrap JS -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
```

### Step 3: Homepage Implementation

```razor
@page "/"

<PageTitle>Home - SaaS Product</PageTitle>

<!-- Hero Section -->
<HeroSection 
    Title="Scale Your Business with Confidence"
    Subtitle="The All-in-One Platform for Modern Teams"
    Summary="Streamline operations, boost productivity, and accelerate growth with our comprehensive business management platform."
    ImageUrl="/images/hero-dashboard.png"
    UseBackgroundImage="false"
    PrimaryButtonText="Start Free Trial"
    PrimaryButtonUrl="/signup"
    SecondaryButtonText="Watch Demo"
    SecondaryButtonUrl="/demo"
    Variant="HeroVariant.Hero"
    Alignment="Alignment.Left" />

<!-- Social Proof -->
<section class="py-4 bg-light">
    <div class="container">
        <div class="row text-center">
            <div class="col-12">
                <p class="text-muted mb-4">Trusted by over 10,000 businesses worldwide</p>
            </div>
        </div>
    </div>
</section>

<InfiniteLogoCarousel 
    CustomLogos="@trustLogos"
    AnimationDuration="45"
    PauseOnHover="true"
    EnableGrayscale="true"
    LogoHeight="40px" />

<!-- Features Section -->
<section class="py-5">
    <div class="container">
        <div class="row text-center mb-5">
            <div class="col-lg-8 mx-auto">
                <h2 class="display-5 fw-bold">Everything You Need to Succeed</h2>
                <p class="lead">Powerful features designed to help your business thrive in today's competitive landscape.</p>
            </div>
        </div>
        
        <div class="row g-4">
            @foreach (var feature in features)
            {
                <div class="col-md-6 col-lg-4">
                    <div class="card h-100 border-0 shadow-sm hover-lift">
                        <div class="card-body p-4">
                            <div class="@feature.IconClass text-primary mb-3" style="font-size: 2.5rem;"></div>
                            <h5 class="card-title">@feature.Title</h5>
                            <p class="card-text text-muted">@feature.Description</p>
                            <a href="@feature.Link" class="btn btn-outline-primary btn-sm">Learn More</a>
                        </div>
                    </div>
                </div>
            }
        </div>
    </div>
</section>

<!-- Statistics Section -->
<section class="py-5 bg-primary text-white">
    <div class="container">
        <div class="row text-center">
            <div class="col-md-3 mb-4">
                <div class="h2 fw-bold mb-0">99.9%</div>
                <p class="mb-0">Uptime</p>
            </div>
            <div class="col-md-3 mb-4">
                <div class="h2 fw-bold mb-0">10K+</div>
                <p class="mb-0">Happy Customers</p>
            </div>
            <div class="col-md-3 mb-4">
                <div class="h2 fw-bold mb-0">50M+</div>
                <p class="mb-0">Tasks Completed</p>
            </div>
            <div class="col-md-3 mb-4">
                <div class="h2 fw-bold mb-0">24/7</div>
                <p class="mb-0">Support</p>
            </div>
        </div>
    </div>
</section>

<!-- Testimonials Section -->
<section class="py-5">
    <div class="container">
        <div class="row text-center mb-5">
            <div class="col-12">
                <h2 class="display-5 fw-bold">What Our Customers Say</h2>
            </div>
        </div>
        
        <div class="row">
            @foreach (var testimonial in testimonials)
            {
                <div class="col-lg-4 mb-4">
                    <div class="card border-0 shadow-sm h-100">
                        <div class="card-body p-4">
                            <div class="text-warning mb-3">
                                @for (int i = 0; i < 5; i++)
                                {
                                    <i class="fas fa-star"></i>
                                }
                            </div>
                            <blockquote class="blockquote">
                                <p class="mb-3">"@testimonial.Quote"</p>
                            </blockquote>
                            <div class="d-flex align-items-center">
                                <img src="@testimonial.AvatarUrl" alt="@testimonial.Name" class="rounded-circle me-3" width="50" height="50">
                                <div>
                                    <div class="fw-bold">@testimonial.Name</div>
                                    <div class="text-muted small">@testimonial.Title, @testimonial.Company</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            }
        </div>
    </div>
</section>

<!-- CTA Section -->
<section class="py-5 bg-gradient" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
    <div class="container text-center text-white">
        <div class="row">
            <div class="col-lg-8 mx-auto">
                <h2 class="display-5 fw-bold mb-3">Ready to Transform Your Business?</h2>
                <p class="lead mb-4">Join thousands of businesses that have already made the switch. Start your free trial today.</p>
                <div class="d-flex flex-column flex-sm-row justify-content-center gap-3">
                    <a href="/signup" class="btn btn-light btn-lg px-4">Start Free Trial</a>
                    <a href="/contact" class="btn btn-outline-light btn-lg px-4">Contact Sales</a>
                </div>
                <p class="mt-3 mb-0"><small>No credit card required • 14-day free trial • Cancel anytime</small></p>
            </div>
        </div>
    </div>
</section>

@code {
    private List<LogoItem> trustLogos = new()
    {
        new LogoItem { Name = "Slack", ImageUrl = "/images/logos/slack.png" },
        new LogoItem { Name = "Dropbox", ImageUrl = "/images/logos/dropbox.png" },
        new LogoItem { Name = "Shopify", ImageUrl = "/images/logos/shopify.png" },
        new LogoItem { Name = "Mailchimp", ImageUrl = "/images/logos/mailchimp.png" },
        new LogoItem { Name = "Zoom", ImageUrl = "/images/logos/zoom.png" },
        new LogoItem { Name = "Trello", ImageUrl = "/images/logos/trello.png" }
    };

    private List<FeatureItem> features = new()
    {
        new FeatureItem
        {
            Title = "Project Management",
            Description = "Organize tasks, track progress, and collaborate with your team in real-time.",
            IconClass = "fas fa-tasks",
            Link = "/features/project-management"
        },
        new FeatureItem
        {
            Title = "Team Collaboration",
            Description = "Built-in chat, file sharing, and video conferencing for seamless teamwork.",
            IconClass = "fas fa-users",
            Link = "/features/collaboration"
        },
        new FeatureItem
        {
            Title = "Analytics & Reports",
            Description = "Gain insights with detailed analytics and customizable reporting tools.",
            IconClass = "fas fa-chart-line",
            Link = "/features/analytics"
        },
        new FeatureItem
        {
            Title = "Integrations",
            Description = "Connect with 100+ popular tools and services your team already uses.",
            IconClass = "fas fa-plug",
            Link = "/features/integrations"
        },
        new FeatureItem
        {
            Title = "Mobile Apps",
            Description = "Stay productive on the go with our native iOS and Android applications.",
            IconClass = "fas fa-mobile-alt",
            Link = "/features/mobile"
        },
        new FeatureItem
        {
            Title = "Enterprise Security",
            Description = "Bank-grade security with SSO, 2FA, and advanced permission controls.",
            IconClass = "fas fa-shield-alt",
            Link = "/features/security"
        }
    };

    private List<TestimonialItem> testimonials = new()
    {
        new TestimonialItem
        {
            Name = "Sarah Johnson",
            Title = "CEO",
            Company = "TechStart Inc.",
            Quote = "This platform has completely transformed how we manage our projects. Our team productivity has increased by 40% since we started using it.",
            AvatarUrl = "/images/testimonials/sarah.jpg"
        },
        new TestimonialItem
        {
            Name = "Michael Chen",
            Title = "Project Manager",
            Company = "Design Studio",
            Quote = "The collaboration features are incredible. We can work seamlessly with clients and team members regardless of location.",
            AvatarUrl = "/images/testimonials/michael.jpg"
        },
        new TestimonialItem
        {
            Name = "Emily Davis",
            Title = "Operations Director",
            Company = "Growth Corp",
            Quote = "The analytics and reporting capabilities give us insights we never had before. It's helped us make better business decisions.",
            AvatarUrl = "/images/testimonials/emily.jpg"
        }
    };

    public class FeatureItem
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string IconClass { get; set; } = "";
        public string Link { get; set; } = "";
    }

    public class TestimonialItem
    {
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public string Company { get; set; } = "";
        public string Quote { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
    }
}
```

## Blog System Tutorial

Now let's build a complete blog system with content management.

### Step 1: Configure GitHub CMS

```csharp
// Program.cs - Add to the Osirion configuration
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddGitHubCms(options => {
            options.Owner = "your-username";
            options.Repository = "blog-content";
            options.ContentPath = "content";
            options.Branch = "main";
            options.CacheExpirationMinutes = 60;
            options.ApiToken = builder.Configuration["GitHub:Token"];
        })
        .AddOsirionStyle(CssFramework.Bootstrap);
});
```

### Step 2: Create Content Repository Structure

Create a GitHub repository with this structure:

```
blog-content/
??? content/
?   ??? blog/
?   ?   ??? 2024/
?   ?   ?   ??? 01/
?   ?   ?   ?   ??? getting-started-with-blazor.md
?   ?   ?   ?   ??? building-modern-web-apps.md
?   ?   ?   ??? 02/
?   ?   ?       ??? performance-optimization.md
?   ?   ??? categories/
?   ?       ??? tutorials.md
?   ?       ??? tips.md
?   ?       ??? announcements.md
?   ??? pages/
?       ??? about.md
?       ??? contact.md
??? assets/
    ??? images/
        ??? featured/
        ??? authors/
```

### Step 3: Sample Blog Post

**content/blog/2024/01/getting-started-with-blazor.md:**

```markdown
---
title: "Getting Started with Blazor: A Complete Guide"
subtitle: "Everything you need to know to build your first Blazor application"
description: "Learn the fundamentals of Blazor development, from setting up your environment to deploying your first application."
date: "2024-01-15T10:00:00Z"
lastModified: "2024-01-16T14:30:00Z"
author: "Alex Developer"
authorBio: "Senior Full-Stack Developer with 8 years of experience in .NET technologies"
authorImage: "/assets/images/authors/alex.jpg"
categories: [tutorials, blazor]
tags: [blazor, dotnet, web-development, tutorial, beginner]
featured: true
featuredImage: "/assets/images/featured/blazor-guide.jpg"
featuredImageAlt: "Blazor development environment"
readingTime: "12 min"
difficulty: "beginner"
slug: "getting-started-with-blazor"
excerpt: "Discover how to build modern web applications with Blazor. This comprehensive guide covers everything from setup to deployment."
---

# Getting Started with Blazor: A Complete Guide

Blazor is Microsoft's revolutionary framework that allows developers to build interactive web applications using C# instead of JavaScript. In this comprehensive guide, we'll explore everything you need to know to get started with Blazor development.

## What is Blazor?

Blazor is a free, open-source web framework that enables developers to create web apps using C# and HTML. It's part of the ASP.NET Core ecosystem and offers two main hosting models:

### Blazor Server
- Runs on the server
- UI updates sent over SignalR connection
- Minimal client-side requirements

### Blazor WebAssembly
- Runs entirely in the browser
- Downloaded to client as WebAssembly
- Can work offline

## Setting Up Your Development Environment

Before we start building, let's ensure you have everything needed:

### Prerequisites
1. **.NET 8 SDK** or later
2. **Visual Studio 2022** or **VS Code**
3. **Git** (for version control)

### Installation Steps

1. **Download .NET SDK**
   ```bash
   # Verify installation
   dotnet --version
   ```

2. **Install Visual Studio Extensions** (if using VS Code)
   ```bash
   # Install C# extension
   code --install-extension ms-dotnettools.csharp
   ```

3. **Create Your First Project**
   ```bash
   # Create a new Blazor Server project
   dotnet new blazorserver -n MyFirstBlazorApp
   cd MyFirstBlazorApp
   dotnet run
   ```

## Your First Component

Let's create a simple counter component to understand Blazor basics:

```razor
@page "/counter"

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

### Key Concepts Explained

1. **@page Directive**: Defines the route for the component
2. **Data Binding**: The `@currentCount` syntax binds data to the UI
3. **Event Handling**: `@onclick` handles button click events
4. **@code Block**: Contains the component's C# logic

## Component Communication

Blazor components can communicate through various methods:

### Parameters
```razor
<!-- Parent Component -->
<ChildComponent Message="Hello from parent!" />

<!-- Child Component -->
@code {
    [Parameter] public string Message { get; set; } = "";
}
```

### Event Callbacks
```razor
<!-- Child Component -->
<button @onclick="NotifyParent">Click me</button>

@code {
    [Parameter] public EventCallback<string> OnMessageSent { get; set; }
    
    private async Task NotifyParent()
    {
        await OnMessageSent.InvokeAsync("Hello from child!");
    }
}
```

## State Management

For larger applications, consider these state management patterns:

### Dependency Injection
```csharp
// Services/CounterService.cs
public class CounterService
{
    public int Count { get; private set; }
    
    public event Action? OnChange;
    
    public void Increment()
    {
        Count++;
        OnChange?.Invoke();
    }
}

// Program.cs
builder.Services.AddScoped<CounterService>();
```

### Cascading Values
```razor
<!-- App.razor -->
<CascadingValue Value="@userInfo">
    <Router>
        <!-- Route components -->
    </Router>
</CascadingValue>

<!-- Child Component -->
@code {
    [CascadingParameter] public UserInfo UserInfo { get; set; }
}
```

## Best Practices

### 1. Component Organization
- Keep components small and focused
- Use proper naming conventions
- Organize files in logical folders

### 2. Performance Optimization
- Use `@key` directive for dynamic lists
- Implement `ShouldRender()` for expensive components
- Avoid frequent StateHasChanged() calls

### 3. Error Handling
```razor
<ErrorBoundary>
    <ChildContent>
        <!-- Your components -->
    </ChildContent>
    <ErrorContent>
        <div class="alert alert-danger">
            An error occurred. Please try again.
        </div>
    </ErrorContent>
</ErrorBoundary>
```

## Next Steps

Now that you understand the basics, here are some next steps:

1. **Learn about Forms**: Explore Blazor's form handling and validation
2. **API Integration**: Learn to consume REST APIs and GraphQL
3. **Authentication**: Implement user authentication and authorization
4. **Advanced Components**: Build complex, reusable components
5. **Deployment**: Deploy to Azure, AWS, or other cloud providers

## Conclusion

Blazor opens up exciting possibilities for .NET developers to build modern web applications. With its component-based architecture and powerful features, you can create rich, interactive user experiences while leveraging your existing C# skills.

Ready to dive deeper? Check out our [Advanced Blazor Techniques](../02/advanced-blazor-techniques.md) tutorial next!

---

*Have questions about Blazor development? Leave a comment below or reach out on [Twitter](https://twitter.com/yourusername).*
```

### Step 4: Blog Pages Implementation

**Components/Pages/Blog.razor:**

```razor
@page "/blog"
@page "/blog/category/{category}"
@page "/blog/tag/{tag}"
@using Osirion.Blazor.Components
@using Osirion.Blazor.Cms.Components

<PageTitle>@GetPageTitle() - Your Blog</PageTitle>

<!-- Breadcrumbs -->
<div class="container mt-3">
    <OsirionBreadcrumbs 
        Path="@Navigation.Uri"
        ShowHome="true"
        HomeText="Home"
        HomeUrl="/"
        SegmentFormatter="@FormatSegment" />
</div>

<!-- Page Header -->
<section class="py-5 bg-light">
    <div class="container">
        <div class="row">
            <div class="col-lg-8 mx-auto text-center">
                <h1 class="display-4">@GetPageTitle()</h1>
                <p class="lead">@GetPageDescription()</p>
                @if (!string.IsNullOrEmpty(Category) || !string.IsNullOrEmpty(Tag))
                {
                    <div class="mt-3">
                        <a href="/blog" class="btn btn-outline-primary">? All Posts</a>
                    </div>
                }
            </div>
        </div>
    </div>
</section>

<!-- Main Content -->
<div class="container my-5">
    <div class="row">
        <!-- Blog Posts -->
        <div class="col-lg-8">
            @if (!string.IsNullOrEmpty(searchQuery))
            {
                <div class="alert alert-info">
                    <i class="fas fa-search me-2"></i>
                    Showing results for: <strong>@searchQuery</strong>
                    <button type="button" class="btn-close float-end" @onclick="ClearSearch"></button>
                </div>
            }

            <ContentList 
                Directory="blog"
                Category="@Category"
                Tag="@Tag"
                Query="@searchQuery"
                Count="@pageSize"
                Skip="@((currentPage - 1) * pageSize)"
                SortBy="SortField.Date"
                SortDirection="SortDirection.Descending"
                ShowExcerpt="true"
                ShowMetadata="true">
                
                <ItemTemplate Context="item">
                    <article class="card border-0 shadow-sm mb-4">
                        @if (!string.IsNullOrEmpty(item.FeaturedImage))
                        {
                            <img src="@item.FeaturedImage" class="card-img-top" alt="@item.Title" style="height: 250px; object-fit: cover;">
                        }
                        <div class="card-body">
                            @if (item.IsFeatured)
                            {
                                <span class="badge bg-primary mb-2">Featured</span>
                            }
                            
                            <h2 class="card-title h4">
                                <a href="/blog/@item.Slug" class="text-decoration-none">@item.Title</a>
                            </h2>
                            
                            @if (!string.IsNullOrEmpty(item.Excerpt))
                            {
                                <p class="card-text text-muted">@item.Excerpt</p>
                            }
                            
                            <div class="d-flex justify-content-between align-items-center mt-3">
                                <div class="d-flex align-items-center">
                                    @if (!string.IsNullOrEmpty(item.AuthorImage))
                                    {
                                        <img src="@item.AuthorImage" alt="@item.Author" class="rounded-circle me-2" width="32" height="32">
                                    }
                                    <small class="text-muted">
                                        @item.Author • @item.PublishDate?.ToString("MMM dd, yyyy")
                                        @if (!string.IsNullOrEmpty(item.ReadingTime))
                                        {
                                            <span> • @item.ReadingTime</span>
                                        }
                                    </small>
                                </div>
                                <a href="/blog/@item.Slug" class="btn btn-outline-primary btn-sm">Read More</a>
                            </div>
                            
                            @if (item.Tags.Any())
                            {
                                <div class="mt-3">
                                    @foreach (var itemTag in item.Tags.Take(3))
                                    {
                                        <a href="/blog/tag/@itemTag" class="badge bg-light text-dark text-decoration-none me-1">#@itemTag</a>
                                    }
                                </div>
                            }
                        </div>
                    </article>
                </ItemTemplate>
                
                <EmptyTemplate>
                    <div class="text-center py-5">
                        <i class="fas fa-search fa-3x text-muted mb-3"></i>
                        <h3>No posts found</h3>
                        <p class="text-muted">Try adjusting your search or browse all posts.</p>
                        <a href="/blog" class="btn btn-primary">View All Posts</a>
                    </div>
                </EmptyTemplate>
            </ContentList>
            
            <!-- Pagination -->
            @if (totalPages > 1)
            {
                <nav aria-label="Blog pagination">
                    <ul class="pagination justify-content-center">
                        <li class="page-item @(currentPage == 1 ? "disabled" : "")">
                            <a class="page-link" href="@GetPageUrl(currentPage - 1)">Previous</a>
                        </li>
                        
                        @for (int i = Math.Max(1, currentPage - 2); i <= Math.Min(totalPages, currentPage + 2); i++)
                        {
                            <li class="page-item @(i == currentPage ? "active" : "")">
                                <a class="page-link" href="@GetPageUrl(i)">@i</a>
                            </li>
                        }
                        
                        <li class="page-item @(currentPage == totalPages ? "disabled" : "")">
                            <a class="page-link" href="@GetPageUrl(currentPage + 1)">Next</a>
                        </li>
                    </ul>
                </nav>
            }
        </div>
        
        <!-- Sidebar -->
        <div class="col-lg-4">
            <div class="sticky-top" style="top: 2rem;">
                <!-- Search -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="mb-0"><i class="fas fa-search me-2"></i>Search</h5>
                    </div>
                    <div class="card-body">
                        <SearchBox 
                            Placeholder="Search articles..."
                            MinSearchLength="2"
                            OnSearch="@HandleSearch" />
                    </div>
                </div>
                
                <!-- Featured Posts -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="mb-0"><i class="fas fa-star me-2"></i>Featured Posts</h5>
                    </div>
                    <div class="card-body">
                        <ContentList 
                            FeaturedCount="3"
                            ShowExcerpt="false"
                            ShowMetadata="false">
                            <ItemTemplate Context="item">
                                <div class="d-flex mb-3">
                                    @if (!string.IsNullOrEmpty(item.FeaturedImage))
                                    {
                                        <img src="@item.FeaturedImage" alt="@item.Title" class="rounded me-3" width="60" height="60" style="object-fit: cover;">
                                    }
                                    <div class="flex-grow-1">
                                        <h6 class="mb-1">
                                            <a href="/blog/@item.Slug" class="text-decoration-none">@item.Title</a>
                                        </h6>
                                        <small class="text-muted">@item.PublishDate?.ToString("MMM dd")</small>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </ContentList>
                    </div>
                </div>
                
                <!-- Categories -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="mb-0"><i class="fas fa-folder me-2"></i>Categories</h5>
                    </div>
                    <div class="card-body">
                        <CategoriesList>
                            <CategoryTemplate Context="category">
                                <div class="d-flex justify-content-between align-items-center mb-2">
                                    <a href="/blog/category/@category.Name" class="text-decoration-none">
                                        @category.Name.Replace("-", " ").ToTitleCase()
                                    </a>
                                    <span class="badge bg-secondary">@category.Count</span>
                                </div>
                            </CategoryTemplate>
                        </CategoriesList>
                    </div>
                </div>
                
                <!-- Popular Tags -->
                <div class="card mb-4">
                    <div class="card-header">
                        <h5 class="mb-0"><i class="fas fa-tags me-2"></i>Popular Tags</h5>
                    </div>
                    <div class="card-body">
                        <TagCloud MaxTags="20" ShowCounts="true" />
                    </div>
                </div>
                
                <!-- Newsletter Signup -->
                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0"><i class="fas fa-envelope me-2"></i>Newsletter</h5>
                    </div>
                    <div class="card-body">
                        <p class="small text-muted mb-3">Get the latest posts delivered right to your inbox.</p>
                        <form>
                            <div class="mb-3">
                                <input type="email" class="form-control" placeholder="Enter your email">
                            </div>
                            <button type="submit" class="btn btn-primary w-100">Subscribe</button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    [Parameter] public string? Category { get; set; }
    [Parameter] public string? Tag { get; set; }
    [SupplyParameterFromQuery] public string? Search { get; set; }
    [SupplyParameterFromQuery] public int Page { get; set; } = 1;
    
    [Inject] NavigationManager Navigation { get; set; } = default!;
    
    private string searchQuery = "";
    private int currentPage = 1;
    private int pageSize = 6;
    private int totalPages = 1;
    
    protected override void OnParametersSet()
    {
        searchQuery = Search ?? "";
        currentPage = Page > 0 ? Page : 1;
    }
    
    private string GetPageTitle()
    {
        if (!string.IsNullOrEmpty(Category))
            return $"Category: {Category.Replace("-", " ").ToTitleCase()}";
        if (!string.IsNullOrEmpty(Tag))
            return $"Tag: {Tag.Replace("-", " ").ToTitleCase()}";
        if (!string.IsNullOrEmpty(searchQuery))
            return $"Search Results";
        return "Blog";
    }
    
    private string GetPageDescription()
    {
        if (!string.IsNullOrEmpty(Category))
            return $"All posts in the {Category.Replace("-", " ")} category.";
        if (!string.IsNullOrEmpty(Tag))
            return $"All posts tagged with {Tag.Replace("-", " ")}.";
        if (!string.IsNullOrEmpty(searchQuery))
            return $"Search results for '{searchQuery}'.";
        return "Latest articles, tutorials, and insights from our team.";
    }
    
    private void HandleSearch(string query)
    {
        var url = "/blog";
        if (!string.IsNullOrEmpty(query))
        {
            url += $"?search={Uri.EscapeDataString(query)}";
        }
        Navigation.NavigateTo(url);
    }
    
    private void ClearSearch()
    {
        Navigation.NavigateTo("/blog");
    }
    
    private string GetPageUrl(int page)
    {
        var url = "/blog";
        if (!string.IsNullOrEmpty(Category))
            url += $"/category/{Category}";
        if (!string.IsNullOrEmpty(Tag))
            url += $"/tag/{Tag}";
        
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(searchQuery))
            queryParams.Add($"search={Uri.EscapeDataString(searchQuery)}");
        if (page > 1)
            queryParams.Add($"page={page}");
        
        if (queryParams.Any())
            url += "?" + string.Join("&", queryParams);
        
        return url;
    }
    
    private string FormatSegment(string segment)
    {
        return segment.Replace("-", " ").ToTitleCase();
    }
}

<style>
.hover-lift {
    transition: transform 0.2s ease-in-out;
}

.hover-lift:hover {
    transform: translateY(-2px);
}
</style>
```

This comprehensive examples document provides practical, ready-to-use code for building real-world applications with Osirion.Blazor. Each example demonstrates best practices and shows how to leverage the full power of the component library.