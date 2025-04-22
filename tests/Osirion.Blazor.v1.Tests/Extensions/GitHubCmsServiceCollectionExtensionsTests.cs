namespace Osirion.Blazor.Tests.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services.GitHub;
using Shouldly;
using Xunit;

public class GitHubCmsServiceCollectionExtensionsTests
{
    [Fact]
    public void AddGitHubCms_WithConfiguration_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "GitHubCms:Owner", "test-owner" },
                { "GitHubCms:Repository", "test-repo" },
                { "GitHubCms:ContentPath", "content" },
                { "GitHubCms:Branch", "main" },
                { "GitHubCms:ApiToken", "test-token" },
                { "GitHubCms:CacheDurationMinutes", "60" }
            })
            .Build();

        // Act
        services.AddGitHubCms(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<GitHubCmsOptions>>().Value;
        var service = serviceProvider.GetRequiredService<IGitHubCmsService>();

        // Assert
        options.Owner.ShouldBe("test-owner");
        options.Repository.ShouldBe("test-repo");
        options.ContentPath.ShouldBe("content");
        options.Branch.ShouldBe("main");
        options.ApiToken.ShouldBe("test-token");
        options.CacheDurationMinutes.ShouldBe(60);
        service.ShouldNotBeNull();
        service.ShouldBeAssignableTo<GitHubCmsService>();
    }

    [Fact]
    public void AddGitHubCms_WithActionDelegate_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddGitHubCms(options =>
        {
            options.Owner = "test-owner";
            options.Repository = "test-repo";
            options.ContentPath = "blog";
            options.Branch = "develop";
            options.ApiToken = "test-token";
            options.CacheDurationMinutes = 45;
        });
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<GitHubCmsOptions>>().Value;
        var service = serviceProvider.GetRequiredService<IGitHubCmsService>();

        // Assert
        options.Owner.ShouldBe("test-owner");
        options.Repository.ShouldBe("test-repo");
        options.ContentPath.ShouldBe("blog");
        options.Branch.ShouldBe("develop");
        options.ApiToken.ShouldBe("test-token");
        options.CacheDurationMinutes.ShouldBe(45);
        service.ShouldNotBeNull();
        service.ShouldBeAssignableTo<GitHubCmsService>();
    }

    [Fact]
    public void AddGitHubCms_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services!.AddGitHubCms(configuration));
    }

    [Fact]
    public void AddGitHubCms_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? configuration = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services.AddGitHubCms(configuration!));
    }

    [Fact]
    public void AddGitHubCms_WithNullActionDelegate_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<GitHubCmsOptions>? configureOptions = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services.AddGitHubCms(configureOptions!));
    }
}