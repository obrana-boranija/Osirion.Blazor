using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Web.Tests.Base;

/// <summary>
/// Base class for Cms.Web component tests: registers the services the content components inject, with a
/// provider manager that has no provider, so components render only the items the test passes in.
/// </summary>
public abstract class CmsWebComponentTestBase : BunitContext
{
    /// <summary>Content with a script element, an event handler and a script URL.</summary>
    protected const string HostileContent =
        "<p>Hello</p><img src=\"/a.png\" alt=\"a\" onerror=\"alert(1)\"><script>alert(2)</script><a href=\"javascript:alert(3)\">link</a>";

    /// <summary>What <see cref="HostileContent"/> must look like once sanitized.</summary>
    protected const string SanitizedContent = "<p>Hello</p><img src=\"/a.png\" alt=\"a\"><a>link</a>";

    protected CmsWebComponentTestBase()
    {
        Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(new ThemingOptions()));
        Services.AddSingleton(Substitute.For<IContentProviderManager>());
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
        return Render(parameterBuilder);
    }

    protected static ContentItem CreateItem(string content) =>
        ContentItem.Create("post-1", "Post", content, "blog/post.md", "test");
}
