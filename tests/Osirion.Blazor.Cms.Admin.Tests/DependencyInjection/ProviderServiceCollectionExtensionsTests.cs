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
}
