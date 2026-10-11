# OsirionSteps

Purpose
Numbered steps for a "how it works" section. Steps sit side by side on wide screens and stack on narrow ones;
`Vertical` always stacks them with a line joining the numbers. It renders an ordered list without script.

Key parameters
- `Items` (required): `IReadOnlyList<StepItem>`, `StepItem(Title, Description)` in `Osirion.Blazor.Core.Models`
- `Vertical`: always stacked (default false)
- `Class` and unmatched attributes go on the list

Usage
```razor
<OsirionSteps Items="steps" />

@code {
    private readonly StepItem[] steps =
    [
        new("Install the package", "dotnet add package Osirion.Blazor"),
        new("Point it at your content", "A GitHub repository or a local folder of Markdown files."),
        new("Publish", "Static SSR pages that need no client script."),
    ];
}
```

Notes
- The numbers come from a CSS counter on the list, so they always match the order; screen readers announce the
  list's length and each position, and the drawn number is hidden from them.
- Title and description are plain text.
