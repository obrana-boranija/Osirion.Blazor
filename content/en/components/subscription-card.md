---
id: 'osirion-subscription-card'
order: 3
layout: docs
title: OsirionSubscriptionCard
permalink: /components/subscription-card
description: OsirionSubscriptionCard is a newsletter signup form that works under static SSR, with validation, a honeypot and optional topic choices.
author: Dejan Demonjić
date: 2025-09-03
last_modified: 2026-10-11
featured_image: /assets/images/blazor-cover.png
categories:
- Components
- Core Components
- Cards
- Forms
tags:
- subscription-card
- newsletter
- forms
- validation
- static-ssr
is_featured: true
published: true
slug: subscription-card
lang: en
redirect_from:
- /docs/components/core/cards/subscription-card
custom_fields:
  group: cards
seo_properties:
  title: 'OsirionSubscriptionCard - Osirion.Blazor'
  description: 'OsirionSubscriptionCard is a newsletter signup form that works under static SSR, with validation, a honeypot and optional topic choices.'
  image: /assets/images/blazor-cover.png
  canonical: 'https://getosirion.com/components/subscription-card'
  lang: en
  robots: index, follow
  og_title: 'OsirionSubscriptionCard - Osirion.Blazor'
  og_description: 'A newsletter signup form for static SSR: email validation, privacy consent, a honeypot and topic choices.'
  og_image_url: '/assets/images/blazor-cover.png'
  og_type: article
  json_ld: ''
  type: Article
  twitter_card: summary_large_image
  twitter_title: 'OsirionSubscriptionCard - Osirion.Blazor'
  twitter_description: 'A newsletter signup form for static SSR with validation and topic choices.'
  twitter_image_url: '/assets/images/blazor-cover.png'
  meta_tags: {}
---

# OsirionSubscriptionCard

`OsirionSubscriptionCard` is a newsletter signup form in a card: an email field, an optional name field, a privacy
consent checkbox and optional topics to choose from. It posts as a normal form, so it works on static SSR pages
without a circuit or WebAssembly. Your code receives the validated data in `OnSubscribe` and stores it wherever your
mailing list lives.

## Usage

```razor
@using Osirion.Blazor.Core.Models

<OsirionSubscriptionCard Id="newsletter" OnSubscribe="SaveAsync" />

@code {
    private Task SaveAsync(SubscriptionModel subscription)
    {
        // Send subscription.Email to your mailing list provider here.
        // Throwing an exception shows ErrorMessage instead of SuccessMessage.
        return Task.CompletedTask;
    }
}
```

A card with a name field and topics, where one topic cannot be turned off:

```razor
@using Osirion.Blazor.Core.Models

<OsirionSubscriptionCard Id="blog-signup"
                         Title="Get new articles by email"
                         Description="One email when a new article is published."
                         ShowNameField="true"
                         ShowSubscriptionCategories="true"
                         SubscriptionCategories="topics"
                         PrivacyPolicyText="@PrivacyText"
                         OnSubscribe="SaveAsync" />

@code {
    private const string PrivacyText = "I agree to the <a href=\"/privacy\">privacy policy</a>";

    private readonly SubscriptionCategory[] topics =
    [
        new() { Id = "articles", Name = "New articles", IsRequired = true },
        new() { Id = "releases", Name = "Release notes", Description = "About one email a month", IsDefault = true }
    ];

    private Task SaveAsync(SubscriptionModel subscription)
    {
        // subscription.Name, subscription.Email and subscription.SubscriptionCategories (the chosen ids)
        return Task.CompletedTask;
    }
}
```

