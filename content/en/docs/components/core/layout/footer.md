---
id: 'osirion-footer'
order: 3
layout: docs
title: OsirionFooter Component
permalink: /docs/components/core/layout/footer
description: Learn how to use the OsirionFooter component to create comprehensive website footers with links, social media, company info, and responsive layouts.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Layout
tags:
- blazor
- footer
- layout
- navigation
- social-media
- copyright
- responsive
is_featured: true
published: true
slug: components/core/layout/footer
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionFooter Component - Website Footers | Osirion.Blazor'
  description: 'Create comprehensive website footers with the OsirionFooter component. Features navigation links, social media, company info, and responsive layouts.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/layout/footer'
  lang: en
  robots: index, follow
  og_title: 'OsirionFooter Component - Osirion.Blazor'
  og_description: 'Create comprehensive website footers with navigation, social media, and company information.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionFooter Component - Osirion.Blazor'
  twitter_description: 'Create comprehensive website footers with responsive layouts.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionFooter Component

The OsirionFooter component provides a comprehensive solution for creating website footers with navigation links, social media icons, company information, and copyright notices. It supports multiple layouts, responsive design, and accessibility features.

## Component Overview

OsirionFooter is designed to handle all common footer requirements including multi-section navigation, social media links, company branding, legal links, and custom content areas. It provides flexible layout options and maintains consistency across different screen sizes.

### Key Features

**Multi-Section Navigation**: Organize links into logical sections with titles
**Social Media Integration**: Display social media links with icons
**Company Branding**: Include company logo, description, and contact information
**Copyright Management**: Automatic copyright notices with custom options
**Responsive Layout**: Adapts to different screen sizes and orientations
**Accessibility Compliant**: Proper ARIA labels and semantic markup
**Multiple Variants**: Choose from default, minimal, or centered layouts
**Grid Options**: Flexible grid layouts for different content volumes
**Docking Support**: Option to dock footer to bottom of viewport
**Custom Content Areas**: Support for additional content sections

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `CompanyName` | `string` | `"Company"` | Company name displayed in copyright notice. |
| `CompanyUrl` | `string` | `"/"` | URL for the company name link. |
| `Description` | `string?` | `null` | Company or site description shown below the logo. |
| `Copyright` | `string?` | `null` | Custom copyright text. Overrides default format when provided. |
| `ShowCopyright` | `bool` | `true` | Whether to display the copyright notice. |
| `Logo` | `RenderFragment?` | `null` | Logo content to display in the footer. |
| `LeftContent` | `RenderFragment?` | `null` | Additional content for the left section. |
| `RightContent` | `RenderFragment?` | `null` | Additional content for the right section. |
| `Links` | `IReadOnlyList<FooterSection>?` | `null` | Footer navigation link sections. |
| `SocialLinks` | `IReadOnlyList<FooterSocialLink>?` | `null` | Social media links with icons. |
| `BottomLinks` | `IReadOnlyList<FooterLink>?` | `null` | Bottom navigation links (legal, etc.). |
| `ShowSocialLinks` | `bool` | `true` | Whether to display social media links. |
| `SocialTitle` | `string` | `"Follow Us"` | Title for the social links section. |
| `ShowDivider` | `bool` | `true` | Whether to show divider between sections. |
| `ShowTopSection` | `bool` | `true` | Whether to display the main footer section. |
| `ShowBottomSection` | `bool` | `true` | Whether to display the bottom footer section. |
| `Variant` | `string` | `"default"` | Footer variant (default, minimal, centered). |
| `GridLayout` | `string` | `"auto"` | Grid layout (auto, 4-column, 3-column, 2-column). |
| `Docked` | `bool` | `false` | Whether to dock footer to bottom of viewport. |
| `DockingMode` | `string` | `"sticky"` | Docking mode (fixed, sticky). |
| `ChildContent` | `RenderFragment?` | `null` | Additional content shown at the bottom. |

## Basic Usage

### Simple Footer

