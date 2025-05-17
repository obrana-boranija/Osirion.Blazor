using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Osirion.Blazor.Components;

namespace Osirion.Blazor.Tests.Base;

/// <summary>
/// Base test class for Osirion component tests
/// Handles common setup for all component tests
/// </summary>
public abstract class OsirionComponentTestBase : TestContext
{
    protected OsirionComponentTestBase()
    {
        // Add default ThemingOptions
        Services.AddSingleton(Options.Create(new ThemingOptions()));

        // Set default JSInterop mode
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Use parameter to set interactivity (works in both .NET 8 and 9)
        SetupClientSide();
    }

    /// <summary>
    /// Setup the test context for server-side rendering (non-interactive)
    /// </summary>
    protected void SetupServerSide()
    {
        // For OsirionComponentBase in .NET 8, we need to pass the SetInteractive parameter
        AddGlobalParameter("SetInteractive", false);
    }

    /// <summary>
    /// Setup the test context for client-side rendering (interactive)
    /// </summary>
    protected void SetupClientSide()
    {
        // For OsirionComponentBase in .NET 8, we need to pass the SetInteractive parameter
        AddGlobalParameter("SetInteractive", true);
    }

    /// <summary>
    /// Adds a global parameter to the test context
    /// </summary>
    private void AddGlobalParameter(string name, object value)
    {
        // Fix: Use a wrapper class to store the ComponentParameter since it is a struct
        Services.AddSingleton(new ComponentParameterWrapper(ComponentParameter.CreateParameter(name, value)));
    }

    /// <summary>
    /// Wrapper class to store ComponentParameter as a reference type
    /// </summary>
    private class ComponentParameterWrapper
    {
        public ComponentParameter Parameter { get; }

        public ComponentParameterWrapper(ComponentParameter parameter)
        {
            Parameter = parameter;
        }
    }
}
