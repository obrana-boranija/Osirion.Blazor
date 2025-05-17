using Bunit;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class ScrollToTopTests : TestContext
{
    public ScrollToTopTests()
    {
        SetRendererInfo(new RendererInfo("Server", false));
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
    [Fact]
    public void ScrollToTop_ShouldUseDefaultOptions_WhenNoOptionsProvided()
    {
        // Act
        var cut = RenderComponent<ScrollToTop>();

        // Assert
        cut.Instance.Position.ShouldBe(Position.BottomRight);
        cut.Instance.Behavior.ShouldBe(ScrollBehavior.Smooth);
        cut.Instance.VisibilityThreshold.ShouldBe(300);
        cut.Instance.Title.ShouldBe("Scroll to top");
    }

    [Fact]
    public void ScrollToTop_ShouldUseParameterValues_WhenProvided()
    {
        // Act
        var cut = RenderComponent<ScrollToTop>(parameters => parameters
            .Add(p => p.Position, Position.TopLeft)
            .Add(p => p.Behavior, ScrollBehavior.Instant)
            .Add(p => p.VisibilityThreshold, 500)
            .Add(p => p.Text, "Back to top")
            .Add(p => p.Title, "Go up")
            .Add(p => p.CssClass, "custom-class"));

        // Assert
        cut.Instance.Position.ShouldBe(Position.TopLeft);
        cut.Instance.Behavior.ShouldBe(ScrollBehavior.Instant);
        cut.Instance.VisibilityThreshold.ShouldBe(500);
        cut.Instance.Text.ShouldBe("Back to top");
        cut.Instance.Title.ShouldBe("Go up");
        cut.Instance.CssClass.ShouldBe("custom-class");
    }
}