```razor
@using Osirion.Blazor.Components

<OsirionFooter 
    CompanyName="Your Company"
    CompanyUrl="https://yourcompany.com"
    Description="Building amazing experiences through innovative technology." />
```

### Footer with Navigation Links

```razor
<OsirionFooter 
    CompanyName="TechCorp"
    CompanyUrl="/"
    Description="Innovation through technology"
    Links="@GetFooterLinks()">
    
    <Logo>
        <img src="/logo.svg" alt="TechCorp" height="32" />
    </Logo>
    
</OsirionFooter>

@code {
    private IReadOnlyList<FooterSection> GetFooterLinks()
    {
        return new List<FooterSection>
        {
            new FooterSection
            {
                Title = "Products",
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "Web Development", Href = "/services/web" },
                    new FooterLink { Text = "Mobile Apps", Href = "/services/mobile" },
                    new FooterLink { Text = "Cloud Solutions", Href = "/services/cloud" },
                    new FooterLink { Text = "Consulting", Href = "/services/consulting" }
                }
            },
            new FooterSection
            {
                Title = "Resources",
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "Documentation", Href = "/docs" },
                    new FooterLink { Text = "Blog", Href = "/blog" },
                    new FooterLink { Text = "Community", Href = "/community" },
                    new FooterLink { Text = "Support", Href = "/support" }
                }
            },
            new FooterSection
            {
                Title = "Company",
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "About", Href = "/about" },
                    new FooterLink { Text = "Careers", Href = "/careers" },
                    new FooterLink { Text = "Contact", Href = "/contact" },
                    new FooterLink { Text = "Press", Href = "/press" }
                }
            }
        };
    }
}
```

### Footer with Social Media

```razor
<OsirionFooter 
    CompanyName="Social Company"
    CompanyUrl="/"
    Description="Connect with us on social media"
    SocialLinks="@GetSocialLinks()"
    SocialTitle="Follow Us"
    Links="@GetBasicLinks()">
    
    <Logo>
        <div class="footer-logo">
            <h3>SocialCorp</h3>
        </div>
    </Logo>
    
</OsirionFooter>

@code {
    private IReadOnlyList<FooterSocialLink> GetSocialLinks()
    {
        return new List<FooterSocialLink>
        {
            new FooterSocialLink
            {
                Text = "Twitter",
                Href = "https://twitter.com/company",
                AriaLabel = "Follow us on Twitter",
                Icon = @<svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M23.953 4.57a10 10 0 01-2.825.775 4.958 4.958 0 002.163-2.723c-.951.555-2.005.959-3.127 1.184a4.92 4.92 0 00-8.384 4.482C7.69 8.095 4.067 6.13 1.64 3.162a4.822 4.822 0 00-.666 2.475c0 1.71.87 3.213 2.188 4.096a4.904 4.904 0 01-2.228-.616v.06a4.923 4.923 0 003.946 4.827 4.996 4.996 0 01-2.212.085 4.936 4.936 0 004.604 3.417 9.867 9.867 0 01-6.102 2.105c-.39 0-.779-.023-1.17-.067a13.995 13.995 0 007.557 2.209c9.053 0 13.998-7.496 13.998-13.985 0-.21 0-.42-.015-.63A9.935 9.935 0 0024 4.59z"/>
                </svg>
            },
            new FooterSocialLink
            {
                Text = "LinkedIn",
                Href = "https://linkedin.com/company/yourcompany",
                AriaLabel = "Follow us on LinkedIn",
                Icon = @<svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M20.447 20.452h-3.554v-5.569c0-1.328-.027-3.037-1.852-3.037-1.853 0-2.136 1.445-2.136 2.939v5.667H9.351V9h3.414v1.561h.046c.477-.9 1.637-1.85 3.37-1.85 3.601 0 4.267 2.37 4.267 5.455v6.286zM5.337 7.433c-1.144 0-2.063-.926-2.063-2.065 0-1.138.92-2.063 2.063-2.063 1.14 0 2.064.925 2.064 2.063 0 1.139-.925 2.065-2.064 2.065zm1.782 13.019H3.555V9h3.564v11.452zM22.225 0H1.771C.792 0 0 .774 0 1.729v20.542C0 23.227.792 24 1.771 24h20.451C23.2 24 24 23.227 24 22.271V1.729C24 .774 23.2 0 22.222 0h.003z"/>
                </svg>
            },
            new FooterSocialLink
            {
                Text = "GitHub",
                Href = "https://github.com/yourcompany",
                AriaLabel = "View our GitHub repositories",
                Icon = @<svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/>
                </svg>
            }
        };
    }
    
    private IReadOnlyList<FooterSection> GetBasicLinks()
    {
        return new List<FooterSection>
        {
            new FooterSection
            {
                Title = "Quick Links",
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "Home", Href = "/" },
                    new FooterLink { Text = "About", Href = "/about" },
                    new FooterLink { Text = "Services", Href = "/services" },
                    new FooterLink { Text = "Contact", Href = "/contact" }
                }
            }
        };
    }
}
```

