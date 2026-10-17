using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Interfaces.Content;
using Osirion.Blazor.Cms.Domain.Interfaces.Directory;
using Osirion.Blazor.Cms.Infrastructure.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.DependencyInjection;

public class GitHubProviderDefaultTests
{
    [Fact]
    public void AddGitHubProvidersFromConfiguration_DefaultWithoutProviderId_NamesTheIdTheProviderReports()
    {
        // Fails before the fix: the setter was registered as github-Docs-o2-r2 while the provider reports
        // github-o2-r2, so the registry never found the provider to make the default.
        var configuration = Configure(new Dictionary<string, string?>
        {
            ["Osirion:Cms:Web:GitHub:Blog:Owner"] = "o",
            ["Osirion:Cms:Web:GitHub:Blog:Repository"] = "r",
            ["Osirion:Cms:Web:GitHub:Docs:Owner"] = "o2",
            ["Osirion:Cms:Web:GitHub:Docs:Repository"] = "r2",
            ["Osirion:Cms:Web:GitHub:Docs:IsDefault"] = "true"
        });
        using var provider = BuildProvider(configuration);

        var setter = provider.GetServices<IDefaultProviderSetter>().Cast<DefaultProviderSetter>().ShouldHaveSingleItem();
        var reportedIds = provider.GetServices<IContentProvider>().Select(p => p.ProviderId).ToList();

        reportedIds.ShouldContain(setter.ProviderId);
        setter.ProviderId.ShouldBe("github-o2-r2");
    }

    [Fact]
    public void AddGitHubProvidersFromConfiguration_DefaultWithProviderId_UsesTheConfiguredId()
    {
        var configuration = Configure(new Dictionary<string, string?>
        {
            ["Osirion:Cms:Web:GitHub:Docs:Owner"] = "o2",
            ["Osirion:Cms:Web:GitHub:Docs:Repository"] = "r2",
            ["Osirion:Cms:Web:GitHub:Docs:ProviderId"] = "docs",
            ["Osirion:Cms:Web:GitHub:Docs:IsDefault"] = "true"
        });
        using var provider = BuildProvider(configuration);

        var setter = provider.GetServices<IDefaultProviderSetter>().Cast<DefaultProviderSetter>().ShouldHaveSingleItem();

        setter.ProviderId.ShouldBe("docs");
        provider.GetServices<IContentProvider>().Select(p => p.ProviderId).ShouldContain("docs");
    }

    private static IConfiguration Configure(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    private static ServiceProvider BuildProvider(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMemoryCache();
        services.AddSingleton(configuration);
        services.AddSingleton(Substitute.For<IStateStorageService>());
        services.AddSingleton(Substitute.For<IMarkdownProcessor>());
        services.AddSingleton(Substitute.For<IFrontMatterExtractor>());
        services.AddSingleton(Substitute.For<IContentQueryFilter>());
        services.AddDirectoryServices();
        services.AddGitHubProvidersFromConfiguration(configuration);
        return services.BuildServiceProvider();
    }
}
