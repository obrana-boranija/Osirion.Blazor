# Osirion.Blazor CMS Admin Components Documentation

This documentation covers the Admin module of Osirion.Blazor CMS, which provides administrative UI components for managing content in GitHub repositories. The Admin module is designed for content editors and administrators to create, edit, and publish content through a web interface.

The Admin module includes components for authentication, dashboard, repository management, content editing, navigation, layouts, and security. These components integrate with GitHub APIs and the CMS domain services to provide a complete content management system.

## Component List by Feature

The Admin module is organized into the following features:

### Authentication
- Login

### Dashboard
- CmsAdminDashboard

### Repository Management
- BranchSelector
- RepositorySelector

### Navigation
- DefaultNavigation

### Content Editor
- ContentEditor
- MetadataPreview
- MetadataEditor
- SeoMetadataForm
- MarkdownEditorWithPreview
- CommitPanel
- BasicMetadataForm
- SocialMetadataForm
- AdvancedMetadataForm
- TagInput

### Layouts
- AdminPage
- AdminLayout
- CmsLayoutEditor

### Security
- AuthGuard

Each component is described below with usage examples.

## Authentication Components

### Login

The `Login` component provides a login form for admin authentication.

#### Parameters

- `OnLogin`: EventCallback<LoginModel> - Login event handler
- `Title`: string - Form title (default: "Admin Login")
- `UsernamePlaceholder`: string - Username placeholder
- `PasswordPlaceholder`: string - Password placeholder
- `LoginButtonText`: string - Login button text (default: "Login")

#### Example

```razor
<Login OnLogin="@HandleLogin" />
```

## Dashboard Components

### CmsAdminDashboard

The `CmsAdminDashboard` component displays the main admin dashboard.

#### Parameters

- `Stats`: DashboardStats - Dashboard statistics
- `RecentActivity`: IEnumerable<ActivityItem> - Recent activity items
- `QuickActions`: IEnumerable<ActionItem> - Quick action items

#### Example

```razor
<CmsAdminDashboard 
    Stats="@stats" 
    RecentActivity="@activity" 
    QuickActions="@actions" />
```

## Repository Management Components

### BranchSelector

The `BranchSelector` component allows selecting a repository branch.

#### Parameters

- `Repository`: string - Repository name
- `SelectedBranch`: string - Currently selected branch
- `Branches`: IEnumerable<string> - Available branches
- `OnBranchChanged`: EventCallback<string> - Branch change event

#### Example

```razor
<BranchSelector 
    Repository="my-repo" 
    SelectedBranch="@selectedBranch" 
    Branches="@branches" 
    OnBranchChanged="@HandleBranchChange" />
```

### RepositorySelector

The `RepositorySelector` component allows selecting a repository.

#### Parameters

- `SelectedRepository`: string - Currently selected repository
- `Repositories`: IEnumerable<RepositoryInfo> - Available repositories
- `OnRepositoryChanged`: EventCallback<string> - Repository change event

#### Example

```razor
<RepositorySelector 
    SelectedRepository="@selectedRepo" 
    Repositories="@repos" 
    OnRepositoryChanged="@HandleRepoChange" />
```

## Navigation Components

### DefaultNavigation

The `DefaultNavigation` component provides default admin navigation.

#### Parameters

- `MenuItems`: IEnumerable<MenuItem> - Navigation menu items
- `CurrentPath`: string - Current path
- `Collapsed`: bool - Navigation collapsed state

#### Example

```razor
<DefaultNavigation 
    MenuItems="@menuItems" 
    CurrentPath="@currentPath" />
```

## Content Editor Components

### ContentEditor

The `ContentEditor` component is the main content editor interface.

#### Parameters

- `ContentItem`: ContentItem - The content item being edited
- `OnSave`: EventCallback<ContentItem> - Save event handler
- `OnPublish`: EventCallback<ContentItem> - Publish event handler
- `OnPreview`: EventCallback<ContentItem> - Preview event handler

#### Example

```razor
<ContentEditor 
    ContentItem="@contentItem" 
    OnSave="@HandleSave" 
    OnPublish="@HandlePublish" />
```

### MetadataPreview

The `MetadataPreview` component displays a preview of content metadata.

#### Parameters

- `Metadata`: ContentMetadata - Content metadata
- `ShowAll`: bool - Show all metadata fields (default: false)

#### Example

```razor
<MetadataPreview Metadata="@metadata" ShowAll="true" />
```

### MetadataEditor

The `MetadataEditor` component provides an editor for content metadata.

#### Parameters

- `Metadata`: ContentMetadata - Content metadata
- `OnMetadataChanged`: EventCallback<ContentMetadata> - Metadata change event

#### Example

```razor
<MetadataEditor 
    Metadata="@metadata" 
    OnMetadataChanged="@HandleMetadataChange" />
```

### SeoMetadataForm

