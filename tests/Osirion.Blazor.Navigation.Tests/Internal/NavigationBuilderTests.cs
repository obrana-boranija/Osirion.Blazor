using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;
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

    public static TheoryData<string> ScrollToTopRegistrations => new() { "action", "configuration" };

    [Theory]
    [MemberData(nameof(ScrollToTopRegistrations))]
    public void AddScrollToTop_RegistersManagerPerScope(string registration)
    {
        var services = new ServiceCollection();
        RegisterScrollToTop(new NavigationBuilder(services), registration);

        var descriptor = services.Single(d => d.ServiceType == typeof(ScrollToTopManager));

        descriptor.Lifetime.ShouldBe(ServiceLifetime.Scoped);
    }

    [Theory]
    [MemberData(nameof(ScrollToTopRegistrations))]
    public void AddScrollToTop_ManagerChangedInOneScope_IsNotChangedOrNotifiedInAnotherScope(string registration)
    {
        var services = new ServiceCollection();
        RegisterScrollToTop(new NavigationBuilder(services), registration);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using var firstUser = provider.CreateScope();
        using var secondUser = provider.CreateScope();
        var firstManager = firstUser.ServiceProvider.GetRequiredService<ScrollToTopManager>();
        var secondManager = secondUser.ServiceProvider.GetRequiredService<ScrollToTopManager>();
        var secondUserNotifications = 0;
        secondManager.ConfigurationChanged += (_, _) => secondUserNotifications++;

        firstManager.IsEnabled = false;

        secondManager.IsEnabled.ShouldBeTrue();
        secondUserNotifications.ShouldBe(0);
    }

    private static void RegisterScrollToTop(NavigationBuilder builder, string registration)
    {
        if (registration == "action")
        {
            builder.AddScrollToTop(options => options.VisibilityThreshold = 500);
            return;
        }

        builder.AddScrollToTop(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["VisibilityThreshold"] = "500" })
            .Build());
    }

    [Fact]
    public void AddScrollToTop_ShouldHonourEnabledFalse()
    {
        var services = new ServiceCollection();
        var builder = new NavigationBuilder(services);

        builder.AddScrollToTop(options =>
        {
            options.Enabled = false;
            options.IconCssClass = "fa fa-arrow-up";
        });

        using var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetRequiredService<ScrollToTopManager>().IsEnabled.ShouldBeFalse();
        serviceProvider.GetRequiredService<ScrollToTopManager>().IconCssClass.ShouldBe("fa fa-arrow-up");
        var options = serviceProvider.GetRequiredService<IOptions<ScrollToTopOptions>>().Value;
        options.Enabled.ShouldBeFalse();
        options.IconCssClass.ShouldBe("fa fa-arrow-up");
    }

    [Fact]
    public void AddScrollToTop_FromConfiguration_ShouldHonourEnabledFalse()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Enabled"] = "false" })
            .Build();
        var services = new ServiceCollection();

        new NavigationBuilder(services).AddScrollToTop(configuration);

        using var serviceProvider = services.BuildServiceProvider();
        serviceProvider.GetRequiredService<ScrollToTopManager>().IsEnabled.ShouldBeFalse();
    }
}
