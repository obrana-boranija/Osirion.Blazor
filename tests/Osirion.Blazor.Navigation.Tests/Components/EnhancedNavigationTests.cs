using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Navigation.Components;
using Osirion.Blazor.Navigation.Options;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class EnhancedNavigationTests : TestContext
{
    [Fact]
    public void EnhancedNavigation_ShouldNotRenderScript_WhenServerSide()
    {
        // Arrange
        // Mock server-side environment
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions()));

        // Act
        var cut = RenderComponent<EnhancedNavigation>();

        // Assert
        cut.Markup.ShouldBeEmpty();
    }

    [Fact]
    public void EnhancedNavigation_ShouldUseDefaultOptions_WhenNoOptionsProvided()
    {
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<EnhancedNavigation>();

        // Assert
        cut.Instance.Behavior.ShouldBe(ScrollBehavior.Auto);
        cut.Instance.ResetScrollOnNavigation.ShouldBeTrue();
        cut.Instance.PreserveScrollForSamePageNavigation.ShouldBeTrue();
    }

    [Fact]
    public void EnhancedNavigation_ShouldUseParameterValues_WhenProvided()
    {
        // Arrange
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<EnhancedNavigation>(parameters => parameters
            .Add(p => p.Behavior, ScrollBehavior.Smooth)
            .Add(p => p.ResetScrollOnNavigation, false)
            .Add(p => p.PreserveScrollForSamePageNavigation, false));

        // Assert
        cut.Instance.Behavior.ShouldBe(ScrollBehavior.Smooth);
        cut.Instance.ResetScrollOnNavigation.ShouldBeFalse();
        cut.Instance.PreserveScrollForSamePageNavigation.ShouldBeFalse();
    }
}