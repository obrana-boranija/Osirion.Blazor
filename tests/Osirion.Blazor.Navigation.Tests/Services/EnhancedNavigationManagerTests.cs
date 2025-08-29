using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Navigation.Services;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Services;

public class EnhancedNavigationManagerTests
{
    [Fact]
    public void Constructor_ShouldUseProvidedOptions()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions
        {
            Behavior = ScrollBehavior.Smooth,
            ResetScrollOnNavigation = false,
            PreserveScrollForSamePageNavigation = false
        });

        // Act
        var manager = new EnhancedNavigationManager(options);

        // Assert
        manager.Behavior.ShouldBe(ScrollBehavior.Smooth);
        manager.ResetScrollOnNavigation.ShouldBeFalse();
        manager.PreserveScrollForSamePageNavigation.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_ShouldCreateDefaultOptions_WhenOptionsIsNull()
    {
        // Arrange & Act
        var manager = new EnhancedNavigationManager(null!);

        // Assert
        manager.Behavior.ShouldBe(ScrollBehavior.Auto);
        manager.ResetScrollOnNavigation.ShouldBeTrue();
        manager.PreserveScrollForSamePageNavigation.ShouldBeTrue();
    }

    [Fact]
    public void UpdateOptions_ShouldUpdateAllProperties()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());
        var manager = new EnhancedNavigationManager(options);

        var newOptions = new EnhancedNavigationOptions
        {
            Behavior = ScrollBehavior.Smooth,
            ResetScrollOnNavigation = false,
            PreserveScrollForSamePageNavigation = false
        };

        bool eventRaised = false;
        manager.OptionsChanged += (sender, args) => eventRaised = true;

        // Act
        manager.UpdateOptions(newOptions);

        // Assert
        manager.Behavior.ShouldBe(ScrollBehavior.Smooth);
        manager.ResetScrollOnNavigation.ShouldBeFalse();
        manager.PreserveScrollForSamePageNavigation.ShouldBeFalse();
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void UpdateOptions_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());
        var manager = new EnhancedNavigationManager(options);

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => manager.UpdateOptions(null!));
    }

    [Fact]
    public void OptionsChanged_ShouldBeRaisedWhenOptionsAreUpdated()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions());
        var manager = new EnhancedNavigationManager(options);

        var newOptions = new EnhancedNavigationOptions
        {
            Behavior = ScrollBehavior.Smooth
        };

        object? eventSender = null;
        EventArgs? eventArgs = null;
        manager.OptionsChanged += (sender, args) =>
        {
            eventSender = sender;
            eventArgs = args;
        };

        // Act
        manager.UpdateOptions(newOptions);

        // Assert
        eventSender.ShouldBe(manager);
        eventArgs.ShouldNotBeNull();
    }
}