using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Domain.Models.GitHub;

namespace Osirion.Blazor.Cms.Admin.Features.Repository.Services;

public class RepositoryService
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;

    public RepositoryService(IContentRepositoryAdapter repositoryAdapter)
    {
        _repositoryAdapter = repositoryAdapter;
    }

    public async Task<List<GitHubRepository>> GetRepositoriesAsync()
    {
        return await _repositoryAdapter.GetRepositoriesAsync();
    }

    public async Task<List<GitHubBranch>> GetBranchesAsync(string repositoryName)
    {
        return await _repositoryAdapter.GetBranchesAsync(repositoryName);
    }

    public async Task<GitHubBranch> CreateBranchAsync(string branchName, string baseBranch)
    {
        return await _repositoryAdapter.CreateBranchAsync(branchName, baseBranch);
    }

    public void SetRepository(string repositoryName)
    {
        _repositoryAdapter.SetRepository(repositoryName);
    }

    public void SetBranch(string branchName)
    {
        _repositoryAdapter.SetBranch(branchName);
    }
}