---
id: 'osirion-page-layout'
order: 1
layout: docs
title: OsirionPageLayout Component
permalink: /docs/components/core/layout/page-layout
description: Learn how to use the OsirionPageLayout component to create flexible page layouts with sticky footer support and responsive design.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Layout
tags:
- blazor
- page-layout
- sticky-footer
- layout-components
- responsive-design
- page-structure
is_featured: true
published: true
slug: components/core/layout/page-layout
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionPageLayout Component - Flexible Page Layouts | Osirion.Blazor'
  description: 'Create flexible page layouts with the OsirionPageLayout component. Features sticky footer support, responsive design, and customizable structure.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/layout/page-layout'
  lang: en
  robots: index, follow
  og_title: 'OsirionPageLayout Component - Osirion.Blazor'
  og_description: 'Create flexible page layouts with sticky footer support and responsive design.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionPageLayout Component - Osirion.Blazor'
  twitter_description: 'Create flexible page layouts with sticky footer support and responsive design.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionPageLayout Component

The OsirionPageLayout component provides a flexible and robust page layout structure with sticky footer support, responsive design, and customizable content areas. It serves as the foundation for creating consistent page layouts across your Blazor application.

## Component Overview

OsirionPageLayout creates a structured page layout with clearly defined header, body, and footer sections. It automatically handles common layout challenges such as sticky footers, responsive behavior, and proper content flow while maintaining semantic HTML structure.

### Key Features

**Sticky Footer Support**: Automatically positions footer at bottom of viewport when content is short
**Flexible Structure**: Customizable header, body, and footer sections with render fragment support
**Responsive Design**: Mobile-first approach with responsive breakpoints and adaptive behavior
**Semantic HTML**: Proper semantic structure with header, main, and footer elements
**Height Strategies**: Multiple minimum height strategies for different layout requirements
**Framework Agnostic**: Compatible with Bootstrap, Tailwind CSS, and custom CSS frameworks
**SSR Compatible**: Full Server-Side Rendering support with proper hydration

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Header` | `RenderFragment?` | `null` | Content for the page header section. |
| `Body` | `RenderFragment?` | `null` | Content for the main body section. |
| `Footer` | `RenderFragment?` | `null` | Content for the page footer section. |
| `StickyFooter` | `bool` | `true` | Whether to use sticky footer layout that pins footer to bottom. |
| `MinHeightStrategy` | `string` | `"viewport"` | Minimum height strategy: "viewport", "content", or "auto". |
| `Class` | `string?` | `null` | Additional CSS classes to apply to the layout container. |
| `Style` | `string?` | `null` | Inline styles to apply to the layout container. |

## Basic Usage

### Simple Page Layout

```razor
@using Osirion.Blazor.Components

<OsirionPageLayout>
    <Header>
        <nav class="navbar">
            <div class="navbar-brand">
                <a href="/">My Application</a>
            </div>
            <div class="navbar-nav">
                <a href="/about">About</a>
                <a href="/contact">Contact</a>
            </div>
        </nav>
    </Header>
    
    <Body>
        <main class="main-content">
            <h1>Welcome to My Application</h1>
            <p>This is the main content area of the page.</p>
            
            @* Your page content goes here *@
            @ChildContent
        </main>
    </Body>
    
    <Footer>
        <footer class="page-footer">
            <div class="footer-content">
                <p>&copy; 2025 My Application. All rights reserved.</p>
                <div class="footer-links">
                    <a href="/privacy">Privacy Policy</a>
                    <a href="/terms">Terms of Service</a>
                </div>
            </div>
        </footer>
    </Footer>
