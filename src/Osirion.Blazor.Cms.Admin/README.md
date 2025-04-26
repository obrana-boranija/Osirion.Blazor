# Osirion.Blazor.Cms.Admin

[![NuGet](https://img.shields.io/nuget/v/Osirion.Blazor.Cms.Admin)](https://www.nuget.org/packages/Osirion.Blazor.Cms.Admin)
[![License](https://img.shields.io/github/license/obrana-boranija/Osirion.Blazor)](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt)

Content Management System (CMS) Administration module for Osirion.Blazor, providing a robust, flexible admin interface for managing content across different providers.

## Features

- **Unified Content Management**: Manage content from various providers (GitHub, FileSystem, etc.)
- **Rich Editing Experience**: Advanced markdown editor with live preview
- **SSR Compatible**: Works with Server-Side Rendering and Static SSG
- **Flexible Authentication**: Supports multiple authentication strategies
- **Multi-Provider Support**: Seamless integration with existing Osirion.Blazor CMS providers
- **Real-time Collaboration**: Draft sharing and collaborative editing features
- **Comprehensive Access Control**: Role-based permissions and content governance

## Installation

```bash
dotnet add package Osirion.Blazor.Cms.Admin
```

## Quick Start

### Configuration

```csharp
// In Program.cs
builder.Services.AddOsirionCmsAdmin(options => 
{
    options.UseGitHubProvider(github => 
    {
        github.Owner = "your-username";
        github.Repository = "your-content-repo";
    });

    options.ConfigureAuthentication(auth => 
    {
        auth.UseGitHubAuthentication();
        // Or use custom authentication
    });
});
```

### Basic Usage

```razor
@page "/admin"
@using Osirion.Blazor.Cms.Admin.Components

<CmsAdminDashboard>
    <ContentEditor />
    <MediaLibrary />
    <UserManagement />
</CmsAdminDashboard>
```

## Core Components

### ContentEditor
- Advanced markdown editing
- Front matter management
- Live preview
- Draft saving
- Version history

### MediaLibrary
- Upload and manage media files
- Image optimization
- Folder organization
- File metadata management

### UserManagement
- Role-based access control
- User invitation
- Permissions configuration

### Analytics Dashboard
- Content performance tracking
- User engagement metrics
- SEO insights

## Authentication Strategies

### GitHub OAuth
```csharp
options.ConfigureAuthentication(auth => 
{
    auth.UseGitHubAuthentication(github => 
    {
        github.ClientId = "your-client-id";
        github.ClientSecret = "your-client-secret";
    });
});
```

### Custom Authentication
```csharp
options.ConfigureAuthentication(auth => 
{
    auth.UseCustomProvider(custom => 
    {
        custom.ConfigureServices(services => { ... });
        custom.ConfigureMiddleware(app => { ... });
    });
});
```

## Configuration Options

```csharp
builder.Services.AddOsirionCmsAdmin(options => 
{
    // Provider configuration
    options.UseGitHubProvider(github => { ... });
    options.UseFileSystemProvider(fs => { ... });

    // Authentication
    options.ConfigureAuthentication(auth => { ... });

    // UI Customization
    options.ConfigureTheme(theme => 
    {
        theme.UseDarkMode();
        theme.SetPrimaryColor("#007b31");
    });

    // Content Restrictions
    options.ConfigureContentRules(rules => 
    {
        rules.RequireApproval();
        rules.SetMaximumDraftAge(30);
    });
});
```

## Localization

```csharp
builder.Services.AddOsirionCmsAdmin(options => 
{
    options.UseLocalization(localization => 
    {
        localization.AddSupportedCultures("en-US", "fr-FR", "de-DE");
        localization.SetDefaultCulture("en-US");
    });
});
```

## Security Best Practices

- Implement role-based access control
- Use secure authentication providers
- Enable two-factor authentication
- Audit logging for all administrative actions

## Extensibility

Custom providers and components can be added easily:

```csharp
public class CustomContentProvider : IContentProvider 
{
    // Implement provider logic
}

builder.Services.AddOsirionCmsAdmin(options => 
{
    options.AddProvider<CustomContentProvider>();
});
```

## Documentation

For more detailed documentation, see [CMS Admin Documentation](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/docs/CMS_ADMIN.md).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/LICENSE.txt) file for details.

## Contributing

We welcome contributions! Please see our [contributing guidelines](https://github.com/obrana-boranija/Osirion.Blazor/blob/master/CONTRIBUTING.md).