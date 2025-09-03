---
id: 'responsive-showcase-section'
order: 3
layout: docs
title: OsirionResponsiveShowcaseSection Component
permalink: /docs/components/core/sections/responsive-showcase-section
description: Learn how to use the OsirionResponsiveShowcaseSection component to demonstrate responsive design and test components across different viewport sizes with browser chrome simulation.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Sections
- Responsive Design
tags:
- blazor
- responsive
- showcase
- viewport
- testing
- demo
- mobile
- tablet
- desktop
is_featured: true
published: true
slug: components/core/sections/responsive-showcase-section
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionResponsiveShowcaseSection Component - Responsive Design Testing | Osirion.Blazor'
  description: 'Test and showcase responsive designs with the OsirionResponsiveShowcaseSection component. Features viewport switching and browser chrome simulation.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/sections/responsive-showcase-section'
  lang: en
  robots: index, follow
  og_title: 'OsirionResponsiveShowcaseSection Component - Osirion.Blazor'
  og_description: 'Showcase responsive designs with viewport switching and browser chrome simulation.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionResponsiveShowcaseSection Component - Osirion.Blazor'
  twitter_description: 'Test responsive designs with viewport switching and browser simulation.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionResponsiveShowcaseSection Component

The OsirionResponsiveShowcaseSection component provides a professional way to demonstrate responsive design by simulating different viewport sizes with browser chrome styling. Perfect for component libraries, design systems, and showcasing responsive layouts.

## Component Overview

OsirionResponsiveShowcaseSection is designed for developers and designers who need to showcase how components adapt across different screen sizes. It provides viewport switching capabilities with realistic browser chrome simulation, making it ideal for documentation, presentations, and client demos.

### Key Features

**Viewport Simulation**: Switch between desktop, tablet, and mobile views
**Browser Chrome**: Realistic browser window styling with controls
**Code Integration**: Optional code snippet display with syntax highlighting
**Dimension Display**: Show current viewport dimensions and descriptions
**SSR Compatible**: Works without JavaScript for basic functionality
**Accessibility Compliant**: Full keyboard navigation and screen reader support
**Responsive Testing**: Perfect for testing component responsiveness
**Interactive Controls**: Easy-to-use viewport switching buttons
**Professional Styling**: Clean, modern appearance for presentations
**Customizable**: Flexible configuration options for different use cases

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment?` | `null` | The component or content to showcase in the viewport. |
| `Title` | `string` | `"Component Preview"` | The title displayed in the showcase header. |
| `ShowDimensions` | `bool` | `true` | Whether to show viewport dimensions information. |
| `ShowCode` | `bool` | `true` | Whether to show the code snippet section. |
| `CodeSnippet` | `string?` | `null` | The code snippet to display when Show Code is clicked. |
| `InitialViewport` | `ViewportMode` | `Desktop` | The initial viewport mode to display. |
| `ShowBrowserChrome` | `bool` | `true` | Whether to include browser chrome styling. |

## Viewport Modes

| Mode | Dimensions | Description | Best For |
|------|------------|-------------|----------|
| `Desktop` | 1200px × 600px+ | Large screens and laptops | Full layouts, complex interfaces |
| `Tablet` | 768px × 500px+ | iPad and tablet devices | Medium layouts, touch interfaces |
| `Mobile` | 375px × 600px+ | iPhone and Android phones | Compact layouts, mobile-first design |

## Basic Usage

### Simple Component Showcase

```razor
@using Osirion.Blazor.Components

<OsirionResponsiveShowcaseSection 
    Title="Responsive Card Component"
    ShowDimensions="true"
    ShowCode="true"
    CodeSnippet="<div class=\"card\">
  <div class=\"card-body\">
    <h5 class=\"card-title\">Sample Card</h5>
    <p class=\"card-text\">This card adapts to different screen sizes.</p>
    <a href=\"#\" class=\"btn btn-primary\">Learn More</a>
  </div>