</OsirionPageLayout>
```

### Layout with Sticky Footer

```razor
<OsirionPageLayout StickyFooter="true" MinHeightStrategy="viewport">
    <Header>
        <header class="app-header">
            <div class="container">
                <h1 class="logo">Brand Name</h1>
                <nav class="main-navigation">
                    <ul>
                        <li><a href="/home">Home</a></li>
                        <li><a href="/services">Services</a></li>
                        <li><a href="/portfolio">Portfolio</a></li>
                        <li><a href="/contact">Contact</a></li>
                    </ul>
                </nav>
            </div>
        </header>
    </Header>
    
    <Body>
        <main class="main-container">
            <section class="hero-section">
                <div class="container">
                    <h2>Welcome to Our Services</h2>
                    <p>Professional solutions for your business needs.</p>
                </div>
            </section>
            
            <section class="content-section">
                <div class="container">
                    <!-- Page content here -->
                    @ChildContent
                </div>
            </section>
        </main>
    </Body>
    
    <Footer>
        <footer class="sticky-footer">
            <div class="container">
                <div class="footer-grid">
                    <div class="footer-column">
                        <h4>Company</h4>
                        <ul>
                            <li><a href="/about">About Us</a></li>
                            <li><a href="/team">Our Team</a></li>
                            <li><a href="/careers">Careers</a></li>
                        </ul>
                    </div>
                    <div class="footer-column">
                        <h4>Support</h4>
                        <ul>
                            <li><a href="/help">Help Center</a></li>
                            <li><a href="/contact">Contact</a></li>
                            <li><a href="/faq">FAQ</a></li>
                        </ul>
                    </div>
                    <div class="footer-column">
                        <h4>Legal</h4>
                        <ul>
                            <li><a href="/privacy">Privacy</a></li>
                            <li><a href="/terms">Terms</a></li>
                            <li><a href="/cookies">Cookies</a></li>
                        </ul>
                    </div>
                </div>
                <div class="footer-bottom">
                    <p>&copy; 2025 Brand Name. All rights reserved.</p>
                </div>
            </div>
        </footer>
    </Footer>
</OsirionPageLayout>

<style>
.app-header {
    background: #ffffff;
    border-bottom: 1px solid #e5e7eb;
    padding: 1rem 0;
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 1rem;
}

.main-navigation ul {
    display: flex;
    list-style: none;
    gap: 2rem;
    margin: 0;
    padding: 0;
}

.hero-section {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 4rem 0;
    text-align: center;
}

.content-section {
    padding: 3rem 0;
    min-height: 400px;
}

.sticky-footer {
    background: #1f2937;
    color: #ffffff;
    padding: 3rem 0 1rem;
}

.footer-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 2rem;
    margin-bottom: 2rem;
}

.footer-column h4 {
    color: #f9fafb;
    margin-bottom: 1rem;
}

.footer-column ul {
    list-style: none;
    padding: 0;
}

.footer-column li {
    margin-bottom: 0.5rem;
}

.footer-column a {
    color: #d1d5db;
    text-decoration: none;
}

.footer-column a:hover {
    color: #ffffff;
}

.footer-bottom {
    text-align: center;
    padding-top: 2rem;
    border-top: 1px solid #374151;
    color: #9ca3af;
}
</style>
```

## Advanced Usage

### Dynamic Layout with Conditional Sections

```razor
@inject NavigationManager Navigation
@inject IUserService UserService