## Advanced Usage

### Comprehensive Footer with All Features

```razor
<OsirionFooter 
    CompanyName="Enterprise Solutions"
    CompanyUrl="https://enterprise.com"
    Description="Empowering businesses with cutting-edge technology solutions since 2010."
    Links="@GetComprehensiveLinks()"
    SocialLinks="@GetComprehensiveSocialLinks()"
    BottomLinks="@GetLegalLinks()"
    Variant="default"
    GridLayout="4-column">
    
    <Logo>
        <div class="enterprise-logo">
            <img src="/enterprise-logo.svg" alt="Enterprise Solutions" height="40" />
            <span class="logo-text">Enterprise Solutions</span>
        </div>
    </Logo>
    
    <LeftContent>
        <div class="newsletter-signup">
            <h4>Stay Updated</h4>
            <p>Subscribe to our newsletter for the latest updates and insights.</p>
            <form class="newsletter-form" @onsubmit="SubscribeToNewsletter" @onsubmit:preventDefault="true">
                <div class="input-group">
                    <input type="email" @bind="emailAddress" placeholder="Enter your email" class="form-control" />
                    <button type="submit" class="btn btn-primary">Subscribe</button>
                </div>
            </form>
        </div>
    </LeftContent>
    
    <RightContent>
        <div class="contact-info">
            <h4>Contact Information</h4>
            <address>
                <strong>Enterprise Solutions</strong><br>
                123 Business Street<br>
                Technology City, TC 12345<br>
                <strong>Phone:</strong> +1 (555) 123-4567<br>
                <strong>Email:</strong> info@enterprise.com
            </address>
        </div>
    </RightContent>
    
</OsirionFooter>

@code {
    private string emailAddress = "";
    
    private async Task SubscribeToNewsletter()
    {
        // Handle newsletter subscription
        if (!string.IsNullOrWhiteSpace(emailAddress))
        {
            // Add your subscription logic here
            emailAddress = "";
            // Show success message
        }
    }
    
    private IReadOnlyList<FooterSection> GetComprehensiveLinks()
    {
        return new List<FooterSection>
        {
            new FooterSection
            {
                Title = "Solutions",
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "Cloud Infrastructure", Href = "/solutions/cloud" },
                    new FooterLink { Text = "Data Analytics", Href = "/solutions/analytics" },
                    new FooterLink { Text = "Digital Transformation", Href = "/solutions/digital" },
                    new FooterLink { Text = "Cybersecurity", Href = "/solutions/security" },
                    new FooterLink { Text = "AI & Machine Learning", Href = "/solutions/ai" }
                }
            },
            new FooterSection
            {
                Title = "Industries",
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "Healthcare", Href = "/industries/healthcare" },
                    new FooterLink { Text = "Financial Services", Href = "/industries/finance" },
                    new FooterLink { Text = "Manufacturing", Href = "/industries/manufacturing" },
                    new FooterLink { Text = "Retail", Href = "/industries/retail" },
                    new FooterLink { Text = "Education", Href = "/industries/education" }
                }
            },
            new FooterSection
            {
                Title = "Resources",
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "Documentation", Href = "/resources/docs" },
                    new FooterLink { Text = "Case Studies", Href = "/resources/case-studies" },
                    new FooterLink { Text = "Whitepapers", Href = "/resources/whitepapers" },
                    new FooterLink { Text = "Webinars", Href = "/resources/webinars" },
                    new FooterLink { Text = "Training", Href = "/resources/training" }
                }
            },
            new FooterSection
            {
                Title = "Company",
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "About Us", Href = "/company/about" },
                    new FooterLink { Text = "Leadership", Href = "/company/leadership" },
                    new FooterLink { Text = "Careers", Href = "/company/careers" },
                    new FooterLink { Text = "News & Press", Href = "/company/news" },
                    new FooterLink { Text = "Investor Relations", Href = "/company/investors" }
                }
            }
        };
    }
    
    private IReadOnlyList<FooterSocialLink> GetComprehensiveSocialLinks()
    {
        return new List<FooterSocialLink>
        {
            new FooterSocialLink
            {
                Text = "LinkedIn",
                Href = "https://linkedin.com/company/enterprise-solutions",
                AriaLabel = "Follow Enterprise Solutions on LinkedIn"
                // Icon implementation here
            },
            new FooterSocialLink
            {
                Text = "Twitter",
                Href = "https://twitter.com/enterprise_sol",
                AriaLabel = "Follow us on Twitter"
                // Icon implementation here
            },
            new FooterSocialLink
            {
                Text = "YouTube",
                Href = "https://youtube.com/c/enterprise-solutions",
                AriaLabel = "Watch our videos on YouTube"
                // Icon implementation here
            },
            new FooterSocialLink
            {
                Text = "GitHub",
                Href = "https://github.com/enterprise-solutions",
                AriaLabel = "View our open source projects"
                // Icon implementation here
            }
        };
    }
    
    private IReadOnlyList<FooterLink> GetLegalLinks()
    {
        return new List<FooterLink>
        {
            new FooterLink { Text = "Privacy Policy", Href = "/legal/privacy" },
            new FooterLink { Text = "Terms of Service", Href = "/legal/terms" },
            new FooterLink { Text = "Cookie Policy", Href = "/legal/cookies" },
            new FooterLink { Text = "Accessibility", Href = "/legal/accessibility" }
        };
    }
}

<style>
.enterprise-logo {
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.logo-text {
    font-weight: 600;
    font-size: 1.25rem;
    color: #495057;
}

.newsletter-signup {
    max-width: 300px;
}

.newsletter-signup h4 {
    font-size: 1.125rem;
    margin-bottom: 0.5rem;
    color: #495057;
}

.newsletter-signup p {
    font-size: 0.875rem;
    color: #6c757d;
    margin-bottom: 1rem;
}

.newsletter-form .input-group {
    display: flex;
    gap: 0.5rem;
}

.newsletter-form .form-control {
    flex: 1;
    padding: 0.5rem;
    border: 1px solid #ced4da;
    border-radius: 0.25rem;
}

.newsletter-form .btn {
    padding: 0.5rem 1rem;
    background: #007bff;
    color: white;
    border: 1px solid #007bff;
    border-radius: 0.25rem;
    cursor: pointer;
}

.newsletter-form .btn:hover {
    background: #0056b3;
    border-color: #0056b3;
}

.contact-info h4 {
    font-size: 1.125rem;
    margin-bottom: 1rem;
    color: #495057;
}

.contact-info address {
    font-style: normal;
    line-height: 1.6;
    color: #6c757d;
}

.contact-info strong {
    color: #495057;
}
</style>
```