</div>">
    
    <div class="card" style="max-width: 100%;">
        <div class="card-body">
            <h5 class="card-title">Sample Card</h5>
            <p class="card-text">
                This card demonstrates how content adapts across different viewport sizes. 
                Notice how the layout changes when you switch between desktop, tablet, and mobile views.
            </p>
            <a href="#" class="btn btn-primary">Learn More</a>
        </div>
    </div>
</OsirionResponsiveShowcaseSection>
```

### Navigation Component Showcase

```razor
<OsirionResponsiveShowcaseSection 
    Title="Responsive Navigation"
    InitialViewport="ViewportMode.Mobile"
    ShowDimensions="true"
    CodeSnippet="<nav class=\"navbar navbar-expand-lg navbar-light bg-light\">
  <div class=\"container-fluid\">
    <a class=\"navbar-brand\" href=\"#\">Brand</a>
    <button class=\"navbar-toggler\" type=\"button\">
      <span class=\"navbar-toggler-icon\"></span>
    </button>
    <div class=\"navbar-nav\">
      <a class=\"nav-link\" href=\"#\">Home</a>
      <a class=\"nav-link\" href=\"#\">About</a>
      <a class=\"nav-link\" href=\"#\">Services</a>
      <a class=\"nav-link\" href=\"#\">Contact</a>
    </div>
  </div>
</nav>">
    
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <div class="container-fluid">
            <a class="navbar-brand" href="#">
                <img src="/images/logo.svg" alt="Logo" width="30" height="24" class="d-inline-block align-text-top">
                Brand
            </a>
            <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarNav">
                <ul class="navbar-nav ms-auto">
                    <li class="nav-item">
                        <a class="nav-link active" href="#">Home</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">About</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">Services</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#">Contact</a>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
</OsirionResponsiveShowcaseSection>
```

### Grid Layout Showcase

```razor
<OsirionResponsiveShowcaseSection 
    Title="Responsive Grid System"
    ShowDimensions="true"
    CodeSnippet="<div class=\"row g-3\">
  <div class=\"col-lg-4 col-md-6 col-12\">
    <div class=\"card h-100\">
      <div class=\"card-body\">
        <h6 class=\"card-title\">Feature 1</h6>
        <p class=\"card-text\">Description of feature one.</p>
      </div>
    </div>
  </div>
  <!-- Repeat for other features -->
</div>">
    
    <div class="container-fluid">
        <div class="row g-3">
            <div class="col-lg-4 col-md-6 col-12">
                <div class="card h-100">
                    <div class="card-body">
                        <div class="d-flex align-items-center mb-2">
                            <i class="fas fa-rocket text-primary me-2"></i>
                            <h6 class="card-title mb-0">Performance</h6>
                        </div>
                        <p class="card-text small">Lightning-fast load times and optimized performance.</p>
                    </div>
                </div>
            </div>
            <div class="col-lg-4 col-md-6 col-12">
                <div class="card h-100">
                    <div class="card-body">
                        <div class="d-flex align-items-center mb-2">
                            <i class="fas fa-shield-alt text-success me-2"></i>
                            <h6 class="card-title mb-0">Security</h6>
                        </div>
                        <p class="card-text small">Enterprise-grade security with data encryption.</p>
                    </div>
                </div>
            </div>
            <div class="col-lg-4 col-md-6 col-12">
                <div class="card h-100">
                    <div class="card-body">
                        <div class="d-flex align-items-center mb-2">
                            <i class="fas fa-users text-info me-2"></i>
                            <h6 class="card-title mb-0">Collaboration</h6>
                        </div>
                        <p class="card-text small">Real-time collaboration tools for teams.</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</OsirionResponsiveShowcaseSection>
```

## Advanced Usage

### Component Library Documentation

```razor
@using Osirion.Blazor.Components

<div class="component-documentation">
    <div class="row">
        <div class="col-12">
            <h2>Button Component Variants</h2>
            <p class="lead">
                Explore how our button component adapts across different screen sizes and contexts.
            </p>
        </div>
    </div>
    
    <div class="row g-4">
        <!-- Primary Buttons -->
        <div class="col-12">
            <OsirionResponsiveShowcaseSection 
                Title="Primary Button Variations"
                ShowDimensions="false"
                CodeSnippet="<!-- Primary Buttons -->