<OsirionPageLayout 
    StickyFooter="@shouldUseSticky"
    MinHeightStrategy="@heightStrategy"
    Class="@GetLayoutClasses()">
    
    <Header>
        @if (showHeader)
        {
            <header class="dynamic-header @GetHeaderClasses()">
                <div class="header-container">
                    <!-- Logo/Brand -->
                    <div class="brand-section">
                        <a href="/" class="brand-link">
                            <img src="/images/logo.svg" alt="Brand Logo" class="brand-logo" />
                            <span class="brand-text">@brandName</span>
                        </a>
                    </div>
                    
                    <!-- Navigation -->
                    <nav class="main-nav" aria-label="Main navigation">
                        @if (isAuthenticated)
                        {
                            <ul class="nav-list authenticated">
                                <li><a href="/dashboard">Dashboard</a></li>
                                <li><a href="/profile">Profile</a></li>
                                <li><a href="/settings">Settings</a></li>
                                <li>
                                    <button class="logout-btn" @onclick="HandleLogout">
                                        Logout
                                    </button>
                                </li>
                            </ul>
                        }
                        else
                        {
                            <ul class="nav-list guest">
                                <li><a href="/features">Features</a></li>
                                <li><a href="/pricing">Pricing</a></li>
                                <li><a href="/about">About</a></li>
                                <li>
                                    <a href="/login" class="login-link">Login</a>
                                </li>
                                <li>
                                    <a href="/register" class="register-btn">Sign Up</a>
                                </li>
                            </ul>
                        }
                    </nav>
                    
                    <!-- Mobile menu toggle -->
                    <button class="mobile-menu-toggle" @onclick="ToggleMobileMenu" aria-label="Toggle mobile menu">
                        <span class="hamburger-line"></span>
                        <span class="hamburger-line"></span>
                        <span class="hamburger-line"></span>
                    </button>
                </div>
                
                <!-- Mobile menu -->
                @if (mobileMenuOpen)
                {
                    <div class="mobile-menu" @onclick:stopPropagation="true">
                        <!-- Mobile navigation items -->
                        <div class="mobile-nav">
                            @if (isAuthenticated)
                            {
                                <a href="/dashboard" class="mobile-nav-item">Dashboard</a>
                                <a href="/profile" class="mobile-nav-item">Profile</a>
                                <a href="/settings" class="mobile-nav-item">Settings</a>
                                <button class="mobile-nav-item logout" @onclick="HandleLogout">Logout</button>
                            }
                            else
                            {
                                <a href="/features" class="mobile-nav-item">Features</a>
                                <a href="/pricing" class="mobile-nav-item">Pricing</a>
                                <a href="/about" class="mobile-nav-item">About</a>
                                <a href="/login" class="mobile-nav-item">Login</a>
                                <a href="/register" class="mobile-nav-item register">Sign Up</a>
                            }
                        </div>
                    </div>
                }
            </header>
        }
    </Header>
    
    <Body>
        <main class="page-body @GetBodyClasses()" role="main">
            @if (showBreadcrumbs && !isHomePage)
            {
                <div class="breadcrumb-container">
                    <nav aria-label="Breadcrumb">
                        <ol class="breadcrumb">
                            @foreach (var crumb in breadcrumbs)
                            {
                                <li class="breadcrumb-item @(crumb.IsActive ? "active" : "")">
                                    @if (crumb.IsActive)
                                    {
                                        <span>@crumb.Text</span>
                                    }
                                    else
                                    {
                                        <a href="@crumb.Url">@crumb.Text</a>
                                    }
                                </li>
                            }
                        </ol>
                    </nav>
                </div>
            }
            
            <div class="content-wrapper">
                @ChildContent
            </div>
            
            @if (showScrollToTop && showScrollButton)
            {
                <button class="scroll-to-top" @onclick="ScrollToTop" aria-label="Scroll to top">
                    ↑
                </button>
            }
        </main>
    </Body>
    
    <Footer>
        @if (showFooter)
        {
            <footer class="dynamic-footer @GetFooterClasses()">
                <div class="footer-container">
                    @if (isLandingPage)
                    {
                        <!-- Extended footer for landing pages -->
                        <div class="footer-sections">
                            <div class="footer-section">
                                <h4>Product</h4>
                                <ul>
                                    <li><a href="/features">Features</a></li>
                                    <li><a href="/pricing">Pricing</a></li>
                                    <li><a href="/integrations">Integrations</a></li>
                                    <li><a href="/api">API</a></li>
                                </ul>
                            </div>
                            <div class="footer-section">
                                <h4>Company</h4>
                                <ul>
                                    <li><a href="/about">About Us</a></li>
                                    <li><a href="/blog">Blog</a></li>
                                    <li><a href="/careers">Careers</a></li>
                                    <li><a href="/contact">Contact</a></li>
                                </ul>
                            </div>
                            <div class="footer-section">
                                <h4>Resources</h4>
                                <ul>
                                    <li><a href="/docs">Documentation</a></li>
                                    <li><a href="/help">Help Center</a></li>
                                    <li><a href="/community">Community</a></li>
                                    <li><a href="/status">Status</a></li>
                                </ul>
                            </div>
                            <div class="footer-section">
                                <h4>Connect</h4>
                                <div class="social-links">
                                    <a href="@socialLinks.Twitter" aria-label="Twitter">Twitter</a>
                                    <a href="@socialLinks.LinkedIn" aria-label="LinkedIn">LinkedIn</a>
                                    <a href="@socialLinks.GitHub" aria-label="GitHub">GitHub</a>
                                </div>
                                <div class="newsletter-signup">
                                    <h5>Newsletter</h5>
                                    <form @onsubmit="HandleNewsletterSignup" @onsubmit:preventDefault="true">
                                        <input type="email" placeholder="Your email" @bind="newsletterEmail" />
                                        <button type="submit">Subscribe</button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    }
                    else
                    {
                        <!-- Minimal footer for app pages -->
                        <div class="footer-simple">
                            <div class="footer-links">
                                <a href="/privacy">Privacy</a>
                                <a href="/terms">Terms</a>
                                <a href="/support">Support</a>
                            </div>
                        </div>
                    }
                    
                    <div class="footer-bottom">
                        <p>&copy; @DateTime.Now.Year @brandName. All rights reserved.</p>
                        <div class="footer-meta">
                            <span>Version @appVersion</span>
                            @if (showBuildInfo)
                            {
                                <span>Build @buildNumber</span>
                            }
                        </div>
                    </div>
                </div>
            </footer>
        }
    </Footer>
