---
id: 'navigation-components-overview'
order: 1
layout: docs
title: Navigation Components Overview
permalink: /docs/components/navigation
description: Complete overview of Osirion.Blazor Navigation components for advanced navigation, menus, scroll behavior, and wayfinding in Blazor applications.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Navigation Components
- User Experience
tags:
- blazor
- navigation
- menus
- scroll-behavior
- user-experience
is_featured: true
published: true
slug: components/navigation
lang: en
custom_fields: {}
seo_properties:
  title: 'Navigation Components - Osirion.Blazor Advanced Navigation'
  description: 'Explore Osirion.Blazor Navigation components for advanced menus, scroll behavior, and enhanced user navigation in Blazor applications.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/navigation'
  lang: en
  robots: index, follow
  og_title: 'Navigation Components - Osirion.Blazor'
  og_description: 'Advanced navigation components for enhanced user experience in Blazor applications.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Navigation Components - Osirion.Blazor'
  twitter_description: 'Advanced navigation components for Blazor applications.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Navigation Components Overview

The Osirion.Blazor Navigation module provides advanced navigation components designed to enhance user experience through intelligent menus, scroll management, and responsive navigation patterns. These components are optimized for modern web applications with support for complex navigation scenarios.

## Module Features

**Enhanced Navigation**: Advanced scroll behavior and page transition management
**Responsive Menus**: Multi-level menu system with mobile optimization
**Scroll Management**: Intelligent scroll-to-top and scroll position management
**Accessibility First**: Full keyboard navigation and screen reader support
**Performance Optimized**: Efficient rendering and minimal JavaScript footprint
**Framework Agnostic**: Works with any CSS framework or custom styling

## Core Components

### Navigation Enhancement Components

Components that enhance the overall navigation experience and behavior.

#### EnhancedNavigation
Advanced navigation component that provides enhanced scroll behavior and page transition management.

**Key Features:**
- Enhanced page load handling
- Scroll position management
- Smooth scroll behavior
- Page transition optimization
- SSR compatibility
- Configurable scroll behavior

```razor
<EnhancedNavigation 
    Behavior="ScrollBehavior.Smooth"
    ResetScrollOnNavigation="true"
    PreserveScrollForSamePageNavigation="true" />
```

**Parameters:**
- `Behavior`: Scroll behavior (Auto, Smooth, Instant)
- `ResetScrollOnNavigation`: Reset scroll position on navigation
- `PreserveScrollForSamePageNavigation`: Preserve scroll for same-page navigation

**Configuration:**
```csharp
// Program.cs
builder.Services.Configure<EnhancedNavigationOptions>(options =>
{
    options.Behavior = ScrollBehavior.Smooth;
    options.ResetScrollOnNavigation = true;
    options.PreserveScrollForSamePageNavigation = true;
});
```

### Menu Components

Comprehensive menu system with support for complex navigation structures.

#### Menu
Primary menu component with branding, responsive behavior, and customizable layouts.

**Key Features:**
- Brand logo and text integration
- Sticky positioning support
- Responsive mobile menu
- Multi-level menu support
- Custom branding templates
- Flexible styling options

```razor
<Menu 
    BrandText="Your Company"
    BrandLogo="/images/logo.svg"
    Href="/"
    Sticky="true"
    StickyZIndex="1000">
    
    <MenuItem Text="Home" Url="/" />
    <MenuItem Text="Products" Url="/products">
        <MenuItem Text="Web Apps" Url="/products/web" />
        <MenuItem Text="Mobile Apps" Url="/products/mobile" />
    </MenuItem>
    <MenuItem Text="About" Url="/about" />
    <MenuItem Text="Contact" Url="/contact" />
</Menu>
```

**Key Parameters:**
- `BrandText`: Text displayed in the brand area
- `BrandLogo`: Logo image URL
- `BrandingTemplate`: Custom branding content
- `Href`: Brand link URL
- `Sticky`: Enable sticky positioning
- `StickyZIndex`: Z-index for sticky menu

#### MenuItem
Individual menu item component with support for nested items and various states.

**Key Features:**
- Active state detection
- Nested menu item support
- Custom content templates
- Icon integration
- Accessibility attributes
- Flexible styling

```razor
<MenuItem 
    Text="Documentation"
    Url="/docs"
    Icon="book"
    IsActive="@IsCurrentPage("/docs")"
    OpenInNewTab="false">
    
    <MenuItem Text="Getting Started" Url="/docs/getting-started" />
    <MenuItem Text="Components" Url="/docs/components" />
    <MenuDivider />
    <MenuItem Text="Examples" Url="/docs/examples" />
</MenuItem>
```

