---
id: 'osirion-testimonial-card'
order: 2
layout: docs
title: OsirionTestimonialCard Component
permalink: /docs/components/core/cards/testimonial-card
description: Learn how to use the OsirionTestimonialCard component to display customer testimonials, reviews, and recommendations with professional layouts.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Cards
tags:
- blazor
- testimonials
- reviews
- cards
- rating
- social-proof
- user-interface
is_featured: true
published: true
slug: components/core/cards/testimonial-card
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionTestimonialCard Component - Customer Testimonials | Osirion.Blazor'
  description: 'Display customer testimonials and reviews with the OsirionTestimonialCard component. Features ratings, profile images, LinkedIn integration, and responsive design.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/cards/testimonial-card'
  lang: en
  robots: index, follow
  og_title: 'OsirionTestimonialCard Component - Osirion.Blazor'
  og_description: 'Create compelling customer testimonials with ratings, profile images, and LinkedIn integration.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionTestimonialCard Component - Osirion.Blazor'
  twitter_description: 'Create compelling customer testimonials with professional layouts.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionTestimonialCard Component

The OsirionTestimonialCard component provides a professional way to display customer testimonials, reviews, and recommendations. It features rating systems, profile images, LinkedIn integration, and multiple visual variants to match your design needs.

## Component Overview

OsirionTestimonialCard helps you showcase social proof through customer testimonials. It supports star ratings, profile images with optimized loading, LinkedIn profile links, and various styling options to create compelling testimonial displays.

### Key Features

**Rating System**: Display 1-5 star ratings with accessible ARIA labels
**Profile Integration**: Include customer photos with performance optimization
**LinkedIn Connectivity**: Link testimonials to LinkedIn profiles
**Multiple Variants**: Choose from default, minimal, highlighted, or compact styles
**Responsive Design**: Adapts to different screen sizes and layouts
**Accessibility Compliant**: Full ARIA support and semantic markup
**Performance Optimized**: LCP optimization for first visible testimonials
**Flexible Content**: Support for both simple text and rich content
**Read More Links**: Optional links to full testimonials or case studies
**Framework Agnostic**: Compatible with Bootstrap, Tailwind, and other CSS frameworks

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Name` | `string` (Required) | `""` | The name of the person giving the testimonial. |
| `TestimonialText` | `string?` | `null` | The testimonial text content. |
| `TestimonialContent` | `RenderFragment?` | `null` | Custom testimonial content. Overrides TestimonialText when provided. |
| `ProfileImageUrl` | `string?` | `null` | URL for the person's profile image. |
| `Company` | `string?` | `null` | The person's company name. |
| `Position` | `string?` | `null` | The person's job title or position. |
| `LinkedInUrl` | `string?` | `null` | LinkedIn profile URL for the person. |
| `ShowRating` | `bool` | `false` | Whether to display star ratings. |
| `Rating` | `int` | `5` | Rating value from 1 to 5 stars. |
| `Variant` | `TestimonialVariant` | `Default` | Visual variant (Default, Minimal, Highlighted, Compact). |
| `Size` | `TestimonialSize` | `Normal` | Card size (Small, Normal, Large). |
| `ImageSize` | `int` | `64` | Profile image size in pixels. |
| `Elevated` | `bool` | `true` | Whether the card should have elevated appearance. |
| `Borderless` | `bool` | `false` | Whether the card should be borderless. |
| `ReadMoreHref` | `string?` | `null` | URL for the read more link. |
| `ReadMoreText` | `string` | `"Read full testimonial"` | Text for the read more link. |
| `ReadMoreVariant` | `ReadMoreVariant` | `Default` | Variant for the read more link. |
| `ReadMoreTarget` | `string?` | `null` | Target for the read more link. |
| `IsFirstVisible` | `bool` | `false` | Whether this is the first visible testimonial (for LCP optimization). |
| `IsHiddenFromAssistiveTech` | `bool` | `false` | Whether the card is hidden from assistive technology. |
| `ChildContent` | `RenderFragment?` | `null` | Additional content to display in the card. |

## Basic Usage

### Simple Testimonial

```razor
@using Osirion.Blazor.Components

<OsirionTestimonialCard 
    Name="Sarah Johnson"
    TestimonialText="This product has completely transformed how we handle our customer communications. The interface is intuitive and the results are outstanding!"
    Company="TechCorp Solutions"
    Position="Marketing Director" />
