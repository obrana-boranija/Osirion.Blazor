using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Services.State;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Services.State;

// StateManager is obsolete but still registered, so it keeps working until it is removed.
#pragma warning disable CS0618
public class StateManagerTests
{
    private const string StateKey = "osirion_cms_admin_state";

    private readonly CmsState _state = new();
    private readonly IStateStorageService _storage = Substitute.For<IStateStorageService>();
    private readonly CmsEventMediator _mediator = new(NullLogger<CmsEventMediator>.Instance);
    private readonly CmsAdminOptions _options = new() { PersistUserSelections = true };

    [Fact]
    public async Task InitializeAsync_PersistenceOff_DoesNotTouchStorage()
    {
        _options.PersistUserSelections = false;

        await CreateManager().InitializeAsync();

        await _storage.DidNotReceive().InitializeAsync();
    }

    [Fact]
    public async Task InitializeAsync_SavedState_RestoresIt()
    {
        var saved = new CmsState();
        saved.SelectRepository(new GitHubRepository { Name = "site" });
        _storage.IsInitialized.Returns(true);
        _storage.GetStateAsync<string>(StateKey).Returns(saved.Serialize());

        await CreateManager().InitializeAsync();

        _state.SelectedRepository.ShouldNotBeNull().Name.ShouldBe("site");
    }

    [Fact]
    public async Task InitializeAsync_NothingSaved_KeepsTheCurrentState()
    {
        _storage.IsInitialized.Returns(true);
        _storage.GetStateAsync<string>(StateKey).Returns((string?)null);

        await CreateManager().InitializeAsync();

        _state.SelectedRepository.ShouldBeNull();
    }

    [Fact]
    public async Task InitializeAsync_SecondCall_LoadsOnlyOnce()
    {
        _storage.IsInitialized.Returns(true);
        var manager = CreateManager();

        await manager.InitializeAsync();
        await manager.InitializeAsync();

        await _storage.Received(1).GetStateAsync<string>(StateKey);
    }

    [Fact]
    public async Task InitializeAsync_StorageUnavailable_TriesAgainNextTime()
    {
        _storage.IsInitialized.Returns(false);
        var manager = CreateManager();

        await manager.InitializeAsync();
        await manager.InitializeAsync();

        await _storage.Received(2).InitializeAsync();
        await _storage.DidNotReceive().GetStateAsync<string>(Arg.Any<string>());
    }

    [Fact]
    public async Task InitializeAsync_StorageThrows_IsSwallowed()
    {
        _storage.InitializeAsync().ThrowsAsync(new InvalidOperationException("no storage"));

        await Should.NotThrowAsync(() => CreateManager().InitializeAsync());
    }

    [Fact]
    public void StateChange_WithStorage_SavesTheSerializedState()
    {
        _storage.IsInitialized.Returns(true);
        _ = CreateManager();

        _state.SelectRepository(new GitHubRepository { Name = "site" });

        _storage.Received().SaveStateAsync(StateKey, Arg.Is<string>(json => json.Contains("site")));
    }

    [Fact]
    public void StateChange_StorageNotReady_SavesNothing()
    {
        _storage.IsInitialized.Returns(false);
        _ = CreateManager();

        _state.SelectRepository(new GitHubRepository { Name = "site" });

        _storage.DidNotReceive().SaveStateAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void StateResetRequested_ClearsStorageAndState()
    {
        _storage.IsInitialized.Returns(true);
        _state.SelectRepository(new GitHubRepository { Name = "site" });
        _ = CreateManager();

        _mediator.Publish(new StateResetRequestedEvent());

        _storage.Received(1).RemoveStateAsync(StateKey);
        _state.SelectedRepository.ShouldBeNull();
    }

    [Fact]
    public void Dispose_StopsSavingOnCmsStateChanges()
    {
        _storage.IsInitialized.Returns(true);
        CreateManager().Dispose();

        _state.SelectRepository(new GitHubRepository { Name = "site" });

        _storage.DidNotReceive().SaveStateAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    // Known defect, pinned as current behavior: Dispose leaves the mediator subscriptions in place, so a disposed manager still saves on selection events. Update this expectation when it is fixed.
    [Fact]
    public void Dispose_MediatorEventAfterDispose_StillSaves()
    {
        _storage.IsInitialized.Returns(true);
        CreateManager().Dispose();

        _mediator.Publish(new RepositorySelectedEvent(new GitHubRepository { Name = "site" }));

        _storage.Received(1).SaveStateAsync(StateKey, Arg.Any<string>());
    }

    private StateManager CreateManager() =>
        new(_state, _storage, _mediator, Options.Create(_options), NullLogger<StateManager>.Instance);
}
#pragma warning restore CS0618