**Parameters:**
- `Text`: Menu item display text
- `Url`: Navigation URL
- `Icon`: Icon identifier or CSS class
- `IsActive`: Active state indicator
- `OpenInNewTab`: Open link in new tab
- `ChildContent`: Nested menu items

#### MenuGroup
Container component for organizing related menu items.

**Key Features:**
- Logical grouping of menu items
- Group header support
- Collapsible sections
- Visual separation
- Accessibility grouping

```razor
<MenuGroup 
    Title="Products"
    IsCollapsible="true"
    IsExpanded="true">
    
    <MenuItem Text="Web Applications" Url="/products/web" />
    <MenuItem Text="Mobile Applications" Url="/products/mobile" />
    <MenuItem Text="Desktop Applications" Url="/products/desktop" />
</MenuGroup>
```

**Parameters:**
- `Title`: Group title/header
- `IsCollapsible`: Enable collapse functionality
- `IsExpanded`: Initial expanded state
- `ShowDivider`: Show visual divider

#### MenuDivider
Visual separator component for menu organization.

**Key Features:**
- Visual separation between menu sections
- Customizable styling
- Accessibility-friendly markup
- Responsive behavior

```razor
<MenuItem Text="Home" Url="/" />
<MenuItem Text="Products" Url="/products" />
<MenuDivider />
<MenuItem Text="Support" Url="/support" />
<MenuItem Text="Contact" Url="/contact" />
```

### Scroll Management Components

Components for managing scroll behavior and providing scroll-related functionality.

#### ScrollToTop
Scroll-to-top button component with customizable appearance and behavior.

**Key Features:**
- Automatic show/hide based on scroll position
- Smooth scroll animation
- Customizable appearance
- Accessibility compliant
- Performance optimized

```razor
<ScrollToTop 
    ShowAfterPixels="300"
    ScrollBehavior="ScrollBehavior.Smooth"
    ButtonText="Back to Top"
    Position="BottomRight" />
```

**Parameters:**
- `ShowAfterPixels`: Pixels scrolled before showing button
- `ScrollBehavior`: Scroll animation behavior
- `ButtonText`: Button text (for accessibility)
- `Position`: Button position on screen
- `ZIndex`: Button z-index

#### ScrollToTopProvider
Provider component that manages scroll-to-top functionality globally.

**Key Features:**
- Global scroll management
- Multiple scroll targets
- Event-based scroll triggering
- Performance optimization
- Memory leak prevention

```razor
<ScrollToTopProvider>
    <!-- Your application content -->
    <Router AppAssembly="@typeof(App).Assembly">
        <!-- Routes and content -->
    </Router>
    
    <ScrollToTop />
</ScrollToTopProvider>
```

## Installation and Setup

### Package Installation

```bash
dotnet add package Osirion.Blazor.Navigation
```

### Service Registration

```csharp
// Program.cs
builder.Services.AddOsirionNavigation(options =>
{
    options.EnableEnhancedNavigation = true;
    options.DefaultScrollBehavior = ScrollBehavior.Smooth;
    options.EnableScrollToTop = true;
});

// Configure enhanced navigation options
builder.Services.Configure<EnhancedNavigationOptions>(options =>
{
    options.Behavior = ScrollBehavior.Smooth;
    options.ResetScrollOnNavigation = true;
    options.PreserveScrollForSamePageNavigation = true;
});
```

### Import Statements

```razor
@* _Imports.razor *@
@using Osirion.Blazor.Navigation.Components
@using Osirion.Blazor.Navigation.Options
@using Osirion.Blazor.Navigation.Enums
```

### CSS Integration

```html
<!-- Include navigation styles -->
<link href="_content/Osirion.Blazor.Navigation/css/osirion-navigation.css" rel="stylesheet" />
```

## Usage Examples

### Basic Navigation Setup

```razor
<!-- MainLayout.razor -->
<div class="page">
    <EnhancedNavigation 
        Behavior="ScrollBehavior.Smooth"
        ResetScrollOnNavigation="true" />
    
    <header class="main-header">
        <Menu 
            BrandText="Your Company"
            BrandLogo="/images/logo.svg"
            Sticky="true">
            
            <MenuItem Text="Home" Url="/" />
            <MenuItem Text="Products" Url="/products" />
            <MenuItem Text="About" Url="/about" />
            <MenuItem Text="Contact" Url="/contact" />
        </Menu>
    </header>
    
    <main class="main-content">
        @Body
    </main>
    
    <ScrollToTopProvider>
        <ScrollToTop ShowAfterPixels="300" />
    </ScrollToTopProvider>
</div>
```