</OsirionPageLayout>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    private bool isAuthenticated = false;
    private bool mobileMenuOpen = false;
    private bool showScrollButton = false;
    private string brandName = "Your App";
    private string appVersion = "1.0.0";
    private string buildNumber = "12345";
    private string newsletterEmail = "";
    
    // Layout configuration
    private bool shouldUseSticky => !isAppPage;
    private string heightStrategy => isLandingPage ? "viewport" : "content";
    private bool showHeader => !isFullscreenPage;
    private bool showFooter => !isAppPage && !isFullscreenPage;
    private bool showBreadcrumbs => isAppPage && isAuthenticated;
    private bool showScrollToTop => !isAppPage;
    private bool showBuildInfo => isDevelopment;
    
    // Page type detection
    private bool isHomePage => currentPath == "/";
    private bool isLandingPage => landingPages.Contains(currentPath);
    private bool isAppPage => appPages.Any(path => currentPath.StartsWith(path));
    private bool isFullscreenPage => fullscreenPages.Contains(currentPath);
    private bool isDevelopment => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    
    private string currentPath = "";
    private List<BreadcrumbItem> breadcrumbs = new();
    private SocialLinks socialLinks = new();
    
    private readonly string[] landingPages = { "/", "/features", "/pricing", "/about" };
    private readonly string[] appPages = { "/dashboard", "/profile", "/settings", "/admin" };
    private readonly string[] fullscreenPages = { "/login", "/register", "/onboarding" };
    
    protected override async Task OnInitializedAsync()
    {
        currentPath = Navigation.ToBaseRelativePath(Navigation.Uri);
        isAuthenticated = await UserService.IsAuthenticatedAsync();
        
        // Generate breadcrumbs
        breadcrumbs = GenerateBreadcrumbs(currentPath);
        
        // Setup scroll listener for scroll-to-top button
        await SetupScrollListener();
    }
    
    private string GetLayoutClasses()
    {
        var classes = new List<string> { "osirion-page-layout" };
        
        if (isLandingPage) classes.Add("landing-layout");
        if (isAppPage) classes.Add("app-layout");
        if (isFullscreenPage) classes.Add("fullscreen-layout");
        if (mobileMenuOpen) classes.Add("mobile-menu-open");
        
        return string.Join(" ", classes);
    }
    
    private string GetHeaderClasses()
    {
        var classes = new List<string> { "page-header" };
        
        if (isLandingPage) classes.Add("landing-header");
        if (isAppPage) classes.Add("app-header");
        if (isAuthenticated) classes.Add("authenticated");
        
        return string.Join(" ", classes);
    }
    
    private string GetBodyClasses()
    {
        var classes = new List<string> { "page-main" };
        
        if (isLandingPage) classes.Add("landing-main");
        if (isAppPage) classes.Add("app-main");
        if (showBreadcrumbs) classes.Add("with-breadcrumbs");
        
        return string.Join(" ", classes);
    }
    
    private string GetFooterClasses()
    {
        var classes = new List<string> { "page-footer" };
        
        if (isLandingPage) classes.Add("landing-footer");
        else classes.Add("simple-footer");
        
        return string.Join(" ", classes);
    }
    
    private void ToggleMobileMenu()
    {
        mobileMenuOpen = !mobileMenuOpen;
        StateHasChanged();
    }
    
    private async Task HandleLogout()
    {
        await UserService.LogoutAsync();
        Navigation.NavigateTo("/");
    }
    
    private async Task HandleNewsletterSignup()
    {
        if (!string.IsNullOrWhiteSpace(newsletterEmail))
        {
            await NewsletterService.SubscribeAsync(newsletterEmail);
            newsletterEmail = "";
            StateHasChanged();
        }
    }
    
    private List<BreadcrumbItem> GenerateBreadcrumbs(string path)
    {
        var items = new List<BreadcrumbItem> { new("Home", "/", false) };
        
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var currentPath = "";
        
        foreach (var segment in segments)
        {
            currentPath += $"/{segment}";
            var isLast = segment == segments.Last();
            var title = segment.Replace("-", " ").ToTitleCase();
            
            items.Add(new BreadcrumbItem(title, currentPath, isLast));
        }
        
        return items;
    }
    
    private async Task SetupScrollListener()
    {
        // Setup scroll event listener for scroll-to-top button
        await Task.CompletedTask; // Implement scroll listener logic
    }
    
    private async Task ScrollToTop()
    {
        await JSRuntime.InvokeVoidAsync("scrollToTop");
    }
    
    public record BreadcrumbItem(string Text, string Url, bool IsActive);
    public record SocialLinks
    {
        public string Twitter { get; init; } = "https://twitter.com/company";
        public string LinkedIn { get; init; } = "https://linkedin.com/company/company";
        public string GitHub { get; init; } = "https://github.com/company";
    }
}

