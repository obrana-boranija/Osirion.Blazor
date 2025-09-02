# OsirionSubscriptionCard

Newsletter subscription component with optional name, categories, privacy agreement, and SSR-friendly status.

Basic usage

```razor
@using Osirion.Blazor.Components
@using Osirion.Blazor.Core.Models

<OsirionSubscriptionCard Id="newsletter" OnSubscribe="Handle" />

@code {
    private Task Handle(SubscriptionModel model)
    {
        // Persist subscription
        return Task.CompletedTask;
    }
}
```

Categories

```razor
<OsirionSubscriptionCard 
    Id="signup"
    ShowSubscriptionCategories="true"
    SubscriptionCategories="@cats" />

@code {
    private IReadOnlyList<SubscriptionCategory> cats = new []
    {
        new SubscriptionCategory { Id = "newsletter", Name = "Newsletter", IsDefault = true },
        new SubscriptionCategory { Id = "updates", Name = "Product Updates" }
    };
}
```

Notes

- Success and error states via SubmissionResultValue.
- UseBuiltInEmailService optional. Honeypot Website prevents spam.
