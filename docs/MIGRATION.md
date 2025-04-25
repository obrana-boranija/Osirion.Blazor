# Migration Guide

## Upgrading to v1.5.0

### Breaking Changes

Three breaking changes:

1. `osirion-cms.css` has been renamed to `osirion.css` to better reflect its broader purpose as it now includes styles for all components, not just CMS components.

2. Styling options have been moved from `GitHubCmsOptions` to a new dedicated `OsirionStyleOptions` class. If you previously used `GitHubCmsOptions` for styling configuration, you'll need to update your code to use the new class.

3. Service registration extensions have been refactored for better organization and maintainability. The previous extension methods still work but using the new fluent API is recommended.

### New Features

This version introduces CSS framework integration and an improved service registration API:

1. **Framework Integration**: Seamless integration with Bootstrap, Fluent UI, MudBlazor, and Radzen
2. **CssFramework enum**: For specifying which framework to integrate with
3. **Framework-specific CSS variable mappings**: Automatic mapping of Osirion variables to framework variables
4. **Enhanced OsirionStyles component**: Now supports framework integration parameter
5. **New OsirionStyleOptions class**: Dedicated class for styling configuration
6. **Fluent API for service registration**: New `AddOsirionBlazor` method for fluent configuration
7. **Configuration-based registration**: Easily register all services from appsettings.json

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.5.0
   ```

2. Update CSS reference in your `_Host.cshtml` or `App.razor`:
   ```html
   <!-- Before -->
   <link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion-cms.css" />
   
   <!-- After -->
   <link rel="stylesheet" href="_content/Osirion.Blazor/css/osirion.css" />
   ```

3. If you previously configured styling in `GitHubCmsOptions`, update to the new `OsirionStyleOptions`:
   ```csharp
   // Before
   builder.Services.Configure<GitHubCmsOptions>(options => {
       options.UseStyles = true;
       options.CustomVariables = "--osirion-primary-color: #0077cc;";
       options.FrameworkIntegration = CssFramework.Bootstrap;
   });
   
   // After
   builder.Services.AddOsirionStyle(options => {
       options.UseStyles = true;
       options.CustomVariables = "--osirion-primary-color: #0077cc;";
       options.FrameworkIntegration = CssFramework.Bootstrap;
   });
   
   // Or even simpler for framework integration
   builder.Services.AddOsirionStyle(CssFramework.Bootstrap);
   ```

4. Consider updating to the new fluent API for service registration:
   ```csharp
   // Before - individual service registration
   builder.Services.AddGitHubCms(options => { ... });
   builder.Services.AddScrollToTop(options => { ... });
   builder.Services.AddClarityTracker(options => { ... });
   
   // After - fluent API
   builder.Services.AddOsirionBlazor(osirion => {
       osirion
           .AddGitHubCms(options => { ... })
           .AddScrollToTop(options => { ... })
           .AddOsirionStyle(CssFramework.Bootstrap)
           .AddClarityTracker(options => { ... });
   });
   
   // Or from configuration
   builder.Services.AddOsirionBlazor(builder.Configuration);
   ```

5. (Optional) Update appsettings.json if used:
   ```json
   // Before
   {
     "GitHubCms": {
       "UseStyles": true,
       "CustomVariables": "...",
       "FrameworkIntegration": "Bootstrap"
     }
   }
   
   // After
   {
     "OsirionStyle": {
       "UseStyles": true,
       "CustomVariables": "...",
       "FrameworkIntegration": "Bootstrap"
     }
   }
   ```

6. (Optional) Apply framework integration via HTML class:
   ```html
   <html class="osirion-bootstrap-integration">
       <!-- Your app content -->
   </html>
   ```

## Upgrading to v1.4.0

### Breaking Changes

None. Version 1.4.0 is fully backward compatible with v1.3.0.

### New Features

This version introduces the ScrollToTop component:

1. **ScrollToTop**: A customizable button component that allows users to quickly scroll back to the top of the page
2. **ButtonPosition enum**: For positioning UI elements (BottomRight, BottomLeft, TopRight, TopLeft)
3. **ScrollToTopOptions**: Configuration options for DI-based setup

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.4.0
   ```

2. Add the ScrollToTop component to your layout:
   ```razor
   @using Osirion.Blazor.Components.Navigation
   
   <ScrollToTop />
   ```

3. (Optional) Configure ScrollToTop via DI in Program.cs:
   ```csharp
   using Osirion.Blazor.Extensions;
   
   builder.Services.AddScrollToTop(options =>
   {
       options.Position = ButtonPosition.BottomRight;
       options.Behavior = ScrollBehavior.Smooth;
       options.VisibilityThreshold = 300;
       options.Text = "Top"; // Optional text label
   });
   ```

