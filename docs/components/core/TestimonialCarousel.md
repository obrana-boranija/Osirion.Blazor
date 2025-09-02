# OsirionTestimonialCarousel

CSS-driven infinite carousel for testimonials. No JavaScript required. Configurable speed, direction, and card appearance.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionTestimonialCarousel Title="What customers say" />
```

Custom items

```razor
<OsirionTestimonialCarousel 
    CustomTestimonials="@items"
    CardVariant="TestimonialVariant.Highlighted"
    CardElevated="true"
    ShowRating="true" />

@code {
    private List<TestimonialItem> items = new()
    {
        new("Jane Doe", "CTO", "Acme", "Outstanding quality.", ProfileImageUrl: "/img/jane.jpg", Rating: 5, ShowRating: true)
    };
}
```
