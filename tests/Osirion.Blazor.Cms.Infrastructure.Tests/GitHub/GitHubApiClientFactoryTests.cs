using Microsoft.Extensions.Configuration;
using NSubstitute;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

public class GitHubApiClientFactoryTests
{
    private readonly RecordingHttpMessageHandler _handler = new();

    [Fact]
    public void GetClient_CalledTwice_ReturnsDistinctInstances()
    {
        var factory = GitHubTestServices.CreateFactory(_handler);

        factory.GetClient(GitHubTestServices.BlogProvider)
            .ShouldNotBeSameAs(factory.GetClient(GitHubTestServices.BlogProvider));
    }

    [Fact]
    public void GetDefaultClient_CalledTwice_ReturnsDistinctInstances()
    {
        var factory = GitHubTestServices.CreateFactory(_handler);

        factory.GetDefaultClient().ShouldNotBeSameAs(factory.GetDefaultClient());
    }

    [Fact]
    public async Task GetClient_SeedsClientWithProviderSettings()
    {
        var factory = GitHubTestServices.CreateFactory(_handler);

        await factory.GetClient(GitHubTestServices.BlogProvider).GetFileContentAsync("a.md");

        var request = _handler.Requests.ShouldHaveSingleItem();
        request.Uri.AbsoluteUri.ShouldBe("https://api.github.com/repos/o/r/contents/a.md?ref=main");
        request.AuthorizationParameter.ShouldBe(GitHubTestServices.ServerToken);
    }

    [Fact]
    public async Task MutatingOneClient_DoesNotChangeRequestsOfAnotherClientForTheSameProvider()
    {
        // Proves an admin session that changes token, repository and branch on its client can no longer
        // redirect the public site's reads or attach a user's token to them.
        var factory = GitHubTestServices.CreateFactory(_handler);
        var publicClient = factory.GetClient(GitHubTestServices.BlogProvider);
        var adminClient = factory.GetDefaultClient();

        adminClient.SetAccessToken("user-token");
        adminClient.SetRepository("evil", "x");
        adminClient.SetBranch("feature");
        await publicClient.GetFileContentAsync("a.md");

        var request = _handler.Requests.ShouldHaveSingleItem();
        request.Uri.AbsoluteUri.ShouldBe("https://api.github.com/repos/o/r/contents/a.md?ref=main");
        request.AuthorizationParameter.ShouldBe(GitHubTestServices.ServerToken);
    }

    [Fact]
    public async Task Client_CreatesShortLivedHttpClientForEveryOperation()
    {
        // Proves no HttpClient is captured for the life of the process.
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(Arg.Any<string>())
            .Returns(_ => new HttpClient(_handler, disposeHandler: false));
        var client = GitHubTestServices.CreateFactory(httpClientFactory).GetClient(GitHubTestServices.BlogProvider);

        await client.GetFileContentAsync("a.md");
        await client.GetFileContentAsync("b.md");

        httpClientFactory.Received(2).CreateClient("GitHub_Blog");
    }

    [Fact]
    public void GetClient_WithUnknownProvider_ThrowsInvalidOperationException()
    {
        var factory = GitHubTestServices.CreateFactory(_handler);

        Should.Throw<InvalidOperationException>(() => factory.GetClient("Unknown"));
    }

    [Theory]
    [InlineData("Owner")]
    [InlineData("Repository")]
    public void GetClient_WithProviderMissingRequiredSetting_ThrowsInvalidOperationException(string missingSetting)
    {
        var settings = new Dictionary<string, string?>
        {
            ["Osirion:Cms:Web:GitHub:Blog:Owner"] = "o",
            ["Osirion:Cms:Web:GitHub:Blog:Repository"] = "r"
        };
        settings.Remove($"Osirion:Cms:Web:GitHub:Blog:{missingSetting}");
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var factory = GitHubTestServices.CreateFactory(_handler, configuration);

        Should.Throw<InvalidOperationException>(() => factory.GetClient(GitHubTestServices.BlogProvider));
    }

    [Fact]
    public void GetDefaultClient_WithoutProviders_ThrowsInvalidOperationException()
    {
        var factory = GitHubTestServices.CreateFactory(_handler, new ConfigurationBuilder().Build());

        Should.Throw<InvalidOperationException>(() => factory.GetDefaultClient());
    }
}
