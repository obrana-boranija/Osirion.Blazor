using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Cms.Domain.Options;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Theming;
using Osirion.Blazor.Theming.Options;
using Shouldly;

namespace Osirion.Blazor.Tests.Extensions;

public class OsirionServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOsirion_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services!.AddOsirion());
    }

    [Fact]
    public void AddOsirion_WithValidServices_ShouldReturnServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddOsirion();

        // Assert
        result.ShouldBe(services);
    }

    [Fact]
    public void AddOsirion_WithConfiguration_ShouldInvokeConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configurationCalled = false;

        // Act
        services.AddOsirion(builder =>
        {
            configurationCalled = true;
            builder.Services.ShouldBe(services);
        });

        // Assert
        configurationCalled.ShouldBeTrue();
    }

    [Fact]
    public void AddOsirion_WithConfigFile_ShouldConfigureAllModules()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Content configuration
                { "Osirion:Content:GitHub:Owner", "test-owner" },
                { "Osirion:Content:GitHub:Repository", "test-repo" },
                { "Osirion:Content:FileSystem:BasePath", "/test/path" },

                // Analytics configuration  
                { "Osirion:Analytics:Clarity:SiteId", "clarity-123" },
                { "Osirion:Analytics:Matomo:SiteId", "matomo-456" },

                // Navigation configuration
                { "Osirion:Navigation:Enhanced:Behavior", "Smooth" },
                { "Osirion:Navigation:ScrollToTop:Position", "TopLeft" },
                
                // Theming configuration
                { "Osirion:Theming:Framework", "Bootstrap" },
                { "Osirion:Theming:EnableDarkMode", "true" }
            })
            .Build();

        // Act
        var result = services.AddOsirion(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBe(services);

        // Verify Content configuration
        var githubOptions = serviceProvider.GetService<IOptions<GitHubOptions>>();
        githubOptions.ShouldNotBeNull();
        githubOptions.Value.Owner.ShouldBe("test-owner");
        githubOptions.Value.Repository.ShouldBe("test-repo");

        var fileSystemOptions = serviceProvider.GetService<IOptions<FileSystemOptions>>();
        fileSystemOptions.ShouldNotBeNull();
        fileSystemOptions.Value.BasePath.ShouldBe("/test/path");

        // Verify Analytics configuration
        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();
        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBe("clarity-123");

        var matomoOptions = serviceProvider.GetService<IOptions<MatomoOptions>>();
        matomoOptions.ShouldNotBeNull();
        matomoOptions.Value.SiteId.ShouldBe("matomo-456");

        // Verify Navigation configuration
        var enhancedNavOptions = serviceProvider.GetService<IOptions<EnhancedNavigationOptions>>();
        enhancedNavOptions.ShouldNotBeNull();
        enhancedNavOptions.Value.Behavior.ShouldBe(ScrollBehavior.Smooth);

        var scrollToTopOptions = serviceProvider.GetService<IOptions<ScrollToTopOptions>>();
        scrollToTopOptions.ShouldNotBeNull();
        scrollToTopOptions.Value.Position.ShouldBe(Position.TopLeft);

        // Verify Theming configuration
        var themingOptions = serviceProvider.GetService<IOptions<ThemingOptions>>();
        themingOptions.ShouldNotBeNull();
        themingOptions.Value.Framework.ShouldBe(CssFramework.Bootstrap);
        themingOptions.Value.EnableDarkMode.ShouldBeTrue();
    }

    [Fact]
    public void AddOsirion_WithBuilder_ShouldConfigureModulesSelectively()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddOsirion(builder =>
        {
            builder.UseContent(content =>
            {
                content.AddGitHub(options =>
                {
                    options.Owner = "test-owner";
                    options.Repository = "test-repo";
                });
            });

            builder.UseAnalytics(analytics =>
            {
                analytics.AddClarity(options =>
                {
                    options.SiteId = "test-clarity-id";
                    options.Enabled = true;
                });
            });
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verify Content is configured
        var githubOptions = serviceProvider.GetService<IOptions<GitHubOptions>>();
        githubOptions.ShouldNotBeNull();
        githubOptions.Value.Owner.ShouldBe("test-owner");
        githubOptions.Value.Repository.ShouldBe("test-repo");

        // Verify Analytics is configured
        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();
        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBe("test-clarity-id");
        clarityOptions.Value.Enabled.ShouldBeTrue();
    }
}
