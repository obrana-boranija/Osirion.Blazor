using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;

/// <summary>Defines the RepositorySelectorViewModel API contract.</summary>
public class RepositorySelectorViewModel
{
    private readonly RepositoryService _repositoryService;
    private readonly CmsState _state;
    private readonly ILogger<RepositorySelectorViewModel> _logger;

    /// <summary>Gets or sets the Repositories value.</summary>
    public List<GitHubRepository> Repositories { get; set; } = new();
    /// <summary>Performs the SelectedRepository operation.</summary>
    public GitHubRepository? SelectedRepository => _state.SelectedRepository;
    /// <summary>Gets or sets the IsLoading value.</summary>
    public bool IsLoading { get; private set; }
    /// <summary>Gets or sets the ErrorMessage value.</summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>Performs the StateChanged operation.</summary>
    public event Action? StateChanged;

    /// <summary>Performs the RepositorySelectorViewModel operation.</summary>
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

    /// <summary>Performs the LoadRepositories operation asynchronously.</summary>
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

    /// <summary>Performs the SelectRepository operation asynchronously.</summary>
    public async Task SelectRepositoryAsync(string repositoryName)
    {
        if (string.IsNullOrWhiteSpace(repositoryName))
        {
            _state.SelectRepository(null);
            return;
        }

        var repository = Repositories.FirstOrDefault(r => r.Name == repositoryName);
        if (repository is not null)
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

            if (defaultBranch is not null)
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

    /// <summary>Performs the NotifyStateChanged operation.</summary>
    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}
