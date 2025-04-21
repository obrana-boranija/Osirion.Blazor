using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components.Analytics.Options;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;
using Shouldly;

namespace Osirion.Blazor.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOsirionBlazor_Basic_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirionBlazor();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.ShouldNotBeNull();
    }

    [Fact]
    public void AddOsirionBlazor_WithBuilderPattern_RegistersRequestedServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirionBlazor(builder =>
        {
            builder
                .AddScrollToTop(manager =>
                {
                    manager.Position = ButtonPosition.BottomLeft;
                    manager.Text = "Test";
                })
                .AddClarityTracker(options =>
                {
                    options.SiteId = "test-id";
                    options.Track = true;
                });
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var scrollToTopManager = serviceProvider.GetService<ScrollToTopManager>();
        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();

        scrollToTopManager.ShouldNotBeNull();
        scrollToTopManager.Position.ShouldBe(ButtonPosition.BottomLeft);
        scrollToTopManager.Text.ShouldBe("Test");

        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBe("test-id");
        clarityOptions.Value.Track.ShouldBeTrue();
    }

    [Fact]
    public void AddOsirionBlazor_WithConfiguration_RegistersServicesFromConfig()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ScrollToTop:Position", "TopRight" },
                { "ScrollToTop:Text", "Up" },
                { "Clarity:SiteId", "clarity-id" },
                { "Matomo:SiteId", "matomo-id" },
                { "GitHubCms:Owner", "test-owner" }
            })
            .Build();

        // Act
        services.AddOsirionBlazor(builder =>
        {
            builder.AddAllServices(configuration);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var scrollToTopManager = serviceProvider.GetService<ScrollToTopManager>();
        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();
        var matomoOptions = serviceProvider.GetService<IOptions<MatomoOptions>>();
        var gitHubCmsOptions = serviceProvider.GetService<IOptions<GitHubCmsOptions>>();

        scrollToTopManager.ShouldNotBeNull();
        scrollToTopManager.Position.ShouldBe(ButtonPosition.TopRight);
        scrollToTopManager.Text.ShouldBe("Up");

        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBe("clarity-id");

        matomoOptions.ShouldNotBeNull();
        matomoOptions.Value.SiteId.ShouldBe("matomo-id");

        gitHubCmsOptions.ShouldNotBeNull();
        gitHubCmsOptions.Value.Owner.ShouldBe("test-owner");
    }

    [Fact]
    public void AddOsirionBlazor_WithNullServices_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;
        Action configureAction = () => { };

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services!.AddOsirionBlazor(builder => configureAction()));
    }

    [Fact]
    public void AddOsirionBlazor_WithNullConfigureAction_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<OsirionBlazorBuilder>? configureAction = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services.AddOsirionBlazor(configureAction!));
    }

    [Fact]
    public void OsirionBlazorBuilder_ShouldAllowChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - This should compile and not throw
        services.AddOsirionBlazor(builder =>
        {
            builder
                .AddScrollToTop(options => { })
                .AddClarityTracker(options => { })
                .AddMatomoTracker(options => { })
                .AddGitHubCms(options => { });
        });
    }

    [Fact]
    public void OsirionBlazorBuilder_WithSimpleScrollToTop_RegistersCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddOsirionBlazor(builder =>
        {
            builder.AddScrollToTop(
                position: ButtonPosition.TopRight,
                behavior: ScrollBehavior.Instant,
                visibilityThreshold: 500,
                text: "To Top");
        });

        var serviceProvider = services.BuildServiceProvider();
        var manager = serviceProvider.GetService<ScrollToTopManager>();

        // Assert
        manager.ShouldNotBeNull();
        manager.Position.ShouldBe(ButtonPosition.TopRight);
        manager.Behavior.ShouldBe(ScrollBehavior.Instant);
        manager.VisibilityThreshold.ShouldBe(500);
        manager.Text.ShouldBe("To Top");
    }
}