### Minimal Footer Variant

```razor
<OsirionFooter 
    CompanyName="MinimalCorp"
    CompanyUrl="/"
    Variant="minimal"
    ShowDivider="false"
    Links="@GetMinimalLinks()"
    BottomLinks="@GetMinimalBottomLinks()">
    
    <Logo>
        <span class="minimal-logo">MinimalCorp</span>
    </Logo>
    
</OsirionFooter>

@code {
    private IReadOnlyList<FooterSection> GetMinimalLinks()
    {
        return new List<FooterSection>
        {
            new FooterSection
            {
                Links = new List<FooterLink>
                {
                    new FooterLink { Text = "About", Href = "/about" },
                    new FooterLink { Text = "Services", Href = "/services" },
                    new FooterLink { Text = "Contact", Href = "/contact" },
                    new FooterLink { Text = "Blog", Href = "/blog" }
                }
            }
        };
    }
    
    private IReadOnlyList<FooterLink> GetMinimalBottomLinks()
    {
        return new List<FooterLink>
        {
            new FooterLink { Text = "Privacy", Href = "/privacy" },
            new FooterLink { Text = "Terms", Href = "/terms" }
        };
    }
}

<style>
.minimal-logo {
    font-weight: 700;
    font-size: 1.5rem;
    color: #495057;
}
</style>
```