<button class=\"btn btn-primary\">Default</button>
<button class=\"btn btn-primary btn-lg\">Large</button>
<button class=\"btn btn-primary btn-sm\">Small</button>

<!-- Responsive Button Group -->
<div class=\"btn-group-vertical d-block d-sm-none\">
  <button class=\"btn btn-primary\">Mobile Stack</button>
  <button class=\"btn btn-outline-primary\">Mobile Stack</button>
</div>

<div class=\"btn-group d-none d-sm-flex\">
  <button class=\"btn btn-primary\">Desktop Group</button>
  <button class=\"btn btn-outline-primary\">Desktop Group</button>
</div>">
                
                <div class="button-showcase p-4">
                    <div class="mb-3">
                        <h5>Standard Sizes</h5>
                        <button class="btn btn-primary me-2">Default</button>
                        <button class="btn btn-primary btn-lg me-2">Large</button>
                        <button class="btn btn-primary btn-sm">Small</button>
                    </div>
                    
                    <div class="mb-3">
                        <h5>Responsive Behavior</h5>
                        <!-- Mobile: Stack vertically -->
                        <div class="btn-group-vertical d-block d-sm-none">
                            <button class="btn btn-primary">Mobile Stack</button>
                            <button class="btn btn-outline-primary">Mobile Stack</button>
                        </div>
                        
                        <!-- Desktop: Show as group -->
                        <div class="btn-group d-none d-sm-flex">
                            <button class="btn btn-primary">Desktop Group</button>
                            <button class="btn btn-outline-primary">Desktop Group</button>
                        </div>
                    </div>
                    
                    <div>
                        <h5>Full-Width Mobile</h5>
                        <button class="btn btn-primary w-100 d-block d-sm-inline-block">
                            Responsive Width
                        </button>
                    </div>
                </div>
            </OsirionResponsiveShowcaseSection>
        </div>
        
        <!-- Form Controls -->
        <div class="col-12">
            <OsirionResponsiveShowcaseSection 
                Title="Responsive Form Layout"
                InitialViewport="ViewportMode.Tablet"
                CodeSnippet="<form class=\"row g-3\">
  <div class=\"col-md-6\">
    <label for=\"firstName\" class=\"form-label\">First Name</label>
    <input type=\"text\" class=\"form-control\" id=\"firstName\">
  </div>
  <div class=\"col-md-6\">
    <label for=\"lastName\" class=\"form-label\">Last Name</label>
    <input type=\"text\" class=\"form-control\" id=\"lastName\">
  </div>
  <div class=\"col-12\">
    <label for=\"email\" class=\"form-label\">Email</label>
    <input type=\"email\" class=\"form-control\" id=\"email\">
  </div>
  <div class=\"col-12\">
    <button type=\"submit\" class=\"btn btn-primary\">Submit</button>
  </div>
</form>">
                
                <div class="form-showcase p-4">
                    <form class="row g-3">
                        <div class="col-md-6">
                            <label for="firstName" class="form-label">First Name</label>
                            <input type="text" class="form-control" id="firstName" placeholder="John">
                        </div>
                        <div class="col-md-6">
                            <label for="lastName" class="form-label">Last Name</label>
                            <input type="text" class="form-control" id="lastName" placeholder="Doe">
                        </div>
                        <div class="col-12">
                            <label for="email" class="form-label">Email Address</label>
                            <input type="email" class="form-control" id="email" placeholder="john.doe@example.com">
                        </div>
                        <div class="col-md-8">
                            <label for="message" class="form-label">Message</label>
                            <textarea class="form-control" id="message" rows="3" placeholder="Your message here..."></textarea>
                        </div>
                        <div class="col-md-4 d-flex align-items-end">
                            <button type="submit" class="btn btn-primary w-100">Send Message</button>
                        </div>
                    </form>
                </div>
            </OsirionResponsiveShowcaseSection>
        </div>
    </div>
</div>

<style>
.component-documentation {
    margin: 2rem 0;
}

