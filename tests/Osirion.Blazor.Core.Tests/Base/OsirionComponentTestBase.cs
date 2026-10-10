using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Core.Tests.Base;

/// <summary>
/// Base class for Core component tests: registers the services every <see cref="OsirionComponentBase"/> needs.
/// </summary>
public abstract class OsirionComponentTestBase : TestContext
{
    protected OsirionComponentTestBase()
    {
        Services.AddSingleton(Options.Create(new ThemingOptions()));
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    /// <summary>
    /// Renders a component with static server-side rendering, the mode every component must support.
    /// Register test services before calling this: the service provider is built on first render.
    /// </summary>
    protected IRenderedComponent<TComponent> RenderStatic<TComponent>(
        Action<ComponentParameterCollectionBuilder<TComponent>> parameterBuilder)
        where TComponent : IComponent
    {
        SetRendererInfo(new RendererInfo("Static", false));
        return RenderComponent(parameterBuilder);
    }
}
