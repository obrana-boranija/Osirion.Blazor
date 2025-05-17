using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Navigation.Internal;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Navigation.Services;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Internal;

public class NavigationBuilderTests
{
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new NavigationBuilder(null!));
    }

    [Fact]
    public void Services_ShouldReturnProvidedServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new NavigationBuilder(services);

        // Act & Assert
        builder.Services.ShouldBe(services);
    }

    [Fact]
    public void AddEnhancedNavigation_ShouldRegisterEnhancedNavigationManager()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new NavigationBuilder(services);

        // Act
        builder.AddEnhancedNavigation(options => {
            options.Behavior = ScrollBehavior.Smooth;
            options.ResetScrollOnNavigation = false;
            options.PreserveScrollForSamePageNavigation = false;
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var manager = serviceProvider.GetService<EnhancedNavigationManager>();
        manager.ShouldNotBeNull();

        var options = serviceProvider.GetService<IOptions<EnhancedNavigationOptions>>()?.Value;
        options.ShouldNotBeNull();
        options.Behavior.ShouldBe(ScrollBehavior.Smooth);
        options.ResetScrollOnNavigation.ShouldBeFalse();
        options.PreserveScrollForSamePageNavigation.ShouldBeFalse();
    }

    [Fact]
    public void AddEnhancedNavigation_WithConfiguration_ShouldRegisterEnhancedNavigationManager()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new NavigationBuilder(services);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Behavior", "Smooth" },
                { "ResetScrollOnNavigation", "false" },
                { "PreserveScrollForSamePageNavigation", "false" }
            })
            .Build();

        // Act
        builder.AddEnhancedNavigation(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var manager = serviceProvider.GetService<EnhancedNavigationManager>();
        manager.ShouldNotBeNull();

        var options = serviceProvider.GetService<IOptions<EnhancedNavigationOptions>>()?.Value;
        options.ShouldNotBeNull();
        options.Behavior.ShouldBe(ScrollBehavior.Smooth);
        options.ResetScrollOnNavigation.ShouldBeFalse();
        options.PreserveScrollForSamePageNavigation.ShouldBeFalse();
    }

    [Fact]
    public void AddScrollToTop_ShouldRegisterScrollToTopManager()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new NavigationBuilder(services);

        // Act
        builder.AddScrollToTop(options => {
            options.Position = Position.TopLeft;
            options.Behavior = ScrollBehavior.Instant;
            options.VisibilityThreshold = 500;
            options.Text = "Back to top";
            options.Title = "Go to top";
            options.CssClass = "custom-class";
            options.CustomIcon = "<svg>...</svg>";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var manager = serviceProvider.GetService<ScrollToTopManager>();
        manager.ShouldNotBeNull();
        manager.IsEnabled.ShouldBeTrue();
        manager.Position.ShouldBe(Position.TopLeft);
        manager.Behavior.ShouldBe(ScrollBehavior.Instant);
        manager.VisibilityThreshold.ShouldBe(500);
        manager.Text.ShouldBe("Back to top");
        manager.Title.ShouldBe("Go to top");
        manager.CssClass.ShouldBe("custom-class");
        manager.CustomIcon.ShouldBe("<svg>...</svg>");

        var options = serviceProvider.GetService<IOptions<ScrollToTopOptions>>()?.Value;
        options.ShouldNotBeNull();
        options.Position.ShouldBe(Position.TopLeft);
        options.Behavior.ShouldBe(ScrollBehavior.Instant);
        options.VisibilityThreshold.ShouldBe(500);
        options.Text.ShouldBe("Back to top");
        options.Title.ShouldBe("Go to top");
        options.CssClass.ShouldBe("custom-class");
        options.CustomIcon.ShouldBe("<svg>...</svg>");
    }

    [Fact]
    public void AddScrollToTop_WithConfiguration_ShouldRegisterScrollToTopManager()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new NavigationBuilder(services);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Position", "TopLeft" },
                { "Behavior", "Instant" },
                { "VisibilityThreshold", "500" },
                { "Text", "Back to top" },
                { "Title", "Go to top" },
                { "CssClass", "custom-class" },
                { "CustomIcon", "<svg>...</svg>" }
            })
            .Build();

        // Act
        builder.AddScrollToTop(configuration);

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var manager = serviceProvider.GetService<ScrollToTopManager>();
        manager.ShouldNotBeNull();
        manager.IsEnabled.ShouldBeTrue();
        manager.Position.ShouldBe(Position.TopLeft);
        manager.Behavior.ShouldBe(ScrollBehavior.Instant);
        manager.VisibilityThreshold.ShouldBe(500);
        manager.Text.ShouldBe("Back to top");
        manager.Title.ShouldBe("Go to top");
        manager.CssClass.ShouldBe("custom-class");
        manager.CustomIcon.ShouldBe("<svg>...</svg>");

        var options = serviceProvider.GetService<IOptions<ScrollToTopOptions>>()?.Value;
        options.ShouldNotBeNull();
        options.Position.ShouldBe(Position.TopLeft);
        options.Behavior.ShouldBe(ScrollBehavior.Instant);
        options.VisibilityThreshold.ShouldBe(500);
        options.Text.ShouldBe("Back to top");
        options.Title.ShouldBe("Go to top");
        options.CssClass.ShouldBe("custom-class");
        options.CustomIcon.ShouldBe("<svg>...</svg>");
    }

    [Fact]
    public void AddEnhancedNavigation_ThrowsArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new NavigationBuilder(services);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.AddEnhancedNavigation((IConfiguration)null!));
    }

    [Fact]
    public void AddScrollToTop_ThrowsArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new NavigationBuilder(services);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => builder.AddScrollToTop((IConfiguration)null!));
    }
}