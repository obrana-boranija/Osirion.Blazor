using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.Shared;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Osirion.Blazor.Cms.Domain.ValueObjects;
using Shouldly;
using ContentEditorComponent = Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components.ContentEditor;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentEditor.Components;

public class ContentEditorStateTests : AdminComponentTestContext
{
    private readonly IContentEditorService _editorService = Substitute.For<IContentEditorService>();
    private readonly ContentEditorViewModel _viewModel;
    private Action<CreateNewContentEvent>? _createNewHandler;

    public ContentEditorStateTests()
    {
        _viewModel = new ContentEditorViewModel(
            _editorService,
            Substitute.For<IEventPublisher>(),
            Substitute.For<IEventSubscriber>());
        _editorService.GenerateFileNameFromTitle(Arg.Any<string>()).Returns("generated.md");

        Services.AddSingleton(_viewModel);
        ComponentFactories.AddStub<MarkdownEditorWithPreview>();
        ComponentFactories.AddStub<MetadataEditor>();
        RenderInteractively();

        Services.GetRequiredService<IEventSubscriber>()
            .When(subscriber => subscriber.Subscribe(Arg.Any<Action<CreateNewContentEvent>>()))
            .Do(call => _createNewHandler = call.Arg<Action<CreateNewContentEvent>>());
    }

    private CmsState State => Services.GetRequiredService<CmsState>();

    private static ContentItem CreatePost(string path) => new()
    {
        Path = path,
        Content = "Body",
        Metadata = FrontMatter.Create("Title", "Description", new DateTime(2024, 1, 15))
    };

    [Fact]
    public void ContentEditor_StatePostAndEmptyViewModel_InitializesFromState()
    {
        var post = CreatePost("blog/a.md");
        State.SetEditingPost(post, isNew: true);

        var cut = Render<ContentEditorComponent>();

        _viewModel.EditingPost.ShouldBeSameAs(post);
        _viewModel.IsCreatingNew.ShouldBeTrue();
        cut.Find("input[type='text']").GetAttribute("value").ShouldBe("generated.md");
    }

    [Fact]
    public void ContentEditor_StatePostReplacedAfterRender_FollowsNewPost()
    {
        _viewModel.InitializeFromState(CreatePost("blog/a.md"), false);
        var cut = Render<ContentEditorComponent>();
        var replacement = CreatePost("blog/b.md");

        State.SetEditingPost(replacement);
        cut.Render();

        _viewModel.EditingPost.ShouldBeSameAs(replacement);
        _viewModel.CommitMessage.ShouldBe("Update b.md");
    }

    [Fact]
    public void ContentEditor_NewFileInStateAfterRender_IsNotFollowedOnRender()
    {
        var original = CreatePost("blog/a.md");
        _viewModel.InitializeFromState(original, false);
        var cut = Render<ContentEditorComponent>();

        State.SetEditingPost(CreatePost("blog/new.md"), isNew: true);
        cut.Render();

        _viewModel.EditingPost.ShouldBeSameAs(original);
    }

    [Fact]
    public async Task ContentEditor_CreateNewEventWithNewFileInState_InitializesNewPost()
    {
        var cut = Render<ContentEditorComponent>();
        var created = CreatePost("blog/new.md");
        await cut.InvokeAsync(() => State.SetEditingPost(created, isNew: true));

        var handler = _createNewHandler.ShouldNotBeNull();

        await cut.InvokeAsync(() => handler(new CreateNewContentEvent("blog")));

        _viewModel.EditingPost.ShouldBeSameAs(created);
        _viewModel.IsCreatingNew.ShouldBeTrue();
    }

    [Fact]
    public async Task ContentEditor_CreateNewEventWithoutNewFile_LeavesViewModel()
    {
        var cut = Render<ContentEditorComponent>();
        await cut.InvokeAsync(() =>
        {
            State.SetEditingPost(CreatePost("blog/a.md"));
            _viewModel.DiscardChanges();
        });

        var handler = _createNewHandler.ShouldNotBeNull();

        await cut.InvokeAsync(() => handler(new CreateNewContentEvent("blog")));

        _viewModel.EditingPost.ShouldBeNull();
    }

