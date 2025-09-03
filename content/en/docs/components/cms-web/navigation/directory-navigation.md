---
title: "Directory Navigation - Osirion Blazor CMS"
description: "Hierarchical directory navigation component with expandable subdirectories, item counts, and customizable URL formatting for file-system-like navigation."
category: "CMS Web Components"
subcategory: "Navigation"
tags: ["cms", "navigation", "directories", "hierarchy", "tree-view", "file-system"]
created: "2025-01-26"
modified: "2025-01-26"
author: "Osirion Team"
status: "current"
version: "1.0"
slug: "directory-navigation"
section: "components"
layout: "component"
seo:
  title: "Directory Navigation Component | Osirion Blazor CMS Documentation"
  description: "Learn how to implement hierarchical directory navigation with expandable subdirectories and custom URL formatting."
  keywords: ["Blazor", "CMS", "navigation", "directories", "hierarchy", "tree navigation", "file system"]
  canonical: "/docs/components/cms.web/navigation/directory-navigation"
  image: "/images/components/directory-navigation-preview.jpg"
navigation:
  parent: "CMS Web Navigation"
  order: 3
breadcrumb:
  - text: "Documentation"
    link: "/docs"
  - text: "Components"
    link: "/docs/components"
  - text: "CMS Web"
    link: "/docs/components/cms.web"
  - text: "Navigation"
    link: "/docs/components/cms.web/navigation"
  - text: "Directory Navigation"
    link: "/docs/components/cms.web/navigation/directory-navigation"
---

# Directory Navigation Component

The **DirectoryNavigation** component provides a hierarchical, tree-like navigation interface for directory structures. It supports expandable subdirectories, active state highlighting, and customizable URL formatting, making it perfect for file browsers, documentation sites, and any application requiring structured navigation.

## Overview

This component creates an intuitive directory navigation experience similar to file explorers, with support for nested directories, item counts, and interactive expansion/collapse functionality. It's designed to handle complex hierarchical structures while maintaining performance and usability.

## Key Features

- **Hierarchical Structure**: Multi-level directory navigation with parent-child relationships
- **Expandable Subdirectories**: Interactive expand/collapse functionality
- **Active State Tracking**: Visual indication of current directory
- **Item Count Display**: Shows number of items in each directory
- **Custom URL Formatting**: Flexible URL generation for directories
- **Event Handling**: Click events for custom navigation logic
- **Loading States**: Built-in loading and empty state handling
- **Responsive Design**: Mobile-friendly tree navigation
- **Keyboard Navigation**: Accessible keyboard interaction support

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Directories` | `IReadOnlyList<DirectoryItem>?` | `null` | List of directories to display |
| `CurrentDirectory` | `string?` | `null` | Path of currently active directory |
| `ExpandedDirectory` | `string?` | `null` | Path of directory to expand subdirectories |
| `ExpandAllSubdirectories` | `bool` | `false` | Whether to expand all subdirectories by default |
| `IsLoading` | `bool` | `false` | Whether the navigation is in loading state |
| `ShowItemCount` | `bool` | `true` | Whether to display item count for each directory |
| `ShowSubdirectories` | `bool` | `true` | Whether to show subdirectories |
| `LoadingText` | `string` | `"Loading navigation..."` | Text shown during loading |
| `NoContentText` | `string` | `"No directories available."` | Text shown when no directories exist |
| `DirectoryUrlFormatter` | `Func<DirectoryItem, string>?` | `null` | Custom function to format directory URLs |
| `Title` | `string?` | `null` | Optional title displayed above the navigation |
| `DirectoryClicked` | `EventCallback<DirectoryItem>` | - | Event fired when a directory is clicked |

## Basic Usage

### Simple Directory Navigation

```razor
@using Osirion.Blazor.Cms.Web.Components

<DirectoryNavigation Directories="@directories"
                     CurrentDirectory="@currentPath" />

@code {
    private IReadOnlyList<DirectoryItem>? directories;
    private string? currentPath;
    
    protected override async Task OnInitializedAsync()
    {
        directories = await DirectoryService.GetRootDirectoriesAsync();
        currentPath = "/docs/getting-started";
    }
}
```

### Navigation with Custom URL Formatting

```razor
<DirectoryNavigation Directories="@directories"
                     DirectoryUrlFormatter="@FormatDirectoryUrl"
                     CurrentDirectory="@currentPath"
                     ShowItemCount="true" />

