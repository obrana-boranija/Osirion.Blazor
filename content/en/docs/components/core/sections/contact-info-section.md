---
id: 'contact-info-section'
order: 2
layout: docs
title: OsirionContactInfoSection Component
permalink: /docs/components/core/sections/contact-info-section
description: Learn how to use the OsirionContactInfoSection component to display structured contact information with address, phone, email, and website details in a consistent format.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Sections
tags:
- blazor
- contact
- information
- address
- phone
- email
- website
is_featured: true
published: true
slug: contact-info-section
lang: en
custom_fields: {}
seo_properties:
  title: 'OsirionContactInfoSection Component - Contact Information Display | Osirion.Blazor'
  description: 'Display structured contact information with the OsirionContactInfoSection component. Features address, phone, email, and website details with icons.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/sections/contact-info-section'
  lang: en
  robots: index, follow
  og_title: 'OsirionContactInfoSection Component - Osirion.Blazor'
  og_description: 'Display structured contact information with address, phone, email, and website details.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionContactInfoSection Component - Osirion.Blazor'
  twitter_description: 'Display structured contact information with professional formatting and icons.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# OsirionContactInfoSection Component

The OsirionContactInfoSection component provides a professional, structured way to display contact information including address, phone number, email, and website details with consistent styling and built-in accessibility features.

## Component Overview

OsirionContactInfoSection is designed to showcase contact information in an organized and visually appealing format. Perfect for contact pages, about sections, or anywhere you need to display business contact details with proper semantic markup and accessibility support.

### Key Features

**Structured Display**: Organized contact information with icons and labels
**Multiple Contact Types**: Support for address, phone, email, and website
**Accessibility Compliant**: Proper semantic markup and ARIA support
**Click-to-Contact**: Automatic phone and email links for user convenience
**Custom Labels**: Configurable field labels for internationalization
**Icon Integration**: Built-in SVG icons for each contact type
**Responsive Design**: Adapts to all screen sizes automatically
**Theme Compatible**: Works with all CSS frameworks and themes
**SEO Optimized**: Structured data friendly markup
**Professional Styling**: Clean, modern appearance out of the box

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ContactInfo` | `ContactInformation?` | `null` | **Required.** The contact information object to display. |
| `Title` | `string` | `"Contact information"` | The section title text. |
| `AddressLabel` | `string` | `"Address:"` | Label text for the address field. |
| `PhoneLabel` | `string` | `"Phone:"` | Label text for the phone field. |
| `EmailLabel` | `string` | `"Email:"` | Label text for the email field. |
| `WebsiteLabel` | `string` | `"Website:"` | Label text for the website field. |

## ContactInformation Model

| Property | Type | Description |
|----------|------|-------------|
| `Address` | `string` | Physical address or location. |
| `Phone` | `string` | Phone number (automatically linked for calls). |
| `Email` | `string` | Email address (automatically linked for emails). |
| `Website` | `string` | Website URL (automatically linked). |
| `Message` | `string` | Additional message or description. |

## Basic Usage

### Simple Contact Information

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

@{
    var contactInfo = new ContactInformation
    {
        Address = "123 Business Street, Suite 100<br>San Francisco, CA 94105",
        Phone = "+1 (555) 123-4567",
        Email = "contact@company.com",
        Website = "https://www.company.com",
        Message = "Get in touch with our team. We're here to help with any questions you may have."
    };
}

<OsirionContactInfoSection 
    ContactInfo="contactInfo"
    Title="Get in Touch" />
```

### Multi-Location Contact Information

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<div class="row g-4">
    <div class="col-lg-6">
        @{
            var headOffice = new ContactInformation
            {
                Address = "123 Corporate Plaza<br>New York, NY 10001<br>United States",
                Phone = "+1 (212) 555-0123",
                Email = "ny@company.com",
                Website = "https://www.company.com",
                Message = "Our headquarters office serving the East Coast region."
            };
        }
        
        <OsirionContactInfoSection 
            ContactInfo="headOffice"
            Title="New York Office" />
    </div>
    
    <div class="col-lg-6">
        @{
            var westCoast = new ContactInformation
            {
                Address = "456 Innovation Drive<br>San Francisco, CA 94105<br>United States",
                Phone = "+1 (415) 555-0456",
                Email = "sf@company.com",
                Website = "https://www.company.com",
                Message = "Our West Coast office focused on technology and innovation."
            };
        }
        
        <OsirionContactInfoSection 
            ContactInfo="westCoast"
            Title="San Francisco Office" />
    </div>
