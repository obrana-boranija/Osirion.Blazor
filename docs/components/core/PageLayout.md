# OsirionPageLayout

Layout shell with optional header, body, footer slots and sticky footer support.

Basic usage

```razor
@using Osirion.Blazor.Components

<OsirionPageLayout StickyFooter="true">
    <Header>
        <h1>Title</h1>
    </Header>
    <Body>
        @Body
    </Body>
    <Footer>
        <small>Copyright</small>
    </Footer>
</OsirionPageLayout>
```

Notes

- MinHeightStrategy: "viewport" or "content".
