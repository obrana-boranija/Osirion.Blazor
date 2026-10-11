using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Shared.Components;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Cms.Admin.Tests.TestDoubles;

/// <summary>
/// Registers the services every admin component needs through <c>BaseComponent</c>.
/// </summary>
public abstract class AdminComponentTestContext : BunitContext
{
    protected AdminComponentTestContext()
    {
        Services.AddSingleton(Options.Create(new ThemingOptions()));
        Services.AddSingleton(Substitute.For<IEventPublisher>());
        Services.AddSingleton(Substitute.For<IEventSubscriber>());
        Services.AddSingleton(new CmsState());
        Services.AddSingleton<ILogger<BaseComponent>>(NullLogger<BaseComponent>.Instance);

        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    /// <summary>
    /// Renders as an interactive server circuit. Call it after the last service registration, because it builds
    /// the service provider.
    /// </summary>
    protected void RenderInteractively()
    {
        SetRendererInfo(new RendererInfo("Server", isInteractive: true));
    }
}
