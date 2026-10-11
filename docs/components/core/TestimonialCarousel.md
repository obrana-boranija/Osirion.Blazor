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


Motion and accessibility
- The animation runs only while the carousel is on screen and the visitor has not asked for reduced motion; vertical wheel scrolling always reaches the page
- It pauses while keyboard focus is inside it, and the enhancement script adds a pause button (override the labels with data-pause-label and data-play-label attributes)
- Under prefers-reduced-motion every testimonial is shown statically; the looping copies are hidden
- The looping copies are aria-hidden and inert, so their links are not focusable