</div>
```

### Localized Contact Section

```razor
@{
    var contactInfoES = new ContactInformation
    {
        Address = "Calle Principal 123<br>28001 Madrid<br>España",
        Phone = "+34 91 123 4567",
        Email = "contacto@empresa.es",
        Website = "https://www.empresa.es",
        Message = "Póngase en contacto con nuestro equipo. Estamos aquí para ayudarle."
    };
}

<OsirionContactInfoSection 
    ContactInfo="contactInfoES"
    Title="Información de Contacto"
    AddressLabel="Dirección:"
    PhoneLabel="Teléfono:"
    EmailLabel="Correo Electrónico:"
    WebsiteLabel="Sitio Web:" />
```

## Advanced Usage

### Contact Section with Additional Information

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<div class="contact-section-wrapper">
    <div class="row g-5 align-items-start">
        <div class="col-lg-6">
            @{
                var contactInfo = new ContactInformation
                {
                    Address = "123 Business Center<br>Suite 500<br>Seattle, WA 98101",
                    Phone = "+1 (206) 555-7890",
                    Email = "hello@techstartup.io",
                    Website = "https://www.techstartup.io",
                    Message = "We're a team of passionate developers and designers creating innovative solutions. Reach out to discuss your project!"
                };
            }
            
            <OsirionContactInfoSection 
                ContactInfo="contactInfo"
                Title="Contact Our Team"
                Class="contact-info-enhanced" />
        </div>
        
        <div class="col-lg-6">
            <div class="contact-extras">
                <div class="contact-hours mb-4">
                    <h4 class="h5 mb-3">
                        <i class="fas fa-clock text-primary me-2"></i>
                        Business Hours
                    </h4>
                    <ul class="list-unstyled">
                        <li><strong>Monday - Friday:</strong> 9:00 AM - 6:00 PM PST</li>
                        <li><strong>Saturday:</strong> 10:00 AM - 2:00 PM PST</li>
                        <li><strong>Sunday:</strong> Closed</li>
                    </ul>
                </div>
                
                <div class="contact-social mb-4">
                    <h4 class="h5 mb-3">
                        <i class="fas fa-share-alt text-primary me-2"></i>
                        Follow Us
                    </h4>
                    <div class="social-links">
                        <a href="#" class="social-link" aria-label="Twitter">
                            <i class="fab fa-twitter"></i>
                        </a>
                        <a href="#" class="social-link" aria-label="LinkedIn">
                            <i class="fab fa-linkedin"></i>
                        </a>
                        <a href="#" class="social-link" aria-label="GitHub">
                            <i class="fab fa-github"></i>
                        </a>
                        <a href="#" class="social-link" aria-label="Instagram">
                            <i class="fab fa-instagram"></i>
                        </a>
                    </div>
                </div>
                
                <div class="contact-emergency">
                    <div class="alert alert-info">
                        <h6 class="alert-heading">
                            <i class="fas fa-exclamation-circle me-2"></i>
                            Emergency Support
                        </h6>
                        <p class="mb-2">For urgent technical issues outside business hours:</p>
                        <p class="mb-0">
                            <strong>Emergency Hotline:</strong> 
                            <a href="tel:+12065551234" class="alert-link">+1 (206) 555-1234</a>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
.contact-section-wrapper {
    padding: 3rem 0;
    background: #f8f9fa;
    border-radius: 1rem;
    margin: 2rem 0;
}

.contact-info-enhanced {
    background: white;
    padding: 2.5rem;
    border-radius: 1rem;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
    border: 1px solid #e9ecef;
}

.contact-extras {
    padding: 1rem 0;
}

.contact-hours ul li {
    padding: 0.5rem 0;
    border-bottom: 1px solid #e9ecef;
}

.contact-hours ul li:last-child {
    border-bottom: none;
}

.social-links {
    display: flex;
    gap: 1rem;
    flex-wrap: wrap;
}

.social-link {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 40px;
    height: 40px;
    background: #007bff;
    color: white;
    border-radius: 50%;
    text-decoration: none;
    transition: all 0.3s ease;
}

.social-link:hover {
    background: #0056b3;
    color: white;
    transform: translateY(-2px);
}

.contact-emergency .alert {
    border-left: 4px solid #17a2b8;
}

@media (max-width: 768px) {
    .contact-section-wrapper {
        padding: 2rem 1rem;
        margin: 1rem 0;
    }
    
    .contact-info-enhanced {
        padding: 1.5rem;
    }
}
</style>
```

