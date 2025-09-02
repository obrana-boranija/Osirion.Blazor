# OsirionContactForm

Accessible contact form with validation, spam protection, optional privacy and contact info, and SSR navigation feedback.

Basic usage

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<OsirionContactForm OnSubmit="HandleSubmit" />

@code {
    private Task HandleSubmit(ContactFormModel model)
    {
        // Send email, save, or call API
        return Task.CompletedTask;
    }
}
```

Highlights

- Built-in success, error, spam states via query SubmissionResultValue.
- Optional UseBuiltInEmailService with EmailServiceFactory.
- SideBySideLayout and UseDarkTheme for layout and theming.
