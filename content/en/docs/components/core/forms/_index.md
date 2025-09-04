---
id: 'core-forms-components-overview'
order: 1
layout: docs
title: Core Forms Components Overview
permalink: /docs/components/core/forms
description: Complete overview of Osirion.Blazor Core Forms components including contact forms and form handling for user interaction and data collection.
author: Dejan Demonjić
date: 2025-09-03
featured_image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
categories:
- Components
- Core Components
- Forms
tags:
- blazor
- core-components
- forms
- contact-forms
- user-input
- validation
is_featured: true
published: true
slug: components/core/forms
lang: en
custom_fields: {}
seo_properties:
  title: 'Core Forms Components - Osirion.Blazor Form Handling'
  description: 'Explore Osirion.Blazor Core Forms components for contact forms and user input handling with validation.'
  image: https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png
  canonical: 'https://getosirion.com/docs/components/core/forms'
  lang: en
  robots: index, follow
  og_title: 'Core Forms Components - Osirion.Blazor'
  og_description: 'Complete documentation for Core Forms components with validation and user input handling.'
  og_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Core Forms Components Documentation'
  twitter_description: 'Complete documentation for Osirion.Blazor Core Forms components.'
  twitter_image_url: 'https://storage.googleapis.com/croct-assets-b931d070/blog/Headless_CMS_within_the_React_framework_3_1_da922d2562/Headless_CMS_within_the_React_framework_3_1_da922d2562.png'
  meta_tags: {}
---

# Core Forms Components Overview

The Osirion.Blazor Core Forms module provides comprehensive form components for user interaction and data collection. These components offer robust validation, accessibility features, and seamless integration with backend services.

## Available Components

### Contact Form
A fully-featured contact form component with built-in validation, spam protection, and email handling capabilities. Perfect for customer inquiries, support requests, and general communication.

## Key Features

- **Built-in Validation**: Client-side and server-side validation support
- **Accessibility**: WCAG compliant with proper ARIA labels and keyboard navigation
- **Spam Protection**: Integrated anti-spam measures and captcha support
- **Email Integration**: Direct email sending and notification capabilities
- **Responsive Design**: Mobile-friendly layouts and touch-optimized inputs
- **Customizable Styling**: Flexible theming and appearance options

## Getting Started

To use form components in your project:

```razor
@using Osirion.Blazor.Core

<ContactForm OnSubmit="@HandleContactSubmit"
             EnableSpamProtection="true"
             RequiredFields="@requiredFields"
             CustomFields="@customFields" />
```

## Form Features

The Contact Form component includes:

- **Standard Fields**: Name, email, subject, and message inputs
- **Custom Fields**: Support for additional form fields and data types
- **File Attachments**: Optional file upload capabilities
- **Multi-language**: Localization support for international applications
- **Progress Indicators**: Visual feedback during form submission
- **Success/Error States**: Clear messaging for user feedback

## Validation Options

Comprehensive validation features:

- **Email Validation**: Proper email format checking
- **Required Field Validation**: Ensure essential information is provided
- **Custom Validation Rules**: Define application-specific validation logic
- **Real-time Feedback**: Immediate validation feedback as users type
- **Server-side Validation**: Backend validation integration

## Integration

Forms integrate seamlessly with:

- Email services (SMTP, SendGrid, etc.)
- Customer relationship management (CRM) systems
- Database storage and logging
- Analytics and tracking systems
- Notification services

The form components are designed to be production-ready with enterprise-level features while remaining easy to implement and customize.