### Department-Specific Contact Information

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<div class="departments-contact">
    <div class="text-center mb-5">
        <h2>Contact by Department</h2>
        <p class="lead">Reach out to the right team for faster assistance</p>
    </div>
    
    <div class="row g-4">
        <div class="col-lg-4 col-md-6">
            <div class="department-card">
                <div class="department-header">
                    <i class="fas fa-shopping-cart fa-2x text-primary mb-3"></i>
                    <h4>Sales Team</h4>
                </div>
                
                @{
                    var salesContact = new ContactInformation
                    {
                        Phone = "+1 (555) 123-SALE",
                        Email = "sales@company.com",
                        Message = "Questions about pricing, demos, or getting started? Our sales team is here to help."
                    };
                }
                
                <OsirionContactInfoSection 
                    ContactInfo="salesContact"
                    Title=""
                    Class="department-contact" />
            </div>
        </div>
        
        <div class="col-lg-4 col-md-6">
            <div class="department-card">
                <div class="department-header">
                    <i class="fas fa-tools fa-2x text-success mb-3"></i>
                    <h4>Technical Support</h4>
                </div>
                
                @{
                    var supportContact = new ContactInformation
                    {
                        Phone = "+1 (555) 123-TECH",
                        Email = "support@company.com",
                        Website = "https://help.company.com",
                        Message = "Need help with our product? Our technical team provides 24/7 support."
                    };
                }
                
                <OsirionContactInfoSection 
                    ContactInfo="supportContact"
                    Title=""
                    Class="department-contact" />
            </div>
        </div>
        
        <div class="col-lg-4 col-md-6">
            <div class="department-card">
                <div class="department-header">
                    <i class="fas fa-handshake fa-2x text-info mb-3"></i>
                    <h4>Partnerships</h4>
                </div>
                
                @{
                    var partnerContact = new ContactInformation
                    {
                        Phone = "+1 (555) 123-PRTN",
                        Email = "partners@company.com",
                        Website = "https://partners.company.com",
                        Message = "Interested in partnerships, integrations, or becoming a reseller?"
                    };
                }
                
                <OsirionContactInfoSection 
                    ContactInfo="partnerContact"
                    Title=""
                    Class="department-contact" />
            </div>
        </div>
    </div>
</div>

<style>
.departments-contact {
    padding: 4rem 0;
}

.department-card {
    background: white;
    border: 1px solid #e9ecef;
    border-radius: 1rem;
    padding: 2rem;
    height: 100%;
    transition: all 0.3s ease;
    position: relative;
    overflow: hidden;
}

.department-card::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 4px;
    background: linear-gradient(45deg, #007bff, #6f42c1);
    transform: scaleX(0);
    transition: transform 0.3s ease;
}

.department-card:hover::before {
    transform: scaleX(1);
}

.department-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
    border-color: transparent;
}

.department-header {
    text-align: center;
    margin-bottom: 1.5rem;
    padding-bottom: 1.5rem;
    border-bottom: 1px solid #e9ecef;
}

.department-header h4 {
    color: #333;
    margin-bottom: 0;
    font-weight: 600;
}

.department-contact {
    background: transparent;
    padding: 0;
    box-shadow: none;
    border: none;
}

