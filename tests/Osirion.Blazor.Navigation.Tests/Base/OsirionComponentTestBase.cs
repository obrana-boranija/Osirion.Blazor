using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Tests.Base;

/// <summary>
/// Base test class for Osirion component tests: registers the services every component needs.
/// </summary>
public abstract class OsirionComponentTestBase : BunitContext
{
    protected OsirionComponentTestBase()
    {
        Services.AddSingleton(Options.Create(new ThemingOptions()));
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}
