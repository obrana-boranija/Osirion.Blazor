using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.Services;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.DependencyInjection;

public class ProviderServiceCollectionExtensionsTests
{
    [Fact]
    public void AddGitHubAdminServices_AfterTransientTypedClient_LeavesSingleScopedRegistration()
    {
        // Proves an earlier transient registration cannot survive next to the scoped admin service.
        var services = new ServiceCollection();
        services.AddHttpClient<IGitHubAdminService, GitHubAdminService>();

        services.AddGitHubAdminServices(new ConfigurationBuilder().Build());

        services.Where(descriptor => descriptor.ServiceType == typeof(IGitHubAdminService))
            .ShouldHaveSingleItem().Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddGitHubAdminServices_GitHubApiClientPrimaryHandler_DoesNotUseCookies()
    {
        // Proves the admin registration on its own turns cookies off, so a pooled handler cannot replay a cookie
        // GitHub set during one user's request on another user's request. It failed before the fix, when only the
        // Infrastructure registrations added the cookie policy.
        var services = new ServiceCollection();
        services.AddGitHubAdminServices(new ConfigurationBuilder().Build());
        using var provider = services.BuildServiceProvider();

        var handler = provider.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler(nameof(IGitHubApiClient));
        while (handler is DelegatingHandler delegatingHandler)
        {
            handler = delegatingHandler.InnerHandler!;
        }

        var usesCookies = handler switch
        {
            HttpClientHandler httpClientHandler => httpClientHandler.UseCookies,
            SocketsHttpHandler socketsHandler => socketsHandler.UseCookies,
            _ => throw new InvalidOperationException($"Unexpected primary handler {handler.GetType()}")
        };
        usesCookies.ShouldBeFalse();
    }
}