@code {
    private string FormatDirectoryUrl(DirectoryItem directory)
    {
        return $"/browse/{directory.Path}";
    }
}
```

### Expandable Navigation with Event Handling

```razor
<DirectoryNavigation Directories="@directories"
                     DirectoryClicked="@OnDirectoryClicked"
                     ExpandedDirectory="@expandedPath"
                     ShowSubdirectories="true" />

@code {
    private string? expandedPath;
    
    private async Task OnDirectoryClicked(DirectoryItem directory)
    {
        expandedPath = directory.Path;
        await LoadDirectoryContentsAsync(directory.Path);
    }
    
    private async Task LoadDirectoryContentsAsync(string path)
    {
        // Custom logic for loading directory contents
        await ContentService.LoadDirectoryAsync(path);
    }
}
```

## Advanced Examples

### Documentation Site Navigation

```razor
@page "/docs/{*path}"

<div class="docs-layout">
    <aside class="docs-sidebar">
        <DirectoryNavigation Directories="@docDirectories"
                           Title="Documentation"
                           CurrentDirectory="@GetCurrentDocPath()"
                           DirectoryUrlFormatter="@FormatDocUrl"
                           ExpandedDirectory="@GetExpandedDocPath()"
                           ShowItemCount="false" />
    </aside>
    
    <main class="docs-content">
        @if (currentDoc != null)
        {
            <article>
                <h1>@currentDoc.Title</h1>
                @((MarkupString)currentDoc.Content)
            </article>
        }
    </main>
</div>

@code {
    [Parameter] public string Path { get; set; } = string.Empty;
    
    private IReadOnlyList<DirectoryItem>? docDirectories;
    private ContentItem? currentDoc;
    
    protected override async Task OnInitializedAsync()
    {
        docDirectories = await DocumentationService.GetDirectoryStructureAsync();
    }
    
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Path))
        {
            currentDoc = await DocumentationService.GetByPathAsync($"/docs/{Path}");
        }
    }
    
    private string GetCurrentDocPath()
    {
        return string.IsNullOrEmpty(Path) ? "/docs" : $"/docs/{Path}";
    }
    
    private string GetExpandedDocPath()
    {
        if (string.IsNullOrEmpty(Path)) return "/docs";
        
        var segments = Path.Split('/');
        return segments.Length > 1 
            ? $"/docs/{string.Join('/', segments.Take(segments.Length - 1))}"
            : "/docs";
    }
    
    private string FormatDocUrl(DirectoryItem directory)
    {
        return $"/docs/{directory.Path}";
    }
}
```

### File Browser Interface

```razor
@page "/files/{*currentPath}"

<div class="file-browser">
    <div class="browser-header">
        <ContentBreadcrumbs Directory="@currentDirectory" />
        
        <div class="browser-actions">
            <button @onclick="RefreshDirectory" class="btn btn-outline-primary">
                <i class="fas fa-refresh"></i> Refresh
            </button>
            <button @onclick="CreateFolder" class="btn btn-primary">
                <i class="fas fa-folder-plus"></i> New Folder
            </button>
        </div>
    </div>
    
    <div class="browser-content">
        <aside class="browser-navigation">
            <DirectoryNavigation Directories="@allDirectories"
                               Title="Folders"
                               CurrentDirectory="@CurrentPath"
                               DirectoryClicked="@OnDirectorySelected"
                               DirectoryUrlFormatter="@FormatFileUrl"
                               ExpandedDirectory="@expandedPath"
                               ShowItemCount="true" />
        </aside>
        
        <main class="browser-files">
            @if (isLoading)
            {
                <div class="text-center">
                    <div class="spinner-border" role="status">
                        <span class="visually-hidden">Loading files...</span>
                    </div>
                </div>
            }
            else if (currentFiles?.Any() == true)
            {
                <div class="file-grid">
                    @foreach (var file in currentFiles)
                    {
                        <div class="file-item">
                            <i class="@GetFileIcon(file.Type)"></i>
                            <span class="file-name">@file.Name</span>
                            <span class="file-size">@FormatFileSize(file.Size)</span>
                        </div>
                    }
                </div>
            }
            else
            {
                <div class="empty-directory">
                    <i class="fas fa-folder-open fa-3x"></i>
                    <p>This folder is empty</p>
                </div>
            }
        </main>
    </div>
