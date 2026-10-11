using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Infrastructure.Builders;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Builders;

public class CmsAdminBuilderTests
{
    private static CmsAdminBuilder CreateBuilder(IServiceCollection services) =>
        new(services, new ConfigurationBuilder().Build(), NullLogger<CmsAdminBuilder>.Instance);

    [Fact]
    public void Constructor_WithNullArguments_Throws()
    {
        var configuration = new ConfigurationBuilder().Build();
        var logger = NullLogger<CmsAdminBuilder>.Instance;

        Should.Throw<ArgumentNullException>(() => new CmsAdminBuilder(null!, configuration, logger));
        Should.Throw<ArgumentNullException>(() => new CmsAdminBuilder(new ServiceCollection(), null!, logger));
        Should.Throw<ArgumentNullException>(() => new CmsAdminBuilder(new ServiceCollection(), configuration, null!));
    }

    [Fact]
    public void Configure_WithNullDelegate_Throws()
    {
        var builder = CreateBuilder(new ServiceCollection());

        Should.Throw<ArgumentNullException>(() => builder.Configure(null!));
    }

    [Fact]
    public void Configure_RegistersOptionsClientsAndScopedAdminServices()
    {
        var services = new ServiceCollection();
        var builder = CreateBuilder(services);

        builder.Configure(options => options.Owner = "admin-owner").ShouldBeSameAs(builder);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<GitHubOptions>>().Value.Owner.ShouldBe("admin-owner");
        services.ShouldContain(d => d.ServiceType == typeof(IGitHubApiClient));
        services.ShouldContain(d => d.ServiceType == typeof(IGitHubTokenProvider));
        services.Single(d => d.ServiceType == typeof(IAuthenticationService)).Lifetime.ShouldBe(ServiceLifetime.Scoped);
        services.Single(d => d.ServiceType == typeof(IGitHubAdminService)).Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Fact]
    public void Configure_WhenTheHttpClientRegistrationFails_RegistersTheServiceDirectly()
    {
        var services = new FailingOnceServiceCollection(typeof(IGitHubApiClient));
        var builder = CreateBuilder(services);

        builder.Configure(_ => { });

        var apiClient = services.Single(d => d.ServiceType == typeof(IGitHubApiClient));
        apiClient.ImplementationType.ShouldBe(typeof(GitHubApiClient));
        apiClient.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddGitHubAuth_WithNullDelegate_Throws()
    {
        var builder = CreateBuilder(new ServiceCollection());

        Should.Throw<ArgumentNullException>(() => builder.AddGitHubAuth(null!));
    }

    [Fact]
    public void AddGitHubAuth_RegistersOptionsTokenProviderAndAuthentication()
    {
        var services = new ServiceCollection();
        var builder = CreateBuilder(services);

        builder.AddGitHubAuth(options => options.ClientId = "client").ShouldBeSameAs(builder);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<GithubAuthorizationOptions>>().Value.ClientId.ShouldBe("client");
        services.ShouldContain(d => d.ServiceType == typeof(IGitHubTokenProvider));
        services.Single(d => d.ServiceType == typeof(IAuthenticationService)).Lifetime.ShouldBe(ServiceLifetime.Scoped);
        services.ShouldNotContain(d => d.ServiceType == typeof(IGitHubAdminService));
    }

    [Fact]
    public void AddGitHubAuth_WhenTheServiceIsAlreadyRegistered_DoesNotRegisterItAgain()
    {
        var services = new ServiceCollection();
        services.AddScoped<IGitHubTokenProvider, GitHubTokenProvider>();
        var builder = CreateBuilder(services);

        builder.AddGitHubAuth(_ => { });

        services.Count(d => d.ServiceType == typeof(IGitHubTokenProvider)).ShouldBe(1);
    }
}
