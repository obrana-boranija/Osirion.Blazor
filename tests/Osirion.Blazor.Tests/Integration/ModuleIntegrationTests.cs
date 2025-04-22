using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Analytics.Options;
using Osirion.Blazor.Analytics.Services;
using Osirion.Blazor.Content.Options;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Navigation.Services;
using Osirion.Blazor.Theming;
using Osirion.Blazor.Theming.Options;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Tests.Integration;

/// <summary>
/// Integration tests to verify all modules work together
/// </summary>
public class ModuleIntegrationTests
{
    [Fact]
    public void AllModules_ShouldBeConfigurable_UsingFluentAPI()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirion(builder =>
        {
            builder
                .UseContent(content =>
                {
                    content.AddGitHub(options =>
                    {
                        options.Owner = "test-owner";
                        options.Repository = "test-repo";
                    });
                    content.AddFileSystem(options =>
                    {
                        options.BasePath = "/test/path";
                    });
                })
                .UseAnalytics(analytics =>
                {
                    analytics.AddClarity(options =>
                    {
                        options.SiteId = "clarity-test";
                        options.Enabled = true;
                    });
                    analytics.AddMatomo(options =>
                    {
                        options.SiteId = "matomo-test";
                        options.TrackerUrl = "https://matomo.test/";
                    });
                })
                .UseNavigation(navigation =>
                {
                    navigation.UseEnhancedNavigation(options =>
                    {
                        options.Behavior = ScrollBehavior.Smooth;
                    });
                    navigation.AddScrollToTop(options =>
                    {
                        options.Position = Position.TopLeft;
                    });
                })
                .UseTheming(theming =>
                {
                    theming.UseFramework(CssFramework.Bootstrap);
                    theming.EnableDarkMode();
                });
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        // Content module
        var githubOptions = serviceProvider.GetRequiredService<IOptions<GitHubContentOptions>>();
        githubOptions.Value.Owner.ShouldBe("test-owner");
        githubOptions.Value.Repository.ShouldBe("test-repo");

        var fileSystemOptions = serviceProvider.GetRequiredService<IOptions<FileSystemContentOptions>>();
        fileSystemOptions.Value.BasePath.ShouldBe("/test/path");

        // Analytics module
        var clarityOptions = serviceProvider.GetRequiredService<IOptions<ClarityOptions>>();
        clarityOptions.Value.SiteId.ShouldBe("clarity-test");
        clarityOptions.Value.Enabled.ShouldBeTrue();

        var matomoOptions = serviceProvider.GetRequiredService<IOptions<MatomoOptions>>();
        matomoOptions.Value.SiteId.ShouldBe("matomo-test");
        matomoOptions.Value.TrackerUrl.ShouldBe("https://matomo.test/");

        // Navigation module
        var enhancedNavOptions = serviceProvider.GetRequiredService<IOptions<EnhancedNavigationOptions>>();
        enhancedNavOptions.Value.Behavior.ShouldBe(ScrollBehavior.Smooth);

        var scrollToTopOptions = serviceProvider.GetRequiredService<IOptions<ScrollToTopOptions>>();
        scrollToTopOptions.Value.Position.ShouldBe(Position.TopLeft);

        // Theming module
        var themingOptions = serviceProvider.GetRequiredService<IOptions<ThemingOptions>>();
        themingOptions.Value.Framework.ShouldBe(CssFramework.Bootstrap);
        themingOptions.Value.EnableDarkMode.ShouldBeTrue();

        // Verify services are registered
        var analyticsService = serviceProvider.GetService<IAnalyticsService>();
        analyticsService.ShouldNotBeNull();

        var navigationService = serviceProvider.GetService<INavigationService>();
        navigationService.ShouldNotBeNull();

        var themeService = serviceProvider.GetService<IThemeService>();
        themeService.ShouldNotBeNull();
    }

    [Fact]
    public void AllModules_ShouldBeConfigurable_UsingConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Content
                { "Osirion:Content:GitHub:Owner", "test-owner" },
                { "Osirion:Content:GitHub:Repository", "test-repo" },
                { "Osirion:Content:FileSystem:BasePath", "/test/path" },

                // Analytics
                { "Osirion:Analytics:Clarity:SiteId", "clarity-test" },
                { "Osirion:Analytics:Clarity:Enabled", "true" },
                { "Osirion:Analytics:Matomo:SiteId", "matomo-test" },
                { "Osirion:Analytics:Matomo:TrackerUrl", "https://matomo.test/" },

                // Navigation
                { "Osirion:Navigation:Enhanced:Behavior", "Smooth" },
                { "Osirion:Navigation:ScrollToTop:Position", "TopLeft" },

                // Theming
                { "Osirion:Theming:Framework", "Bootstrap" },
                { "Osirion:Theming:EnableDarkMode", "true" }
            })
            .Build();

        // Act
        services.AddOsirion(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        // Content module
        var githubOptions = serviceProvider.GetRequiredService<IOptions<GitHubContentOptions>>();
        githubOptions.Value.Owner.ShouldBe("test-owner");
        githubOptions.Value.Repository.ShouldBe("test-repo");

        var fileSystemOptions = serviceProvider.GetRequiredService<IOptions<FileSystemContentOptions>>();
        fileSystemOptions.Value.BasePath.ShouldBe("/test/path");

        // Analytics module
        var clarityOptions = serviceProvider.GetRequiredService<IOptions<ClarityOptions>>();
        clarityOptions.Value.SiteId.ShouldBe("clarity-test");
        clarityOptions.Value.Enabled.ShouldBeTrue();

        var matomoOptions = serviceProvider.GetRequiredService<IOptions<MatomoOptions>>();
        matomoOptions.Value.SiteId.ShouldBe("matomo-test");
        matomoOptions.Value.TrackerUrl.ShouldBe("https://matomo.test/");

        // Navigation module
        var enhancedNavOptions = serviceProvider.GetRequiredService<IOptions<EnhancedNavigationOptions>>();
        enhancedNavOptions.Value.Behavior.ShouldBe(ScrollBehavior.Smooth);

        var scrollToTopOptions = serviceProvider.GetRequiredService<IOptions<ScrollToTopOptions>>();
        scrollToTopOptions.Value.Position.ShouldBe(Position.TopLeft);

        // Theming module
        var themingOptions = serviceProvider.GetRequiredService<IOptions<ThemingOptions>>();
        themingOptions.Value.Framework.ShouldBe(CssFramework.Bootstrap);
        themingOptions.Value.EnableDarkMode.ShouldBeTrue();
    }

    [Fact]
    public void PartialConfiguration_ShouldOnlyConfigureSpecifiedModules()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirion(builder =>
        {
            builder.UseContent(content =>
            {
                content.AddGitHub(options =>
                {
                    options.Owner = "test-owner";
                    options.Repository = "test-repo";
                });
            });
            // Note: Analytics and other modules not configured
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        // Content should be configured
        var githubOptions = serviceProvider.GetRequiredService<IOptions<GitHubContentOptions>>();
        githubOptions.Value.Owner.ShouldBe("test-owner");
        githubOptions.Value.Repository.ShouldBe("test-repo");

        // Analytics should have default options
        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();
        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBeNull();

        // Navigation should have default options  
        var enhancedNavOptions = serviceProvider.GetService<IOptions<EnhancedNavigationOptions>>();
        enhancedNavOptions.ShouldNotBeNull();
        enhancedNavOptions.Value.Behavior.ShouldBe(ScrollBehavior.Auto);

        // Theming should have default options
        var themingOptions = serviceProvider.GetService<IOptions<ThemingOptions>>();
        themingOptions.ShouldNotBeNull();
        themingOptions.Value.Framework.ShouldBe(CssFramework.None);
    }
}