# BranchSelector

Purpose
Select a git branch for repository operations in the admin UI.

Parameters
- `Repository`: string - Repository name
- `SelectedBranch`: string - Currently selected branch
- `Branches`: IEnumerable<string> - Available branches
- `OnBranchChanged`: EventCallback<string> - Fired when selection changes

Example
```razor
<BranchSelector Repository="my-repo" SelectedBranch="@selectedBranch" Branches="@branches" OnBranchChanged="@OnBranchChanged" />

@code {
    private string selectedBranch = "main";
    private IEnumerable<string> branches = new[] { "main", "develop" };

    private void OnBranchChanged(string branch)
    {
        selectedBranch = branch;
        // refresh repository view
    }
}
```