4. (Optional) Configure via appsettings.json:
   ```json
   {
     "ScrollToTop": {
       "Position": "BottomRight",
       "Behavior": "Smooth",
       "VisibilityThreshold": 300,
       "Text": "Top",
       "UseStyles": true
     }
   }
   ```

### Configuration Options

The `ScrollToTopOptions` class provides the following configuration options:

```csharp
public class ScrollToTopOptions
{
    public ScrollBehavior Behavior { get; set; } = ScrollBehavior.Smooth;
    public int VisibilityThreshold { get; set; } = 300;
    public ButtonPosition Position { get; set; } = ButtonPosition.BottomRight;
    public string Title { get; set; } = "Scroll to top";
    public string? Text { get; set; }
    public string? CssClass { get; set; }
    public string? CustomIcon { get; set; }
    public bool UseStyles { get; set; } = true;
    public string? CustomVariables { get; set; }
}
```

### CSS Styling

The ScrollToTop component can be styled using CSS variables:

```css
:root {
  /* Colors */
  --osirion-scroll-background: rgba(0, 0, 0, 0.3);
  --osirion-scroll-hover-background: rgba(0, 0, 0, 0.5);
  --osirion-scroll-color: #ffffff;
  --osirion-scroll-hover-color: #ffffff;
  
  /* Sizes */
  --osirion-scroll-size: 40px;
  --osirion-scroll-margin: 20px;
  --osirion-scroll-border-radius: 4px;
  --osirion-scroll-z-index: 1000;
}
```

## Upgrading to v1.3.0

### Breaking Changes

None. Version 1.3.0 is fully backward compatible with v1.2.0.

### New Features

This version introduces GitHub CMS components for markdown-based content management:

1. **ContentList**: Display lists of content with filtering options
2. **ContentView**: Display a single content item
3. **CategoriesList**: List content categories
4. **TagCloud**: Show content tags
5. **SearchBox**: Provide search functionality
6. **DirectoryNavigation**: Create navigation based on repository structure

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.3.0
   ```

2. Add new using statements to `_Imports.razor`:
   ```razor
   @using Osirion.Blazor.Components.GitHubCms
   @using Osirion.Blazor.Services.GitHub
   @using Osirion.Blazor.Models.Cms
   ```

3. Configure GitHub CMS service in `Program.cs`:
   ```csharp
   builder.Services.AddGitHubCms(options =>
   {
       options.Owner = "your-github-username";
       options.Repository = "your-content-repo";
       options.ContentPath = "content";
       options.Branch = "main";
   });
   ```

4. Add GitHub CMS components to your pages:
   ```razor
   <ContentList Directory="blog" />
   <ContentView Path="blog/my-post.md" />
   <CategoriesList />
   <TagCloud />
   <SearchBox />
   <DirectoryNavigation />
   ```

## Upgrading to v1.2.0

### Breaking Changes

None. Version 1.2.0 is fully backward compatible with v1.1.0.

### New Features

This version introduces analytics components:

1. **ClarityTracker**: Microsoft Clarity integration
2. **MatomoTracker**: Matomo analytics integration
3. **AnalyticsServiceCollectionExtensions**: DI configuration helpers

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.2.0
   ```

2. Add new using statements to `_Imports.razor`:
   ```razor
   @using Osirion.Blazor.Components.Analytics
   @using Osirion.Blazor.Components.Analytics.Options
   ```

3. Configure analytics services in `Program.cs` (optional):
   ```csharp
   builder.Services.AddClarityTracker(builder.Configuration);
   builder.Services.AddMatomoTracker(builder.Configuration);
   ```

4. Add analytics components to your layout:
   ```razor
   @inject IOptions<ClarityOptions>? ClarityOptions
   @inject IOptions<MatomoOptions>? MatomoOptions

   @if (ClarityOptions?.Value != null)
   {
       <ClarityTracker Options="@ClarityOptions.Value" />
   }

   @if (MatomoOptions?.Value != null)
   {
       <MatomoTracker Options="@MatomoOptions.Value" />
   }
   ```

## Upgrading from v1.0.0 to v1.1.0

### Breaking Changes

None. Version 1.1.0 is fully backward compatible with v1.0.0.

### New Features

This version introduces the EnhancedNavigationInterceptor component.

### Migration Steps

1. Update your package reference:
   ```bash
   dotnet add package Osirion.Blazor --version 1.1.0
   ```

2. Add the navigation component to your layout:
   ```razor
   <EnhancedNavigationInterceptor Behavior="ScrollBehavior.Smooth" />
   ```