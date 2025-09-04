---
id: 'base-section'
order: 1
layout: docs
title: OsirionBaseSection Component
permalink: /docs/components/core/sections/base-section
description: Learn how to use the OsirionBaseSection component to create structured content sections with consistent styling, background options, and framework-agnostic container layouts.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Sections
tags:
- blazor
- section
- layout
- structure
- background
- container
is_featured: true
published: true
slug: base-section
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionBaseSection Component - Structured Content Sections | Osirion.Blazor'
  description: 'Create structured content sections with the OsirionBaseSection component. Features background images, patterns, and framework-agnostic layouts.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/sections/base-section'
  lang: en
  robots: index, follow
  og_title: 'OsirionBaseSection Component - Osirion.Blazor'
  og_description: 'Create structured content sections with background images, patterns, and consistent layouts.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionBaseSection Component - Osirion.Blazor'
  twitter_description: 'Create structured content sections with background images and consistent layouts.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionBaseSection Component

The OsirionBaseSection component provides a foundational structure for creating consistent content sections across your application. It offers flexible styling options, background support, and framework-agnostic container layouts.

## Component Overview

OsirionBaseSection is designed to standardize section layouts throughout your application. Whether you're building landing pages, content areas, or feature sections, this component ensures consistent spacing, alignment, and styling while adapting to your chosen CSS framework.

### Key Features

**Framework Agnostic**: Automatic container classes for Bootstrap, Fluent UI, MudBlazor, and Radzen
**Flexible Content**: Support for custom titles, descriptions, and child content
**Background Options**: Images, patterns, and color backgrounds with overlay support
**Responsive Design**: Consistent layouts across all screen sizes
**Accessibility Compliant**: Semantic markup with proper landmark roles
**Padding Control**: Multiple padding sizes for different content needs
**Text Alignment**: Support for all text alignment options
**Pattern Integration**: Built-in background pattern support
**SEO Optimized**: Proper heading structure and semantic elements
**Styling Flexibility**: Easy customization with CSS variables and classes

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string?` | `null` | **Required.** Unique identifier for the section element. |
| `ChildContent` | `RenderFragment?` | `null` | The main content of the section. |
| `Title` | `string?` | `null` | The section title text. |
| `TitleContent` | `RenderFragment?` | `null` | Custom title content (overrides Title). |
| `Description` | `string?` | `null` | The section description text. |
| `DescriptionContent` | `RenderFragment?` | `null` | Custom description content (overrides Description). |
| `ContainerClass` | `string?` | `null` | Override the default container CSS class. |
| `TextAlignment` | `Alignment` | `Center` | Text alignment (Left, Center, Right, Justify). |
| `BackgroundImageUrl` | `string?` | `null` | URL of the background image. |
| `BackgroundColor` | `string?` | `null` | Background color override. |
| `TextColor` | `string?` | `null` | Text color override. |
| `ShowOverlay` | `bool` | `true` | Whether to show overlay on background images. |
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | Background pattern type. |
| `ShowPattern` | `bool` | `false` | Whether to show the background pattern. |
| `Padding` | `SectionPadding` | `Medium` | Section padding size (None, Small, Medium, Large). |
| `HasDivider` | `bool` | `false` | Whether to show section divider/shadow effect. |

## Section Padding Options

| Padding | Description | Best Use Case |
|---------|-------------|---------------|
| `None` | No padding | Full-width sections, custom layouts |
| `Small` | Minimal padding | Compact sections, sidebars |
| `Medium` | Standard padding | General content sections |
| `Large` | Extra padding | Feature sections, hero areas |

## Basic Usage

### Simple Content Section

```razor
@using Osirion.Blazor.Components

<OsirionBaseSection Id="intro-section" 
    Title="Welcome to Our Platform"
    Description="Discover the tools that will transform your workflow and boost productivity.">
    
    <div class="row g-4">
        <div class="col-md-4">
            <div class="feature-item">
                <i class="fas fa-rocket fa-3x text-primary mb-3"></i>
                <h4>Fast Performance</h4>
                <p>Lightning-fast load times and smooth interactions.</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="feature-item">
                <i class="fas fa-shield-alt fa-3x text-success mb-3"></i>
                <h4>Secure by Design</h4>
                <p>Enterprise-grade security built into every feature.</p>
            </div>
        </div>
        <div class="col-md-4">
            <div class="feature-item">
                <i class="fas fa-users fa-3x text-info mb-3"></i>
                <h4>Team Collaboration</h4>
                <p>Tools designed for seamless team collaboration.</p>
            </div>
        </div>
    </div>
