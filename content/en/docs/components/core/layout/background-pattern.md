---
id: 'osirion-background-pattern'
order: 5
layout: docs
title: OsirionBackgroundPattern Component
permalink: /docs/components/core/layout/background-pattern
description: Learn how to use the OsirionBackgroundPattern component to add beautiful background patterns to enhance your UI design with various pattern types.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Layout
tags:
- blazor
- background
- pattern
- design
- visual
- branding
- ui
is_featured: true
published: true
slug: background-pattern
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionBackgroundPattern Component - Background Patterns | Osirion.Blazor'
  description: 'Add beautiful background patterns with the OsirionBackgroundPattern component. Features multiple pattern types and customizable styling.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/layout/background-pattern'
  lang: en
  robots: index, follow
  og_title: 'OsirionBackgroundPattern Component - Osirion.Blazor'
  og_description: 'Add beautiful background patterns to enhance your UI design with various pattern types.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionBackgroundPattern Component - Osirion.Blazor'
  twitter_description: 'Add beautiful background patterns to enhance your UI design.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionBackgroundPattern Component

The OsirionBackgroundPattern component adds beautiful, subtle background patterns to enhance your UI design. It provides a variety of pattern types from subtle dots to animated grids, perfect for creating visual interest without overwhelming content.

## Component Overview

OsirionBackgroundPattern is designed to add visual depth and professional polish to your layouts. It offers multiple pattern types that can be easily customized and integrated into any design system, making your applications more visually appealing and modern.

### Key Features

**Multiple Pattern Types**: Choose from dots, grids, honeycomb, circuit, and more
**Image Masking**: Optional mask overlay for better content readability
**Professional Design**: Subtle patterns that enhance without distracting
**Performance Optimized**: CSS-based patterns for smooth rendering
**Responsive Ready**: Patterns adapt to different screen sizes
**Framework Agnostic**: Works with any CSS framework
**Easy Integration**: Simple drop-in component for any layout
**Customizable Styling**: Override default styles with custom CSS
**Modern Aesthetics**: Contemporary patterns for professional applications

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `BackgroundPattern` | `BackgroundPatternType?` | `null` | The type of background pattern to display. |
| `MaskImage` | `bool` | `true` | Whether to apply a mask overlay for better content readability. |

## Pattern Types

### Available Pattern Types

| Pattern Type | Description | Use Case |
|--------------|-------------|----------|
| `Dots` | Subtle dots pattern | Professional, clean designs |
| `Grid` | Clean grid lines | Structured, organized layouts |
| `Diagonal` | Dynamic diagonal lines | Modern, energetic designs |
| `Honeycomb` | Unique hexagonal pattern | Creative, unique applications |
| `GradientMesh` | Modern gradient overlay | Subtle, contemporary feel |
| `AnimatedGrid` | Subtle animated grid | Interactive, dynamic content |
| `TechWave` | Tech-inspired wave pattern | Technology, innovation themes |
| `Circuit` | Circuit board pattern | Tech, engineering applications |
| `DotsFade` | Dots with radial fade | Sophisticated, elegant designs |

## Basic Usage

### Simple Pattern Application

```razor
@using Osirion.Blazor.Components

<div class="hero-section position-relative">
    <OsirionBackgroundPattern BackgroundPattern="BackgroundPatternType.Dots" />
    
    <div class="container position-relative" style="z-index: 1;">
        <div class="row justify-content-center">
            <div class="col-lg-8 text-center">
                <h1 class="display-4 fw-bold mb-4">Welcome to Our Platform</h1>
                <p class="lead mb-4">
                    Experience the power of modern web development with our comprehensive Blazor component library.
                </p>
                <div class="d-flex gap-3 justify-content-center">
                    <button class="btn btn-primary btn-lg">Get Started</button>
                    <button class="btn btn-outline-secondary btn-lg">Learn More</button>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
.hero-section {
    min-height: 100vh;
    display: flex;
    align-items: center;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
}
</style>
```

### Grid Pattern with Custom Styling

