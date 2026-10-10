using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Infrastructure.Directory;
using Osirion.Blazor.Cms.Infrastructure.FileSystem;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection;

/// <summary>
/// Every content provider must keep its own directory cache: a shared cache answers one provider's directory
/// requests with the directories of whichever provider loaded first.
/// </summary>
public sealed class DirectoryCacheIsolationTests : IDisposable
{
    private readonly string _emptyContentRoot = Path.Combine(Path.GetTempPath(), "osirion-dircache-" + Guid.NewGuid().ToString("N"));

    public DirectoryCacheIsolationTests()
    {
        System.IO.Directory.CreateDirectory(_emptyContentRoot);
    }

    [Fact]
    public async Task FileSystemDirectoryRepository_DefaultCacheHoldsAnotherProvidersDirectories_ReturnsOnlyItsOwn()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.Configure<FileSystemOptions>(options =>
        {
            options.BasePath = _emptyContentRoot;
            options.WatchForChanges = false;
        });
        services.AddSingleton(Substitute.For<IMarkdownProcessor>());
        services.AddSingleton(Substitute.For<IFrontMatterExtractor>());
        services.AddDirectoryServices();
        services.AddOsirionRepositories();
        using var provider = services.BuildServiceProvider();
        var defaultCache = provider.GetRequiredService<IDirectoryCacheManager>();
        await defaultCache.GetCachedDirectoriesAsync(_ => Task.FromResult(new Dictionary<string, DirectoryItem>
        {
            ["github-dir"] = DirectoryItem.Create("github-dir", "blog", "blog", "github-blog")
        }));
        using var scope = provider.CreateScope();

        var directories = await scope.ServiceProvider.GetRequiredService<FileSystemDirectoryRepository>().GetAllAsync();

        directories.ShouldBeEmpty();
    }

    [Fact]
    public void AddGitHubProvidersFromConfiguration_TwoProviders_EachDirectoryRepositoryHasItsOwnCache()
    {
        var configuration = GitHubTestServices.CreateConfiguration();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(configuration);
        services.AddSingleton(Substitute.For<IStateStorageService>());
        services.AddSingleton(Substitute.For<IMarkdownProcessor>());
        services.AddSingleton(Substitute.For<IFrontMatterExtractor>());
        services.AddDirectoryServices();
        services.AddGitHubProvidersFromConfiguration(configuration);
        using var provider = services.BuildServiceProvider();

        var blogCache = GetCache(provider, GitHubTestServices.BlogProvider);
        var docsCache = GetCache(provider, GitHubTestServices.DocsProvider);

        blogCache.ShouldNotBeSameAs(docsCache);
        blogCache.ShouldNotBeSameAs(provider.GetRequiredService<IDirectoryCacheManager>());
    }

    public void Dispose()
    {
        System.IO.Directory.Delete(_emptyContentRoot, recursive: true);
    }

    private static IDirectoryCacheManager GetCache(IServiceProvider provider, string providerName)
    {
        var repository = provider.GetRequiredKeyedService<GitHubDirectoryRepository>($"GitHubDirectoryRepository_{providerName}");
        var field = typeof(DirectoryRepositoryBase).GetField("CacheManager", BindingFlags.Instance | BindingFlags.NonPublic)!;

        return (IDirectoryCacheManager)field.GetValue(repository)!;
    }
}