### Docked Footer

```razor
<OsirionFooter 
    CompanyName="StickyFooter Inc."
    CompanyUrl="/"
    Description="Always at the bottom of your screen"
    Docked="true"
    DockingMode="sticky"
    Variant="minimal"
    ShowTopSection="false"
    BottomLinks="@GetQuickLinks()">
    
    <Logo>
        <span class="docked-logo">📌 StickyFooter</span>
    </Logo>
    
</OsirionFooter>

@code {
    private IReadOnlyList<FooterLink> GetQuickLinks()
    {
        return new List<FooterLink>
        {
            new FooterLink { Text = "Home", Href = "/" },
            new FooterLink { Text = "About", Href = "/about" },
            new FooterLink { Text = "Contact", Href = "/contact" }
        };
    }
}

<style>
.docked-logo {
    font-weight: 600;
    font-size: 1rem;
}

/* Ensure main content has proper spacing when footer is docked */
.osirion-footer-docked-sticky ~ * {
    padding-bottom: 80px; /* Adjust based on footer height */
}
</style>
```

## Styling Examples

### Bootstrap Integration

```razor
<OsirionFooter 
    CompanyName="Bootstrap Corp"
    CompanyUrl="/"
    Class="bg-dark text-light"
    Links="@links"
    SocialLinks="@socialLinks">
    
    <Logo>
        <img src="/logo-white.svg" alt="Bootstrap Corp" height="32" />
    </Logo>
    
</OsirionFooter>

<style>
/* Bootstrap-compatible styling */
.osirion-footer.bg-dark {
    background: #212529;
    color: #ffffff;
}

.osirion-footer-container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 3rem 1rem 1.5rem;
}

.osirion-footer-main {
    display: grid;
    grid-template-columns: 1fr 2fr 1fr;
    gap: 2rem;
    margin-bottom: 2rem;
}

.osirion-footer-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 2rem;
}

.osirion-footer-section-title {
    color: #ffffff;
    font-weight: 600;
    font-size: 1.125rem;
    margin-bottom: 1rem;
}

.osirion-footer-list {
    list-style: none;
    padding: 0;
    margin: 0;
}

.osirion-footer-list-item {
    margin-bottom: 0.5rem;
}

.osirion-footer-link {
    color: #adb5bd;
    text-decoration: none;
    transition: color 0.2s;
}

.osirion-footer-link:hover {
    color: #ffffff;
    text-decoration: none;
}

.osirion-footer-social-links {
    display: flex;
    gap: 1rem;
}

.osirion-footer-social-link {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 40px;
    height: 40px;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 0.25rem;
    color: #adb5bd;
    text-decoration: none;
    transition: all 0.2s;
}

.osirion-footer-social-link:hover {
    background: rgba(255, 255, 255, 0.2);
    color: #ffffff;
    transform: translateY(-2px);
}

.osirion-footer-divider {
    height: 1px;
    background: rgba(255, 255, 255, 0.1);
    margin: 2rem 0;
}

.osirion-footer-bottom {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    gap: 1rem;
}

.osirion-footer-copyright {
    color: #adb5bd;
    font-size: 0.875rem;
}

.osirion-footer-bottom-link {
    color: #adb5bd;
    text-decoration: none;
    font-size: 0.875rem;
}

.osirion-footer-bottom-link:hover {
    color: #ffffff;
}

@media (max-width: 768px) {
    .osirion-footer-main {
        grid-template-columns: 1fr;
        gap: 1.5rem;
    }
    
    .osirion-footer-grid {
        grid-template-columns: 1fr;
        gap: 1.5rem;
    }
    
    .osirion-footer-bottom {
        flex-direction: column;
        text-align: center;
    }
}
</style>
```

