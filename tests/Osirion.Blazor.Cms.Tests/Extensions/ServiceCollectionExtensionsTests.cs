using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Cms.Domain.Services;

namespace Osirion.Blazor.Cms.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOsirionCms_RegistersRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddOsirionCms(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify that basic services are registered
        Assert.NotNull(serviceProvider.GetService<IContentProviderManager>());
    }

    [Fact]
    public void AddOsirionGitHubContentProvider_RegistersGitHubProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirionGitHubContentProvider(options => {
            options.Owner = "test-owner";
            options.Repository = "test-repository";
            options.Branch = "main";
        });

        // Assert
        var descriptor = Assert.Single(services, d =>
            d.ServiceType == typeof(Action<GitHubOptions>));

        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddOsirionFileSystemContentProvider_RegistersFileSystemProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirionFileSystemContentProvider(options => {
            options.BasePath = "test/path";
            options.WatchForChanges = true;
        });

        // Assert
        var descriptor = Assert.Single(services, d =>
            d.ServiceType == typeof(Action<FileSystemOptions>));

        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }
}