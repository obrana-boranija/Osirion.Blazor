# OsirionTestimonialCard

Professional testimonial card with profile, rating, quote, and optional LinkedIn and read-more.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionTestimonialCard 
    Name="Sarah Johnson"
    Position="Senior Developer"
    Company="Tech Solutions"
    TestimonialText="This platform transformed our workflow."
    ProfileImageUrl="/img/sarah.jpg"
    ShowRating="true"
    Rating="5" />
```

Notes

- Variants: Default, Minimal, Highlighted, Compact.
- Sizes: Small, Normal, Large. ImageSize auto-tunes by size.
- Read more via ReadMoreHref/Text/Variant.
