using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
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

public class BranchSelectorViewModelTests
{
    private readonly IContentRepositoryAdapter _adapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly CmsState _state = new();
    private readonly CmsEventMediator _mediator = new(NullLogger<CmsEventMediator>.Instance);
    private readonly RecordingLogger<BranchSelectorViewModel> _logger = new();
    private readonly BranchSelectorViewModel _viewModel;
    private readonly GitHubRepository _repository = new() { Name = "site", FullName = "owner/site", DefaultBranch = "main" };

    public BranchSelectorViewModelTests()
    {
        var service = new RepositoryService(_adapter, NullLogger<RepositoryService>.Instance);
        _viewModel = new BranchSelectorViewModel(service, _state, _mediator, _logger);
        _state.SelectRepository(_repository);
    }

    [Fact]
    public async Task RefreshBranchesAsync_ProviderFails_ShowsGenericMessageAndLogsTheException()
    {
        _adapter.GetBranchesAsync("site").Returns(Task.FromException<List<GitHubBranch>>(new InvalidOperationException("401 for token ghp_secret")));

        await _viewModel.RefreshBranchesAsync();

        _viewModel.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
        _viewModel.ErrorMessage!.ShouldNotContain("ghp_secret");
        _state.ErrorMessage!.ShouldNotContain("ghp_secret");
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception != null);
    }

    [Fact]
    public async Task RepositorySelectedEvent_LoadsBranchesForTheNewRepository()
    {
        var branches = new List<GitHubBranch> { new() { Name = "main" } };
        _adapter.GetBranchesAsync("site").Returns(branches);

        _mediator.Publish(new RepositorySelectedEvent(_repository));

        await _adapter.Received(1).GetBranchesAsync("site");
        _viewModel.Branches.ShouldBe(branches);
    }

    [Fact]
    public void Dispose_StopsReactingToStateAndRepositoryEvents()
    {
        var notifications = 0;
        _viewModel.StateChanged += () => notifications++;
        _viewModel.Dispose();

        _state.SelectRepository(_repository);
        _mediator.Publish(new RepositorySelectedEvent(_repository));

        notifications.ShouldBe(0);
        _adapter.DidNotReceiveWithAnyArgs().GetBranchesAsync(default!);
    }
}
