using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Core.State;

public class CmsStateMutationTests
{
    private readonly CmsState _state = new();
    private int _notifications;

    public CmsStateMutationTests()
    {
        _state.StateChanged += () => _notifications++;
    }

    [Fact]
    public void SetRepositories_StoresThemAndNotifies()
    {
        var repositories = new List<GitHubRepository> { new() { Name = "site" } };

        _state.SetRepositories(repositories);

        _state.AvailableRepositories.ShouldBeSameAs(repositories);
        _notifications.ShouldBe(1);
    }

    [Fact]
    public void SetBranches_StoresThemAndNotifies()
    {
        var branches = new List<GitHubBranch> { new() { Name = "main" } };

        _state.SetBranches(branches);

        _state.AvailableBranches.ShouldBeSameAs(branches);
        _notifications.ShouldBe(1);
    }

    [Fact]
    public void SelectItem_StoresItAndNotifies()
    {
        var item = new GitHubItem { Path = "post.md" };

        _state.SelectItem(item);

        _state.SelectedItem.ShouldBeSameAs(item);
        _notifications.ShouldBe(1);
    }

    [Fact]
    public void SetSaving_StoresTheFlagAndNotifies()
    {
        _state.SetSaving(true);

        _state.IsSaving.ShouldBeTrue();
        _notifications.ShouldBe(1);
    }

    [Fact]
    public void SetStatusMessage_ClearsTheErrorAndSetErrorMessageClearsTheStatus()
    {
        _state.SetErrorMessage("Broken");
        _state.SetStatusMessage("Saved");

        _state.ErrorMessage.ShouldBeNull();
        _state.StatusMessage.ShouldBe("Saved");

        _state.SetErrorMessage("Broken again");

        _state.StatusMessage.ShouldBeNull();
        _state.ErrorMessage.ShouldBe("Broken again");
    }

    [Fact]
    public void ClearMessages_ClearsBothAndNotifies()
    {
        _state.SetStatusMessage("Saved");

        _state.ClearMessages();

        _state.StatusMessage.ShouldBeNull();
        _state.ErrorMessage.ShouldBeNull();
        _notifications.ShouldBe(2);
    }

    [Fact]
    public void StateChanged_RemovedHandler_IsNotCalled()
    {
        var calls = 0;
        void Handler() => calls++;
        _state.StateChanged += Handler;
        _state.StateChanged -= Handler;

        _state.SetSaving(true);

        calls.ShouldBe(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("null")]
    [InlineData("{ not json")]
    public void DeserializeFrom_UnusableInput_KeepsTheStateWithoutNotifying(string serialized)
    {
        _state.SetCurrentPath("blog", []);
        _notifications = 0;

        _state.DeserializeFrom(serialized);

        _state.CurrentPath.ShouldBe("blog");
        _notifications.ShouldBe(0);
    }

    [Fact]
    public void DeserializeFrom_NonStringPathAndTheme_KeepsTheirCurrentValues()
    {
        _state.SetCurrentPath("blog", []);

        _state.DeserializeFrom("{\"CurrentPath\":42,\"CurrentTheme\":true}");

        _state.CurrentPath.ShouldBe("blog");
        _state.CurrentTheme.ShouldBe("light");
    }

    [Fact]
    public void SerializeThenDeserialize_RoundTripsTheSelections()
    {
        _state.SelectRepository(new GitHubRepository { Name = "site" });
        _state.SelectBranch(new GitHubBranch { Name = "main" });
        _state.SetCurrentPath("blog", []);
        var restored = new CmsState();

        restored.DeserializeFrom(_state.Serialize());

        restored.SelectedRepository!.Name.ShouldBe("site");
        restored.SelectedBranch!.Name.ShouldBe("main");
        restored.CurrentPath.ShouldBe("blog");
        restored.CurrentTheme.ShouldBe("light");
    }
}
