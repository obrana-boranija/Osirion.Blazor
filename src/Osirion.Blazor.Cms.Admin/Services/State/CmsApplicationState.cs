using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Services.State
{
    public class CmsApplicationState
    {
        // State properties
        public GitHubRepository? SelectedRepository { get; private set; }
        public GitHubBranch? SelectedBranch { get; private set; }
        public string CurrentPath { get; private set; } = string.Empty;
        public List<GitHubItem> CurrentItems { get; private set; } = new();
        public string? StatusMessage { get; private set; }
        public string? ErrorMessage { get; private set; }

        // State changed event
        public event Action? StateChanged;

        // State modification methods
        public void SelectRepository(GitHubRepository repository)
        {
            SelectedRepository = repository;
            SelectedBranch = null;
            CurrentPath = string.Empty;
            CurrentItems.Clear();
            NotifyStateChanged();
        }

        public void SelectBranch(GitHubBranch branch)
        {
            SelectedBranch = branch;
            CurrentPath = string.Empty;
            CurrentItems.Clear();
            NotifyStateChanged();
        }

        public void SetCurrentPath(string path, List<GitHubItem> items)
        {
            CurrentPath = path;
            CurrentItems = items;
            NotifyStateChanged();
        }

        public void SetStatusMessage(string message)
        {
            StatusMessage = message;
            ErrorMessage = null;
            NotifyStateChanged();
        }

        public void SetErrorMessage(string message)
        {
            ErrorMessage = message;
            StatusMessage = null;
            NotifyStateChanged();
        }

        public void ClearMessages()
        {
            StatusMessage = null;
            ErrorMessage = null;
            NotifyStateChanged();
        }

        protected void NotifyStateChanged()
        {
            StateChanged?.Invoke();
        }
    }
}