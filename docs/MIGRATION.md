# Migration Guide

## Upgrading to v1.3.0

### Breaking Changes

None. Version 1.3.0 is fully backward compatible with v1.2.0.

### New Features

This version introduces GitHub CMS components for markdown-based content management:

1. **GitHubCmsService**: Service for fetching and caching content from GitHub repositories
2. **ContentList**: Display lists of content with filtering options
3. **ContentView**: Display a single content item
4. **CategoriesList**: List content categories
5. **TagCloud**: Show content tags
6. **SearchBox**: Provide search functionality
7. **DirectoryNavigation**: Create navigation based on repository structure
8. **OsirionStyles**: Default styling for GitHub CMS components

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
   <!-- Add default styling -->
   <OsirionStyles />
   
   <!-- Content list -->
   <ContentList Directory="blog" />
   
   <!-- Content view -->
   <ContentView Path="blog/my-post.md" />
   
   <!-- Categories and tags -->
   <CategoriesList />
   <TagCloud MaxTags="20" />
   
   <!-- Search -->
   <SearchBox Placeholder="Search..." />
   
   <!-- Directory navigation -->
   <DirectoryNavigation CurrentDirectory="blog" />
   ```

### Configuration Options

The `GitHubCmsOptions` class provides the following configuration options:

```csharp
public class GitHubCmsOptions
{
    // Required properties
    public string Owner { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
    
    // Optional properties with defaults
    public string ContentPath { get; set; } = string.Empty;
    public string Branch { get; set; } = "main";
    public string? ApiToken { get; set; }
    public int CacheDurationMinutes { get; set; } = 30;
    public List<string> SupportedExtensions { get; set; } = new() { ".md", ".markdown" };
    public bool? UseStyles { get; set; }
    public string? CustomVariables { get; set; }
}
```

You can configure these options through `appsettings.json`:

```json
{
  "GitHubCms": {
    "Owner": "your-github-username",
    "Repository": "your-content-repo",
    "ContentPath": "content",
    "Branch": "main",
    "ApiToken": "",
    "CacheDurationMinutes": 30,
    "SupportedExtensions": [".md", ".markdown"],
    "UseStyles": true,
    "CustomVariables": "--osirion-primary-color: #3b82f6;"
  }
}
```

### Markdig Package Dependency

The GitHub CMS functionality requires the Markdig package. This dependency is automatically included when you install Osirion.Blazor 1.3.0, but if you're encountering issues, you can manually add the package:

```bash
dotnet add package Markdig --version 0.41.0
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

### Configuration Options

For analytics, you can now use either configuration-based or programmatic setup:

#### Configuration-based (appsettings.json)
```json
{
  "Clarity": {
    "TrackerUrl": "https://www.clarity.ms/tag/",
    "SiteId": "your-site-id",
    "Track": true
  },
  "Matomo": {
    "TrackerUrl": "//analytics.example.com/",
    "SiteId": "1",
    "Track": true
  }
}
```

#### Programmatic
```csharp
builder.Services.AddClarityTracker(options =>
{
    options.TrackerUrl = "https://www.clarity.ms/tag/";
    options.SiteId = "your-site-id";
    options.Track = true;
});
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

## Common Issues

### Analytics Components Not Rendering

Ensure that:
1. Configuration is properly set up in appsettings.json or Program.cs
2. All required properties (TrackerUrl, SiteId) are provided
3. Track property is set to true
4. Components are properly injected in your layout

### EnhancedNavigationInterceptor Not Working

Verify that:
1. You're using Blazor with enhanced navigation enabled
2. The component is placed in your main layout
3. Only one instance of the component exists in your application

### GitHub CMS Not Loading Content

Check that:
1. The GitHub repository exists and is accessible
2. The Owner and Repository properties are correctly configured
3. The content path exists in the repository
4. You've provided an API token for private repositories
5. The markdown files have valid frontmatter
6. You're not exceeding GitHub API rate limits