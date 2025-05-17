using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Navigation.Components;
using Osirion.Blazor.Navigation.Options;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class EnhancedNavigationTests : TestContext
{
    public EnhancedNavigationTests()
    {
        // Mock server-side environment by setting SetInteractive to false
        Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions()));

        // Set default JSInterop mode
        JSInterop.Mode = JSRuntimeMode.Loose;
        SetRendererInfo(new RendererInfo("Server", false));

    }

    [Fact]
    public void EnhancedNavigation_ShouldUseDefaultOptions_WhenNoOptionsProvided()
    {
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