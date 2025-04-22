using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components.Navigation;
using Osirion.Blazor.Services;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Osirion.Blazor.Tests.Components.Navigation;

public class ScrollToTopProviderTests : TestContext
{
    [Fact]
    public void GlobalScrollToTop_DoesNotRender_WhenDisabled()
    {
        // Arrange
        var manager = new ScrollToTopManager
        {
            IsEnabled = false
        };
        Services.AddSingleton(manager);

        // Act
        var cut = RenderComponent<ScrollToTopProvider>();

        // Assert
        cut.Markup.ShouldBeEmpty();
    }

    [Fact]
    public void GlobalScrollToTop_Renders_WhenEnabled()
    {
        // Arrange
        var manager = new ScrollToTopManager
        {
            IsEnabled = true,
            Position = ButtonPosition.BottomRight,
            Text = "Test"
        };
        Services.AddSingleton(manager);

        // Act
        var cut = RenderComponent<ScrollToTopProvider>();

        // Assert
        cut.Markup.ShouldNotBeEmpty();
        cut.Markup.ShouldContain("scroll-to-top");
        cut.Markup.ShouldContain("bottom-right");
        cut.Markup.ShouldContain("Test");
    }

    [Fact]
    public void GlobalScrollToTop_PassesParameters_FromManager()
    {
        // Arrange
        var manager = new ScrollToTopManager
        {
            IsEnabled = true,
            Position = ButtonPosition.TopLeft,
            Behavior = ScrollBehavior.Instant,
            VisibilityThreshold = 500,
            Text = "Top",
            Title = "Custom Title",
            CssClass = "custom-class"
        };
        Services.AddSingleton(manager);

        // Act
        var cut = RenderComponent<ScrollToTopProvider>();

        // Assert
        cut.Markup.ShouldContain("top-left");
        cut.Markup.ShouldContain("Custom Title");
        cut.Markup.ShouldContain("Top");
        cut.Markup.ShouldContain("custom-class");
    }
}
