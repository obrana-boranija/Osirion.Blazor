using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Components;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Repository.Components;

public class BranchSelectorTests : AdminComponentTestContext
{
    private readonly IContentRepositoryAdapter _adapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly CmsState _state = new();
    private readonly List<GitHubBranch> _branches = [new() { Name = "main" }, new() { Name = "dev" }];

    public BranchSelectorTests()
    {
        var mediator = new CmsEventMediator(NullLogger<CmsEventMediator>.Instance);
        var service = new RepositoryService(_adapter, NullLogger<RepositoryService>.Instance);
        Services.AddSingleton(_state);
        Services.AddSingleton(new BranchSelectorViewModel(service, _state, mediator));
        _adapter.GetBranchesAsync("site").Returns(_ => _branches.ToList());
    }

    private void SelectRepository() =>
        _state.SelectRepository(new GitHubRepository { Name = "site", DefaultBranch = "main" });

    [Fact]
    public void BranchSelector_NoRepository_ShowsTheEmptyStateAndTitle()
    {
        var cut = Render<BranchSelector>(parameters => parameters.Add(p => p.Title, "Branches"));

        cut.Find(".card-title").TextContent.ShouldBe("Branches");
        cut.Markup.ShouldContain("No branches found. Please select a repository first.");
    }

    [Fact]
    public void BranchSelector_Loading_ShowsSpinner()
    {
        SelectRepository();
        var pending = new TaskCompletionSource<List<GitHubBranch>>();
        _adapter.GetBranchesAsync("site").Returns(pending.Task);

        var cut = Render<BranchSelector>();

        cut.Markup.ShouldContain("Loading branches...");
        pending.SetResult([]);
    }

    [Fact]
    public void BranchSelector_Branches_ListsThemAfterThePrompt()
    {
        SelectRepository();

        var cut = Render<BranchSelector>(parameters => parameters.Add(p => p.SelectPrompt, "Pick one"));

        var options = cut.FindAll("select.form-select option").Select(option => option.TextContent).ToList();
        options.ShouldBe(["Pick one", "main", "dev"]);
        cut.Find("select.form-select").GetAttribute("value").ShouldBe(string.Empty);
        cut.Markup.ShouldContain("New Branch");
    }

    [Fact]
    public void BranchSelector_CreateNotAllowed_HidesTheFooter()
    {
        SelectRepository();

        var cut = Render<BranchSelector>(parameters => parameters.Add(p => p.AllowCreateBranch, false));

        cut.FindAll(".card-footer").ShouldBeEmpty();
    }

    [Fact]
    public void BranchSelector_LoadFails_ShowsTheErrorAndRetryReloads()
    {
        SelectRepository();
        _adapter.GetBranchesAsync("site").ThrowsAsync(new InvalidOperationException("offline"));
        RenderInteractively();
        var cut = Render<BranchSelector>();
        cut.Find(".alert-danger").TextContent.ShouldContain(UserMessages.BranchOperationFailed);
        _adapter.GetBranchesAsync("site").Returns(_ => _branches.ToList());

        cut.FindAll("button").Single(button => button.TextContent.Trim() == "Retry").Click();

        cut.FindAll(".alert-danger").ShouldBeEmpty();
        cut.FindAll("select.form-select option").Count.ShouldBe(3);
    }

    [Fact]
    public void BranchSelector_RefreshButton_ReloadsTheBranches()
    {
        SelectRepository();
        RenderInteractively();
        var cut = Render<BranchSelector>();

        cut.Find("button[title='Refresh branches']").Click();

        _adapter.Received(2).GetBranchesAsync("site");
    }

    [Fact]
    public void BranchSelector_SelectBranch_SelectsItAndNotifiesTheParent()
    {
        SelectRepository();
        GitHubBranch? changed = null;
        RenderInteractively();
        var cut = Render<BranchSelector>(parameters => parameters
            .Add(p => p.OnBranchChange, (GitHubBranch branch) => changed = branch));

        cut.Find("select.form-select").Change("dev");

        _state.SelectedBranch!.Name.ShouldBe("dev");
        changed!.Name.ShouldBe("dev");
        _adapter.Received(1).SetBranch("dev");
    }

