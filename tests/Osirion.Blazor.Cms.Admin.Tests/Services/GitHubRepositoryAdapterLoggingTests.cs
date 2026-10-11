using NSubstitute;
using Osirion.Blazor.Cms.Admin.Services.Adapters;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Models.GitHub;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Services;

public class GitHubRepositoryAdapterLoggingTests
{
    [Fact]
    public async Task GetContentsAsync_LogsThePathAsDetailsAndTheMethodNameAsTheMethod()
    {
        // The details argument used to land in the [CallerMemberName] parameter, so paths were logged as method names.
        var service = Substitute.For<IGitHubAdminService>();
        service.GetRepositoryContentsAsync("blog/post.md").Returns(new List<GitHubItem>());
        var logger = new RecordingLogger<GitHubRepositoryAdapter>();
        var adapter = new GitHubRepositoryAdapter(service, logger);

        await adapter.GetContentsAsync("blog/post.md");

        logger.Entries.ShouldContain(entry => entry.Message == "GitHub repository adapter: GetContentsAsync blog/post.md");
    }
}
