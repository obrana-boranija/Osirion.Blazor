using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        services.AddLogging();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddOsirionCms(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify that basic services are registered
        Assert.NotNull(serviceProvider.GetService<IContentProviderManager>());
    }

    [Fact]
    public void AddOsirionCms_RegistersFileSystemAbstractionForFileSystemProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddOsirionCms(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        Assert.NotNull(serviceProvider.GetService<System.IO.Abstractions.IFileSystem>());
    }

    [Fact]
    public void AddOsirionGitHubContentProvider_ConfiguresGitHubOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirionGitHubContentProvider(options =>
        {
            options.Owner = "test-owner";
            options.Repository = "test-repository";
            options.Branch = "main";
        });

        // Assert
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<GitHubOptions>>().Value;
        Assert.Equal("test-owner", options.Owner);
        Assert.Equal("test-repository", options.Repository);
        Assert.Equal("main", options.Branch);
    }

    [Fact]
    public void AddOsirionFileSystemContentProvider_ConfiguresFileSystemOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirionFileSystemContentProvider(options =>
        {
            options.BasePath = "test/path";
            options.WatchForChanges = true;
        });

        // Assert
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<FileSystemOptions>>().Value;
        Assert.Equal("test/path", options.BasePath);
        Assert.True(options.WatchForChanges);
    }
}