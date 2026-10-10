using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.DependencyInjection;

public class AddOsirionCmsAdminBuilderTests
{
    [Fact]
    public void AddOsirionCmsAdmin_WithBuilderAction_DoesNotBuildAServiceProvider()
    {
        var services = new ServiceCollection();
        var factoryCalls = 0;
        services.AddSingleton(_ =>
        {
            factoryCalls++;
            return new object();
        });

        services.AddOsirionCmsAdmin(builder => builder.UseFileSystemProvider());

        factoryCalls.ShouldBe(0);
    }

    [Fact]
    public void AddOsirionCmsAdmin_WithBuilderActionAndNoLoggingOrConfigurationRegistered_Succeeds()
    {
        var services = new ServiceCollection();

        Should.NotThrow(() => services.AddOsirionCmsAdmin(_ => { }));
    }
}
