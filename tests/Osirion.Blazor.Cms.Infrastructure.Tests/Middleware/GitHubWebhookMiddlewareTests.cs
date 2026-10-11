using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Osirion.Blazor.Cms.Domain.Interfaces;
using Osirion.Blazor.Cms.Infrastructure.GitHub;
using Osirion.Blazor.Cms.Infrastructure.Middleware;
using Shouldly;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.Middleware;

public class GitHubWebhookMiddlewareTests
{
    private const string WebhookPath = "/api/github/webhook";

    private readonly IGitHubWebhookHandler _webhookHandler = Substitute.For<IGitHubWebhookHandler>();
    private bool _nextCalled;
    private readonly GitHubWebhookMiddleware _middleware;

    public GitHubWebhookMiddlewareTests()
    {
        _middleware = new GitHubWebhookMiddleware(
            _ =>
            {
                _nextCalled = true;
                return Task.CompletedTask;
            },
            NullLogger<GitHubWebhookMiddleware>.Instance);
    }

    [Fact]
    public async Task InvokeAsync_GetToWebhookPath_CallsNextWithoutHandler()
    {
        var context = CreateContext(HttpMethods.Get, WebhookPath);

        await _middleware.InvokeAsync(context, _webhookHandler);

        _nextCalled.ShouldBeTrue();
        await _webhookHandler.DidNotReceiveWithAnyArgs().HandleWebhookAsync(default!);
    }

    [Fact]
    public async Task InvokeAsync_PostToPathBelowWebhookPath_CallsNextWithoutHandler()
    {
        var context = CreateContext(HttpMethods.Post, WebhookPath + "/extra");

        await _middleware.InvokeAsync(context, _webhookHandler);

        _nextCalled.ShouldBeTrue();
        await _webhookHandler.DidNotReceiveWithAnyArgs().HandleWebhookAsync(default!);
    }

    [Fact]
    public async Task InvokeAsync_PostToWebhookPathInOtherCase_CallsHandler()
    {
        var context = CreateContext(HttpMethods.Post, "/API/GitHub/Webhook");

        await _middleware.InvokeAsync(context, _webhookHandler);

        await _webhookHandler.Received(1).HandleWebhookAsync(context.Request);
    }

    [Fact]
    public async Task InvokeAsync_HandlerAccepts_Writes202WithBody()
    {
        var context = CreateContext(HttpMethods.Post, WebhookPath);
        _webhookHandler.HandleWebhookAsync(Arg.Any<HttpRequest>()).Returns(true);

        await _middleware.InvokeAsync(context, _webhookHandler);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status202Accepted);
        ReadBody(context).ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task InvokeAsync_HandlerRejects_Writes400WithGenericBody()
    {
        var context = CreateContext(HttpMethods.Post, WebhookPath);
        _webhookHandler.HandleWebhookAsync(Arg.Any<HttpRequest>()).Returns(false);

        await _middleware.InvokeAsync(context, _webhookHandler);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        ReadBody(context).ShouldBe("Webhook request rejected");
    }

    [Fact]
    public async Task InvokeAsync_HandlerThrows_Writes500WithoutExceptionDetails()
    {
        var context = CreateContext(HttpMethods.Post, WebhookPath);
        _webhookHandler.HandleWebhookAsync(Arg.Any<HttpRequest>())
            .ThrowsAsync(new InvalidOperationException("secret detail"));

        await _middleware.InvokeAsync(context, _webhookHandler);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        ReadBody(context).ShouldNotContain("secret detail");
    }

    [Fact]
    public async Task InvokeAsync_DeclaredBodyOverLimit_Writes413WithoutCallingHandler()
    {
        var context = CreateContext(HttpMethods.Post, WebhookPath);
        context.Request.ContentLength = GitHubWebhookHandler.MaxPayloadBytes + 1L;

        await _middleware.InvokeAsync(context, _webhookHandler);

        context.Response.StatusCode.ShouldBe(StatusCodes.Status413PayloadTooLarge);
        ReadBody(context).ShouldNotBeNullOrWhiteSpace();
        await _webhookHandler.DidNotReceiveWithAnyArgs().HandleWebhookAsync(default!);
    }

    [Theory]
    [InlineData(30_000_000L)]
    [InlineData(null)]
    public async Task InvokeAsync_WritableBodySizeLimitAboveTheWebhookLimit_IsLoweredWithRoomAboveThePayloadLimit(long? hostLimit)
    {
        var context = CreateContext(HttpMethods.Post, WebhookPath);
        var bodySizeFeature = new MaxRequestBodySizeFeature(hostLimit);
        context.Features.Set<IHttpMaxRequestBodySizeFeature>(bodySizeFeature);

        await _middleware.InvokeAsync(context, _webhookHandler);

        bodySizeFeature.MaxRequestBodySize.ShouldBe(GitHubWebhookMiddleware.MaxRequestBodySize);
        GitHubWebhookMiddleware.MaxRequestBodySize.ShouldBeGreaterThan(GitHubWebhookHandler.MaxPayloadBytes);
    }

    [Fact]
    public async Task InvokeAsync_StricterHostBodySizeLimit_IsKept()
    {
        var context = CreateContext(HttpMethods.Post, WebhookPath);
        var bodySizeFeature = new MaxRequestBodySizeFeature(1_000_000);
        context.Features.Set<IHttpMaxRequestBodySizeFeature>(bodySizeFeature);

        await _middleware.InvokeAsync(context, _webhookHandler);

        bodySizeFeature.MaxRequestBodySize.ShouldBe(1_000_000);
    }

    [Fact]
    public async Task InvokeAsync_ReadOnlyBodySizeLimit_LeavesItUnchangedAndCallsHandler()
    {
        var context = CreateContext(HttpMethods.Post, WebhookPath);
        var bodySizeFeature = new MaxRequestBodySizeFeature(30_000_000, isReadOnly: true);
        context.Features.Set<IHttpMaxRequestBodySizeFeature>(bodySizeFeature);

        await _middleware.InvokeAsync(context, _webhookHandler);

        bodySizeFeature.MaxRequestBodySize.ShouldBe(30_000_000);
        await _webhookHandler.Received(1).HandleWebhookAsync(context.Request);
    }

    private static DefaultHttpContext CreateContext(string method, string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        context.Request.Body = new MemoryStream();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static string ReadBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        return reader.ReadToEnd();
    }

    /// <summary>A body size limit that, like Kestrel's, refuses changes once it is read-only.</summary>
    private sealed class MaxRequestBodySizeFeature(long? initialLimit, bool isReadOnly = false) : IHttpMaxRequestBodySizeFeature
    {
        private long? _maxRequestBodySize = initialLimit;

        public bool IsReadOnly => isReadOnly;

        public long? MaxRequestBodySize
        {
            get => _maxRequestBodySize;
            set
            {
                if (isReadOnly)
                {
                    throw new InvalidOperationException("The maximum request body size cannot be modified after the request body has been read.");
                }

                _maxRequestBodySize = value;
            }
        }
    }
}