### Advanced Multi-Level Menu

```razor
<Menu 
    BrandText="TechCorp"
    BrandLogo="/images/techcorp-logo.svg"
    Sticky="true">
    
    <MenuItem Text="Home" Url="/" />
    
    <MenuItem Text="Products" Url="/products">
        <MenuGroup Title="Web Solutions">
            <MenuItem Text="E-commerce Platform" Url="/products/ecommerce" />
            <MenuItem Text="CMS Solutions" Url="/products/cms" />
            <MenuItem Text="Custom Web Apps" Url="/products/custom-web" />
        </MenuGroup>
        
        <MenuDivider />
        
        <MenuGroup Title="Mobile Solutions">
            <MenuItem Text="iOS Applications" Url="/products/ios" />
            <MenuItem Text="Android Applications" Url="/products/android" />
            <MenuItem Text="Cross-Platform Apps" Url="/products/cross-platform" />
        </MenuGroup>
    </MenuItem>
    
    <MenuItem Text="Services" Url="/services">
        <MenuItem Text="Consulting" Url="/services/consulting" />
        <MenuItem Text="Development" Url="/services/development" />
        <MenuItem Text="Support" Url="/services/support" />
        <MenuDivider />
        <MenuItem Text="Training" Url="/services/training" />
    </MenuItem>
    
    <MenuItem Text="Resources" Url="/resources">
        <MenuItem Text="Documentation" Url="/docs" />
        <MenuItem Text="Blog" Url="/blog" />
        <MenuItem Text="Case Studies" Url="/case-studies" />
        <MenuItem Text="Downloads" Url="/downloads" />
    </MenuItem>
    
    <MenuItem Text="About" Url="/about" />
    <MenuItem Text="Contact" Url="/contact" />
</Menu>
```

### Documentation Navigation

```razor
<!-- Documentation layout with sidebar navigation -->
<div class="docs-layout">
    <aside class="docs-sidebar">
        <Menu Orientation="Vertical" Sticky="false">
            <MenuGroup Title="Getting Started" IsExpanded="true">
                <MenuItem Text="Installation" Url="/docs/installation" />
                <MenuItem Text="Quick Start" Url="/docs/quick-start" />
                <MenuItem Text="Configuration" Url="/docs/configuration" />
            </MenuGroup>
            
            <MenuGroup Title="Components" IsExpanded="false">
                <MenuItem Text="Core Components" Url="/docs/components/core" />
                <MenuItem Text="CMS Components" Url="/docs/components/cms" />
                <MenuItem Text="Navigation Components" Url="/docs/components/navigation" />
            </MenuGroup>
            
            <MenuGroup Title="Advanced Topics">
                <MenuItem Text="Theming" Url="/docs/theming" />
                <MenuItem Text="Performance" Url="/docs/performance" />
                <MenuItem Text="Security" Url="/docs/security" />
            </MenuGroup>
        </Menu>
    </aside>
    
    <main class="docs-content">
        @Body
    </main>
</div>
```

### Custom Branding Template

```razor
<Menu Sticky="true">
    <BrandingTemplate>
        <div class="custom-brand">
            <img src="/images/logo.svg" alt="Company Logo" class="brand-logo" />
            <div class="brand-info">
                <span class="brand-name">TechCorp</span>
                <span class="brand-tagline">Innovation Hub</span>
            </div>
        </div>
    </BrandingTemplate>
    
    <!-- Menu items -->
    <MenuItem Text="Home" Url="/" />
    <MenuItem Text="Innovation" Url="/innovation" />
    <MenuItem Text="Research" Url="/research" />
</Menu>
```

### Dynamic Menu with Authentication

