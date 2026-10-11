using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Common.Constants;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.ViewModels;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentBrowser.ViewModels;

public class FileExplorerViewModelEdgeCaseTests
{
    private readonly IContentRepositoryAdapter _adapter = Substitute.For<IContentRepositoryAdapter>();
    private readonly CmsState _state = new();
    private readonly CmsEventMediator _mediator = new(NullLogger<CmsEventMediator>.Instance);

    private FileExplorerViewModel CreateViewModel(NavigationManager navigationManager)
    {
        var service = new ContentBrowserService(_adapter, _mediator, NullLogger<ContentBrowserService>.Instance);
        return new FileExplorerViewModel(service, _state, navigationManager, _mediator, NullLogger<FileExplorerViewModel>.Instance);
    }

    private sealed class FailingNavigationManager : NavigationManager
    {
        public FailingNavigationManager()
        {
            Initialize("https://admin.example/", "https://admin.example/");
        }

        protected override void NavigateToCore(string uri, NavigationOptions options) =>
            throw new InvalidOperationException("navigation refused");
    }

    [Fact]
    public void Contents_ReturnsTheItemsOfTheSharedState()
    {
        var viewModel = CreateViewModel(new TestNavigationManager());
        var items = new List<GitHubItem> { new() { Name = "a.md", Type = "file", Path = "a.md" } };

        _state.SetCurrentPath("docs", items);

        viewModel.Contents.ShouldBeSameAs(items);
        viewModel.CurrentPath.ShouldBe("docs");
    }

    [Fact]
    public async Task HandleItemClickAsync_NullItem_DoesNothing()
    {
        var viewModel = CreateViewModel(new TestNavigationManager());

        await viewModel.HandleItemClickAsync(null!);

        viewModel.SelectedItem.ShouldBeNull();
    }

    [Fact]
    public async Task OpenFileAsync_Directory_DoesNotStartLoading()
    {
        var navigation = new TestNavigationManager();
        var viewModel = CreateViewModel(navigation);
        var notifications = 0;
        viewModel.StateChanged += () => notifications++;

        await viewModel.OpenFileAsync(new GitHubItem { Name = "blog", Type = "dir", Path = "blog" });

        notifications.ShouldBe(0);
        navigation.Navigations.ShouldBeEmpty();
    }

    [Fact]
    public async Task OpenFileAsync_NavigationFails_SetsErrorMessageOnViewModelAndState()
    {
        var viewModel = CreateViewModel(new FailingNavigationManager());

        await viewModel.OpenFileAsync(new GitHubItem { Name = "post.md", Type = "file", Path = "post.md" });

        viewModel.ErrorMessage.ShouldBe(UserMessages.OpenFailed);
        viewModel.ErrorMessage!.ShouldNotContain("navigation refused");
        _state.ErrorMessage.ShouldBe(viewModel.ErrorMessage);
        viewModel.IsLoading.ShouldBeFalse();
    }

    [Fact]
    public async Task OpenFileAsync_MarkdownFile_NavigatesToTheEditorWithThePath()
    {
        var navigation = new TestNavigationManager();
        var viewModel = CreateViewModel(navigation);

        await viewModel.OpenFileAsync(new GitHubItem { Name = "post.md", Type = "file", Path = "blog/post.md" });

        navigation.Navigations.ShouldHaveSingleItem().Uri.ShouldBe("/osirion/content/edit?Path=blog%2Fpost.md");
    }

    // Before the fix the path went into the query unencoded, so "&", "#" and "+" cut or changed it.
    [Theory]
    [InlineData("blog/q&a.md", "/osirion/content/edit?Path=blog%2Fq%26a.md")]
    [InlineData("blog/c#.md", "/osirion/content/edit?Path=blog%2Fc%23.md")]
    [InlineData("blog/c++.md", "/osirion/content/edit?Path=blog%2Fc%2B%2B.md")]
    public async Task OpenFileAsync_PathWithQueryCharacters_IsEncodedInTheEditorUrl(string path, string expected)
    {
        var navigation = new TestNavigationManager();
        var viewModel = CreateViewModel(navigation);

        await viewModel.OpenFileAsync(new GitHubItem { Name = Path.GetFileName(path), Type = "file", Path = path });

        navigation.Navigations.ShouldHaveSingleItem().Uri.ShouldBe(expected);
    }

    [Fact]
    public async Task DeleteFileAsync_NothingPending_ClosesTheConfirmationWithoutDeleting()
    {
        var viewModel = CreateViewModel(new TestNavigationManager());

        await viewModel.DeleteFileAsync();

        viewModel.IsShowingDeleteConfirmation.ShouldBeFalse();
        await _adapter.DidNotReceiveWithAnyArgs().DeleteFileAsync(default!, default!, default!);
    }
}
