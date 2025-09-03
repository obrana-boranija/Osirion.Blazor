---
title: "HTML Renderer - Osirion Blazor"
description: "Secure HTML rendering component with syntax highlighting, line numbers, and code block support for Blazor applications."
category: "Core Components"
subcategory: "Rendering"
tags: ["html", "rendering", "syntax-highlighting", "security", "code-blocks"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "html-renderer"
section: "components"
layout: "component"
seo:
  title: "HTML Renderer Component | Osirion Blazor Documentation"
  description: "Learn how to securely render HTML content with syntax highlighting and code block support in Blazor using OsirionHtmlRenderer."
  keywords: ["Blazor", "HTML rendering", "syntax highlighting", "code blocks", "security", "sanitization"]
  canonical: "/docs/components/core/rendering/html-renderer"
  image: "/images/components/html-renderer-preview.jpg"
navigation:
  parent: "Core Components"
  order: 31
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "Core"
    link: "/docs/components/core"
  - text: "Rendering"
    link: "/docs/components/core/rendering"
  - text: "HTML Renderer"
    link: "/docs/components/core/rendering/html-renderer"
---

# HTML Renderer Component

The **OsirionHtmlRenderer** component provides secure HTML rendering capabilities with built-in syntax highlighting for code blocks. It supports various features like line numbers, copy buttons, and HTML sanitization to ensure safe content display.

## Overview

This component is designed to render HTML content safely while providing enhanced features for code display. It automatically detects code blocks and applies syntax highlighting using Prism.js, making it perfect for documentation, blog posts, and technical content.

## Key Features

- **Secure HTML Rendering**: Built-in HTML sanitization to prevent XSS attacks
- **Syntax Highlighting**: Automatic code highlighting using Prism.js
- **Line Numbers**: Optional line numbering for code blocks
- **Copy to Clipboard**: Built-in copy functionality for code blocks
- **Language Detection**: Automatic language detection and mapping
- **Responsive Design**: Mobile-friendly with responsive layout
- **Theme Support**: Compatible with light and dark themes
- **Custom Styling**: Configurable CSS classes and styling options

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `HtmlContent` | `string?` | `null` | The HTML content to render |
| `EnableSyntaxHighlighting` | `bool` | `true` | Enable syntax highlighting for code blocks |
| `ShowLineNumbers` | `bool` | `false` | Show line numbers in code blocks |
| `ShowCopyButton` | `bool` | `true` | Show copy to clipboard button for code blocks |
| `SanitizeHtml` | `bool` | `true` | Enable HTML sanitization for security |
| `CssClass` | `string?` | `null` | Additional CSS classes for the container |
| `Style` | `string?` | `null` | Inline styles for the container |
| `ChildContent` | `RenderFragment?` | `null` | Child content to render instead of HtmlContent |
| `OnContentProcessed` | `EventCallback<string>` | - | Callback fired when content processing is complete |
| `OnCodeBlockCopied` | `EventCallback<string>` | - | Callback fired when code is copied to clipboard |

## Basic Usage

### Simple HTML Rendering

```razor
@using Osirion.Blazor.Core.Components.Rendering

<OsirionHtmlRenderer HtmlContent="@htmlContent" />

@code {
    private string htmlContent = "<h1>Hello World</h1><p>This is a <strong>bold</strong> paragraph.</p>";
}
```

### Code Block with Syntax Highlighting

```razor
<OsirionHtmlRenderer HtmlContent="@codeContent" 
                     ShowLineNumbers="true"
                     ShowCopyButton="true" />

@code {
    private string codeContent = @"
        <pre><code class=""language-csharp"">
        public class ExampleClass
        {
            public string Name { get; set; }
            
            public void DoSomething()
            {
                Console.WriteLine(""Hello, World!"");
            }
        }
        </code></pre>";
}
```

## Advanced Examples

### Custom Styled Renderer

```razor
<OsirionHtmlRenderer HtmlContent="@content"
                     CssClass="custom-renderer border rounded"
                     Style="background-color: var(--bs-light);"
                     EnableSyntaxHighlighting="true"
                     ShowLineNumbers="true"
                     OnContentProcessed="HandleContentProcessed" />

@code {
    private string content = @"
        <h2>API Documentation</h2>
        <p>Here's how to use the API:</p>
        <pre><code class=""language-javascript"">
        fetch('/api/data')
            .then(response => response.json())
            .then(data => console.log(data));
        </code></pre>";
        
    private void HandleContentProcessed(string processedContent)
    {
        // Handle content processing completion
        Console.WriteLine("Content processed successfully");
    }
}
```

### Multiple Language Support

```razor
<div class="row">
    <div class="col-md-6">
        <h3>C# Example</h3>
        <OsirionHtmlRenderer HtmlContent="@csharpCode" 
                             ShowLineNumbers="true" />
    </div>
    <div class="col-md-6">
        <h3>JavaScript Example</h3>
        <OsirionHtmlRenderer HtmlContent="@jsCode" 
                             ShowLineNumbers="true" />
    </div>
</div>

@code {
    private string csharpCode = @"
        <pre><code class=""language-csharp"">
        var items = await repository.GetAllAsync();
        return items.Where(x => x.IsActive).ToList();
        </code></pre>";
        
    private string jsCode = @"
        <pre><code class=""language-javascript"">
        const items = await fetch('/api/items');
        return items.filter(x => x.isActive);
        </code></pre>";
}
```

### With Copy Callback

```razor
<OsirionHtmlRenderer HtmlContent="@codeExample"
                     ShowCopyButton="true"
                     OnCodeBlockCopied="HandleCodeCopied" />

@if (!string.IsNullOrEmpty(copiedMessage))
{
    <div class="alert alert-success mt-2">
        @copiedMessage
    </div>
}

@code {
    private string codeExample = @"
        <pre><code class=""language-bash"">
        dotnet add package Osirion.Blazor.Core
        dotnet restore
        dotnet build
        </code></pre>";
        
    private string copiedMessage = string.Empty;
    
    private void HandleCodeCopied(string copiedText)
    {
        copiedMessage = $"Copied {copiedText.Length} characters to clipboard!";
        StateHasChanged();
        
        // Clear message after 3 seconds
        Task.Delay(3000).ContinueWith(_ => 
        {
            copiedMessage = string.Empty;
            InvokeAsync(StateHasChanged);
        });
    }
}
```

### Markdown-Style Content

```razor
<OsirionHtmlRenderer HtmlContent="@markdownContent"
                     EnableSyntaxHighlighting="true"
                     SanitizeHtml="true"
                     CssClass="markdown-content" />

@code {
    private string markdownContent = @"
        <h1>Getting Started Guide</h1>
        <p>Follow these steps to set up your project:</p>
        <ol>
            <li>Install the package</li>
            <li>Configure services</li>
            <li>Add components</li>
        </ol>
        
        <h2>Installation</h2>
        <pre><code class=""language-bash"">
        dotnet add package Osirion.Blazor.Core
        </code></pre>
        
        <h2>Configuration</h2>
        <pre><code class=""language-csharp"">
        services.AddOsirionBlazor();
        </code></pre>";
}
```

### Conditional Rendering

```razor
<div class="form-check mb-3">
    <input class="form-check-input" type="checkbox" @bind="showAdvancedFeatures" id="advancedCheck">
    <label class="form-check-label" for="advancedCheck">
        Show Advanced Features
    </label>
</div>

<OsirionHtmlRenderer HtmlContent="@content"
                     EnableSyntaxHighlighting="@showAdvancedFeatures"
                     ShowLineNumbers="@showAdvancedFeatures"
                     ShowCopyButton="@showAdvancedFeatures" />

@code {
    private bool showAdvancedFeatures = false;
    
    private string content = @"
        <h2>Code Example</h2>
        <pre><code class=""language-csharp"">
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOsirionBlazor(options =>
            {
                options.EnableSyntaxHighlighting = true;
                options.ShowLineNumbers = true;
            });
        }
        </code></pre>";
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-html-renderer {
    /* Main container */
}

.osirion-html-renderer pre {
    /* Code block container */
    position: relative;
    background-color: var(--bs-gray-100);
    border-radius: 0.375rem;
    padding: 1rem;
}

.osirion-html-renderer .line-numbers {
    /* Line numbers container */
    counter-reset: linenumber;
}

.osirion-html-renderer .copy-button {
    /* Copy to clipboard button */
    position: absolute;
    top: 0.5rem;
    right: 0.5rem;
}
```

### Custom Styling Example

```css
.custom-renderer {
    border: 1px solid var(--bs-border-color);
    border-radius: 0.5rem;
    padding: 1rem;
}

.custom-renderer pre {
    background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
    border-left: 4px solid var(--bs-primary);
}

.custom-renderer .copy-button {
    background-color: var(--bs-primary);
    color: white;
    border: none;
    border-radius: 0.25rem;
    padding: 0.25rem 0.5rem;
    font-size: 0.75rem;
}
```

## Security Considerations

### HTML Sanitization

The component includes built-in HTML sanitization when `SanitizeHtml` is enabled:

```razor
<!-- Safe: Malicious scripts will be removed -->
<OsirionHtmlRenderer HtmlContent="@userContent" 
                     SanitizeHtml="true" />

@code {
    // This will be sanitized automatically
    private string userContent = @"
        <h1>Safe Content</h1>
        <script>alert('This will be removed');</script>
        <p>This paragraph will remain.</p>";
}
```

### Trusted Content

For trusted content, you can disable sanitization:

```razor
<!-- Only use with trusted content -->
<OsirionHtmlRenderer HtmlContent="@trustedContent" 
                     SanitizeHtml="false" />
```

## Performance Optimization

### Lazy Loading

For large documents, consider implementing lazy loading:

```razor
@if (shouldRender)
{
    <OsirionHtmlRenderer HtmlContent="@largeContent" />
}
else
{
    <button class="btn btn-primary" @onclick="LoadContent">
        Load Content
    </button>
}

@code {
    private bool shouldRender = false;
    private string largeContent = string.Empty;
    
    private async Task LoadContent()
    {
        // Load content asynchronously
        largeContent = await GetLargeContentAsync();
        shouldRender = true;
    }
}
```

## Accessibility Features

- **Screen Reader Support**: Proper ARIA labels and roles
- **Keyboard Navigation**: Copy button is keyboard accessible
- **High Contrast**: Respects system high contrast settings
- **Focus Management**: Proper focus indicators

### Accessibility Example

```razor
<OsirionHtmlRenderer HtmlContent="@content"
                     CssClass="high-contrast"
                     ShowCopyButton="true"
                     aria-label="Code example with syntax highlighting" />
```

## Browser Compatibility

- **Modern Browsers**: Full support for Chrome, Firefox, Safari, Edge
- **Syntax Highlighting**: Requires Prism.js (included automatically)
- **Copy Functionality**: Uses Clipboard API with fallback
- **Mobile Support**: Responsive design with touch-friendly buttons

## Integration Examples

### With Markdown Parser

```razor
@using Markdig

<OsirionHtmlRenderer HtmlContent="@ParseMarkdown(markdownText)" 
                     EnableSyntaxHighlighting="true" />

@code {
    private string markdownText = @"
        # Hello World
        
        ```csharp
        Console.WriteLine(""Hello, World!"");
        ```";
    
    private string ParseMarkdown(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
        return Markdown.ToHtml(markdown, pipeline);
    }
}
```

### With API Content

```razor
<OsirionHtmlRenderer HtmlContent="@apiContent" 
                     OnContentProcessed="HandleContentReady" />

@code {
    private string apiContent = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        apiContent = await Http.GetStringAsync("/api/documentation/content");
    }
    
    private void HandleContentReady(string content)
    {
        // Content is ready and processed
        Logger.LogInformation("Documentation content loaded successfully");
    }
}
```

## Best Practices

1. **Security First**: Always enable HTML sanitization for user-generated content
2. **Performance**: Use lazy loading for large documents
3. **Accessibility**: Provide proper ARIA labels and keyboard navigation
4. **Mobile-Friendly**: Test copy functionality on mobile devices
5. **Theme Support**: Ensure proper contrast in both light and dark themes
6. **Error Handling**: Implement proper error boundaries for content processing
7. **SEO**: Use semantic HTML structure in your content
8. **Caching**: Cache processed content when possible to improve performance

## Common Use Cases

- **Documentation Sites**: Technical documentation with code examples
- **Blog Posts**: Articles with embedded code snippets
- **API Documentation**: Interactive API examples and responses
- **Tutorial Content**: Step-by-step guides with code samples
- **Release Notes**: Formatted changelog content
- **Help Systems**: Context-sensitive help with examples

The OsirionHtmlRenderer component provides a secure and feature-rich solution for displaying HTML content with enhanced code presentation capabilities, making it an essential tool for any Blazor application that needs to render formatted content safely.