</div>

@code {
    [Parameter] public string CurrentPath { get; set; } = string.Empty;
    
    private IReadOnlyList<DirectoryItem>? allDirectories;
    private DirectoryItem? currentDirectory;
    private IReadOnlyList<FileItem>? currentFiles;
    private string? expandedPath;
    private bool isLoading = false;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadDirectoryStructureAsync();
    }
    
    protected override async Task OnParametersSetAsync()
    {
        await LoadCurrentDirectoryAsync();
    }
    
    private async Task LoadDirectoryStructureAsync()
    {
        allDirectories = await FileService.GetDirectoryTreeAsync();
    }
    
    private async Task LoadCurrentDirectoryAsync()
    {
        isLoading = true;
        
        try
        {
            currentDirectory = await FileService.GetDirectoryAsync(CurrentPath);
            currentFiles = await FileService.GetFilesAsync(CurrentPath);
            expandedPath = CurrentPath;
        }
        finally
        {
            isLoading = false;
        }
    }
    
    private async Task OnDirectorySelected(DirectoryItem directory)
    {
        await Navigation.NavigateToAsync($"/files/{directory.Path}");
    }
    
    private string FormatFileUrl(DirectoryItem directory)
    {
        return $"/files/{directory.Path}";
    }
    
    private async Task RefreshDirectory()
    {
        await LoadCurrentDirectoryAsync();
    }
    
    private async Task CreateFolder()
    {
        // Implementation for creating a new folder
        var folderName = await PromptForFolderName();
        if (!string.IsNullOrEmpty(folderName))
        {
            await FileService.CreateDirectoryAsync(CurrentPath, folderName);
            await LoadDirectoryStructureAsync();
        }
    }
    
    private string GetFileIcon(string fileType)
    {
        return fileType.ToLower() switch
        {
            "pdf" => "fas fa-file-pdf",
            "doc" or "docx" => "fas fa-file-word",
            "xls" or "xlsx" => "fas fa-file-excel",
            "jpg" or "jpeg" or "png" or "gif" => "fas fa-file-image",
            "mp4" or "avi" or "mov" => "fas fa-file-video",
            "mp3" or "wav" or "ogg" => "fas fa-file-audio",
            _ => "fas fa-file"
        };
    }
    
    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size = size / 1024;
        }
        
        return $"{size:0.##} {sizes[order]}";
    }
}
```

### Project Resource Navigator

```razor
@page "/resources/{projectId}"

<div class="resource-navigator">
    <div class="navigator-header">
        <h2>@projectName Resources</h2>
        <div class="view-controls">
            <button @onclick="@(() => SetViewMode("tree"))" 
                    class="btn @(viewMode == "tree" ? "btn-primary" : "btn-outline-primary")">
                <i class="fas fa-sitemap"></i> Tree View
            </button>
            <button @onclick="@(() => SetViewMode("list"))" 
                    class="btn @(viewMode == "list" ? "btn-primary" : "btn-outline-primary")">
                <i class="fas fa-list"></i> List View
            </button>
        </div>
    </div>
    
    @if (viewMode == "tree")
    {
        <DirectoryNavigation Directories="@resourceDirectories"
                           CurrentDirectory="@selectedDirectory"
                           DirectoryClicked="@OnResourceDirectoryClicked"
                           DirectoryUrlFormatter="@FormatResourceUrl"
                           ExpandAllSubdirectories="@expandAll"
                           ShowItemCount="true"
                           Title="Resource Categories" />
        
        <div class="resource-actions">
            <button @onclick="ToggleExpandAll" class="btn btn-outline-secondary">
                @(expandAll ? "Collapse All" : "Expand All")
            </button>
        </div>
    }
    else
    {
        <div class="resource-list">
            @if (allResources?.Any() == true)
            {
                @foreach (var resource in allResources)
                {
                    <div class="resource-item">
                        <h4>@resource.Name</h4>
                        <p>@resource.Description</p>
                        <small class="text-muted">@resource.Category</small>
                    </div>
                }
            }
        </div>
    }
</div>

