using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;

public class RepositorySelectorViewModel
{
    private readonly RepositoryService _repositoryService;
    private readonly CmsState _state;
    private readonly ILogger<RepositorySelectorViewModel> _logger;

    public List<GitHubRepository> Repositories { get; private set; } = new();
    public GitHubRepository? SelectedRepository => _state.SelectedRepository;
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    public event Action? StateChanged;

    public RepositorySelectorViewModel(
        RepositoryService repositoryService,
        CmsState state,
        ILogger<RepositorySelectorViewModel> logger)
    {
        _repositoryService = repositoryService;
        _state = state;
        _logger = logger;

        _state.StateChanged += OnStateChanged;
    }

    private void OnStateChanged()
    {
        NotifyStateChanged();
    }

    public async Task LoadRepositoriesAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            _logger.LogInformation("Loading repositories");
            Repositories = await _repositoryService.GetRepositoriesAsync();
            _logger.LogInformation("Loaded {Count} repositories", Repositories.Count);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load repositories: {ex.Message}";
            _logger.LogError(ex, "Failed to load repositories");
            _state.SetErrorMessage(ErrorMessage);
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task SelectRepositoryAsync(string repositoryName)
    {
        if (string.IsNullOrEmpty(repositoryName))
        {
            _state.SelectRepository(null);
            return;
        }

        var repository = Repositories.FirstOrDefault(r => r.Name == repositoryName);
        if (repository != null)
        {
            IsLoading = true;
            NotifyStateChanged();

            try
            {
                _logger.LogInformation("Selecting repository: {Name}", repository.Name);

                // Update state
                _state.SelectRepository(repository);

                // Update service
                _repositoryService.SetRepository(repository.Name);

                // Load branches
                await LoadBranchesForRepositoryAsync(repository.Name);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to select repository: {ex.Message}";
                _logger.LogError(ex, "Failed to select repository: {Name}", repository.Name);
                _state.SetErrorMessage(ErrorMessage);
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }
    }

    private async Task LoadBranchesForRepositoryAsync(string repositoryName)
    {
        try
        {
            var branches = await _repositoryService.GetBranchesAsync(repositoryName);
            _logger.LogInformation("Loaded {Count} branches for repository {Name}", branches.Count, repositoryName);

            // If default branch exists, select it
            var defaultBranch = branches.FirstOrDefault(b =>
                b.Name == (_state.SelectedRepository?.DefaultBranch ?? "main"));

            if (defaultBranch != null)
            {
                _state.SelectBranch(defaultBranch);
                _repositoryService.SetBranch(defaultBranch.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load branches for repository: {Name}", repositoryName);
            throw;
        }
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}