using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.Repository.Services;
using Osirion.Blazor.Cms.Admin.Features.Repository.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.Repository.ViewModels;

public class BranchSelectorViewModelBehaviorTests
{
    private readonly IContentRepositoryAdapter _adapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly CmsState _state = new();
    private readonly CmsEventMediator _mediator = new(NullLogger<CmsEventMediator>.Instance);
    private readonly BranchSelectorViewModel _viewModel;
    private readonly GitHubRepository _repository = new() { Name = "site", FullName = "owner/site", DefaultBranch = "main" };
    private readonly List<GitHubBranch> _branches = [new() { Name = "main" }, new() { Name = "dev" }];

    public BranchSelectorViewModelBehaviorTests()
    {
        var service = new RepositoryService(_adapter, NullLogger<RepositoryService>.Instance);
        _viewModel = new BranchSelectorViewModel(service, _state, _mediator);
        _adapter.GetBranchesAsync("site").Returns(_ => _branches.ToList());
    }

    private async Task LoadBranchesAsync()
    {
        _state.SelectRepository(_repository);
        await _viewModel.RefreshBranchesAsync();
    }

    [Fact]
    public async Task RefreshBranchesAsync_NoRepository_DoesNothing()
    {
        await _viewModel.RefreshBranchesAsync();

        await _adapter.DidNotReceiveWithAnyArgs().GetBranchesAsync(default!);
        _viewModel.Branches.ShouldBeEmpty();
    }

    [Fact]
    public async Task RefreshBranchesAsync_BranchesFound_DefaultsTheBaseBranchToTheRepositoryDefault()
    {
        await LoadBranchesAsync();

        _viewModel.Branches.Select(branch => branch.Name).ShouldBe(["main", "dev"]);
        _viewModel.BaseBranchName.ShouldBe("main");
        _viewModel.IsLoading.ShouldBeFalse();
    }

    [Fact]
    public async Task RefreshBranchesAsync_BaseBranchAlreadyChosen_KeepsIt()
    {
        _viewModel.BaseBranchName = "dev";

        await LoadBranchesAsync();

        _viewModel.BaseBranchName.ShouldBe("dev");
    }

    [Fact]
    public async Task RefreshBranchesAsync_NoBranches_LeavesTheBaseBranchEmpty()
    {
        _branches.Clear();

        await LoadBranchesAsync();

        _viewModel.BaseBranchName.ShouldBeEmpty();
    }

    [Fact]
    public async Task SelectBranchAsync_EmptyName_ClearsTheSelection()
    {
        await LoadBranchesAsync();
        _state.SelectBranch(_branches[0]);

        await _viewModel.SelectBranchAsync(" ");

        _viewModel.SelectedBranch.ShouldBeNull();
    }

    [Fact]
    public async Task SelectBranchAsync_UnknownBranch_ChangesNothing()
    {
        await LoadBranchesAsync();

        await _viewModel.SelectBranchAsync("missing");

        _viewModel.SelectedBranch.ShouldBeNull();
        _adapter.DidNotReceiveWithAnyArgs().SetBranch(default!);
    }

    [Fact]
    public async Task SelectBranchAsync_KnownBranch_SelectsConfiguresAndPublishes()
    {
        var published = EventRecorder.Record<BranchSelectedEvent>(_mediator);
        await LoadBranchesAsync();

        await _viewModel.SelectBranchAsync("dev");

        _viewModel.SelectedBranch!.Name.ShouldBe("dev");
        _adapter.Received(1).SetBranch("dev");
        published.ShouldHaveSingleItem().Branch.Name.ShouldBe("dev");
        _viewModel.IsLoading.ShouldBeFalse();
    }

    [Fact]
    public async Task SelectBranchAsync_AdapterFails_ShowsTheGenericMessage()
    {
        await LoadBranchesAsync();
        _adapter.When(adapter => adapter.SetBranch("dev")).Do(_ => throw new InvalidOperationException("token ghp_secret"));

        await _viewModel.SelectBranchAsync("dev");

        _viewModel.ErrorMessage.ShouldBe(UserMessages.BranchOperationFailed);
        _state.ErrorMessage.ShouldBe(UserMessages.BranchOperationFailed);
        _viewModel.IsLoading.ShouldBeFalse();
    }

    [Fact]
    public void SetCreatingNewBranch_TrueWithRepository_OpensTheFormWithTheDefaultBase()
    {
        _state.SelectRepository(_repository);

        _viewModel.SetCreatingNewBranch(true);

        _viewModel.IsCreatingNewBranch.ShouldBeTrue();
        _viewModel.BaseBranchName.ShouldBe("main");
    }

    [Fact]
    public void SetCreatingNewBranch_TrueWithoutRepository_ResetsTheName()
    {
        _viewModel.NewBranchName = "draft";

        _viewModel.SetCreatingNewBranch(true);

        _viewModel.IsCreatingNewBranch.ShouldBeTrue();
        _viewModel.NewBranchName.ShouldBeEmpty();
    }

    [Fact]
    public void SetCreatingNewBranch_False_ClosesTheFormAndResetsTheName()
    {
        _viewModel.NewBranchName = "draft";
        var notifications = 0;
        _viewModel.StateChanged += () => notifications++;

        _viewModel.SetCreatingNewBranch(false);

        _viewModel.IsCreatingNewBranch.ShouldBeFalse();
        _viewModel.NewBranchName.ShouldBeEmpty();
        notifications.ShouldBe(1);
    }

    [Theory]
    [InlineData("", "main")]
    [InlineData("feature", "")]
    public async Task CreateBranchAsync_MissingName_DoesNotCallTheRepository(string name, string baseBranch)
    {
        _viewModel.NewBranchName = name;
        _viewModel.BaseBranchName = baseBranch;

        await _viewModel.CreateBranchAsync();

        await _adapter.DidNotReceiveWithAnyArgs().CreateBranchAsync(default!, default!);
    }

    [Fact]
    public async Task CreateBranchAsync_Succeeds_RefreshesSelectsTheNewBranchAndClosesTheForm()
    {
        await LoadBranchesAsync();
        _viewModel.SetCreatingNewBranch(true);
        _viewModel.NewBranchName = "feature";
        _adapter.CreateBranchAsync("feature", "main").Returns(new GitHubBranch { Name = "feature" });
        _branches.Add(new GitHubBranch { Name = "feature" });

        await _viewModel.CreateBranchAsync();

        _viewModel.SelectedBranch!.Name.ShouldBe("feature");
        _viewModel.IsCreatingNewBranch.ShouldBeFalse();
        _viewModel.IsCreatingBranch.ShouldBeFalse();
        _viewModel.NewBranchName.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateBranchAsync_RepositoryFails_ShowsTheGenericMessageAndKeepsTheForm()
    {
        await LoadBranchesAsync();
        _viewModel.SetCreatingNewBranch(true);
        _viewModel.NewBranchName = "feature";
        _adapter.CreateBranchAsync("feature", "main").ThrowsAsync(new InvalidOperationException("exists"));

        await _viewModel.CreateBranchAsync();

        _viewModel.ErrorMessage.ShouldBe(UserMessages.BranchOperationFailed);
        _viewModel.IsCreatingNewBranch.ShouldBeTrue();
        _viewModel.IsCreatingBranch.ShouldBeFalse();
    }
}
