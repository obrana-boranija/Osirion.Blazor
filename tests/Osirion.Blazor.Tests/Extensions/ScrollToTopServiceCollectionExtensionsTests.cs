using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Extensions;
using Osirion.Blazor.Options;
using Osirion.Blazor.Services;
using Shouldly;

namespace Osirion.Blazor.Tests.Extensions;

public class ScrollToTopServiceCollectionExtensionsTests
{
    [Fact]
    public void AddScrollToTop_WithConfiguration_ShouldConfigureManagerAndOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ScrollToTop:Position", "TopLeft" },
                { "ScrollToTop:Behavior", "Instant" },
                { "ScrollToTop:VisibilityThreshold", "500" },
                { "ScrollToTop:Text", "Up" },
                { "ScrollToTop:Title", "Go to top" },
                { "ScrollToTop:CssClass", "custom-class" }
            })
            .Build();

        // Act
        var result = services.AddScrollToTop(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(services);

        // Verify manager configuration
        var manager = serviceProvider.GetRequiredService<ScrollToTopManager>();
        manager.IsEnabled.ShouldBeTrue();
        manager.Position.ShouldBe(ButtonPosition.TopLeft);
        manager.Behavior.ShouldBe(ScrollBehavior.Instant);
        manager.VisibilityThreshold.ShouldBe(500);
        manager.Text.ShouldBe("Up");
        manager.Title.ShouldBe("Go to top");
        manager.CssClass.ShouldBe("custom-class");

        // Verify options configuration
        var options = serviceProvider.GetRequiredService<IOptions<ScrollToTopOptions>>();
        options.Value.Position.ShouldBe(ButtonPosition.TopLeft);
        options.Value.Behavior.ShouldBe(ScrollBehavior.Instant);
        options.Value.VisibilityThreshold.ShouldBe(500);
        options.Value.Text.ShouldBe("Up");
        options.Value.Title.ShouldBe("Go to top");
        options.Value.CssClass.ShouldBe("custom-class");
    }

    [Fact]
    public void AddScrollToTop_WithParameters_ShouldConfigureManagerAndOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddScrollToTop(
            position: ButtonPosition.BottomLeft,
            behavior: ScrollBehavior.Smooth,
            visibilityThreshold: 400,
            text: "Back to top");

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(services);

        // Verify manager configuration
        var manager = serviceProvider.GetRequiredService<ScrollToTopManager>();
        manager.IsEnabled.ShouldBeTrue();
        manager.Position.ShouldBe(ButtonPosition.BottomLeft);
        manager.Behavior.ShouldBe(ScrollBehavior.Smooth);
        manager.VisibilityThreshold.ShouldBe(400);
        manager.Text.ShouldBe("Back to top");

        // Verify options configuration
        var options = serviceProvider.GetRequiredService<IOptions<ScrollToTopOptions>>();
        options.Value.Position.ShouldBe(ButtonPosition.BottomLeft);
        options.Value.Behavior.ShouldBe(ScrollBehavior.Smooth);
        options.Value.VisibilityThreshold.ShouldBe(400);
        options.Value.Text.ShouldBe("Back to top");
    }

    [Fact]
    public void AddScrollToTop_WithActionDelegate_ShouldConfigureManagerAndOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddScrollToTop(manager =>
        {
            manager.IsEnabled = true;
            manager.Position = ButtonPosition.TopRight;
            manager.Behavior = ScrollBehavior.Auto;
            manager.VisibilityThreshold = 600;
            manager.Text = "Scroll up";
            manager.Title = "Return to top";
            manager.CssClass = "test-class";
            manager.CustomIcon = "<svg>Test</svg>";
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(services);

        // Verify manager configuration
        var manager = serviceProvider.GetRequiredService<ScrollToTopManager>();
        manager.IsEnabled.ShouldBeTrue();
        manager.Position.ShouldBe(ButtonPosition.TopRight);
        manager.Behavior.ShouldBe(ScrollBehavior.Auto);
        manager.VisibilityThreshold.ShouldBe(600);
        manager.Text.ShouldBe("Scroll up");
        manager.Title.ShouldBe("Return to top");
        manager.CssClass.ShouldBe("test-class");
        manager.CustomIcon.ShouldBe("<svg>Test</svg>");

        // Verify options configuration
        var options = serviceProvider.GetRequiredService<IOptions<ScrollToTopOptions>>();
        options.Value.Position.ShouldBe(ButtonPosition.TopRight);
        options.Value.Behavior.ShouldBe(ScrollBehavior.Auto);
        options.Value.VisibilityThreshold.ShouldBe(600);
        options.Value.Text.ShouldBe("Scroll up");
        options.Value.Title.ShouldBe("Return to top");
        options.Value.CssClass.ShouldBe("test-class");
        options.Value.CustomIcon.ShouldBe("<svg>Test</svg>");
    }

    [Fact]
    public void AddScrollToTop_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => services!.AddScrollToTop(configuration));
    }

    [Fact]
    public void AddScrollToTop_WithNullConfiguration_ShouldReturnServicesWithoutChanges()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? configuration = null;

        // Act
        var result = services.AddScrollToTop(configuration!);

        // Assert
        result.ShouldBeSameAs(services);
        services.Count.ShouldBe(0); // No services added
    }

    [Fact]
    public void AddScrollToTop_WithNullConfigureAction_ShouldReturnServicesWithoutChanges()
    {
        // Arrange
        var services = new ServiceCollection();
        Action<ScrollToTopManager>? configure = null;

        // Act
        var result = services.AddScrollToTop(configure!);

        // Assert
        result.ShouldBeSameAs(services);
        services.Count.ShouldBe(0); // No services added
    }

    [Fact]
    public void AddScrollToTopOptions_ShouldRegisterOptionsButNotManager()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ScrollToTop:Position", "TopLeft" },
                { "ScrollToTop:Behavior", "Instant" },
                { "ScrollToTop:Text", "Up" }
            })
            .Build();

        // Act
        var result = services.AddScrollToTopOptions(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        result.ShouldBeSameAs(services);

        // The manager should not be registered
        var manager = serviceProvider.GetService<ScrollToTopManager>();
        manager.ShouldBeNull();

        // But the options should be
        var options = serviceProvider.GetService<IOptions<ScrollToTopOptions>>();
        options.ShouldNotBeNull();
        options.Value.Position.ShouldBe(ButtonPosition.TopLeft);
        options.Value.Behavior.ShouldBe(ScrollBehavior.Instant);
        options.Value.Text.ShouldBe("Up");
    }
}