<script>
    window.scrollToTop = () => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };
</script>
```

### Layout with Sidebar

```razor
<OsirionPageLayout StickyFooter="false" MinHeightStrategy="viewport" Class="sidebar-layout">
    <Header>
        <header class="app-header">
            <div class="header-content">
                <button class="sidebar-toggle" @onclick="ToggleSidebar" aria-label="Toggle sidebar">
                    ☰
                </button>
                <h1 class="app-title">Dashboard</h1>
                <div class="header-actions">
                    <button class="notifications-btn">🔔</button>
                    <div class="user-menu">
                        <img src="/images/user-avatar.jpg" alt="User Avatar" class="user-avatar" />
                        <span class="user-name">John Doe</span>
                    </div>
                </div>
            </div>
        </header>
    </Header>
    
    <Body>
        <div class="layout-with-sidebar @(sidebarOpen ? "sidebar-open" : "sidebar-closed")">
            <aside class="sidebar" aria-label="Main navigation">
                <nav class="sidebar-nav">
                    <ul class="nav-items">
                        <li class="nav-item">
                            <a href="/dashboard" class="nav-link @GetActiveClass("/dashboard")">
                                <span class="nav-icon">📊</span>
                                <span class="nav-text">Dashboard</span>
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="/analytics" class="nav-link @GetActiveClass("/analytics")">
                                <span class="nav-icon">📈</span>
                                <span class="nav-text">Analytics</span>
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="/users" class="nav-link @GetActiveClass("/users")">
                                <span class="nav-icon">👥</span>
                                <span class="nav-text">Users</span>
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="/settings" class="nav-link @GetActiveClass("/settings")">
                                <span class="nav-icon">⚙️</span>
                                <span class="nav-text">Settings</span>
                            </a>
                        </li>
                    </ul>
                </nav>
            </aside>
            
            <main class="main-content">
                <div class="content-container">
                    @ChildContent
                </div>
            </main>
        </div>
    </Body>
    
    <Footer>
        <footer class="app-footer">
            <div class="footer-content">
                <p>&copy; 2025 Dashboard App</p>
                <div class="footer-links">
                    <a href="/help">Help</a>
                    <a href="/privacy">Privacy</a>
                </div>
            </div>
        </footer>
    </Footer>
