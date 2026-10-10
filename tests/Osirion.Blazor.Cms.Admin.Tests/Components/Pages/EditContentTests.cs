using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Osirion.Blazor.Cms.Admin.Components.Pages;
using Osirion.Blazor.Cms.Admin.Core.State;
using Osirion.Blazor.Cms.Admin.Features.ContentEditor.Components;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Entities;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Components.Pages;

public class EditContentTests : AdminComponentTestContext
{
    private readonly IGitHubAdminService _gitHubService = Substitute.For<IGitHubAdminService>();
    private readonly RecordingLogger<EditContent> _logger = new();

    public EditContentTests()
    {
        Services.AddSingleton(_gitHubService);
        Services.AddSingleton<ILogger<EditContent>>(_logger);
        ComponentFactories.AddStub<ContentEditor>();
        RenderInteractively();
    }

    private IRenderedComponent<EditContent> RenderWithPath(string path)
    {
        Services.GetRequiredService<BunitNavigationManager>().NavigateTo($"/osirion/content/edit?Path={Uri.EscapeDataString(path)}");
        return Render<EditContent>();
    }

    [Fact]
    public void EditContent_WithPath_FetchesTheFileExactlyOnce()
    {
        _gitHubService.GetBlogPostAsync("blog/a.md").Returns(new ContentItem { Path = "blog/a.md", Content = "x" });

        var cut = RenderWithPath("blog/a.md");

        cut.WaitForState(() => Services.GetRequiredService<CmsState>().EditingPost is not null);
        cut.Render();
        _gitHubService.Received(1).GetBlogPostAsync("blog/a.md");
    }

    [Fact]
    public void EditContent_FileAlreadyInSharedState_DoesNotFetchIt()
    {
        var post = new ContentItem { Path = "blog/a.md", Content = "x" };
        Services.GetRequiredService<CmsState>().SetEditingPost(post);

        RenderWithPath("blog/a.md");

        _gitHubService.DidNotReceiveWithAnyArgs().GetBlogPostAsync(default!);
    }

    [Fact]
    public void EditContent_LoadFails_ShowsGenericMessageAndLogsTheException()
    {
        _gitHubService.GetBlogPostAsync("blog/a.md")
            .Returns(Task.FromException<ContentItem>(new InvalidOperationException("token ghp_secret rejected")));

        RenderWithPath("blog/a.md");

        var state = Services.GetRequiredService<CmsState>();
        state.ErrorMessage.ShouldNotBeNullOrWhiteSpace();
        state.ErrorMessage.ShouldNotContain("ghp_secret");
        _logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception != null
            && entry.Exception.Message.Contains("ghp_secret", StringComparison.Ordinal));
    }
}
