# Markdown Editor Components

The Osirion.Blazor.Cms.Admin package includes a set of powerful Markdown editor components designed for content management. These components are built to be SSR-compatible and provide a seamless editing experience with synchronized preview.

## Features

- **Split View**: Edit and preview your content side-by-side
- **Synchronized Scrolling**: Scroll in one panel and the other follows
- **Toolbar Support**: Common formatting options with keyboard shortcuts
- **Cursor-Aware Formatting**: Insert formatting at the cursor position
- **Live Preview**: See your changes instantly
- **Fully Customizable**: Control appearance and behavior
- **SSR Compatible**: Works in all Blazor hosting models
- **Dark Mode Support**: Adapts to your application's theme

## Components

The package includes the following components:

1. **MarkdownEditor**: A standalone markdown editor component
2. **MarkdownPreview**: A standalone markdown preview component
3. **MarkdownEditorPreview**: A combined component that integrates both editor and preview

## Installation and Setup

### 1. Register Services

Add the markdown editor services to your application in `Program.cs`:

```csharp
// Register markdown editor services with default settings
builder.Services.AddOsirionMarkdownEditor();

// Or with custom Markdig pipeline configuration
builder.Services.AddOsirionMarkdownEditor(config => {
    config.UseAdvancedExtensions()
          .UseYamlFrontMatter()
          .UseEmojiAndSmiley()
          .UseAutoLinks()
          .UseTaskLists()
          .UsePipeTables();
});
```

### 2. Initialize JavaScript Module

Add the `MarkdownEditorInitializer` component to your `App.razor` file:

```razor
@using Osirion.Blazor.Cms.Admin.Services

<Router AppAssembly="@typeof(App).Assembly">
    <!-- Router configuration -->
</Router>

@* Initialize the Markdown Editor JavaScript module *@
<MarkdownEditorInitializer />
```

## Basic Usage

### Combined Editor and Preview

```razor
@using Osirion.Blazor.Cms.Admin.Components.Editor

<MarkdownEditorPreview 
    Content="@Content" 
    ContentChanged="@((content) => Content = content)" 
    SyncScroll="true" 
    ShowToolbar="true" />

@code {
    private string Content { get; set; } = "# Hello Markdown\n\nStart typing here...";
}
```

### Separate Editor and Preview

```razor
@using Osirion.Blazor.Cms.Admin.Components.Editor

<div class="editor-container">
    <MarkdownEditor 
        @ref="EditorRef"
        Content="@Content" 
        ContentChanged="@((content) => Content = content)" 
        OnScroll="SyncPreviewScroll" />
        
    <MarkdownPreview 
        @ref="PreviewRef"
        Markdown="@Content" 
        OnScroll="SyncEditorScroll" />
</div>

@code {
    private string Content { get; set; } = "# Hello Markdown\n\nStart typing here...";
    private MarkdownEditor EditorRef;
    private MarkdownPreview PreviewRef;
    private bool isScrolling = false;
    
    private async Task SyncPreviewScroll(double position)
    {
        if (!isScrolling && PreviewRef != null)
        {
            isScrolling = true;
            await PreviewRef.SetScrollPositionAsync(position);
            isScrolling = false;
        }
    }
    
    private async Task SyncEditorScroll(double position)
    {
        if (!isScrolling && EditorRef != null)
        {
            isScrolling = true;
            await EditorRef.SetScrollPositionAsync(position);
            isScrolling = false;
        }
    }
}
```

## Component Parameters

### MarkdownEditor

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Content | string | "" | The markdown content to edit |
| ContentChanged | EventCallback<string> | - | Event raised when content changes |
| Placeholder | string | "Enter markdown content here..." | Placeholder text when editor is empty |
| AutoFocus | bool | false | Whether to focus the editor on initialization |
| SyncScroll | bool | true | Whether to sync scrolling with other components |
| OnScroll | EventCallback<double> | - | Event raised when the editor is scrolled |
| CssClass | string | "" | Additional CSS class to apply to the component |

### MarkdownPreview

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Markdown | string | "" | The markdown content to preview |
| Title | string | "Preview" | Title displayed in the preview header |
| ShowHeader | bool | true | Whether to show the preview header |
| SyncScroll | bool | true | Whether to sync scrolling with other components |
| OnScroll | EventCallback<double> | - | Event raised when the preview is scrolled |
| Pipeline | MarkdownPipeline | Default | Markdig pipeline for rendering markdown |
| ContentCssClass | string | "" | CSS class to apply to the rendered content |
| CssClass | string | "" | Additional CSS class to apply to the component |