## Parameters

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `Id` | `string` | `"subscription-card"` | Required. The form name and the prefix of element ids; give each card on a page its own value. |
| `Title` | `string` | `"Stay Updated"` | Heading (`h3`). Rendered as HTML, not encoded. |
| `Description` | `string` | `"Get the latest updates and insights delivered to your inbox."` | Text under the title. Rendered as HTML, not encoded. |
| `ShowNameField` | `bool` | `false` | Adds an optional name field (up to 100 characters). |
| `NamePlaceholder` | `string` | `"Your name (optional)"` | Name field placeholder. |
| `EmailPlaceholder` | `string` | `"Enter your email address"` | Email field placeholder. |
| `SubscribeButtonText` | `string` | `"Subscribe"` | Button text. |
| `SubscribingText` | `string` | `"Subscribing..."` | Declared, but not rendered in 4.0.0. |
| `ShowRequiredIndicators` | `bool` | `true` | Shows `*` after required labels. |
| `PrivacyPolicyText` | `string?` | a link to `/privacy-policy` | Label of the consent checkbox. Rendered as HTML, not encoded. Keep it set: see Behavior. |
| `ShowSubscriptionCategories` | `bool` | `false` | Shows topics to choose from. |
| `SubscriptionCategories` | `IReadOnlyList<SubscriptionCategory>?` | `null` | The topics. With `ShowSubscriptionCategories` on and no list, three sample topics are shown. |
| `CategoriesText` | `string` | `"What would you like to receive?"` | Label above the topics. |
| `ShowCategoriesAsDropdown` | `bool` | `false` | Shows the topics in a `details` drop-down instead of a list of checkboxes. |
| `CategoriesDropdownText` | `string` | `"Select Categories"` | Drop-down button text. |
| `OnSubscribe` | `EventCallback<SubscriptionModel>` | | Called with the validated data. |
| `OnValidationFailed` | `EventCallback<EditContext>` | | Declared, but not called in 4.0.0. |
| `SuccessMessage` | `string` | a thank-you text | Shown after `OnSubscribe` completes. |
| `ErrorMessage` | `string` | an error text | Shown when `OnSubscribe` throws. |
| `SpamMessage` | `string` | a blocked text | Shown when the honeypot field was filled in. |
| `AlreadySubscribedMessage` | `string` | an already-subscribed text | Shown for the `AlreadySubscribed` result, which the card does not produce itself in 4.0.0. |
| `UseBuiltInEmailService` | `bool` | `false` | Leave off. When on, `OnSubscribe` is skipped and, in 4.0.0, the address is only logged. |
| `CompactLayout` | `bool` | `false` | Less padding and a smaller title and description. |
| `UseDarkTheme` | `bool` | `false` | Adds `osirion-subscription-card-dark`, which takes input and option colors from the theme tokens. |
| `ShowBorder` | `bool` | `true` | Card border. |
| `ShowShadow` | `bool` | `true` | Card shadow. |
| `ChildContent` | `RenderFragment?` | `null` | Footer content under the form. |
| `Class` | `string?` | `null` | Extra CSS classes. |
| `Style` | `string?` | `null` | Inline style. |

`SubscriptionCategory` (namespace `Osirion.Blazor.Core.Models`) has `Id`, `Name`, `Description`, `IsDefault` (checked at
first) and `IsRequired` (checked, disabled and always included). `SubscriptionModel` carries `Name`, `Email`,
`AgreeToPrivacyPolicy` and `SubscriptionCategories`, the list of chosen ids.

## Behavior

- The form is an enhanced `EditForm` with `FormName` set to `Id`. Under static SSR it posts to the same page; in an
  interactive page it submits through the circuit. In both cases the card then navigates to the current URL with a
  `SubmissionResultValue` query value and shows the matching message above the form.
- Topic choices are read from the posted form, so they arrive only under static SSR. In an interactive page the topic
  checkboxes are not bound, and `OnSubscribe` receives only the required topics.
- Validation uses data annotations: the email is required, must be a valid address and at most 255 characters.
- The model always requires the privacy agreement. If `PrivacyPolicyText` is null or empty the checkbox is not
  rendered, and the form can never pass validation, so keep a consent text.
- A hidden honeypot field catches simple bots. When it is filled in, `OnSubscribe` is not called and `SpamMessage` is
  shown.
- `Title`, `Description` and `PrivacyPolicyText` are written as raw HTML without sanitizing. Use text you wrote; never
  pass text from visitors or unreviewed content.
- The drop-down uses the native `details` element, so it opens without script.
- The labels "Name" and "Email", "Select your preferences:" and "Required" are fixed English text.
- With the `EnhancedNavigation` component on the page, the redirect after posting can scroll to the top; set its
  `ResetScrollOnNavigation` to `false` if that matters for a card low on the page.

## Styling

The root element has `osirion-subscription-card` and, by setting, `osirion-subscription-card-compact`,
`osirion-subscription-card-dark`, `osirion-subscription-card-bordered` and `osirion-subscription-card-shadow`. Inputs,
labels, checkboxes, the button and the result message take the class names of the CSS framework set in the theming
options (for example `form-control`, `btn btn-primary` and `alert alert-success` with Bootstrap). These custom
properties are defined on the card:

| Custom property | Default |
| --- | --- |
| `--osirion-subscription-card-padding` | `2rem` (`1.5rem` compact) |
| `--osirion-subscription-card-border-radius` | `0.75rem` |
| `--osirion-subscription-card-background` | `--osirion-background-secondary` |
| `--osirion-subscription-card-border-color` | `--osirion-card-border-color` |
| `--osirion-subscription-card-shadow` | a small shadow |
| `--osirion-subscription-card-shadow-hover` | a larger shadow on hover |

## Related

- [OsirionContactForm](/components/contact-form)
- [Cards overview](/components/cards)
