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

public class OsirionBlazorBuilderTests
{
    [Fact]
    public void Constructor_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new OsirionBlazorBuilder(null!));
    }

    [Fact]
    public void Builder_ShouldImplementIOsirionBlazorBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = new OsirionBlazorBuilder(services);

        // Assert
        builder.ShouldBeAssignableTo<IOsirionBlazorBuilder>();
    }

    [Fact]
    public void AddScrollToTop_WithActionDelegate_ShouldRegisterService()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);

        // Act
        var result = builder.AddScrollToTop(manager => {
            manager.Position = ButtonPosition.TopRight;
            manager.Text = "Test";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var scrollToTopManager = serviceProvider.GetService<ScrollToTopManager>();
        scrollToTopManager.ShouldNotBeNull();
        scrollToTopManager.Position.ShouldBe(ButtonPosition.TopRight);
        scrollToTopManager.Text.ShouldBe("Test");
    }

    [Fact]
    public void AddScrollToTop_WithParameters_ShouldRegisterService()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);

        // Act
        var result = builder.AddScrollToTop(
            ButtonPosition.BottomLeft,
            ScrollBehavior.Instant,
            500,
            "Up");

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var scrollToTopManager = serviceProvider.GetService<ScrollToTopManager>();
        scrollToTopManager.ShouldNotBeNull();
        scrollToTopManager.Position.ShouldBe(ButtonPosition.BottomLeft);
        scrollToTopManager.Behavior.ShouldBe(ScrollBehavior.Instant);
        scrollToTopManager.VisibilityThreshold.ShouldBe(500);
        scrollToTopManager.Text.ShouldBe("Up");
    }

    [Fact]
    public void AddClarityTracker_WithActionDelegate_ShouldRegisterOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);

        // Act
        var result = builder.AddClarityTracker(options => {
            options.SiteId = "test-id";
            options.Track = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();
        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBe("test-id");
        clarityOptions.Value.Track.ShouldBeTrue();
    }

    [Fact]
    public void AddClarityTracker_WithConfiguration_ShouldRegisterOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Clarity:SiteId", "config-test-id" },
                { "Clarity:Track", "false" }
            })
            .Build();

        // Act
        var result = builder.AddClarityTracker(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var clarityOptions = serviceProvider.GetService<IOptions<ClarityOptions>>();
        clarityOptions.ShouldNotBeNull();
        clarityOptions.Value.SiteId.ShouldBe("config-test-id");
        clarityOptions.Value.Track.ShouldBeFalse();
    }

    [Fact]
    public void AddMatomoTracker_WithActionDelegate_ShouldRegisterOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);

        // Act
        var result = builder.AddMatomoTracker(options => {
            options.SiteId = "matomo-test-id";
            options.Track = true;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var matomoOptions = serviceProvider.GetService<IOptions<MatomoOptions>>();
        matomoOptions.ShouldNotBeNull();
        matomoOptions.Value.SiteId.ShouldBe("matomo-test-id");
        matomoOptions.Value.Track.ShouldBeTrue();
    }

    [Fact]
    public void AddMatomoTracker_WithConfiguration_ShouldRegisterOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Matomo:SiteId", "config-matomo-id" },
                { "Matomo:Track", "false" }
            })
            .Build();

        // Act
        var result = builder.AddMatomoTracker(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var matomoOptions = serviceProvider.GetService<IOptions<MatomoOptions>>();
        matomoOptions.ShouldNotBeNull();
        matomoOptions.Value.SiteId.ShouldBe("config-matomo-id");
        matomoOptions.Value.Track.ShouldBeFalse();
    }

    [Fact]
    public void AddGitHubCms_WithActionDelegate_ShouldRegisterService()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);

        // Act
        var result = builder.AddGitHubCms(options => {
            options.Owner = "test-owner";
            options.Repository = "test-repo";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var gitHubCmsOptions = serviceProvider.GetService<IOptions<GitHubCmsOptions>>();
        gitHubCmsOptions.ShouldNotBeNull();
        gitHubCmsOptions.Value.Owner.ShouldBe("test-owner");
        gitHubCmsOptions.Value.Repository.ShouldBe("test-repo");
    }

    [Fact]
    public void AddGitHubCms_WithConfiguration_ShouldRegisterService()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "GitHubCms:Owner", "config-owner" },
                { "GitHubCms:Repository", "config-repo" }
            })
            .Build();

        // Act
        var result = builder.AddGitHubCms(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var gitHubCmsOptions = serviceProvider.GetService<IOptions<GitHubCmsOptions>>();
        gitHubCmsOptions.ShouldNotBeNull();
        gitHubCmsOptions.Value.Owner.ShouldBe("config-owner");
        gitHubCmsOptions.Value.Repository.ShouldBe("config-repo");
    }

    [Fact]
    public void AddOsirionStyle_WithActionDelegate_ShouldRegisterOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);

        // Act
        var result = builder.AddOsirionStyle(options => {
            options.UseStyles = false;
            options.FrameworkIntegration = CssFramework.Bootstrap;
            options.CustomVariables = "--test-var: value;";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var styleOptions = serviceProvider.GetService<IOptions<OsirionStyleOptions>>();
        styleOptions.ShouldNotBeNull();
        styleOptions.Value.UseStyles.ShouldBeFalse();
        styleOptions.Value.FrameworkIntegration.ShouldBe(CssFramework.Bootstrap);
        styleOptions.Value.CustomVariables.ShouldBe("--test-var: value;");
    }

    [Fact]
    public void AddOsirionStyle_WithFrameworkParameter_ShouldRegisterOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);

        // Act
        var result = builder.AddOsirionStyle(
            CssFramework.Tailwind,
            false,
            "--test-var: value;");

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var styleOptions = serviceProvider.GetService<IOptions<OsirionStyleOptions>>();
        styleOptions.ShouldNotBeNull();
        styleOptions.Value.UseStyles.ShouldBeFalse();
        styleOptions.Value.FrameworkIntegration.ShouldBe(CssFramework.Tailwind);
        styleOptions.Value.CustomVariables.ShouldBe("--test-var: value;");
    }

    [Fact]
    public void AddOsirionStyle_WithConfiguration_ShouldRegisterOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new OsirionBlazorBuilder(services);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "OsirionStyle:UseStyles", "false" },
                { "OsirionStyle:FrameworkIntegration", "MudBlazor" },
                { "OsirionStyle:CustomVariables", "--config-var: value;" }
            })
            .Build();

        // Act
        var result = builder.AddOsirionStyle(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(builder);
        var styleOptions = serviceProvider.GetService<IOptions<OsirionStyleOptions>>();
        styleOptions.ShouldNotBeNull();
        styleOptions.Value.UseStyles.ShouldBeFalse();
        styleOptions.Value.FrameworkIntegration.ShouldBe(CssFramework.MudBlazor);
        styleOptions.Value.CustomVariables.ShouldBe("--config-var: value;");
    }
}