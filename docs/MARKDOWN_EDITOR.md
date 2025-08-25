# Markdown Editor Components

[![Component](https://img.shields.io/badge/Component-CMS_Admin-blue)](https://github.com/obrana-boranija/Osirion.Blazor/tree/master/src/Osirion.Blazor.Cms.Admin)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Admin)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Admin)

The Osirion.Blazor.Cms.Admin package includes a set of powerful Markdown editor components designed for content management. These components are built to be SSR-compatible and provide a seamless editing experience with synchronized preview, toolbar support, and comprehensive formatting options.

## Features

- **Split View**: Edit and preview your content side-by-side
- **Synchronized Scrolling**: Scroll in one panel and the other follows
- **Toolbar Support**: Common formatting options with keyboard shortcuts
- **Cursor-Aware Formatting**: Insert formatting at the cursor position
- **Live Preview**: See your changes instantly with Markdig rendering
- **Fully Customizable**: Control appearance and behavior
- **SSR Compatible**: Works in all Blazor hosting models
- **Dark Mode Support**: Adapts to your application's theme
- **Accessibility**: Full keyboard navigation and screen reader support
- **Performance Optimized**: Efficient rendering and memory usage

## Components

The package includes the following components:

1. **MarkdownEditor**: A standalone markdown editor component
2. **MarkdownPreview**: A standalone markdown preview component  
3. **MarkdownEditorPreview**: A combined component that integrates both editor and preview
4. **MarkdownEditorInitializer**: JavaScript module initializer

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
          .UsePipeTables()
          .UseBootstrap()
          .UseMediaLinks();
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
    ShowToolbar="true"
    ShowPreview="true" />

@code {
    private string Content { get; set; } = "# Hello Markdown\n\nStart typing here...";
}
```

### Advanced Editor Configuration

```razor
<MarkdownEditorPreview 
    Content="@BlogPost.Content"
    ContentChanged="@UpdateContent"
    Placeholder="Write your blog post content here..."
    EditorTitle="Content Editor"
    PreviewTitle="Live Preview"
    AutoFocus="true"
    SyncScroll="true"
    ShowPreview="@showPreview"
    ShowPreviewChanged="@((show) => showPreview = show)"
    ShowToolbar="true"
    ShowPreviewHeader="true"
    EditorCssClass="custom-editor"
    PreviewCssClass="custom-preview"
    PreviewContentCssClass="blog-content"
    Pipeline="@customPipeline" />

@code {
    private BlogPost BlogPost { get; set; } = new();
    private bool showPreview = true;
    private MarkdownPipeline customPipeline;
    
    protected override void OnInitialized()
    {
        customPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseEmojiAndSmiley()
            .Build();
    }
    
    private void UpdateContent(string content)
    {
        BlogPost.Content = content;
        StateHasChanged();
    }
}
```

### Separate Editor and Preview

```razor
@using Osirion.Blazor.Cms.Admin.Components.Editor

<div class="editor-container row">
    <div class="col-md-6">
        <MarkdownEditor 
            @ref="EditorRef"
            Content="@Content" 
            ContentChanged="@((content) => Content = content)" 
            OnScroll="SyncPreviewScroll"
            Placeholder="Enter your markdown content..."
            AutoFocus="true" />
    </div>
    
    <div class="col-md-6">
        <MarkdownPreview 
            @ref="PreviewRef"
            Markdown="@Content" 
            OnScroll="SyncEditorScroll"
            Title="Preview"
            ShowHeader="true" />
    </div>
</div>

@code {
    private string Content { get; set; } = "# Hello Markdown\n\nStart typing here...";
    private MarkdownEditor? EditorRef;
    private MarkdownPreview? PreviewRef;
    private bool isScrolling = false;
    
    private async Task SyncPreviewScroll(double position)
    {
        if (!isScrolling && PreviewRef is not null)
        {
            isScrolling = true;
            await PreviewRef.SetScrollPositionAsync(position);
            isScrolling = false;
        }
    }
    
    private async Task SyncEditorScroll(double position)
    {
        if (!isScrolling && EditorRef is not null)
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

## Toolbar Integration

### Built-in Toolbar

The `MarkdownEditorPreview` component includes a built-in toolbar with common formatting options:

```razor
<MarkdownEditorPreview 
    Content="@content"
    ContentChanged="@((c) => content = c)"
    ShowToolbar="true" />
```

### Custom Toolbar

Create a custom toolbar by using the methods exposed by the `MarkdownEditor` component:

```razor
@using Osirion.Blazor.Cms.Admin.Components.Editor

<div class="custom-markdown-editor">
    <div class="custom-toolbar mb-3">
        <div class="btn-group" role="group">
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertHeading" title="Heading">
                <i class="bi bi-type-h1"></i>
            </button>
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertBold" title="Bold">
                <i class="bi bi-type-bold"></i>
            </button>
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertItalic" title="Italic">
                <i class="bi bi-type-italic"></i>
            </button>
        </div>
        
        <div class="btn-group ms-2" role="group">
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertLink" title="Link">
                <i class="bi bi-link-45deg"></i>
            </button>
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertImage" title="Image">
                <i class="bi bi-image"></i>
            </button>
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertCode" title="Code">
                <i class="bi bi-code"></i>
            </button>
        </div>
        
        <div class="btn-group ms-2" role="group">
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertList" title="List">
                <i class="bi bi-list-ul"></i>
            </button>
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertNumberedList" title="Numbered List">
                <i class="bi bi-list-ol"></i>
            </button>
            <button type="button" class="btn btn-outline-secondary" @onclick="InsertQuote" title="Quote">
                <i class="bi bi-quote"></i>
            </button>
        </div>
    </div>
    
    <MarkdownEditor @ref="EditorRef" Content="@Content" ContentChanged="@((content) => Content = content)" />
</div>

@code {
    private string Content { get; set; } = "";
    private MarkdownEditor? EditorRef;
    
    private async Task InsertHeading() => await EditorRef!.WrapTextAsync("## ", "", "Heading");
    private async Task InsertBold() => await EditorRef!.WrapTextAsync("**", "**", "bold text");
    private async Task InsertItalic() => await EditorRef!.WrapTextAsync("*", "*", "italic text");
    private async Task InsertLink() => await EditorRef!.WrapTextAsync("[", "](https://example.com)", "link text");
    private async Task InsertImage() => await EditorRef!.InsertTextAsync("![alt text](https://example.com/image.jpg)");
    private async Task InsertCode() => await EditorRef!.WrapTextAsync("`", "`", "code");
    private async Task InsertList() => await EditorRef!.InsertTextAsync("\n- List item");
    private async Task InsertNumberedList() => await EditorRef!.InsertTextAsync("\n1. Numbered item");
    private async Task InsertQuote() => await EditorRef!.WrapTextAsync("> ", "", "Quote text");
}
```

## Styling and Customization

### CSS Variables

Customize the appearance with CSS variables:

```css
:root {
    /* Editor colors */
    --osirion-admin-bg-editor: #ffffff;
    --osirion-admin-bg-header: #f6f8fa;
    --osirion-admin-text-editor: #24292e;
    --osirion-admin-border: #e1e4e8;
    --osirion-admin-primary: #0366d6;
    
    /* Toolbar styling */
    --osirion-toolbar-bg: #ffffff;
    --osirion-toolbar-border: #e1e4e8;
    --osirion-toolbar-button-hover: #f6f8fa;
    
    /* Preview styling */
    --osirion-preview-bg: #ffffff;
    --osirion-preview-text: #333333;
    --osirion-preview-border: #e1e4e8;
}

/* Dark theme example */
.dark-theme {
    --osirion-admin-bg-editor: #1e293b;
    --osirion-admin-bg-header: #161b22;
    --osirion-admin-text-editor: #e5e7eb;
    --osirion-admin-border: #30363d;
    --osirion-admin-primary: #58a6ff;
    
    --osirion-toolbar-bg: #1e293b;
    --osirion-toolbar-border: #30363d;
    --osirion-toolbar-button-hover: #374151;
    
    --osirion-preview-bg: #1e293b;
    --osirion-preview-text: #e5e7eb;
    --osirion-preview-border: #30363d;
}
```

### Custom Styling Classes

Apply custom styling to different parts of the editor:

```css
/* Custom editor styling */
.custom-editor .osirion-markdown-editor {
    border: 2px solid #007bff;
    border-radius: 8px;
}

.custom-editor .osirion-markdown-editor textarea {
    font-family: 'JetBrains Mono', monospace;
    font-size: 14px;
    line-height: 1.6;
}

/* Custom preview styling */
.custom-preview .osirion-markdown-preview {
    border: 2px solid #28a745;
    border-radius: 8px;
}

/* Blog content specific styling */
.blog-content h1, .blog-content h2, .blog-content h3 {
    color: #2c3e50;
    margin-top: 2rem;
    margin-bottom: 1rem;
}

.blog-content p {
    margin-bottom: 1.5rem;
    line-height: 1.8;
}

.blog-content pre {
    background: #f8f9fa;
    border: 1px solid #e9ecef;
    border-radius: 4px;
    padding: 1rem;
    overflow-x: auto;
}

.blog-content blockquote {
    border-left: 4px solid #007bff;
    margin: 1.5rem 0;
    padding: 1rem 1.5rem;
    background: #f8f9fa;
    border-radius: 0 4px 4px 0;
}
```

## Markdig Pipeline Configuration

Customize the Markdown rendering pipeline:

```csharp
// In Program.cs
builder.Services.AddOsirionMarkdownEditor(pipeline => {
    pipeline
        .UseAdvancedExtensions()         // Tables, task lists, etc.
        .UseYamlFrontMatter()           // YAML frontmatter support
        .UseEmojiAndSmiley()            // Emoji support :smile:
        .UseAutoLinks()                 // Auto-link URLs
        .UseTaskLists()                 // [x] Task list items
        .UsePipeTables()                // | Table | Support |
        .UseBootstrap()                 // Bootstrap CSS classes
        .UseMediaLinks()                // Enhanced media embedding
        .UseCustomContainers()          // ::: custom containers
        .UseMathematics()               // LaTeX math support
        .UseGenericAttributes()         // {.class #id} attributes
        .UseCitations()                 // Citation support
        .UseFooters()                   // Footer support
        .UseFootnotes()                 // Footnote support
        .UseFigures()                   // Figure captions
        .UseListExtras()                // Enhanced list support
        .UseAutoIdentifiers()           // Auto-generate heading IDs
        .UseSmartyPants();              // Smart quotes and dashes
});
```

## Integration with Content Management

### Blog Post Editor

```razor
@page "/admin/blog/edit/{id?}"
@using Osirion.Blazor.Cms.Admin.Components.Editor

<div class="container-fluid">
    <div class="row">
        <div class="col-md-8">
            <div class="card">
                <div class="card-header">
                    <h5 class="card-title">Edit Blog Post</h5>
                </div>
                <div class="card-body">
                    <div class="mb-3">
                        <label class="form-label">Title</label>
                        <input type="text" class="form-control" @bind="post.Title" />
                    </div>
                    
                    <div class="mb-3">
                        <label class="form-label">Content</label>
                        <MarkdownEditorPreview 
                            Content="@post.Content"
                            ContentChanged="@((content) => post.Content = content)"
                            ShowToolbar="true"
                            EditorCssClass="blog-editor"
                            PreviewContentCssClass="blog-preview" />
                    </div>
                </div>
            </div>
        </div>
        
        <div class="col-md-4">
            <div class="card">
                <div class="card-header">
                    <h6 class="card-title">Post Settings</h6>
                </div>
                <div class="card-body">
                    <!-- Metadata form -->
                    <MetadataForm BlogPost="@post" />
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    [Parameter] public string? Id { get; set; }
    
    private BlogPost post = new();
    
    protected override async Task OnInitializedAsync()
    {
        if (!string.IsNullOrEmpty(Id))
        {
            post = await BlogService.GetPostAsync(Id);
        }
    }
}
```

### Documentation Editor

```razor
@page "/admin/docs/edit"
@using Osirion.Blazor.Cms.Admin.Components.Editor

<div class="docs-editor">
    <div class="toolbar-container">
        <div class="btn-group">
            <button class="btn btn-outline-primary" @onclick="SaveDraft">
                <i class="bi bi-save"></i> Save Draft
            </button>
            <button class="btn btn-primary" @onclick="Publish">
                <i class="bi bi-check-circle"></i> Publish
            </button>
        </div>
    </div>
    
    <MarkdownEditorPreview 
        Content="@documentContent"
        ContentChanged="@UpdateContent"
        ShowToolbar="true"
        ShowPreview="true"
        SyncScroll="true"
        EditorTitle="Documentation Editor"
        PreviewTitle="Live Preview"
        Pipeline="@docsPipeline" />
</div>

@code {
    private string documentContent = "";
    private MarkdownPipeline docsPipeline;
    
    protected override void OnInitialized()
    {
        // Custom pipeline for documentation
        docsPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseAutoIdentifiers()
            .UseTaskLists()
            .UseCustomContainers()
            .UseMathematics()
            .Build();
    }
    
    private void UpdateContent(string content)
    {
        documentContent = content;
        // Auto-save logic here
    }
    
    private async Task SaveDraft()
    {
        await DocumentService.SaveDraftAsync(documentContent);
    }
    
    private async Task Publish()
    {
        await DocumentService.PublishAsync(documentContent);
    }
}
```

## Accessibility Features

- **Keyboard Navigation**: Full keyboard support for all interactive elements
- **Screen Reader Support**: Proper ARIA labels and descriptions
- **Focus Management**: Logical tab order and focus indicators
- **Semantic HTML**: Uses proper form elements and headings
- **High Contrast**: Enhanced visibility in high contrast mode

## Performance Considerations

- **Efficient Rendering**: Minimal re-renders and optimized change detection
- **Memory Management**: Proper cleanup of JavaScript resources
- **Large Content**: Handles large markdown documents efficiently
- **Scroll Synchronization**: Optimized scroll event handling

## Best Practices

1. **Initialize JavaScript Module**: Always include the `<MarkdownEditorInitializer />` component in your `App.razor`

2. **Use ContentChanged Event**: Always handle the `ContentChanged` event to keep your model in sync

3. **Markdig Extensions**: Configure the pipeline based on your specific needs

4. **SSR Compatibility**: Components work in SSR scenarios, but JavaScript functionality is available after interactivity

5. **Synchronization Logic**: Use guard flags to prevent infinite scroll loops

6. **Content Validation**: Validate markdown content before saving

7. **Auto-save**: Implement auto-save functionality for better user experience

## Troubleshooting

### JavaScript Module Not Found

If you encounter errors like "Could not find 'initializeMarkdownEditor'":

1. Ensure `<MarkdownEditorInitializer />` is included in `App.razor`
2. Check that the JS file is included in your build output
3. Verify you're using components after page is interactive in SSR mode

### Scroll Synchronization Issues

If scroll synchronization isn't working:

1. Check that both components have `SyncScroll="true"`
2. Ensure guard flags prevent infinite loops
3. Verify components have fixed height containers

### Content Not Updating

If content isn't updating properly:

1. Ensure you're handling the `ContentChanged` event
2. Update your model with the new content
3. Check browser console for JavaScript errors

### Performance Issues

For large documents:

1. Consider implementing lazy loading
2. Use debounced content change events
3. Optimize the Markdig pipeline configuration
4. Implement virtual scrolling for very large content