```razor
<Menu BrandText="Your App" Sticky="true">
    <MenuItem Text="Home" Url="/" />
    
    @if (isAuthenticated)
    {
        <MenuItem Text="Dashboard" Url="/dashboard" />
        <MenuItem Text="Profile" Url="/profile" />
        
        <MenuGroup Title="Admin" IsCollapsible="true">
            <MenuItem Text="Users" Url="/admin/users" />
            <MenuItem Text="Settings" Url="/admin/settings" />
            <MenuItem Text="Reports" Url="/admin/reports" />
        </MenuGroup>
        
        <MenuDivider />
        <MenuItem Text="Logout" Url="/logout" />
    }
    else
    {
        <MenuItem Text="Login" Url="/login" />
        <MenuItem Text="Register" Url="/register" />
    }
</Menu>

@code {
    private bool isAuthenticated = false;
    
    protected override async Task OnInitializedAsync()
    {
        // Check authentication status
        isAuthenticated = await AuthService.IsAuthenticatedAsync();
    }
}
```

## Styling and Customization

### CSS Custom Properties

```css
:root {
    /* Menu styling */
    --menu-background: #ffffff;
    --menu-border: #e5e7eb;
    --menu-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    --menu-z-index: 1000;
    
    /* Menu item styling */
    --menu-item-padding: 0.75rem 1rem;
    --menu-item-color: #374151;
    --menu-item-hover-bg: #f3f4f6;
    --menu-item-active-bg: #3b82f6;
    --menu-item-active-color: #ffffff;
    
    /* Brand styling */
    --brand-font-size: 1.25rem;
    --brand-font-weight: 600;
    --brand-color: #111827;
    
    /* Scroll to top styling */
    --scroll-top-bg: #3b82f6;
    --scroll-top-color: #ffffff;
    --scroll-top-size: 3rem;
    --scroll-top-border-radius: 50%;
}
```

### Custom Menu Styles

```css
.custom-menu {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    border: none;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.custom-menu .menu-item {
    color: white;
    transition: all 0.3s ease;
}

.custom-menu .menu-item:hover {
    background-color: rgba(255, 255, 255, 0.1);
    transform: translateY(-1px);
}

.custom-menu .menu-item.active {
    background-color: rgba(255, 255, 255, 0.2);
    font-weight: 600;
}
```

### Responsive Menu Styling

```css
@media (max-width: 768px) {
    .osirion-menu {
        padding: 0.5rem;
    }
    
    .menu-items {
        flex-direction: column;
        width: 100%;
    }
    
    .menu-item {
        width: 100%;
        text-align: left;
        border-bottom: 1px solid var(--menu-border);
    }
    
    .menu-toggle {
        display: block;
    }
}
```

## Accessibility Features

### Keyboard Navigation
- **Tab Navigation**: Full keyboard accessibility for all menu items
- **Arrow Keys**: Navigate through menu items using arrow keys
- **Enter/Space**: Activate menu items
- **Escape**: Close mobile menus and dropdowns

### Screen Reader Support
- **ARIA Labels**: Comprehensive ARIA labeling for navigation elements
- **Role Attributes**: Proper role assignments for navigation structure
- **Live Regions**: Announce navigation changes to screen readers
- **Skip Links**: Skip navigation functionality for screen readers

### Focus Management
- **Visible Focus**: Clear focus indicators for keyboard users
- **Focus Trapping**: Proper focus management in mobile menus
- **Focus Restoration**: Return focus to appropriate elements after navigation

## Performance Considerations

### Efficient Rendering
- **Virtual Scrolling**: Efficient handling of large menu structures
- **Lazy Loading**: Lazy load menu content when needed
- **Minimal DOM**: Optimized DOM structure for fast rendering
- **Event Delegation**: Efficient event handling for menu interactions

### JavaScript Optimization
- **Minimal JavaScript**: Lightweight JavaScript footprint
- **Event Cleanup**: Proper cleanup of event listeners
- **Memory Management**: Prevent memory leaks in SPA scenarios
- **Bundle Size**: Optimized for minimal bundle impact

## Best Practices

### Menu Organization
- Keep menu depth to 3 levels maximum
- Group related items using MenuGroup
- Use meaningful and descriptive labels
- Maintain consistent navigation patterns

### Responsive Design
- Design mobile-first navigation
- Test on various screen sizes
- Ensure touch-friendly interaction areas
- Consider thumb navigation on mobile

### Performance
- Limit the number of menu items
- Use efficient filtering for dynamic menus
- Implement proper caching strategies
- Monitor Core Web Vitals impact

### Accessibility
- Provide alternative navigation methods
- Test with keyboard-only navigation
- Verify screen reader compatibility
- Maintain proper color contrast

The Navigation components provide a comprehensive solution for creating sophisticated navigation experiences while maintaining excellent performance, accessibility, and user experience standards across all device types and interaction methods.
