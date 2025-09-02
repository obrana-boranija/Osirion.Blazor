# OsirionContactInfoSection

Dedicated contact information section. Works standalone or with OsirionContactForm.

Basic usage

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<OsirionContactInfoSection 
    Title="Contact"
    ContactInfo="@info" />

@code {
    private ContactInformation info = new()
    {
        Address = "123 Main St, City",
        Phone = "+1 555-123-4567",
        Email = "hello@example.com",
        Website = "https://example.com"
    };
}
```

Key parameters

- Title: string. Section title.
- ContactInfo: ContactInformation. Address, Phone, Email, Website, Message.
- UseDarkTheme: bool.