### Tailwind CSS Integration

```razor
<OsirionFooter 
    CompanyName="Tailwind Corp"
    CompanyUrl="/"
    Class="bg-gray-900 text-gray-300"
    Links="@links"
    SocialLinks="@socialLinks" />

<style>
/* Tailwind-compatible classes */
.osirion-footer {
    @apply bg-gray-900 text-gray-300;
}

.osirion-footer-container {
    @apply max-w-7xl mx-auto px-4 py-12;
}

.osirion-footer-main {
    @apply grid grid-cols-1 lg:grid-cols-3 gap-8 mb-8;
}

.osirion-footer-grid {
    @apply grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8;
}

.osirion-footer-section-title {
    @apply text-white font-semibold text-lg mb-4;
}

.osirion-footer-list {
    @apply list-none p-0 m-0 space-y-2;
}

.osirion-footer-link {
    @apply text-gray-400 hover:text-white transition-colors duration-200 no-underline;
}

.osirion-footer-social-links {
    @apply flex space-x-4;
}

.osirion-footer-social-link {
    @apply flex items-center justify-center w-10 h-10 bg-gray-800 rounded text-gray-400 hover:text-white hover:bg-gray-700 transition-all duration-200 transform hover:-translate-y-1;
}

.osirion-footer-divider {
    @apply h-px bg-gray-800 my-8;
}

.osirion-footer-bottom {
    @apply flex flex-col md:flex-row justify-between items-center space-y-4 md:space-y-0;
}

.osirion-footer-copyright {
    @apply text-gray-400 text-sm;
}

.osirion-footer-bottom-link {
    @apply text-gray-400 hover:text-white text-sm transition-colors;
}
</style>
```

## Best Practices

### Content Organization

1. **Logical Grouping**: Organize links into logical sections that match your site structure
2. **Priority Placement**: Place most important links in prominent positions
3. **Social Media**: Include relevant social media platforms for your audience
4. **Legal Compliance**: Always include required legal links (Privacy Policy, Terms)
5. **Contact Information**: Provide clear ways for users to contact you

### Accessibility Guidelines

1. **Semantic Markup**: Use proper HTML5 semantic elements
2. **ARIA Labels**: Provide meaningful ARIA labels for social media links
3. **Focus Management**: Ensure proper focus order and visual focus indicators
4. **Color Contrast**: Maintain sufficient color contrast for text and backgrounds
5. **Screen Readers**: Test footer navigation with screen reader software

### Performance Optimization

1. **Image Optimization**: Optimize logo and social media icons for web
2. **Lazy Loading**: Consider lazy loading for non-critical footer content
3. **Minimal JavaScript**: Keep JavaScript usage minimal in footer components
4. **CSS Efficiency**: Use efficient CSS selectors and avoid redundant styles
5. **Mobile Performance**: Optimize for mobile devices and slower connections

The OsirionFooter component provides a comprehensive solution for creating professional, accessible, and responsive website footers that enhance user experience and site navigation.
