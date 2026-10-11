using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Components.Pages;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Pages;

public class EditContentNavigationTests : AdminComponentTestContext
{
    private const string EditPage = "osirion/content/edit";

    private readonly IGitHubAdminService _gitHubService = Substitute.For<IGitHubAdminService>();

    public EditContentNavigationTests()
    {
        Services.AddSingleton(_gitHubService);
        Services.AddSingleton<ILogger<EditContent>>(new RecordingLogger<EditContent>());
        ComponentFactories.AddStub<ContentEditor>();
        RenderInteractively();
    }

    private CmsState State => Services.GetRequiredService<CmsState>();

    private BunitNavigationManager Navigation => Services.GetRequiredService<BunitNavigationManager>();

    private IRenderedComponent<EditContent> RenderWithoutPath()
    {
        Navigation.NavigateTo(EditPage);
        return Render<EditContent>();
    }

    private string CurrentRelativeUri => Navigation.ToBaseRelativePath(Navigation.Uri);

    private static ContentItem CreatePost(string path, string title = "Title", string description = "Description") => new()
    {
        Path = path,
        Content = "Body",
        Metadata = FrontMatter.Create(title, description, new DateTime(2024, 1, 15))
    };

    private static GitHubItem File(string name, string path) => new() { Name = name, Path = path, Type = "file" };

    [Fact]
    public void EditContent_StaticRenderWithoutSelection_ShowsEmptyState()
    {
        SetRendererInfo(new RendererInfo("Static", false));

        var cut = RenderWithoutPath();

        cut.Find("h1").TextContent.ShouldBe("Edit Content");
        cut.Find("p.text-muted").TextContent.ShouldBe("Edit your markdown content");
        cut.Markup.ShouldContain("No Content Selected");
        CurrentRelativeUri.ShouldBe(EditPage);
    }

    [Fact]
    public void EditContent_NoPathWithSelectedMarkdownFile_RedirectsToIt()
    {
        State.SelectItem(File("a.md", "blog/a.md"));

        RenderWithoutPath();

        CurrentRelativeUri.ShouldBe($"{EditPage}?Path=blog/a.md");
    }

    [Fact]
    public void EditContent_NoPathWithSelectedNonMarkdownAndEditedPost_RedirectsToThePost()
    {
        State.SelectItem(File("image.png", "img/image.png"));
        State.SetEditingPost(CreatePost("blog/b.md"));

        RenderWithoutPath();

        CurrentRelativeUri.ShouldBe($"{EditPage}?Path=blog/b.md");
    }

    [Fact]
    public void EditContent_NoPathWhileCreatingNewFile_StaysOnPage()
    {
        State.SetEditingPost(CreatePost("blog/new.md"), isNew: true);

        var cut = RenderWithoutPath();

        CurrentRelativeUri.ShouldBe(EditPage);
        cut.Find("h1").TextContent.ShouldBe("New Post");
        cut.Find("p.text-muted").TextContent.ShouldBe("Create a new markdown file");
        cut.FindComponent<Stub<ContentEditor>>().ShouldNotBeNull();
    }

    [Fact]
    public void EditContent_NoPathWithEditedPostWithoutPath_StaysOnPage()
    {
        State.SetEditingPost(CreatePost(" "));

        RenderWithoutPath();

        CurrentRelativeUri.ShouldBe(EditPage);
    }

    [Fact]
    public void EditContent_NoPathAfterEditingWasCleared_StaysOnPage()
    {
        State.SetEditingPost(CreatePost("blog/b.md"));
        State.ClearEditing();

        var cut = RenderWithoutPath();

        CurrentRelativeUri.ShouldBe(EditPage);
        cut.Markup.ShouldContain("No Content Selected");
    }

    [Fact]
    public void EditContent_EditingExistingPost_ShowsItsTitleAndDescription()
    {
        State.SetEditingPost(CreatePost("blog/b.md", "Hello", "About hello"));
        Navigation.NavigateTo($"{EditPage}?Path=blog/b.md");

        var cut = Render<EditContent>();

        cut.Find("h1").TextContent.ShouldBe("Edit: Hello");
        cut.Find("p.text-muted").TextContent.ShouldBe("About hello");
    }

