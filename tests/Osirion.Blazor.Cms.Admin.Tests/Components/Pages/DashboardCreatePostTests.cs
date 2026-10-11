using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Osirion.Blazor.Cms.Admin.Components.Pages;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Components;
using Osirion.Blazor.Cms.Admin.Features.Repository.Components;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Pages;

public class DashboardCreatePostTests : AdminComponentTestContext
{
    private readonly CmsState _state = new();
    private readonly List<CreateNewContentEvent> _created;

    public DashboardCreatePostTests()
    {
        var mediator = new CmsEventMediator(NullLogger<CmsEventMediator>.Instance);
        _created = EventRecorder.Record<CreateNewContentEvent>(mediator);
        Services.AddSingleton(_state);
        Services.AddSingleton<IEventPublisher>(mediator);
        ComponentFactories.AddStub<RepositorySelector>();
        ComponentFactories.AddStub<BranchSelector>();
        ComponentFactories.AddStub<FileExplorer>();
        RenderInteractively();
    }

    private void SelectRepositoryAndBranch(string path = "")
    {
        _state.SelectRepository(new GitHubRepository { Name = "site" });
        _state.SelectBranch(new GitHubBranch { Name = "main" });
        if (path.Length > 0)
        {
            _state.SetCurrentPath(path, []);
        }
    }

    [Fact]
    public void Dashboard_NotConfigured_ShowsSetupNoticeAndDisablesCreateButtons()
    {
        var cut = Render<Dashboard>();

        cut.Markup.ShouldContain("Repository Setup Required");
        cut.FindAll("button[disabled]").Count.ShouldBe(2);
    }

    [Fact]
    public async Task Dashboard_CreateFromExplorerWhenNotConfigured_ShowsAnErrorAndStays()
    {
        var cut = Render<Dashboard>();
        var onCreate = cut.FindComponent<Stub<FileExplorer>>().Instance.Parameters.Get(p => p.OnCreateFile);

        await cut.InvokeAsync(() => onCreate.InvokeAsync());

        _state.ErrorMessage.ShouldBe("Please select a repository and branch before creating content.");
        _state.IsEditing.ShouldBeFalse();
        _created.ShouldBeEmpty();
    }

    [Fact]
    public void Dashboard_Configured_HidesTheNoticeAndEnablesCreate()
    {
        SelectRepositoryAndBranch();

        var cut = Render<Dashboard>();

        cut.Markup.ShouldNotContain("Repository Setup Required");
        cut.FindAll("button[disabled]").ShouldBeEmpty();
    }

    [Fact]
    public void Dashboard_NewInRoot_StartsEditingPublishesAndNavigatesToTheEditor()
    {
        SelectRepositoryAndBranch();
        var cut = Render<Dashboard>();

        cut.Find("button.btn-primary").Click();

        _state.EditingPost!.Path.ShouldBe("new-post.md");
        _state.IsCreatingNewFile.ShouldBeTrue();
        _created.ShouldHaveSingleItem().Directory.ShouldBe(string.Empty);
        Services.GetRequiredService<NavigationManager>().Uri.ShouldEndWith("/osirion/content/edit");
    }

    [Fact]
    public void Dashboard_QuickActionInDirectory_PlacesThePostInThatDirectory()
    {
        SelectRepositoryAndBranch("blog");
        var cut = Render<Dashboard>();

        cut.FindAll("button").First(button => button.TextContent.Contains("Create New Content")).Click();

        _state.EditingPost!.Path.ShouldBe("blog/new-post.md");
        _created.ShouldHaveSingleItem().Directory.ShouldBe("blog");
    }
}
