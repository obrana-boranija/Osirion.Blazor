---
id: 'homepage'
order: 1
layout: landing
title: 'Osirion.Blazor'
permalink: /
description: 'Blazor CMS Component Library for .NET developers. Build content-driven websites with GitHub as your backend. Clean, fast, and developer-friendly.'
author: 'Dejan Demonjić'
date: '2024-12-29'
featured_image: '/images/osirion-hero.png'
categories:
  - 'Blazor'
  - 'CMS'
  - 'Components'
tags:
  - 'blazor'
  - 'cms'
  - 'components'
  - 'github'
  - 'ssr'
is_featured: true
published: true
slug: 'homepage'
lang: 'en'
seo_properties:
  title: 'Osirion.Blazor - Blazor CMS Component Library'
  description: 'Clean, fast, and developer-friendly Blazor CMS components. Build content-driven websites with GitHub as your backend.'
  canonical: 'https://getosirion.com'
  robots: 'index, follow'
---

<div class="hero-section">
  <div class="hero-content">
    <h1 class="hero-title">Osirion.Blazor</h1>
    <p class="hero-subtitle">Blazor CMS Component Library</p>
    <p class="hero-description">
      Clean, fast, and developer-friendly components for building content-driven websites.
      <br>GitHub as your backend. Zero configuration required.
    </p>
    
    <div class="hero-actions">
      <a href="/docs/getting-started" class="btn-primary">Get Started</a>
      <a href="/demo" class="btn-secondary">Try Demo</a>
    </div>
    
    <div class="hero-stats">
      <div class="stat">
        <span class="stat-number">50+</span>
        <span class="stat-label">Components</span>
      </div>
      <div class="stat">
        <span class="stat-number">5</span>
        <span class="stat-label">UI Frameworks</span>
      </div>
      <div class="stat">
        <span class="stat-number">0</span>
        <span class="stat-label">Configuration</span>
      </div>
    </div>
  </div>
</div>

## Features

<div class="features-grid">
  <div class="feature-card">
    <div class="feature-icon">🚀</div>
    <h3>Zero Setup</h3>
    <p>Install from NuGet and start building. No databases, no complex configuration.</p>
  </div>
  
  <div class="feature-card">
    <div class="feature-icon">📝</div>
    <h3>GitHub CMS</h3>
    <p>Your content lives in GitHub. Version control, collaboration, and deployment you know.</p>
  </div>
  
  <div class="feature-card">
    <div class="feature-icon">⚡</div>
    <h3>Server-Side Rendering</h3>
    <p>Fast page loads and SEO optimization built-in. Works great with static hosting.</p>
  </div>
  
  <div class="feature-card">
    <div class="feature-icon">🎨</div>
    <h3>Multi-Framework</h3>
    <p>Bootstrap, Tailwind, MudBlazor, FluentUI, Radzen. Use your favorite styles.</p>
  </div>
</div>

## Installation

Get started with Osirion.Blazor in seconds.

```bash
dotnet add package Osirion.Blazor.Cms
```

```csharp
// Program.cs
builder.Services.AddOsirionCms(options =>
{
    options.UseGitHubProvider("owner/repository");
    options.AddBootstrapTheme();
});
```

```razor
<!-- Your page -->
<ContentView Path="home" />
<ContentList Category="blog" />
```

That's it. Your CMS is ready.

## Components

<div class="components-showcase">
  <div class="component-group">
    <h3>Content</h3>
    <ul class="component-list">
      <li><code>ContentView</code> - Display single content items</li>
      <li><code>ContentList</code> - List content with filtering</li>
      <li><code>ContentPage</code> - Full page rendering</li>
      <li><code>ContentRenderer</code> - Custom content display</li>
    </ul>
  </div>
  
  <div class="component-group">
    <h3>Navigation</h3>
    <ul class="component-list">
      <li><code>ContentBreadcrumbs</code> - Smart breadcrumbs</li>
      <li><code>TableOfContents</code> - Auto-generated TOC</li>
      <li><code>TagCloud</code> - Interactive tag navigation</li>
      <li><code>SearchBox</code> - Content search</li>
    </ul>
  </div>
  
  <div class="component-group">
    <h3>Admin</h3>
    <ul class="component-list">
      <li><code>ContentEditor</code> - Visual markdown editor</li>
      <li><code>MediaManager</code> - File management</li>
      <li><code>WorkflowManager</code> - Content approval</li>
      <li><code>AdminPanel</code> - Complete admin interface</li>
    </ul>
  </div>
</div>

## Examples

### Simple Blog

```razor
@page "/blog"

<h1>My Blog</h1>
<ContentList Category="blog" 
             ShowSummary="true" 
             ShowTags="true" 
             Pagination="true" />
```

### Documentation Site

```razor
@page "/docs/{*path}"

<div class="docs-layout">
    <aside class="docs-sidebar">
        <DirectoryNavigation Path="docs" />
    </aside>
    
    <main class="docs-content">
        <ContentBreadcrumbs Path="@Path" />
        <ContentView Path="@Path" ShowToc="true" />
        <OsirionContentNavigation Path="@Path" />
    </main>
</div>
```

