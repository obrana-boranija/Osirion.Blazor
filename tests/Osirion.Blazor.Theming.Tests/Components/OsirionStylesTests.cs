using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Components;
using Osirion.Blazor.Theming.Components;
using Osirion.Blazor.Theming.Services;

namespace Osirion.Blazor.Theming.Tests.Components;

public class OsirionStylesTests : TestContext
{
    private readonly IThemeService _themeService = Substitute.For<IThemeService>();

    public OsirionStylesTests()
    {
        Services.AddSingleton(Options.Create(new ThemingOptions()));
        Services.AddSingleton(_themeService);
        SetRendererInfo(new RendererInfo("Static", false));
        ComponentFactories.AddStub<BlazorJSComponents.JS>();
        _themeService.GenerateThemeVariables().Returns(string.Empty);
        _themeService.GetFrameworkClass().Returns(string.Empty);
    }

    [Fact]
    public void OsirionStyles_Rendered_SubscribesToThemeChanges()
    {
        RenderComponent<OsirionStyles>();

        _themeService.Received(1).ThemeChanged += Arg.Any<EventHandler<ThemeChangedEventArgs>>();
    }

    [Fact]
    public void OsirionStyles_Disposed_UnsubscribesFromThemeChanges()
    {
        RenderComponent<OsirionStyles>();

        DisposeComponents();

        _themeService.Received(1).ThemeChanged -= Arg.Any<EventHandler<ThemeChangedEventArgs>>();
    }
}
