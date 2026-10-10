using Bunit;
using Bunit.TestDoubles;
using Osirion.Blazor.Cms.Admin.Features.Layouts.Components;
using Osirion.Blazor.Cms.Admin.Features.Security;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Layouts.Components;

// Before the fix AdminPage declared a second "Theme" parameter next to OsirionComponentBase.Theme, so Blazor threw
// on its first render.
public class AdminPageTests : AdminComponentTestContext
{
    [Fact]
    public void AdminPage_UserWithAdminPolicy_RendersChildContent()
    {
        var authorization = this.AddTestAuthorization();
        authorization.SetAuthorized("alice");
        authorization.SetPolicies(CmsAdminAuthorization.PolicyName);
        RenderInteractively();

        var cut = RenderComponent<AdminPage>(parameters => parameters
            .Add(p => p.Title, "Content")
            .AddChildContent("<p id=\"page-body\">Body</p>"));

        cut.Find("#page-body").TextContent.ShouldBe("Body");
    }
}
