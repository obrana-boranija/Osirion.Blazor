using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Cms.Admin.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.DependencyInjection;

public class AddOsirionCmsAdminConfigurationTests
{
    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    [Fact]
    public void AddOsirionCmsAdmin_WithBuilder_HandsTheRegisteredConfigurationToTheBuilder()
    {
        var services = new ServiceCollection();
        services.AddSingleton(BuildConfiguration(new() { ["Osirion:Cms:FileSystem:Admin:RootPath"] = "/site" }));

        services.AddOsirionCmsAdmin(builder => builder.UseFileSystemProvider());

        services.BuildServiceProvider().GetRequiredService<IOptions<FileSystemAdminOptions>>().Value.RootPath.ShouldBe("/site");
    }

    [Fact]
    public void AddOsirionCmsAdmin_NullArguments_Throw()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration([]);

        Should.Throw<ArgumentNullException>(() => ((IServiceCollection)null!).AddOsirionCmsAdmin(_ => { }));
        Should.Throw<ArgumentNullException>(() => services.AddOsirionCmsAdmin((Action<Builders.IOsirionCmsAdminBuilder>)null!));
        Should.Throw<ArgumentNullException>(() => ((IServiceCollection)null!).AddOsirionCmsAdmin(configuration));
        Should.Throw<ArgumentNullException>(() => services.AddOsirionCmsAdmin((IConfiguration)null!));
    }

    [Fact]
    public void AddOsirionCmsAdmin_WithConfiguration_BindsTheAdminSection()
    {
        var services = new ServiceCollection();

        services.AddOsirionCmsAdmin(BuildConfiguration(new() { ["Osirion:Cms:Admin:DefaultContentProvider"] = "FileSystem" }));

        services.BuildServiceProvider().GetRequiredService<IOptions<CmsAdminOptions>>().Value.DefaultContentProvider.ShouldBe("FileSystem");
    }

    [Fact]
    public void AddOsirionCmsAdmin_ConfigurationWithGitHubSection_RegistersTheGitHubAdminService()
    {
        var services = new ServiceCollection();

        services.AddOsirionCmsAdmin(BuildConfiguration(new() { ["Osirion:Cms:Admin:GitHub:Owner"] = "octo" }));

        services.Where(descriptor => descriptor.ServiceType == typeof(IGitHubAdminService))
            .ShouldHaveSingleItem().Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddOsirionCmsAdmin_ConfigurationWithFileSystemSection_BindsTheFileSystemOptions()
    {
        var services = new ServiceCollection();

        services.AddOsirionCmsAdmin(BuildConfiguration(new()
        {
            ["Osirion:Cms:Admin:FileSystem:RootPath"] = "/admin",
            ["Osirion:Cms:FileSystem:Admin:RootPath"] = "/site"
        }));

        services.BuildServiceProvider().GetRequiredService<IOptions<FileSystemAdminOptions>>().Value.RootPath.ShouldBe("/site");
    }
}