</OsirionBaseSection>
```

### Section with Background Image

```razor
<OsirionBaseSection Id="team-section"
    Title="Meet Our Team"
    Description="The passionate individuals behind our success"
    BackgroundImageUrl="https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=1920&h=1080&fit=crop"
    TextColor="white"
    ShowOverlay="true"
    Padding="SectionPadding.Large">
    
    <div class="row g-4 justify-content-center">
        <div class="col-lg-3 col-md-6">
            <div class="team-member-card">
                <img src="https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=300&h=300&fit=crop" 
                     alt="John Smith" class="team-photo">
                <h5>John Smith</h5>
                <p class="text-muted">CEO & Founder</p>
            </div>
        </div>
        <div class="col-lg-3 col-md-6">
            <div class="team-member-card">
                <img src="https://images.unsplash.com/photo-1494790108755-2616b66c1e2f?w=300&h=300&fit=crop" 
                     alt="Jane Doe" class="team-photo">
                <h5>Jane Doe</h5>
                <p class="text-muted">CTO</p>
            </div>
        </div>
        <div class="col-lg-3 col-md-6">
            <div class="team-member-card">
                <img src="https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&h=300&fit=crop" 
                     alt="Mike Johnson" class="team-photo">
                <h5>Mike Johnson</h5>
                <p class="text-muted">Lead Designer</p>
            </div>
        </div>
    </div>
</OsirionBaseSection>

<style>
.team-member-card {
    background: rgba(255, 255, 255, 0.95);
    padding: 2rem;
    border-radius: 1rem;
    text-align: center;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
    transition: transform 0.3s ease;
}

.team-member-card:hover {
    transform: translateY(-5px);
}

