using System.Reflection;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Components.Pages;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Components;
using Osirion.Blazor.Cms.Admin.Features.Repository.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Pages;

public class DashboardTests : AdminComponentTestContext
{
    public DashboardTests()
    {
        ComponentFactories.AddStub<RepositorySelector>();
        ComponentFactories.AddStub<BranchSelector>();
        ComponentFactories.AddStub<FileExplorer>();
        RenderInteractively();
    }

    [Fact]
    public async Task Dashboard_Disposed_UnsubscribesFromCmsState()
    {
        // The circuit-scoped CmsState outlives the page; a handler left behind keeps every disposed dashboard
        // alive and re-renders it on each state change for the rest of the circuit.
        var state = Services.GetRequiredService<CmsState>();
        var subscribersBefore = CountStateChangedSubscribers(state);
        Render<Dashboard>();
        CountStateChangedSubscribers(state).ShouldBe(subscribersBefore + 1);

        await DisposeComponentsAsync();

        CountStateChangedSubscribers(state).ShouldBe(subscribersBefore);
    }

    private static int CountStateChangedSubscribers(CmsState state)
    {
        var field = typeof(CmsState).GetField("_stateChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;

        return (field.GetValue(state) as Delegate)?.GetInvocationList().Length ?? 0;
    }
}