</OsirionPageLayout>

@code {
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    private bool sidebarOpen = true;
    
    private void ToggleSidebar()
    {
        sidebarOpen = !sidebarOpen;
        StateHasChanged();
    }
    
    private string GetActiveClass(string path)
    {
        return Navigation.Uri.Contains(path) ? "active" : "";
    }
}

<style>
.sidebar-layout {
    --sidebar-width: 250px;
    --sidebar-collapsed-width: 60px;
    --header-height: 60px;
}

.app-header {
    height: var(--header-height);
    background: #ffffff;
    border-bottom: 1px solid #e5e7eb;
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    z-index: 1000;
}

.header-content {
    display: flex;
    align-items: center;
    justify-content: space-between;
    height: 100%;
    padding: 0 1rem;
}

.sidebar-toggle {
    background: none;
    border: none;
    font-size: 1.5rem;
    cursor: pointer;
    padding: 0.5rem;
}

.layout-with-sidebar {
    display: flex;
    min-height: calc(100vh - var(--header-height));
    margin-top: var(--header-height);
}

.sidebar {
    width: var(--sidebar-width);
    background: #1f2937;
    color: white;
    transition: width 0.3s ease;
    overflow: hidden;
}

.sidebar-closed .sidebar {
    width: var(--sidebar-collapsed-width);
}

.sidebar-nav {
    padding: 1rem 0;
}

.nav-items {
    list-style: none;
    padding: 0;
    margin: 0;
}

.nav-item {
    margin-bottom: 0.5rem;
}

.nav-link {
    display: flex;
    align-items: center;
    padding: 0.75rem 1rem;
    color: #d1d5db;
    text-decoration: none;
    transition: background 0.2s;
}

.nav-link:hover,
.nav-link.active {
    background: #374151;
    color: white;
}

.nav-icon {
    margin-right: 0.75rem;
    font-size: 1.25rem;
}

.sidebar-closed .nav-text {
    display: none;
}

.main-content {
    flex: 1;
    background: #f9fafb;
    min-height: calc(100vh - var(--header-height));
}

.content-container {
    padding: 2rem;
}

.app-footer {
    background: #ffffff;
    border-top: 1px solid #e5e7eb;
    padding: 1rem 2rem;
}

.footer-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.footer-links {
    display: flex;
    gap: 1rem;
}

.footer-links a {
    color: #6b7280;
    text-decoration: none;
}