```razor
<section class="features-section position-relative">
    <OsirionBackgroundPattern BackgroundPattern="BackgroundPatternType.Grid" MaskImage="false" />
    
    <div class="container position-relative" style="z-index: 1;">
        <div class="row">
            <div class="col-12 text-center mb-5">
                <h2 class="display-5 fw-bold">Our Features</h2>
                <p class="lead text-muted">Discover what makes our platform special</p>
            </div>
        </div>
        
        <div class="row g-4">
            <div class="col-md-4">
                <div class="feature-card h-100">
                    <div class="feature-icon mb-3">
                        <i class="fas fa-rocket fa-2x text-primary"></i>
                    </div>
                    <h4>Fast Performance</h4>
                    <p class="text-muted">Optimized for speed and efficiency in every interaction.</p>
                </div>
            </div>
            
            <div class="col-md-4">
                <div class="feature-card h-100">
                    <div class="feature-icon mb-3">
                        <i class="fas fa-shield-alt fa-2x text-success"></i>
                    </div>
                    <h4>Secure by Design</h4>
                    <p class="text-muted">Built with security best practices from the ground up.</p>
                </div>
            </div>
            
            <div class="col-md-4">
                <div class="feature-card h-100">
                    <div class="feature-icon mb-3">
                        <i class="fas fa-cogs fa-2x text-info"></i>
                    </div>
                    <h4>Highly Customizable</h4>
                    <p class="text-muted">Adapt and configure to match your exact requirements.</p>
                </div>
            </div>
        </div>
    </div>
</section>

<style>
.features-section {
    padding: 5rem 0;
    background: #f8f9fa;
    overflow: hidden;
}

.feature-card {
    background: white;
    padding: 2rem;
    border-radius: 1rem;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    text-align: center;
    transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.feature-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.feature-icon {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 80px;
    height: 80px;
    background: rgba(0, 123, 255, 0.1);
    border-radius: 50%;
    margin: 0 auto 1rem;
}
</style>
```

## Advanced Usage

### Multiple Patterns in Different Sections

