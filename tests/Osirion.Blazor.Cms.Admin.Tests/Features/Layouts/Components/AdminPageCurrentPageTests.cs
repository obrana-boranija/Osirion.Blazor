using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Components;
using Osirion.Blazor.Cms.Admin.Features.Security;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Layouts.Components;

public class AdminPageCurrentPageTests : AdminComponentTestContext
{
    private readonly CmsState _state = new();

    public AdminPageCurrentPageTests()
    {
        Services.AddSingleton(_state);
        var authorization = this.AddAuthorization();
        authorization.SetAuthorized("alice");
        authorization.SetPolicies(CmsAdminAuthorization.PolicyName);
        RenderInteractively();
    }

    [Fact]
    public void AdminPage_NoCurrentPage_TakesItFromTheLastPathSegment()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/osirion/settings");

        var cut = Render<AdminPage>();

        cut.Instance.CurrentPage.ShouldBe("Settings");
    }

    [Fact]
    public void AdminPage_RootUrlWithoutCurrentPage_LeavesItEmpty()
    {
        var cut = Render<AdminPage>();

        cut.Instance.CurrentPage.ShouldBeEmpty();
    }

    [Fact]
    public void AdminPage_ExplicitCurrentPage_IsKept()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/osirion/settings");

        var cut = Render<AdminPage>(parameters => parameters.Add(p => p.CurrentPage, "Preferences"));

        cut.Instance.CurrentPage.ShouldBe("Preferences");
    }

    [Fact]
    public async Task AdminPage_Disposed_StopsRenderingOnStateChanges()
    {
        var cut = Render<AdminPage>();
        var renders = cut.RenderCount;

        await DisposeComponentsAsync();
        _state.SetStatusMessage("changed");

        cut.RenderCount.ShouldBe(renders);
    }
}
