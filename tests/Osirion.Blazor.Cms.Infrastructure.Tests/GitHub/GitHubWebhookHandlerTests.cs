using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Domain.Options.Configuration;
using Osirion.Blazor.Cms.Domain.Services;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Shouldly;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.GitHub;

public class GitHubWebhookHandlerTests
{
    // Test vector published at https://docs.github.com/en/webhooks/using-webhooks/validating-webhook-deliveries
    private const string GitHubDocsSecret = "It's a Secret to Everybody";
    private const string GitHubDocsPayload = "Hello, World!";
    private const string GitHubDocsSignature = "sha256=757107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e17";

    private const string Secret = "osirion-webhook-secret";
    private const string Owner = "obrana-boranija";
    private const string Repository = "osirion-content";
    private const string HeadSha = "6113728f27ae82c7b1a177c8d03f9e96e0adf246";

    private readonly IGitHubApiClientFactory _apiClientFactory = Substitute.For<IGitHubApiClientFactory>();
    private readonly IContentProviderManager _providerManager = Substitute.For<IContentProviderManager>();
    private readonly Dictionary<string, GitHubProviderOptions> _providerOptions = new();
    private readonly List<IContentProvider> _contentProviders = new();
    private readonly GitHubWebhookHandler _handler;

    public GitHubWebhookHandlerTests()
    {
        _apiClientFactory.GetProviderNames().Returns(_ => _providerOptions.Keys.ToList());
        _apiClientFactory.GetProviderOptions(Arg.Any<string>())
            .Returns(call => _providerOptions.GetValueOrDefault(call.Arg<string>()));
        _providerManager.GetAllProviders().Returns(_ => _contentProviders.ToList());

        _handler = new GitHubWebhookHandler(_providerManager, _apiClientFactory, NullLogger<GitHubWebhookHandler>.Instance);
    }