@code {
    [Parameter] public string ProjectId { get; set; } = string.Empty;
    
    private string projectName = string.Empty;
    private string viewMode = "tree";
    private string? selectedDirectory;
    private bool expandAll = false;
    private IReadOnlyList<DirectoryItem>? resourceDirectories;
    private IReadOnlyList<ResourceItem>? allResources;
    
    protected override async Task OnParametersSetAsync()
    {
        var project = await ProjectService.GetByIdAsync(ProjectId);
        projectName = project?.Name ?? "Unknown Project";
        
        resourceDirectories = await ResourceService.GetDirectoriesAsync(ProjectId);
        allResources = await ResourceService.GetAllAsync(ProjectId);
    }
    
    private async Task OnResourceDirectoryClicked(DirectoryItem directory)
    {
        selectedDirectory = directory.Path;
        var resources = await ResourceService.GetByDirectoryAsync(ProjectId, directory.Path);
        // Handle directory selection
    }
    
    private string FormatResourceUrl(DirectoryItem directory)
    {
        return $"/resources/{ProjectId}/{directory.Path}";
    }
    
    private void SetViewMode(string mode)
    {
        viewMode = mode;
    }
    
    private void ToggleExpandAll()
    {
        expandAll = !expandAll;
    }
}
```

### Admin Content Management

```razor
@page "/admin/content/{*contentPath}"
@attribute [Authorize(Roles = "Admin")]

<div class="admin-content-manager">
    <div class="content-navigation">
        <DirectoryNavigation Directories="@contentDirectories"
                           Title="Content Structure"
                           CurrentDirectory="@ContentPath"
                           DirectoryClicked="@OnContentDirectoryClicked"
                           DirectoryUrlFormatter="@FormatAdminContentUrl"
                           ShowItemCount="true"
                           LoadingText="Loading content structure..." />
        
        <div class="navigation-actions">
            <button @onclick="CreateDirectory" class="btn btn-success btn-sm">
                <i class="fas fa-folder-plus"></i> New Folder
            </button>
            <button @onclick="ImportContent" class="btn btn-primary btn-sm">
                <i class="fas fa-upload"></i> Import
            </button>
        </div>
    </div>
    
    <div class="content-details">
        @if (selectedDirectoryItems?.Any() == true)
        {
            <div class="content-items">
                <h3>Content in @(selectedDirectoryName ?? "Root")</h3>
                
                <div class="items-grid">
                    @foreach (var item in selectedDirectoryItems)
                    {
                        <div class="content-item-card">
                            <div class="item-header">
                                <h5>@item.Title</h5>
                                <div class="item-actions">
                                    <button @onclick="@(() => EditContent(item))" 
                                            class="btn btn-outline-primary btn-sm">
                                        <i class="fas fa-edit"></i>
                                    </button>
                                    <button @onclick="@(() => DeleteContent(item))" 
                                            class="btn btn-outline-danger btn-sm">
                                        <i class="fas fa-trash"></i>
                                    </button>
                                </div>
                            </div>
                            <p class="item-description">@item.Description</p>
                            <div class="item-meta">
                                <small class="text-muted">
                                    Modified: @item.LastModified?.ToString("MMM dd, yyyy")
                                </small>
                            </div>
                        </div>
                    }
                </div>
            </div>
        }
        else
        {
            <div class="empty-directory">
                <i class="fas fa-folder-open fa-3x text-muted"></i>
                <p>No content in this directory</p>
                <button @onclick="CreateContent" class="btn btn-primary">
                    <i class="fas fa-plus"></i> Create Content
                </button>
            </div>
        }
    </div>
</div>