.button-showcase h5 {
    color: #495057;
    font-size: 1rem;
    font-weight: 600;
    margin-bottom: 0.75rem;
}

.form-showcase {
    background: #f8f9fa;
    border-radius: 0.5rem;
}

.form-showcase .form-label {
    font-weight: 500;
    color: #495057;
}
</style>
```

### Design System Showcase

```razor
<div class="design-system-showcase">
    <h2 class="text-center mb-5">Design System Components</h2>
    
    <!-- Typography Showcase -->
    <OsirionResponsiveShowcaseSection 
        Title="Typography Scale"
        ShowCode="false"
        Class="mb-5">
        
        <div class="typography-showcase p-4">
            <h1>Heading 1 - Display</h1>
            <h2>Heading 2 - Page Title</h2>
            <h3>Heading 3 - Section</h3>
            <h4>Heading 4 - Subsection</h4>
            <h5>Heading 5 - Component</h5>
            <h6>Heading 6 - Small</h6>
            
            <p class="lead">
                Lead paragraph: This is a lead paragraph that stands out from regular body text.
            </p>
            
            <p>
                Body text: This is regular paragraph text that forms the main content of articles and descriptions. 
                It should be readable across all device sizes.
            </p>
            
            <small class="text-muted">
                Small text: Used for captions, legal text, and secondary information.
            </small>
        </div>
    </OsirionResponsiveShowcaseSection>
    
    <!-- Card Components -->
    <OsirionResponsiveShowcaseSection 
        Title="Card Component Grid"
        ShowDimensions="true"
        CodeSnippet="<div class=\"row g-4\">
  <div class=\"col-xl-3 col-lg-4 col-md-6\">
    <div class=\"card h-100\">
      <img src=\"...\" class=\"card-img-top\" alt=\"...\">
      <div class=\"card-body d-flex flex-column\">
        <h5 class=\"card-title\">Card Title</h5>
        <p class=\"card-text flex-grow-1\">Card description text.</p>
        <a href=\"#\" class=\"btn btn-primary mt-auto\">Read More</a>
      </div>
    </div>
  </div>
</div>">
        
        <div class="container-fluid">
            <div class="row g-4">
                <div class="col-xl-3 col-lg-4 col-md-6">
                    <div class="card h-100">
                        <div class="card-img-top bg-primary d-flex align-items-center justify-content-center text-white" style="height: 200px;">
                            <i class="fas fa-image fa-2x"></i>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">Feature Card</h5>
                            <p class="card-text flex-grow-1">
                                This card demonstrates responsive behavior across different screen sizes.
                            </p>
                            <a href="#" class="btn btn-primary mt-auto">Learn More</a>
                        </div>
                    </div>
                </div>
                <div class="col-xl-3 col-lg-4 col-md-6">
                    <div class="card h-100">
                        <div class="card-img-top bg-success d-flex align-items-center justify-content-center text-white" style="height: 200px;">
                            <i class="fas fa-chart-line fa-2x"></i>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">Analytics</h5>
                            <p class="card-text flex-grow-1">
                                Track performance and get insights with detailed analytics.
                            </p>
                            <a href="#" class="btn btn-success mt-auto">View Stats</a>
                        </div>
                    </div>
                </div>
                <div class="col-xl-3 col-lg-4 col-md-6">
                    <div class="card h-100">
                        <div class="card-img-top bg-info d-flex align-items-center justify-content-center text-white" style="height: 200px;">
                            <i class="fas fa-users fa-2x"></i>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">Team Tools</h5>
                            <p class="card-text flex-grow-1">
                                Collaborate effectively with powerful team management tools.
                            </p>
                            <a href="#" class="btn btn-info mt-auto">Get Started</a>
                        </div>
                    </div>
                </div>
                <div class="col-xl-3 col-lg-4 col-md-6">
                    <div class="card h-100">
                        <div class="card-img-top bg-warning d-flex align-items-center justify-content-center text-white" style="height: 200px;">
                            <i class="fas fa-cog fa-2x"></i>
                        </div>
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">Settings</h5>
                            <p class="card-text flex-grow-1">
                                Customize your experience with flexible configuration options.
                            </p>
                            <a href="#" class="btn btn-warning mt-auto">Configure</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </OsirionResponsiveShowcaseSection>
