using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Components;
using Osirion.Blazor.Theming.Services;
using Shouldly;

namespace Osirion.Blazor.Theming.Tests.Components;

public class ThemeToggleCallbackTests : BunitContext
{
    private const string ModulePath = "./_content/Osirion.Blazor.Theming/js/theme-toggle-events.js";

    private readonly List<string> _changes = [];
    private readonly BunitJSModuleInterop _module;

    public ThemeToggleCallbackTests()
    {
        var themeService = Substitute.For<IThemeService>();
        themeService.CurrentMode.Returns(ThemeMode.Light);
        Services.AddSingleton(Options.Create(new ThemingOptions()));
        Services.AddSingleton(themeService);
        ComponentFactories.AddStub<BlazorJSComponents.JS>();
        _module = JSInterop.SetupModule(ModulePath);
        _module.Mode = JSRuntimeMode.Loose;
    }

    // Fails before the fix: OnThemeChanged was declared and never raised, in any render mode.
    [Fact]
    public async Task ThemeToggle_Interactive_RaisesOnThemeChangedWhenTheBrowserEventReportsAChange()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        RenderToggle();

        await RaiseFromBrowserAsync("dark");

        _changes.ShouldBe(["dark"]);
    }

    [Fact]
    public void ThemeToggle_Interactive_SubscribesOnceWithAnIdAndTheCallbackTarget()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        RenderToggle();

        var subscription = _module.Invocations["subscribe"].ShouldHaveSingleItem();
        subscription.Arguments[0].ShouldBeOfType<string>().ShouldNotBeNullOrEmpty();
        subscription.Arguments[1].ShouldNotBeNull();
    }

    [Fact]
    public void ThemeToggle_StaticRender_TouchesNoJavaScriptEvenWithACallback()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        RenderToggle();

        JSInterop.Invocations.ShouldBeEmpty();
    }

    [Fact]
    public void ThemeToggle_InteractiveWithoutCallback_TouchesNoJavaScript()
    {
        SetRendererInfo(new RendererInfo("Server", true));

        Render<ThemeToggle>();

        JSInterop.Invocations.ShouldBeEmpty();
    }

    [Fact]
    public async Task ThemeToggle_Disposed_UnsubscribesTheSameId()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        RenderToggle();
        var id = _module.Invocations["subscribe"].Single().Arguments[0];

        await DisposeComponentsAsync();

        _module.Invocations["unsubscribe"].ShouldHaveSingleItem().Arguments[0].ShouldBe(id);
    }

    [Fact]
    public void ThemeToggle_CallbackTarget_IsTheMethodTheScriptNames()
    {
        SetRendererInfo(new RendererInfo("Server", true));
        RenderToggle();

        var method = CallbackTarget().GetType().GetMethod("NotifyThemeChanged");

        method.ShouldNotBeNull();
        method.GetCustomAttribute<JSInvokableAttribute>().ShouldNotBeNull();
    }

    private IRenderedComponent<ThemeToggle> RenderToggle() =>
        Render<ThemeToggle>(parameters => parameters.Add(
            toggle => toggle.OnThemeChanged,
            EventCallback.Factory.Create<string>(this, theme => _changes.Add(theme))));

    // The target is an internal type behind DotNetObjectReference<T>, reached the way the browser reaches it.
    private object CallbackTarget()
    {
        var reference = _module.Invocations["subscribe"].Single().Arguments[1]!;
        return reference.GetType().GetProperty("Value")!.GetValue(reference)!;
    }

    private async Task RaiseFromBrowserAsync(string theme)
    {
        var target = CallbackTarget();
        var task = (Task)target.GetType().GetMethod("NotifyThemeChanged")!.Invoke(target, [theme])!;
        await task;
    }
}