@media (max-width: 768px) {
    .departments-contact {
        padding: 2rem 0;
    }
    
    .department-card {
        padding: 1.5rem;
    }
}
</style>
```

### Contact Section with Map Integration

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<div class="contact-with-map">
    <div class="row g-0">
        <div class="col-lg-6">
            <div class="contact-info-panel">
                @{
                    var contactInfo = new ContactInformation
                    {
                        Address = "123 Innovation Drive<br>Tech District<br>San Francisco, CA 94105",
                        Phone = "+1 (415) 555-0123",
                        Email = "visit@company.com",
                        Website = "https://www.company.com",
                        Message = "Visit our modern office in the heart of San Francisco's tech district. Free parking available."
                    };
                }
                
                <OsirionContactInfoSection 
                    ContactInfo="contactInfo"
                    Title="Visit Our Office"
                    Class="contact-map-info" />
                
                <div class="visit-info mt-4">
                    <h5>Office Features</h5>
                    <ul class="feature-list">
                        <li><i class="fas fa-car text-primary me-2"></i>Free parking garage</li>
                        <li><i class="fas fa-wifi text-primary me-2"></i>Guest WiFi available</li>
                        <li><i class="fas fa-coffee text-primary me-2"></i>Complimentary coffee bar</li>
                        <li><i class="fas fa-wheelchair text-primary me-2"></i>Wheelchair accessible</li>
                    </ul>
                    
                    <div class="directions-link mt-3">
                        <a href="https://maps.google.com/?q=123+Innovation+Drive+San+Francisco+CA" 
                           target="_blank" 
                           rel="noopener noreferrer"
                           class="btn btn-outline-primary">
                            <i class="fas fa-directions me-2"></i>
                            Get Directions
                        </a>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="col-lg-6">
            <div class="map-placeholder">
                <div class="map-content">
                    <i class="fas fa-map-marker-alt fa-3x text-primary mb-3"></i>
                    <h4>Interactive Map</h4>
                    <p>Integrate with Google Maps, Mapbox, or your preferred mapping service</p>
                    <button class="btn btn-primary" onclick="loadMap()">
                        <i class="fas fa-map me-2"></i>
                        Load Map
                    </button>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
.contact-with-map {
    background: white;
    border-radius: 1rem;
    overflow: hidden;
    box-shadow: 0 20px 60px rgba(0, 0, 0, 0.1);
    margin: 2rem 0;
}

.contact-info-panel {
    padding: 3rem;
    background: #f8f9fa;
    height: 100%;
    min-height: 500px;
}

.contact-map-info {
    background: transparent;
    padding: 0;
    box-shadow: none;
    border: none;
}

.visit-info h5 {
    color: #333;
    margin-bottom: 1rem;
    font-weight: 600;
}

.feature-list {
    list-style: none;
    padding: 0;
}

.feature-list li {
    padding: 0.5rem 0;
    border-bottom: 1px solid #e9ecef;
}

.feature-list li:last-child {
    border-bottom: none;
}

.map-placeholder {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    height: 100%;
    min-height: 500px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    text-align: center;
    position: relative;
}

.map-content {
    padding: 2rem;
}

.map-placeholder h4 {
    color: white;
    margin-bottom: 1rem;
}

.map-placeholder p {
    color: rgba(255, 255, 255, 0.9);
    margin-bottom: 2rem;
}

@media (max-width: 992px) {
    .contact-info-panel {
        padding: 2rem;
        min-height: auto;
    }
    
    .map-placeholder {
        min-height: 300px;
    }
}

@media (max-width: 768px) {
    .contact-info-panel {
        padding: 1.5rem;
    }
}
</style>

<script>
function loadMap() {
    // Implementation for loading map
    console.log('Loading interactive map...');
    
    // Example: Replace placeholder with actual map
    document.querySelector('.map-placeholder').innerHTML = `
        <iframe 
            src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3153.5946647767957!2d-122.39624308468171!3d37.79543097975631!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x8085808c4b3b50cf%3A0x50ad2e53b4b4ca2!2sSan%20Francisco%2C%20CA!5e0!3m2!1sen!2sus!4v1635789012345!5m2!1sen!2sus"
            width="100%" 
            height="500" 
            style="border:0;" 
            allowfullscreen="" 
            loading="lazy"
            referrerpolicy="no-referrer-when-downgrade">
        </iframe>
    `;
}
</script>
```

## Styling Examples

### Bootstrap Integration

