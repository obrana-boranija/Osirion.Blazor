# OsirionStickySidebar

Sticky sidebar container that respects header offset and optional scrollbar hiding.

Basic usage

```razor
@using Osirion.Blazor.Components

<div class="row">
  <div class="col-3">
    <OsirionStickySidebar TopOffset="56">
      <ChildContent>
        <ul class="list-unstyled">
            <li><a href="#a">A</a></li>
            <li><a href="#b">B</a></li>
        </ul>
      </ChildContent>
    </OsirionStickySidebar>
  </div>
  <div class="col">
    @Body
  </div>
</div>
```

Key parameters

- TopOffset: int. Typically header height in px.
- HideScrollbar: bool. Clip scrollbars visually.
- IsSticky: bool. Disable to render as normal aside.
