using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components.Analytics.Options;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;
using Shouldly;

namespace Osirion.Blazor.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOsirionBlazor_Basic_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddOsirionBlazor();

        // Assert
        result.ShouldBeSameAs(services);
    }

    [Fact]
    public void AddOsirionBlazor_WithConfiguration_ShouldRegisterServicesFromConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ScrollToTop:Position", "BottomRight" },
                { "ScrollToTop:Text", "Top" },
                { "Clarity:SiteId", "clarity-test-id" },
                { "Matomo:SiteId", "matomo-test-id" },
                { "GitHubCms:Owner", "test-owner" },
                { "OsirionStyle:FrameworkIntegration", "Bootstrap" }
            })
            .Build();

        // Act
        var result = services.AddOsirionBlazor(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(services);

        var scrollToTopManager = serviceProvider.GetService<ScrollToTopManager>();
        scrollToTopManager.ShouldNotBeNull();
        scrollToTopManager.Text.ShouldBe("Top");

        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();
        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBe("clarity-test-id");

        var matomoOptions = serviceProvider.GetService<IOptions<MatomoOptions>>();
        matomoOptions.ShouldNotBeNull();
        matomoOptions.Value.SiteId.ShouldBe("matomo-test-id");

        var gitHubCmsOptions = serviceProvider.GetService<IOptions<GitHubCmsOptions>>();
        gitHubCmsOptions.ShouldNotBeNull();
        gitHubCmsOptions.Value.Owner.ShouldBe("test-owner");

        var osirionStyleOptions = serviceProvider.GetService<IOptions<OsirionStyleOptions>>();
        osirionStyleOptions.ShouldNotBeNull();
        osirionStyleOptions.Value.FrameworkIntegration.ShouldBe(CssFramework.Bootstrap);
    }

    [Fact]
    public void AddOsirionBlazor_WithBuilder_ShouldRegisterRequestedServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddOsirionBlazor(builder => {
            builder
                .AddScrollToTop(manager => {
                    manager.Position = ButtonPosition.TopRight;
                    manager.Text = "Test";
                })
                .AddClarityTracker(options => {
                    options.SiteId = "clarity-id";
                    options.Track = true;
                })
                .AddOsirionStyle(CssFramework.Tailwind);
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(services);

        var scrollToTopManager = serviceProvider.GetService<ScrollToTopManager>();
        scrollToTopManager.ShouldNotBeNull();
        scrollToTopManager.Position.ShouldBe(ButtonPosition.TopRight);
        scrollToTopManager.Text.ShouldBe("Test");

        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();
        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBe("clarity-id");
        clarityOptions.Value.Track.ShouldBeTrue();

        var osirionStyleOptions = serviceProvider.GetService<IOptions<OsirionStyleOptions>>();
        osirionStyleOptions.ShouldNotBeNull();
        osirionStyleOptions.Value.FrameworkIntegration.ShouldBe(CssFramework.Tailwind);
    }

    [Fact]
    public void AddOsirionBlazor_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services!.AddOsirionBlazor());
    }

    [Fact]
    public void AddOsirionBlazor_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? configuration = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services.AddOsirionBlazor(configuration!));
    }

    [Fact]
    public void AddOsirionBlazor_WithNullConfigureAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<IOsirionBlazorBuilder>? configure = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services.AddOsirionBlazor(configure!));
    }
}