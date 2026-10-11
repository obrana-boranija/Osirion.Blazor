using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

public class GitHubPollingServiceTests
{
    private const string ConfiguredProviderId = "github-owner-repo";

    private readonly IContentProviderManager _providerManager = Substitute.For<IContentProviderManager>();
    private readonly IGitHubApiClient _apiClient = Substitute.For<IGitHubApiClient>();
    private readonly GitHubOptions _options = new()
    {
        Owner = "owner",
        Repository = "repo",
        Branch = "main"
    };

    [Fact]
    public void Constructor_WithNullServiceProvider_Throws()
    {
        // Act
        var act = () => new GitHubPollingService(null!, Options.Create(_options), NullLogger<GitHubPollingService>.Instance);

        // Assert
        act.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("serviceProvider");
    }

    [Fact]
    public void Constructor_WithNullLogger_Throws()
    {
        // Act
        var act = () => new GitHubPollingService(Substitute.For<IServiceProvider>(), Options.Create(_options), null!);

        // Assert
        act.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("logger");
    }

    [Fact]
    public void Constructor_WithNullOptions_Throws()
    {
        // Act
        var act = () => new GitHubPollingService(Substitute.For<IServiceProvider>(), null!, NullLogger<GitHubPollingService>.Instance);

        // Assert
        act.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("options");
    }

    [Fact]
    public async Task ExecuteAsync_WhenPollingIsDisabled_FinishesWithoutCheckingAnything()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        _options.EnablePolling = false;
        using var service = new GitHubPollingService(serviceProvider, Options.Create(_options), NullLogger<GitHubPollingService>.Instance);

        // Act
        await service.StartAsync(CancellationToken.None);
        await service.ExecuteTask!;

        // Assert
        service.ExecuteTask.IsCompletedSuccessfully.ShouldBeTrue();
        serviceProvider.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenStoppedBeforeTheFirstTick_StopsWithoutCheckingAnything()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        _options.EnablePolling = true;
        _options.PollingIntervalSeconds = 3600;
        using var service = new GitHubPollingService(serviceProvider, Options.Create(_options), NullLogger<GitHubPollingService>.Instance);
        await service.StartAsync(CancellationToken.None);

        // Act
        await service.StopAsync(CancellationToken.None);