    [Fact]
    public void EditContent_EditingPostWithoutMetadata_ShowsFallbackSubtitle()
    {
        State.SetEditingPost(new ContentItem { Path = "blog/b.md", Metadata = null });
        Navigation.NavigateTo($"{EditPage}?Path=blog/b.md");

        var cut = Render<EditContent>();

        cut.Find("h1").TextContent.ShouldBe("Edit: ");
        cut.Find("p.text-muted").TextContent.ShouldBe("Edit content");
    }

    [Fact]
    public void EditContent_WhileLoading_ShowsSpinner()
    {
        var pending = new TaskCompletionSource<ContentItem>();
        _gitHubService.GetBlogPostAsync("blog/a.md").Returns(pending.Task);
        Navigation.NavigateTo($"{EditPage}?Path=blog/a.md");

        var cut = Render<EditContent>();

        cut.Markup.ShouldContain("Loading content...");
        pending.SetResult(CreatePost("blog/a.md"));
        cut.WaitForAssertion(() => cut.Markup.ShouldNotContain("Loading content..."));
    }

    [Fact]
    public void EditContent_CreateNewPost_StartsNewPostInCurrentFolder()
    {
        State.SetCurrentPath("blog", []);
        var cut = RenderWithoutPath();

        cut.Find(".card-body button.btn-primary").Click();

        State.IsCreatingNewFile.ShouldBeTrue();
        State.EditingPost.ShouldNotBeNull().Path.ShouldBe("blog/new-post.md");
        Services.GetRequiredService<IEventPublisher>().Received(1).Publish(Arg.Is<CreateNewContentEvent>(e => e.Directory == "blog"));
        cut.Find("h1").TextContent.ShouldBe("New Post");
    }

    [Fact]
    public void EditContent_CreateNewPostAtRoot_UsesRootFileName()
    {
        var cut = RenderWithoutPath();

        cut.Find(".card-body button.btn-primary").Click();

        State.EditingPost.ShouldNotBeNull().Path.ShouldBe("new-post.md");
    }

    [Theory]
    [InlineData(".col-auto button")]
    [InlineData(".card-body button.btn-outline-secondary")]
    public void EditContent_BackAndBrowseButtons_GoToContentBrowser(string selector)
    {
        var cut = RenderWithoutPath();

        cut.Find(selector).Click();

        CurrentRelativeUri.ShouldBe("osirion/content");
    }

    [Fact]
    public async Task EditContent_EditorSaveComplete_GoesToContentBrowser()
    {
        State.SetEditingPost(CreatePost("blog/new.md"), isNew: true);
        var cut = RenderWithoutPath();
        var onSaveComplete = cut.FindComponent<Stub<ContentEditor>>().Instance.Parameters.Get(p => p.OnSaveComplete);

        await cut.InvokeAsync(() => onSaveComplete.InvokeAsync(CreatePost("blog/new.md")));

        CurrentRelativeUri.ShouldBe("osirion/content");
    }

    [Fact]
    public async Task EditContent_EditorDiscard_ClearsEditingAndGoesToContentBrowser()
    {
        State.SetEditingPost(CreatePost("blog/new.md"), isNew: true);
        var cut = RenderWithoutPath();
        var onDiscard = cut.FindComponent<Stub<ContentEditor>>().Instance.Parameters.Get(p => p.OnDiscard);

        await cut.InvokeAsync(() => onDiscard.InvokeAsync());

        State.EditingPost.ShouldBeNull();
        State.IsEditing.ShouldBeFalse();
        CurrentRelativeUri.ShouldBe("osirion/content");
    }

    [Fact]
    public async Task EditContent_Disposed_StopsFollowingState()
    {
        RenderWithoutPath();

        await DisposeComponentsAsync();

        // A handler still attached would call StateHasChanged off the renderer's dispatcher and throw.
        Should.NotThrow(() => State.SetEditingPost(CreatePost("blog/new.md"), isNew: true));
    }
}
