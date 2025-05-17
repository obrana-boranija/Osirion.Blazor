using Osirion.Blazor.Navigation.Options;
using Osirion.Blazor.Navigation.Services;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Services;

public class ScrollToTopManagerTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var manager = new ScrollToTopManager();

        // Assert
        manager.IsEnabled.ShouldBeFalse(); // Default is false until explicitly enabled
        manager.Position.ShouldBe(Position.BottomRight);
        manager.Behavior.ShouldBe(ScrollBehavior.Smooth);
        manager.VisibilityThreshold.ShouldBe(300);
        manager.Text.ShouldBeNull();
        manager.Title.ShouldBe("Scroll to top");
        manager.CssClass.ShouldBeNull();
        manager.CustomIcon.ShouldBeNull();
    }

    [Fact]
    public void ApplyOptions_ShouldUpdateAllProperties()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        var options = new ScrollToTopOptions
        {
            Position = Position.TopLeft,
            Behavior = ScrollBehavior.Instant,
            VisibilityThreshold = 500,
            Text = "Back to top",
            Title = "Go to top",
            CssClass = "custom-class",
            CustomIcon = "<svg>...</svg>"
        };

        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.ApplyOptions(options);

        // Assert
        manager.Position.ShouldBe(Position.TopLeft);
        manager.Behavior.ShouldBe(ScrollBehavior.Instant);
        manager.VisibilityThreshold.ShouldBe(500);
        manager.Text.ShouldBe("Back to top");
        manager.Title.ShouldBe("Go to top");
        manager.CssClass.ShouldBe("custom-class");
        manager.CustomIcon.ShouldBe("<svg>...</svg>");
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void ApplyOptions_ShouldThrowArgumentNullException_WhenOptionsIsNull()
    {
        // Arrange
        var manager = new ScrollToTopManager();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => manager.ApplyOptions(null!));
    }

    [Fact]
    public void IsEnabled_ShouldRaiseConfigurationChanged_WhenValueChanges()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.IsEnabled = true;

        // Assert
        manager.IsEnabled.ShouldBeTrue();
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void IsEnabled_ShouldNotRaiseConfigurationChanged_WhenValueDoesNotChange()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        manager.IsEnabled = true; // Set initial value

        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.IsEnabled = true; // Set same value again

        // Assert
        eventRaised.ShouldBeFalse();
    }

    [Fact]
    public void Position_ShouldRaiseConfigurationChanged_WhenValueChanges()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.Position = Position.TopLeft;

        // Assert
        manager.Position.ShouldBe(Position.TopLeft);
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void Behavior_ShouldRaiseConfigurationChanged_WhenValueChanges()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.Behavior = ScrollBehavior.Instant;

        // Assert
        manager.Behavior.ShouldBe(ScrollBehavior.Instant);
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void VisibilityThreshold_ShouldRaiseConfigurationChanged_WhenValueChanges()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.VisibilityThreshold = 500;

        // Assert
        manager.VisibilityThreshold.ShouldBe(500);
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void Text_ShouldRaiseConfigurationChanged_WhenValueChanges()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.Text = "Back to top";

        // Assert
        manager.Text.ShouldBe("Back to top");
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void Title_ShouldRaiseConfigurationChanged_WhenValueChanges()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.Title = "Go to top";

        // Assert
        manager.Title.ShouldBe("Go to top");
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void CssClass_ShouldRaiseConfigurationChanged_WhenValueChanges()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.CssClass = "custom-class";

        // Assert
        manager.CssClass.ShouldBe("custom-class");
        eventRaised.ShouldBeTrue();
    }

    [Fact]
    public void CustomIcon_ShouldRaiseConfigurationChanged_WhenValueChanges()
    {
        // Arrange
        var manager = new ScrollToTopManager();
        bool eventRaised = false;
        manager.ConfigurationChanged += (sender, args) => eventRaised = true;

        // Act
        manager.CustomIcon = "<svg>...</svg>";

        // Assert
        manager.CustomIcon.ShouldBe("<svg>...</svg>");
        eventRaised.ShouldBeTrue();
    }
}