    [Fact]
    public async Task HandleWebhookAsync_PingSignedWithGitHubDocsTestVector_ReturnsTrue()
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);
        var request = CreateRequest("ping", Encoding.UTF8.GetBytes(GitHubDocsPayload), GitHubDocsSignature);

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeTrue();
    }

    [Fact]
    public async Task HandleWebhookAsync_GitHubDocsSignatureWithOneDigitChanged_ReturnsFalse()
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);
        var tamperedSignature = GitHubDocsSignature[..^1] + "8";
        var request = CreateRequest("ping", Encoding.UTF8.GetBytes(GitHubDocsPayload), tamperedSignature);

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleWebhookAsync_GitHubDocsSignatureOverDifferentBody_ReturnsFalse()
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);
        var request = CreateRequest("ping", Encoding.UTF8.GetBytes("Hello, World?"), GitHubDocsSignature);

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleWebhookAsync_UnsignedPush_ReturnsFalseAndDoesNotRefresh()
    {
        AddProvider("Blog", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var request = CreateRequest("push", PushPayload("refs/heads/main"), signature: null);

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
        await cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleWebhookAsync_NoProviderHasWebhookSecret_ReturnsFalseAndDoesNotRefresh(string? webhookSecret)
    {
        AddProvider("Blog", webhookSecret: webhookSecret, providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/main");
        var request = CreateRequest("push", payload, Sign("any-secret", payload));

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
        await cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sha256=")]
    [InlineData("757107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e17")]
    [InlineData("SHA256=757107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e17")]
    [InlineData("sha1=01dc10d0c83e72ed246219cdd91669667fe2ca59")]
    [InlineData("sha256=757107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e1")]
    [InlineData("sha256=757107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e170")]
    [InlineData("sha256=zz7107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e17")]
    [InlineData(" sha256=757107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e17")]
    public async Task HandleWebhookAsync_MalformedSignatureHeader_ReturnsFalse(string signature)
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);
        var request = CreateRequest("ping", Encoding.UTF8.GetBytes(GitHubDocsPayload), signature);

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleWebhookAsync_UppercaseHexDigest_ReturnsTrue()
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);
        var signature = "sha256=" + GitHubDocsSignature["sha256=".Length..].ToUpperInvariant();
        var request = CreateRequest("ping", Encoding.UTF8.GetBytes(GitHubDocsPayload), signature);

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeTrue();
    }

    [Fact]
    public async Task HandleWebhookAsync_SignedPushToTrackedBranch_RefreshesOnlyTheExactProviderId()
    {
        AddProvider("Blog", providerId: "github-osirion");
        var blogUpdater = AddCacheUpdater("github-osirion");
        var docsUpdater = AddCacheUpdater("github-osirion-docs");
        var payload = PushPayload("refs/heads/main");

        var accepted = await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        accepted.ShouldBeTrue();
        await blogUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
        await docsUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task HandleWebhookAsync_TwoProvidersOnSameRepositoryDifferentBranches_RefreshesTheProviderTrackingThePushedBranch()
    {
        AddProvider("Blog", branch: "main", providerId: "github-blog");
        AddProvider("Docs", branch: "docs", providerId: "github-docs");
        var blogUpdater = AddCacheUpdater("github-blog");
        var docsUpdater = AddCacheUpdater("github-docs");
        var payload = PushPayload("refs/heads/docs");

        await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        await docsUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
        await blogUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task HandleWebhookAsync_ProviderWithoutProviderId_RefreshesTheDerivedProviderId()
    {
        AddProvider("Blog", providerId: null);
        var cacheUpdater = AddCacheUpdater($"github-{Owner}-{Repository}");
        var payload = PushPayload("refs/heads/main");

        await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        await cacheUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
    }

    [Fact]
    public async Task HandleWebhookAsync_TagPushNamedLikeTrackedBranch_AcknowledgesWithoutRefresh()
    {
        AddProvider("Blog", branch: "main", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/tags/main");

        var accepted = await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        accepted.ShouldBeTrue();
        await cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task HandleWebhookAsync_PushToBranchWithSlash_RefreshesTheProviderTrackingIt()
    {
        AddProvider("Blog", branch: "feature/x", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/feature/x");

        await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        await cacheUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
    }

    [Fact]
    public async Task HandleWebhookAsync_BranchCaseDiffers_DoesNotRefresh()
    {
        AddProvider("Blog", branch: "Main", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/main");

        await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        await cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task HandleWebhookAsync_DeletedBranchPush_AcknowledgesWithoutRefresh()
    {
        AddProvider("Blog", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/main", deleted: true);

        var accepted = await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        accepted.ShouldBeTrue();
        await cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task HandleWebhookAsync_PushForRepositorySignedWithAnotherRepositorysSecret_DoesNotRefresh()
    {
        AddProvider("Blog", repository: "blog", webhookSecret: "blog-secret", providerId: "github-blog");
        AddProvider("Docs", repository: "docs", webhookSecret: "docs-secret", providerId: "github-docs");
        var docsUpdater = AddCacheUpdater("github-docs");
        var payload = PushPayload("refs/heads/main", repository: "docs");

        await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign("blog-secret", payload)));

        await docsUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task HandleWebhookAsync_SignedPushForUnconfiguredRepository_AcknowledgesWithoutRefresh()
    {
        AddProvider("Blog", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/main", repository: "unrelated");

        var accepted = await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        accepted.ShouldBeTrue();
        await cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task HandleWebhookAsync_OwnerAndRepositoryCaseDiffer_Refreshes()
    {
        AddProvider("Blog", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/main", owner: Owner.ToUpperInvariant(), repository: Repository.ToUpperInvariant());

        await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        await cacheUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
    }

    [Fact]
    public async Task HandleWebhookAsync_SignedPushWithMultiByteCharacters_Refreshes()
    {
        AddProvider("Blog", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/main", commitMessage: "Ažuriraj članak ✓");

        await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        await cacheUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
    }

    [Fact]
    public async Task HandleWebhookAsync_MultiBytePushSignedOverAnotherEncodingOfTheText_DoesNotRefresh()
    {
        AddProvider("Blog", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/main", commitMessage: "Ažuriraj članak ✓");
        var otherEncoding = Encoding.Latin1.GetBytes(Encoding.UTF8.GetString(payload));

        var accepted = await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, otherEncoding)));

        accepted.ShouldBeFalse();
        await cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Theory]
    [InlineData(new byte[] { 0xEF, 0xBB, 0xBF, (byte)'{', (byte)'}' })]
    [InlineData(new byte[] { (byte)'{', (byte)'"', (byte)'z', (byte)'"', (byte)':', (byte)'"', 0xC3, 0x28, (byte)'"', (byte)'}' })]
    public async Task HandleWebhookAsync_BodyThatATextRoundTripChanges_VerifiesAgainstTheRawBytes(byte[] payload)
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);

        var accepted = await _handler.HandleWebhookAsync(CreateRequest("ping", payload, Sign(GitHubDocsSecret, payload)));

        accepted.ShouldBeTrue();
    }

    [Fact]
    public async Task HandleWebhookAsync_BodyLargerThanLimitWithoutContentLength_ReturnsFalse()
    {
        AddProvider("Blog", providerId: "github-blog");
        var payload = new byte[GitHubWebhookHandler.MaxPayloadBytes + 1];
        var request = CreateRequest("ping", payload, Sign(Secret, payload), contentLength: null);

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleWebhookAsync_BodyLargerThanLimitWithoutContentLength_StopsReadingOneBytePastTheLimit()
    {
        AddProvider("Blog", providerId: "github-blog");
        var body = new CountingStream(GitHubWebhookHandler.MaxPayloadBytes * 2);
        var request = CreateRequest("ping", [], Sign(Secret, []), contentLength: null);
        request.Body = body;

        await _handler.HandleWebhookAsync(request);

        body.BytesRead.ShouldBe(GitHubWebhookHandler.MaxPayloadBytes + 1);
    }

    [Fact]
    public async Task HandleWebhookAsync_DeclaredContentLengthOverLimit_ReturnsFalseWithoutReadingTheBody()
    {
        AddProvider("Blog", providerId: "github-blog");
        var body = new CountingStream(16);
        var request = CreateRequest("ping", [], Sign(Secret, []), contentLength: GitHubWebhookHandler.MaxPayloadBytes + 1L);
        request.Body = body;

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
        body.BytesRead.ShouldBe(0);
    }

    [Fact]
    public async Task HandleWebhookAsync_NoProviderHasWebhookSecret_ReturnsFalseWithoutReadingTheBody()
    {
        AddProvider("Blog", webhookSecret: null, providerId: "github-blog");
        var body = new CountingStream(1024);
        var request = CreateRequest("push", [], Sign(Secret, []), contentLength: null);
        request.Body = body;

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
        body.BytesRead.ShouldBe(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("sha1=01dc10d0c83e72ed246219cdd91669667fe2ca59")]
    public async Task HandleWebhookAsync_MissingOrMalformedSignature_ReturnsFalseWithoutReadingTheBody(string? signature)
    {
        AddProvider("Blog", providerId: "github-blog");
        var body = new CountingStream(1024);
        var request = CreateRequest("push", [], signature, contentLength: null);
        request.Body = body;

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
        body.BytesRead.ShouldBe(0);
    }

    [Fact]
    public async Task HandleWebhookAsync_ServerRefusesTheBodyWithIOException_ReturnsFalseAndLogsAWarningNotAnError()
    {
        AddProvider("Blog", providerId: "github-blog");
        var logger = new CapturingLogger<GitHubWebhookHandler>();
        var handler = new GitHubWebhookHandler(_providerManager, _apiClientFactory, logger);
        var request = CreateRequest("ping", [], Sign(Secret, []), contentLength: null);
        request.Body = new FailingStream(bytesBeforeFailure: 4096);

        var accepted = await handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
        logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Warning && entry.Exception == null);
        logger.Entries.ShouldNotContain(entry => entry.Level >= LogLevel.Error);
    }

    [Fact]
    public async Task HandleWebhookAsync_SignedPushWithMalformedJson_ReturnsFalse()
    {
        AddProvider("Blog", providerId: "github-blog");
        var payload = Encoding.UTF8.GetBytes("{\"ref\": ");

        var accepted = await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleWebhookAsync_SignedPushWithoutRepository_ReturnsFalse()
    {
        AddProvider("Blog", providerId: "github-blog");
        var payload = JsonSerializer.SerializeToUtf8Bytes(new { @ref = "refs/heads/main", after = HeadSha });

        var accepted = await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task HandleWebhookAsync_SignedFormUrlEncodedPush_DoesNotRefresh()
    {
        AddProvider("Blog", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var json = Encoding.UTF8.GetString(PushPayload("refs/heads/main"));
        var payload = Encoding.UTF8.GetBytes("payload=" + Uri.EscapeDataString(json));

        await _handler.HandleWebhookAsync(CreateRequest("push", payload, Sign(Secret, payload)));

        await cacheUpdater.DidNotReceiveWithAnyArgs().UpdateCacheAsync(default, default);
    }

    [Fact]
    public async Task HandleWebhookAsync_ValidDelivery_LeavesTheRequestBodyOpen()
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);
        var request = CreateRequest("ping", Encoding.UTF8.GetBytes(GitHubDocsPayload), GitHubDocsSignature);

        await _handler.HandleWebhookAsync(request);

        request.Body.CanRead.ShouldBeTrue();
    }

    [Fact]
    public async Task HandleWebhookAsync_RequestAborted_ReturnsFalse()
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);
        var request = CreateRequest("ping", Encoding.UTF8.GetBytes(GitHubDocsPayload), GitHubDocsSignature);
        using var aborted = new CancellationTokenSource();
        await aborted.CancelAsync();
        request.HttpContext.RequestAborted = aborted.Token;

        var accepted = await _handler.HandleWebhookAsync(request);

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task ProcessWebhookAsync_UnsignedPayload_ReturnsFalse()
    {
        AddProvider("Docs", webhookSecret: GitHubDocsSecret);

        var accepted = await _handler.ProcessWebhookAsync("ping", string.Empty, GitHubDocsPayload);

        accepted.ShouldBeFalse();
    }

    [Fact]
    public async Task ProcessWebhookAsync_SignedPush_RefreshesTheTrackingProvider()
    {
        AddProvider("Blog", providerId: "github-blog");
        var cacheUpdater = AddCacheUpdater("github-blog");
        var payload = PushPayload("refs/heads/main");

        var accepted = await _handler.ProcessWebhookAsync("push", Sign(Secret, payload), Encoding.UTF8.GetString(payload));

        accepted.ShouldBeTrue();
        await cacheUpdater.Received(1).UpdateCacheAsync(HeadSha, true);
    }

    private void AddProvider(
        string name,
        string repository = Repository,
        string branch = "main",
        string? webhookSecret = Secret,
        string? providerId = null)
    {
        _providerOptions[name] = new GitHubProviderOptions
        {
            Name = name,
            Owner = Owner,
            Repository = repository,
            Branch = branch,
            WebhookSecret = webhookSecret,
            ProviderId = providerId
        };
    }

    private IContentCacheUpdater AddCacheUpdater(string providerId)
    {
        var provider = Substitute.For<IContentProvider, IContentCacheUpdater>();
        provider.ProviderId.Returns(providerId);
        var cacheUpdater = (IContentCacheUpdater)provider;
        cacheUpdater.ProviderId.Returns(providerId);
        _contentProviders.Add(provider);
        return cacheUpdater;
    }

    private static byte[] PushPayload(
        string gitRef,
        string owner = Owner,
        string repository = Repository,
        bool deleted = false,
        string commitMessage = "Update content")
    {
        return JsonSerializer.SerializeToUtf8Bytes(new
        {
            @ref = gitRef,
            after = HeadSha,
            deleted,
            repository = new { name = repository, owner = new { login = owner, name = owner } },
            head_commit = new { id = HeadSha, message = commitMessage }
        }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
    }

    private static string Sign(string secret, byte[] payload)
    {
        return "sha256=" + Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), payload));
    }

    private static HttpRequest CreateRequest(string eventType, byte[] body, string? signature, long? contentLength = -1)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/github/webhook";
        context.Request.ContentType = "application/json";
        context.Request.Headers["X-GitHub-Event"] = eventType;
        context.Request.Headers["X-GitHub-Delivery"] = "72d3162e-cc78-11e3-81ab-4c9367dc0958";
        if (signature is not null)
        {
            context.Request.Headers["X-Hub-Signature-256"] = signature;
        }

        context.Request.Body = new MemoryStream(body);
        context.Request.ContentLength = contentLength == -1 ? body.Length : contentLength;
        return context.Request;
    }

    /// <summary>A read-only stream of zeros that records how many bytes were read from it.</summary>
    private sealed class CountingStream(long length) : Stream
    {
        private long _position;

        public long BytesRead => _position;
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = (int)Math.Min(count, length - _position);
            Array.Clear(buffer, offset, read);
            _position += read;
            return read;
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    /// <summary>
    /// A read-only stream of zeros that throws <see cref="IOException"/> after a number of bytes, as
    /// Kestrel does when a body passes its size limit or breaks chunked framing.
    /// </summary>
    private sealed class FailingStream(int bytesBeforeFailure) : Stream
    {
        private int _position;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_position >= bytesBeforeFailure)
            {
                throw new IOException("Request body too large.");
            }

            var read = Math.Min(count, bytesBeforeFailure - _position);
            Array.Clear(buffer, offset, read);
            _position += read;
            return read;
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    /// <summary>A logger that records the level and exception of every entry.</summary>
    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, Exception? Exception)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, exception));
        }
    }
}
