using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Components;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Repository.Components;

public class RepositorySelectorTests : AdminComponentTestContext
{
    private readonly IContentRepositoryAdapter _repositoryAdapter;
    private readonly CmsState _state;

    public RepositorySelectorTests()
    {
        _repositoryAdapter = Substitute.For<IContentRepositoryAdapter>();
        _repositoryAdapter.GetRepositoriesAsync().Returns(new List<GitHubRepository>());
        _repositoryAdapter.GetBranchesAsync(Arg.Any<string>()).Returns(new List<GitHubBranch>());
        _state = new CmsState();

        // The view model and the service have no virtual members, so the component gets the real ones over a substituted adapter.
        var repositoryService = new RepositoryService(_repositoryAdapter, NullLogger<RepositoryService>.Instance);
        Services.AddSingleton(_state);
        Services.AddSingleton(new RepositorySelectorViewModel(
            repositoryService,
            _state,
            NullLogger<RepositorySelectorViewModel>.Instance));
        RenderInteractively();
    }

    private static List<GitHubRepository> CreateRepositories() =>
    [
        new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" },
        new GitHubRepository { Name = "repo2", FullName = "owner/repo2", DefaultBranch = "master" }
    ];

    [Fact]
    public void RepositorySelector_WhenNoRepositories_ShouldShowEmptyState()
    {
        // Act
        var cut = Render<RepositorySelector>();

        // Assert
        cut.Markup.ShouldContain("No repositories found");
    }

    [Fact]
    public void RepositorySelector_WhenLoading_ShouldShowLoadingIndicator()
    {
        // Arrange
        var pendingLoad = new TaskCompletionSource<List<GitHubRepository>>();
        _repositoryAdapter.GetRepositoriesAsync().Returns(pendingLoad.Task);

        // Act
        var cut = Render<RepositorySelector>();

        // Assert
        cut.Find(".spinner-border").ShouldNotBeNull();
        cut.Markup.ShouldContain("Loading repositories");

        pendingLoad.SetResult([]);
    }

    [Fact]
    public void RepositorySelector_WithError_ShouldShowErrorMessage()
    {
        // Arrange
        _repositoryAdapter.GetRepositoriesAsync().Throws(new InvalidOperationException("Test error message"));

        // Act
        var cut = Render<RepositorySelector>();

        // Assert
        cut.Find(".alert-danger").TextContent.ShouldContain("Test error message");
        cut.Find(".card-body button").TextContent.Trim().ShouldBe("Retry");
    }

    [Fact]
    public void RepositorySelector_WithRepositories_ShouldShowRepositorySelect()
    {
        // Arrange
        var repositories = CreateRepositories();
        _repositoryAdapter.GetRepositoriesAsync().Returns(repositories);

        // Act
        var cut = Render<RepositorySelector>();

        // Assert
        cut.Find("select").ShouldNotBeNull();

        // One option per repository plus the prompt
        var options = cut.FindAll("option").ToList();
        options.Count.ShouldBe(repositories.Count + 1);
        options[1].TextContent.ShouldBe("repo1");
        options[2].TextContent.ShouldBe("repo2");
    }

    [Fact]
    public void RepositorySelector_WithSelectedRepository_ShouldShowRepositoryDetails()
    {
        // Arrange
        var repository = new GitHubRepository
        {
            Name = "repo1",
            FullName = "owner/repo1",
            DefaultBranch = "main",
            Description = "Test repository",
            Private = true,
            HtmlUrl = "https://github.com/owner/repo1"
        };
        _repositoryAdapter.GetRepositoriesAsync().Returns([repository]);
        _state.SelectRepository(repository);

        // Act
        var cut = Render<RepositorySelector>();

        // Assert
        var footer = cut.Find(".card-footer");
        footer.TextContent.ShouldContain("repo1");
        footer.TextContent.ShouldContain("Test repository");
        cut.Find(".badge.bg-danger-subtle").ShouldNotBeNull();
        cut.Find("a[href='https://github.com/owner/repo1']").ShouldNotBeNull();
    }

    [Fact]
    public void RepositorySelector_RefreshButton_ShouldCallLoadRepositoriesAsync()
    {
        // Arrange
        var cut = Render<RepositorySelector>();

        // Act
        cut.Find("button[title='Refresh repositories']").Click();

        // Assert
        _repositoryAdapter.Received(2).GetRepositoriesAsync();
    }

    [Fact]
    public void RepositorySelector_SelectingRepository_ShouldCallSelectRepositoryAsync()
    {
        // Arrange
        _repositoryAdapter.GetRepositoriesAsync().Returns(CreateRepositories());
        var cut = Render<RepositorySelector>();

        // Act
        cut.Find("select").Change("repo1");

        // Assert
        _repositoryAdapter.Received(1).SetRepository("repo1");
        _state.SelectedRepository.ShouldNotBeNull().Name.ShouldBe("repo1");
    }

    [Fact]
    public void RepositorySelector_OnInitialized_ShouldLoadRepositories()
    {
        // Act
        Render<RepositorySelector>();

        // Assert
        _repositoryAdapter.Received(1).GetRepositoriesAsync();
    }

    [Fact]
    public void RepositorySelector_WhenSelectedRepositoryChanges_ShouldRerender()
    {
        // Arrange
        var repository = CreateRepositories()[0];
        _repositoryAdapter.GetRepositoriesAsync().Returns([repository]);
        var cut = Render<RepositorySelector>();
        cut.FindAll(".card-footer").ShouldBeEmpty();

        // Act
        cut.InvokeAsync(() => _state.SelectRepository(repository));

        // Assert
        cut.Find(".card-footer").TextContent.ShouldContain("repo1");
    }
}
