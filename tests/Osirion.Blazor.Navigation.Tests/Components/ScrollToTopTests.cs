using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Core.Extensions;
using Osirion.Blazor.Navigation.Options;
using Microsoft.AspNetCore.Components;
using Osirion.Blazor.Components;
using Osirion.Blazor.Navigation.Components;
using Shouldly;

namespace Osirion.Blazor.Navigation.Tests.Components;

public class ScrollToTopTests : BunitContext
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
        var cut = Render<ScrollToTop>();

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
        var cut = Render<ScrollToTop>(parameters => parameters
            .Add(p => p.Position, Position.TopLeft)
            .Add(p => p.Behavior, ScrollBehavior.Instant)
            .Add(p => p.VisibilityThreshold, 500)
            .Add(p => p.Text, "Back to top")
            .Add(p => p.Title, "Go up")
            .Add(p => p.Class, "custom-class"));

        // Assert
        cut.Instance.Position.ShouldBe(Position.TopLeft);
        cut.Instance.Behavior.ShouldBe(ScrollBehavior.Instant);
        cut.Instance.VisibilityThreshold.ShouldBe(500);
        cut.Instance.Text.ShouldBe("Back to top");
        cut.Instance.Title.ShouldBe("Go up");
        cut.Instance.Class.ShouldBe("custom-class");
    }

    [Fact]
    public void ScrollToTop_ShouldRenderAUniqueButtonId_PerInstance()
    {
        var first = Render<ScrollToTop>();
        var second = Render<ScrollToTop>();

        var firstId = first.Find("button.osirion-scroll-to-top").Id;
        var secondId = second.Find("button.osirion-scroll-to-top").Id;

        firstId.ShouldStartWith("osirion-scroll-to-top-");
        firstId.ShouldNotBe(secondId);
        first.Markup.ShouldContain($"getElementById('{firstId}')");
        second.Markup.ShouldContain($"getElementById('{secondId}')");
    }

    [Fact]
    public void ScrollToTop_ShouldKeepTheSameButtonId_AcrossRenders()
    {
        var cut = Render<ScrollToTop>();
        var id = cut.Find("button.osirion-scroll-to-top").Id;

        cut.Render(parameters => parameters.Add(p => p.Title, "Changed"));

        cut.Find("button.osirion-scroll-to-top").Id.ShouldBe(id);
    }

    [Fact]
    public void ScrollToTop_ShouldRegisterListenersOnlyOnceInTheInitializationGuard()
    {
        var cut = Render<ScrollToTop>();
        var markup = System.Net.WebUtility.HtmlDecode(cut.Markup);

        var guardIndex = markup.IndexOf("hasAttribute('data-initialized')", StringComparison.Ordinal);
        var scrollListenerIndex = markup.IndexOf("addEventListener('scroll'", StringComparison.Ordinal);
        var guardedReturnIndex = markup.IndexOf("setAttribute('data-initialized'", StringComparison.Ordinal);

        guardIndex.ShouldBeGreaterThan(-1);
        scrollListenerIndex.ShouldBeGreaterThan(guardedReturnIndex);
        guardedReturnIndex.ShouldBeGreaterThan(guardIndex);
        markup.ShouldNotContain("id=\"osirion-scroll-to-top-script\"");
    }

    [Fact]
    public void ScrollToTop_ShouldRenderNothing_WhenTheConfiguredOptionsDisableIt()
    {
        using var context = new BunitContext();
        context.Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new ScrollToTopOptions { Enabled = false }));
        context.SetRendererInfo(new RendererInfo("Server", false));

        var cut = context.Render<ScrollToTop>();

        cut.FindAll("button").ShouldBeEmpty();
    }

    [Fact]
    public void ScrollToTop_ShouldRender_WhenTheParameterOverridesDisabledOptions()
    {
        using var context = new BunitContext();
        context.Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new ScrollToTopOptions { Enabled = false }));
        context.SetRendererInfo(new RendererInfo("Server", false));

        var cut = context.Render<ScrollToTop>(parameters => parameters.Add(p => p.Enabled, true));

        cut.FindAll("button").Count.ShouldBe(1);
    }

    [Fact]
    public void ScrollToTop_ShouldSanitizeCustomIcon()
    {
        using var context = new BunitContext();
        context.Services.AddOsirionHtmlSanitizer();
        context.SetRendererInfo(new RendererInfo("Server", false));

        var cut = context.Render<ScrollToTop>(parameters => parameters
            .Add(p => p.CustomIcon, "<i class=\"icon\" onclick=\"alert(1)\">x</i><script>alert(2)</script><img src=x onerror=alert(3)>"));

        var button = cut.Find("button.osirion-scroll-to-top");
        button.InnerHtml.ShouldNotContain("<script");
        button.InnerHtml.ShouldNotContain("onclick");
        button.InnerHtml.ShouldNotContain("onerror");
        button.QuerySelector("i.icon").ShouldNotBeNull();
    }

    [Fact]
    public void ScrollToTop_ShouldIgnoreCustomIcon_WhenNoSanitizerIsRegistered()
    {
        var cut = Render<ScrollToTop>(parameters => parameters
            .Add(p => p.CustomIcon, "<script>alert(1)</script>"));

        var button = cut.Find("button.osirion-scroll-to-top");
        button.InnerHtml.ShouldNotContain("alert(1)");
        button.QuerySelector("svg").ShouldNotBeNull();
    }

    [Fact]
    public void ScrollToTop_ShouldRenderIconCssClass_WhenSet()
    {
        var cut = Render<ScrollToTop>(parameters => parameters
            .Add(p => p.IconCssClass, "fa fa-arrow-up"));

        var icon = cut.Find("button.osirion-scroll-to-top > span.fa");
        icon.GetAttribute("aria-hidden").ShouldBe("true");
        cut.FindAll("button svg").ShouldBeEmpty();
    }
}
