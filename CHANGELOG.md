# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.3.0] - 2025-04-20

### Added
- GitHub CMS service and components for markdown-based content management
- Support for frontmatter parsing in markdown files
- Content caching with configurable duration
- Category and tag management components
- Full-text search functionality
- Directory-based navigation
- Comprehensive unit tests for GitHub CMS

### Changed
- Updated package metadata to include CMS capabilities
- Enhanced documentation with GitHub CMS examples

## [1.2.0] - 2025-04-20

### Added
- ClarityTracker component for Microsoft Clarity analytics integration
- MatomoTracker component for Matomo analytics integration
- TrackerBaseOptions abstract class for common analytics properties
- AnalyticsServiceCollectionExtensions for easy dependency injection configuration
- Comprehensive unit tests for all analytics components
- Detailed documentation for analytics components
- Quick reference guide for component usage
- Migration guide for upgrading

### Changed
- Updated package description to include analytics components
- Added analytics-related tags to package metadata
- Improved README with proper usage examples for all components

## [1.1.0] - 2025-01-18

### Added
- EnhancedNavigationInterceptor component for automatic scroll behavior
- SSR compatibility for all components
- Support for .NET 8, .NET 9, and future versions
- Navigation documentation
- Enhanced examples and usage patterns

### Changed
- Improved package metadata and documentation
- Added navigation-related tags to package metadata

## [1.0.0] - 2025-01-15

### Added
- Core package structure
- Basic documentation and licensing
- Service collection extensions
- Initial project setup

// MIGRATION.md (update)
# Migration Guide

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

### Configuration Options

```csharp
public class GitHubCmsOptions
{
    public string Owner { get; set; } = string.Empty;
    public string Repository { get; set; } = string.Empty;
    public string ContentPath { get; set; } = string.Empty;
    public string Branch { get; set; } = "main";
    public string? ApiToken { get; set; }
    public int CacheDurationMinutes { get; set; } = 30;
    public List SupportedExtensions { get; set; } = new() { ".md", ".markdown" };
}
```

## [1.2.0] - 2025-04-20

### Added
- ClarityTracker component for Microsoft Clarity analytics integration
- MatomoTracker component for Matomo analytics integration
- TrackerBaseOptions abstract class for common analytics properties
- AnalyticsServiceCollectionExtensions for easy dependency injection configuration
- Comprehensive unit tests for all analytics components
- Detailed documentation for analytics components
- Quick reference guide for component usage
- Migration guide for upgrading

### Changed
- Updated package description to include analytics components
- Added analytics-related tags to package metadata
- Improved README with proper usage examples for all components

## [1.1.0] - 2025-01-18

### Added
- EnhancedNavigationInterceptor component for automatic scroll behavior
- SSR compatibility for all components
- Support for .NET 8, .NET 9, and future versions
- Navigation documentation
- Enhanced examples and usage patterns

### Changed
- Improved package metadata and documentation
- Added navigation-related tags to package metadata

## [1.0.0] - 2025-01-15

### Added
- Core package structure
- Basic documentation and licensing
- Service collection extensions
- Initial project setup