```razor
@using Osirion.Blazor.Components

<!-- Hero Section with Honeycomb Pattern -->
<section class="hero-section position-relative">
    <OsirionBackgroundPattern BackgroundPattern="BackgroundPatternType.Honeycomb" />
    
    <div class="hero-content">
        <div class="container position-relative" style="z-index: 2;">
            <div class="row align-items-center min-vh-100">
                <div class="col-lg-6">
                    <h1 class="display-3 fw-bold text-white mb-4">
                        Innovation Through Design
                    </h1>
                    <p class="lead text-white-50 mb-4">
                        Creating exceptional digital experiences with cutting-edge technology and beautiful design patterns.
                    </p>
                    <div class="d-flex gap-3">
                        <button class="btn btn-light btn-lg">Explore Now</button>
                        <button class="btn btn-outline-light btn-lg">Watch Demo</button>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="hero-image">
                        <img src="/api/placeholder/500/400" alt="Hero" class="img-fluid rounded-3 shadow-lg" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>

<!-- Tech Section with Circuit Pattern -->
<section class="tech-section position-relative">
    <OsirionBackgroundPattern BackgroundPattern="BackgroundPatternType.Circuit" MaskImage="true" />
    
    <div class="container position-relative" style="z-index: 1;">
        <div class="row">
            <div class="col-12 text-center mb-5">
                <h2 class="display-5 fw-bold">Advanced Technology</h2>
                <p class="lead text-muted">Powered by the latest innovations</p>
            </div>
        </div>
        
        <div class="row g-4">
            <div class="col-lg-3 col-md-6">
                <div class="tech-stat text-center">
                    <div class="stat-number">99.9%</div>
                    <div class="stat-label">Uptime</div>
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="tech-stat text-center">
                    <div class="stat-number">< 100ms</div>
                    <div class="stat-label">Response Time</div>
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="tech-stat text-center">
                    <div class="stat-number">256-bit</div>
                    <div class="stat-label">Encryption</div>
                </div>
            </div>
            <div class="col-lg-3 col-md-6">
                <div class="tech-stat text-center">
                    <div class="stat-number">24/7</div>
                    <div class="stat-label">Support</div>
                </div>
            </div>
        </div>
    </div>
</section>

<!-- Animated Section with TechWave Pattern -->
<section class="animated-section position-relative">
    <OsirionBackgroundPattern BackgroundPattern="BackgroundPatternType.TechWave" />
    
    <div class="container position-relative" style="z-index: 1;">
        <div class="row align-items-center">
            <div class="col-lg-6">
                <div class="content-wrapper">
                    <h2 class="display-5 fw-bold mb-4">Dynamic Solutions</h2>
                    <p class="lead mb-4">
                        Our platform adapts to your needs with intelligent automation and dynamic scaling capabilities.
                    </p>
                    
                    <div class="feature-list">
                        <div class="feature-item d-flex align-items-center mb-3">
                            <div class="feature-icon me-3">
                                <i class="fas fa-check-circle text-success"></i>
                            </div>
                            <span>Automatic scaling based on demand</span>
                        </div>
                        <div class="feature-item d-flex align-items-center mb-3">
                            <div class="feature-icon me-3">
                                <i class="fas fa-check-circle text-success"></i>
                            </div>
                            <span>Real-time performance monitoring</span>
                        </div>
                        <div class="feature-item d-flex align-items-center mb-3">
                            <div class="feature-icon me-3">
                                <i class="fas fa-check-circle text-success"></i>
                            </div>
                            <span>Intelligent resource optimization</span>
                        </div>
                    </div>
                    
                    <button class="btn btn-primary btn-lg mt-4">Learn More</button>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="stats-dashboard">
                    <div class="dashboard-header">
                        <h5>Real-time Metrics</h5>
                    </div>
                    <div class="metrics-grid">
                        <div class="metric-card">
                            <div class="metric-value">1,234</div>
                            <div class="metric-label">Active Users</div>
                            <div class="metric-trend positive">+12%</div>
                        </div>
                        <div class="metric-card">
                            <div class="metric-value">5.6GB</div>
                            <div class="metric-label">Data Processed</div>
                            <div class="metric-trend positive">+8%</div>
                        </div>
                        <div class="metric-card">
                            <div class="metric-value">99.2%</div>
                            <div class="metric-label">Success Rate</div>
                            <div class="metric-trend neutral">0%</div>
                        </div>
                        <div class="metric-card">
                            <div class="metric-value">45ms</div>
                            <div class="metric-label">Avg Response</div>
                            <div class="metric-trend positive">-5%</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>

@code {
    // Component logic if needed
}

<style>
/* Hero Section */
.hero-section {
    background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
    min-height: 100vh;
    overflow: hidden;
}

.hero-content {
    position: relative;
    z-index: 1;
}

.hero-image img {
    transition: transform 0.3s ease;
}

.hero-image:hover img {
    transform: scale(1.05);
}

/* Tech Section */
.tech-section {
    padding: 5rem 0;
    background: #0a0a0a;
    color: white;
}

.tech-stat {
    padding: 2rem;
    background: rgba(255, 255, 255, 0.05);
    border-radius: 1rem;
    border: 1px solid rgba(255, 255, 255, 0.1);
    backdrop-filter: blur(10px);
}

.stat-number {
    font-size: 2.5rem;
    font-weight: bold;
    color: #00d4ff;
    margin-bottom: 0.5rem;
}

.stat-label {
    font-size: 1rem;
    color: rgba(255, 255, 255, 0.7);
    text-transform: uppercase;
    letter-spacing: 1px;
}

/* Animated Section */
.animated-section {
    padding: 5rem 0;
    background: linear-gradient(45deg, #667eea 0%, #764ba2 100%);
    color: white;
}

.content-wrapper {
    padding: 2rem;
}

.feature-item {
    font-size: 1.1rem;
}

.feature-icon {
    width: 24px;
    height: 24px;
    display: flex;
    align-items: center;
    justify-content: center;
}

.stats-dashboard {
    background: rgba(255, 255, 255, 0.1);
    border-radius: 1rem;
    padding: 2rem;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
}

.dashboard-header {
    margin-bottom: 2rem;
    text-align: center;
}

.dashboard-header h5 {
    margin: 0;
    font-weight: 600;
}

.metrics-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
}

.metric-card {
    background: rgba(255, 255, 255, 0.1);
    padding: 1.5rem;
    border-radius: 0.75rem;
    text-align: center;
    border: 1px solid rgba(255, 255, 255, 0.1);
}

.metric-value {
    font-size: 1.5rem;
    font-weight: bold;
    margin-bottom: 0.5rem;
}

.metric-label {
    font-size: 0.875rem;
    opacity: 0.8;
    margin-bottom: 0.5rem;
}

.metric-trend {
    font-size: 0.75rem;
    font-weight: 600;
}

.metric-trend.positive {
    color: #4ade80;
}

.metric-trend.negative {
    color: #f87171;
}

.metric-trend.neutral {
    color: #94a3b8;
}

/* Background Pattern Overrides */
.osirion-bg-wrapper {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    pointer-events: none;
    z-index: 0;
}

.osirion-bg-image-mask::after {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.3);
    z-index: 1;
}

/* Responsive Design */
@media (max-width: 768px) {
    .hero-section .display-3 {
        font-size: 2.5rem;
    }
    
    .metrics-grid {
        grid-template-columns: 1fr;
    }
    
    .stat-number {
        font-size: 2rem;
    }
}
</style>
```