@code {
    [Parameter] public string ContentPath { get; set; } = string.Empty;
    
    private IReadOnlyList<DirectoryItem>? contentDirectories;
    private IReadOnlyList<ContentItem>? selectedDirectoryItems;
    private string? selectedDirectoryName;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadContentStructureAsync();
    }
    
    protected override async Task OnParametersSetAsync()
    {
        await LoadDirectoryContentsAsync();
    }
    
    private async Task LoadContentStructureAsync()
    {
        contentDirectories = await AdminContentService.GetDirectoryStructureAsync();
    }
    
    private async Task LoadDirectoryContentsAsync()
    {
        if (!string.IsNullOrEmpty(ContentPath))
        {
            selectedDirectoryItems = await AdminContentService.GetDirectoryItemsAsync(ContentPath);
            var directory = await AdminContentService.GetDirectoryAsync(ContentPath);
            selectedDirectoryName = directory?.Name;
        }
        else
        {
            selectedDirectoryItems = await AdminContentService.GetRootItemsAsync();
            selectedDirectoryName = "Root";
        }
    }
    
    private async Task OnContentDirectoryClicked(DirectoryItem directory)
    {
        await Navigation.NavigateToAsync($"/admin/content/{directory.Path}");
    }
    
    private string FormatAdminContentUrl(DirectoryItem directory)
    {
        return $"/admin/content/{directory.Path}";
    }
    
    private async Task CreateDirectory()
    {
        // Implementation for creating a new directory
        var directoryName = await PromptForDirectoryName();
        if (!string.IsNullOrEmpty(directoryName))
        {
            await AdminContentService.CreateDirectoryAsync(ContentPath, directoryName);
            await LoadContentStructureAsync();
        }
    }
    
    private async Task ImportContent()
    {
        // Implementation for importing content
        await ShowImportDialog();
    }
    
    private async Task CreateContent()
    {
        await Navigation.NavigateToAsync($"/admin/content/create?directory={ContentPath}");
    }
    
    private async Task EditContent(ContentItem item)
    {
        await Navigation.NavigateToAsync($"/admin/content/edit/{item.Id}");
    }
    
    private async Task DeleteContent(ContentItem item)
    {
        var confirmed = await ConfirmDelete(item.Title);
        if (confirmed)
        {
            await AdminContentService.DeleteAsync(item.Id);
            await LoadDirectoryContentsAsync();
        }
    }
}
```

### Multi-tenant Directory Navigation

```razor
@page "/tenant/{tenantId}/browse/{*directoryPath}"

<div class="tenant-browser">
    <div class="tenant-header">
        <h2>@tenantName</h2>
        <div class="tenant-info">
            <span class="badge bg-primary">@tenantType</span>
            <span class="text-muted">@directoryCount directories</span>
        </div>
    </div>
    
    <div class="browser-layout">
        <DirectoryNavigation Directories="@tenantDirectories"
                           Title="@($"{tenantName} Directories")"
                           CurrentDirectory="@DirectoryPath"
                           DirectoryClicked="@OnTenantDirectoryClicked"
                           DirectoryUrlFormatter="@FormatTenantDirectoryUrl"
                           ExpandedDirectory="@GetParentPath(DirectoryPath)"
                           ShowItemCount="true"
                           ShowSubdirectories="true" />
    </div>
</div>

@code {
    [Parameter] public string TenantId { get; set; } = string.Empty;
    [Parameter] public string DirectoryPath { get; set; } = string.Empty;
    
    private string tenantName = string.Empty;
    private string tenantType = string.Empty;
    private int directoryCount = 0;
    private IReadOnlyList<DirectoryItem>? tenantDirectories;
    
    protected override async Task OnParametersSetAsync()
    {
        var tenant = await TenantService.GetByIdAsync(TenantId);
        if (tenant != null)
        {
            tenantName = tenant.Name;
            tenantType = tenant.Type;
        }
        
        tenantDirectories = await TenantDirectoryService.GetDirectoriesAsync(TenantId);
        directoryCount = tenantDirectories?.Count ?? 0;
    }
    
    private async Task OnTenantDirectoryClicked(DirectoryItem directory)
    {
        await Navigation.NavigateToAsync($"/tenant/{TenantId}/browse/{directory.Path}");
    }
    
    private string FormatTenantDirectoryUrl(DirectoryItem directory)
    {
        return $"/tenant/{TenantId}/browse/{directory.Path}";
    }
    
    private string? GetParentPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        
        var segments = path.Split('/');
        return segments.Length > 1 
            ? string.Join('/', segments.Take(segments.Length - 1))
            : null;
    }
}
```

## Styling and Customization

### CSS Classes

The component generates the following CSS structure:

```css
.osirion-directory-navigation {
    /* Main navigation container */
}

.osirion-directory-title {
    /* Navigation title */
    margin-bottom: 1rem;
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--bs-heading-color);
}

.osirion-directory-list {
    /* Main directory list */
    list-style: none;
    padding: 0;
    margin: 0;
}

.osirion-directory-item {
    /* Individual directory item */
    margin-bottom: 0.25rem;
}

.osirion-directory-link {
    /* Directory link styling */
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.5rem 0.75rem;
    text-decoration: none;
    color: var(--bs-body-color);
    border-radius: 0.25rem;
    transition: all 0.2s ease;
}

