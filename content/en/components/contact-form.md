---
id: 'osirion-contact-form'
order: 1
layout: docs
title: OsirionContactForm
permalink: /components/contact-form
description: A contact form that validates on the server, blocks simple bots with a honeypot and sends mail through SMTP, SendGrid or Infobip, without script.
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
- contact-form
- forms
- static-ssr
- email
- validation
is_featured: true
published: true
slug: contact-form
lang: en
redirect_from:
- /docs/components/core/forms/contact-form
custom_fields:
  group: forms-and-feedback
seo_properties:
  title: 'OsirionContactForm: Blazor contact form with email delivery | Osirion.Blazor'
  description: 'A Blazor contact form for static SSR: server validation, honeypot spam check and email delivery through SMTP, SendGrid or Infobip.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/contact-form'
  lang: en
  robots: index, follow
  og_title: 'OsirionContactForm - Osirion.Blazor'
  og_description: 'Server-validated contact form with a honeypot and built-in SMTP, SendGrid or Infobip delivery.'
  og_image_url: /assets/images/blazor-cover.png
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionContactForm - Osirion.Blazor'
  twitter_description: 'Server-validated Blazor contact form with email delivery and no script.'
  twitter_image_url: /assets/images/blazor-cover.png
  meta_tags: {}
---

# OsirionContactForm

`OsirionContactForm` renders a name, email, subject and message form with a privacy checkbox and an optional
newsletter checkbox. It validates on the server, posts as a normal HTML form under static SSR, and either sends the
message through the built-in email service or hands it to your own callback.

## Usage

The smallest form passes the message to your code:

```razor
@page "/contact"
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<OsirionContactForm Id="contact" OnSubmit="HandleSubmitAsync" />

@code {
    private async Task HandleSubmitAsync(ContactFormModel message)
    {
        // Save the message, queue it or call your CRM here.
        // An exception shows ErrorMessage instead of SuccessMessage.
        await Task.CompletedTask;
    }
}
```

To have the library send the message by email, register the email service (next section) and set
`UseBuiltInEmailService`:

```razor
<OsirionContactForm Id="contact"
                    UseBuiltInEmailService="true"
                    FormTitle="Get in touch"
                    FormDescription="Questions about the library? Write to us."
                    PrivacyPolicyText="I agree to the <a href='/privacy'>privacy policy</a>"
                    ShowSubscribeOption="false"
                    ContactInfo="contactInfo" />

@code {
    private readonly ContactInformation contactInfo = new()
    {
        Address = "Belgrade, Serbia",
        Email = "hello@example.com"
    };
}
```

When `ContactInfo` is set, an [OsirionContactInfoSection](/components/contact-info-section) is
shown next to the form (or below it with `SideBySideLayout="false"`). Without it, no contact details are rendered.

## Email delivery

`AddOsirion(builder.Configuration)` registers the email service when the `Osirion:Email` section exists.
`Provider` is `Smtp` (the default), `SendGrid` or `Infobip`.

```json
{
  "Osirion": {
    "Email": {
      "Provider": "Smtp",
      "ToEmail": "hello@example.com",
      "FromName": "Website contact form",
      "SubjectTemplate": "Contact form: {subject}",
      "Smtp": {
        "Host": "smtp.example.com",
        "Port": 587,
        "EnableSsl": true,
        "Username": "contact@example.com",
        "FromEmail": "contact@example.com"
      }
    }
  }
}
```

Keep the password or API key out of `appsettings.json`: put `Osirion:Email:Smtp:Password`,
`Osirion:Email:SendGrid:ApiKey` or `Osirion:Email:Infobip:ApiKey` in user secrets locally, and in environment
variables or a key vault in production.

| Key | Default | Meaning |
| --- | --- | --- |
| `Provider` | `Smtp` | `Smtp`, `SendGrid` or `Infobip`. |
| `ToEmail` | empty | Where messages are delivered. |
| `FromName` | `Contact Form` | Sender display name. |
| `SubjectTemplate` | `Contact Form: {subject}` | `{subject}` is replaced with the visitor's subject. |
| `Smtp`: `Host`, `Port`, `EnableSsl`, `Username`, `Password`, `FromEmail` | port `587`, SSL on | SMTP server settings. |
| `SendGrid`: `ApiKey`, `FromEmail` | empty | SendGrid settings. |
| `Infobip`: `BaseUrl`, `ApiKey`, `FromEmail` | `BaseUrl` is `https://api.infobip.com` | Infobip settings. |

You can also configure it in code with `UseEmailServices`:

```csharp
using Osirion.Blazor.Core.Configuration;
using Osirion.Blazor.Extensions;

builder.Services.AddOsirion(osirion => osirion.UseEmailServices(email =>
{
    email.Provider = EmailProviderType.SendGrid;
    email.ToEmail = "hello@example.com";
    email.SendGrid.FromEmail = "noreply@example.com";
    email.SendGrid.ApiKey = builder.Configuration["Osirion:Email:SendGrid:ApiKey"] ?? string.Empty;
}));
```

