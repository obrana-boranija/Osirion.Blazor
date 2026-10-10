using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Infrastructure.GitHub;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// Configuration and factories shared by the GitHub client isolation tests.
/// </summary>
internal static class GitHubTestServices
{
    public const string BlogProvider = "Blog";
    public const string DocsProvider = "Docs";
    public const string ServerToken = "server-token";
    public const string DocsToken = "docs-token";

    /// <summary>
    /// Two public providers: "Blog" (o/r, default) and "Docs" (o2/r2), each with its own server token.
    /// </summary>
    public static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Osirion:Cms:Web:GitHub:Blog:Owner"] = "o",
                ["Osirion:Cms:Web:GitHub:Blog:Repository"] = "r",
                ["Osirion:Cms:Web:GitHub:Blog:Branch"] = "main",
                ["Osirion:Cms:Web:GitHub:Blog:ApiToken"] = ServerToken,
                ["Osirion:Cms:Web:GitHub:Blog:ContentPath"] = "content",
                ["Osirion:Cms:Web:GitHub:Blog:IsDefault"] = "true",
                ["Osirion:Cms:Web:GitHub:Docs:Owner"] = "o2",
                ["Osirion:Cms:Web:GitHub:Docs:Repository"] = "r2",
                ["Osirion:Cms:Web:GitHub:Docs:Branch"] = "main",
                ["Osirion:Cms:Web:GitHub:Docs:ApiToken"] = DocsToken
            })
            .Build();
    }

    /// <summary>
    /// Creates a factory whose clients send every request to <paramref name="handler"/>.
    /// </summary>
    public static GitHubApiClientFactory CreateFactory(HttpMessageHandler handler, IConfiguration? configuration = null)
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(Arg.Any<string>())
            .Returns(_ => new HttpClient(handler, disposeHandler: false));

        return CreateFactory(httpClientFactory, configuration);
    }

    /// <summary>
    /// Creates a factory that takes its HTTP clients from <paramref name="httpClientFactory"/>.
    /// </summary>
    public static GitHubApiClientFactory CreateFactory(IHttpClientFactory httpClientFactory, IConfiguration? configuration = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(httpClientFactory);

        return new GitHubApiClientFactory(
            services.BuildServiceProvider(),
            configuration ?? CreateConfiguration(),
            NullLogger<GitHubApiClientFactory>.Instance);
    }
}
