# FileExplorer

Purpose
File explorer component used to browse repository contents in the admin interface.

Parameters
- `Repository`: string - Repository name
- `Path`: string - Current path within repository
- `Items`: IEnumerable<FileItem> - Files and folders at current path
- `OnOpen`: EventCallback<FileItem> - Fired when a file or folder is opened

Example

```razor
<FileExplorer Repository="my-repo" Path="/content" Items="@items" OnOpen="@HandleOpen" />

@code {
    private IEnumerable<FileItem> items = Enumerable.Empty<FileItem>();
    private void HandleOpen(FileItem item) { /* load file */ }
}
```