```

### Testimonial with Rating

```razor
<OsirionTestimonialCard 
    Name="Michael Chen"
    TestimonialText="Exceptional service and support. The team went above and beyond to ensure our success."
    Company="Innovation Labs"
    Position="CTO"
    ShowRating="true"
    Rating="5"
    ProfileImageUrl="/images/testimonials/michael-chen.jpg" />
```

### Testimonial with LinkedIn Integration

```razor
<OsirionTestimonialCard 
    Name="Emma Rodriguez"
    TestimonialText="Working with this team has been a game-changer for our business. Highly recommended!"
    Company="Global Enterprises"
    Position="VP of Operations"
    LinkedInUrl="https://linkedin.com/in/emmarodriguez"
    ProfileImageUrl="/images/testimonials/emma-rodriguez.jpg"
    ShowRating="true"
    Rating="5"
    IsFirstVisible="true" />
```

## Advanced Usage

### Rich Content Testimonial

```razor
<OsirionTestimonialCard 
    Name="David Park"
    Company="StartupX"
    Position="Founder & CEO"
    LinkedInUrl="https://linkedin.com/in/davidpark"
    ProfileImageUrl="/images/testimonials/david-park.jpg"
    ShowRating="true"
    Rating="5"
    Variant="TestimonialVariant.Highlighted">
    
    <TestimonialContent>
        <p class="lead">
            "The results speak for themselves - <strong>300% increase in conversions</strong> 
            within the first quarter of implementation."
        </p>
        <p>
            The platform's intuitive design and powerful analytics have given us insights 
            we never had before. The support team is incredibly responsive and knowledgeable.
        </p>
        <blockquote class="mt-3">
            <em>"This is exactly what we needed to scale our operations efficiently."</em>
        </blockquote>
    </TestimonialContent>
    
</OsirionTestimonialCard>
```

### Testimonial Grid Layout

```razor
<div class="row g-4">
    <div class="col-md-4">
        <OsirionTestimonialCard 
            Name="Alex Thompson"
            TestimonialText="Outstanding platform with incredible support. Our productivity has increased by 40%."
            Company="Digital Solutions"
            Position="Project Manager"
            ShowRating="true"
            Rating="5"
            Variant="TestimonialVariant.Minimal"
            Size="TestimonialSize.Small" />
    </div>
    
    <div class="col-md-4">
        <OsirionTestimonialCard 
            Name="Maria Santos"
            TestimonialText="The best investment we've made for our business. ROI was visible within weeks."
            Company="Growth Partners"
            Position="Business Analyst"
            ShowRating="true"
            Rating="5"
            Variant="TestimonialVariant.Default"
            ProfileImageUrl="/images/testimonials/maria-santos.jpg" />
    </div>
    
    <div class="col-md-4">
        <OsirionTestimonialCard 
            Name="James Wilson"
            TestimonialText="Exceptional customer service and a product that delivers on all its promises."
            Company="Enterprise Corp"
            Position="Operations Director"
            ShowRating="true"
            Rating="4"
            Variant="TestimonialVariant.Compact"
            LinkedInUrl="https://linkedin.com/in/jameswilson" />
    </div>
</div>
```

### Testimonial with Read More Link

```razor
<OsirionTestimonialCard 
    Name="Lisa Anderson"
    TestimonialText="This solution has revolutionized our workflow. The automation features alone have saved us countless hours each week."
    Company="Efficiency Experts"
    Position="Operations Manager"
    ProfileImageUrl="/images/testimonials/lisa-anderson.jpg"
    ShowRating="true"
    Rating="5"
    ReadMoreHref="/case-studies/efficiency-experts"
    ReadMoreText="Read full case study"
    ReadMoreVariant="ReadMoreVariant.Primary"
    ReadMoreTarget="_blank" />
```

### Interactive Testimonial Carousel

```razor
@inject IJSRuntime JSRuntime