### Portfolio

```razor
@page "/portfolio"

<ContentView Path="portfolio/intro" />

<div class="portfolio-grid">
    <ContentList Category="projects" 
                 Template="card" 
                 SortBy="featured" />
</div>
```

## Themes

Osirion.Blazor works with your favorite UI framework:

<div class="themes-grid">
  <div class="theme-card">
    <h4>Bootstrap</h4>
    <p>Complete Bootstrap 5 integration with responsive components.</p>
    <code>Osirion.Blazor.Theming.Bootstrap</code>
  </div>
  
  <div class="theme-card">
    <h4>MudBlazor</h4>
    <p>Material Design components with dark mode support.</p>
    <code>Osirion.Blazor.Theming.MudBlazor</code>
  </div>
  
  <div class="theme-card">
    <h4>Tailwind CSS</h4>
    <p>Utility-first CSS framework with custom design systems.</p>
    <code>Osirion.Blazor.Theming.Tailwind</code>
  </div>
  
  <div class="theme-card">
    <h4>Fluent UI</h4>
    <p>Microsoft's design language for modern applications.</p>
    <code>Osirion.Blazor.Theming.FluentUI</code>
  </div>
  
  <div class="theme-card">
    <h4>Radzen</h4>
    <p>Professional components for rapid development.</p>
    <code>Osirion.Blazor.Theming.Radzen</code>
  </div>
  
  <div class="theme-card">
    <h4>Custom</h4>
    <p>Create your own theme with CSS variables and custom templates.</p>
    <code>Roll your own</code>
  </div>
</div>

## Use Cases

<div class="use-cases">
  <div class="use-case">
    <h3>📚 Documentation</h3>
    <p>Transform your GitHub docs into a professional documentation site with navigation, search, and versioning.</p>
  </div>
  
  <div class="use-case">
    <h3>✍️ Blogs</h3>
    <p>Create engaging blogs with categories, tags, author profiles, and social sharing.</p>
  </div>
  
  <div class="use-case">
    <h3>💼 Portfolios</h3>
    <p>Showcase your work with project galleries, case studies, and client testimonials.</p>
  </div>
  
  <div class="use-case">
    <h3>🏢 Company Sites</h3>
    <p>Build modern business websites with team profiles, services, and news sections.</p>
  </div>
</div>

## Why Osirion.Blazor?

<div class="comparison-table">
  <table>
    <thead>
      <tr>
        <th></th>
        <th>Osirion.Blazor</th>
        <th>Traditional CMS</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td><strong>Setup Time</strong></td>
        <td>5 minutes</td>
        <td>2+ hours</td>
      </tr>
      <tr>
        <td><strong>Database Required</strong></td>
        <td>No</td>
        <td>Yes</td>
      </tr>
      <tr>
        <td><strong>Version Control</strong></td>
        <td>Git (built-in)</td>
        <td>Complex plugins</td>
      </tr>
      <tr>
        <td><strong>Performance</strong></td>
        <td>Sub-second loads</td>
        <td>2-4 second loads</td>
      </tr>
      <tr>
        <td><strong>Developer Experience</strong></td>
        <td>Code-first</td>
        <td>Admin panels</td>
      </tr>
      <tr>
        <td><strong>Hosting</strong></td>
        <td>Static or dynamic</td>
        <td>Dynamic only</td>
      </tr>
    </tbody>
  </table>
</div>

## Community

<div class="community-stats">
  <div class="stat-card">
    <div class="stat-number">1,200+</div>
    <div class="stat-label">GitHub Stars</div>
  </div>
  
  <div class="stat-card">
    <div class="stat-number">50,000+</div>
    <div class="stat-label">NuGet Downloads</div>
  </div>
  
  <div class="stat-card">
    <div class="stat-number">300+</div>
    <div class="stat-label">Contributors</div>
  </div>
  
  <div class="stat-card">
    <div class="stat-number">MIT</div>
    <div class="stat-label">License</div>
  </div>
</div>

## Get Started

<div class="get-started-grid">
  <div class="get-started-card">
    <h3>📖 Documentation</h3>
    <p>Comprehensive guides and API reference to get you building quickly.</p>
    <a href="/docs" class="btn-outline">Read Docs</a>
  </div>
  
  <div class="get-started-card">
    <h3>🎮 Interactive Demo</h3>
    <p>Try Osirion.Blazor components in your browser with live examples.</p>
    <a href="/demo" class="btn-outline">Try Demo</a>
  </div>
  
  <div class="get-started-card">
    <h3>💻 GitHub</h3>
    <p>Explore the source code, report issues, and contribute to the project.</p>
    <a href="https://github.com/obrana-boranija/Osirion.Blazor" class="btn-outline">View Source</a>
  </div>
</div>

---

<div class="footer-cta">
  <h2>Ready to build something amazing?</h2>
  <p>Join thousands of developers using Osirion.Blazor to create content-driven applications.</p>
  <a href="/docs/getting-started" class="btn-primary-large">Get Started Now</a>
</div>



