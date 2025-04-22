using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Options;
using Shouldly;

namespace Osirion.Blazor.Tests.Options;

public class ScrollToTopOptionsTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var options = new ScrollToTopOptions();

        // Assert
        options.Behavior.ShouldBe(ScrollBehavior.Smooth);
        options.VisibilityThreshold.ShouldBe(300);
        options.Position.ShouldBe(ButtonPosition.BottomRight);
        options.Title.ShouldBe("Scroll to top");
        options.Text.ShouldBeNull();
        options.CssClass.ShouldBeNull();
        options.CustomIcon.ShouldBeNull();
        options.UseStyles.ShouldBeTrue();
        options.CustomVariables.ShouldBeNull();
    }

    [Fact]
    public void Section_ShouldBeCorrect()
    {
        // Assert
        ScrollToTopOptions.Section.ShouldBe("ScrollToTop");
    }

    [Fact]
    public void BehaviorString_ShouldReturnCorrectValue()
    {
        // Arrange & Act
        var options = new ScrollToTopOptions
        {
            Behavior = ScrollBehavior.Auto
        };

        // Assert
        options.BehaviorString.ShouldBe("auto");

        // Change and test again
        options.Behavior = ScrollBehavior.Instant;
        options.BehaviorString.ShouldBe("instant");

        options.Behavior = ScrollBehavior.Smooth;
        options.BehaviorString.ShouldBe("smooth");
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var options = new ScrollToTopOptions();
        const string customIcon = "<svg>Custom Icon</svg>";
        const string customText = "Go to top";
        const string customCssClass = "custom-class";
        const string customTitle = "Custom Title";
        const string customVariables = "--variable: value;";

        // Act
        options.Behavior = ScrollBehavior.Instant;
        options.VisibilityThreshold = 500;
        options.Position = ButtonPosition.TopLeft;
        options.Title = customTitle;
        options.Text = customText;
        options.CssClass = customCssClass;
        options.CustomIcon = customIcon;
        options.UseStyles = false;
        options.CustomVariables = customVariables;

        // Assert
        options.Behavior.ShouldBe(ScrollBehavior.Instant);
        options.VisibilityThreshold.ShouldBe(500);
        options.Position.ShouldBe(ButtonPosition.TopLeft);
        options.Title.ShouldBe(customTitle);
        options.Text.ShouldBe(customText);
        options.CssClass.ShouldBe(customCssClass);
        options.CustomIcon.ShouldBe(customIcon);
        options.UseStyles.ShouldBeFalse();
        options.CustomVariables.ShouldBe(customVariables);
    }
}