```razor
<OsirionContactInfoSection 
    ContactInfo="contactInfo"
    Title="Contact Information"
    Class="bg-light p-4 rounded border" />

<style>
/* Bootstrap-compatible contact info styling */
.osirion-contact-info-section {
    font-family: var(--bs-font-sans-serif);
}

.osirion-contact-info-title {
    color: var(--bs-dark);
    font-size: 1.5rem;
    font-weight: 600;
    margin-bottom: 1rem;
    border-bottom: 2px solid var(--bs-primary);
    padding-bottom: 0.5rem;
}

.osirion-contact-info-description {
    color: var(--bs-secondary);
    margin-bottom: 1.5rem;
}

.osirion-contact-info-item {
    display: flex;
    align-items: flex-start;
    margin-bottom: 1rem;
    padding: 0.75rem;
    background: var(--bs-white);
    border-radius: var(--bs-border-radius);
    border: 1px solid var(--bs-border-color);
    transition: all 0.2s ease;
}

.osirion-contact-info-item:hover {
    border-color: var(--bs-primary);
    box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}

.osirion-contact-info-icon {
    width: 24px;
    height: 24px;
    color: var(--bs-primary);
    margin-right: 1rem;
    flex-shrink: 0;
}

.osirion-contact-info-content {
    flex: 1;
}

.osirion-contact-info-label {
    display: block;
    font-weight: 600;
    color: var(--bs-dark);
    margin-bottom: 0.25rem;
}

.osirion-contact-info-value {
    color: var(--bs-body-color);
    text-decoration: none;
}

.osirion-contact-link:hover {
    color: var(--bs-primary);
    text-decoration: underline;
}
</style>
```

### Tailwind CSS Integration

```razor
<OsirionContactInfoSection 
    ContactInfo="contactInfo"
    Title="Contact Information"
    Class="bg-gray-50 p-6 rounded-lg border border-gray-200" />

<style>
/* Tailwind-compatible contact info styling */
.osirion-contact-info-section {
    @apply font-sans;
}

.osirion-contact-info-title {
    @apply text-gray-900 text-2xl font-semibold mb-4 border-b-2 border-blue-500 pb-2;
}

.osirion-contact-info-description {
    @apply text-gray-600 mb-6;
}

.osirion-contact-info-item {
    @apply flex items-start mb-4 p-3 bg-white rounded-lg border border-gray-200 transition-all duration-200;
}

.osirion-contact-info-item:hover {
    @apply border-blue-500 shadow-md;
}

.osirion-contact-info-icon {
    @apply w-6 h-6 text-blue-500 mr-4 flex-shrink-0;
}

.osirion-contact-info-content {
    @apply flex-1;
}

.osirion-contact-info-label {
    @apply block font-semibold text-gray-900 mb-1;
}

.osirion-contact-info-value {
    @apply text-gray-700 no-underline;
}

.osirion-contact-link:hover {
    @apply text-blue-500 underline;
}
</style>
```

## Best Practices

### Content Guidelines

1. **Complete Information**: Provide all relevant contact details
2. **Accurate Data**: Ensure phone numbers, emails, and addresses are current
3. **Clear Formatting**: Use proper formatting for addresses and phone numbers
4. **Professional Tone**: Keep messages professional and helpful
5. **Accessibility**: Provide alt text and proper labels for screen readers

### User Experience

1. **Click-to-Contact**: Ensure phone and email links work properly
2. **Mobile Optimization**: Test contact links on mobile devices
3. **Visual Hierarchy**: Use proper spacing and typography
4. **Loading Performance**: Optimize any associated images or maps
5. **Cross-Platform Testing**: Verify functionality across different devices

### Security Considerations

1. **Email Protection**: Consider using contact forms to prevent spam
2. **Phone Privacy**: Be mindful of displaying direct phone numbers
3. **Address Security**: Only show public business addresses
4. **Link Validation**: Ensure all external links are secure
5. **Data Handling**: Follow privacy regulations for contact information

### Internationalization

1. **Label Localization**: Translate field labels appropriately
2. **Address Formats**: Use proper address formatting for different countries
3. **Phone Formats**: Format phone numbers according to local conventions
4. **Cultural Considerations**: Adapt to local business communication norms
5. **Time Zones**: Include time zone information for business hours

The OsirionContactInfoSection component provides a professional foundation for displaying contact information with excellent accessibility and user experience features.