</div>

<style>
.design-system-showcase {
    margin: 3rem 0;
}

.typography-showcase {
    background: white;
    border: 1px solid #e9ecef;
    border-radius: 0.5rem;
}

.typography-showcase h1 {
    color: #212529;
    margin-bottom: 1rem;
}

.typography-showcase h2,
.typography-showcase h3,
.typography-showcase h4,
.typography-showcase h5,
.typography-showcase h6 {
    color: #495057;
    margin-bottom: 0.75rem;
}

.typography-showcase p {
    margin-bottom: 1rem;
    line-height: 1.6;
}
</style>
```

### Interactive Component Showcase

```razor
<OsirionResponsiveShowcaseSection 
    Title="Interactive Dashboard Widget"
    ShowDimensions="true"
    ShowBrowserChrome="true"
    CodeSnippet="<div class=\"dashboard-widget\">
  <div class=\"widget-header\">
    <h6 class=\"widget-title\">Sales Overview</h6>
    <div class=\"widget-actions\">
      <button class=\"btn btn-sm btn-outline-secondary\">⋯</button>
    </div>
  </div>
  <div class=\"widget-body\">
    <div class=\"metric\">
      <div class=\"metric-value\">$24,500</div>
      <div class=\"metric-label\">This Month</div>
      <div class=\"metric-change positive\">+12.5%</div>
    </div>
    <div class=\"chart-placeholder\">
      [Chart Component]
    </div>
  </div>
</div>">
    
    <div class="dashboard-widget">
        <div class="widget-header">
            <h6 class="widget-title">Sales Overview</h6>
            <div class="widget-actions">
                <div class="dropdown">
                    <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown">
                        <i class="fas fa-ellipsis-h"></i>
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item" href="#">View Details</a></li>
                        <li><a class="dropdown-item" href="#">Export Data</a></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><a class="dropdown-item" href="#">Settings</a></li>
                    </ul>
                </div>
            </div>
        </div>
        <div class="widget-body">
            <div class="row g-3">
                <div class="col-6">
                    <div class="metric">
                        <div class="metric-value">$24,500</div>
                        <div class="metric-label">This Month</div>
                        <div class="metric-change positive">
                            <i class="fas fa-arrow-up me-1"></i>
                            +12.5%
                        </div>
                    </div>
                </div>
                <div class="col-6">
                    <div class="metric">
                        <div class="metric-value">1,234</div>
                        <div class="metric-label">Orders</div>
                        <div class="metric-change positive">
                            <i class="fas fa-arrow-up me-1"></i>
                            +8.2%
                        </div>
                    </div>
                </div>
                <div class="col-12">
                    <div class="chart-placeholder">
                        <div class="chart-bars">
                            <div class="bar" style="height: 60%;"></div>
                            <div class="bar" style="height: 80%;"></div>
                            <div class="bar" style="height: 45%;"></div>
                            <div class="bar" style="height: 90%;"></div>
                            <div class="bar" style="height: 75%;"></div>
                            <div class="bar" style="height: 100%;"></div>
                            <div class="bar" style="height: 85%;"></div>
                        </div>
                        <div class="chart-labels">
                            <small>Mon</small>
                            <small>Tue</small>
                            <small>Wed</small>
                            <small>Thu</small>
                            <small>Fri</small>
                            <small>Sat</small>
                            <small>Sun</small>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</OsirionResponsiveShowcaseSection>

<style>
.dashboard-widget {
    background: white;
    border: 1px solid #e9ecef;
    border-radius: 0.75rem;
    overflow: hidden;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.widget-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1rem 1.25rem;
    border-bottom: 1px solid #e9ecef;
    background: #f8f9fa;
}

.widget-title {
    margin: 0;
    font-weight: 600;
    color: #495057;
}

.widget-body {
    padding: 1.25rem;
}

.metric {
    text-align: center;
}

.metric-value {
    font-size: 2rem;
    font-weight: 700;
    color: #212529;
    line-height: 1;
    margin-bottom: 0.25rem;
}