<div class="testimonial-carousel" @ref="carouselElement">
    <div class="carousel-container">
        @foreach (var (testimonial, index) in testimonials.Select((t, i) => (t, i)))
        {
            <div class="carousel-slide @(currentSlide == index ? "active" : "")" 
                 style="transform: translateX(@((index - currentSlide) * 100)%)">
                
                <OsirionTestimonialCard 
                    Name="@testimonial.Name"
                    TestimonialText="@testimonial.Text"
                    Company="@testimonial.Company"
                    Position="@testimonial.Position"
                    ProfileImageUrl="@testimonial.ImageUrl"
                    LinkedInUrl="@testimonial.LinkedInUrl"
                    ShowRating="true"
                    Rating="@testimonial.Rating"
                    Variant="TestimonialVariant.Highlighted"
                    Size="TestimonialSize.Large"
                    IsFirstVisible="@(index == 0)"
                    IsHiddenFromAssistiveTech="@(index != currentSlide)" />
                    
            </div>
        }
    </div>
    
    <div class="carousel-controls">
        <button class="carousel-btn carousel-prev" 
                @onclick="PreviousSlide"
                disabled="@(currentSlide == 0)">
            ‹ Previous
        </button>
        
        <div class="carousel-indicators">
            @for (int i = 0; i < testimonials.Count; i++)
            {
                int slideIndex = i;
                <button class="carousel-indicator @(currentSlide == i ? "active" : "")"
                        @onclick="() => GoToSlide(slideIndex)"
                        aria-label="Go to slide @(i + 1)"></button>
            }
        </div>
        
        <button class="carousel-btn carousel-next" 
                @onclick="NextSlide"
                disabled="@(currentSlide == testimonials.Count - 1)">
            Next ›
        </button>
    </div>
</div>

@code {
    private ElementReference carouselElement;
    private int currentSlide = 0;
    private System.Threading.Timer? autoSlideTimer;
    
    private List<TestimonialData> testimonials = new()
    {
        new("Sarah Johnson", "This platform has transformed our business operations completely.", "TechCorp", "CEO", "/images/sarah.jpg", "https://linkedin.com/in/sarahjohnson", 5),
        new("Michael Chen", "Outstanding customer support and incredible results.", "Innovation Labs", "CTO", "/images/michael.jpg", "https://linkedin.com/in/michaelchen", 5),
        new("Emma Rodriguez", "The best investment we've made for our company.", "Global Enterprises", "VP Operations", "/images/emma.jpg", "https://linkedin.com/in/emmarodriguez", 5)
    };
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            StartAutoSlide();
        }
    }
    
    private void StartAutoSlide()
    {
        autoSlideTimer = new System.Threading.Timer(async _ =>
        {
            await InvokeAsync(() =>
            {
                NextSlide();
                StateHasChanged();
            });
        }, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }
    
    private void NextSlide()
    {
        currentSlide = (currentSlide + 1) % testimonials.Count;
    }
    
    private void PreviousSlide()
    {
        currentSlide = currentSlide == 0 ? testimonials.Count - 1 : currentSlide - 1;
    }
    
    private void GoToSlide(int index)
    {
        currentSlide = index;
        ResetAutoSlide();
    }
    
    private void ResetAutoSlide()
    {
        autoSlideTimer?.Dispose();
        StartAutoSlide();
    }
    
    public void Dispose()
    {
        autoSlideTimer?.Dispose();
    }
    
    public record TestimonialData(string Name, string Text, string Company, string Position, string ImageUrl, string LinkedInUrl, int Rating);
}

<style>
.testimonial-carousel {
    position: relative;
    max-width: 800px;
    margin: 0 auto;
    overflow: hidden;
}

.carousel-container {
    position: relative;
    width: 100%;
    min-height: 300px;
}

.carousel-slide {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    transition: transform 0.5s ease-in-out;
    opacity: 0;
}

.carousel-slide.active {
    opacity: 1;
}

.carousel-controls {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 2rem;
    padding: 0 1rem;
}

.carousel-btn {
    background: #007bff;
    color: white;
    border: none;
    padding: 0.5rem 1rem;
    border-radius: 0.25rem;
    cursor: pointer;
    transition: background-color 0.2s;
}

.carousel-btn:hover:not(:disabled) {
    background: #0056b3;
}

.carousel-btn:disabled {
    background: #6c757d;
    cursor: not-allowed;
}

.carousel-indicators {
    display: flex;
    gap: 0.5rem;
}

.carousel-indicator {
    width: 12px;
    height: 12px;
    border-radius: 50%;
    border: none;
    background: #dee2e6;
    cursor: pointer;
    transition: background-color 0.2s;
}

.carousel-indicator.active {
    background: #007bff;
}

.carousel-indicator:hover {
    background: #adb5bd;
}

@media (max-width: 768px) {
    .carousel-controls {
        flex-direction: column;
        gap: 1rem;
    }
    
    .carousel-btn {
        padding: 0.375rem 0.75rem;
        font-size: 0.875rem;
    }
}
</style>
```

### Testimonial Wall with Masonry Layout

```razor
<div class="testimonial-wall">
    @foreach (var testimonial in GetTestimonialsByImportance())
    {
        <div class="testimonial-item @GetTestimonialItemClass(testimonial)">
            <OsirionTestimonialCard 
                Name="@testimonial.Name"
                TestimonialText="@testimonial.Text"
                Company="@testimonial.Company"
                Position="@testimonial.Position"
                ProfileImageUrl="@testimonial.ImageUrl"
                LinkedInUrl="@testimonial.LinkedInUrl"
                ShowRating="@testimonial.ShowRating"
                Rating="@testimonial.Rating"
                Variant="@testimonial.Variant"
                Size="@testimonial.Size"
                Elevated="@testimonial.IsHighlighted"
                ReadMoreHref="@testimonial.ReadMoreUrl"
                ReadMoreText="@testimonial.ReadMoreText" />
        </div>
    }