### MarkdownEditorPreview

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| Content | string | "" | The markdown content to edit and preview |
| ContentChanged | EventCallback<string> | - | Event raised when content changes |
| Placeholder | string | "Enter markdown content here..." | Placeholder text when editor is empty |
| EditorTitle | string | "Editor" | Title displayed in the editor header |
| PreviewTitle | string | "Preview" | Title displayed in the preview header |
| AutoFocus | bool | false | Whether to focus the editor on initialization |
| SyncScroll | bool | true | Whether to sync scrolling between editor and preview |
| ShowPreview | bool | true | Whether to show the preview panel |
| ShowPreviewChanged | EventCallback<bool> | - | Event raised when preview visibility changes |
| ShowToolbar | bool | true | Whether to show the toolbar |
| ShowPreviewHeader | bool | true | Whether to show the preview header |
| EditorCssClass | string | "" | CSS class to apply to the editor |
| PreviewCssClass | string | "" | CSS class to apply to the preview |
| PreviewContentCssClass | string | "" | CSS class to apply to the rendered content |
| Pipeline | MarkdownPipeline | Default | Markdig pipeline for rendering markdown |
| CssClass | string | "" | Additional CSS class to apply to the component |

## Methods

### MarkdownEditor

- **FocusAsync()**: Sets focus to the editor
- **InsertTextAsync(string text)**: Inserts text at the cursor position
- **WrapTextAsync(string prefix, string suffix, string defaultText)**: Wraps selected text with prefix and suffix
- **GetSelectionAsync()**: Gets the currently selected text in the editor
- **SetScrollPositionAsync(double position)**: Sets the scroll position programmatically

### MarkdownPreview

- **SetScrollPositionAsync(double position)**: Sets the scroll position programmatically

### MarkdownEditorPreview

- **FocusEditorAsync()**: Sets focus to the editor
- **InsertTextAsync(string text)**: Inserts text at the cursor position
- **WrapTextAsync(string prefix, string suffix, string defaultText)**: Wraps selected text with prefix and suffix

## Styling

The components come with default styling that integrates with the Osirion admin theme. You can customize the appearance by overriding CSS variables or applying additional classes.

### CSS Variables

```css
:root {
    /* Editor colors */
    --osirion-admin-bg-editor: #ffffff;
    --osirion-admin-bg-header: #f6f8fa;
    --osirion-admin-text-editor: #24292e;
    --osirion-admin-border: #e1e4e8;
    --osirion-admin-primary: #0366d6;
}

/* Dark theme example */
.dark-theme {
    --osirion-admin-bg-editor: #1e293b;
    --osirion-admin-bg-header: #161b22;
    --osirion-admin-text-editor: #e5e7eb;
    --osirion-admin-border: #30363d;
    --osirion-admin-primary: #58a6ff;
}
```

## Best Practices

1. **Initialize the JavaScript Module**: Always include the `<MarkdownEditorInitializer />` component in your `App.razor` to ensure the JavaScript functions are available.

2. **Use ContentChanged Event**: Always handle the `ContentChanged` event to keep your model in sync with the editor.

3. **Markdig Extensions**: Configure the Markdig pipeline based on your specific needs. Some extensions might not be compatible with your content requirements.

4. **SSR Compatibility**: The components are designed to work in SSR scenarios, but be aware that JavaScript functionality will only be available after the page is interactive.

5. **Synchronization Logic**: When implementing custom scroll synchronization, always use a guard flag (like `isScrolling`) to prevent infinite loops.

## Example: Custom Toolbar

You can create a custom toolbar by using the methods exposed by the `MarkdownEditor` component:

```razor
@using Osirion.Blazor.Cms.Admin.Components.Editor

<div class="custom-markdown-editor">
    <div class="custom-toolbar">
        <button @onclick="InsertHeading">H</button>
        <button @onclick="InsertBold">B</button>
        <button @onclick="InsertItalic">I</button>
        <button @onclick="InsertLink">Link</button>
        <button @onclick="InsertImage">Image</button>
    </div>
    
    <MarkdownEditor @ref="EditorRef" Content="@Content" ContentChanged="@((content) => Content = content)" />
</div>

@code {
    private string Content { get; set; } = "";
    private MarkdownEditor EditorRef;
    
    private async Task InsertHeading() => await EditorRef.WrapTextAsync("## ", "", "Heading");
    private async Task InsertBold() => await EditorRef.WrapTextAsync("**", "**", "bold text");
    private async Task InsertItalic() => await EditorRef.WrapTextAsync("*", "*", "italic text");
    private async Task InsertLink() => await EditorRef.WrapTextAsync("[", "](https://example.com)", "link text");
    private async Task InsertImage() => await EditorRef.InsertTextAsync("![alt text](https://example.com/image.jpg)");
}
```

## Troubleshooting

### JavaScript Module Not Found

If you encounter errors like "Could not find 'initializeMarkdownEditor' in 'undefined'", ensure that:

1. The `<MarkdownEditorInitializer />` component is included in your `App.razor`
2. The JS file is correctly included in your build output by checking the project file
3. You're using the components after the page is interactive in SSR mode

### Scroll Synchronization Issues

If scroll synchronization isn't working properly:

1. Check that both components have `SyncScroll="true"`
2. Ensure that you're not calling `SetScrollPositionAsync` in a loop
3. Verify that the components have a fixed height or percentage-based height of a parent container

### Content Not Updating

If the content isn't updating properly:

1. Ensure you're handling the `ContentChanged` event
2. Update your model with the new content
3. Check for any errors in the browser console