.team-photo {
    width: 120px;
    height: 120px;
    border-radius: 50%;
    object-fit: cover;
    margin-bottom: 1rem;
    border: 4px solid white;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

.team-member-card h5 {
    color: #333;
    margin-bottom: 0.5rem;
    font-weight: 600;
}
</style>
```

### Section with Background Pattern

```razor
<OsirionBaseSection Id="stats-section"
    Title="Our Impact in Numbers"
    Description="See how we're making a difference across the globe"
    BackgroundPattern="BackgroundPatternType.TechWave"
    ShowPattern="true"
    BackgroundColor="#4f46e5"
    TextColor="white"
    Padding="SectionPadding.Large">
    
    <div class="row g-4 text-center">
        <div class="col-lg-3 col-md-6">
            <div class="stat-item">
                <div class="stat-number">50K+</div>
                <div class="stat-label">Active Users</div>
            </div>
        </div>
        <div class="col-lg-3 col-md-6">
            <div class="stat-item">
                <div class="stat-number">1M+</div>
                <div class="stat-label">Projects Created</div>
            </div>
        </div>
        <div class="col-lg-3 col-md-6">
            <div class="stat-item">
                <div class="stat-number">99.9%</div>
                <div class="stat-label">Uptime</div>
            </div>
        </div>
        <div class="col-lg-3 col-md-6">
            <div class="stat-item">
                <div class="stat-number">24/7</div>
                <div class="stat-label">Support</div>
            </div>
        </div>
    </div>
</OsirionBaseSection>

<style>
.stat-item {
    padding: 2rem;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 1rem;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
    transition: transform 0.3s ease;
}

.stat-item:hover {
    transform: translateY(-5px);
}

.stat-number {
    font-size: 3rem;
    font-weight: 900;
    line-height: 1;
    margin-bottom: 0.5rem;
    background: linear-gradient(45deg, #fbbf24, #f59e0b);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
}

.stat-label {
    font-size: 1.1rem;
    opacity: 0.9;
    font-weight: 500;
}
</style>
```

## Advanced Usage

### Custom Title and Description with Render Fragments

```razor
@using Osirion.Blazor.Components

<OsirionBaseSection Id="features-section"
    BackgroundColor="#f8f9fa"
    Padding="SectionPadding.Large"
    TextAlignment="Alignment.Center">
    
    <TitleContent>
        <div class="section-header-custom">
            <span class="section-badge">
                <i class="fas fa-star me-2"></i>
                Premium Features
            </span>
            <h2 class="section-title-gradient">
                Powerful Tools for
                <br />
                <span class="highlight">Modern Teams</span>
            </h2>
        </div>
    </TitleContent>
    
    <DescriptionContent>
        <div class="section-description-custom">
            <p class="lead mb-4">
                Discover the comprehensive suite of tools designed to streamline your workflow,
                enhance collaboration, and accelerate your project delivery.
            </p>
            <div class="feature-badges">
                <span class="badge bg-primary me-2">Cloud Native</span>
                <span class="badge bg-success me-2">Real-time Sync</span>
                <span class="badge bg-info me-2">API First</span>
                <span class="badge bg-warning">Enterprise Ready</span>
            </div>
        </div>
    </DescriptionContent>
    
    <div class="feature-grid">
        <div class="row g-4">
            <div class="col-lg-4 col-md-6">
                <div class="feature-card h-100">
                    <div class="feature-icon">
                        <i class="fas fa-bolt text-warning"></i>
                    </div>
                    <h4>Lightning Fast</h4>
                    <p>Optimized performance that scales with your needs. Experience sub-second response times.</p>
                    <a href="/features/performance" class="feature-link">
                        Learn more <i class="fas fa-arrow-right ms-1"></i>
                    </a>
                </div>
            </div>
            <div class="col-lg-4 col-md-6">
                <div class="feature-card h-100">
                    <div class="feature-icon">
                        <i class="fas fa-lock text-success"></i>
                    </div>
                    <h4>Bank-Level Security</h4>
                    <p>Enterprise-grade security with end-to-end encryption and compliance certifications.</p>
                    <a href="/features/security" class="feature-link">
                        Learn more <i class="fas fa-arrow-right ms-1"></i>
                    </a>
                </div>
            </div>
            <div class="col-lg-4 col-md-6">
                <div class="feature-card h-100">
                    <div class="feature-icon">
                        <i class="fas fa-sync text-primary"></i>
                    </div>
                    <h4>Real-time Collaboration</h4>
                    <p>Work together seamlessly with real-time updates, comments, and team synchronization.</p>
                    <a href="/features/collaboration" class="feature-link">
                        Learn more <i class="fas fa-arrow-right ms-1"></i>
                    </a>
                </div>
            </div>
        </div>
    </div>
</OsirionBaseSection>

<style>
.section-header-custom {
    max-width: 600px;
    margin: 0 auto;
    margin-bottom: 3rem;
}

.section-badge {
    display: inline-block;
    background: linear-gradient(45deg, #667eea, #764ba2);
    color: white;
    padding: 0.75rem 1.5rem;
    border-radius: 2rem;
    font-weight: 600;
    margin-bottom: 1.5rem;
    box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.section-title-gradient {
    font-size: clamp(2.5rem, 5vw, 4rem);
    font-weight: 900;
    line-height: 1.1;
    margin-bottom: 0;
}

.highlight {
    background: linear-gradient(45deg, #667eea, #764ba2);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
}

.section-description-custom {
    max-width: 700px;
    margin: 0 auto;
}

.feature-badges .badge {
    font-size: 0.875rem;
    padding: 0.5rem 1rem;
    border-radius: 1rem;
}

.feature-grid {
    margin-top: 4rem;
}

.feature-card {
    background: white;
    padding: 2.5rem;
    border-radius: 1.5rem;
    border: 1px solid #e5e7eb;
    transition: all 0.3s ease;
    position: relative;
    overflow: hidden;
}

.feature-card::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 4px;
    background: linear-gradient(45deg, #667eea, #764ba2);
    transform: scaleX(0);
    transition: transform 0.3s ease;
}

.feature-card:hover::before {
    transform: scaleX(1);
}

.feature-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
    border-color: transparent;
}

.feature-icon {
    width: 60px;
    height: 60px;
    border-radius: 50%;
    background: #f8f9fa;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 1.5rem;
    font-size: 1.5rem;
}

.feature-card h4 {
    color: #1f2937;
    margin-bottom: 1rem;
    font-weight: 700;
}

.feature-card p {
    color: #6b7280;
    margin-bottom: 1.5rem;
    line-height: 1.6;
}

.feature-link {
    color: #667eea;
    text-decoration: none;
    font-weight: 600;
    transition: color 0.3s ease;
}

.feature-link:hover {
    color: #764ba2;
}

@media (max-width: 768px) {
    .feature-card {
        padding: 2rem;
    }
    
    .section-header-custom {
        margin-bottom: 2rem;
    }
    
    .feature-grid {
        margin-top: 2rem;
    }
}
</style>
```

### Multi-Framework Container Example

```razor
<!-- Bootstrap -->
<OsirionBaseSection Id="bootstrap-section"
    Title="Bootstrap Section"
    Description="Uses Bootstrap container classes"
    Framework="CssFramework.Bootstrap">
    <div class="row">
        <div class="col-md-6">Bootstrap content</div>
        <div class="col-md-6">Bootstrap content</div>
    </div>
</OsirionBaseSection>

<!-- Fluent UI -->
<OsirionBaseSection Id="fluent-section"
    Title="Fluent UI Section"
    Description="Uses Fluent UI container classes"
    Framework="CssFramework.FluentUI">
    <div class="ms-Grid">
        <div class="ms-Grid-row">
            <div class="ms-Grid-col ms-sm6">Fluent content</div>
            <div class="ms-Grid-col ms-sm6">Fluent content</div>
        </div>
    </div>
</OsirionBaseSection>

<!-- Custom Container -->
<OsirionBaseSection Id="custom-section"
    Title="Custom Container Section"
    Description="Uses custom container class"
    ContainerClass="my-custom-container">
    <div class="custom-grid">
        <div class="custom-item">Custom content</div>
        <div class="custom-item">Custom content</div>
    </div>
</OsirionBaseSection>
```

### Compact Sidebar Section

```razor
<OsirionBaseSection Id="sidebar-section"
    Title="Quick Links"
    Description="Essential navigation"
    Padding="SectionPadding.Small"
    TextAlignment="Alignment.Left"
    BackgroundColor="#f1f5f9"
    HasDivider="true">
    
    <nav class="sidebar-nav">
        <ul class="nav flex-column">
            <li class="nav-item">
                <a class="nav-link" href="/dashboard">
                    <i class="fas fa-tachometer-alt me-2"></i>
                    Dashboard
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/projects">
                    <i class="fas fa-folder me-2"></i>
                    Projects
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/team">
                    <i class="fas fa-users me-2"></i>
                    Team
                </a>
            </li>
            <li class="nav-item">
                <a class="nav-link" href="/settings">
                    <i class="fas fa-cog me-2"></i>
                    Settings
                </a>
            </li>
        </ul>
    </nav>
</OsirionBaseSection>

<style>
.sidebar-nav .nav-link {
    padding: 0.75rem 1rem;
    color: #64748b;
    border-radius: 0.5rem;
    margin-bottom: 0.25rem;
    transition: all 0.2s ease;
}

.sidebar-nav .nav-link:hover {
    background-color: #e2e8f0;
    color: #334155;
}

.sidebar-nav .nav-link.active {
    background-color: #3b82f6;
    color: white;
}
</style>
```

## Styling Examples

### Bootstrap Integration

```razor
<OsirionBaseSection Id="bootstrap-section"
    Title="Bootstrap Styled Section"
    Class="bg-light border-top border-primary border-5"
    Padding="SectionPadding.Large">
    
    <div class="row g-4">
        <div class="col-lg-8">
            <div class="card h-100">
                <div class="card-body">
                    <h5 class="card-title">Main Content</h5>
                    <p class="card-text">Bootstrap-styled content area.</p>
                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="card h-100 bg-primary text-white">
                <div class="card-body">
                    <h5 class="card-title">Sidebar</h5>
                    <p class="card-text">Additional information.</p>
                </div>
            </div>
        </div>
    </div>
</OsirionBaseSection>
```

### Tailwind CSS Integration

```razor
<OsirionBaseSection Id="tailwind-section"
    Title="Tailwind Styled Section"
    Class="bg-gray-50 border-t-4 border-blue-500"
    Padding="SectionPadding.Large">
    
    <div class="grid lg:grid-cols-3 gap-6">
        <div class="bg-white p-6 rounded-lg shadow-sm">
            <h5 class="text-xl font-semibold mb-3">Feature 1</h5>
            <p class="text-gray-600">Tailwind-styled content.</p>
        </div>
        <div class="bg-white p-6 rounded-lg shadow-sm">
            <h5 class="text-xl font-semibold mb-3">Feature 2</h5>
            <p class="text-gray-600">Tailwind-styled content.</p>
        </div>
        <div class="bg-white p-6 rounded-lg shadow-sm">
            <h5 class="text-xl font-semibold mb-3">Feature 3</h5>
            <p class="text-gray-600">Tailwind-styled content.</p>
        </div>
    </div>
</OsirionBaseSection>
```

## Best Practices

### Content Structure

1. **Semantic Markup**: Use proper heading hierarchy and semantic elements
2. **Clear Sections**: Each section should have a clear purpose and focus
3. **Consistent Spacing**: Use the padding system for consistent vertical rhythm
4. **Progressive Enhancement**: Ensure content works without styling
5. **Accessible Navigation**: Provide proper landmarks and navigation cues

### Performance Optimization

1. **Image Optimization**: Optimize background images for web delivery
2. **Lazy Loading**: Consider lazy loading for non-critical background images
3. **CSS Efficiency**: Minimize custom styles and leverage framework classes
4. **Render Performance**: Avoid complex calculations in render fragments
5. **Memory Management**: Properly dispose of heavy resources

### Accessibility

1. **Keyboard Navigation**: Ensure all interactive elements are keyboard accessible
2. **Screen Readers**: Test with screen reader software
3. **Color Contrast**: Maintain sufficient contrast ratios
4. **Focus Management**: Provide clear focus indicators
5. **ARIA Labels**: Use appropriate ARIA attributes when needed

### Design Guidelines

1. **Visual Hierarchy**: Use proper heading levels and spacing
2. **Content Flow**: Structure content logically from top to bottom
3. **Responsive Design**: Test across all device sizes
4. **Brand Consistency**: Maintain consistent styling across sections
5. **User Experience**: Focus on clear messaging and easy navigation

The OsirionBaseSection component provides a solid foundation for building consistent, accessible, and well-structured content sections throughout your application.