    [Fact]
    public void BranchSelector_SelectPrompt_ClearsTheBranchWithoutNotifying()
    {
        SelectRepository();
        var notified = false;
        RenderInteractively();
        var cut = Render<BranchSelector>(parameters => parameters
            .Add(p => p.OnBranchChange, (GitHubBranch _) => notified = true));

        cut.Find("select.form-select").Change(string.Empty);

        _state.SelectedBranch.ShouldBeNull();
        notified.ShouldBeFalse();
    }

    [Fact]
    public void BranchSelector_SelectWithoutListener_StillSelects()
    {
        SelectRepository();
        RenderInteractively();
        var cut = Render<BranchSelector>();

        cut.Find("select.form-select").Change("main");

        _state.SelectedBranch!.Name.ShouldBe("main");
    }

    [Fact]
    public void BranchSelector_NewBranchForm_OpensWithDisabledCreateAndCancels()
    {
        SelectRepository();
        RenderInteractively();
        var cut = Render<BranchSelector>();

        cut.Find(".btn-outline-primary").Click();
        cut.Find("#newBranchName").ShouldNotBeNull();
        cut.Find(".card-footer .btn-primary").HasAttribute("disabled").ShouldBeTrue();
        cut.FindAll("#baseBranch option").Select(option => option.TextContent).ShouldBe(["main", "dev"]);
        cut.FindAll(".card-footer button").Single(button => button.TextContent.Trim() == "Cancel").Click();

        cut.FindAll("#newBranchName").ShouldBeEmpty();
    }

    [Fact]
    public void BranchSelector_CreateBranch_ShowsProgressThenSelectsTheNewBranch()
    {
        SelectRepository();
        GitHubBranch? changed = null;
        var pendingCreate = new TaskCompletionSource<GitHubBranch>();
        _adapter.CreateBranchAsync("feature", "dev").Returns(pendingCreate.Task);
        RenderInteractively();
        var cut = Render<BranchSelector>(parameters => parameters
            .Add(p => p.OnBranchChange, (GitHubBranch branch) => changed = branch));

        cut.Find(".btn-outline-primary").Click();
        cut.Find("#newBranchName").Change("feature");
        cut.Find("#baseBranch").Change("dev");
        cut.Find(".card-footer .btn-primary").Click();
        cut.Find(".card-footer").TextContent.ShouldContain("Creating...");
        _branches.Add(new GitHubBranch { Name = "feature" });
        pendingCreate.SetResult(new GitHubBranch { Name = "feature" });

        cut.WaitForAssertion(() => changed.ShouldNotBeNull());
        changed!.Name.ShouldBe("feature");
        _state.SelectedBranch!.Name.ShouldBe("feature");
    }

    [Fact]
    public void BranchSelector_CreateBranchFailsWithoutListener_ShowsTheError()
    {
        SelectRepository();
        _adapter.CreateBranchAsync("feature", "main").ThrowsAsync(new InvalidOperationException("exists"));
        RenderInteractively();
        var cut = Render<BranchSelector>();

        cut.Find(".btn-outline-primary").Click();
        cut.Find("#newBranchName").Change("feature");
        cut.Find(".card-footer .btn-primary").Click();

        cut.Find(".alert-danger").TextContent.ShouldContain(UserMessages.BranchOperationFailed);
    }

    [Fact]
    public async Task BranchSelector_Dispose_StopsListeningToTheViewModel()
    {
        var cut = Render<BranchSelector>();
        var renders = cut.RenderCount;

        await DisposeComponentsAsync();
        _state.SetStatusMessage("changed");

        cut.RenderCount.ShouldBe(renders);
    }
}
