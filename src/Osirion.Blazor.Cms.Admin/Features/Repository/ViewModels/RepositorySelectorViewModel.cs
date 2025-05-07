using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;

public class RepositorySelectorViewModel
{
    private readonly RepositoryService _repositoryService;
    private readonly CmsApplicationState _appState;
    private readonly CmsEventMediator _eventMediator;

    public List<GitHubRepository> Repositories { get; private set; } = new();
    public GitHubRepository? SelectedRepository => _appState.SelectedRepository;
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    public event Action? StateChanged;

    public RepositorySelectorViewModel(
        RepositoryService repositoryService,
        CmsApplicationState appState,
        CmsEventMediator eventMediator)
    {
        _repositoryService = repositoryService;
        _appState = appState;
        _eventMediator = eventMediator;

        _appState.StateChanged += OnAppStateChanged;
    }

    public async Task RefreshRepositoriesAsync(bool resetSelection = true)
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            Repositories = await _repositoryService.GetRepositoriesAsync();

            if (resetSelection)
            {
                // Reset repository selection
                _appState.SelectRepository(null);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load repositories: {ex.Message}";
            _appState.SetErrorMessage(ErrorMessage);
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
            _appState.SelectRepository(null);
            return;
        }

        var repository = Repositories.Find(r => r.Name == repositoryName);
        if (repository != null)
        {
            IsLoading = true;
            NotifyStateChanged();

            try
            {
                // Set the selected repository in state
                _appState.SelectRepository(repository);

                // Configure the repository adapter
                _repositoryService.SetRepository(repository.Name);

                // Publish repository selected event
                _eventMediator.Publish(new RepositorySelectedEvent(repository));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to select repository: {ex.Message}";
                _appState.SetErrorMessage(ErrorMessage);
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }
    }

    private void OnAppStateChanged()
    {
        NotifyStateChanged();
    }

    protected void NotifyStateChanged()
    {
        StateChanged?.Invoke();
    }
}