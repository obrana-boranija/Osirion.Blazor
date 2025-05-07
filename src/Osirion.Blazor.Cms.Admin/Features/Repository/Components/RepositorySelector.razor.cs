using Microsoft.AspNetCore.Components;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.Components
{
    public partial class RepositorySelector(NavigationManager navigationManager)
    {
        [Parameter]
        public string Title { get; set; } = "Select Repository";

        [Parameter]
        public string SelectPrompt { get; set; } = "-- Select a repository --";

        [Parameter]
        public EventCallback<string> OnRepositoryChanged { get; set; }

        private string SelectedRepositoryName => ViewModel.SelectedRepository?.Name ?? string.Empty;

        protected override async Task OnInitializedAsync()
        {
            // Subscribe to view model state changes
            ViewModel.StateChanged += StateHasChanged;

            // Load repositories on initialization
            await RefreshRepositories();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
        }

        public void Dispose()
        {
            ViewModel.StateChanged -= StateHasChanged;
        }

        private async Task RefreshRepositories()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                await ViewModel.RefreshRepositoriesAsync();
            });
        }

        private async Task OnRepositorySelected(ChangeEventArgs e)
        {
            var repositoryName = e.Value?.ToString() ?? string.Empty;

            await ExecuteWithLoadingAsync(async () =>
            {
                await ViewModel.SelectRepositoryAsync(repositoryName);

                if (OnRepositoryChanged.HasDelegate)
                {
                    await OnRepositoryChanged.InvokeAsync(repositoryName);
                }
            });
        }

        private void OpenRepositoryLink()
        {
            if (ViewModel.SelectedRepository != null && !string.IsNullOrEmpty(ViewModel.SelectedRepository.HtmlUrl))
            {
                navigationManager.NavigateTo(ViewModel.SelectedRepository.HtmlUrl, true);
            }
        }

        private string GetRepositorySelectorClass()
        {
            return $"osirion-admin-repository-selector {CssClass}".Trim();
        }
    }
}