.osirion-directory-link:hover {
    background: var(--bs-light);
    color: var(--bs-primary);
}

.osirion-directory-link.osirion-active {
    background: var(--bs-primary);
    color: white;
}

.osirion-directory-count {
    /* Item count badge */
    background: var(--bs-secondary);
    color: white;
    font-size: 0.75rem;
    padding: 0.125rem 0.375rem;
    border-radius: 0.75rem;
    min-width: 1.5rem;
    text-align: center;
}

.osirion-active .osirion-directory-count {
    background: rgba(255, 255, 255, 0.2);
}

.osirion-subdirectory-list {
    /* Subdirectory list */
    list-style: none;
    padding-left: 1.5rem;
    margin: 0.25rem 0 0 0;
    border-left: 2px solid var(--bs-border-color);
}

.osirion-subdirectory-list .osirion-directory-link {
    padding: 0.375rem 0.5rem;
    font-size: 0.875rem;
}

.osirion-loading,
.osirion-no-directories {
    /* Loading and empty states */
    text-align: center;
    padding: 2rem;
    color: var(--bs-text-muted);
    font-style: italic;
}
```

### Custom Styling Examples

#### Tree-like Appearance
```css
.osirion-directory-navigation {
    font-family: 'Monaco', 'Menlo', monospace;
    background: #f8f9fa;
    border-radius: 0.5rem;
    padding: 1rem;
}

.osirion-directory-item::before {
    content: "📁";
    margin-right: 0.5rem;
}

.osirion-subdirectory-list .osirion-directory-item::before {
    content: "📂";
}

.osirion-directory-link.osirion-active::before {
    content: "📂";
}
```

#### Modern Card Design
```css
.osirion-directory-list {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.osirion-directory-item {
    background: white;
    border: 1px solid var(--bs-border-color);
    border-radius: 0.5rem;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
    margin-bottom: 0;
}

.osirion-directory-link {
    border: none;
    border-radius: 0.5rem;
}

.osirion-subdirectory-list {
    border-left: none;
    padding-left: 1rem;
    background: var(--bs-light);
    border-radius: 0 0 0.5rem 0.5rem;
}
```

## Performance Optimization

### Virtual Scrolling for Large Trees

```razor
<div class="directory-container" style="height: 400px; overflow-y: auto;">
    <DirectoryNavigation Directories="@visibleDirectories" />
</div>

@code {
    private IReadOnlyList<DirectoryItem>? allDirectories;
    private IReadOnlyList<DirectoryItem>? visibleDirectories;
    private int visibleRange = 100;
    
    protected override async Task OnInitializedAsync()
    {
        allDirectories = await DirectoryService.GetAllAsync();
        visibleDirectories = allDirectories?.Take(visibleRange).ToList();
    }
}
```

### Lazy Loading Subdirectories

```razor
<DirectoryNavigation Directories="@rootDirectories"
                     DirectoryClicked="@LoadSubdirectories" />

@code {
    private IReadOnlyList<DirectoryItem>? rootDirectories;
    
    private async Task LoadSubdirectories(DirectoryItem directory)
    {
        if (directory.Children == null || !directory.Children.Any())
        {
            var subdirectories = await DirectoryService.GetSubdirectoriesAsync(directory.Path);
            // Update directory with loaded subdirectories
        }
    }
}
```

## Common Use Cases

- **Documentation Sites**: Hierarchical content organization
- **File Browsers**: File system navigation interfaces  
- **Admin Panels**: Content management navigation
- **Project Management**: Resource and asset organization
- **E-learning Platforms**: Course structure navigation
- **Code Repositories**: Source code directory browsing
- **Digital Asset Management**: Media organization systems

## Best Practices

1. **Performance**: Load subdirectories lazily for large hierarchies
2. **User Experience**: Provide clear visual hierarchy with proper indentation
3. **Accessibility**: Ensure keyboard navigation and screen reader support
4. **Mobile Design**: Consider collapsible navigation on small screens
5. **State Management**: Persist expanded/collapsed states across navigation
6. **Loading States**: Show progress indicators for slow-loading directories
7. **Error Handling**: Gracefully handle missing or inaccessible directories
8. **Search Integration**: Consider adding search functionality for large trees

The DirectoryNavigation component provides a robust foundation for hierarchical navigation in Blazor applications, with extensive customization options for different use cases and design requirements.