### Interactive Pattern Showcase

```razor
@inject IJSRuntime JSRuntime

<div class="pattern-showcase">
    <div class="showcase-header text-center mb-5">
        <h2 class="display-4 fw-bold">Background Patterns</h2>
        <p class="lead">Experience our collection of beautiful background patterns</p>
    </div>
    
    <div class="pattern-selector mb-4">
        <div class="row justify-content-center">
            <div class="col-md-8">
                <div class="pattern-buttons d-flex flex-wrap justify-content-center gap-2">
                    @foreach (var pattern in availablePatterns)
                    {
                        <button class="btn pattern-btn @(selectedPattern == pattern.Type ? "active" : "")"
                                @onclick="() => SelectPattern(pattern.Type)">
                            @pattern.Name
                        </button>
                    }
                </div>
            </div>
        </div>
    </div>
    
    <div class="pattern-demo position-relative">
        <OsirionBackgroundPattern BackgroundPattern="selectedPattern" MaskImage="maskEnabled" />
        
        <div class="demo-content position-relative" style="z-index: 2;">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-lg-8 text-center">
                        <h3 class="mb-4">Pattern Preview: @GetPatternName(selectedPattern)</h3>
                        <p class="lead mb-4">
                            This is how your content will look with the selected background pattern. 
                            Patterns are designed to be subtle and enhance your content without overwhelming it.
                        </p>
                        
                        <div class="demo-controls">
                            <div class="form-check form-switch d-inline-block me-4">
                                <input class="form-check-input" type="checkbox" id="maskToggle" 
                                       @bind="maskEnabled" @onchange="ToggleMask" />
                                <label class="form-check-label" for="maskToggle">
                                    Enable Mask Overlay
                                </label>
                            </div>
                            
                            <div class="color-scheme-selector d-inline-block">
                                <label class="form-label me-2">Color Scheme:</label>
                                <select @bind="colorScheme" @onchange="ChangeColorScheme" class="form-select d-inline-block w-auto">
                                    <option value="light">Light</option>
                                    <option value="dark">Dark</option>
                                    <option value="brand">Brand Colors</option>
                                </select>
                            </div>
                        </div>
                        
                        <div class="demo-cards mt-5">
                            <div class="row g-4">
                                <div class="col-md-4">
                                    <div class="demo-card">
                                        <div class="card-icon mb-3">
                                            <i class="fas fa-palette fa-2x"></i>
                                        </div>
                                        <h5>Beautiful Design</h5>
                                        <p>Professional patterns that enhance your visual design.</p>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="demo-card">
                                        <div class="card-icon mb-3">
                                            <i class="fas fa-rocket fa-2x"></i>
                                        </div>
                                        <h5>High Performance</h5>
                                        <p>CSS-based patterns for optimal rendering performance.</p>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="demo-card">
                                        <div class="card-icon mb-3">
                                            <i class="fas fa-mobile-alt fa-2x"></i>
                                        </div>
                                        <h5>Responsive Ready</h5>
                                        <p>Patterns that look great on all devices and screen sizes.</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="pattern-info mt-5">
        <div class="container">
            <div class="row">
                <div class="col-12">
                    <div class="info-card">
                        <h4>Implementation Example</h4>
                        <pre><code class="language-razor">@GetImplementationCode()</code></pre>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private BackgroundPatternType selectedPattern = BackgroundPatternType.Dots;
    private bool maskEnabled = true;
    private string colorScheme = "light";
    
    private List<PatternInfo> availablePatterns = new()
    {
        new(BackgroundPatternType.Dots, "Dots"),
        new(BackgroundPatternType.Grid, "Grid"),
        new(BackgroundPatternType.Diagonal, "Diagonal"),
        new(BackgroundPatternType.Honeycomb, "Honeycomb"),
        new(BackgroundPatternType.GradientMesh, "Gradient Mesh"),
        new(BackgroundPatternType.AnimatedGrid, "Animated Grid"),
        new(BackgroundPatternType.TechWave, "Tech Wave"),
        new(BackgroundPatternType.Circuit, "Circuit"),
        new(BackgroundPatternType.DotsFade, "Dots Fade")
    };
    
    private void SelectPattern(BackgroundPatternType pattern)
    {
        selectedPattern = pattern;
        StateHasChanged();
    }
    
    private void ToggleMask()
    {
        StateHasChanged();
    }
    
    private void ChangeColorScheme()
    {
        StateHasChanged();
    }
    
    private string GetPatternName(BackgroundPatternType pattern)
    {
        return availablePatterns.FirstOrDefault(p => p.Type == pattern)?.Name ?? pattern.ToString();
    }
    
    private string GetImplementationCode()
    {
        return $@"<OsirionBackgroundPattern 
    BackgroundPattern=""BackgroundPatternType.{selectedPattern}"" 
    MaskImage=""{maskEnabled.ToString().ToLower()}"" />";
    }
    
    public record PatternInfo(BackgroundPatternType Type, string Name);
}

<style>
.pattern-showcase {
    padding: 4rem 0;
}

.pattern-btn {
    border: 2px solid #e9ecef;
    background: white;
    color: #495057;
    transition: all 0.3s ease;
}

.pattern-btn:hover {
    border-color: #007bff;
    color: #007bff;
}

.pattern-btn.active {
    background: #007bff;
    border-color: #007bff;
    color: white;
}

.pattern-demo {
    min-height: 600px;
    border-radius: 1rem;
    overflow: hidden;
    border: 1px solid #e9ecef;
    background: var(--demo-bg-color, #f8f9fa);
    color: var(--demo-text-color, #495057);
}

.demo-content {
    padding: 4rem 2rem;
}

.demo-controls {
    background: rgba(255, 255, 255, 0.9);
    padding: 1rem 2rem;
    border-radius: 2rem;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
    display: inline-flex;
    align-items: center;
    gap: 2rem;
    flex-wrap: wrap;
    justify-content: center;
}

.demo-card {
    background: rgba(255, 255, 255, 0.9);
    padding: 2rem;
    border-radius: 1rem;
    text-align: center;
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
    transition: transform 0.3s ease;
}

.demo-card:hover {
    transform: translateY(-5px);
}

.card-icon {
    color: #007bff;
}

.info-card {
    background: #f8f9fa;
    padding: 2rem;
    border-radius: 1rem;
    border: 1px solid #e9ecef;
}

.info-card h4 {
    margin-bottom: 1rem;
    color: #495057;
}

.info-card pre {
    background: #ffffff;
    border: 1px solid #e9ecef;
    border-radius: 0.5rem;
    padding: 1rem;
    margin: 0;
}

/* Color Scheme Variants */
[data-color-scheme="light"] .pattern-demo {
    --demo-bg-color: #f8f9fa;
    --demo-text-color: #495057;
}

[data-color-scheme="dark"] .pattern-demo {
    --demo-bg-color: #212529;
    --demo-text-color: #ffffff;
}

[data-color-scheme="brand"] .pattern-demo {
    --demo-bg-color: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    --demo-text-color: #ffffff;
}

@media (max-width: 768px) {
    .pattern-buttons {
        gap: 0.5rem;
    }
    
    .pattern-btn {
        font-size: 0.875rem;
        padding: 0.5rem 1rem;
    }
    
    .demo-controls {
        flex-direction: column;
        gap: 1rem;
    }
    
    .demo-content {
        padding: 2rem 1rem;
    }
}
</style>
```

