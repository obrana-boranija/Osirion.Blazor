# RepositorySelector

Purpose
Repository selection component used to switch between content repositories.

Parameters
- `SelectedRepository`: string - Currently selected repository
- `Repositories`: IEnumerable<RepositoryInfo> - Available repositories
- `OnRepositoryChanged`: EventCallback<string> - Fired when selection changes

Example
```razor
<RepositorySelector SelectedRepository="@selectedRepo" Repositories="@repos" OnRepositoryChanged="@HandleRepoChange" />

@code {
    private string selectedRepo = "org/site-content";
    private IEnumerable<RepositoryInfo> repos = new[] { new RepositoryInfo("org/site-content"), new RepositoryInfo("org/blog") };

    private void HandleRepoChange(string repo)
    {
        selectedRepo = repo;
        // load repository contents
    }
}
```