        // Assert
        service.ExecuteTask!.IsCompleted.ShouldBeTrue();
        serviceProvider.ReceivedCalls().ShouldBeEmpty();
    }

    [Fact]
    public async Task CheckForChangesAsync_WithANewCommit_UpdatesTheMatchingProvider()
    {
        // Arrange
        var provider = AddCacheUpdater(ConfiguredProviderId);
        ReturnBranches(Branch("main", "sha-1"), Branch("dev", "sha-dev"));
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await provider.Received(1).UpdateCacheAsync("sha-1");
    }

    [Fact]
    public async Task CheckForChangesAsync_WhenTheCommitIsUnchanged_DoesNotUpdateAgain()
    {
        // Arrange
        var provider = AddCacheUpdater(ConfiguredProviderId);
        ReturnBranches(Branch("main", "sha-1"));
        var service = CreateService();
        await service.CheckForChangesAsync(CancellationToken.None);

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await provider.Received(1).UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WhenTheCommitChanges_UpdatesWithTheNewSha()
    {
        // Arrange
        var provider = AddCacheUpdater(ConfiguredProviderId);
        ReturnBranches(Branch("main", "sha-1"));
        var service = CreateService();
        await service.CheckForChangesAsync(CancellationToken.None);
        ReturnBranches(Branch("main", "sha-2"));

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        Received.InOrder(() =>
        {
            provider.UpdateCacheAsync("sha-1");
            provider.UpdateCacheAsync("sha-2");
        });
    }

    [Fact]
    public async Task CheckForChangesAsync_WithoutGitHubCacheUpdaters_DoesNotCallGitHub()
    {
        // Arrange
        var plainProvider = Substitute.For<IContentProvider>();
        plainProvider.ProviderId.Returns(ConfiguredProviderId);
        var fileProvider = AddCacheUpdater("filesystem-docs");
        _providerManager.GetAllProviders().Returns([plainProvider, (IContentProvider)fileProvider]);
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await _apiClient.DidNotReceive().GetBranchesAsync(Arg.Any<CancellationToken>());
        await fileProvider.DidNotReceive().UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WithAnotherRepositoryProvider_SkipsIt()
    {
        // Arrange
        var other = AddCacheUpdater("github-someone-else");
        ReturnBranches(Branch("main", "sha-1"));
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await _apiClient.DidNotReceive().GetBranchesAsync(Arg.Any<CancellationToken>());
        await other.DidNotReceive().UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WithAConfiguredProviderId_MatchesThatIdInstead()
    {
        // Arrange
        _options.ProviderId = "github-custom";
        var custom = AddCacheUpdater("github-custom");
        var byRepository = AddCacheUpdater(ConfiguredProviderId);
        ReturnBranches(Branch("main", "sha-1"));
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await custom.Received(1).UpdateCacheAsync("sha-1");
        await byRepository.DidNotReceive().UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WhenTheBranchIsMissing_DoesNotUpdate()
    {
        // Arrange
        var provider = AddCacheUpdater(ConfiguredProviderId);
        ReturnBranches(Branch("dev", "sha-dev"));
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await provider.DidNotReceive().UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WhenGitHubReturnsNoBranches_DoesNotUpdate()
    {
        // Arrange
        var provider = AddCacheUpdater(ConfiguredProviderId);
        _apiClient.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns((List<GitHubBranch>?)null);
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await provider.DidNotReceive().UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WhenTheBranchHasNoSha_DoesNotUpdate()
    {
        // Arrange
        var provider = AddCacheUpdater(ConfiguredProviderId);
        ReturnBranches(Branch("main", " "));
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await provider.DidNotReceive().UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WhenReadingBranchesFails_DoesNotUpdateOrThrow()
    {
        // Arrange
        var provider = AddCacheUpdater(ConfiguredProviderId);
        _apiClient.GetBranchesAsync(Arg.Any<CancellationToken>()).ThrowsAsync(new HttpRequestException("offline"));
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await provider.DidNotReceive().UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WhenAnUpdateFails_RetriesThatProviderNextTime()
    {
        // Arrange
        var failing = AddCacheUpdater(ConfiguredProviderId);
        failing.UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>()).ThrowsAsync(new InvalidOperationException("boom"));
        ReturnBranches(Branch("main", "sha-1"));
        var service = CreateService();
        await service.CheckForChangesAsync(CancellationToken.None);

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await failing.Received(2).UpdateCacheAsync("sha-1");
    }

    [Fact]
    public async Task CheckForChangesAsync_WithAProviderWhoseIdOnlyStartsWithTheConfiguredId_SkipsIt()
    {
        // Arrange
        var configured = AddCacheUpdater(ConfiguredProviderId);
        var otherRepository = AddCacheUpdater(ConfiguredProviderId + "2");
        ReturnBranches(Branch("main", "sha-1"));
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await configured.Received(1).UpdateCacheAsync("sha-1");
        await otherRepository.DidNotReceive().UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>());
    }

    [Fact]
    public async Task CheckForChangesAsync_WithAConfiguredProviderIdThatDoesNotMentionGitHub_PollsThatProvider()
    {
        // Arrange
        _options.ProviderId = "docs";
        var custom = AddCacheUpdater("docs");
        ReturnBranches(Branch("main", "sha-1"));
        var service = CreateService();

        // Act
        await service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await custom.Received(1).UpdateCacheAsync("sha-1");
    }

    [Fact]
    public async Task CheckForChangesAsync_WhenServicesAreMissing_DoesNotThrow()
    {
        // Arrange
        var service = new GitHubPollingService(
            new ServiceCollection().BuildServiceProvider(),
            Options.Create(_options),
            NullLogger<GitHubPollingService>.Instance);

        // Act
        var act = () => service.CheckForChangesAsync(CancellationToken.None);

        // Assert
        await act.ShouldNotThrowAsync();
    }

    private GitHubPollingService CreateService()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_providerManager);
        services.AddSingleton(_apiClient);

        return new GitHubPollingService(
            services.BuildServiceProvider(),
            Options.Create(_options),
            NullLogger<GitHubPollingService>.Instance);
    }

    private readonly List<IContentProvider> _providers = [];

    private IContentCacheUpdater AddCacheUpdater(string providerId)
    {
        var provider = Substitute.For<IContentProvider, IContentCacheUpdater>();
        provider.ProviderId.Returns(providerId);
        var updater = (IContentCacheUpdater)provider;
        updater.ProviderId.Returns(providerId);

        _providers.Add(provider);
        _providerManager.GetAllProviders().Returns(_providers.ToList());

        return updater;
    }

    private void ReturnBranches(params GitHubBranch[] branches)
    {
        _apiClient.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns(branches.ToList());
    }

    private static GitHubBranch Branch(string name, string sha) => new()
    {
        Name = name,
        Commit = new GitHubCommitRef { Sha = sha }
    };
}