The `SeoMetadataForm` component provides a form for SEO metadata.

#### Parameters

- `SeoData`: SeoMetadata - SEO metadata
- `OnChanged`: EventCallback<SeoMetadata> - Change event

#### Example

```razor
<SeoMetadataForm 
    SeoData="@seoData" 
    OnChanged="@HandleSeoChange" />
```

### MarkdownEditorWithPreview

The `MarkdownEditorWithPreview` component provides a markdown editor with live preview.

#### Parameters

- `Content`: string - Markdown content
- `OnContentChanged`: EventCallback<string> - Content change event
- `Height`: string - Editor height (default: "400px")
- `ShowToolbar`: bool - Show toolbar (default: true)

#### Example

```razor
<MarkdownEditorWithPreview 
    Content="@markdownContent" 
    OnContentChanged="@HandleContentChange" />
```

### CommitPanel

The `CommitPanel` component provides a panel for committing changes.

#### Parameters

- `Changes`: IEnumerable<FileChange> - File changes
- `OnCommit`: EventCallback<CommitInfo> - Commit event
- `Branch`: string - Target branch

#### Example

```razor
<CommitPanel 
    Changes="@changes" 
    OnCommit="@HandleCommit" 
    Branch="main" />
```

### BasicMetadataForm

The `BasicMetadataForm` component provides a form for basic metadata.

#### Parameters

- `Metadata`: BasicMetadata - Basic metadata
- `OnChanged`: EventCallback<BasicMetadata> - Change event

#### Example

```razor
<BasicMetadataForm 
    Metadata="@basicMetadata" 
    OnChanged="@HandleBasicChange" />
```

### SocialMetadataForm

The `SocialMetadataForm` component provides a form for social media metadata.

#### Parameters

- `Metadata`: SocialMetadata - Social metadata
- `OnChanged`: EventCallback<SocialMetadata> - Change event

#### Example

```razor
<SocialMetadataForm 
    Metadata="@socialMetadata" 
    OnChanged="@HandleSocialChange" />
```

### AdvancedMetadataForm

The `AdvancedMetadataForm` component provides a form for advanced metadata.

#### Parameters

- `Metadata`: AdvancedMetadata - Advanced metadata
- `OnChanged`: EventCallback<AdvancedMetadata> - Change event

#### Example

```razor
<AdvancedMetadataForm 
    Metadata="@advancedMetadata" 
    OnChanged="@HandleAdvancedChange" />
```

### TagInput

The `TagInput` component provides an input for content tags.

#### Parameters

- `Tags`: IEnumerable<string> - Current tags
- `OnTagsChanged`: EventCallback<IEnumerable<string>> - Tags change event
- `Placeholder`: string - Input placeholder
- `MaxTags`: int - Maximum number of tags

#### Example

```razor
<TagInput 
    Tags="@tags" 
    OnTagsChanged="@HandleTagsChange" 
    Placeholder="Add tags..." />
```

## Layout Components

### AdminPage

The `AdminPage` component provides a basic admin page layout.

#### Parameters

- `Title`: string - Page title
- `ShowSidebar`: bool - Show sidebar (default: true)
- `SidebarContent`: RenderFragment - Sidebar content

#### Example

```razor
<AdminPage Title="Content Management">
    <SidebarContent>
        <nav>Admin menu</nav>
    </SidebarContent>
    <p>Page content</p>
</AdminPage>
```

### AdminLayout

The `AdminLayout` component provides the main admin layout.

#### Parameters

- `HeaderContent`: RenderFragment - Header content
- `NavigationContent`: RenderFragment - Navigation content
- `FooterContent`: RenderFragment - Footer content

#### Example

```razor
<AdminLayout>
    <HeaderContent>
        <h1>Admin Panel</h1>
    </HeaderContent>
    <NavigationContent>
        <DefaultNavigation MenuItems="@menuItems" />
    </NavigationContent>
    <p>Main content</p>
</AdminLayout>
```

### CmsLayoutEditor

The `CmsLayoutEditor` component provides a layout editor for CMS pages.

#### Parameters

- `Layout`: PageLayout - Current layout
- `OnLayoutChanged`: EventCallback<PageLayout> - Layout change event
- `AvailableComponents`: IEnumerable<ComponentInfo> - Available components

#### Example

```razor
<CmsLayoutEditor 
    Layout="@pageLayout" 
    OnLayoutChanged="@HandleLayoutChange" 
    AvailableComponents="@components" />
```

## Security Components

### AuthGuard

The `AuthGuard` component guards admin routes requiring authentication.

#### Parameters

- `ChildContent`: RenderFragment - Protected content
- `RequiredRole`: string - Required user role
- `RedirectUrl`: string - Redirect URL for unauthorized users

#### Example

```razor
<AuthGuard RequiredRole="Admin" RedirectUrl="/login">
    <p>Protected admin content</p>
</AuthGuard>
```
