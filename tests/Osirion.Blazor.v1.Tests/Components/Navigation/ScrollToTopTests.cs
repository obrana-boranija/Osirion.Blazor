using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Options;
using Shouldly;

namespace Osirion.Blazor.Tests.Components.Navigation;

public class ScrollToTopTests : TestContext
{
    [Fact]
    public void ScrollToTop_ShouldRenderWithDefaultValues()
    {
        // Arrange & Act
        var cut = RenderComponent<ScrollToTop>();

        // Assert
        cut.Markup.ShouldContain("scroll-to-top");
        cut.Markup.ShouldContain("bottom-right");
        cut.Markup.ShouldContain("<svg"); // Default icon
        cut.Markup.ShouldNotContain("scroll-text"); // No text by default
    }

    [Fact]
    public void ScrollToTop_ShouldRenderWithCustomPosition()
    {
        // Arrange & Act
        var cut = RenderComponent<ScrollToTop>(parameters => parameters
            .Add(p => p.Position, ButtonPosition.TopLeft));

        // Assert
        cut.Markup.ShouldContain("scroll-to-top");
        cut.Markup.ShouldContain("top-left");
    }

    [Fact]
    public void ScrollToTop_ShouldRenderWithCustomText()
    {
        // Arrange
        var text = "Back to top";

        // Act
        var cut = RenderComponent<ScrollToTop>(parameters => parameters
            .Add(p => p.Text, text));

        // Assert
        cut.Markup.ShouldContain("scroll-text");
        cut.Markup.ShouldContain(text);
    }

    [Fact]
    public void ScrollToTop_ShouldRenderWithCustomIcon()
    {
        // Arrange
        var customIcon = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" viewBox=\"0 0 24 24\"><path d=\"M12 3l12 18h-24z\"/></svg>";

        // Act
        var cut = RenderComponent<ScrollToTop>(parameters => parameters
            .Add(p => p.CustomIcon, customIcon));

        // Assert
        cut.Markup.ShouldContain(customIcon);
        cut.Markup.ShouldNotContain("path d=\"M12 19V5M5 12l7-7 7 7\""); // Default icon shouldn't be present
    }

    [Fact]
    public void ScrollToTop_ShouldRenderWithCustomCssClass()
    {
        // Arrange
        var cssClass = "custom-class";

        // Act
        var cut = RenderComponent<ScrollToTop>(parameters => parameters
            .Add(p => p.CssClass, cssClass));

        // Assert
        cut.Markup.ShouldContain($"scroll-to-top bottom-right {cssClass}");
    }

    [Fact]
    public void ScrollToTop_ShouldUseConfiguredOptions()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ScrollToTopOptions
        {
            Position = ButtonPosition.BottomLeft,
            Text = "Go up",
            CssClass = "configured-class",
            Title = "Custom title"
        });

        Services.AddSingleton<IOptions<ScrollToTopOptions>>(options);

        // Act
        var cut = RenderComponent<ScrollToTop>();

        // Assert
        cut.Markup.ShouldContain("bottom-left");
        cut.Markup.ShouldContain("Go up");
        cut.Markup.ShouldContain("configured-class");
        cut.Markup.ShouldContain("Custom title");
    }

    [Fact]
    public void ScrollToTop_ShouldOverrideConfigurationWithParameters()
    {
        // Arrange
        var options = Microsoft.Extensions.Options.Options.Create(new ScrollToTopOptions
        {
            Position = ButtonPosition.BottomLeft,
            Text = "Go up",
            CssClass = "configured-class",
            Title = "Custom title"
        });

        Services.AddSingleton<IOptions<ScrollToTopOptions>>(options);

        // Act
        var cut = RenderComponent<ScrollToTop>(parameters => parameters
            .Add(p => p.Position, ButtonPosition.TopRight)
            .Add(p => p.Text, "Override text"));

        // Assert
        cut.Markup.ShouldContain("top-right");
        cut.Markup.ShouldContain("Override text");
        cut.Markup.ShouldContain("configured-class"); // Should keep other options
    }

    [Fact]
    public void GetScript_ShouldReturnCorrectScript()
    {
        // Arrange
        var component = new TestScrollToTop
        {
            Behavior = ScrollBehavior.Smooth,
            VisibilityThreshold = 400
        };

        // Act
        var script = component.TestGetScript();

        // Assert
        script.ShouldContain("behavior: 'smooth'");
        script.ShouldContain("updateScrollButtonVisibility(scrollButton, 400)");
    }

    /// <summary>
    /// Test class to access protected methods
    /// </summary>
    private class TestScrollToTop : ScrollToTop
    {
        public string TestGetScript() => GetScript();
    }
}