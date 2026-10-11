using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Features.ContentBrowser.Services;
using Osirion.Blazor.Cms.Admin.Infrastructure.Adapters;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Features.ContentBrowser;

public class ContentBrowserServiceTreeTests
{
    [Fact]
    public async Task GetDirectoryTreeAsync_SubdirectoryFails_LogsPublishesAndRethrows()
    {
        var adapter = Substitute.For<IContentRepositoryAdapter>();
        var mediator = new CmsEventMediator(NullLogger<CmsEventMediator>.Instance);
        var errors = EventRecorder.Record<ErrorOccurredEvent>(mediator);
        var logger = new RecordingLogger<ContentBrowserService>();
        var service = new ContentBrowserService(adapter, mediator, logger);
        var failure = new InvalidOperationException("offline");
        adapter.GetContentsAsync("content").Returns([new GitHubItem { Name = "blog", Type = "dir", Path = "content/blog" }]);
        adapter.GetContentsAsync("content/blog").ThrowsAsync(failure);

        var thrown = await Should.ThrowAsync<InvalidOperationException>(() => service.GetDirectoryTreeAsync("content"));

        thrown.ShouldBeSameAs(failure);
        logger.HasEntry(LogLevel.Error, "Error fetching directory tree", "content").ShouldBeTrue();
        var error = errors.ShouldHaveSingleItem();
        error.Message.ShouldBe("Failed to load directory tree for content");
        error.Exception.ShouldBeSameAs(failure);
    }
}
