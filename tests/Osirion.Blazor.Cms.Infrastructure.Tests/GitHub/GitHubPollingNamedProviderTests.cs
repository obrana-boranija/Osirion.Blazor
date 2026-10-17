using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.Extensions;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

public class GitHubPollingNamedProviderTests
{
    private static readonly TimeSpan PollingWait = TimeSpan.FromSeconds(15);

    private readonly IContentProviderManager _providerManager = Substitute.For<IContentProviderManager>();
    private readonly IGitHubApiClientFactory _apiClientFactory = Substitute.For<IGitHubApiClientFactory>();

    [Fact]
    public async Task ExecuteAsync_NamedProviderWithPolling_PollsWithItsOwnSettings()
    {
        // Fails before the fix: the service read IOptions<GitHubOptions>, which only the single-provider path
        // binds, so a named provider polled with EnablePolling=false and never refreshed its cache.
        var docs = AddProvider("github-o2-r2", "sha-docs");
        using var host = BuildPollingService(new Dictionary<string, string?>
        {
            ["Osirion:Cms:Web:GitHub:Docs:Owner"] = "o2",
            ["Osirion:Cms:Web:GitHub:Docs:Repository"] = "r2",
            ["Osirion:Cms:Web:GitHub:Docs:EnablePolling"] = "true",
            ["Osirion:Cms:Web:GitHub:Docs:PollingIntervalSeconds"] = "1"
        });

        await host.StartAsync();
        var updated = await docs.Updated.Task.WaitAsync(PollingWait);
        await host.StopAsync();

        updated.ShouldBe("sha-docs");
        _apiClientFactory.Received().GetClient("Docs");
    }

    [Fact]
    public async Task ExecuteAsync_TwoNamedProviders_PollsOnlyTheOneThatEnablesPolling()
    {
        var blog = AddProvider("github-o-r", "sha-blog");
        var docs = AddProvider("github-o2-r2", "sha-docs");
        using var host = BuildPollingService(new Dictionary<string, string?>
        {
            ["Osirion:Cms:Web:GitHub:Blog:Owner"] = "o",
            ["Osirion:Cms:Web:GitHub:Blog:Repository"] = "r",
            ["Osirion:Cms:Web:GitHub:Docs:Owner"] = "o2",
            ["Osirion:Cms:Web:GitHub:Docs:Repository"] = "r2",
            ["Osirion:Cms:Web:GitHub:Docs:EnablePolling"] = "true",
            ["Osirion:Cms:Web:GitHub:Docs:PollingIntervalSeconds"] = "1"
        });

        await host.StartAsync();
        await docs.Updated.Task.WaitAsync(PollingWait);
        await host.StopAsync();

        blog.Updated.Task.IsCompleted.ShouldBeFalse();
        _apiClientFactory.DidNotReceive().GetClient("Blog");
    }

    [Fact]
    public async Task ExecuteAsync_NamedProvidersWithoutPolling_FinishesWithoutPolling()
    {
        var blog = AddProvider("github-o-r", "sha-blog");
        using var host = BuildPollingService(new Dictionary<string, string?>
        {
            ["Osirion:Cms:Web:GitHub:Blog:Owner"] = "o",
            ["Osirion:Cms:Web:GitHub:Blog:Repository"] = "r"
        });
        var service = host.Services.GetServices<IHostedService>().OfType<GitHubPollingService>().Single();

        await service.StartAsync(CancellationToken.None);
        await service.ExecuteTask!.WaitAsync(PollingWait);

        blog.Updated.Task.IsCompleted.ShouldBeFalse();
        _apiClientFactory.ReceivedCalls().ShouldBeEmpty();
    }

    private TestProvider AddProvider(string providerId, string sha)
    {
        var provider = new TestProvider(providerId);
        var client = Substitute.For<IGitHubApiClient>();
        client.GetBranchesAsync(Arg.Any<CancellationToken>()).Returns([new GitHubBranch
        {
            Name = "main",
            Commit = new GitHubCommitRef { Sha = sha }
        }]);
        provider.Updater.UpdateCacheAsync(Arg.Any<string?>(), Arg.Any<bool>()).Returns(call =>
        {
            provider.Updated.TrySetResult(call.ArgAt<string?>(0));
            return Task.CompletedTask;
        });
        _providers.Add((IContentProvider)provider.Updater);
        _providerManager.GetAllProviders().Returns(_providers.ToList());
        _clients[providerId] = client;
        _apiClientFactory.GetClient(Arg.Any<string>()).Returns(call =>
            _clients[ProviderIdOf(call.ArgAt<string>(0))]);

        return provider;
    }

    private static string ProviderIdOf(string providerName) => providerName switch
    {
        "Blog" => "github-o-r",
        "Docs" => "github-o2-r2",
        _ => throw new InvalidOperationException(providerName)
    };

    private readonly List<IContentProvider> _providers = [];
    private readonly Dictionary<string, IGitHubApiClient> _clients = [];

    private ServiceProvider BuildPollingServices(Dictionary<string, string?> values)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMemoryCache();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton(_providerManager);
        services.AddSingleton(_apiClientFactory);
        services.AddSingleton(Substitute.For<IStateStorageService>());
        services.AddSingleton(Substitute.For<IMarkdownProcessor>());
        services.AddSingleton(Substitute.For<IFrontMatterExtractor>());
        services.AddSingleton(Substitute.For<IContentQueryFilter>());
        services.AddDirectoryServices();
        services.AddGitHubProvidersFromConfiguration(configuration);
        services.AddGitHubWebhookAndPolling();
        return services.BuildServiceProvider();
    }

    private PollingHost BuildPollingService(Dictionary<string, string?> values)
    {
        return new PollingHost(BuildPollingServices(values));
    }

    private sealed class PollingHost(ServiceProvider provider) : IDisposable
    {
        public IServiceProvider Services => provider;

        public Task StartAsync() => Task.WhenAll(provider.GetServices<IHostedService>().OfType<GitHubPollingService>()
            .Select(service => service.StartAsync(CancellationToken.None)));

        public Task StopAsync() => Task.WhenAll(provider.GetServices<IHostedService>().OfType<GitHubPollingService>()
            .Select(service => service.StopAsync(CancellationToken.None)));

        public void Dispose() => provider.Dispose();
    }

    private sealed class TestProvider
    {
        public TestProvider(string providerId)
        {
            var substitute = Substitute.For<IContentProvider, IContentCacheUpdater>();
            substitute.ProviderId.Returns(providerId);
            Updater = (IContentCacheUpdater)substitute;
            Updater.ProviderId.Returns(providerId);
        }

        public IContentCacheUpdater Updater { get; }

        public TaskCompletionSource<string?> Updated { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}
