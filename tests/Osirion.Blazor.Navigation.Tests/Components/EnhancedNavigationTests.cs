using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Components;
using Osirion.Blazor.Navigation.Options;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class EnhancedNavigationTests : BunitContext
{
    private static BunitContext CreateContext(EnhancedNavigationOptions options)
    {
        var context = new BunitContext();
        context.Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(options));
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.SetRendererInfo(new RendererInfo("Server", false));
        return context;
    }

    public EnhancedNavigationTests()
    {
        // Mock server-side environment by setting SetInteractive to false
        Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new EnhancedNavigationOptions()));

        // Set default JSInterop mode
        JSInterop.Mode = JSRuntimeMode.Loose;
        SetRendererInfo(new RendererInfo("Server", false));

    }

    [Fact]
    public void EnhancedNavigation_ShouldLeaveParametersUnset_WhenNoneProvided()
    {
        // Act
        var cut = Render<EnhancedNavigation>();

        // Assert
        cut.Instance.Behavior.ShouldBeNull();
        cut.Instance.ResetScrollOnNavigation.ShouldBeNull();
        cut.Instance.PreserveScrollForSamePageNavigation.ShouldBeNull();
    }

    [Fact]
    public void EnhancedNavigation_ShouldUseParameterValues_WhenProvided()
    {
        // Act
        var cut = Render<EnhancedNavigation>(parameters => parameters
            .Add(p => p.Behavior, ScrollBehavior.Smooth)
            .Add(p => p.ResetScrollOnNavigation, false)
            .Add(p => p.PreserveScrollForSamePageNavigation, false));

        // Assert
        cut.Instance.Behavior.ShouldBe(ScrollBehavior.Smooth);
        cut.Instance.ResetScrollOnNavigation.ShouldBe(false);
        cut.Instance.PreserveScrollForSamePageNavigation.ShouldBe(false);
    }

    [Fact]
    public void EnhancedNavigation_ShouldApplyConfiguredOptions_WhenParametersAreNotSet()
    {
        // Arrange
        using var context = CreateContext(new EnhancedNavigationOptions
        {
            Behavior = ScrollBehavior.Smooth,
            ResetScrollOnNavigation = false,
            PreserveScrollForSamePageNavigation = false
        });

        // Act
        var cut = context.Render<EnhancedNavigation>();

        // Assert
        cut.Markup.ShouldContain("behavior: 'smooth'");
        cut.Markup.ShouldNotContain("if (true)");
    }

    [Fact]
    public void EnhancedNavigation_ShouldPreferExplicitParameter_OverConfiguredOptions()
    {
        // Arrange
        using var context = CreateContext(new EnhancedNavigationOptions
        {
            Behavior = ScrollBehavior.Smooth,
            ResetScrollOnNavigation = false,
            PreserveScrollForSamePageNavigation = false
        });

        // Act
        var cut = context.Render<EnhancedNavigation>(parameters => parameters
            .Add(p => p.Behavior, ScrollBehavior.Instant)
            .Add(p => p.ResetScrollOnNavigation, true));

        // Assert
        cut.Markup.ShouldContain("behavior: 'instant'");
        cut.Markup.ShouldNotContain("behavior: 'smooth'");
        cut.Markup.ShouldContain("if (true)");
    }
}