</div>

@code {
    private List<EnhancedTestimonialData> GetTestimonialsByImportance()
    {
        return new List<EnhancedTestimonialData>
        {
            new("John Smith", "This platform increased our efficiency by 200%. Absolutely incredible!", 
                "Fortune 500 Corp", "Chief Technology Officer", "/images/john-smith.jpg", 
                "https://linkedin.com/in/johnsmith", 5, true, TestimonialVariant.Highlighted, 
                TestimonialSize.Large, "/case-studies/fortune-500", "Read case study"),
                
            new("Anna Williams", "Best customer service I've experienced in 20 years of business.", 
                "Local Business Inc", "Owner", "/images/anna-williams.jpg", 
                null, 5, true, TestimonialVariant.Default, TestimonialSize.Normal, null, null),
                
            new("Robert Davis", "Simple, effective, and powerful. Everything we needed.", 
                "StartupCo", "Founder", "/images/robert-davis.jpg", 
                "https://linkedin.com/in/robertdavis", 4, true, TestimonialVariant.Minimal, 
                TestimonialSize.Small, null, null),
                
            new("Jennifer Brown", "The automation features alone justify the investment.", 
                "Enterprise Solutions", "Operations Manager", "/images/jennifer-brown.jpg", 
                "https://linkedin.com/in/jenniferbrown", 5, true, TestimonialVariant.Compact, 
                TestimonialSize.Normal, "/testimonials/jennifer-brown", "Full testimonial"),
                
            new("Mark Johnson", "Responsive support team and a product that actually works as advertised.", 
                "Tech Innovations", "Product Manager", "/images/mark-johnson.jpg", 
                null, 5, true, TestimonialVariant.Default, TestimonialSize.Small, null, null)
        };
    }
    
    private string GetTestimonialItemClass(EnhancedTestimonialData testimonial)
    {
        var classes = new List<string> { "testimonial-item" };
        
        if (testimonial.IsHighlighted)
        {
            classes.Add("testimonial-highlighted");
        }
        
        if (testimonial.Size == TestimonialSize.Large)
        {
            classes.Add("testimonial-large");
        }
        else if (testimonial.Size == TestimonialSize.Small)
        {
            classes.Add("testimonial-small");
        }
        
        return string.Join(" ", classes);
    }
    
    public record EnhancedTestimonialData(
        string Name, string Text, string Company, string Position, string ImageUrl, 
        string? LinkedInUrl, int Rating, bool ShowRating, TestimonialVariant Variant, 
        TestimonialSize Size, string? ReadMoreUrl, string? ReadMoreText)
    {
        public bool IsHighlighted => Variant == TestimonialVariant.Highlighted;
    }
}

<style>
.testimonial-wall {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 1.5rem;
    margin: 2rem 0;
}

.testimonial-item {
    break-inside: avoid;
    page-break-inside: avoid;
}

.testimonial-highlighted {
    grid-row-end: span 2;
}

.testimonial-large {
    grid-column-end: span 2;
}

.testimonial-small {
    align-self: start;
}

@media (max-width: 768px) {
    .testimonial-wall {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .testimonial-large {
        grid-column-end: span 1;
    }
    
    .testimonial-highlighted {
        grid-row-end: span 1;
    }
}

@supports (display: subgrid) {
    .testimonial-wall {
        display: grid;
        grid-template-rows: masonry;
    }
}
</style>
```

## Styling Examples

### Bootstrap Integration

```razor
<OsirionTestimonialCard 
    Name="Customer Name"
    TestimonialText="Great product and service!"
    Company="Company Name"
    Position="Job Title"
    Class="card shadow-sm border-0"
    ShowRating="true"
    Rating="5" />

<style>
/* Bootstrap-compatible styling */
.osirion-testimonial-card.card {
    transition: transform 0.2s, box-shadow 0.2s;
}

.osirion-testimonial-card.card:hover {
    transform: translateY(-2px);
    box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15) !important;
}

.osirion-testimonial-rating {
    margin-bottom: 1rem;
}

