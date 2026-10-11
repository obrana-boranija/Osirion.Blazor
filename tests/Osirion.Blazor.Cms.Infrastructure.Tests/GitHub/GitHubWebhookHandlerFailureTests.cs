using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

/// <summary>
/// Covers how <see cref="GitHubWebhookHandler"/> answers when a verified delivery fails while it is dispatched, and
/// the size limit of <see cref="GitHubWebhookHandler.ProcessWebhookAsync"/>.
/// </summary>
public class GitHubWebhookHandlerFailureTests
{
    private const string Secret = "osirion-webhook-secret";
    private const string Owner = "owner";
    private const string Repository = "content";
    private const string HeadSha = "6113728f27ae82c7b1a177c8d03f9e96e0adf246";
    private const string ProviderId = "github-blog";

    private readonly IGitHubApiClientFactory _apiClientFactory = Substitute.For<IGitHubApiClientFactory>();
    private readonly IContentProviderManager _providerManager = Substitute.For<IContentProviderManager>();
    private readonly IContentCacheUpdater _cacheUpdater;
    private readonly GitHubWebhookHandler _handler;

    public GitHubWebhookHandlerFailureTests()
    {
        var options = new GitHubProviderOptions
        {
            Name = "Blog",
            Owner = Owner,
            Repository = Repository,
            Branch = "main",
            WebhookSecret = Secret,
            ProviderId = ProviderId
        };
        _apiClientFactory.GetProviderNames().Returns(["Blog"]);
        _apiClientFactory.GetProviderOptions("Blog").Returns(options);

        var provider = Substitute.For<IContentProvider, IContentCacheUpdater>();
        _cacheUpdater = (IContentCacheUpdater)provider;
        _cacheUpdater.ProviderId.Returns(ProviderId);
        _providerManager.GetAllProviders().Returns([provider]);

        _handler = new GitHubWebhookHandler(_providerManager, _apiClientFactory, NullLogger<GitHubWebhookHandler>.Instance);
    }

    private static byte[] PushPayload()
    {
        return JsonSerializer.SerializeToUtf8Bytes(new
        {
            @ref = "refs/heads/main",
            after = HeadSha,
            deleted = false,
            repository = new { name = Repository, owner = new { login = Owner, name = Owner } }
        });
    }

    /// <summary>A valid push payload padded with an extra field so it is exactly <paramref name="totalBytes"/> long.</summary>
    private static string PaddedPushPayload(int totalBytes)
    {
        string Serialize(string padding) => JsonSerializer.Serialize(new
        {
            @ref = "refs/heads/main",
            after = HeadSha,
            deleted = false,
            repository = new { name = Repository, owner = new { login = Owner, name = Owner } },
            padding
        });

        var payload = Serialize(new string('a', totalBytes - Encoding.UTF8.GetByteCount(Serialize(string.Empty))));
        Encoding.UTF8.GetByteCount(payload).ShouldBe(totalBytes);
        return payload;
    }

    private static string Sign(byte[] payload)
    {
        return "sha256=" + Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(Secret), payload));
    }

    private static HttpRequest CreatePushRequest(byte[] body)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Headers["X-GitHub-Event"] = "push";
        context.Request.Headers["X-Hub-Signature-256"] = Sign(body);
        context.Request.Body = new MemoryStream(body);
        context.Request.ContentLength = body.Length;
        return context.Request;
    }

    [Fact]
    public void Constructor_WithNullDependencies_Throws()
    {
        var logger = NullLogger<GitHubWebhookHandler>.Instance;

        Should.Throw<ArgumentNullException>(() => new GitHubWebhookHandler(null!, _apiClientFactory, logger));
        Should.Throw<ArgumentNullException>(() => new GitHubWebhookHandler(_providerManager, null!, logger));
        Should.Throw<ArgumentNullException>(() => new GitHubWebhookHandler(_providerManager, _apiClientFactory, null!));
    }

    [Fact]
    public async Task HandleWebhookAsync_WhenTheCacheRefreshFails_ReturnsFalse()
    {
        _cacheUpdater.UpdateCacheAsync(Arg.Any<string>(), Arg.Any<bool>()).ThrowsAsync(new InvalidOperationException("refresh failed"));

        var accepted = await _handler.HandleWebhookAsync(CreatePushRequest(PushPayload()));

        accepted.ShouldBeFalse();
        await _cacheUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
    }

    [Fact]
    public async Task HandleWebhookAsync_WhenTheRefreshIsCancelledWithoutTheRequestBeingAborted_ReturnsFalse()
    {
        _cacheUpdater.UpdateCacheAsync(Arg.Any<string>(), Arg.Any<bool>()).ThrowsAsync(new OperationCanceledException());

        var accepted = await _handler.HandleWebhookAsync(CreatePushRequest(PushPayload()));

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task ProcessWebhookAsync_WhenTheCacheRefreshFails_ReturnsFalse()
    {
        _cacheUpdater.UpdateCacheAsync(Arg.Any<string>(), Arg.Any<bool>()).ThrowsAsync(new InvalidOperationException("refresh failed"));
        var payload = PushPayload();

        var accepted = await _handler.ProcessWebhookAsync("push", Sign(payload), Encoding.UTF8.GetString(payload));

        accepted.ShouldBeFalse();
        await _cacheUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
    }

    [Fact]
    public async Task ProcessWebhookAsync_WithASignedPushPayloadOverTheLimit_RejectsItBeforeParsing()
    {
        var logger = new RecordingLogger<GitHubWebhookHandler>();
        var handler = new GitHubWebhookHandler(_providerManager, _apiClientFactory, logger);
        var payload = PaddedPushPayload(GitHubWebhookHandler.MaxPayloadBytes + 1);

        var accepted = await handler.ProcessWebhookAsync("push", Sign(Encoding.UTF8.GetBytes(payload)), payload);

        accepted.ShouldBeFalse();
        logger.Contains(LogLevel.Warning, "the body exceeds").ShouldBeTrue();
        logger.Contains(LogLevel.Information, "Received GitHub webhook").ShouldBeFalse();
        await _cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task ProcessWebhookAsync_WithASignedPushPayloadAtTheLimit_RefreshesTheCache()
    {
        var payload = PaddedPushPayload(GitHubWebhookHandler.MaxPayloadBytes);

        var accepted = await _handler.ProcessWebhookAsync("push", Sign(Encoding.UTF8.GetBytes(payload)), payload);

        accepted.ShouldBeTrue();
        await _cacheUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
    }
}