Without the `Osirion.Blazor` meta-package, `services.AddOsirionEmailServices(configuration)` from
`Osirion.Blazor.Core.Extensions` does the same. The mail is HTML, every visitor value in it is HTML-encoded, and the
visitor's address is set as Reply-To, so you can answer directly. SendGrid and Infobip calls time out after 30
seconds.

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Id` | `string` | `"contact-form"` | Required. Form name and element id prefix; use a unique value per form on a page. |
| `FormTitle` | `string` | `"Write us"` | Heading (`h2`) above the form; empty hides it. |
| `FormDescription` | `string` | empty | Text below the title. |
| `NamePlaceholder` | `string` | `"Your name"` | Placeholder of the name field. |
| `EmailPlaceholder` | `string` | `"Your email address"` | Placeholder of the email field. |
| `SubjectPlaceholder` | `string` | `"Subject"` | Placeholder of the subject field. |
| `MessagePlaceholder` | `string` | `"Your message"` | Placeholder of the message field. |
| `MessageRows` | `int` | `6` | Rows of the message textarea. |
| `SubmitButtonText` | `string` | `"Send Message"` | Submit button text. |
| `ShowResetButton` | `bool` | `false` | Shows a reset button. |
| `ResetButtonText` | `string` | `"Reset"` | Reset button text. |
| `ShowRequiredIndicators` | `bool` | `true` | Shows `*` after required labels. |
| `PrivacyPolicyText` | `string?` | text linking to `/privacy-policy` | Label of the privacy checkbox, rendered as HTML. |
| `ShowSubscribeOption` | `bool` | `true` | Shows the newsletter checkbox. |
| `SubscribeText` | `string` | `"I would like to receive updates and newsletters"` | Label of the newsletter checkbox. |
| `ShowContactInformation` | `bool` | `true` | Shows the contact information section when `ContactInfo` is set. |
| `ContactInfoTitle` | `string` | `"Contact information"` | Title of that section. |
| `ContactInfo` | `ContactInformation?` | `null` | Address, phone, email, website and message to show. |
| `UseBuiltInEmailService` | `bool` | `false` | Sends the message through the registered email service. |
| `OnSubmit` | `EventCallback<ContactFormModel>` | none | Called with a valid, non-spam submission; with the built-in service, after a successful send. |
| `SuccessMessage` | `string` | thank-you text | Shown after a successful submission. |
| `ErrorMessage` | `string` | error text | Shown when sending or your callback fails. |
| `SpamMessage` | `string` | spam text | Shown when the honeypot field was filled. |
| `SideBySideLayout` | `bool` | `true` | Form and contact information side by side; `false` stacks them. |
| `UseDarkTheme` | `bool` | `false` | Uses the form's dark palette instead of the theme colors. |

`Class`, `Style` and unmatched attributes from `OsirionComponentBase` go on the outer element. `SubmittingText`,
`OnReset` and `OnValidationFailed` are declared but not used by the current markup.

## Behavior

- **Static SSR.** The form is an `EditForm` with `FormName` set to `Id`, so it posts and validates on the server
  without script. After a valid post the component redirects to the same page with a `SubmissionResultValue` query
  value, and the page shows the success, error or spam message. Invalid fields show their messages in place.
- **Validation** comes from data annotations on `ContactFormModel`: a name up to 100 characters, a valid email up to
  255, a subject up to 200, a message of 10 to 5,000 characters, and a ticked privacy checkbox.
- **Spam.** A hidden `Website` field acts as a honeypot. When a bot fills it, nothing is sent, `OnSubmit` is not
  called and `SpamMessage` is shown. A missing or failing email service shows `ErrorMessage`, never the spam message.
- **Without the email service.** With `UseBuiltInEmailService="true"` and no email registration, the form logs a
  warning and shows `ErrorMessage`.
- **Accessibility.** Every field has a `label`, and each input points to its validation message with
  `aria-describedby`. Status icons are hidden from assistive technology; the message text carries the meaning.

> [!WARNING]
> `PrivacyPolicyText` is rendered as raw HTML. Use only text you wrote; never pass visitor or CMS input.

> [!CAUTION]
> The model always requires `AgreeToPrivacyPolicy`. If you hide the checkbox by setting `PrivacyPolicyText` to null
> or empty, the form can no longer pass validation, so keep the checkbox.

Rate-limit the page that hosts the form: the honeypot stops simple bots, not a determined sender.

## Styling

The outer element has `osirion-contact-form-container` plus `osirion-contact-form-side-by-side` or
`osirion-contact-form-stacked`, and `osirion-contact-form-dark` with `UseDarkTheme`. Override the form's custom
properties on the container:

```css
.osirion-contact-form-container {
    --osirion-contact-accent: #0e7490;
    --osirion-contact-radius: 0;
}
```

The properties are `--osirion-contact-bg`, `--osirion-contact-form-bg`, `--osirion-contact-text`,
`--osirion-contact-text-muted`, `--osirion-contact-border`, `--osirion-contact-accent`, `--osirion-contact-radius` and
`--osirion-contact-spacing`. In the light palette they default to the theme tokens (`--osirion-background-primary`,
`--osirion-text-primary`, `--osirion-action-primary` and so on), so a CSS framework adapter recolors the form. Buttons
and the status message use the active framework's classes.

## Related

- [Forms overview](/components/forms-and-feedback)
- [OsirionContactInfoSection](/components/contact-info-section)
- [OsirionFooter](/components/footer)