## Styling Examples

### Bootstrap Integration

```razor
<div class="bg-primary position-relative">
    <OsirionBackgroundPattern BackgroundPattern="BackgroundPatternType.Grid" Class="opacity-25" />
    
    <div class="container py-5 position-relative" style="z-index: 1;">
        <div class="row">
            <div class="col-12 text-center text-white">
                <h2 class="display-4 fw-bold">Bootstrap + Patterns</h2>
                <p class="lead">Beautiful integration with Bootstrap utilities</p>
            </div>
        </div>
    </div>
</div>

<style>
/* Bootstrap-compatible pattern styling */
.osirion-bg-wrapper {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    pointer-events: none;
}

.osirion-bg-dots {
    background-image: radial-gradient(circle, rgba(255, 255, 255, 0.3) 1px, transparent 1px);
    background-size: 20px 20px;
}

.osirion-bg-grid {
    background-image: 
        linear-gradient(rgba(255, 255, 255, 0.1) 1px, transparent 1px),
        linear-gradient(90deg, rgba(255, 255, 255, 0.1) 1px, transparent 1px);
    background-size: 20px 20px;
}
</style>
```

### Tailwind CSS Integration

```razor
<div class="relative bg-gradient-to-br from-blue-600 to-purple-700 text-white">
    <OsirionBackgroundPattern BackgroundPattern="BackgroundPatternType.Honeycomb" />
    
    <div class="container mx-auto py-20 px-4 relative z-10">
        <div class="text-center">
            <h2 class="text-4xl font-bold mb-4">Tailwind + Patterns</h2>
            <p class="text-xl opacity-90">Seamless integration with Tailwind utilities</p>
        </div>
    </div>
</div>

<style>
/* Tailwind-compatible pattern classes */
.osirion-bg-wrapper {
    @apply absolute inset-0 pointer-events-none;
}

.osirion-bg-honeycomb {
    background-image: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.1'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E");
}
</style>
```

## Best Practices

### Design Guidelines

1. **Subtlety is Key**: Keep patterns subtle to avoid overwhelming content
2. **Contrast Awareness**: Ensure sufficient contrast between pattern and text
3. **Consistent Usage**: Use patterns consistently throughout your application
4. **Performance Consideration**: Prefer CSS-based patterns over images
5. **Accessibility**: Ensure patterns don't interfere with screen readers

### Layout Integration

1. **Z-Index Management**: Always set proper z-index values for content layers
2. **Responsive Design**: Test patterns on different screen sizes
3. **Color Harmony**: Choose patterns that complement your color scheme
4. **Mask Usage**: Use masks when content readability is important
5. **Performance**: Avoid overusing animated patterns

### User Experience

1. **Content First**: Patterns should enhance, not distract from content
2. **Brand Alignment**: Choose patterns that match your brand personality
3. **Context Appropriate**: Use tech patterns for tech content, elegant patterns for luxury brands
4. **Loading Performance**: Ensure patterns don't slow down page loading
5. **User Preferences**: Consider providing pattern disable options for accessibility

The OsirionBackgroundPattern component provides a professional and performant way to add visual interest to your layouts while maintaining excellent user experience and accessibility standards.
