using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Features.Navigation.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Navigation;

public class DefaultNavigationTests : AdminComponentTestContext
{
    public DefaultNavigationTests()
    {
        SetRendererInfo(new RendererInfo("Static", false));
    }

    [Fact]
    public void DefaultNavigation_OnSettingsPage_MarksSettingsActive()
    {
        Services.GetRequiredService<NavigationManager>().NavigateTo("/osirion/settings");

        var cut = Render<DefaultNavigation>();

        cut.Find("a[href='/admin/settings']").ClassList.ShouldContain("active");
        cut.Find("a[href='/admin/users']").ClassList.ShouldNotContain("active");
    }

    [Fact]
    public void DefaultNavigation_OutsideTheAdmin_MarksNothingActive()
    {
        var cut = Render<DefaultNavigation>();

        cut.FindAll("a.active").ShouldBeEmpty();
    }
}
