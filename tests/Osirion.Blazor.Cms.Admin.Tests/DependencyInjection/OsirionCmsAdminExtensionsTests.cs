using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Services;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.ViewModels;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Domain.Entities;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.DependencyInjection;

// Each scope stands for one admin's circuit. Before the fix the event buses were singletons, so an event raised in
// one circuit ran the handlers of every other circuit.
public class OsirionCmsAdminExtensionsTests
{
    [Fact]
    public void AddOsirionCmsAdminDI_NewContentEventFromAnotherScope_KeepsDraftOfEditorInThisScope()
    {
        using var provider = CreateProvider();
        using var otherAdminScope = provider.CreateScope();
        using var editingAdminScope = provider.CreateScope();
        var draft = new ContentItem { Path = "blog/draft.md", Content = "Unsaved edits" };
        var editor = editingAdminScope.ServiceProvider.GetRequiredService<ContentEditorViewModel>();
        editor.InitializeFromState(draft, isCreatingNew: false);

        otherAdminScope.ServiceProvider.GetRequiredService<IEventPublisher>()
            .Publish(new CreateNewContentEvent("blog"));

        editor.EditingPost.ShouldBeSameAs(draft);
        editor.IsCreatingNew.ShouldBeFalse();
    }

    [Fact]
    public void AddOsirionCmsAdminDI_EventPublishedInSameScope_ReachesSubscriber()
    {
        using var provider = CreateProvider();
        using var scope = provider.CreateScope();
        string? selectedPath = null;
        scope.ServiceProvider.GetRequiredService<IEventSubscriber>()
            .Subscribe<ContentSelectedEvent>(selected => selectedPath = selected.Path);

        scope.ServiceProvider.GetRequiredService<IEventPublisher>()
            .Publish(new ContentSelectedEvent("blog/post.md"));

        selectedPath.ShouldBe("blog/post.md");
    }

    [Fact]
    public void AddOsirionCmsAdminDI_MediatorEventFromAnotherScope_DoesNotReachSubscriberInThisScope()
    {
        using var provider = CreateProvider();
        using var otherAdminScope = provider.CreateScope();
        using var thisAdminScope = provider.CreateScope();
        var received = false;
        thisAdminScope.ServiceProvider.GetRequiredService<CmsEventMediator>()
            .Subscribe<AuthenticationChangedEvent>(_ => received = true);

        otherAdminScope.ServiceProvider.GetRequiredService<CmsEventMediator>()
            .Publish(new AuthenticationChangedEvent(false));

        received.ShouldBeFalse();
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOsirionCmsAdminDI();

        var editorService = Substitute.For<IContentEditorService>();
        editorService.CreateNewBlogPost(Arg.Any<string>())
            .Returns(call => new ContentItem { Path = $"{call.Arg<string>()}/new-post.md" });
        editorService.GenerateFileNameFromTitle(Arg.Any<string>()).Returns("new-post.md");
        services.AddSingleton(editorService);

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }
}
