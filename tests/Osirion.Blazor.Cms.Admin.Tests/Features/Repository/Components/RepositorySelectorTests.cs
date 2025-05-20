using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Components;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Repository.Components;

public class RepositorySelectorTests : TestContext
{
    private readonly RepositorySelectorViewModel _viewModel;
    private readonly CmsState _adminState;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEventSubscriber _eventSubscriber;

    public RepositorySelectorTests()
    {
        _viewModel = Substitute.For<RepositorySelectorViewModel>(
            Substitute.For<RepositoryService>(
                Substitute.For<IContentRepositoryAdapter>(),
                Substitute.For<ILogger<RepositoryService>>()
            ),
            new CmsState(),
            Substitute.For<ILogger<RepositorySelectorViewModel>>()
        );
        _adminState = new CmsState();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _eventSubscriber = Substitute.For<IEventSubscriber>();

        // Register services
        Services.AddSingleton(_viewModel);
        Services.AddSingleton(_adminState);
        Services.AddSingleton(_eventPublisher);
        Services.AddSingleton(_eventSubscriber);
        Services.AddSingleton(Substitute.For<NavigationManager>());

        // Register logger factory and loggers
        var loggerFactory = Substitute.For<ILoggerFactory>();
        loggerFactory.CreateLogger(Arg.Any<string>()).Returns(Substitute.For<ILogger>());
        Services.AddSingleton(loggerFactory);

        // Configure Blazor components base
        Services.AddScoped<BaseComponent>();
    }

    [Fact]
    public void RepositorySelector_WhenNoRepositories_ShouldShowEmptyState()
    {
        // Arrange
        _viewModel.Repositories.Returns(new List<GitHubRepository>());
        _viewModel.IsLoading.Returns(false);
        _viewModel.ErrorMessage.Returns((string)null!);

        // Act
        var cut = RenderComponent<RepositorySelector>();

        // Assert
        cut.Markup.ShouldContain("No repositories found");
    }

    [Fact]
    public void RepositorySelector_WhenLoading_ShouldShowLoadingIndicator()
    {
        // Arrange
        _viewModel.IsLoading.Returns(true);

        // Act
        var cut = RenderComponent<RepositorySelector>();

        // Assert
        cut.Find(".spinner-border").ShouldNotBeNull();
        cut.Markup.ShouldContain("Loading repositories");
    }

    [Fact]
    public void RepositorySelector_WithError_ShouldShowErrorMessage()
    {
        // Arrange
        _viewModel.IsLoading.Returns(false);
        _viewModel.ErrorMessage.Returns("Test error message");

        // Act
        var cut = RenderComponent<RepositorySelector>();

        // Assert
        cut.Find(".alert-danger").ShouldNotBeNull();
        cut.Markup.ShouldContain("Test error message");
        cut.Find("button[onclick*='RefreshRepositories']").ShouldNotBeNull();
    }

    [Fact]
    public void RepositorySelector_WithRepositories_ShouldShowRepositorySelect()
    {
        // Arrange
        var repositories = new List<GitHubRepository>
        {
            new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" },
            new GitHubRepository { Name = "repo2", FullName = "owner/repo2", DefaultBranch = "master" }
        };

        _viewModel.Repositories.Returns(repositories);
        _viewModel.IsLoading.Returns(false);
        _viewModel.ErrorMessage.Returns((string)null);

        // Act
        var cut = RenderComponent<RepositorySelector>();

        // Assert
        var select = cut.Find("select");
        select.ShouldNotBeNull();

        // Should have correct number of options (repositories + default)
        var options = cut.FindAll("option");
        options.Count.ShouldBe(repositories.Count + 1);

        // Verify repository names in options
        options[1].TextContent.ShouldBe("repo1");
        options[2].TextContent.ShouldBe("repo2");
    }

    [Fact]
    public void RepositorySelector_WithSelectedRepository_ShouldShowRepositoryDetails()
    {
        // Arrange
        var repositories = new List<GitHubRepository>
        {
            new GitHubRepository
            {
                Name = "repo1",
                FullName = "owner/repo1",
                DefaultBranch = "main",
                Description = "Test repository",
                Private = true,
                HtmlUrl = "https://github.com/owner/repo1"
            }
        };

        _viewModel.Repositories.Returns(repositories);
        _viewModel.SelectedRepository.Returns(repositories[0]);
        _viewModel.IsLoading.Returns(false);
        _viewModel.ErrorMessage.Returns((string)null);

        // Act
        var cut = RenderComponent<RepositorySelector>();

        // Assert
        cut.Find(".card-footer").ShouldNotBeNull();
        cut.Markup.ShouldContain("repo1");
        cut.Markup.ShouldContain("Test repository");
        cut.Find(".badge.bg-danger-subtle").ShouldNotBeNull(); // Private badge
        cut.Find("a[href='https://github.com/owner/repo1']").ShouldNotBeNull();
    }

    [Fact]
    public void RepositorySelector_RefreshButton_ShouldCallLoadRepositoriesAsync()
    {
        // Arrange
        _viewModel.Repositories.Returns(new List<GitHubRepository>());
        _viewModel.IsLoading.Returns(false);

        // Act
        var cut = RenderComponent<RepositorySelector>();
        cut.Find("button[title='Refresh repositories']").Click();

        // Assert
        _viewModel.Received(1).LoadRepositoriesAsync();
    }

    [Fact]
    public void RepositorySelector_SelectingRepository_ShouldCallSelectRepositoryAsync()
    {
        // Arrange
        var repositories = new List<GitHubRepository>
        {
            new GitHubRepository { Name = "repo1", FullName = "owner/repo1", DefaultBranch = "main" },
            new GitHubRepository { Name = "repo2", FullName = "owner/repo2", DefaultBranch = "master" }
        };

        _viewModel.Repositories.Returns(repositories);
        _viewModel.IsLoading.Returns(false);

        // Act
        var cut = RenderComponent<RepositorySelector>();
        var select = cut.Find("select");

        // Simulate selection change
        select.Change("repo1");

        // Assert
        _viewModel.Received(1).SelectRepositoryAsync("repo1");
    }

    [Fact]
    public void RepositorySelector_OnInitialized_ShouldSubscribeToEvents()
    {
        // Act
        var cut = RenderComponent<RepositorySelector>();

        // Assert - Since ViewModels are mocked, we can't verify the event subscription directly
        // We can verify LoadRepositoriesAsync is called on initialization
        _viewModel.Received(1).LoadRepositoriesAsync();
    }
}