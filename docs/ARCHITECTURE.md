# Architecture Overview

[![Documentation](https://img.shields.io/badge/Documentation-Architecture-blue)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/ARCHITECTURE.md)
[![Version](https://img.shields.io/nuget/v/Osirion.Blazor)](https://www.nuget.org/packages/Osirion.Blazor)

This document provides a comprehensive overview of the Osirion.Blazor architecture, design patterns, and organizational principles.

## Core Architecture Principles

### 1. Modular Design
Osirion.Blazor follows a modular architecture where each module serves a specific purpose and can be used independently:

```
Osirion.Blazor (Meta Package)
??? Osirion.Blazor.Core (Foundation)
??? Osirion.Blazor.Analytics (Analytics Integration)
??? Osirion.Blazor.Navigation (Navigation Components)
??? Osirion.Blazor.Cms (Content Management)
??? Osirion.Blazor.Cms.Core (CMS Abstractions)
??? Osirion.Blazor.Cms.Admin (Admin Interface)
??? Osirion.Blazor.Cms.Infrastructure (CMS Infrastructure)
??? Osirion.Blazor.Theming (Styling System)
```

### 2. SSR-First Design
All components are designed with Server-Side Rendering as the primary target:
- No JavaScript dependencies for core functionality
- Progressive enhancement for interactive features
- Efficient rendering on server and client
- SEO-optimized output

### 3. Provider Pattern
Services use the provider pattern for extensibility:
- **Content Providers**: GitHub, FileSystem, and custom implementations
- **Analytics Providers**: Multiple analytics services
- **Theme Providers**: Various CSS framework integrations

## Project Structure

### Source Organization

```
src/
??? Osirion.Blazor/                    # Meta package
??? Osirion.Blazor.Core/               # Core components and utilities
?   ??? Components/                    # Reusable UI components
?   ?   ??? Layout/                    # Layout components
?   ?   ??? Navigation/                # Navigation components
?   ?   ??? Content/                   # Content display components
?   ?   ??? Interactive/               # Interactive components
?   ??? Base/                          # Base classes and utilities
?   ??? Models/                        # Data models and DTOs
?   ??? Extensions/                    # Extension methods
??? Osirion.Blazor.Analytics/          # Analytics integration
?   ??? Components/                    # Analytics components
?   ??? Providers/                     # Analytics providers
?   ??? Services/                      # Analytics services
??? Osirion.Blazor.Navigation/         # Navigation components
??? Osirion.Blazor.Cms/                # Content management
??? Osirion.Blazor.Cms.Core/           # CMS abstractions
??? Osirion.Blazor.Cms.Admin/          # Admin interface
??? Osirion.Blazor.Cms.Infrastructure/ # CMS infrastructure
??? Osirion.Blazor.Theming/            # Theming system
```

### Naming Conventions

- **Components**: PascalCase (e.g., `HeroSection`, `OsirionBreadcrumbs`)
- **Services**: Interface with `I` prefix (e.g., `IContentProvider`)
- **Options**: Suffix with `Options` (e.g., `GitHubOptions`)
- **Extensions**: Suffix with `Extensions` (e.g., `ServiceCollectionExtensions`)

## Component Architecture

### Component Hierarchy

```
OsirionComponentBase (Base Class)
??? Layout Components
?   ??? HeroSection
?   ??? OsirionPageLayout
?   ??? OsirionFooter
?   ??? OsirionStickySidebar
??? Navigation Components
?   ??? OsirionBreadcrumbs
?   ??? Menu/MenuItem/MenuGroup
?   ??? DirectoryNavigation
??? Content Components
?   ??? ContentList
?   ??? ContentView
?   ??? MarkdownRenderer
?   ??? OsirionHtmlRenderer
??? Interactive Components
    ??? OsirionCookieConsent
    ??? InfiniteLogoCarousel
    ??? SearchBox
```

### Component Design Patterns

#### 1. Composition Pattern
Components are designed for composition and reusability:

```razor
<OsirionPageLayout HasSidebar="true">
    <HeaderContent>
        <OsirionBreadcrumbs />
    </HeaderContent>
    <SidebarContent>
        <CategoriesList />
        <TagCloud />
    </SidebarContent>
    <MainContent>
        <ContentList />
    </MainContent>
</OsirionPageLayout>
```

#### 2. Parameter Validation
Components validate parameters and provide meaningful error messages:

```csharp
protected override void OnParametersSet()
{
    if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Subtitle))
    {
        throw new ArgumentException("Either Title or Subtitle must be provided");
    }
    base.OnParametersSet();
}
```

#### 3. CSS Isolation
Each component includes isolated CSS for styling:

```
ComponentName/
??? ComponentName.razor       # Markup
??? ComponentName.razor.cs    # Code-behind
??? ComponentName.razor.css   # Isolated styles
```

## Service Architecture

### Dependency Injection Patterns

#### 1. Fluent Registration API
```csharp
builder.Services.AddOsirionBlazor(osirion => {
    osirion
        .AddGitHubCms(options => { ... })
        .AddScrollToTop()
        .AddClarityTracker(options => { ... })
        .AddOsirionStyle(CssFramework.Bootstrap);
});
```

#### 2. Configuration-Based Registration
```csharp
builder.Services.AddOsirionBlazor(builder.Configuration);
```

#### 3. Manual Registration
```csharp
builder.Services.AddGitHubCms(options => { ... });
builder.Services.AddScrollToTop();
```

### Service Lifetimes

- **Singleton**: Configuration services, theme services, analytics services
- **Scoped**: Content providers, navigation services
- **Transient**: Utility services, factory instances

## Data Flow Architecture

### Content Management Flow

```
GitHub Repository
    ?
GitHubApiClient
    ?
GitHubContentRepository
    ?
ContentProvider
    ?
ContentList/ContentView Components
    ?
UI Rendering
```

### Analytics Flow

```
User Interaction
    ?
Analytics Service
    ?
Analytics Provider (Clarity/Matomo/GA4)
    ?
External Analytics Platform
```

## Caching Strategy

### Multi-Level Caching

1. **In-Memory Cache**: Fast access to frequently used content
2. **Response Cache**: HTTP response caching for static content
3. **Component Cache**: Rendered component output caching

### Cache Invalidation

- **Time-based**: Configurable expiration times
- **Event-based**: Webhook-triggered cache invalidation
- **Manual**: Administrative cache clearing

## Error Handling

### Exception Hierarchy

```
OsirionException (Base)
??? ContentProviderException
??? ContentValidationException
??? DirectoryNotFoundException
??? ContentNotFoundException
```

### Error Recovery

- Graceful degradation for missing content
- Fallback content for failed API calls
- User-friendly error messages

## Security Considerations

### Content Security

1. **HTML Sanitization**: All user content is sanitized before rendering
2. **Access Control**: Provider-level access controls
3. **Token Management**: Secure API token handling

### Privacy Compliance

1. **GDPR Compliance**: Cookie consent management
2. **Data Minimization**: Only necessary data collection
3. **User Control**: Granular privacy settings

## Performance Optimization

### Rendering Performance

1. **Server-Side Rendering**: Primary rendering strategy
2. **Component Caching**: Aggressive caching of rendered output
3. **Lazy Loading**: Deferred loading of non-critical content

### Memory Management

1. **Disposable Services**: Proper resource cleanup
2. **Weak References**: Prevent memory leaks in long-running apps
3. **Cache Eviction**: Automatic cleanup of stale cache entries

## Extensibility Points

### Custom Providers

Implement custom providers for various services:

```csharp
public class CustomContentProvider : IContentProvider
{
    public string ProviderId => "custom";
    // Implementation...
}
```

### Custom Components

Extend base classes for custom components:

```csharp
public class CustomComponent : OsirionComponentBase
{
    // Custom implementation
}
```

### Custom Transformers

Implement content transformation:

```csharp
public class CustomTransformer : IContentTransformer
{
    public Task<string> TransformAsync(string content, ContentItem item)
    {
        // Transform content
        return Task.FromResult(transformedContent);
    }
}
```

## Testing Architecture

### Testing Strategy

1. **Unit Tests**: Individual component and service testing
2. **Integration Tests**: Multi-component interaction testing
3. **End-to-End Tests**: Full application workflow testing

### Testing Tools

- **xUnit**: Primary testing framework
- **bUnit**: Blazor component testing
- **Shouldly**: Assertion library
- **NSubstitute**: Mocking framework

### Test Organization

```
tests/
??? Osirion.Blazor.Core.Tests/
??? Osirion.Blazor.Analytics.Tests/
??? Osirion.Blazor.Cms.Tests/
??? Osirion.Blazor.E2ETests/
```

## Configuration Management

### Configuration Hierarchy

1. **Default Values**: Sensible defaults for all options
2. **appsettings.json**: Application-level configuration
3. **Environment Variables**: Environment-specific overrides
4. **Code Configuration**: Programmatic configuration

### Configuration Validation

- Schema validation for configuration objects
- Runtime validation with meaningful error messages
- Configuration change detection and hot reload

## Deployment Considerations

### Platform Support

- **Azure App Service**: Full support with optimizations
- **Azure Static Web Apps**: SSG deployment support
- **Docker**: Containerized deployment
- **On-Premises**: Self-hosted deployment options

### Build Optimization

- **Tree Shaking**: Unused code elimination
- **Bundle Optimization**: Efficient asset bundling
- **Precompilation**: Ahead-of-time compilation support

## Future Architecture Considerations

### Planned Enhancements

1. **Microservices Support**: Distributed content management
2. **Real-time Updates**: SignalR integration for live updates
3. **Edge Computing**: CDN integration for global performance
4. **Machine Learning**: Content recommendation and optimization

### Backward Compatibility

- Semantic versioning for all packages
- Deprecation warnings before breaking changes
- Migration guides for major version updates
- Legacy provider support during transition periods