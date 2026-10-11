# OsirionPageHeading

Purpose
The heading of a page that has no hero: the title as the page's `h1` and an optional sentence below it, in the same
readable width as the body of `AboutPage` and `ContactPage`. It renders without script.

Key parameters
- `Title` (required): the `h1`
- `Description`: the sentence under the title
- `ChildContent`: after the description, for example badges or a date
- `Class` and unmatched attributes go on the `header`

Usage
```razor
<OsirionPageHeading Title="Privacy policy" Description="What this site stores and why." />
```

Notes
- `AboutPage` and `ContactPage` use it when `ShowHero` is off.
- `OsirionBaseSection` renders its title as an `h2`, so a page made of sections needs a heading like this one for its
  `h1`.
