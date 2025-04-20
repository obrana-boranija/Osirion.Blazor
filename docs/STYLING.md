# Styling Guide for Osirion.Blazor

Osirion.Blazor provides a flexible styling system using CSS variables (custom properties) to easily customize the appearance of all components.

## Basic Styling Options

### 1. Direct CSS Reference

Add the styles directly in your `App.razor` or `_Host.cshtml` file:

```html
<!-- Reference the default styles -->
<link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />

<!-- Optionally override variables -->
<style>
    :root {
        --osirion-primary-color: #0077cc;
        --osirion-border-radius: 0.25rem;
    }
</style>
```

### 2. Using the OsirionStyles Component

Include the component in your layout:

```razor
@using Osirion.Blazor.Components.GitHubCms

<!-- In your layout component -->
<OsirionStyles />

<!-- With custom variables -->
<OsirionStyles CustomVariables="--osirion-primary-color: #0077cc;" />
```

### 3. Configuration-Based

Configure styling options in `Program.cs`:

```csharp
// In Program.cs
builder.Services.Configure<GitHubCmsOptions>(options => {
    options.UseStyles = true;
    options.CustomVariables = "--osirion-primary-color: #0077cc;";
});
```

Then simply include the component without parameters:

```razor
<OsirionStyles />
```

## CSS Variables Reference

Osirion.Blazor uses a comprehensive set of CSS variables that you can customize:

### Colors
```css
--osirion-primary-color: #2563eb;
--osirion-primary-hover-color: #1d4ed8;
--osirion-text-color: #374151;
--osirion-light-text-color: #6b7280;
--osirion-border-color: #e5e7eb;
--osirion-background-color: #ffffff;
--osirion-light-background-color: #f3f4f6;
--osirion-tag-background-color: #f3f4f6;
--osirion-tag-text-color: #4b5563;
--osirion-category-background-color: #e0f2fe;
--osirion-category-text-color: #0369a1;
--osirion-active-background-color: #e0f2fe;
--osirion-active-text-color: #0369a1;
```

### Sizes
```css
--osirion-border-radius: 0.5rem;
--osirion-small-border-radius: 0.375rem;
--osirion-tag-border-radius: 999px;
--osirion-gap: 1.5rem;
--osirion-small-gap: 0.5rem;
--osirion-padding: 1.5rem;
--osirion-small-padding: 0.5rem;
```

### Typography
```css
--osirion-font-size: 1rem;
--osirion-small-font-size: 0.875rem;
--osirion-tiny-font-size: 0.75rem;
--osirion-title-font-size: 1.25rem;
--osirion-large-title-font-size: 2.5rem;
--osirion-line-height: 1.7;
```

### Effects
```css
--osirion-transition-speed: 0.2s;
--osirion-box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
```

### Layout
```css
--osirion-content-width: 800px;
--osirion-card-min-width: 300px;
--osirion-image-height: 200px;
```

## Theming Examples

### Basic Customization

```css
:root {
  --osirion-primary-color: #0077cc;
  --osirion-border-radius: 0.25rem;
  --osirion-font-size: 1.1rem;
}
```

### Dark Theme

```css
:root {
  --osirion-primary-color: #60a5fa;
  --osirion-primary-hover-color: #93c5fd;
  --osirion-text-color: #e5e7eb;
  --osirion-light-text-color: #9ca3af;
  --osirion-border-color: #4b5563;
  --osirion-background-color: #1f2937;
  --osirion-light-background-color: #374151;
  --osirion-tag-background-color: #374151;
  --osirion-tag-text-color: #e5e7eb;
  --osirion-category-background-color: #1e40af;
  --osirion-category-text-color: #e5e7eb;
  --osirion-active-background-color: #1e40af;
  --osirion-active-text-color: #e5e7eb;
}
```

### Theme Switching

You can implement theme switching by toggling CSS classes:

```css
/* Base theme */
:root {
  --osirion-primary-color: #2563eb;
  --osirion-background-color: #ffffff;
  /* Other variables... */
}

/* Dark theme */
.dark-theme {
  --osirion-primary-color: #60a5fa;
  --osirion-background-color: #1f2937;
  /* Override other variables... */
}
```

Then in your Blazor app:

```razor
<div class="@(IsDarkTheme ? "dark-theme" : "")">
    <ContentList />
</div>

@code {
    private bool IsDarkTheme { get; set; }
    
    private void ToggleTheme()
    {
        IsDarkTheme = !IsDarkTheme;
    }
}
```

## Best Practices

1. **Override Only What You Need**: Only override the variables you want to change to minimize maintenance.

2. **Create Theme Files**: For major themes, create separate CSS files to organize your variables.

3. **Test Responsive Behavior**: Ensure your customizations work on different screen sizes.

4. **Consider Accessibility**: Maintain sufficient contrast when changing colors.

5. **Apply Themes Consistently**: Apply theme classes at the highest possible level in your component hierarchy.