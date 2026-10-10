using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Components;
using Osirion.Blazor.Cms.Admin.Features.Dashboard.Components;
using Osirion.Blazor.Cms.Admin.Features.Repository.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Dashboard.Components;

// Before the fix CmsAdminDashboard declared a second "Theme" parameter next to OsirionComponentBase.Theme, so Blazor
// threw on its first render.
public class CmsAdminDashboardTests : AdminComponentTestContext
{
    [Fact]
    public void CmsAdminDashboard_Render_ShowsChildContent()
    {
        Services.AddSingleton(Substitute.For<IGitHubAdminService>());
        ComponentFactories.AddStub<RepositorySelector>();
        ComponentFactories.AddStub<BranchSelector>();
        ComponentFactories.AddStub<FileExplorer>();
        ComponentFactories.AddStub<Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.ContentEditor>();
        RenderInteractively();

        var cut = Render<CmsAdminDashboard>(parameters => parameters
            .AddChildContent("<p id=\"dashboard-body\">Body</p>"));

        cut.Find("#dashboard-body").TextContent.ShouldBe("Body");
    }
}