.osirion-testimonial-star-filled {
    color: #ffc107;
}

.osirion-testimonial-star-empty {
    color: #e9ecef;
}

.osirion-testimonial-quote {
    font-style: italic;
    margin-bottom: 1.5rem;
    border-left: 4px solid #007bff;
    padding-left: 1rem;
}

.osirion-testimonial-footer {
    border-top: 1px solid #e9ecef;
    padding-top: 1rem;
}

.osirion-testimonial-person-row {
    display: flex;
    align-items: center;
    gap: 1rem;
}

.osirion-testimonial-image {
    border-radius: 50%;
    object-fit: cover;
}

.osirion-testimonial-name {
    font-weight: 600;
    color: #495057;
    font-style: normal;
}

.osirion-testimonial-details {
    color: #6c757d;
    font-size: 0.875rem;
}

.osirion-testimonial-name-link {
    text-decoration: none;
    color: inherit;
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

.osirion-testimonial-name-link:hover {
    color: #007bff;
}

.osirion-testimonial-linkedin-icon {
    opacity: 0.7;
}

.osirion-testimonial-actions {
    margin-top: 1rem;
    text-align: right;
}
</style>
```

### Tailwind CSS Integration

```razor
<OsirionTestimonialCard 
    Name="Customer Name"
    TestimonialText="Excellent service and support!"
    Company="Tech Company"
    Position="Engineering Manager"
    Class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300"
    ShowRating="true"
    Rating="4" />

<style>
/* Tailwind-compatible classes */
.osirion-testimonial-card {
    @apply p-6 bg-white rounded-lg shadow-md transition-all duration-300;
}

.osirion-testimonial-card:hover {
    @apply shadow-lg transform -translate-y-1;
}

.osirion-testimonial-rating {
    @apply flex gap-1 mb-4;
}

.osirion-testimonial-star-filled {
    @apply text-yellow-400;
}

.osirion-testimonial-star-empty {
    @apply text-gray-300;
}

.osirion-testimonial-quote {
    @apply italic mb-6 border-l-4 border-blue-500 pl-4 text-gray-700;
}

.osirion-testimonial-footer {
    @apply border-t border-gray-200 pt-4;
}

.osirion-testimonial-person-row {
    @apply flex items-center space-x-4;
}

.osirion-testimonial-image {
    @apply rounded-full object-cover;
}

.osirion-testimonial-name {
    @apply font-semibold text-gray-900 not-italic;
}

.osirion-testimonial-details {
    @apply text-gray-600 text-sm;
}

.osirion-testimonial-name-link {
    @apply no-underline text-gray-900 flex items-center space-x-2 hover:text-blue-600 transition-colors;
}

.osirion-testimonial-linkedin-icon {
    @apply opacity-70;
}

.osirion-testimonial-actions {
    @apply mt-4 text-right;
}

/* Variant styles */
.osirion-testimonial-highlighted {
    @apply bg-gradient-to-br from-blue-50 to-indigo-50 border border-blue-200;
}

.osirion-testimonial-minimal {
    @apply shadow-none border border-gray-200;
}

.osirion-testimonial-compact {
    @apply p-4;
}

/* Size styles */
.osirion-testimonial-small .osirion-testimonial-quote {
    @apply text-sm mb-4;
}

.osirion-testimonial-large .osirion-testimonial-quote {
    @apply text-lg mb-8;
}
</style>
```

## Best Practices

### Content Guidelines

1. **Authenticity**: Use real testimonials from actual customers
2. **Specificity**: Include specific details and results when possible
3. **Diversity**: Show testimonials from different types of customers
4. **Credibility**: Include names, positions, and companies for verification
5. **Balance**: Mix different testimonial lengths and styles

### Performance Optimization

1. **Image Loading**: Use `IsFirstVisible` for above-the-fold testimonials
2. **Lazy Loading**: Enable lazy loading for testimonials below the fold
3. **Image Optimization**: Provide appropriately sized profile images
4. **Content Loading**: Consider skeleton states for dynamic content
5. **Accessibility**: Ensure proper ARIA labels and semantic markup

### User Experience

1. **Visual Hierarchy**: Use different variants to highlight important testimonials
2. **Social Proof**: Include LinkedIn links for professional credibility
3. **Call to Action**: Add read more links to detailed case studies
4. **Mobile Design**: Ensure testimonials work well on all screen sizes
5. **Interaction**: Consider hover effects and smooth transitions

The OsirionTestimonialCard component provides a comprehensive solution for displaying customer testimonials with professional styling and excellent user experience.