.user-menu {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.user-avatar {
    width: 32px;
    height: 32px;
    border-radius: 50%;
}

@media (max-width: 768px) {
    .sidebar {
        position: fixed;
        top: var(--header-height);
        left: 0;
        height: calc(100vh - var(--header-height));
        transform: translateX(-100%);
        transition: transform 0.3s ease;
        z-index: 999;
    }
    
    .sidebar-open .sidebar {
        transform: translateX(0);
    }
    
    .main-content {
        width: 100%;
        margin-left: 0;
    }
}
</style>
```

## Height Strategies

### Viewport Strategy

```razor
<!-- Layout fills full viewport height -->
<OsirionPageLayout MinHeightStrategy="viewport" StickyFooter="true">
    <Header>
        <div class="header">Header content always at top</div>
    </Header>
    <Body>
        <div class="body">Content area expands to fill available space</div>
    </Body>
    <Footer>
        <div class="footer">Footer always at bottom of viewport</div>
    </Footer>
</OsirionPageLayout>
```

### Content Strategy

```razor
<!-- Layout height based on content -->
<OsirionPageLayout MinHeightStrategy="content" StickyFooter="false">
    <Header>
        <div class="header">Header</div>
    </Header>
    <Body>
        <div class="body">
            <p>Content determines the height.</p>
            <p>Footer will be positioned after content.</p>
        </div>
    </Body>
    <Footer>
        <div class="footer">Footer follows content</div>
    </Footer>
</OsirionPageLayout>
```

### Auto Strategy

```razor
<!-- Automatic height based on content and viewport -->
<OsirionPageLayout MinHeightStrategy="auto" StickyFooter="true">
    <Header>
        <div class="header">Adaptive header</div>
    </Header>
    <Body>
        <div class="body">
            Automatically adjusts based on content length and viewport size.
        </div>
    </Body>
    <Footer>
        <div class="footer">Smart footer positioning</div>
    </Footer>
</OsirionPageLayout>
```

## Responsive Behavior

### Mobile-First Layout

```razor
<OsirionPageLayout Class="responsive-layout">
    <Header>
        <header class="responsive-header">
            <div class="header-container">
                <div class="mobile-header">
                    <button class="menu-toggle" @onclick="ToggleMobileMenu">☰</button>
                    <h1 class="mobile-title">App</h1>
                </div>
                <nav class="desktop-nav @(mobileMenuOpen ? "mobile-open" : "")">
                    <a href="/home">Home</a>
                    <a href="/about">About</a>
                    <a href="/contact">Contact</a>
                </nav>
            </div>
        </header>
    </Header>
    
    <Body>
        <main class="responsive-main">
            @ChildContent
        </main>
    </Body>
    
    <Footer>
        <footer class="responsive-footer">
            <div class="footer-mobile">
                <p>&copy; 2025 App</p>
            </div>
            <div class="footer-desktop">
                <div class="footer-sections">
                    <div class="footer-section">
                        <h4>Links</h4>
                        <a href="/privacy">Privacy</a>
                        <a href="/terms">Terms</a>
                    </div>
                </div>
            </div>
        </footer>
    </Footer>
</OsirionPageLayout>

<style>
.responsive-layout {
    /* Mobile-first styles */
}

.responsive-header {
    background: #ffffff;
    padding: 1rem;
    border-bottom: 1px solid #e5e7eb;
}

.mobile-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
}

.desktop-nav {
    display: none;
    position: absolute;
    top: 100%;
    left: 0;
    right: 0;
    background: white;
    border-top: 1px solid #e5e7eb;
    padding: 1rem;
}

.desktop-nav.mobile-open {
    display: block;
}

.footer-mobile {
    display: block;
    text-align: center;
}

.footer-desktop {
    display: none;
}

/* Tablet styles */
@media (min-width: 768px) {
    .mobile-header {
        display: none;
    }
    
    .desktop-nav {
        display: flex;
        position: static;
        background: transparent;
        border: none;
        gap: 2rem;
    }
    
    .footer-mobile {
        display: none;
    }
    
    .footer-desktop {
        display: block;
    }
}

/* Desktop styles */
@media (min-width: 1024px) {
    .responsive-main {
        max-width: 1200px;
        margin: 0 auto;
        padding: 2rem;
    }
    
    .footer-sections {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
        gap: 2rem;
    }
}
</style>
```

## Best Practices

### Layout Guidelines

1. **Semantic Structure**: Use proper HTML5 semantic elements (header, main, footer)
2. **Responsive Design**: Implement mobile-first responsive design principles
3. **Accessibility**: Ensure proper ARIA labels and keyboard navigation
4. **Performance**: Minimize layout shifts and optimize rendering
5. **Consistency**: Maintain consistent layout patterns across pages

### Sticky Footer Implementation

1. **Flexbox Layout**: Use flexbox for reliable sticky footer behavior
2. **Min-Height**: Set appropriate minimum heights for content areas
3. **Footer Positioning**: Ensure footer sticks to bottom when content is short
4. **Responsive Behavior**: Test sticky footer on various screen sizes

### Content Organization

1. **Clear Hierarchy**: Establish clear visual hierarchy in layout sections
2. **Spacing**: Use consistent spacing between layout sections
3. **Breakpoints**: Define appropriate responsive breakpoints
4. **Loading States**: Handle loading states gracefully in layout sections

The OsirionPageLayout component provides a solid foundation for creating consistent, accessible, and responsive page layouts in Blazor applications with minimal setup and maximum flexibility.