.metric-label {
    font-size: 0.875rem;
    color: #6c757d;
    margin-bottom: 0.25rem;
}

.metric-change {
    font-size: 0.875rem;
    font-weight: 500;
}

.metric-change.positive {
    color: #198754;
}

.metric-change.negative {
    color: #dc3545;
}

.chart-placeholder {
    background: #f8f9fa;
    border-radius: 0.5rem;
    padding: 1rem;
    margin-top: 1rem;
}

.chart-bars {
    display: flex;
    align-items: end;
    justify-content: space-between;
    height: 100px;
    margin-bottom: 0.5rem;
}

.bar {
    background: linear-gradient(to top, #0d6efd, #6ea8fe);
    width: 12px;
    border-radius: 2px 2px 0 0;
    min-height: 4px;
}

.chart-labels {
    display: flex;
    justify-content: space-between;
    color: #6c757d;
}

@media (max-width: 576px) {
    .widget-body {
        padding: 1rem;
    }
    
    .metric-value {
        font-size: 1.5rem;
    }
    
    .chart-bars {
        height: 60px;
    }
}
</style>
```

## Styling Examples

### Custom Theme Integration

```razor
<OsirionResponsiveShowcaseSection 
    Title="Custom Theme Example"
    Class="theme-dark"
    ShowBrowserChrome="false">
    
    <div class="custom-theme-content p-4">
        <h3>Dark Theme Components</h3>
        <p>This showcases how components look in a custom dark theme.</p>
        
        <div class="row g-3">
            <div class="col-md-6">
                <button class="btn btn-primary w-100">Primary Action</button>
            </div>
            <div class="col-md-6">
                <button class="btn btn-outline-light w-100">Secondary Action</button>
            </div>
        </div>
    </div>
</OsirionResponsiveShowcaseSection>

<style>
.osirion-responsive-showcase.theme-dark {
    background: #1a1a1a;
    border-radius: 0.5rem;
}

.osirion-responsive-showcase.theme-dark .showcase-header {
    background: #2d2d2d;
    color: white;
}

.osirion-responsive-showcase.theme-dark .viewport-btn {
    background: #404040;
    color: white;
    border: 1px solid #555;
}

.osirion-responsive-showcase.theme-dark .viewport-btn.active {
    background: #0d6efd;
    border-color: #0d6efd;
}

.custom-theme-content {
    background: #2d2d2d;
    color: white;
    border-radius: 0.375rem;
}

.custom-theme-content h3 {
    color: #f8f9fa;
    margin-bottom: 1rem;
}

.custom-theme-content p {
    color: #adb5bd;
}
</style>
```

## Best Practices

### Documentation Guidelines

1. **Clear Examples**: Provide realistic, functional examples
2. **Code Snippets**: Include relevant code for each showcase
3. **Responsive Testing**: Test across all viewport sizes
4. **Performance**: Optimize showcased components for smooth interactions
5. **Accessibility**: Ensure showcased content is accessible

### Design Considerations

1. **Realistic Content**: Use real-world content in examples
2. **Visual Hierarchy**: Maintain clear visual hierarchy across viewports
3. **Interactive Elements**: Show how interactive elements adapt
4. **Loading States**: Demonstrate loading and error states
5. **Edge Cases**: Include edge cases and extreme content lengths

### Technical Implementation

1. **SSR Compatibility**: Ensure showcased components work server-side
2. **Progressive Enhancement**: Start with basic functionality
3. **Performance Optimization**: Minimize JavaScript dependencies
4. **Browser Testing**: Test across different browsers
5. **Mobile Testing**: Verify touch interactions on mobile devices

### Content Strategy

1. **User Journey**: Show components in realistic user scenarios
2. **Feature Demonstration**: Highlight key features and benefits
3. **Comparison Views**: Show before/after or variant comparisons
4. **Error Handling**: Include error states and validation
5. **Success States**: Show completion and success states

The OsirionResponsiveShowcaseSection component provides a powerful tool for demonstrating responsive design and creating professional component documentation.