    [Fact]
    public async Task ContentEditor_Disposed_UnsubscribesFromViewModelAndEvents()
    {
        Render<ContentEditorComponent>();

        await DisposeComponentsAsync();

        // A handler still attached would call StateHasChanged off the renderer's dispatcher and throw.
        Should.NotThrow(() => _viewModel.InitializeFromState(CreatePost("blog/a.md"), false));
        Services.GetRequiredService<IEventSubscriber>().Received(1).Unsubscribe(Arg.Any<Action<CreateNewContentEvent>>());
    }

    [Fact]
    public async Task ContentEditor_ContentChanged_UpdatesPostAndShowsUnsavedMarker()
    {
        _viewModel.InitializeFromState(CreatePost("blog/a.md"), false);
        var cut = Render<ContentEditorComponent>();
        cut.Markup.ShouldNotContain("Changes not saved");
        var contentChanged = cut.FindComponent<Stub<MarkdownEditorWithPreview>>().Instance.Parameters.Get(p => p.ContentChanged);

        await cut.InvokeAsync(() => contentChanged.InvokeAsync("Edited"));

        _viewModel.EditingPost!.Content.ShouldBe("Edited");
        cut.Markup.ShouldContain("Changes not saved");
    }

    [Fact]
    public async Task ContentEditor_MetadataChanged_UpdatesPostAndShowsUnsavedMarker()
    {
        _viewModel.InitializeFromState(CreatePost("blog/a.md"), false);
        var cut = Render<ContentEditorComponent>();
        cut.FindAll("button.nav-link")[1].Click();
        var metadataChanged = cut.FindComponent<Stub<MetadataEditor>>().Instance.Parameters.Get(p => p.MetadataChanged);
        var metadata = FrontMatter.Create("Renamed");

        await cut.InvokeAsync(() => metadataChanged.InvokeAsync(metadata));

        _viewModel.EditingPost!.Metadata.ShouldBeSameAs(metadata);
        cut.Markup.ShouldContain("Changes not saved");
    }

    [Fact]
    public async Task ContentEditor_SaveCompletes_RaisesSaveCompleteAndClearsMarker()
    {
        var post = CreatePost("blog/a.md");
        _editorService.SaveBlogPostAsync(post, Arg.Any<string>())
            .Returns(new GitHubFileCommitResponse { Content = new GitHubFileContent { Path = post.Path, Sha = "sha" } });
        _viewModel.InitializeFromState(post, false);
        ContentItem? saved = null;
        var cut = Render<ContentEditorComponent>(parameters => parameters
            .Add(p => p.OnSaveComplete, (ContentItem item) => saved = item));
        var contentChanged = cut.FindComponent<Stub<MarkdownEditorWithPreview>>().Instance.Parameters.Get(p => p.ContentChanged);
        await cut.InvokeAsync(() => contentChanged.InvokeAsync("Edited"));

        cut.Find("button.btn-primary").Click();

        saved.ShouldBeSameAs(post);
        cut.Markup.ShouldNotContain("Changes not saved");
    }

    [Fact]
    public void ContentEditor_Discard_RaisesDiscard()
    {
        _viewModel.InitializeFromState(CreatePost("blog/a.md"), false);
        var discarded = false;
        var cut = Render<ContentEditorComponent>(parameters => parameters.Add(p => p.OnDiscard, () => discarded = true));

        cut.Find("button.btn-outline-secondary").Click();

        discarded.ShouldBeTrue();
    }

    [Fact]
    public void ContentEditor_PreviewVisibility_IsPassedToEditor()
    {
        _viewModel.InitializeFromState(CreatePost("blog/a.md"), false);

        var cut = Render<ContentEditorComponent>(parameters => parameters.Add(p => p.IsPreviewVisible, false));

        cut.FindComponent<Stub<MarkdownEditorWithPreview>>().Instance.Parameters.Get(p => p.ShowPreview).ShouldBeFalse();
    }

    [Fact]
    public void ContentEditor_NewPostWithBlankFileName_DisablesSave()
    {
        _viewModel.InitializeFromState(CreatePost("blog/a.md"), true);
        var cut = Render<ContentEditorComponent>();

        cut.Find("input[type='text']").Input(" ");

        cut.Find("button.btn-primary").HasAttribute("disabled").ShouldBeTrue();
        _viewModel.FileName.ShouldBe(" ");
    }
}
