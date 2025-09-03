# CommitPanel

Purpose
UI panel for preparing and committing changes to the repository from the admin editor.

Parameters
- `Changes`: IEnumerable<FileChange> - Files staged for commit
- `Branch`: string - Target branch for commit
- `OnCommit`: EventCallback<CommitInfo> - Fired when the user commits
- `DefaultMessage`: string - Default commit message

Example

```razor
<CommitPanel Changes="@changes" Branch="main" OnCommit="@HandleCommit" DefaultMessage="Update content" />

@code {
    private IEnumerable<FileChange> changes = Enumerable.Empty<FileChange>();
    private void HandleCommit(CommitInfo info) { /* perform commit */ }
}
```
