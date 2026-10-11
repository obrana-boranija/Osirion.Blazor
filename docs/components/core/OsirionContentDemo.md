# OsirionContentDemo

Purpose
Renders a live example inside Markdown content: a Razor component registered as a demo, and, when the demo has source
code, a Code tab next to the preview. `OsirionHtmlRenderer` (used by every CMS page component) renders one for each
fenced `demo` block whose text is a registered key.

Setup
```csharp
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Core.Services;

builder.Services.AddOsirionContentDemos(demos => demos
    .Add<InstallTabsDemo>("install-tabs",
        title: "Install command tabs",
        source: ContentDemoRegistry.ReadEmbeddedSource(
            typeof(InstallTabsDemo).Assembly,
            "MySite.Components.Demos.InstallTabsDemo.razor")));
```

Embed the demo's `.razor` file so its source can be shown:

```xml
<ItemGroup>
  <EmbeddedResource Include="Components\Demos\*.razor" />
</ItemGroup>
```

In Markdown
````markdown
```demo
install-tabs
```
````

Key parameters
- `Demo`: the `ContentDemo` to render (key, component type, parameters, title, source)
- `Key`: a registered key, resolved from `IContentDemoRegistry` when `Demo` is not set
- `EnableCopyButton`: copy button on the source (default true)
- `PreviewText`, `CodeText`: tab labels (default "Preview" and "Code")

Notes
- Keys are 1 to 60 lowercase letters, digits or hyphens; registering a key twice throws.
- Only blocks at the top level of the content become demos. A `demo` block inside a list or quote, an unknown key, or
  `OsirionHtmlRenderer EnableDemos="false"` leaves it a code block, so a typo is visible instead of silently empty.
- The source is HTML-encoded before it is shown; it is never run. Demos are components you register, so content can
  only choose among them.
- The marker that holds a demo's place is added after the content is sanitized and carries a random value per
  render, so content cannot forge one.
- Demos render under static SSR like any component. A demo that needs interactivity sets its own render mode.
- On GitHub, the same Markdown shows the block as a code block with the key, which keeps files readable there.
