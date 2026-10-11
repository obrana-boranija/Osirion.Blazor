---
id: 'core-forms-components-overview'
order: 1
layout: docs
title: Forms
permalink: /docs/components/core/forms
description: Form components in Osirion.Blazor.Core. OsirionContactForm posts and validates on the server under static SSR and can send mail itself.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Forms
tags:
- blazor
- forms
- contact-form
- static-ssr
- validation
is_featured: true
published: true
slug: forms
lang: en
custom_fields: {}
seo_properties:
  title: 'Forms - Osirion.Blazor'
  description: 'Form components in Osirion.Blazor.Core: a contact form that validates on the server and sends mail under static SSR.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/docs/components/core/forms'
  lang: en
  robots: index, follow
  og_title: 'Forms - Osirion.Blazor'
  og_description: 'A contact form for static SSR with server validation, a honeypot and email delivery.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'Forms - Osirion.Blazor'
  twitter_description: 'A contact form for static SSR with server validation and email delivery.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# Forms

The forms group of `Osirion.Blazor.Core` currently holds one component, the contact form. It is built on Blazor's
`EditForm`, so it posts as a plain HTML form under static SSR, validates with data annotations on the server, and
needs no script.

| Component | What it does |
| --- | --- |
| [OsirionContactForm](/docs/components/core/forms/contact-form) | Name, email, subject and message fields with a privacy checkbox, a honeypot spam check and optional delivery through SMTP, SendGrid or Infobip. |

## Quick start

```razor
@using Osirion.Blazor.Components

<OsirionContactForm Id="contact" UseBuiltInEmailService="true" />
```

`UseBuiltInEmailService` needs the email service registered, either from an `Osirion:Email` configuration section
read by `AddOsirion(builder.Configuration)` or in code with `UseEmailServices`. Without it, handle the message
yourself through `OnSubmit`. The [contact form page](/docs/components/core/forms/contact-form) covers both.

The newsletter sign-up form lives with the cards: see
[OsirionSubscriptionCard](/docs/components/core/cards/subscription-card).
